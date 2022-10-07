using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Amazon.GetAmazonOrderSummary;

public class GetAmazonOrderEmailsSummaryRequest: IRequest<GetAmazonOrderEmailsSummaryResponse>
{
	public Dictionary<Email, List<string>> EmailsWithOrders { get; init; } = new();
}