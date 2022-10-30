using MailButler.Dtos;

namespace MailButler.Api.Options;

public sealed class ForwardToGetMyInvoicesOptions
{
	public Account SmtpAccount { get; init; } = new();
	public List<Recipient> Recipients { get; init; } = new();
	public string IonosAccountId { get; init; } = string.Empty;
	public int DaysToCheck { get; init; } = 7;
}