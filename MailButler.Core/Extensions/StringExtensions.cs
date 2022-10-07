using System.Text.RegularExpressions;

namespace MailButler.Core.Extensions;

public static class StringExtensions
{
	/// <summary>
	///     Filter a string value by a simple filter.
	///     The * is used to like % in SQL, to escape use `
	/// </summary>
	/// <param name="value">The String to be filtered</param>
	/// <param name="filter">The Filter, can start or end with "*"</param>
	/// <returns></returns>
	public static bool IsSimpleFilterMatch(this string filter, string value)

	{
		if (filter == "*")
			return true;

		if (!filter.Contains("*")) return filter == value;

		return Regex.IsMatch(
			value,
			filter.EscapeAsSimpleFilter(),
			RegexOptions.Singleline | RegexOptions.IgnoreCase
		);
	}


	public static string EscapeAsSimpleFilter(this string filter)

	{
		const string escapeString = "`";
		var placeHolder = Guid.NewGuid().ToString();
		return "^"
		       // "*\\bla\\* -> .*\\bla\\.*
		       // "^*\\bla\\* -> \*\\bla\\.*
		       + Regex
			       // `* -> <Guid>
			       .Escape(filter.Replace(escapeString +
			                              "*", placeHolder))
			       // * -> .* 
			       .Replace("\\*", ".*")

			       // <Guid> -> `*
			       .Replace(placeHolder, "\\*")
		       + "$";
	}
}