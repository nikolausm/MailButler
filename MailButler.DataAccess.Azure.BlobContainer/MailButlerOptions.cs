namespace MailButler.DataAccess.Azure.BlobContainer;

public sealed class MailButlerOptions
{
	public DeleteFromKnownSenderOptions DeleteFromKnownSender { get; init; } = new();
}