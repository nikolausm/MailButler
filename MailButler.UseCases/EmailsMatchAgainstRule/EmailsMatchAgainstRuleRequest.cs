using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.EmailsMatchAgainstRule;

public class EmailsMatchAgainstRuleRequest: IRequest<EmailsMatchAgainstRuleResponse>
{
	public List<Email> Emails { get; set; } = new();
	public IRules Rule { get; set; } 
}