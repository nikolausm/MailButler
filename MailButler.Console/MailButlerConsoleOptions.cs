using MailButler.Dtos;
using MailButler.Options;

namespace MailButler.Console;


public sealed class MailButlerConsoleOptions
{
	public Account SmtpAccount { get; init; } = new();
	public AzureJsonOptions AzureJson { get; init; } = new();
	public List<Account> Accounts { get; init; } = new();
	public AmazonOrderSummaryActionOptions AmazonOrderSummaryAction { get; init; } = new();
	public ForwardToGetMyInvoicesOptions ForwardToGetMyInvoices { get; init; } = new();
	public DeleteFromKnownSenderOptions DeleteFromKnownSender { get; init; } = new();
	public MarkOldEmailsAsReadOptions MarkOldEmailsAsRead { get; init; } = new();
}