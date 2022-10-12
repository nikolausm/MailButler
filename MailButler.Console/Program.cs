// See https://aka.ms/new-console-template for more information

using MailButler.Console;
using MailButler.Dtos;
using MailButler.UseCases.Components.Extensions.DependencyInjection;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json",
		true)
	.AddUserSecrets<MailButlerOptions>()
	.AddEnvironmentVariables()
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


using (var _ = logger.BeginScope("AmazonOrderSummaryAction =>"))
{
	logger.LogInformation("Starting");
	await scope.ServiceProvider.GetRequiredService<AmazonOrderSummaryAction>()
		.ExecuteAsync(
			new AmazonOrderSummaryRequest
			{
				SmtpAccount = smtpAccount
			}, tokenSource.Token
		);
	logger.LogInformation("Finished run for {Date:yyyy-MM-dd}", DateTime.Today);
}