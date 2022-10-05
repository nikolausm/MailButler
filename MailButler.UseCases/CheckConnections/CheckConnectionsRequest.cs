using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.CheckConnections;

public sealed class CheckConnectionsRequest: IRequest<CheckConnectionsResponse>
{
	public List<Account> Accounts { get; init; }
}