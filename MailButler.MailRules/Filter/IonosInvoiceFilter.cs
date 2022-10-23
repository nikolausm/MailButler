using System.Collections;

namespace MailButler.MailRules.Filter;

public sealed record IonosInvoiceFilter() : IFilters
{
	private readonly IFilters _filter;
	private readonly IFilters? _predecessor;

	public IonosInvoiceFilter(string ionosAccountId)
		: this()
	{
		_filter = new UnreadFilter()
			.And(new HasAttachmentFilter())
			.And(
				Field.SenderAddress,
				FilterType.Equals,
				"noreply@ionos.de"
			).And(Field.Subject, FilterType.StartsWith, "Ihre Rechnung")
			.And(new HasAttachmentFilter())
			.And(Field.Subject, FilterType.EndsWith, $"für den Vertrag {ionosAccountId}");
		_predecessor = _filter.Predecessor;
	}

	public IEnumerator<IFilter> GetEnumerator()
	{
		return _filter.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_filter).GetEnumerator();
	}

	public int Count => _filter.Count;

	public IFilter this[int index] => _filter[index];

	public IFilters And(IFilters filters)
	{
		return _filter.And(filters);
	}

	public IFilters Or(IFilters filters)
	{
		return _filter.Or(filters);
	}

	public IFilters And(params IFilter[] rules)
	{
		return _filter.And(rules);
	}

	public IFilters Or(params IFilter[] rules)
	{
		return _filter.Or(rules);
	}

	public IFilters Or(Field field, FilterType filterType, string value)
	{
		return _filter.Or(field, filterType, value);
	}

	public IFilters And(Field field, FilterType filterType, string value)
	{
		return _filter.And(field, filterType, value);
	}

	public LogicalOperator LogicalOperator => _filter.LogicalOperator;

	public IFilters? Predecessor
	{
		get => _predecessor;
		init => _predecessor = value;
	}
}