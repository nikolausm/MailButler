using MailButler.Dtos;

namespace MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;

public sealed class AmazonOrderSummaryRequest
{
	public bool MarkEmailAsRead { get; init; }
	public Account SmtpAccount { get; init; } = new ();
	public bool EvenIfAllEmailsAreRead { get; init; } = true;
	public DateTime StartDate { get; init; } = DateTime.Now.AddDays(-7);
}