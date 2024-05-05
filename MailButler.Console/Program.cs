// See https://aka.ms/new-console-template for more information

using MailButler.Configuration.AzureJson.Extensions.Configuration;
using MailButler.Console;
using MailButler.Console.Extensions.DependencyInjection;
using MailButler.UseCases.Components.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var baseConfigurationBuilder = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.AddJsonFile(
		"appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json",
		true
	);
var baseConfiguration = baseConfigurationBuilder
	.AddEnvironmentVariables()
	.AddUserSecrets<Program>()
	.Build();

var configuration = baseConfigurationBuilder
	.AddAzureJson<MailButlerConsoleOptions>(
		baseConfiguration["MailButler:AzureJson:ContainerRoot"] ??
		throw new Exception("AzureJson:ContainerRoot is not set"),
		baseConfiguration["MailButler:AzureJson:FileName"] ?? throw new Exception("AzureJson:FileName is not set"),
		"MailButler"
	)
	.AddEnvironmentVariables()
	.AddUserSecrets<Program>()
	.Build();


var services = new ServiceCollection();
services.AddUseCases();
services.AddTransient<MailButlerMainAction>();
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

services.Configure<MailButlerConsoleOptions>(configuration.GetSection("MailButler"));
services.AddMailButler();

using var scope = services.BuildServiceProvider().CreateScope();
await scope.ServiceProvider.GetRequiredService<MailButlerMainAction>().ExecuteAsync();