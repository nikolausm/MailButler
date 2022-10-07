using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.MarkAsRead;

public sealed class MarkAsReadRequest : IRequest<MarkAsReadResponse>
{
	public Account Account { get; init; } = new();
	public List<Email> Emails { get; init; } = new();
}