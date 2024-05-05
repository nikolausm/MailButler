using MailButler.Core;
using MailButler.Dtos;
using MailKit;
using MailKit.Net.Imap;
using Mediator;
using UniqueId = MailKit.UniqueId;

namespace MailButler.UseCases.Components.MarkAsRead;

public sealed class MarkAsReadHandler : IRequestHandler<MarkAsReadRequest, MarkAsReadResponse>
{
	private readonly IImapClientFactory _clientFactory;

	public MarkAsReadHandler(IImapClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
	}

	public async ValueTask<MarkAsReadResponse> Handle(MarkAsReadRequest request, CancellationToken cancellationToken)
	{
		try
		{
			using var client = await _clientFactory.ImapClientAsync(request.Account, cancellationToken);

			await client.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

			await SeenFlagAsync(request, cancellationToken, client);

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

	private static async Task SeenFlagAsync(MarkAsReadRequest request, CancellationToken cancellationToken,
		IImapClient client)
	{
		await client.Inbox.AddFlagsAsync(
			request.Emails.Where(email => !email.IsRead)
				.Select(email => new UniqueId(email.Id.Validity, email.Id.Id))
				.ToList(),
			MessageFlags.Seen,
			true,
			cancellationToken
		);

        
		// SetFlags to MessageFlags
		await client.Inbox.SetFlagsAsync(
			request.Emails.Where(email => !email.IsRead)
				.Select(email => new UniqueId(email.Id.Validity, email.Id.Id))
				.ToList(),
			MessageFlags.Seen,
			true,
			cancellationToken
		);
		
	}
}