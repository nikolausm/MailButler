using MailKit;
using Microsoft.Exchange.WebServices.Data;

namespace MailButler.Core.Exchange.Extensions.Webservices.Data;

public static class MessageSummaryItemsExtensions
{
	public static IEnumerable<PropertyDefinitionBase> ToExchangeProperties(this MessageSummaryItems origin)
	{
		// Got to all selected values 
		foreach (var value in Enum.GetValues(typeof(MessageSummaryItems)))
		{
			// If the value is not selected, skip it
			if (!origin.HasFlag((MessageSummaryItems)value))
			{
				continue;
			}
				
			// If the value is selected, add it to the result
			yield return value switch
			{
				MessageSummaryItems.UniqueId => EmailMessageSchema.Id,
				MessageSummaryItems.Envelope => EmailMessageSchema.InternetMessageHeaders,
				MessageSummaryItems.BodyStructure => EmailMessageSchema.MimeContent,
				MessageSummaryItems.InternalDate => EmailMessageSchema.DateTimeReceived,
				MessageSummaryItems.Size => EmailMessageSchema.Size,
				MessageSummaryItems.Flags => EmailMessageSchema.Flag,
				MessageSummaryItems.GMailMessageId => EmailMessageSchema.InternetMessageId,
				MessageSummaryItems.GMailThreadId => EmailMessageSchema.InternetMessageId,
				MessageSummaryItems.GMailLabels => EmailMessageSchema.Categories,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}