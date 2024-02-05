using MailButler.Dtos;

namespace MailButler.Api.Options;

public sealed class DeleteFromKnownSenderOptions
{
	public List<string> SenderAddresses { get; init; } = new();
	public Account SmtpAccount { get; init; } = new();
	public int DaysToCheck { get; set; } = 7;
}