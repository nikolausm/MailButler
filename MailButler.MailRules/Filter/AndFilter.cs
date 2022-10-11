namespace MailButler.MailRules.Filter;

internal record AndFilter : Filter, IFilter
{
	public AndFilter(Field field, FilterType filterType, string value) : base(field, filterType, value,
		LogicalOperator.And)
	{
	}
}