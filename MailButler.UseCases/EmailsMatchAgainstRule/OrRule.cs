namespace MailButler.UseCases.EmailsMatchAgainstRule;

public record OrRule : Rule, IRule
{
	public OrRule(Field field, FilterType filterType, string value) : base(field, filterType, value, Operator.Or)
	{
	}
}