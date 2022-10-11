using System.ComponentModel.DataAnnotations;

namespace MailButler.DataAccess.Database.MsSql.Entities;

public class Account
{
	[Key]
	public Guid Id { get; init; }

	[MaxLength(100)]
	public string Username { get; init; } = string.Empty;
	[MaxLength(200)]
	public string? Password { get; init; }
	[MaxLength(200)]
	public string? ImapServer { get; init; }
	public int ImapPort { get; init; } = 0;
	[MaxLength(500)]
	public string? ClientId { get; init; }
	[MaxLength(500)]
	public string? ClientSecret { get; init; }
	public AccountType Type { get; init; } = AccountType.None;
	[MaxLength(200)]
	public string? SmtpServer { get; init; }
	public int SmtpPort { get; init; } = 0;
	[MaxLength(200)]
	public string Name { get; init; } = string.Empty;
	[MaxLength(1000)]
	public string Description { get; init; } = string.Empty;
}