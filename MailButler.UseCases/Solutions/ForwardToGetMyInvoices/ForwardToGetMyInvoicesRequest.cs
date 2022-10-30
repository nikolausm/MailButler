using MailButler.Dtos;

namespace MailButler.UseCases.Solutions.ForwardToGetMyInvoices;

public sealed class ForwardToGetMyInvoicesRequest
{
	public DateTime DateTime { get; init; } = DateTime.Now.AddDays(2);
	public int DaysToCheck { get; init; } = 2;

	public string IonosAccountId { get; init; } = default!;
	public Account SmtpAccount { get; init; } = new();
	public List<Recipient> Recipients { get; init; } = new();
	public List<Account> Accounts { get; init; } = new();
	public bool MarkEmailAsRead { get; init; } = true;
}