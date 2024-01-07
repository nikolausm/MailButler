using System.Collections.Concurrent;
using MailButler.Core;
using MailButler.Dtos;
using MailButler.UseCases.Components.Extensions;
using MailKit;
using MailKit.Search;
using Mediator;
using Microsoft.Extensions.Logging;
using MimeKit;
using UniqueId = MailKit.UniqueId;

namespace MailButler.UseCases.Components.FetchEmails;

public sealed class FetchEmailsHandler : IRequestHandler<FetchEmailsRequest, FetchEmailsResponse>
{
	private readonly IImapClientFactory _imapClientFactory;
	private readonly ILogger<FetchEmailsHandler> _logger;

	public FetchEmailsHandler(
		IImapClientFactory imapClientFactory,
		ILogger<FetchEmailsHandler> logger
	)
	{
		_imapClientFactory = imapClientFactory;
		_logger = logger;
	}

	public async ValueTask<FetchEmailsResponse> Handle(FetchEmailsRequest request, CancellationToken cancellationToken)
	{
		using (var logScope = _logger.BeginScope(request.Account.Name))
		{
			List<Email> emails = new();
			try
			{
				using var client = await _imapClientFactory.ImapClientAsync(request.Account, cancellationToken);
				var source = client.Inbox;
				await source.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
				_logger.LogTrace("{Account}", request.Account.ToString());
				_logger.LogTrace("Total messages: {TotalMessageCount}", source.Count);
				_logger.LogTrace("Recent messages: {RecentMessageCount}", source.Recent);

				//var res = await source.SearchAsync(SearchQuery.SubjectContains("vanvilla Duschsystem"));
				IList<UniqueId> ids = await source.SearchAsync(
					SearchQuery.SentSince(request.StartDate),
					cancellationToken
				)!;
				//ids = res;

				_logger.LogTrace("Fetching {Count} messages", ids.Count);
				var mimMessages = await GetMessagesAsync(ids, source, cancellationToken);
				_logger.LogTrace("Fetched {Count} messages", mimMessages.Count);

				_logger.LogTrace("Fetching {Count} summaries", ids.Count);
				var flagsInformation = await source.FetchAsync(ids, MessageSummaryItems.Flags, cancellationToken);
				_logger.LogTrace("Fetched {Count} summaries", flagsInformation.Count);

				for (var i = 0; i < ids.Count; i++)
				{
					if (i % 100 == 0 || i == ids.Count - 1)
					{
						_logger.LogTrace("Added Emails {Current}/{Count}", i + 1, ids.Count);
					}

					emails.Add(mimMessages[i].ToEmail(flagsInformation[i].Flags!.Value, ids[i], request.Account.Id));
				}

				return new FetchEmailsResponse
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

				return new FetchEmailsResponse
				{
					Message = $"Error fetching emails from account {request.Account.Name}",
					Status = Status.Failed,
					Result = emails
				};
			}
		}
	}

	private async Task<IList<MimeMessage>> GetMessagesAsync(
		IList<UniqueId> ids,
		IMailFolder source,
		CancellationToken cancellationToken = default
	)
	{
		List<MimeMessage> messages = new();
		foreach (var id in ids)
		{
			var message = await source.GetMessageAsync(id, cancellationToken);
			messages.Add(message);
			if (messages.Count % 100 == 0 || messages.Count == ids.Count)
			{
				_logger.LogTrace("Message {Current}/{Count} retrieved", messages.Count, ids.Count);
			}

			if (messages.Count == ids.Count)
			{
				break;
			}
		}

		return messages;
	}
}