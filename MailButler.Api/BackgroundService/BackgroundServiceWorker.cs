using MailButler.Api.Options;
using MailButler.UseCases.Components.CheckConnections;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using MailButler.UseCases.Solutions.ForwardToGetMyInvoices;
using MailButler.UseCases.Solutions.Spamfilter;
using Mediator;
using Microsoft.Extensions.Options;
using Action = MailButler.Api.Dtos.Action;

namespace MailButler.Api.BackgroundService;

public sealed class BackgroundServiceWorker : Microsoft.Extensions.Hosting.BackgroundService
{
	private readonly IServiceScopeFactory _factory;
	private readonly ILogger<BackgroundServiceWorker> _logger;
	private readonly IOptions<MailButlerOptions> _mailButlerOptions;
	private readonly BackgroundServiceQueue _queue;


	public BackgroundServiceWorker(
		IServiceScopeFactory factory,
		ILogger<BackgroundServiceWorker> logger,
		BackgroundServiceQueue queue,
		IOptions<MailButlerOptions> mailButlerOptions,
		IConfiguration configuration
	)
	{
		_factory = factory;
		_logger = logger;
		_queue = queue;
		_mailButlerOptions = mailButlerOptions;
	}

	public Action Action { get; private set; }

	public DateTime Started { get; set; }

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Starting background service worker");
		var lastStatusUpdate = DateTime.MinValue;
		while (!stoppingToken.IsCancellationRequested)
		{
			if ((DateTime.Now - lastStatusUpdate).TotalMinutes > 5)
			{
				lastStatusUpdate = DateTime.Now;
				_logger.LogInformation("Background service worker is still running");
			}

			if (!_queue.TryDequeue(out var type))
			{
				await Task.Delay(1000, stoppingToken);
				continue;
			}

			using var scope = _factory.CreateScope();
			_logger.LogInformation("Starting {Action}", type);
			Action = type;
			Started = DateTime.Now;

			switch (type)
			{
				case Action.CheckAccounts:
					var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
					await mediator.Send(new CheckConnectionsRequest
					{
						Accounts = _mailButlerOptions.Value.Accounts
					});
				break;
				case Action.AmazonOrderSummary:
					var amazonOrderSummaryActionOptions = _mailButlerOptions.Value.AmazonOrderSummaryAction;
					await scope.ServiceProvider
						.GetRequiredService<AmazonOrderSummaryAction>()
						.ExecuteAsync(
							new AmazonOrderSummaryRequest
							{
								MarkEmailAsRead = amazonOrderSummaryActionOptions.MarkEmailAsRead,
								EvenIfAllEmailsAreRead = amazonOrderSummaryActionOptions.EvenIfAllEmailsAreRead,
								SmtpAccount = amazonOrderSummaryActionOptions.SmtpAccount,
								DateTime = DateTime.Now.AddDays(-7),
								DaysToCheck = amazonOrderSummaryActionOptions.DaysToCheck,
								Accounts = _mailButlerOptions.Value.Accounts
							}, stoppingToken
						);

					break;
				case Action.ForwardInvoicesToGetMyInvoices:
					var forwardToGetMyInvoicesOptions = _mailButlerOptions.Value.ForwardToGetMyInvoices;
					await scope.ServiceProvider
						.GetRequiredService<ForwardToGetMyInvoicesAction>()
						.ExecuteAsync(
							new ForwardToGetMyInvoicesRequest
							{
								SmtpAccount = forwardToGetMyInvoicesOptions.SmtpAccount,
								DateTime = DateTime.Now.AddDays(-2),
								DaysToCheck = forwardToGetMyInvoicesOptions.DaysToCheck,
								IonosAccountId = forwardToGetMyInvoicesOptions.IonosAccountId,
								Recipients = forwardToGetMyInvoicesOptions.Recipients,
								Accounts = _mailButlerOptions.Value.Accounts
							}, stoppingToken
						);
					break;
				case Action.MarkOldEmailAsRead:

					MarkOldEmailsAsReadOptions markOldEmailsAsReadOptions =
						_mailButlerOptions.Value.MarkOldEmailsAsRead;
					await scope.ServiceProvider
						.GetRequiredService<MarkOldEmailsAsReadAction>()
						.ExecuteAsync(
							new MarkOldEmailsAsReadRequest()
							{
								SenderAddresses = markOldEmailsAsReadOptions.SenderAddresses,
								SmtpAccount = markOldEmailsAsReadOptions.SmtpAccount,
								DaysToCheck = markOldEmailsAsReadOptions.DaysToCheck,
								Accounts = _mailButlerOptions.Value.Accounts,
								TimeSpan = markOldEmailsAsReadOptions.TimeSpan
							}, stoppingToken
						);
					break;
				case Action.DeleteFromKnownSender:
					var deleteFromKnownSenderOptions = _mailButlerOptions.Value.DeleteFromKnownSender;
					await scope.ServiceProvider
						.GetRequiredService<DeleteFromKnownSenderAction>()
						.ExecuteAsync(
							new DeleteFromKnownSenderRequest
							{
								SenderAddresses = deleteFromKnownSenderOptions.SenderAddresses,
								SmtpAccount = deleteFromKnownSenderOptions.SmtpAccount,
								DaysToCheck = deleteFromKnownSenderOptions.DaysToCheck,
								Accounts = _mailButlerOptions.Value.Accounts
							}, stoppingToken
						);
					break;
				case Action.Unknown:
				case Action.CurrentAction:
				default:
					throw new ArgumentOutOfRangeException(type.ToString());
			}

			Action = Action.Unknown;
			Started = DateTime.MinValue;
		}

		_logger.LogInformation("Finished background service worker");
	}
}