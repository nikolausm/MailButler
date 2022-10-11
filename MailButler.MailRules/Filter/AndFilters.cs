namespace MailButler.MailRules.Filter;

internal sealed class AndFilters : Filters, IFilters
{
	public AndFilters(IFilters parent, params IFilter[] filters) : base(filters)
	{
		Predecessor = parent;
		LogicalOperator = LogicalOperator.And;
	}
}