namespace MailButler.UseCases.EmailsMatchAgainstRule;

public record Rule : IRule
{
	public static IRules Create(Field field, FilterType filterType, string value)
	{
		return new BaseRules(new Rule(field, filterType, value, Operator.And));
	}

	public Rule(Field field, FilterType filterType, string value)
	: this(field, filterType, value, Operator.None)
	{
	}
	
	internal Rule(Field field, FilterType filterType, string value, Operator @operator = Operator.None)
	{
		
		Field = field;
		FilterType = filterType;
		Value = value;
		Operator = @operator;
	}

	public Field Field { get; init; }
	public Operator Operator { get; init; } = Operator.None;
	public string Value { get; init; } = "";
	public FilterType FilterType { get; init; }
}