namespace MailButler.UseCases.EmailsMatchAgainstRule;

public record AndRule : Rule, IRule
{
	public AndRule(Field field, FilterType filterType, string value) : base(field, filterType, value, Operator.And)
	{
	}
}