using MailButler.Dtos;

namespace MailButler.UseCases.Solutions.Spamfilter;

public sealed class DeleteFromKnownSenderRequest
{
	public List<string> SenderAddresses { get; init; } = new();
	public List<Account> Accounts { get; init; } = new();
	public Account SmtpAccount { get; init; } = new();
	public int DaysToCheck { get; init; } = 7;
	public bool DeleteEmails { get; init; } = true;
}