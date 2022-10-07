using MailButler.Dtos;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace MailButler.Core;

public sealed class SmtpClientFactory : ISmtpClientFactory
{
	public async Task<SmtpClient> SmtpClientAsync(Account account, CancellationToken cancellationToken)
	{
		var smtp = new SmtpClient();
		await smtp.ConnectAsync(account.SmtpServer, account.SmtpPort, SecureSocketOptions.StartTls, cancellationToken);
		await smtp.AuthenticateAsync(account.Username, account.Password, cancellationToken);
		return smtp;
	}
}