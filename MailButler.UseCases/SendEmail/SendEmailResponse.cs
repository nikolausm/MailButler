using MailButler.Dtos;

namespace MailButler.UseCases.SendEmail;

public class SendEmailResponse: BaseResponse<DateTime?>
{
	public Email Email { get; init; } = new Email();
}