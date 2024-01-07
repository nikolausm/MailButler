//using MailKit.Search;

using MailKit.Search;
using Microsoft.Exchange.WebServices.Data;

namespace MailButler.Core.Exchange.Extensions.Webservices.Data;

public static class SearchQueryExtensions
{
	public static SearchFilter ToExchangeSearchFilter(this SearchQuery origin)
	{
		string textValue = null;
		if (origin is TextSearchQuery textSearchQuery)
		{
			textValue = textSearchQuery.Text;
		}

		if (origin is FilterSearchQuery filterSearchQuery)
		{
			textValue = filterSearchQuery.Name;
		}

		if (origin is HeaderSearchQuery headerSearchQuery)
		{
			textValue = headerSearchQuery.Value;
		}

		if (origin is DateSearchQuery dateSearchQuery)
		{
			textValue = dateSearchQuery.Date.ToString("yyyy-MM-dd");
		}
		if (origin is BinarySearchQuery binarySearchQuery)
		{
			return new SearchFilter.SearchFilterCollection(
				binarySearchQuery.Term == SearchTerm.And
					? LogicalOperator.And
					: LogicalOperator.Or,
				binarySearchQuery.Left.ToExchangeSearchFilter(),
				binarySearchQuery.Right.ToExchangeSearchFilter()
			);
		}

		switch (origin.Term)
		{
			case SearchTerm.And when origin is UnarySearchQuery unarySearchQuery:
				return new SearchFilter.SearchFilterCollection(
					LogicalOperator.And,
					unarySearchQuery.Operand.ToExchangeSearchFilter()
				);
			case SearchTerm.Or when origin is UnarySearchQuery unarySearchQuery:
				return new SearchFilter.SearchFilterCollection(
					LogicalOperator.Or,
					unarySearchQuery.Operand.ToExchangeSearchFilter()
				);
			case SearchTerm.All:
				return new SearchFilter.Exists(ItemSchema.Subject);
			case SearchTerm.Seen:
				return new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, true);
			case SearchTerm.NotSeen:
			case SearchTerm.New:
				return new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false);
			case SearchTerm.SubjectContains:
				return new SearchFilter.ContainsSubstring(ItemSchema.Subject, textValue);
			case SearchTerm.BodyContains:
				return new SearchFilter.ContainsSubstring(ItemSchema.Body, textValue);
			case SearchTerm.FromContains:
				return new SearchFilter.ContainsSubstring(EmailMessageSchema.From, textValue);
			case SearchTerm.ToContains:
				return new SearchFilter.ContainsSubstring(EmailMessageSchema.ToRecipients, textValue);
			case SearchTerm.CcContains:
				return new SearchFilter.ContainsSubstring(EmailMessageSchema.CcRecipients, textValue);
			case SearchTerm.BccContains:
				return new SearchFilter.ContainsSubstring(EmailMessageSchema.BccRecipients, textValue);
			case SearchTerm.SentOn:
				return new SearchFilter.IsEqualTo(ItemSchema.DateTimeSent, textValue);
			case SearchTerm.SentBefore:
				return new SearchFilter.IsLessThanOrEqualTo(ItemSchema.DateTimeSent, textValue);
			case SearchTerm.SentSince:
				return new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.DateTimeSent, textValue);
			case SearchTerm.Older:
				return new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.DateTimeReceived, textValue);
			case SearchTerm.Younger:
				return new SearchFilter.IsLessThanOrEqualTo(ItemSchema.DateTimeReceived, textValue);
			case SearchTerm.Uid when origin is UidSearchQuery uidSearchQuery:
				SearchFilter[] items = uidSearchQuery.Uids
					.ToList()
					.ConvertAll(uid =>
						(SearchFilter)new SearchFilter.IsEqualTo(EmailMessageSchema.InternetMessageId, uid))
					.ToArray();
				return new SearchFilter.SearchFilterCollection(LogicalOperator.Or, items);
			default:
				throw new NotSupportedException($"Search term {origin.Term} not supported.");
		}
	}
}