namespace MailButler.Dtos;

public record Recipient
{
	public string Name { get; init; } = String.Empty;
	public string Address { get; init; } = String.Empty;
}
