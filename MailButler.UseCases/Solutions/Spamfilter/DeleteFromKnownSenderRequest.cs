using MailButler.Dtos;

namespace MailButler.UseCases.Solutions.Spamfilter;

public sealed class DeleteFromKnownSenderRequest
{
	public List<string> SenderAddresses { get; init; }
	public List<Account> Accounts { get; init; }
	public Account SmtpAccount { get; init; }
	public int DaysToCheck { get; init; } = 7;
	public bool DeleteEmails { get; init; } = true;
}