using MailButler.Dtos;

namespace MailButler.Api;

public sealed class AmazonOrderSummaryActionOptions
{
	public bool MarkEmailAsRead { get; init; }
	public bool EvenIfAllEmailsAreRead { get; init; }
	public Account SmtpAccount { get; init; } = new();
	public int DaysToCheck { get; init; } = 7;
}