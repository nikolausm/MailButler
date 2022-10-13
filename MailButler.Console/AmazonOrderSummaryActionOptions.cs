using MailButler.Dtos;

namespace MailButler.Console;

public sealed class AmazonOrderSummaryActionOptions
{
	public DateTime LastRun { get; init; }
	public bool MarkEmailAsRead { get; init; }
	public bool EvenIfAllEmailsAreRead { get; init; }
	public Account SmtpAccount { get; init; } = new();
	public int DaysToCheck { get; init; } = 7;
}