namespace MailButler.Dtos;

public class Email
{
	public string Subject { get; init; } = string.Empty;
	public string? TextBody { get; init; }
	public string? HtmlBody { get; init; }
	public MailBoxAddress Sender { get; init; } = new();
	public List<MailBoxAddress> To { get; init; } = new();
	public string Id { get; init; } = string.Empty;
	public DateTime Sent { get; init; }
	public List<MailBoxAddress> Cc { get; init; } = new();
}