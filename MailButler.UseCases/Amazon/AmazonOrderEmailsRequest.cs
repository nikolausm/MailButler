using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Amazon;

public sealed class AmazonOrderEmailsRequest : IRequest<AmazonOrderEmailsResponse>
{
	public List<Email> Emails { get; init; } = new();
}