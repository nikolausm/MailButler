using MailKit;
using Microsoft.Exchange.WebServices.Data;
using MimeKit;

namespace MailButler.Core.Exchange;

internal sealed class ExchangeMessageSummary : IMessageSummary
{
	private readonly EmailMessage _message;

	public ExchangeMessageSummary(EmailMessage message)
	{
		_message = message;
	}

	public string ThreadId { get; }
	public UniqueId UniqueId => new (UInt32.Parse(_message.Id.UniqueId));

	public int Index => Int32.Parse(_message.Id.UniqueId);
	public ulong? GMailMessageId { get; }
	public ulong? GMailThreadId { get; }
	public IList<string> GMailLabels { get; }

	public MessageSummaryItems Items => MessageSummaryItems.Full;

	public string EnvelopeId => _message.InternetMessageId;

	public HeaderList Headers { get; }
	public DateTimeOffset? InternalDate => _message.DateTimeReceived;
	public DateTimeOffset? SaveDate { get; }

	public uint? Size => (uint)_message.Size;
	public ulong? ModSeq { get; }

	public IMailFolder Folder { get; }
	public MessageSummaryItems Fields { get; }
	public BodyPart Body { get; }
	public BodyPartText TextBody { get; }
	public BodyPartText HtmlBody { get; }
	public IEnumerable<BodyPartBasic> BodyParts { get; }
	public IEnumerable<BodyPartBasic> Attachments { get; }
	public string PreviewText { get; }
	public Envelope Envelope { get; }
	public string NormalizedSubject { get; }
	public DateTimeOffset Date { get; }
	public bool IsReply { get; }

	public MessageFlags? Flags
	{
		get
		{
			var flags = MessageFlags.None;
				
			if (_message.Flag.FlagStatus == ItemFlagStatus.Flagged)
			{
				flags |= MessageFlags.Flagged;
			}

			if (_message.IsRead)
			{
				flags |= MessageFlags.Seen;
			}

			if (_message.InternetMessageId != null)
			{
				// TODO: Check if this is correct
				flags |= MessageFlags.Answered;
			}

			if (_message.IsDraft)
			{
				flags |= MessageFlags.Draft;
			}
				
			if (_message.IsFromMe
			    || _message.IsResend
			    || _message.IsSubmitted
			    || _message.IsUnmodified
			    || _message.IsReadReceiptRequested
			    || _message.IsSubmitted)
			{
				// TODO: Check if this is correct
				flags |= MessageFlags.UserDefined;
			}

			return flags;
		}
	}

	public IReadOnlySet<string> Keywords => _message.Categories.ToHashSet();
	public IReadOnlyList<Annotation> Annotations => new List<Annotation>();
	public IList<UniqueId> UniqueIds => new[] { UniqueId };

	public MessageIdList References
	{
		get
		{
			var result = new MessageIdList();
				
			if ( _message.References != null)
				result.AddRange(_message.References.Split(' '));

			return result;
		}
	}

	public string EmailId => _message.InternetMessageId;

	public string[] InReplyTo => _message.InReplyTo.Split(' ');

	public IList<string> FlagsList
	{
		get
		{
				
			var flags = new List<string>();
				
			if (_message.Flag.FlagStatus == ItemFlagStatus.Flagged)
			{
				flags.Add("\\Flagged");
			}

			if (_message.IsRead)
			{
				flags.Add("\\Seen");
			}

			if (_message.InternetMessageId != null)
			{
				flags.Add("\\MessageId");
			}

			if (_message.IsDraft)
			{
				flags.Add("\\Draft");
			}
				
			if (_message.IsFromMe)
			{
				flags.Add("\\FromMe");
			}
				
			if (_message.IsResend)
			{
				flags.Add("\\Resend");
			}
				
			if (_message.IsSubmitted)
			{
				flags.Add("\\Submitted");
			}
				
			if (_message.IsUnmodified)
			{
				flags.Add("\\Unmodified");
			}
				
			if (_message.IsReadReceiptRequested)
			{
				flags.Add("\\ReadReceiptRequested");
			}
				
			if (_message.IsDeliveryReceiptRequested)
			{
				flags.Add("\\DeliveryReceiptRequested");
			}
				
			if (_message.IsDeliveryReceiptRequested)
			{
				flags.Add("\\DeliveryReceiptRequested");
			}
				
			if (_message.IsReadReceiptRequested)
			{
				flags.Add("\\ReadReceiptRequested");
			}
				
			if (_message.IsAssociated)
			{
				flags.Add("\\Associated");
			}
				
			if (_message.IsSubmitted)
			{
				flags.Add("\\Submitted");
			}
				
			return flags;
		}
	}
}