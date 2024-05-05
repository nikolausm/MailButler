using FluentAssertions;
using MailButler.Api;
using MailButler.Configuration;
using MailButler.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MailButler.AzureBlob.Configuration.Tests;

public class CustomConfigurationProviderTests
{
	[Fact]
	public void Load_FromValidJObject_ContainsAllKeys()
	{
		var mock = new Mock<IDataProvider<JObject>>();
		mock
			.Setup(x => x.Data())
			.Returns(
				JsonConvert.DeserializeObject<JObject>(
					JsonConvert.SerializeObject(
						new DeleteFromKnownSenderOptions
						{
							SenderAddresses = new List<string>
							{
								"a@b.com",
								"c@d.com",
								"foo@bar.de"
							}
						}
					)
				)!
			);
		var provider = new CustomConfigurationProvider<MailButlerApiOptions>(
			mock.Object,
			"MailButler"
		);

		provider.Load();

		provider.TryGet("MailButler:SenderAddresses:0", out var _).Should().BeTrue();
		provider.TryGet("MailButler:SenderAddresses:1", out var _).Should().BeTrue();
		provider.TryGet("MailButler:SenderAddresses:2", out var _).Should().BeTrue();
	}
}