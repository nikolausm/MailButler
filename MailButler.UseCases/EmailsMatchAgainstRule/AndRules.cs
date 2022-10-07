namespace MailButler.UseCases.EmailsMatchAgainstRule;

public sealed class AndRules : Rules, IRules
{
	public AndRules(IRules parent, params IRule[] rules) : base(rules)
	{
		Predecessor = parent;
		Operator = Operator.And;
	}
}