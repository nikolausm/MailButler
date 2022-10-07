using MailButler.Core;
using MailButler.Dtos;
using MediatR;
using MimeKit;

namespace MailButler.UseCases.SendEmail;

public sealed class SendEmailHandler: IRequestHandler<SendEmailRequest, SendEmailResponse>
{
	private readonly ISmtpClientFactory _smtpClientFactory;

	public SendEmailHandler(ISmtpClientFactory smtpClientFactory)
	{
		_smtpClientFactory = smtpClientFactory;
	}
	
	public async Task<SendEmailResponse> Handle(SendEmailRequest request, CancellationToken cancellationToken)
	{
		try
		{
			using var smtpClient = await _smtpClientFactory.SmtpClientAsync(request.Account, cancellationToken);
			await smtpClient.SendAsync(MimeMessage(request.Email, request.Account), cancellationToken);

			return new SendEmailResponse
			{
				Email = request.Email
			};
		}
		catch (Exception e)
		{
			return new SendEmailResponse
			{
				Email = request.Email,
				Status = Status.Failed,
				Message = e.Message
			};
		}
	}
	
	private static MimeMessage MimeMessage(Email email, Account account)
	{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress(email.Sender.Name, email.Sender.Address));
		message.To.Add(new MailboxAddress(account.Name, account.Username));
		message.Subject = email.Subject;
		message.Body = new TextPart("plain") { Text = email.TextBody };
		return message;
	}
}