namespace MailButler.UseCases.EmailsMatchAgainstRule;

public sealed class OrRules : Rules, IRules
{
	public OrRules(IRules parent, params IRule[] rules) : base(rules)
	{
		Predecessor = parent;
		Operator = Operator.Or;
	}
}