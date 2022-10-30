using System.Text.RegularExpressions;
using MailButler.Core.Extensions;
using MailButler.Dtos;
using MailButler.MailRules.Filter;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MailButler.UseCases.Components.EmailsMatchAgainstRule;

public sealed class
	EmailsMatchAgainstRuleHandler : IRequestHandler<EmailsMatchAgainstRuleRequest, EmailsMatchAgainstRuleResponse>
{
	private readonly ILogger<EmailsMatchAgainstRuleHandler> _logger;

	public EmailsMatchAgainstRuleHandler(ILogger<EmailsMatchAgainstRuleHandler> logger)
	{
		_logger = logger;
	}

	public Task<EmailsMatchAgainstRuleResponse> Handle(
		EmailsMatchAgainstRuleRequest request,
		CancellationToken cancellationToken
	)
	{
		return Task.FromResult(new EmailsMatchAgainstRuleResponse
		{
			Result = request.Emails.Where(email => Filter(email, request.Filter)).ToList()
		});
	}

	/// <summary>
	///     Since were getting the tail, we need to get the start.
	/// </summary>
	/// <param name="filters"></param>
	/// <returns></returns>
	private List<IFilters> AsReversedList(IFilters filters)
	{
		List<IFilters> children = new()
		{
			filters
		};

		if (filters.Predecessor is not null)
		{
			var current = filters.Predecessor;
			while (current is not null)
			{
				children.Add(current);
				current = current.Predecessor;
			}
		}

		children.Reverse();
		return children;
	}

	private bool Filter(Email email, IFilters filters)
	{
		return SplitInGroups(AsReversedList(filters))
			.ToList()
			.Any(groupedRules => groupedRules.All(innerRules => AreRulesValid(email, innerRules)));
	}

	/// <summary>
	///     "A and b and c or d and e" will be list
	///     - A and b and c
	///     - d and e
	/// </summary>
	/// <param name="source"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	private IEnumerable<IList<T>> SplitInGroups<T>(IList<T> source)
		where T : ILocicalOperator
	{
		List<T> currentSet = new();
		foreach (var rule in source)
		{
			if (currentSet.Any() && rule.LogicalOperator == LogicalOperator.Or)
			{
				yield return currentSet;
				currentSet = new List<T>();
			}

			currentSet.Add(rule);
		}

		yield return currentSet;
	}

	private bool AreRulesValid(Email email, IFilters innerFilters)
	{
		return SplitInGroups(
				innerFilters.ToList()
			)
			.ToList()
			.Any(items => AllTrue(email, items));
	}

	private bool AllTrue(Email email, IEnumerable<IFilter> rules)
	{
		return rules.All(rule => Match(email, rule));
	}

	private bool Match(Email email, IFilter filter)
	{
		switch (filter)
		{
			case HasAttachmentFilter:
				return email.HasAttachments;
			case UnreadFilter:
				return !email.IsRead;
		}

		var result = filter.FilterType switch
		{
			FilterType.Contains => Value(email, filter.Field).Contains(filter.Value),
			FilterType.RegularExpression => Regex.IsMatch(Value(email, filter.Field), filter.Value),
			FilterType.SimpleString => filter.Value.IsSimpleFilterMatch(Value(email, filter.Field)),
			FilterType.EndsWith => Value(email, filter.Field).EndsWith(filter.Value),
			FilterType.StartsWith => Value(email, filter.Field).StartsWith(filter.Value),
			FilterType.Equals => Value(email, filter.Field)
				.Equals(filter.Value, StringComparison.InvariantCultureIgnoreCase),
			_ => throw new ArgumentOutOfRangeException()
		};

		if (!_logger.IsEnabled(LogLevel.Debug))
			return result;

		var value = Value(email, filter.Field);
		if (value.Length > 40) value = value.Substring(0, 40) + "...";

		_logger.LogTrace(
			"{Filter} returns {Result} for {Email} from {From} where Value: {Value}",
			filter,
			email.Id,
			email.Sender.Address,
			result,
			value
		);

		return result;
	}

	private string Value(Email email, Field field)
	{
		return field switch
		{
			Field.SenderAddress => email.Sender.Address ?? "",
			Field.SenderName => email.Sender.Name ?? "",
			Field.Subject => email.Subject,
			Field.TextBody => email.TextBody ?? "",
			Field.HtmlBody => email.HtmlBody ?? "",
			Field.AnyTextField => (email.TextBody ?? "") + " : " + (email.HtmlBody ?? "") + ":" + email.Subject,
			_ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
		};
	}
}