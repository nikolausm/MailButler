namespace MailButler.MailRules.Filter;

internal record OrFilter : Filter, IFilter
{
	public OrFilter(Field field, FilterType filterType, string value) : base(field, filterType, value,
		LogicalOperator.Or)
	{
	}
}