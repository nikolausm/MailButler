// See https://aka.ms/new-console-template for more information


using MailButler.Console;
using MailButler.Dtos;
using MailButler.UseCases.Amazon;
using MailButler.UseCases.Amazon.GetAmazonOrderEmails;
using MailButler.UseCases.Amazon.GetAmazonOrderSummary;
using MailButler.UseCases.CheckConnections;
using MailButler.UseCases.EmailsMatchAgainstRule;
using MailButler.UseCases.Extensions.DependencyInjection;
using MailButler.UseCases.FetchEmails;
using MailButler.UseCases.MarkAsRead;
using MailButler.UseCases.SendEmail;
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

if (result.Status == Status.Failed)
{
	Console.WriteLine("Failed to get connections");
	return;
}

foreach (var account in options.Value.Accounts)
{
	FetchEmailsResponse fetchEmailsResponse = await mediator.Send(
		new FetchEmailsRequest
		{
			Account = account
		}
	);

	EmailsMatchAgainstRuleResponse emailsMatchAgainstRuleResponse = await mediator.Send(
		new EmailsMatchAgainstRuleRequest()
		{
			Rule = Rule.Create(
				Field.SenderAddress,
				FilterType.Contains,
				"@amazon."
			).And(
				Field.AnyTextField,
				FilterType.RegularExpression,
				"\\d{3}-\\d{7}-\\d{7}"
			).Or(
				new Rule(Field.Subject, FilterType.Contains,
					"Nora Sonn und andere teilen ihre Standpunkte und Ideen auf LinkedIn"),
				new OrRule(Field.AnyTextField, FilterType.Contains, "YGrzifix!")
			),
			Emails = fetchEmailsResponse.Result
		}
	);	
}

/*
FetchEmailByIdResponse emailById = await mediator.Send(
	new FetchEmailByIdRequest
	{
		Account = options.Value.Accounts[0],
		EmailId = new UniqueId(14437, 1479036453)
	}
);

Console.Write(emailById.Result);
*/
/*

foreach (var account in result.Result.Keys)
{
	FetchEmailsResponse fetchEmailsResponse = await mediator.Send(
		new FetchEmailsRequest
		{
			Account = account
		}
	);

	if (fetchEmailsResponse.Status == Status.Failed)
	{
		Console.WriteLine("Failed to fetch emails");
		return;
	}

	GetAmazonOrderEmailsResponse getAmazonOrderEmails = await mediator.Send(new GetAmazonOrderEmailsRequest
	{
		Emails = fetchEmailsResponse.Result
	});

	if (getAmazonOrderEmails.Status == Status.Failed)
	{
		Console.WriteLine("Failed to get amazon order emails");
		return;
	}

	GetAmazonOrderEmailsSummaryResponse getSummaryEmailForAmazon = await mediator.Send(
		new GetAmazonOrderEmailsSummaryRequest
		{
			EmailsWithOrders = getAmazonOrderEmails.Result
		}
	);

	if (getAmazonOrderEmails.Status == Status.Failed)
	{
		Console.WriteLine("Failed to get summary amazon order emails");
		return;
	}

	SendEmailResponse sendEmailResponse = await mediator.Send(
		new SendEmailRequest
		{
			Account = account,
			Email = getSummaryEmailForAmazon.Result
		}
	);
	
	if (sendEmailResponse.Status == Status.Failed)
	{
		Console.WriteLine($"Failed to send summary email: {sendEmailResponse.Message}");
		return;
	}
	
	MarkAsReadResponse markAsReadResponse = await mediator.Send(
		new MarkAsReadRequest
		{
			Account = account,
			Emails = getAmazonOrderEmails.Result.Keys.ToList()
		}
	);

	if (markAsReadResponse.Status == Status.Failed)
	{
		Console.WriteLine($"Failed to mark emails as read: {markAsReadResponse.Message}");
	}

	Console.WriteLine(getSummaryEmailForAmazon.Result);
}
*/