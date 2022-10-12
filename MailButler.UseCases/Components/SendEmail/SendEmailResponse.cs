using MailButler.Dtos;

namespace MailButler.UseCases.Components.SendEmail;

public class SendEmailResponse : BaseResponse<DateTime?>
{
	public Email Email { get; init; } = new();
}