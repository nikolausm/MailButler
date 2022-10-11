namespace MailButler.DataAccess.Database.MsSql.Entities;

public class Action
{
	public Filter Filter { get; init; }
	public Action Rule { get; init; }
}