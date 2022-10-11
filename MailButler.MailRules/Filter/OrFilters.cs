namespace MailButler.MailRules.Filter;

internal sealed class OrFilters : Filters, IFilters
{
	public OrFilters(IFilters parent, params IFilter[] filters) : base(filters)
	{
		Predecessor = parent;
		LogicalOperator = LogicalOperator.Or;
	}
}