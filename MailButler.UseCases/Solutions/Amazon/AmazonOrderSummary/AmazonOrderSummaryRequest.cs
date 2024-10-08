using MailButler.Dtos;

namespace MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;

public sealed class AmazonOrderSummaryRequest
{
	public bool MarkEmailAsRead { get; init; }
	public Account SmtpAccount { get; init; } = new();
	public bool EvenIfAllEmailsAreRead { get; init; } = false;
	public DateTime DateTime { get; init; } = DateTime.Now.AddDays(-7);
	public int DaysToCheck { get; init; } = 7;
	public List<Account> Accounts { get; init; } = new();
	
}