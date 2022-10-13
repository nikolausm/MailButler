// See https://aka.ms/new-console-template for more information

using MailButler.Console;
using MailButler.Dtos;
using MailButler.UseCases.Components.Extensions.DependencyInjection;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
services.AddTransient<IList<Account>>(sp => sp.GetRequiredService<IOptions<MailButlerOptions>>().Value.Accounts);
services.AddTransient<AmazonOrderSummaryAction>();


using var scope = services.BuildServiceProvider().CreateScope();

var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
var smtpAccount = scope.ServiceProvider.GetRequiredService<IList<Account>>()
	.First(r => r.Name.Contains("iCloud", StringComparison.InvariantCultureIgnoreCase));
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
				StartDate = options.AmazonOrderSummaryAction.StartDate
			}, tokenSource.Token
		);
	logger.LogInformation("Finished run for {Date:yyyy-MM-dd}", DateTime.Today);
	
	#region Update Runtime
	dynamic? jsonObj = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(
		File.ReadAllText("appsettings.json")
	);
	if (jsonObj is null)
	{
		throw new Exception("Failed to read appsettings.json");
	}
	jsonObj["MailButler"]["AmazonOrderSummaryAction"]["StartDate"] = startDate.ToString("s");
	File.WriteAllText("appsettings.json", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
	#endregion
}

