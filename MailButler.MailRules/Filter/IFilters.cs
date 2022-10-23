namespace MailButler.MailRules.Filter;

public interface IFilters : IReadOnlyList<IFilter>, IChainingOperation, ILocicalOperator
{
	IFilters? Predecessor { get; init; }
	
}