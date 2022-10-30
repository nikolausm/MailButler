using MailButler.Dtos;
using MediatR;

namespace MailButler.UseCases.Components.EmailsSummary;

public sealed class EmailsSummaryRequest: IRequest<EmailsSummaryResponse>
{
	public List<Email> Emails { get; init; } = new();
}