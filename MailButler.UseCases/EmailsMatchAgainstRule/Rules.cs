namespace MailButler.UseCases.EmailsMatchAgainstRule;

public abstract class Rules : List<IRule>, IRules
{
	public Rules(params IRule[] rules)
	{
		AddRange(rules);
	}

	public Field Field { get; init; }
	public Operator Operator { get; init; }
	
	public IRules And(Field field, FilterType filterType, string value)
	{
		return new AndRules(this, new Rule(field, filterType, value, Operator.And));
	}
	
	public IRules Or(Field field, FilterType filterType, string value)
	{
		return new OrRules(this, new Rule(field, filterType, value, Operator.Or));
	}

	public IRules And(params IRule[] rules)
	{
		return new AndRules(this, rules);
	}

	public IRules Or(params IRule[] rules)
	{
		return new OrRules(this, rules);
	}

	public IRules? Predecessor { get; init; }
}