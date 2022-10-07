namespace MailButler.UseCases.EmailsMatchAgainstRule;

public enum Field
{
	Unknown,
	Subject,
	TextBody,
	HtmlBody,
	ToAddress,
	ToName,
	SenderName,
	SenderAddress,
	AnyTextField
}