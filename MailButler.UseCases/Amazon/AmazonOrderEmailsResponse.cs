using MailButler.Dtos;
using MailButler.UseCases.FetchEmails;

namespace MailButler.UseCases.Amazon;

public sealed class AmazonOrderEmailsResponse: BaseResponse<Dictionary<string, List<string>>>
{
}