using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Komponents.Amazon.GetAmazonOrderEmails;

public sealed class GetAmazonOrderEmailsRequest : IRequest<GetAmazonOrderEmailsResponse>
{
	public List<Email> Emails { get; init; } = new();
}