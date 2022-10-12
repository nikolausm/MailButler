using System.Text.RegularExpressions;
using Extensions.Dictionary;
using MailButler.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MailButler.UseCases.Components.Amazon.GetAmazonOrderEmails;

public sealed class GetAmazonOrderEmailsHandler : IRequestHandler<GetAmazonOrderEmailsRequest, GetAmazonOrderEmailsResponse>
{
	private const string AmazonOrderPattern = "\\d{3}-\\d{7}-\\d{7}";
	private const string AmazonEmailFilter = "@amazon.";
	private readonly ILogger<GetAmazonOrderEmailsHandler> _logger;

	public GetAmazonOrderEmailsHandler(
		ILogger<GetAmazonOrderEmailsHandler> logger
	)
	{
		_logger = logger;
	}

	public Task<GetAmazonOrderEmailsResponse> Handle(
		GetAmazonOrderEmailsRequest request,
		CancellationToken cancellationToken
	)
	{
		Dictionary<Email, List<string>> orders = new();

		try
		{
			_logger.LogTrace("Total messages: {TotalMessageCount}", request.Emails.Count);

			request.Emails
				.ForEach(message => AddOrderInformation(message, orders));

			return Task.FromResult(
				new GetAmazonOrderEmailsResponse
				{
					Result = orders
				}
			);
		}
		catch (Exception exception)
		{
			if (_logger.IsEnabled(LogLevel.Error))
				_logger.LogError(
					exception,
					"Failed to process Amazon Order Emails from {Request}",
					request.ToDictionary()
				);

			return Task.FromResult(
				new GetAmazonOrderEmailsResponse
				{
					Status = Status.Failed,
					Message = exception.Message,
					Result = orders
				}
			);
		}
	}

	private static void AddOrderInformation(Email message, Dictionary<Email, List<string>> orders)
	{
		// Get Order Numbers
		foreach (var match in Regex.Matches(TextValues(message), AmazonOrderPattern).Distinct())
		{
			if (!orders.ContainsKey(message)) orders.Add(message, new List<string>());

			if (!orders[message].Contains(match.Value))
			{
				orders[message].Add(match.Value);
			}
		}
	}

	private static string TextValues(Email message)
	{
		return (message.TextBody ?? "") + " - " + (message.HtmlBody ?? " ") + " - " + message.Subject;
	}
}