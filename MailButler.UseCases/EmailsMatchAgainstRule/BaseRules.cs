namespace MailButler.UseCases.EmailsMatchAgainstRule;

public sealed class BaseRules : Rules, IRules
{
	public BaseRules(params IRule[] rules) : base(rules)
	{
		Operator = Operator.None;
	}
}