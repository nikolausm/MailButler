using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.FetchEmails;

public sealed class FetchEmailsRequest : IRequest<FetchEmailsResponse>
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public DateTime StartDate { get; init; } = DateTime.Now;
	public Account Account { get; init; } = null!;
}