using Extensions.Dictionary;
using MailButler.Core;
using MailButler.Dtos;
using MailButler.UseCases.Extensions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;
using UniqueId = MailButler.Dtos.UniqueId;

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

			IList<MailKit.UniqueId>? ids = await source.SearchAsync(
				SearchQuery.SentSince(request.StartDate),
				cancellationToken
			);

			List<MimeMessage> messages = new();
			ids.ToList().ForEach(id => messages.Add(source.GetMessageAsync(id, cancellationToken).GetAwaiter().GetResult()));
			var messageSummaries = await source.FetchAsync(ids, MessageSummaryItems.Flags, cancellationToken);

			for (int i = 0; i < ids.Count; i++)
			{
				emails.Add(messages[i].ToEmail(messageSummaries[i].Flags!.Value, ids[i]));
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
}