using System.Text;
using MailButler.Dtos;
using Mediator;

namespace MailButler.UseCases.Components.EmailsSummary;

public sealed class EmailsSummaryRequestHandler: IRequestHandler<EmailsSummaryRequest, EmailsSummaryResponse>
{
	private readonly EmailBodyParts _emailBodyParts;

	public EmailsSummaryRequestHandler(EmailBodyParts emailBodyParts)
	{
		_emailBodyParts = emailBodyParts;
	}
	public ValueTask<EmailsSummaryResponse> Handle(EmailsSummaryRequest request, CancellationToken cancellationToken)
	{
		try
		{
			return ValueTask.FromResult(new EmailsSummaryResponse
			{
				Result = new Email
				{
					Subject = request.Subject,
					TextBody = CreateTextBody(request.Emails),
					HtmlBody = CreateHtmlBody(request.Emails)
				}
			});
		}
		catch (Exception e)
		{
			return ValueTask.FromResult(new EmailsSummaryResponse
			{
				Message = e.Message,
				Status = Status.Failed
			});
		}
	}

	private string? CreateHtmlBody(List<Email> requestEmails)
	{
		StringBuilder sb = new (_emailBodyParts.HtmlStyle());
		sb.Append(_emailBodyParts.HtmlLogo());

		if (requestEmails.Count == 0)
		{
			sb.Append("<p>No emails to report.</p>");
			return sb.ToString();
		}
		
		sb.Append("<h2>Deleted Emails</h2>");
		sb.Append("<p><li>");
		sb.AppendJoin("</li><li>", requestEmails.Select(email => $"From: {email.Sender.Address} Subject: {email.Subject}"));
		sb.Append("</li></p>");
		
		return sb.ToString();
	}

	private string? CreateTextBody(List<Email> requestEmails)
	{
		StringBuilder sb = new ();
	
		if (requestEmails.Count == 0)
		{
			sb.Append("No emails to report");
			return sb.ToString();
		}
		
		sb.AppendLine("Deleted Emails");
		sb.AppendLine();

		sb.AppendJoin("\r\n", requestEmails.Select(email => $" - From: {email.Sender.Address} Subject: {email.Subject}"));
		return sb.ToString();
	}
}