namespace MailButler.UseCases.EmailsMatchAgainstRule;

public interface IOperation
{
	
	IRules And(params IRule[] rules);
	IRules Or(params IRule[] rules);
	IRules Or(Field field, FilterType filterType, string value);
	IRules And(Field field, FilterType filterType, string value);
}