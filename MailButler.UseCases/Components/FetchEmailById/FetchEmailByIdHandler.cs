using Extensions.Dictionary;
using MailButler.Core;
using MailButler.Dtos;
using MailButler.UseCases.Components.Extensions;
using MailKit;
using Mediator;
using Microsoft.Extensions.Logging;
using UniqueId = MailKit.UniqueId;

namespace MailButler.UseCases.Components.FetchEmailById;

public sealed class FetchEmailByIdHandler : IRequestHandler<FetchEmailByIdRequest, FetchEmailByIdResponse>
{
	private readonly IImapClientFactory _imapClientFactory;
	private readonly ILogger<FetchEmailByIdHandler> _logger;

	public FetchEmailByIdHandler(
		IImapClientFactory imapClientFactory,
		ILogger<FetchEmailByIdHandler> logger
	)
	{
		_imapClientFactory = imapClientFactory;
		_logger = logger;
	}

	public async ValueTask<FetchEmailByIdResponse> Handle(FetchEmailByIdRequest request,
		CancellationToken cancellationToken)
	{
		try
		{
			using var client = await _imapClientFactory.ImapClientAsync(request.Account, cancellationToken);
			var source = client.Inbox;
			await source.OpenAsync(FolderAccess.ReadOnly, cancellationToken);

			_logger.LogTrace("Total messages: {TotalMessageCount}", source.Count);
			_logger.LogTrace("Recent messages: {RecentMessageCount}", source.Recent);

			var message = await source.GetMessageAsync(
				new UniqueId(request.EmailId.Id, request.EmailId.Validity),
				cancellationToken);

			IList<IMessageSummary> messageSummaries = await source.FetchAsync(
				new List<UniqueId>
				{
					new(request.EmailId.Id, request.EmailId.Validity)
				},
				MessageSummaryItems.Flags,
				cancellationToken
			);

			var mail = message.ToEmail(
				messageSummaries.Single().Flags!.Value,
				new UniqueId(request.EmailId.Id, request.EmailId.Validity),
				request.Account.Id);

			return new FetchEmailByIdResponse
			{
				Result = mail
			};
		}
		catch (Exception exception)
		{
			if (_logger.IsEnabled(LogLevel.Error))
				_logger.LogError(
					exception,
					"Error fetching emails for request {Request}",
					request.ToDictionary()
				);

			return new FetchEmailByIdResponse
			{
				Message = "Error fetching emails",
				Status = Status.Failed,
				Result = new Email()
			};
		}
	}
}