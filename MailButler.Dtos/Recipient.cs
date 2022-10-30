namespace MailButler.Dtos;

public record Recipient
{
	public string Name { get; init; } = string.Empty;
	public string Address { get; init; } = string.Empty;
}