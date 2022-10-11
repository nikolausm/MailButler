using System.ComponentModel.DataAnnotations;

namespace MailButler.DataAccess.Database.MsSql.Entities;

public class User {
	
	[MaxLength(200)]
	public string Username { get; init; } = string.Empty;
	
	[Key]
	public Guid Id { get; init; }
	
	public virtual List<Account> Accounts { get; init; } = new();
}