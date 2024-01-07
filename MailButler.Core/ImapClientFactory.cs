using System.Net;
using Extensions.Dictionary;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using MailButler.Core.Exchange;
using MailButler.Dtos;
using MailKit.Net.Imap;
using MailKit.Security;
using Microsoft.Extensions.Logging;

namespace MailButler.Core;

public sealed class ImapClientFactory : IImapClientFactory
{
	private readonly ILoggerFactory _loggerFactory;
	private readonly ILogger<ImapClientFactory> _logger;

	public ImapClientFactory(ILogger<ImapClientFactory> logger, ILoggerFactory loggerFactory)
	{
		_loggerFactory = loggerFactory;
		_logger = logger;
	}

	public async Task<IImapClient> ImapClientAsync(Account account, CancellationToken cancellationToken)
	{
		switch (account.Type)
		{
			case AccountType.Exchange:
				return await ConnectAndAuthenticateExchangeAsync(cancellationToken, account);
			case AccountType.OAuth2:
				return await ConnectAndAuthenticateOAuth2GoogleAsync(cancellationToken, account);
			case AccountType.Imap:
				return await ConnectAndAuthenticateAsync(cancellationToken, account);
			case AccountType.None:
				if (_logger.IsEnabled(LogLevel.Debug))
					_logger.LogWarning(
						"Account type is not set for account {Account}",
						account.ToDictionary()
					);
				throw new InvalidOperationException("None as Type ist not allowed");

			default:
				throw new NotImplementedException();
		}
	}

	private async Task<IImapClient> ConnectAndAuthenticateExchangeAsync(CancellationToken cancellationToken,
		Account account)
	{
		if (string.IsNullOrEmpty(account.Password))
		{
			throw new InvalidOperationException("Password is not set for Exchange account");
		}

		if (string.IsNullOrEmpty(account.FolderUrl))
		{
			throw new InvalidOperationException("FolderUrl is not set for Exchange account");
		}

		if (string.IsNullOrEmpty(account.Username))
		{
			throw new InvalidOperationException("Username is not set for Exchange account");
		}

		var client = new ExchangeImapClient(
			_loggerFactory.CreateLogger<ExchangeImapClient>(),
			account.Username,
			account.Password,
			account.FolderUrl
		);

		await client.AuthenticateAsync(new NetworkCredential(account.Username, account.Password), cancellationToken);

		return client;
	}

	private static async Task<IImapClient> ConnectAndAuthenticateOAuth2GoogleAsync(
		CancellationToken cancellationToken,
		Account account
	)
	{
		ClientSecrets clientSecrets = new()
		{
			ClientId = account.ClientId,
			ClientSecret = account.ClientSecret
		};

		GoogleAuthorizationCodeFlow codeFlow = new(
			new GoogleAuthorizationCodeFlow.Initializer
			{
				DataStore = new FileDataStore("CredentialCacheFolder"),
				Scopes = new[] { "https://mail.google.com/" },
				ClientSecrets = clientSecrets
			}
		);
		// Note: For a web app, you'll want to use AuthorizationCodeWebApp instead.
		var codeReceiver = new LocalServerCodeReceiver();
		var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

		var credential = await authCode.AuthorizeAsync(account.Username, cancellationToken);

		if (credential.Token.IsExpired(SystemClock.Default))
		{
			await credential.RefreshTokenAsync(cancellationToken);
		}

		var oauth2 = new SaslMechanismOAuth2(credential.UserId, credential.Token.AccessToken);

		ImapClient client = new();

		await client.ConnectAsync(account.ImapServer, account.ImapPort, SecureSocketOptions.SslOnConnect,
			cancellationToken);
		await client.AuthenticateAsync(oauth2, cancellationToken);
		return client;
	}

	private static async Task<ImapClient> ConnectAndAuthenticateAsync(CancellationToken cancellationToken,
		Account account)
	{
		ImapClient client = new();
		await client.ConnectAsync(
			account.ImapServer,
			account.ImapPort,
			SecureSocketOptions.SslOnConnect,
			cancellationToken
		);
		await client.AuthenticateAsync(account.Username, account.Password, cancellationToken);
		return client;
	}
}