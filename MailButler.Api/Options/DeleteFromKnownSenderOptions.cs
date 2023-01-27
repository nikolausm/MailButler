using MailButler.Dtos;
using Org.BouncyCastle.Asn1.X509;

namespace MailButler.Api.Options;

public sealed class DeleteFromKnownSenderOptions
{
	public List<string> SenderAddresses { get; init; } = new();
	public Account SmtpAccount { get; init; } = new();
	public int DaysToCheck { get; set; } = 7;
}