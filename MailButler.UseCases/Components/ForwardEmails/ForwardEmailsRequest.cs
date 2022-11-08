using MailButler.Dtos;
using Mediator;

namespace MailButler.UseCases.Components.ForwardEmails;

public sealed class ForwardEmailsRequest : IRequest<ForwardEmailsResponse>
{
	public Guid Id { get; } = Guid.NewGuid();
	public List<Recipient> Recipients { get; init; } = default!;
	public Account SmtpAccount { get; init; } = new();
	public IList<Account> Accounts { get; init; } = null!;
	public IList<Email> Emails { get; init; } = null!;
}