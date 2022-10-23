using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Components.Amazon.GetAmazonOrderSummary;

public class GetAmazonOrderEmailsSummaryRequest : IRequest<GetAmazonOrderEmailsSummaryResponse>
{
	public Dictionary<Email, List<string>> EmailsWithOrders { get; init; } = new();
	public List<Account> Accounts { get; set; } = new();
}