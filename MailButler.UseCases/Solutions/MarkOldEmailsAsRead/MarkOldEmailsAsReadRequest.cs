using MailButler.Dtos;

namespace MailButler.UseCases.Solutions.Spamfilter;

public sealed class MarkOldEmailsAsReadRequest
{
	public List<string> SenderAddresses { get; init; } = new();
	public List<Account> Accounts { get; init; } = new();
	public Account SmtpAccount { get; init; } = new();
	public int DaysToCheck { get; init; } = 60;
	public TimeSpan TimeSpan { get; init; } = TimeSpan.FromDays(3);
}