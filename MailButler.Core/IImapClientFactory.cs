using MailButler.Dtos;
using MailKit.Net.Imap;

namespace MailButler.Core;

public interface IImapClientFactory
{
	Task<IImapClient> ImapClientAsync(Account account, CancellationToken cancellationToken);
}