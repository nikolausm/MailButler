// See https://aka.ms/new-console-template for more information

using MailButler.Console;
using MailButler.UseCases.Components.Extensions.DependencyInjection;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json",
		true)
	.AddEnvironmentVariables()
	.AddUserSecrets<MailButlerOptions>()
	.Build();

var services = new ServiceCollection();
services.AddUseCases();
services.AddLogging(builder =>
{
	builder.AddConfiguration(configuration.GetSection("Logging"));
	builder.AddSimpleConsole(options =>
	{
		options.IncludeScopes = true;
		options.SingleLine = true;
		options.TimestampFormat = "hh:mm:ss ";
	});
});

services.AddSingleton<IConfiguration>(_ => configuration);

services.Configure<MailButlerOptions>(configuration.GetSection("MailButler"));
services.AddTransient<AmazonOrderSummaryAction>();


using var scope = services.BuildServiceProvider().CreateScope();

var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
var tokenSource = new CancellationTokenSource();
var options = scope.ServiceProvider.GetRequiredService<IOptions<MailButlerOptions>>().Value;
var startDate = DateTime.Now;
using (var _ = logger.BeginScope("AmazonOrderSummaryAction =>"))
{
	logger.LogInformation("Starting");
	await scope.ServiceProvider.GetRequiredService<AmazonOrderSummaryAction>()
		.ExecuteAsync(
			new AmazonOrderSummaryRequest
			{
				SmtpAccount = options.AmazonOrderSummaryAction.SmtpAccount,
				MarkEmailAsRead = options.AmazonOrderSummaryAction.MarkEmailAsRead,
				EvenIfAllEmailsAreRead = options.AmazonOrderSummaryAction.EvenIfAllEmailsAreRead,
				DateTime = options.AmazonOrderSummaryAction.LastRun,
				DaysToCheck = options.AmazonOrderSummaryAction.DaysToCheck,
				Accounts = options.Accounts
			}, tokenSource.Token
		);
	logger.LogInformation("Finished run for {Date:yyyy-MM-dd}", DateTime.Today);

	#region Update Runtime

	dynamic? jsonObj = JsonConvert.DeserializeObject<JObject>(
		File.ReadAllText("appsettings.json")
	);
	if (jsonObj is null) throw new Exception("Failed to read appsettings.json");
	jsonObj["MailButler"]["AmazonOrderSummaryAction"]["LastRun"] = startDate.ToString("s");
	File.WriteAllText("appsettings.json", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));

	#endregion
}