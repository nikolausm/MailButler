namespace MailButler.DataAccess.Azure.BlobContainer;

public sealed class DeleteFromKnownSenderOptions
{
	public List<string> SenderAddresses { get; init; } = new();
}