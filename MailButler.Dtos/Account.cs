namespace MailButler.Dtos;

public sealed record Account
{
	public Guid Id { get; init; }
	public string Username { get; init; } = string.Empty;
	public string? Password { get; init; }
	public string? ImapServer { get; init; }
	public int ImapPort { get; init; } = 0;
	public string? ClientId { get; init; }
	public string? ClientSecret { get; init; }
	public AccountType Type { get; init; } = AccountType.None;
	public string? SmtpServer { get; init; }
	public int SmtpPort { get; init; } = 0;
	public string Name { get; init; } = string.Empty;

	public override string ToString()
	{
		return $"{{Id: {Id}, Name: {Name}}}";
	}
}