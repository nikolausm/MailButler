using MailButler.Dtos;
using MailKit;
using MimeKit;
using UniqueId = MailKit.UniqueId;

namespace MailButler.UseCases.Components.Extensions;

public static class MailKitExtensions
{
	public static Email ToEmail(this MimeMessage message, MessageFlags summary, UniqueId id, Guid accountId)
	{
		return new Email
		{
			HasAttachments = message.Attachments.Any(at => at.IsAttachment),
			AccountId = accountId,
			Id = new Dtos.UniqueId(id.Validity, id.Id),
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