namespace MailButler.Api.Options;

public sealed class AzureJsonOptions
{
	public string ContainerRoot { get; init; } = "";
	public string FileName { get; init; } = "";
}