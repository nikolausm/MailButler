namespace MailButler.MailRules.Filter;

public sealed record HasAttachmentFilter : Filter
{
	public HasAttachmentFilter()
	{
		LogicalOperator = LogicalOperator.And;
	}
}