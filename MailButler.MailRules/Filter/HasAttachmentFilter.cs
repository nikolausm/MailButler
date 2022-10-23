using System.Runtime.CompilerServices;

namespace MailButler.MailRules.Filter;

public sealed record HasAttachmentFilter : Filter
{
	public HasAttachmentFilter()
	{
		this.LogicalOperator = LogicalOperator.And;
	}
}