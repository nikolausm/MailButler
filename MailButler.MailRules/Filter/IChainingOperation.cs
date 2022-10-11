namespace MailButler.MailRules.Filter;

public interface IChainingOperation
{
	IFilters And(IFilters filters);
	IFilters Or(IFilters filters);
	IFilters And(params IFilter[] rules);
	IFilters Or(params IFilter[] rules);
	IFilters Or(Field field, FilterType filterType, string value);
	IFilters And(Field field, FilterType filterType, string value);
}