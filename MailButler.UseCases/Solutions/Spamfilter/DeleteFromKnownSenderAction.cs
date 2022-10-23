using MailButler.UseCases.Components.DeleteEmails;
using MailButler.UseCases.Components.SearchEmails;
using MailKit.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MailButler.UseCases.Solutions.Spamfilter;

public sealed class DeleteFromKnownSenderAction
{
	private readonly IMediator _mediator;
	private readonly ILogger<DeleteFromKnownSenderAction> _logger;

	public DeleteFromKnownSenderAction(IMediator mediator, ILogger<DeleteFromKnownSenderAction> logger)

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
		).ToList();

		await Task.WhenAll(tasks);

		_logger.LogInformation(
			"Found {EmailCount} emails to delete",
			tasks.Sum(t => t.Result.Result.Count)
		);

		foreach (var email in tasks.SelectMany(task => task.Result.Result))
		{
			_logger.LogDebug("Deleting email: {Email}", email);
		}

		#endregion

		#region Deleting emails
		if (request.DeleteEmails)
		{
			var deleteTasks = request.Accounts.Select(account =>
				_mediator.Send(new DeleteEmailsRequest
				{
					Account = account,
					Emails = tasks
						.SelectMany(t => t.Result.Result)
						.Where(e => e.AccountId == account.Id).ToList()
				}, cancellationToken)).ToList();

			await Task.WhenAll(deleteTasks);

			_logger.LogInformation("Deleted emails");
		}
		#endregion
	}

	private static SearchQuery BuildSearchQuery(DeleteFromKnownSenderRequest request)
	{
		SearchQuery dateSearch = new DateSearchQuery(SearchTerm.SentSince, DateTime.Now.AddDays(-request.DaysToCheck));
		SearchQuery? emails = null;
		foreach (var email in request.SenderAddresses)
		{
			emails = emails is null
				? SearchQuery.FromContains(email)
				: emails.Or(SearchQuery.FromContains(email));
		}

		return dateSearch.And(emails);
	}
}