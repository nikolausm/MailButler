using MailButler.Dtos;

namespace MailButler.Options;

public sealed class MarkOldEmailsAsReadOptions
{
	public List<string> SenderAddresses { get; init; } = new();
	public Account SmtpAccount { get; init; } = new();
	public TimeSpan TimeSpan { get; set; } = TimeSpan.FromDays(3);
}