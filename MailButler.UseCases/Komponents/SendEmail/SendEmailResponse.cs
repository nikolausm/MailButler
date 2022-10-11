using MailButler.Dtos;

namespace MailButler.UseCases.Komponents.SendEmail;

public class SendEmailResponse : BaseResponse<DateTime?>
{
	public Email Email { get; init; } = new();
}