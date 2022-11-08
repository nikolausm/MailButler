using MailButler.Dtos;
using Mediator;

namespace MailButler.UseCases.Components.MarkAsRead;

public sealed class MarkAsReadRequest : IRequest<MarkAsReadResponse>
{
	public Account Account { get; init; } = new();
	public List<Email> Emails { get; init; } = new();
}