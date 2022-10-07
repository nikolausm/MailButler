using Extensions.Dictionary;
using MailButler.Core;
using MailButler.Dtos;
using MailButler.UseCases.Extensions;
using MailKit;
using MailKit.Net.Imap;
using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailButler.UseCases.FetchEmailById;

public sealed class FetchEmailByIdHandler : IRequestHandler<FetchEmailByIdRequest, FetchEmailByIdResponse>
{
	private readonly ILogger<FetchEmailByIdHandler> _logger;
	private readonly IImapClientFactory _imapClientFactory;

	public FetchEmailByIdHandler(
		IImapClientFactory imapClientFactory,
		ILogger<FetchEmailByIdHandler> logger
	)
	{
		_imapClientFactory = imapClientFactory;
		_logger = logger;
	}

	public async Task<FetchEmailByIdResponse> Handle(FetchEmailByIdRequest request, CancellationToken cancellationToken)
	{
		try
		{
			using ImapClient client = await _imapClientFactory.ImapClientAsync(request.Account, cancellationToken);
			var source = client.Inbox;
			await source.OpenAsync(FolderAccess.ReadOnly, cancellationToken);

			_logger.LogTrace("Total messages: {TotalMessageCount}", source.Count);
			_logger.LogTrace("Recent messages: {RecentMessageCount}", source.Recent);
			
			MimeMessage message = await source.GetMessageAsync(
					new MailKit.UniqueId(request.EmailId.Id, request.EmailId.Validity),
					cancellationToken);
			
			IList<IMessageSummary> messageSummaries = await source.FetchAsync(
				new List<MailKit.UniqueId>
				{
					new (request.EmailId.Id, request.EmailId.Validity)
				},
				MessageSummaryItems.Flags,
				cancellationToken
			);

			Email mail = message.ToEmail(messageSummaries.Single().Flags!.Value, 
				new MailKit.UniqueId(request.EmailId.Id, request.EmailId.Validity));
			
			return new FetchEmailByIdResponse
			{
				Result = mail
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

			return new FetchEmailByIdResponse
			{
				Message = "Error fetching emails",
				Status = Status.Failed,
				Result = new Email()
			};
		}
	}
}