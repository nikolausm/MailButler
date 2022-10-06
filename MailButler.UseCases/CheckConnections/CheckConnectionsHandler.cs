using Extensions.Dictionary;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using MailButler.Core;
using MailButler.Dtos;
using MailKit.Net.Imap;
using MailKit.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MailButler.UseCases.CheckConnections;

public sealed class CheckConnectionsHandler : IRequestHandler<CheckConnectionsRequest, CheckConnectionsResponse>
{
	private readonly ILogger<CheckConnectionsHandler> _logger;
	private readonly IImapClientFactory _imapClientFactory;

	public CheckConnectionsHandler(ILogger<CheckConnectionsHandler> logger, IImapClientFactory imapClientFactory)
	{
		_logger = logger;
		_imapClientFactory = imapClientFactory;
	}

	public async Task<CheckConnectionsResponse> Handle(CheckConnectionsRequest request,
		CancellationToken cancellationToken)
	{
		Dictionary<Account, ConnectionStatus> connectionStatuses = new();
		foreach (var account in request.Accounts)
			try
			{
				using ImapClient client = await _imapClientFactory.ImapClientAsync(account, cancellationToken);
				
				connectionStatuses.Add(account, new ConnectionStatus { Works = client.IsAuthenticated && client.IsConnected });
			}
			catch (Exception exception)
			{
				if (_logger.IsEnabled(LogLevel.Warning))
					_logger.LogWarning(
						exception,
						"Failed to connect to account {Account}",
						account.ToDictionary()
					);

				connectionStatuses.Add(account, new ConnectionStatus { Works = false, Error = exception.Message });
			}

		return new CheckConnectionsResponse
		{
			Result = connectionStatuses
		};
	}

	private static async Task<bool> ConnectAndAuthenticateOAuth2Async(
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

		using ImapClient client = new();

		await client.ConnectAsync("imap.gmail.com", account.ImapPort, SecureSocketOptions.SslOnConnect,
			cancellationToken);
		await client.AuthenticateAsync(oauth2, cancellationToken);
		return client.IsAuthenticated && client.IsConnected;
	}

	private static async Task<bool> ConnectAndAuthenticateAsync(CancellationToken cancellationToken,
		Account account)
	{
		using ImapClient client = new();
		await client.ConnectAsync(
			account.ImapServer,
			account.ImapPort,
			SecureSocketOptions.SslOnConnect,
			cancellationToken
		);
		await client.AuthenticateAsync(account.Username, account.Password, cancellationToken);
		return client.IsAuthenticated && client.IsConnected;
	}
}