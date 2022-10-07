namespace MailButler.UseCases.EmailsMatchAgainstRule;

public interface IRule: IOperator
{
	Field Field { get; init; }
	string Value { get; init; }
	FilterType FilterType { get; init; }
}