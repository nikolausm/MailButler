namespace MailButler.Dtos;

public sealed class Account
{
	public Guid Id { get; init; }
	public string Username { get; init; }
	public string? Password { get; init; }
	public string? ImapServer { get; init; }
	public int ImapPort { get; init; } = 0;
	public string? ClientId { get; init; }
	public string? ClientSecret { get; init; }
	public AccountType Type { get; init; } = AccountType.Imap;
}