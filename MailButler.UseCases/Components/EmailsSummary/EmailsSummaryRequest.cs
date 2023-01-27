using MailButler.Dtos;
using Mediator;

namespace MailButler.UseCases.Components.EmailsSummary;

public sealed class EmailsSummaryRequest: IRequest<EmailsSummaryResponse>
{
	public List<Email> Emails { get; init; } = new();
	public string Subject { get; set; } = "MailButler: Spamfilter Summary";
}