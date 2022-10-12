using MailButler.Dtos;

namespace MailButler.UseCases.Components.Amazon.GetAmazonOrderEmails;

public sealed class GetAmazonOrderEmailsResponse : BaseResponse<Dictionary<Email, List<string>>>
{
}