using MailButler.Dtos;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using Action = MailButler.Api.Dtos.Action;

namespace MailButler.Api;

public sealed class BackgroundServiceWorker : BackgroundService
{
	private readonly IServiceScopeFactory _factory;
	private readonly ILogger<BackgroundServiceWorker> _logger;
	private readonly BackgroundServiceQueue _queue;


	public BackgroundServiceWorker(
		IServiceScopeFactory factory,
		ILogger<BackgroundServiceWorker> logger,
		BackgroundServiceQueue queue
	)
	{
		_factory = factory;
		_logger = logger;
		_queue = queue;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Starting background service worker");
		while (!stoppingToken.IsCancellationRequested)
		{
			if (!_queue.TryDequeue(out Action type))
			{
				await Task.Delay(1000, stoppingToken);
				continue;
			}

			using IServiceScope scope = _factory.CreateScope();
			
			switch (type)
			{
				case Action.AmazonOrderSummary:
					_logger.LogInformation("Starting {Action}", type);
					await scope.ServiceProvider
						.GetRequiredService<AmazonOrderSummaryAction>()
						.ExecuteAsync(
							new AmazonOrderSummaryRequest
							{
								MarkEmailAsRead = false,
								EvenIfAllEmailsAreRead = true,
								SmtpAccount = scope.ServiceProvider
									.GetRequiredService<IList<Account>>().First(
									r => r
										.Name
										.Contains("iCloud", StringComparison.InvariantCultureIgnoreCase)
								),
								DateTime = DateTime.Now.AddDays(-7),
								DaysToCheck = 7
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