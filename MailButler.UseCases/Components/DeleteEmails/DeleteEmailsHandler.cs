using MailButler.Core;
using MailButler.Dtos;
using MailKit;
using MediatR;
using UniqueId = MailKit.UniqueId;

namespace MailButler.UseCases.Components.DeleteEmails;

public sealed class DeleteEmailsHandler : IRequestHandler<DeleteEmailsRequest, DeleteEmailsResponse>
{
	private readonly IImapClientFactory _clientFactory;

	public DeleteEmailsHandler(IImapClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
	}

	public async Task<DeleteEmailsResponse> Handle(DeleteEmailsRequest request, CancellationToken cancellationToken)
	{
		try
		{
			using var client = await _clientFactory.ImapClientAsync(request.Account, cancellationToken);

			await client.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

			await client.Inbox.AddFlagsAsync(
				request.Emails.Select(e => new UniqueId(e.Id.Validity, e.Id.Id)).ToArray(),
				MessageFlags.Deleted,
				true,
				cancellationToken
			);

			await client.Inbox.ExpungeAsync(cancellationToken);
			return new DeleteEmailsResponse
			{
				Result = true
			};
		}
		catch (Exception e)
		{
			return new DeleteEmailsResponse
			{
				Result = false,
				Message = e.Message,
				Status = Status.Failed
			};
		}
	}
}