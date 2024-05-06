using MailButler.Dtos;
using MailButler.UseCases.Components.EmailsSummary;
using MailButler.UseCases.Components.MarkAsRead;
using MailButler.UseCases.Components.SearchEmails;
using MailButler.UseCases.Components.SendEmail;
using MailKit.Search;
using Mediator;
using Microsoft.Extensions.Logging;

namespace MailButler.UseCases.Solutions.MarkOldEmailsAsRead;

public sealed class MarkOldEmailsAsReadAction
{
	private readonly ILogger<MarkOldEmailsAsReadAction> _logger;
	private readonly IMediator _mediator;

	public MarkOldEmailsAsReadAction(
		IMediator mediator,
		ILogger<MarkOldEmailsAsReadAction> logger
	)
	{
		_mediator = mediator;
		_logger = logger;
	}

	public async Task ExecuteAsync(MarkOldEmailsAsReadRequest request, CancellationToken cancellationToken)
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
			"Found {EmailCount} emails to be read",
			tasks.Sum(t => t.Result.Result.Count)
		);
		
		foreach (Email email in tasks.SelectMany(task => task.Result.Result))
			_logger.LogDebug("Email: {Email}", email);

		#endregion

		#region Marking emails as read

		var markAsRead = request.Accounts.Select(account =>
				_mediator.Send(
					new MarkAsReadRequest
					{
						Account = account,
						Emails = tasks.SelectMany(task => task.Result.Result).ToList()
					}
					, cancellationToken)
			).Select(t => t.AsTask())
			.ToList();

		await Task.WhenAll(markAsRead);

		_logger.LogInformation("Marked emails as read");

		#endregion

		#region Send Summary Email

		// Send summary email

		var summaryEmail = await _mediator.Send(new EmailsSummaryRequest
			{
				Subject = "MailButler: Marked old emails as read",
				Emails = tasks.SelectMany(task => task.Result.Result).ToList()
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


	private static SearchQuery BuildSearchQuery(MarkOldEmailsAsReadRequest request)
	{
		SearchQuery dateSearch = new DateSearchQuery(SearchTerm.SentSince, DateTime.Now.AddDays(-request.DaysToCheck));
		dateSearch.And(new DateSearchQuery(SearchTerm.SentBefore, DateTime.Now - request.TimeSpan));

		SearchQuery? emails = null;
		foreach (var email in request.SenderAddresses)
			emails = emails is null
				? SearchQuery.FromContains(email)
				: emails.Or(SearchQuery.FromContains(email));
		
		return dateSearch
			.And(SearchQuery.NotSeen)
			.And(emails);
	}
}