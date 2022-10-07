using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.SendEmail;

public class SendEmailRequest: IRequest<SendEmailResponse>
{
	public Email Email { get; init; } = new();
	public Account Account { get; init; } = new();
}