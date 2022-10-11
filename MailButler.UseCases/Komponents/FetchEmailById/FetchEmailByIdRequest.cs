using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Komponents.FetchEmailById;

public sealed class FetchEmailByIdRequest : IRequest<FetchEmailByIdResponse>
{
	public Guid Id { get; } = Guid.NewGuid();
	public UniqueId EmailId { get; init; } = default;
	public Account Account { get; init; } = null!;
}