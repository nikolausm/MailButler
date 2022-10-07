using MailButler.Dtos;
using MailKit;
using MimeKit;
using UniqueId = MailButler.Dtos.UniqueId;

namespace MailButler.UseCases.Extensions;

public static class MailKitExtensions
{
	public static Email ToEmail(this MimeMessage message, MessageFlags summary, MailKit.UniqueId id)
	{
		return new Email
		{
			Id = new UniqueId(id.Id, id.Validity),
			Sender = new MailBoxAddress
			{
				Name = message.From.Single().Name,
				Address = ((MailboxAddress)message.From.Single()).Address
			},
			To = message.To.Mailboxes.Select(x => new MailBoxAddress
			{
				Name = x.Name,
				Address = x.Address
			}).ToList(),

			Cc = message.Cc.Mailboxes.Select(x => new MailBoxAddress
			{
				Name = x.Name,
				Address = x.Address
			}).ToList(),

			IsRead = summary.HasFlag(MessageFlags.Seen),
			Sent = message.Date.DateTime,
			Subject = message.Subject,
			HtmlBody = message.HtmlBody,
			TextBody = message.TextBody
		};
	}
}