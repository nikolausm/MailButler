using MailButler.Core;
using MailButler.Dtos;
using MailKit;
using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;
using UniqueId = MailKit.UniqueId;

namespace MailButler.UseCases.Components.ForwardEmails;

public sealed class ForwardEmailsHandler : IRequestHandler<ForwardEmailsRequest, ForwardEmailsResponse>
{
	private readonly IImapClientFactory _imapClientFactory;
	private readonly ILogger<ForwardEmailsHandler> _logger;
	private readonly ISmtpClientFactory _smtpClientFactory;

	public ForwardEmailsHandler(
		IImapClientFactory imapClientFactory,
		ILogger<ForwardEmailsHandler> logger,
		ISmtpClientFactory smtpClientFactory
	)
	{
		_imapClientFactory = imapClientFactory;
		_logger = logger;
		_smtpClientFactory = smtpClientFactory;
	}

	public async Task<ForwardEmailsResponse> Handle(ForwardEmailsRequest request, CancellationToken cancellationToken)
	{
		List<Email> emails = new();
		try
		{
			foreach (var account in request.Accounts)
			{
				using var client = await _imapClientFactory.ImapClientAsync(account, cancellationToken);
				var source = client.Inbox;
				await source.OpenAsync(FolderAccess.ReadOnly, cancellationToken);

				foreach (var email in request.Emails.Where(email => email.AccountId == account.Id))
				{
					await ForwardEmail(request, cancellationToken, source, email);
					emails.Add(email);
				}
			}

			return new ForwardEmailsResponse
			{
				Result = emails
			};
		}
		catch (Exception exception)
		{
			if (_logger.IsEnabled(LogLevel.Error))
				_logger.LogError(
					exception,
					"Error fetching emails for request {Request}",
					request.ToString()
				);

			return new ForwardEmailsResponse
			{
				Message = $"Error forwarding emails: {exception.Message}",
				Status = Status.Failed,
				Result = emails
			};
		}
	}

	private async Task ForwardEmail(ForwardEmailsRequest request, CancellationToken cancellationToken, IMailFolder source,
		Email email)
	{
		var message = await source.GetMessageAsync(
			new UniqueId(email.Id.Validity, email.Id.Id),
			cancellationToken
		);

		var recipients = request
			.Recipients
			.Select(
				recipient =>
					new MailboxAddress(recipient.Name, recipient.Address)
			)
			.ToList();

		using var smtpClient = await _smtpClientFactory
			.SmtpClientAsync(request.SmtpAccount, cancellationToken);

		message.From.Clear();
		message.From.Add(new MailboxAddress(request.SmtpAccount.Name, request.SmtpAccount.Username));
		await smtpClient
			.SendAsync(
				message,
				new MailboxAddress(request.SmtpAccount.Name, request.SmtpAccount.Username),
				recipients, cancellationToken
			);
	}
}