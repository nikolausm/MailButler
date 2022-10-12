using MailButler.Dtos;
using MailButler.MailRules.Filter;
using MailButler.UseCases.Components.Amazon.GetAmazonOrderEmails;
using MailButler.UseCases.Components.Amazon.GetAmazonOrderSummary;
using MailButler.UseCases.Components.CheckConnections;
using MailButler.UseCases.Components.EmailsMatchAgainstRule;
using MailButler.UseCases.Components.FetchEmails;
using MailButler.UseCases.Components.MarkAsRead;
using MailButler.UseCases.Components.SendEmail;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;

public sealed class AmazonOrderSummaryAction
{
	private readonly IMediator _mediator;
	private readonly IList<Account> _accounts;
	private readonly ILogger<AmazonOrderSummaryAction> _logger;

	public AmazonOrderSummaryAction(
		IMediator mediator,
		IList<Account> accounts,
		ILogger<AmazonOrderSummaryAction> logger
	)
	{
		_mediator = mediator;
		_accounts = accounts;
		_logger = logger;
	}

	public async Task ExecuteAsync(AmazonOrderSummaryRequest request, CancellationToken cancellationToken)
	{
		var checkConnectionsResponse = await _mediator.Send(
			new CheckConnectionsRequest
			{
				Accounts = _accounts.ToList()
			}, cancellationToken
		);

		if (checkConnectionsResponse.Status == Status.Failed)
		{
			_logger.LogError("Failed to get connections {Message}", checkConnectionsResponse.Message);
			return;
		}

		List<Task<(List<Email> Emails, Account Account)>> items = checkConnectionsResponse
			.Result
			.Keys
			.Select(
				account => Task.Run(
					async () => (
						(await _mediator.Send(
								new FetchEmailsRequest
								{
									Account = account
								}, cancellationToken
							)
						).Result, account
					)
				)
			).ToList();

		await Task.WhenAll(items);


		items.ForEach(
			finishedTask =>
			{
				_logger.LogInformation(
					"Add {AmountOfEmails} emails from {AccountId}",
					finishedTask.Result.Emails.Count,
					finishedTask.Result.Account
				);
			}
		);


		EmailsMatchAgainstRuleResponse emailMatchAgainstRuleResponse = await _mediator.Send(
			new EmailsMatchAgainstRuleRequest
			{
				Emails = items.SelectMany(resultOfTask => resultOfTask.Result.Emails).ToList(),
				Filter = new Filter(
					Field.SenderAddress,
					FilterType.Contains,
					"@amazon."
				).And(
					Field.AnyTextField,
					FilterType.RegularExpression,
					"\\d{3}-\\d{7}-\\d{7}"
				)
			}, cancellationToken);

		GetAmazonOrderEmailsResponse getAmazonOrderEmailsResponse = await _mediator.Send(new GetAmazonOrderEmailsRequest
		{
			Emails = emailMatchAgainstRuleResponse.Result
		}, cancellationToken);

		if (getAmazonOrderEmailsResponse.Status == Status.Failed)
		{
			_logger.LogError("Failed to get amazon order emails: {Message}", getAmazonOrderEmailsResponse.Message);
			return;
		}

		GetAmazonOrderEmailsSummaryResponse getSummaryEmailForAmazon = await _mediator.Send(
			new GetAmazonOrderEmailsSummaryRequest
			{
				EmailsWithOrders = getAmazonOrderEmailsResponse.Result
			}, cancellationToken
		);

		if (getSummaryEmailForAmazon.Status == Status.Failed)
		{
			_logger.LogError(
				"Failed to get summary amazon order emails: {Message}",
				getSummaryEmailForAmazon.Message
			);
			return;
		}
		
		SendEmailResponse sendEmailResponse = await _mediator.Send(
			new SendEmailRequest
			{
				Account = request.SmtpAccount,
				Email = getSummaryEmailForAmazon.Result
			}, cancellationToken
		);

		if (sendEmailResponse.Status == Status.Failed)
		{
			_logger.LogError("Failed to send summary email: {Message}", sendEmailResponse.Message);
			return;
		}

		foreach (var account in getAmazonOrderEmailsResponse.Result.Keys.Select(d => d.AccountId).Distinct())
		{
			MarkAsReadResponse markAsReadResponse = await _mediator.Send(
				new MarkAsReadRequest
				{
					Account = _accounts.Single(a => a.Id == account),
					Emails = getAmazonOrderEmailsResponse.Result.Keys.ToList()
				}
			);

			if (markAsReadResponse.Status == Status.Failed)
			{
				_logger.LogError("Failed to mark emails as read: {Message}", markAsReadResponse.Message);
			}
		}


		_logger.LogInformation("Result: {Result}", getSummaryEmailForAmazon.Result);
	}
}