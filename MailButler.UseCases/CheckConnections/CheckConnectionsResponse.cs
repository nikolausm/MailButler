using MailButler.Dtos;

namespace MailButler.UseCases.CheckConnections;

public class CheckConnectionsResponse : BaseResponse<Dictionary<Account, ConnectionStatus>>
{
}