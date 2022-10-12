using ExtensionsDictionary = Extensions.Dictionary;

namespace MailButler.Core.Extensions;

public static class DictionaryExtensions
{
	public static IDictionary<string, object?> ToDictionary<T>(this T instance)
		where T : new()
	{
		return ExtensionsDictionary.ObjectExtensions.ToDictionary(instance);
	}

	public static string? ToString<T>(this IDictionary<string, object?> origin)
		where T : notnull
	{
		return "Filter: " + string.Join(" - ",
			origin
				.Select(kv => $"{kv.Key}: {kv.Value}\r\n")
		);
	}

	public static string? ToDictionaryAsString<T>(this T instance)
		where T : new()
	{
		return ToString<T>(instance.ToDictionary());
	}
}