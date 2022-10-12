namespace MailButler.UseCases.Components.CheckConnections;

public class ConnectionStatus
{
	public bool Works { get; init; }
	public string? Error { get; init; }
}