namespace MailButler.Dtos;

public sealed record MailBoxAddress
{
	public string? Name { get; init; } = string.Empty;
	public string? Address { get; init; } = string.Empty;

	public override string ToString()
	{
		return $"{Name} \"{Address}\"";
	}
}