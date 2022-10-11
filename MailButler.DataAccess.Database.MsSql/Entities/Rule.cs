using System.ComponentModel.DataAnnotations;

namespace MailButler.DataAccess.Database.MsSql.Entities;

public class Rule
{
	[Key]
	public Guid Id { get; init; }
	[MaxLength(200)]
	public string Name { get; set; } = default!;

	[MaxLength(500)]
	public string Description { get; set; } = default!;
	
	
}
