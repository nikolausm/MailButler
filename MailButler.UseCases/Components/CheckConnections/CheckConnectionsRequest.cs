using MailButler.Dtos;
using Mediator;

namespace MailButler.UseCases.Components.CheckConnections;

public sealed class CheckConnectionsRequest : IRequest<CheckConnectionsResponse>
{
	public List<Account> Accounts { get; init; } = new();
}