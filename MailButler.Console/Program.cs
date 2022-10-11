// See https://aka.ms/new-console-template for more information

using Google.Apis.Util;
using MailButler.Dtos;
using MailButler.MailRules.Filter;
using MailButler.UseCases.Komponents.Extensions.DependencyInjection;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailButlerOptions = MailButler.Console.MailButlerOptions;

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

var tokenSource = new CancellationTokenSource();

List<int> hoursToRun = new() { 7, 12, 21 };
logger.LogInformation("Started this Service: {Date:yyyy-MM-dd}", DateTime.Now);
while (!tokenSource.IsCancellationRequested)
{
	while (hoursToRun.Contains(DateTime.Now.Hour))
	{
		await Task.Delay(1000 * 60 * 60);
	}
	
	using (var _ = logger.BeginScope("MailButler"))
	{
		logger.LogInformation("Starting");
		await scope.ServiceProvider.GetRequiredService<AmazonOrderSummaryAction>()
			.ExecuteAsync(CancellationToken.None);
		logger.LogInformation("Finished run for {Date:yyyy-MM-dd}", DateTime.Today);
	}
	
	while (hoursToRun.Contains(DateTime.Now.Hour))
	{
		await Task.Delay(1000 * 60 * 60);
	}
}