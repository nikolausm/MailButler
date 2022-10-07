using System.Text;
using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Amazon.GetAmazonOrderSummary;

public class
	GetAmazonOrderEmailsSummaryHandler : IRequestHandler<GetAmazonOrderEmailsSummaryRequest,
		GetAmazonOrderEmailsSummaryResponse>
{
	public Task<GetAmazonOrderEmailsSummaryResponse> Handle(GetAmazonOrderEmailsSummaryRequest request,
		CancellationToken cancellationToken)
	{
		if (request.EmailsWithOrders.Keys.Count == 0)
		{
			return Task.FromResult(new GetAmazonOrderEmailsSummaryResponse
			{
				Result = new Email
				{
					Subject = $"Summary of Amazon Orders ",
					TextBody = "No emails found with Amazon orders"
				}
			});
		}

		if (request.EmailsWithOrders.Keys.All(email => email.IsRead))
		{
			return Task.FromResult(
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
						TextBody = "All emails are already read"
					}
				}
			);
		}

		return Task.FromResult(
			new GetAmazonOrderEmailsSummaryResponse
			{
				Result = SummaryEmail(request.EmailsWithOrders)
			}
		);
	}

	private Email SummaryEmail(Dictionary<Email, List<string>> emailsWithOrders)
	{
		StringBuilder textBody = new();
		foreach (var order in emailsWithOrders
			         .OrderByDescending(e => e.Key.Sent)
			         .SelectMany(e => e.Value)
			         .Distinct()
		        )
		{
			textBody.AppendLine(order + ":");
			HashSet<string> lines = new();
			foreach (KeyValuePair<Email, List<string>> email in emailsWithOrders
				         .Where(e => e.Value.Contains(order))
				         .OrderByDescending(e => e.Key.Sent)
			        )
			{
				lines.Add($"- {(email.Key.IsRead ? "*" : "")}{email.Key.Sent:yyyy-MM-dd}: {email.Key.Subject}");
			}

			textBody.AppendJoin("\r\n", lines);
			textBody.AppendLine("\r\n");
		}

		return new Email
		{
			Sender = new MailBoxAddress
			{
				Name = "MailButler",
				Address = "mailbutler@minicon.eu"
			},
			TextBody = textBody.ToString(),
			Subject = $"Summary of Amazon Orders Since: {emailsWithOrders.Min(email => email.Key.Sent):yyyy-MM-dd}"
		};
	}
}