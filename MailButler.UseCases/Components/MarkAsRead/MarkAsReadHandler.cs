using MailButler.Core;
using MailButler.Dtos;
using MailKit;
using MediatR;
using UniqueId = MailKit.UniqueId;

namespace MailButler.UseCases.Components.MarkAsRead;

public sealed class MarkAsReadHandler : IRequestHandler<MarkAsReadRequest, MarkAsReadResponse>
{
	private readonly IImapClientFactory _clientFactory;

	public MarkAsReadHandler(IImapClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
	}

	public async Task<MarkAsReadResponse> Handle(MarkAsReadRequest request, CancellationToken cancellationToken)
	{
		try
		{
			using var client = await _clientFactory.ImapClientAsync(request.Account, cancellationToken);

			await client.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

			await client.Inbox.AddFlagsAsync(
				request.Emails.Where(email => !email.IsRead)
					.Select(email => new UniqueId(email.Id.Validity, email.Id.Id))
					.ToList(),
				MessageFlags.Seen,
				true,
				cancellationToken
			);

			return new MarkAsReadResponse
			{
				Result = true
			};
		}
		catch (Exception e)
		{
			return new MarkAsReadResponse
			{
				Result = false,
				Message = e.Message,
				Status = Status.Failed
			};
		}
	}
}