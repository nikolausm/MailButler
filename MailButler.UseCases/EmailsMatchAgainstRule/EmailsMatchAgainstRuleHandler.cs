using System.Text.RegularExpressions;
using MailButler.Core.Extensions;
using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.EmailsMatchAgainstRule;

public sealed class
	EmailsMatchAgainstRuleHandler : IRequestHandler<EmailsMatchAgainstRuleRequest, EmailsMatchAgainstRuleResponse>
{
	public async Task<EmailsMatchAgainstRuleResponse> Handle(EmailsMatchAgainstRuleRequest request,
		CancellationToken cancellationToken)
	{

		return new EmailsMatchAgainstRuleResponse
		{
			Result = request.Emails.Where(email => Filter(email, request.Rule)).ToList()
		};
	}

	/// <summary>
	/// Since were getting the tail, we need to get the start.
	/// </summary>
	/// <param name="rules"></param>
	/// <returns></returns>
	private List<IRules> AsReversedList(IRules rules)
	{
		List<IRules> children = new()
		{
			rules
		};

		if (rules.Predecessor is not null)
		{
			var current = rules.Predecessor;
			while (current is not null)
			{
				children.Add(current);
				current = current.Predecessor;
			}
		}

		children.Reverse();
		return children;
	}

	private bool Filter(Email email, IRules rules)
	{
		return SplitInGroups(AsReversedList(rules))
			.ToList()
			.Any(groupedRules => groupedRules.Any(innerRules => AreRulesValid(email, innerRules)));
	}

	/// <summary>
	/// "A and b and c or d and e" will be list
	/// - A and b and c
	/// - d and e
	/// </summary>
	/// <param name="source"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	private IEnumerable<IList<T>> SplitInGroups<T>(IList<T> source)
		where T : IOperator
	{
		List<T> currentSet = new();
		foreach (var rule in source)
		{
			if (currentSet.Any() && rule.Operator == Operator.Or)
			{
				yield return currentSet;
				currentSet = new();
			}
			currentSet.Add(rule);
		}

		yield return currentSet;
	}

	private bool AreRulesValid(Email email, IRules innerRules)
		=> SplitInGroups(
				innerRules.ToList()
			)
			.ToList()
			.Any(items => AllTrue(email, items));

	private bool AllTrue(Email email, IEnumerable<IRule> rules)
	{
		return rules.All(rule => Match(email, rule));
	}

	private bool Match(Email email, IRule rule)
	{
		return rule.FilterType switch
		{
			FilterType.Contains => Value(email, rule.Field).Contains(rule.Value),
			FilterType.RegularExpression => Regex.IsMatch(Value(email, rule.Field), rule.Value),
			FilterType.SimpleString => rule.Value.IsSimpleFilterMatch(Value(email, rule.Field)),
			FilterType.EndsWith => Value(email, rule.Field).EndsWith(rule.Value),
			FilterType.StartsWith => Value(email, rule.Field).StartsWith(rule.Value),
			_ => throw new ArgumentOutOfRangeException()
		};
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