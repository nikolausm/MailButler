using MailButler.Dtos;

namespace MailButler.UseCases.Components.CheckConnections;

public class CheckConnectionsResponse : BaseResponse<Dictionary<Account, ConnectionStatus>>
{
}