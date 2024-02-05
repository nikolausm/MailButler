using MailButler.Dtos;
using MailButler.UseCases.Components.DeleteEmails;
using MailButler.UseCases.Components.EmailsSummary;
using MailButler.UseCases.Components.SearchEmails;
using MailButler.UseCases.Components.SendEmail;
using MailKit.Search;
using Mediator;
using Microsoft.Extensions.Logging;

namespace MailButler.UseCases.Solutions.Spamfilter;

public sealed class DeleteFromKnownSenderAction
{
	private readonly ILogger<DeleteFromKnownSenderAction> _logger;
	private readonly IMediator _mediator;

	public DeleteFromKnownSenderAction(
		IMediator mediator,
		ILogger<DeleteFromKnownSenderAction> logger
	)
	{
		_mediator = mediator;
		_logger = logger;
	}

	public async Task ExecuteAsync(DeleteFromKnownSenderRequest request, CancellationToken cancellationToken)
	{
		#region Getting emails

		var tasks = request.Accounts.Select(
				account => _mediator.Send(
					new SearchEmailsRequest
					{
						Account = account,
						Query = BuildSearchQuery(request)
					},
					cancellationToken
				)
			).Select(t => t.AsTask())
			.ToList();

		await Task.WhenAll(tasks);

		_logger.LogInformation(
			"Found {EmailCount} emails to delete",
			tasks.Sum(t => t.Result.Result.Count)
		);

		foreach (var email in tasks.SelectMany(task => task.Result.Result))
			_logger.LogDebug("Deleting email: {Email}", email);

		#endregion

		#region Deleting emails

		if (request.DeleteEmails)
		{
			var deleteTasks = request.Accounts.Select(account =>
					_mediator.Send(
						new DeleteEmailsRequest
						{
							Account = account,
							Emails = tasks
								.SelectMany(t => t.Result.Result)
								.Where(e => e.AccountId == account.Id).ToList()
						}
						, cancellationToken)
				).Select(t => t.AsTask())
				.ToList();

			await Task.WhenAll(deleteTasks);

			_logger.LogInformation("Deleted emails");
		}

		#endregion

		#region Send Summary Email

		// Send summary email

		var summaryEmail = await _mediator.Send(new EmailsSummaryRequest
			{
				Emails = tasks.SelectMany(task => task.Result.Result).ToList(),
				Subject = "Spamfilter Summary"
			}, cancellationToken
		);

		var sendEmailResponse = await _mediator.Send(
			new SendEmailRequest
			{
				Account = request.SmtpAccount,
				Email = summaryEmail.Result
			}, cancellationToken
		);

		if (sendEmailResponse.Status == Status.Failed)
			_logger.LogError("Failed to send summary email: {Message}", sendEmailResponse.Message);

		#endregion
	}


	private static SearchQuery BuildSearchQuery(DeleteFromKnownSenderRequest request)
	{
		SearchQuery dateSearch = new DateSearchQuery(SearchTerm.SentSince, DateTime.Now.AddDays(-request.DaysToCheck));
		SearchQuery? emails = null;
		foreach (var email in request.SenderAddresses)
			emails = emails is null
				? SearchQuery.FromContains(email)
				: emails.Or(SearchQuery.FromContains(email));

		return dateSearch.And(emails);
	}
}