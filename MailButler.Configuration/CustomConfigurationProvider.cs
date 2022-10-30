using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MailButler.Configuration;

public sealed class CustomConfigurationProvider<T> : ConfigurationProvider
{
	private readonly IDataProvider<JToken> _dataProvider;
	private readonly string _keyName;

	public CustomConfigurationProvider(
		IDataProvider<JToken> dataProvider,
		string? keyName = null
	)
	{
		_dataProvider = dataProvider;
		_keyName = keyName ?? "";
	}

	public override void Load()
	{
		Data.Clear();
		foreach (var item in new ConfigurationData(_dataProvider.Data()))
			Data.Add((
					string.IsNullOrEmpty(_keyName)
						? ""
						: _keyName + ":") + item.Key,
				item.Value
			);
		OnReload();
	}
}