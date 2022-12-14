using MailButler.Dtos;
using MailButler.MailRules.Filter;
using Mediator;

namespace MailButler.UseCases.Components.EmailsMatchAgainstRule;

public class EmailsMatchAgainstRuleRequest : IRequest<EmailsMatchAgainstRuleResponse>
{
	public List<Email> Emails { get; set; } = new();
	public IFilters Filter { get; set; } = new BaseFilters();
}