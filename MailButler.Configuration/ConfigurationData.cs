using Newtonsoft.Json.Linq;

namespace MailButler.Configuration;

public sealed class ConfigurationData : Dictionary<string, string>
{
	public ConfigurationData(JToken source)
		: base(FromJToken(source))
	{
	}

	private static IEnumerable<KeyValuePair<string, string>> FromJToken(JToken source)
	{
		return ToNested(source);
	}

	private static IEnumerable<KeyValuePair<string, string>> ToNested(JToken source)
	{
		if (source is JArray or JObject or JProperty)
		{
			foreach (var child in source.Children())
			foreach (var item in ToNested(child))
				yield return item;
		}
		else
		{
			var value = source.Value<string>();
			if (value is null) yield break;
			yield return new KeyValuePair<string, string>(ConfigurationKey(source.Path), value);
		}
	}

	private static string ConfigurationKey(string jsonPath)
	{
		var path = jsonPath
			.Replace(".", ":")
			.Replace("[", ":")
			.Replace("]", "");

		return $"{path}";
	}
}