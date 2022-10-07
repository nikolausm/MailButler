namespace MailButler.UseCases.EmailsMatchAgainstRule;

public interface IRules : IReadOnlyList<IRule>,IOperation, IOperator
{
	IRules? Predecessor { get; init; }
}