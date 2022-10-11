using MailButler.Dtos;

namespace MailButler.UseCases.Komponents.CheckConnections;

public class CheckConnectionsResponse : BaseResponse<Dictionary<Account, ConnectionStatus>>
{
}