using Extensions.Dictionary;
using MailButler.Core;
using MailButler.Dtos;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailButler.UseCases.FetchEmails;

public sealed class FetchEmailsHandler : IRequestHandler<FetchEmailsRequest, FetchEmailsResponse>
{
	private readonly ILogger<FetchEmailsHandler> _logger;
	private readonly IImapClientFactory _imapClientFactory;

	public FetchEmailsHandler(
		IImapClientFactory imapClientFactory,
		ILogger<FetchEmailsHandler> logger
	)
	{
		_imapClientFactory = imapClientFactory;
		_logger = logger;
	}

	public async Task<FetchEmailsResponse> Handle(FetchEmailsRequest request, CancellationToken cancellationToken)
	{
		List<Email> emails = new();
		try
		{
			using ImapClient client = await _imapClientFactory.ImapClientAsync(request.Account, cancellationToken);
			var source = client.Inbox;
			await source.OpenAsync(FolderAccess.ReadOnly, cancellationToken);

			_logger.LogTrace("Total messages: {TotalMessageCount}", source.Count);
			_logger.LogTrace("Recent messages: {RecentMessageCount}", source.Recent);

			var ids = await source.SearchAsync(
				SearchQuery.SentSince(request.StartDate),
				cancellationToken
			);

			foreach (var id in ids)
			{
				var message = await source.GetMessageAsync(id, cancellationToken);

				emails.Add(
					Email(message)
				);
			}

			return new FetchEmailsResponse
			{
				Result = emails
			};
		}
		catch (Exception exception)
		{
			if (_logger.IsEnabled(LogLevel.Error))
			{
				_logger.LogError(
					exception,
					"Error fetching emails for request {Request}",
					request.ToDictionary()
				);
			}

			return new FetchEmailsResponse
			{
				Message = "Error fetching emails",
				Status = Status.Failed,
				Result = emails
			};
		}
	}

	private static Email Email(MimeMessage message)
	{
		return new Email
		{
			Sender = new MailBoxAddress
			{
				Name = message.Sender.Name,
				Address = message.Sender.Address
			},
			To = message.To.Mailboxes.Select(x => new MailBoxAddress
			{
				Name = x.Name,
				Address = x.Address
			}).ToList(),

			Cc = message.Cc.Mailboxes.Select(x => new MailBoxAddress
			{
				Name = x.Name,
				Address = x.Address
			}).ToList(),

			Sent = message.Date.DateTime,
			Subject = message.Subject,
			HtmlBody = message.HtmlBody,
			TextBody = message.TextBody
		};
	}
}