namespace MailButler.MailRules.Filter;

public abstract class Filters : List<IFilter>, IFilters
{
	public Filters(params IFilter[] filters)
	{
		AddRange(filters);
	}

	public LogicalOperator LogicalOperator { get; init; } = LogicalOperator.None;

	public IFilters And(Field field, FilterType filterType, string value)
	{
		return new AndFilters(this, new Filter(field, filterType, value, LogicalOperator.And));
	}

	public IFilters Or(Field field, FilterType filterType, string value)
	{
		return new OrFilters(this, new Filter(field, filterType, value, LogicalOperator.Or));
	}

	public IFilters And(IFilters filters)
	{
		return new AndFilters(this, filters.ToArray());
	}

	public IFilters Or(IFilters filters)
	{
		return new OrFilters(this, filters.ToArray());
	}

	public IFilters And(params IFilter[] rules)
	{
		return new AndFilters(this, rules);
	}

	public IFilters Or(params IFilter[] rules)
	{
		return new OrFilters(this, rules);
	}

	public IFilters? Predecessor { get; init; }
}