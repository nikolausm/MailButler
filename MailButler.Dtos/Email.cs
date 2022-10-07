

namespace MailButler.Dtos;

public record Email
{
	public string Subject { get; init; } = string.Empty;
	public string? TextBody { get; init; }
	public string? HtmlBody { get; init; }
	public MailBoxAddress Sender { get; init; } = new();
	public List<MailBoxAddress> To { get; init; } = new();
	public UniqueId Id { get; init; } = default;
	public DateTime Sent { get; init; }
	public List<MailBoxAddress> Cc { get; init; } = new();
	public bool IsRead { get; init; }

	public override string ToString()
	{
		return $"{(IsRead ? "" : "*")}From: {Sender}\r\n{Subject}\r\n----------------\r\n{TextBody}";
	}
}