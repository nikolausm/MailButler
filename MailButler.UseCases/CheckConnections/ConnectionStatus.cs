namespace MailButler.UseCases.CheckConnections;

public class ConnectionStatus
{
	public bool Works { get; init; }
	public string? Error { get; init; }
}