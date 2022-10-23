using MailButler.Dtos;

namespace MailButler.Api;

public sealed class MailButlerOptions
{
	public List<Account> Accounts { get; init; } = new();
	public AmazonOrderSummaryActionOptions AmazonOrderSummaryAction { get; init; } = new();
	public ForwardToGetMyInvoicesOptions ForwardToGetMyInvoices { get; init; } = new();
	public DeleteFromKnownSenderOptions DeleteFromKnownSender { get; init; } = new();
}