using MailButler.Dtos;

namespace MailButler.UseCases.Komponents.Amazon.GetAmazonOrderEmails;

public sealed class GetAmazonOrderEmailsResponse : BaseResponse<Dictionary<Email, List<string>>>
{
}