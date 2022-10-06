﻿// See https://aka.ms/new-console-template for more information


using MailButler.Console;
using MailButler.UseCases;
using MailButler.UseCases.CheckConnections;
using MailButler.UseCases.Extensions.DependencyInjection;
using MailButler.UseCases.FetchEmails;
using MailKit;
using MailKit.Net.Imap;
using MediatR;
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
services.AddLogging(builder => builder.AddConsole());

services.AddSingleton<IConfiguration>(_ => configuration);

services.Configure<MailButlerOptions>(configuration.GetSection("MailButler"));

using var scope = services.BuildServiceProvider().CreateScope();

var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
var options = scope.ServiceProvider.GetRequiredService<IOptions<MailButlerOptions>>();

var result = await mediator.Send(
	new CheckConnectionsRequest
	{
		Accounts = options.Value.Accounts
	}
);

var fetchEmailsResponse = await mediator.Send(
	new FetchEmailsRequest
	{
		Account = result.Result.First().Key
	}
);

Console.WriteLine("Found: " + fetchEmailsResponse.Result.Count);