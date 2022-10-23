using MailButler.Dtos;
using MailKit.Search;
using MediatR;

namespace MailButler.UseCases.Components.SearchEmails;

public sealed class SearchEmailsRequest : IRequest<SearchEmailsResponse>
{
	public Guid Id { get; } = Guid.NewGuid();
	public SearchQuery Query { get; init; } = new();
	public Account Account { get; init; } = null!;
}