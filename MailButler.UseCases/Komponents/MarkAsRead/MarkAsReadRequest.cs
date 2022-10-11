using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Komponents.MarkAsRead;

public sealed class MarkAsReadRequest : IRequest<MarkAsReadResponse>
{
	public Account Account { get; init; } = new();
	public List<Email> Emails { get; init; } = new();
}