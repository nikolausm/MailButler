using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MailButler.Configuration;

public sealed class ConfigurationSourceFromDataProvider<T> : IConfigurationSource
{
	private readonly IDataProvider<JToken> _dataProvider;
	private readonly string _key;

	public ConfigurationSourceFromDataProvider(IDataProvider<JToken> dataProvider, string key)
	{
		_dataProvider = dataProvider;
		_key = key;
	}

	public IConfigurationProvider Build(IConfigurationBuilder builder)
	{
		return new CustomConfigurationProvider<T>(_dataProvider, _key);
	}
}