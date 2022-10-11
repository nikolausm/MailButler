using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Komponents.FetchEmails;

public sealed class FetchEmailsRequest : IRequest<FetchEmailsResponse>
{
	public Guid Id { get; } = Guid.NewGuid();
	public DateTime StartDate { get; init; } = DateTime.Now.AddDays(-14);
	public Account Account { get; init; } = null!;
}