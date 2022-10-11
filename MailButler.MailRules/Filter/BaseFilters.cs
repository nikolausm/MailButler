namespace MailButler.MailRules.Filter;

public sealed class BaseFilters : Filters, IFilters
{
	public BaseFilters(params IFilter[] filters) : base(filters)
	{
		LogicalOperator = LogicalOperator.None;
	}
}