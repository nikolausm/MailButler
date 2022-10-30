using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MailButler.Configuration.AzureJson.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
	/// <summary>
	///     Adds the ability to add Json files from Azure to Configuration.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="containerRoot"></param>
	/// <param name="fileName"></param>
	/// <param name="prefix">
	///     Optional: Keys in Configuration will be prefixed with this key.
	///     <typeparam name="T"></typeparam>
	///     <returns></returns>
	public static IConfigurationBuilder AddAzureJson<T>(
		this IConfigurationBuilder builder,
		string containerRoot,
		string fileName,
		string prefix = ""
	)
	{
		return builder.Add(
			new ConfigurationSourceFromDataProvider<T>(
				new AzureJsonDataProvider<JToken>(containerRoot, fileName), prefix)
		);
	}
}