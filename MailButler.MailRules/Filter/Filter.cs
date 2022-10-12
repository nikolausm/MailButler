using MailButler.Core.Extensions;

namespace MailButler.MailRules.Filter;

public record Filter : IFilter
{
	public Filter()
	{
	}

	public Filter(Field field, FilterType filterType, string value)
		: this(field, filterType, value, LogicalOperator.None)
	{
	}

	internal Filter(Field field, FilterType filterType, string value,
		LogicalOperator logicalOperator = LogicalOperator.None)
	{
		Field = field;
		FilterType = filterType;
		Value = value;
		LogicalOperator = logicalOperator;
	}

	public Field Field { get; init; }
	public LogicalOperator LogicalOperator { get; init; } = LogicalOperator.None;
	public string Value { get; init; } = "";
	public FilterType FilterType { get; init; }

	public IFilters And(IFilters filters)
	{
		return new AndFilters(new BaseFilters(this), filters.ToArray());
	}

	public IFilters Or(IFilters filters)
	{
		return new OrFilters(new BaseFilters(this), filters.ToArray());
	}

	public IFilters And(params IFilter[] rules)
	{
		return new AndFilters(new BaseFilters(this), rules);
	}

	public IFilters Or(params IFilter[] rules)
	{
		return new OrFilters(new BaseFilters(this), rules);
	}

	public IFilters Or(Field field, FilterType filterType, string value)
	{
		return new BaseFilters(this, new OrFilter(field, filterType, value));
	}

	public IFilters And(Field field, FilterType filterType, string value)
	{
		return new BaseFilters(this, new AndFilter(field, filterType, value));
	}

	public override string? ToString()
	{
		return this.ToDictionaryAsString();
	}

	public static IFilters Create(Field field, FilterType filterType, string value)
	{
		return new BaseFilters(new Filter(field, filterType, value, LogicalOperator.And));
	}

	public static IFilters Create(params IFilter[] rules)
	{
		return new BaseFilters(rules);
	}
}