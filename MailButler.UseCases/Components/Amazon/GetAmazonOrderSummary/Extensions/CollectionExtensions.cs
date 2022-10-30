using MailButler.Dtos;

namespace MailButler.UseCases.Components.Amazon.GetAmazonOrderSummary.Extensions;

public static class CollectionExtensions
{
	private static bool IsAmazonSellerEmail(KeyValuePair<Email, List<string>> email)
	{
		return email.Key.Sender.Address == "donotreply@amazon.com";
	}

	public static IList<string> Orders(this Dictionary<Email, List<string>> source)
	{
		return source.Where(item => !IsAmazonSellerEmail(item))
			.OrderByDescending(e => e.Key.Sent)
			.SelectMany(e => e.Value)
			.Distinct()
			.ToList();
	}

	public static Dictionary<Email, List<string>> OrdersEmailsIfAnyUnread(this Dictionary<Email, List<string>> source,
		string order)
	{
		var orderEmails = source.Where(e => e.Value.Contains(order))
			.OrderByDescending(e => e.Key.Sent)
			.ToDictionary(e => e.Key, e => e.Value);

		if (orderEmails.All(e => e.Key.IsRead)) return new Dictionary<Email, List<string>>();

		return orderEmails;
	}

	public static int CountOrdersWithUnreadEmails(this Dictionary<Email, List<string>> source)
	{
		return source
			.SelectMany(e => e.Value)
			.Distinct()
			.Count(order => source.OrdersEmailsIfAnyUnread(order).Any());
	}

	public static List<KeyValuePair<Email, List<string>>> UnreadSellerEmails(
		this Dictionary<Email, List<string>> emailsWithOrders
	)
	{
		return emailsWithOrders
			.Where(item => IsAmazonSellerEmail(item))
			.Where(item => !item.Key.IsRead)
			.OrderByDescending(e => e.Key.Sent)
			.ToList();
	}
}