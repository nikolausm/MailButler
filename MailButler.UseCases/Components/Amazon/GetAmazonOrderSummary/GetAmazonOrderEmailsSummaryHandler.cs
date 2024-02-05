using System.Text;
using MailButler.Dtos;
using MailButler.UseCases.Components.Amazon.GetAmazonOrderSummary.Extensions;
using Mediator;

namespace MailButler.UseCases.Components.Amazon.GetAmazonOrderSummary;

public class GetAmazonOrderEmailsSummaryHandler : IRequestHandler<GetAmazonOrderEmailsSummaryRequest,
	GetAmazonOrderEmailsSummaryResponse>
{
	private const string EmailId = "edae6e37-91c6-4103-b779-ad800107ad1c";
	private readonly EmailBodyParts _emailBodyParts;

	private IList<Account> _accounts = new List<Account>();


	public GetAmazonOrderEmailsSummaryHandler(EmailBodyParts emailBodyParts)
	{
		_emailBodyParts = emailBodyParts;
	}

	public ValueTask<GetAmazonOrderEmailsSummaryResponse> Handle(
		GetAmazonOrderEmailsSummaryRequest request,
		CancellationToken cancellationToken)
	{
		_accounts = request.Accounts;
		if (request.EmailsWithOrders.Keys.Count == 0)
			return ValueTask.FromResult(new GetAmazonOrderEmailsSummaryResponse
			{
				Result = new Email
				{
					Subject = "Summary of Amazon Orders",
					TextBody = "No emails found with Amazon orders",
					HtmlBody = _emailBodyParts.HtmlLogo() + "<p>No emails found with Amazon orders</p>"
				}
			});

		if (request.EmailsWithOrders.Keys.All(email => email.IsRead))
			return ValueTask.FromResult(
				new GetAmazonOrderEmailsSummaryResponse
				{
					Result = new Email
					{
						Sender = new MailBoxAddress
						{
							Name = "MailButler",
							Address = "mailbutler@minicon.eu"
						},
						Subject =
							$"Summary of Amazon Orders Since: {request.EmailsWithOrders.Min(email => email.Key.Sent):yyyy-MM-dd}",
						TextBody = "All emails are already read",
						HtmlBody = _emailBodyParts.HtmlLogo() + "<p>No new emails with Amazon orders</p>"
					}
				}
			);

		return ValueTask.FromResult(
			new GetAmazonOrderEmailsSummaryResponse
			{
				Result = SummaryEmail(request.EmailsWithOrders)
			}
		);
	}

	private Email SummaryEmail(Dictionary<Email, List<string>> emailsWithOrders)
	{
		return new Email
		{
			Sender = new MailBoxAddress
			{
				Name = "MailButler",
				Address = "mailbutler@minicon.eu"
			},
			TextBody = TextBody(emailsWithOrders),
			HtmlBody = HtmlBody(emailsWithOrders),
			Subject =
				$"Summary of Amazon Orders OrdersEmailsIfAnyUnread since: {emailsWithOrders.Min(email => email.Key.Sent):yyyy-MM-dd}"
		};
	}


	private string HtmlBody(Dictionary<Email, List<string>> emailsWithOrders)
	{
		StringBuilder htmlBody = new(_emailBodyParts.HtmlStyle());
		htmlBody.AppendLine(_emailBodyParts.HtmlLogo());

		var accounts = _accounts.ToDictionary(e => e.Id, account => account);

		AppendSellerEmailsHtmlSummary(emailsWithOrders, htmlBody);
		AppendOrderEmailsHtmlSummary(emailsWithOrders, htmlBody, accounts);

		htmlBody.AppendLine("<p>Mit freundlichen Grüßen</p>");
		htmlBody.AppendLine("<p>MailButler</p>");
		htmlBody.AppendLine($"<!-- Id: {EmailId} -->");
		return htmlBody.ToString();
	}

	private static void AppendSellerEmailsHtmlSummary(
		Dictionary<Email, List<string>> emailsWithOrders,
		StringBuilder htmlBody
	)
	{
		var sellerEmails = emailsWithOrders.UnreadSellerEmails();
		if (!sellerEmails.Any())
			return;

		htmlBody.AppendLine("<h2>Sold on Amazon</h2>");
		foreach (var sellerEmail in sellerEmails
			         .GroupBy(e => e.Key.Subject))
		{
			htmlBody.AppendLine("<h3>");
			htmlBody.Append($"{sellerEmail.Key}");
			htmlBody.AppendLine("</h3>");

			if (sellerEmail.Count() > 1) htmlBody.AppendLine("<ol>");

			htmlBody.AppendJoin(
				"\r\n",
				sellerEmail
					.SelectMany(item => item.Value.Select(order => (item.Key.Sent, order)))
					.Select(item => $" <li>{item.Sent:yyyy-MM-dd}: {item.order}</li>")
			);

			if (sellerEmail.Count() > 1) htmlBody.AppendLine("</ol>");
		}

		htmlBody.AppendLine();
	}

	private static void AppendOrderEmailsHtmlSummary(
		Dictionary<Email, List<string>> emailsWithOrders,
		StringBuilder htmlBody,
		Dictionary<Guid, Account> accounts
	)
	{
		var ordersEmails = emailsWithOrders.Orders().ToList();

		if (!ordersEmails.Any())
			return;

		var titleWritten = false;

		foreach (var order in ordersEmails)
		{
			var orderEmails = emailsWithOrders.OrdersEmailsIfAnyUnread(order);
			if (!orderEmails.Any()) continue;

			if (!titleWritten)
			{
				htmlBody.AppendLine($"<h2>Orders: {emailsWithOrders.CountOrdersWithUnreadEmails()}</h2>");
				titleWritten = true;
			}

			htmlBody.AppendLine(
				$"<h3><a href=\"https://www.amazon.de/gp/your-account/order-details/ref=ppx_yo_dt_b_order_details_o00?ie=UTF8&orderID={order}\">{order}</a><span>({accounts[orderEmails.First().Key.AccountId].Name})</span>:</h3>");
			if (orderEmails.Count > 1) htmlBody.AppendLine("<ol>");

			htmlBody.AppendJoin(
				"\r\n",
				orderEmails
					.Select(
						email
							=>
							$"<li>{(email.Key.IsRead ? "" : "*")}{email.Key.Sent:yyyy-MM-dd}: {email.Key.Subject}</li>"
					)
			);

			if (orderEmails.Count > 1) htmlBody.AppendLine("</ol>");
		}
	}

	private string TextBody(Dictionary<Email, List<string>> emailsWithOrders)
	{
		StringBuilder textBody = new();
		var accounts = _accounts.ToDictionary(e => e.Id, account => account);

		AppendSellerEmailsTextSummary(emailsWithOrders, textBody);
		AppendOrderEmailsTextSummary(emailsWithOrders, textBody, accounts);

		textBody.AppendLine("Mit freundlichen Grüßen");
		textBody.AppendLine("MailButler");

		textBody.AppendLine("r\n\r\n\r\n\r\n");
		textBody.AppendLine($"Id: {EmailId}");
		return textBody.ToString();
	}

	private static void AppendOrderEmailsTextSummary(
		Dictionary<Email, List<string>> emailsWithOrders,
		StringBuilder textBody,
		Dictionary<Guid, Account> accounts
	)
	{
		if (!emailsWithOrders.Any())
			return;

		var titleWritten = false;
		foreach (var order in emailsWithOrders.Orders())
		{
			HashSet<string> lines = new();
			var orderEmails = emailsWithOrders.OrdersEmailsIfAnyUnread(order);
			if (!orderEmails.Any()) continue;

			if (!titleWritten)
			{
				textBody.AppendLine("Orders: ");
				titleWritten = true;
			}

			textBody.AppendLine($"{order} ({accounts[orderEmails.First().Key.AccountId].Name}):");

			foreach (var email in orderEmails)
				lines.Add($"- {(email.Key.IsRead ? "" : "*")}{email.Key.Sent:yyyy-MM-dd}: {email.Key.Subject}");

			textBody.AppendJoin("\r\n", lines);
			textBody.AppendLine();
		}
	}

	private static void AppendSellerEmailsTextSummary(
		Dictionary<Email, List<string>> emailsWithOrders,
		StringBuilder textBody
	)
	{
		var sellerEmails = emailsWithOrders.UnreadSellerEmails();
		if (sellerEmails.Any(e => !e.Key.IsRead))
		{
			var ordersInSellerEmails = sellerEmails.Sum(ite => ite.Value.Count);
			textBody.AppendLine($"Sold on Amazon: {ordersInSellerEmails}");
			foreach (var sellerEmail in sellerEmails.GroupBy(e => e.Key.Subject))
			{
				textBody.AppendLine($" {sellerEmail.Key}: {sellerEmail.Sum(ite => ite.Value.Count)}");
				textBody.AppendJoin(
					"\r\n",
					sellerEmail
						.SelectMany(item => item.Value)
						.Select(item => $" - {item}")
				);
			}

			textBody.AppendLine();
		}
	}
}