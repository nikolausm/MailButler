using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Components.SendEmail;

public class SendEmailRequest : IRequest<SendEmailResponse>
{
	public Email Email { get; init; } = new();
	public Account Account { get; init; } = new();
	public string From { get; init; } = string.Empty;
}