using FluentAssertions;
using MailButler.Dtos;
using MailButler.MailRules.Filter;
using MailButler.UseCases.Components.EmailsMatchAgainstRule;
using Microsoft.Extensions.Logging;
using Moq;

namespace MailButler.UseCases.Tests;

public class EmailMatchAgainstRuleTests
{
	[Fact]
	public async Task MultipleAndFilters_Working()
	{
		var req = new EmailsMatchAgainstRuleRequest
		{
			Emails = new List<Email>
			{
				new()
				{
					Subject = "Ihre Rechnung für den Vertrag 123456789",
					AccountId = Guid.NewGuid(),
					IsRead = false,
					Sender = new MailBoxAddress
					{
						Address = "noreply@ionos.de",
						Name = "Mr. Foo"
					},
					HasAttachments = true
				},
				new()
				{
					Subject = "Ihre Rechnung für den Vertrag XXXX",
					AccountId = Guid.NewGuid(),
					IsRead = false,
					Sender = new MailBoxAddress
					{
						Address = "noreply@ionos.de",
						Name = "Mr. Foo"
					},
					HasAttachments = true
				}
			},
			Filter = new IonosInvoiceFilter("123456789")
		};

		var result = await new EmailsMatchAgainstRuleHandler(new Mock<ILogger<EmailsMatchAgainstRuleHandler>>().Object)
			.Handle(req, CancellationToken.None);

		result
			.Result
			.Should()
			.HaveCount(1);
	}
}