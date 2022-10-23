using MailButler.Dtos;
using MailButler.MailRules.Filter;
using MailButler.UseCases.Components.CheckConnections;
using MailButler.UseCases.Components.EmailsMatchAgainstRule;
using MailButler.UseCases.Components.FetchEmails;
using MailButler.UseCases.Components.ForwardEmails;
using MailButler.UseCases.Components.MarkAsRead;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MailButler.UseCases.Solutions.ForwardToGetMyInvoices;

public sealed class ForwardToGetMyInvoicesAction
{
	private readonly IMediator _mediator;
	private readonly ILogger<ForwardToGetMyInvoicesAction> _logger;

	public ForwardToGetMyInvoicesAction(
		IMediator mediator,
		ILogger<ForwardToGetMyInvoicesAction> logger
	)
	{
		_mediator = mediator;
		_logger = logger;
	}

	public async Task ExecuteAsync(ForwardToGetMyInvoicesRequest request, CancellationToken cancellationToken)
	{
		#region Check connection

		var checkConnectionsResponse = await _mediator.Send(
			new CheckConnectionsRequest
			{
				Accounts = request.Accounts
			}, cancellationToken
		);

		if (checkConnectionsResponse.Status == Status.Failed)
		{
			_logger.LogError("Failed to get connections {Message}", checkConnectionsResponse.Message);
			return;
		}

		#endregion

		#region Fetch emails from accounts

		List<Task<(List<Email> Emails, Account Account)>> items = checkConnectionsResponse
			.Result
			.Keys
			.Select(
				account => Task.Run(
					async () => (
						(await _mediator.Send(
								new FetchEmailsRequest
								{
									StartDate = request.DateTime.AddDays(-Math.Abs(request.DaysToCheck)),
									Account = account
								}, cancellationToken
							)
						).Result, account
					),
					cancellationToken
				)
			).ToList();

		await Task.WhenAll(items);

		items.ForEach(
			finishedTask =>
			{
				_logger.LogInformation(
					"Add {AmountOfEmails} emails from {AccountId}",
					finishedTask.Result.Emails.Count,
					finishedTask.Result.Account.Name
				);
			}
		);

		#endregion

		#region Filter ionos emails

		var emailMatchAgainstRuleResponse = await _mediator.Send(
			new EmailsMatchAgainstRuleRequest
			{
				Emails = items.SelectMany(resultOfTask => resultOfTask.Result.Emails).ToList(),
				Filter = new IonosInvoiceFilter(request.IonosAccountId)
			}, cancellationToken);
		if (emailMatchAgainstRuleResponse.Status == Status.Failed)
		{
			_logger.LogError("Failed to get amazon order emails: {Message}", emailMatchAgainstRuleResponse.Message);
			return;
		}

		#endregion

		if (emailMatchAgainstRuleResponse.Result.Count == 0)
		{
			_logger.LogInformation("No emails found");
			return;
		}

		#region Forward emails

		var forwardEmailsResponse = await _mediator.Send(
			new ForwardEmailsRequest
			{
				Emails = emailMatchAgainstRuleResponse.Result,
				Accounts = request.Accounts,
				SmtpAccount = request.SmtpAccount,
				Recipients = request.Recipients
			}, cancellationToken
		);

		if (forwardEmailsResponse.Status == Status.Failed)
		{
			_logger.LogError(
				"Failed to forward emails: {Message}",
				forwardEmailsResponse.Message
			);
			return;
		}

		#endregion

		_logger.LogInformation(
			"Forwarded {AmountOfEmails} emails",
			forwardEmailsResponse.Result.Count
		);

		if (request.MarkEmailAsRead)
			await MarkEmailsAsRead(forwardEmailsResponse.Result, request);

		_logger.LogInformation("Result: {Result}", forwardEmailsResponse.Result);
	}

	private async Task MarkEmailsAsRead(List<Email> getAmazonOrderEmailsResponse, ForwardToGetMyInvoicesRequest request)
	{
		foreach (var accountId in getAmazonOrderEmailsResponse.Select(d => d.AccountId).Distinct())
		{
			var markAsReadResponse = await _mediator.Send(
				new MarkAsReadRequest
				{
					Account = request.Accounts.Single(a => a.Id == accountId),
					Emails = getAmazonOrderEmailsResponse
				}
			);

			if (markAsReadResponse.Status == Status.Failed)
				_logger.LogError("Failed to mark emails as read: {Message}", markAsReadResponse.Message);
		}
	}
}