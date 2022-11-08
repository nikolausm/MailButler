using MailButler.Dtos;
using Mediator;

namespace MailButler.UseCases.Components.Amazon.GetAmazonOrderEmails;

public sealed class GetAmazonOrderEmailsRequest : IRequest<GetAmazonOrderEmailsResponse>
{
	public List<Email> Emails { get; init; } = new();
}