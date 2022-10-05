namespace MailButler.Dtos;

public class Email
{
	public string Subject { get; init; }
	public string? TextBody { get; init; }
	public string? HtmlBody { get; init; }
	public MailBoxAddress Sender { get; init; }
	public List<MailBoxAddress> To { get; init; }
	public string Id { get; init; }
	public DateTime Sent { get; init; }
	public List<MailBoxAddress> Cc { get; init; }
}