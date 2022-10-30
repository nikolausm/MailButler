namespace MailButler.MailRules.Filter;

public sealed record UnreadFilter : Filter
{
	public UnreadFilter()
	{
		LogicalOperator = LogicalOperator.And;
	}
}