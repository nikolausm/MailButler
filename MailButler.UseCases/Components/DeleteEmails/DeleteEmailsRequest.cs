using MailButler.Dtos;
using Mediator;

namespace MailButler.UseCases.Components.DeleteEmails;

public sealed class DeleteEmailsRequest : IRequest<DeleteEmailsResponse>
{
	public Account Account { get; init; } = new();
	public List<Email> Emails { get; init; } = new();
}