namespace MailButler.UseCases.EmailsMatchAgainstRule;

public enum FilterType
{
	RegularExpression,
	SimpleString,
	Contains,
	EndsWith,
	StartsWith
}