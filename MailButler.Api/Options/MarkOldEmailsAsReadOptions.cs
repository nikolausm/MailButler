using MailButler.Dtos;

namespace MailButler.Api.Options;

public sealed class MarkOldEmailsAsReadOptions
{
	public List<string> SenderAddresses { get; init; } = new();
	public Account SmtpAccount { get; init; } = new();
	public int DaysToCheck { get; set; } = 60;
	public TimeSpan TimeSpan { get; set; } = TimeSpan.FromDays(3);
}