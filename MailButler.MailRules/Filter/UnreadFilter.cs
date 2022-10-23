namespace MailButler.MailRules.Filter;

public sealed record UnreadFilter : Filter
{
	public UnreadFilter()
	{
		this.LogicalOperator = LogicalOperator.And;
	}
}