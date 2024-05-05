using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using MailButler.UseCases.Solutions.MarkOldEmailsAsRead;
using MailButler.UseCases.Solutions.Spamfilter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MailButler.Console;

public class MailButlerMainAction
{
	private readonly ILogger<MailButlerMainAction> _logger;
	private readonly MailButlerConsoleOptions _consoleOptions;
	private readonly IServiceProvider _serviceProvider;

	public MailButlerMainAction(
		ILogger<MailButlerMainAction> logger,
		IOptions<MailButlerConsoleOptions> options,
		IServiceProvider serviceProvider
	)
	{
		_logger = logger;
		_consoleOptions = options.Value;
		_serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync()
	{
		var tokenSource = new CancellationTokenSource();
		await DeleteFromKnownSenderActionAsync(tokenSource.Token);
		await MarkOldEmailsAsReadAsync(tokenSource);
		await AmazonOrderSummaryActionAsync(tokenSource);
	}

	private async Task DeleteFromKnownSenderActionAsync(CancellationToken cancellationToken = default)
	{
		using (var _ = _logger.BeginScope("DeleteFromKnownSenderActionAsync =>"))
		{
			_logger.LogInformation("Starting");
			await _serviceProvider.GetRequiredService<DeleteFromKnownSenderAction>()
				.ExecuteAsync(
					new DeleteFromKnownSenderRequest
					{
						SmtpAccount = _consoleOptions.SmtpAccount,
						Accounts = _consoleOptions.Accounts,
						SenderAddresses = _consoleOptions.DeleteFromKnownSender.SenderAddresses
					},
					cancellationToken: cancellationToken
				);
			_logger.LogInformation("Finished run for {Date:yyyy-MM-dd}", DateTime.Today);
		}
	}
	private async Task AmazonOrderSummaryActionAsync(CancellationTokenSource tokenSource)
	{
		using (var _ = _logger.BeginScope("AmazonOrderSummaryActionAsync =>"))
		{
			var startDate = DateTime.Now;
			_logger.LogInformation("Starting");
			await _serviceProvider.GetRequiredService<AmazonOrderSummaryAction>()
				.ExecuteAsync(
					new AmazonOrderSummaryRequest
					{
						SmtpAccount = _consoleOptions.AmazonOrderSummaryAction.SmtpAccount,
						MarkEmailAsRead = _consoleOptions.AmazonOrderSummaryAction.MarkEmailAsRead,
						EvenIfAllEmailsAreRead = _consoleOptions.AmazonOrderSummaryAction.EvenIfAllEmailsAreRead,
						DateTime = _consoleOptions.AmazonOrderSummaryAction.LastRun,
						DaysToCheck = _consoleOptions.AmazonOrderSummaryAction.DaysToCheck,
						Accounts = _consoleOptions.Accounts
					}, tokenSource.Token
				);
			_logger.LogInformation("Finished run for {Date:yyyy-MM-dd}", DateTime.Today);

			#region Update Runtime

			dynamic? jsonObj = JsonConvert.DeserializeObject<JObject>(
				await File.ReadAllTextAsync("appsettings.json")
			);
			if (jsonObj is null) throw new Exception("Failed to read appsettings.json");
			jsonObj["MailButler"][nameof(_consoleOptions.AmazonOrderSummaryAction)]["LastRun"] = startDate.ToString("s");
			await File.WriteAllTextAsync("appsettings.json", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));

			#endregion
		}
	}

	private async Task MarkOldEmailsAsReadAsync(CancellationTokenSource tokenSource)
	{
		using (var _ = _logger.BeginScope("MarkOldEmailsAsReadAsync =>"))
		{
			var startDate = DateTime.Now;
			_logger.LogInformation("Starting");
			await _serviceProvider.GetRequiredService<MarkOldEmailsAsReadAction>()
				.ExecuteAsync(
					new MarkOldEmailsAsReadRequest
					{
						SenderAddresses = _consoleOptions.MarkOldEmailsAsRead.SenderAddresses,
						SmtpAccount = _consoleOptions.SmtpAccount,
						DaysToCheck = 14,
						Accounts = _consoleOptions.Accounts,
						TimeSpan = _consoleOptions.MarkOldEmailsAsRead.TimeSpan
					}, tokenSource.Token
				);
			_logger.LogInformation("Finished run for {Date:yyyy-MM-dd}", DateTime.Today);

			#region Update Runtime

			dynamic? jsonObj = JsonConvert.DeserializeObject<JObject>(
				await File.ReadAllTextAsync("appsettings.json")
			);
			if (jsonObj is null) throw new Exception("Failed to read appsettings.json");
			jsonObj["MailButler"][nameof(_consoleOptions.MarkOldEmailsAsRead)]["LastRun"] = startDate.ToString("s");
			await File.WriteAllTextAsync("appsettings.json", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));

			#endregion
		}
	}
}