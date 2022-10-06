using MailButler.Dtos;
using MailKit;
using MailKit.Search;
using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailButler.UseCases.FetchEmails;

public sealed class FetchEmailsHandler : IRequestHandler<FetchEmailsRequest, FetchEmailsResponse>
{
	private readonly ILogger<FetchEmailsHandler> _logger;
	private readonly IMailFolder _source;

	public FetchEmailsHandler(
		IMailFolder source,
		ILogger<FetchEmailsHandler> logger
	)
	{
		_source = source;
		_logger = logger;
	}

	public async Task<FetchEmailsResponse> Handle(FetchEmailsRequest request, CancellationToken cancellationToken)
	{
		List<Email> emails = new();
		try
		{
			await _source.OpenAsync(FolderAccess.ReadOnly, cancellationToken);

			_logger.LogTrace("Total messages: {TotalMessageCount}", _source.Count);
			_logger.LogTrace("Recent messages: {RecentMessageCount}", _source.Recent);

			var ids = await _source.SearchAsync(
				SearchQuery.SentSince(request.StartDate),
				cancellationToken
			);

			foreach (var id in ids)
			{
				var message = await _source.GetMessageAsync(id, cancellationToken);

				emails.Add(
					Email(message)
				);
			}

			return new FetchEmailsResponse
			{
				Result = emails
			};
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error fetching emails for request {Request}", request);
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