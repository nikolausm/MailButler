using MailButler.Dtos;

namespace MailButler.UseCases.Amazon.GetAmazonOrderEmails;

public sealed class GetAmazonOrderEmailsResponse : BaseResponse<Dictionary<Email, List<string>>>
{
}