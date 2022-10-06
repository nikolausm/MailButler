using Extensions.Dictionary;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using MailButler.Dtos;
using MailKit.Net.Imap;
using MailKit.Security;
using Microsoft.Extensions.Logging;

namespace MailButler.Core;

public sealed class ImapClientFactory : IImapClientFactory
{
	private readonly ILogger<ImapClientFactory> _logger;

	public ImapClientFactory(ILogger<ImapClientFactory> logger)
	{
		_logger = logger;
	}

	public async Task<ImapClient> ImapClientAsync(Account account, CancellationToken cancellationToken)
	{
		switch (account.Type)
		{
			case AccountType.OAuth2:
				return await ConnectAndAuthenticateOAuth2Async(cancellationToken, account);
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

	private static async Task<ImapClient> ConnectAndAuthenticateOAuth2Async(
		CancellationToken cancellationToken,
		Account account
	)
	{
		ClientSecrets clientSecrets = new()
		{
			ClientId = account.ClientId,
			ClientSecret = account.ClientSecret
		};

		GoogleAuthorizationCodeFlow codeFlow = new(new GoogleAuthorizationCodeFlow.Initializer
		{
			DataStore = new FileDataStore("CredentialCacheFolder"),
			Scopes = new[] { "https://mail.google.com/" },
			ClientSecrets = clientSecrets
		});

// Note: For a web app, you'll want to use AuthorizationCodeWebApp instead.
		var codeReceiver = new LocalServerCodeReceiver();
		var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

		var credential = await authCode.AuthorizeAsync(account.Username, cancellationToken);

		if (credential.Token.IsExpired(SystemClock.Default))
			await credential.RefreshTokenAsync(cancellationToken);

		var oauth2 = new SaslMechanismOAuth2(credential.UserId, credential.Token.AccessToken);

		ImapClient client = new();

		await client.ConnectAsync("imap.gmail.com", account.ImapPort, SecureSocketOptions.SslOnConnect,
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