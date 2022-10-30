namespace MailButler.Configuration;

public interface IDataProvider<out T>
{
	T Data();
}