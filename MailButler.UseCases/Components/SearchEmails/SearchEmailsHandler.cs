using MailButler.Core;
using MailButler.Dtos;
using MailButler.UseCases.Components.Extensions;
using MailKit;
using MailKit.Search;
using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailButler.UseCases.Components.SearchEmails;

public sealed class SearchEmailsHandler : IRequestHandler<SearchEmailsRequest, SearchEmailsResponse>
{
	private readonly IImapClientFactory _imapClientFactory;
	private readonly ILogger<SearchEmailsHandler> _logger;

	public SearchEmailsHandler(
		IImapClientFactory imapClientFactory,
		ILogger<SearchEmailsHandler> logger
	)
	{
		_imapClientFactory = imapClientFactory;
		_logger = logger;
	}

	public async Task<SearchEmailsResponse> Handle(SearchEmailsRequest request, CancellationToken cancellationToken)
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

			var ids = await source.SearchAsync(
				request.Query,
				cancellationToken
			);

			List<MimeMessage> messages = new();
			ids.ToList().ForEach(id =>
				messages.Add(source.GetMessageAsync(id, cancellationToken).GetAwaiter().GetResult()));
			var messageSummaries = await source.FetchAsync(ids, MessageSummaryItems.Flags, cancellationToken);

			for (var i = 0; i < ids.Count; i++)
				emails.Add(messages[i].ToEmail(messageSummaries[i].Flags!.Value, ids[i], request.Account.Id));

			return new SearchEmailsResponse
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

			return new SearchEmailsResponse
			{
				Message = $"Error fetching emails from account {request.Account.Name}",
				Status = Status.Failed,
				Result = emails
			};
		}
	}
}