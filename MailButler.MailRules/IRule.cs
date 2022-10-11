using AnyOfTypes;
using MailButler.MailRules.Filter;


namespace MailButler.MailRules;

public interface IRule: IList<AnyOf<IFilter, IAction<object, object>>>
{
	public string Name { get; init; }
	public string Description { get; init; }
}

public interface IAction<in TIn, TOut>
{
	Task<TOut> ExecuteAsync(TIn input);
}





