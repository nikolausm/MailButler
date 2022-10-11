using System.ComponentModel.DataAnnotations;

namespace MailButler.DataAccess.Database.MsSql.Entities;

public sealed class Filter
{
	[Key]
	public Guid Id { get; init; }
	public Field Field { get; init; } = Field.Unknown;
	public LogicalOperator LogicalOperator { get; init; } = LogicalOperator.None;
	[MaxLength(2000)]
	public string Value { get; init; } = "";
	public FilterType FilterType { get; init; } = FilterType.Unknown;
}