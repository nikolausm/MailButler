namespace MailButler.DataAccess.Database.MsSql.Entities;

public enum FilterType
{
	Unknown,
	RegularExpression,
	SimpleString,
	Contains,
	EndsWith,
	StartsWith,
}