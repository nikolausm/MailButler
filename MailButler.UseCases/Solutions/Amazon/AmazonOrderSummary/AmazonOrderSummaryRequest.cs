using MailButler.Dtos;

namespace MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;

public sealed class AmazonOrderSummaryRequest
{
	public Account SmtpAccount { get; init; }
	public bool EvenIfAllEmailsAreRead { get; init; } = true;
}