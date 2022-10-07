using MailButler.Dtos;
using MailKit.Net.Smtp;

namespace MailButler.Core;

public interface ISmtpClientFactory
{
	Task<SmtpClient> SmtpClientAsync(Account account, CancellationToken cancellationToken);
}