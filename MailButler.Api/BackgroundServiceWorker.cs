using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using Microsoft.Extensions.Options;
using Action = MailButler.Api.Dtos.Action;

namespace MailButler.Api;

public sealed class BackgroundServiceWorker : BackgroundService
{
	private readonly IServiceScopeFactory _factory;
	private readonly ILogger<BackgroundServiceWorker> _logger;
	private readonly IOptions<MailButlerOptions> _mailButlerOptions;
	private readonly BackgroundServiceQueue _queue;


	public BackgroundServiceWorker(
		IServiceScopeFactory factory,
		ILogger<BackgroundServiceWorker> logger,
		BackgroundServiceQueue queue,
		IOptions<MailButlerOptions> mailButlerOptions
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

			switch (type)
			{
				case Action.AmazonOrderSummary:
					var options = _mailButlerOptions.Value.AmazonOrderSummaryAction;
					_logger.LogInformation("Starting {Action}", type);
					Action = type;
					Started = DateTime.Now;
					await scope.ServiceProvider
						.GetRequiredService<AmazonOrderSummaryAction>()
						.ExecuteAsync(
							new AmazonOrderSummaryRequest
							{
								MarkEmailAsRead = options.MarkEmailAsRead,
								EvenIfAllEmailsAreRead = options.EvenIfAllEmailsAreRead,
								SmtpAccount = options.SmtpAccount,
								DateTime = DateTime.Now.AddDays(-7),
								DaysToCheck = options.DaysToCheck
							}, stoppingToken
						);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		_logger.LogInformation("Finished background service worker");
	}
}