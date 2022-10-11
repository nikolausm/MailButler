namespace MailButler.MailRules.Filter;

public interface IFilter : ILocicalOperator, IChainingOperation
{
	Field Field { get; init; }
	string Value { get; init; }
	FilterType FilterType { get; init; }
}