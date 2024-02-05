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
			if (!origin.HasFlag((MessageSummaryItems)value)) continue;

			// If the value is selected, add it to the result
			yield return value switch
			{
				MessageSummaryItems.UniqueId => ItemSchema.Id,
				MessageSummaryItems.Envelope => ItemSchema.InternetMessageHeaders,
				MessageSummaryItems.BodyStructure => ItemSchema.MimeContent,
				MessageSummaryItems.InternalDate => ItemSchema.DateTimeReceived,
				MessageSummaryItems.Size => ItemSchema.Size,
				MessageSummaryItems.Flags => ItemSchema.Flag,
				MessageSummaryItems.GMailMessageId => EmailMessageSchema.InternetMessageId,
				MessageSummaryItems.GMailThreadId => EmailMessageSchema.InternetMessageId,
				MessageSummaryItems.GMailLabels => ItemSchema.Categories,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}
	}
}