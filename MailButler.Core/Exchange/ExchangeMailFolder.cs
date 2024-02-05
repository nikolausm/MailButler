using System.Collections;
using System.Text;
using MailButler.Core.Exchange.Extensions.Webservices.Data;
using MailKit;
using MailKit.Search;
using Microsoft.Exchange.WebServices.Data;
using MimeKit;
using MimeContent = MimeKit.MimeContent;
using Task = System.Threading.Tasks.Task;

namespace MailButler.Core.Exchange;

internal class ExchangeMailFolder : IMailFolder
{
	private readonly Lazy<Folder> _folder;
	private readonly ExchangeService _service;
	private readonly IDictionary<UniqueId, EmailMessage> _fetcheItems;

	public ExchangeMailFolder(ExchangeService service, WellKnownFolderName folderName)
	{
		_service = service;
		_folder = new Lazy<Folder>(() => Folder.Bind(service, new FolderId(folderName)));
		_fetcheItems = new Dictionary<UniqueId, EmailMessage>();
	}

	public bool SupportsReadOnly => true;

	public bool SupportsReadWrite => false;

	public bool SupportsSetFlags => false;

	public bool SupportsAddFlags => false;

	public bool SupportsRemoveFlags => false;

	public bool SupportsThreadableFlags => false;

	public UniqueId? UniqueIdValidity => null;

	public bool IsNamespace { get; }
	public string FullName => _folder.Value.DisplayName;

	public string Name => _folder.Value.DisplayName;
	public string Id { get; }
	public bool IsSubscribed { get; private set; }

	public int Recent => _fetcheItems.Count;
	public int Count => _folder.Value.TotalCount;

	public HashSet<ThreadingAlgorithm> ThreadingAlgorithms { get; } = new();
	public event EventHandler<EventArgs>? Opened;
	public event EventHandler<EventArgs>? Closed;
	public event EventHandler<EventArgs>? Deleted;
	public event EventHandler<FolderRenamedEventArgs>? Renamed;
	public event EventHandler<EventArgs>? Subscribed;
	public event EventHandler<EventArgs>? Unsubscribed;
	public event EventHandler<MessageEventArgs>? MessageExpunged;
	public event EventHandler<MessagesVanishedEventArgs>? MessagesVanished;
	public event EventHandler<MessageFlagsChangedEventArgs>? MessageFlagsChanged;
	public event EventHandler<MessageLabelsChangedEventArgs>? MessageLabelsChanged;
	public event EventHandler<AnnotationsChangedEventArgs>? AnnotationsChanged;
	public event EventHandler<MessageSummaryFetchedEventArgs>? MessageSummaryFetched;
	public event EventHandler<MetadataChangedEventArgs>? MetadataChanged;
	public event EventHandler<ModSeqChangedEventArgs>? ModSeqChanged;
	public event EventHandler<EventArgs>? HighestModSeqChanged;
	public event EventHandler<EventArgs>? UidNextChanged;
	public event EventHandler<EventArgs>? UidValidityChanged;
	public event EventHandler<EventArgs>? IdChanged;
	public event EventHandler<EventArgs>? SizeChanged;
	public event EventHandler<EventArgs>? CountChanged;
	public event EventHandler<EventArgs>? RecentChanged;
	public event EventHandler<EventArgs>? UnreadChanged;

	public int FirstUnread { get; }
	public int Unread => _folder.Value.UnreadCount;

	public bool IsOpen => throw new NotImplementedException();
	public bool Exists { get; }
	public ulong HighestModSeq { get; }
	public uint UidValidity { get; }
	public UniqueId? UidNext { get; }
	public uint? AppendLimit { get; }
	public ulong? Size { get; }

	public bool Supports(FolderFeature feature)
	{
		throw new NotImplementedException();
	}

	public FolderAccess Open(FolderAccess access, uint uidValidity, ulong highestModSeq, IList<UniqueId> uids,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<FolderAccess> OpenAsync(FolderAccess access, uint uidValidity, ulong highestModSeq,
		IList<UniqueId> uids,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	FolderAccess IMailFolder.Open(FolderAccess access, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	async Task<FolderAccess> IMailFolder.OpenAsync(FolderAccess access, CancellationToken cancellationToken)
	{
		_folder.Value.Load();
		return access;
	}

	public void Close(bool expunge, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
	}

	public Task CloseAsync(bool expunge, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.CompletedTask;
	}

	public IMailFolder Create(string name, bool isMessageFolder, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IMailFolder> CreateAsync(string name, bool isMessageFolder,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IMailFolder Create(string name, IEnumerable<SpecialFolder> specialUses,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IMailFolder> CreateAsync(string name, IEnumerable<SpecialFolder> specialUses,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IMailFolder Create(string name, SpecialFolder specialUse,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IMailFolder> CreateAsync(string name, SpecialFolder specialUse,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Rename(IMailFolder parent, string name, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task RenameAsync(IMailFolder parent, string name, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Delete(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task DeleteAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Subscribe(CancellationToken cancellationToken = new())
	{
		IsSubscribed = true;
	}

	public async Task SubscribeAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Unsubscribe(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task UnsubscribeAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<IMailFolder> GetSubfolders(StatusItems items, bool subscribedOnly = false,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<IMailFolder>> GetSubfoldersAsync(StatusItems items, bool subscribedOnly = false,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<IMailFolder> GetSubfolders(bool subscribedOnly = false, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<IMailFolder>> GetSubfoldersAsync(bool subscribedOnly = false,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IMailFolder GetSubfolder(string name, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IMailFolder> GetSubfolderAsync(string name, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Check(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task CheckAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Status(StatusItems items, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task StatusAsync(StatusItems items, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public AccessControlList GetAccessControlList(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<AccessControlList> GetAccessControlListAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public AccessRights GetAccessRights(string name, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<AccessRights> GetAccessRightsAsync(string name, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public AccessRights GetMyAccessRights(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<AccessRights> GetMyAccessRightsAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void AddAccessRights(string name, AccessRights rights, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task AddAccessRightsAsync(string name, AccessRights rights,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void RemoveAccessRights(string name, AccessRights rights,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task RemoveAccessRightsAsync(string name, AccessRights rights,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void SetAccessRights(string name, AccessRights rights, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task SetAccessRightsAsync(string name, AccessRights rights,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void RemoveAccess(string name, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task RemoveAccessAsync(string name, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public FolderQuota GetQuota(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<FolderQuota> GetQuotaAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public FolderQuota SetQuota(uint? messageLimit, uint? storageLimit,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<FolderQuota> SetQuotaAsync(uint? messageLimit, uint? storageLimit,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public string GetMetadata(MetadataTag tag, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<string> GetMetadataAsync(MetadataTag tag, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public MetadataCollection GetMetadata(IEnumerable<MetadataTag> tags, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<MetadataCollection> GetMetadataAsync(IEnumerable<MetadataTag> tags,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public MetadataCollection GetMetadata(MetadataOptions options, IEnumerable<MetadataTag> tags,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<MetadataCollection> GetMetadataAsync(MetadataOptions options, IEnumerable<MetadataTag> tags,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void SetMetadata(MetadataCollection metadata, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task SetMetadataAsync(MetadataCollection metadata, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<int>> StoreAsync(IList<int> indexes, ulong modseq, IList<Annotation> annotations,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<UniqueId> Search(SearchQuery query, CancellationToken cancellationToken)
	{
		var propSet = new PropertySet(BasePropertySet.IdOnly);

		var values = _folder.Value
			.FindItems(query.ToExchangeSearchFilter(), new ItemView(1))
			.ToList()
			.ConvertAll(result => new UniqueId(uint.Parse(result.Id.UniqueId)));

		return values;
	}

	public Task<IList<UniqueId>> SearchAsync(SearchQuery query, CancellationToken cancellationToken)
	{
		return Task.Run(() => Search(query, cancellationToken), cancellationToken);
	}

	public IList<UniqueId> Search(IList<UniqueId> uids, SearchQuery query, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UniqueId>> SearchAsync(IList<UniqueId> uids, SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public SearchResults Search(SearchOptions options, SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<SearchResults> SearchAsync(SearchOptions options, SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public SearchResults Search(SearchOptions options, IList<UniqueId> uids, SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<SearchResults> SearchAsync(SearchOptions options, IList<UniqueId> uids, SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<UniqueId> Sort(SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UniqueId>> SortAsync(SearchQuery query, IList<OrderBy> orderBy,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<UniqueId> Sort(IList<UniqueId> uids, SearchQuery query, IList<OrderBy> orderBy,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UniqueId>> SortAsync(IList<UniqueId> uids, SearchQuery query, IList<OrderBy> orderBy,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public SearchResults Sort(SearchOptions options, SearchQuery query, IList<OrderBy> orderBy,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<SearchResults> SortAsync(SearchOptions options, SearchQuery query, IList<OrderBy> orderBy,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public SearchResults Sort(SearchOptions options, IList<UniqueId> uids, SearchQuery query, IList<OrderBy> orderBy,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<SearchResults> SortAsync(SearchOptions options, IList<UniqueId> uids, SearchQuery query,
		IList<OrderBy> orderBy,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<MessageThread> Thread(ThreadingAlgorithm algorithm, SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<MessageThread>> ThreadAsync(ThreadingAlgorithm algorithm, SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<MessageThread> Thread(IList<UniqueId> uids, ThreadingAlgorithm algorithm, SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<MessageThread>> ThreadAsync(IList<UniqueId> uids, ThreadingAlgorithm algorithm,
		SearchQuery query,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public object SyncRoot { get; }
	public IMailFolder ParentFolder { get; }
	public FolderAttributes Attributes { get; }
	public AnnotationAccess AnnotationAccess { get; }
	public AnnotationScope AnnotationScopes { get; }
	public uint MaxAnnotationSize { get; }
	public MessageFlags PermanentFlags { get; }
	public IReadOnlySet<string> PermanentKeywords { get; }
	public MessageFlags AcceptedFlags { get; }
	public IReadOnlySet<string> AcceptedKeywords { get; }
	public char DirectorySeparator { get; }
	public FolderAccess Access { get; }

	public void Expunge(CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
	}

	public Task ExpungeAsync(CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.CompletedTask;
	}

	public void Expunge(IList<UniqueId> uids, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task ExpungeAsync(IList<UniqueId> uids, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueId? Append(IAppendRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueId?> AppendAsync(IAppendRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueId? Append(FormatOptions options, IAppendRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueId?> AppendAsync(FormatOptions options, IAppendRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<UniqueId> Append(IList<IAppendRequest> requests, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UniqueId>> AppendAsync(IList<IAppendRequest> requests,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<UniqueId> Append(FormatOptions options, IList<IAppendRequest> requests,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UniqueId>> AppendAsync(FormatOptions options, IList<IAppendRequest> requests,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueId? Replace(UniqueId uid, IReplaceRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueId?> ReplaceAsync(UniqueId uid, IReplaceRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueId? Replace(FormatOptions options, UniqueId uid, IReplaceRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueId?> ReplaceAsync(FormatOptions options, UniqueId uid, IReplaceRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueId? Replace(int index, IReplaceRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueId?> ReplaceAsync(int index, IReplaceRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueId? Replace(FormatOptions options, int index, IReplaceRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueId?> ReplaceAsync(FormatOptions options, int index, IReplaceRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueId? CopyTo(UniqueId uid, IMailFolder destination, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueId?> CopyToAsync(UniqueId uid, IMailFolder destination,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueIdMap CopyTo(IList<UniqueId> uids, IMailFolder destination,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueIdMap> CopyToAsync(IList<UniqueId> uids, IMailFolder destination,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueId? MoveTo(UniqueId uid, IMailFolder destination, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueId?> MoveToAsync(UniqueId uid, IMailFolder destination,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public UniqueIdMap MoveTo(IList<UniqueId> uids, IMailFolder destination,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<UniqueIdMap> MoveToAsync(IList<UniqueId> uids, IMailFolder destination,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void CopyTo(int index, IMailFolder destination, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task CopyToAsync(int index, IMailFolder destination, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void CopyTo(IList<int> indexes, IMailFolder destination, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task CopyToAsync(IList<int> indexes, IMailFolder destination,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void MoveTo(int index, IMailFolder destination, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task MoveToAsync(int index, IMailFolder destination, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void MoveTo(IList<int> indexes, IMailFolder destination, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task MoveToAsync(IList<int> indexes, IMailFolder destination,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<IMessageSummary> Fetch(IList<UniqueId> uids, IFetchRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<IMessageSummary>> FetchAsync(IList<UniqueId> uids, IFetchRequest request,
		CancellationToken cancellationToken = new())
	{
		return await
			Task.Run(() => uids
					.Chunk(100)
					.ToList()
					.SelectMany(chunk => chunk.Select(uid => (IMessageSummary)new MessageSummary(this, (int)uid.Id)))
					.ToList(),
				cancellationToken
			);
	}

	public IList<IMessageSummary> Fetch(IList<int> indexes, IFetchRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<IMessageSummary>> FetchAsync(IList<int> indexes, IFetchRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<IMessageSummary> Fetch(int min, int max, IFetchRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<IMessageSummary>> FetchAsync(int min, int max, IFetchRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public HeaderList GetHeaders(UniqueId uid, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<HeaderList> GetHeadersAsync(UniqueId uid, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public HeaderList GetHeaders(UniqueId uid, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<HeaderList> GetHeadersAsync(UniqueId uid, BodyPart part,
		CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public HeaderList GetHeaders(int index, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<HeaderList> GetHeadersAsync(int index, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public HeaderList GetHeaders(int index, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<HeaderList> GetHeadersAsync(int index, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public MimeMessage GetMessage(UniqueId uid, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<MimeMessage> GetMessageAsync(
		UniqueId uid,
		CancellationToken cancellationToken = default,
		ITransferProgress progress = null
	)
	{
		// Fetch the message from the folder using the UID
		var result = _folder.Value.FindItems(
			new UidSearchQuery(uid).ToExchangeSearchFilter(), new ItemView(int.MaxValue)
			// new SearchQuery(), new ItemView(Int32.MaxValue)
			// new UidSearchQuery(uid), new ItemView(int.MaxValue)
		);


		// If the message is null or doesn't have a body, return null
		if (result == null) return null;

		var message = result.Items.First();

		// Generate StreamContent from the message body
		var mimePart = new MimePart(message.Body.BodyType == BodyType.Text ? "text/plain" : "text/html")
		{
			Content = new MimeContent(new MemoryStream(Encoding.UTF8.GetBytes(message.Body.Text))),
			ContentTransferEncoding = ContentEncoding.Base64,
			ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
			FileName = "message.txt"
		};

		// Return the MimeMessage
		return new MimeMessage(
			new List<InternetAddress> { GetFromAddress(message) },
			GetToAddress(message).ToList(),
			message.Subject,
			mimePart
		);
	}


	public MimeMessage GetMessage(int index, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<MimeMessage> GetMessageAsync(int index, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public MimeEntity GetBodyPart(UniqueId uid, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<MimeEntity> GetBodyPartAsync(UniqueId uid, BodyPart part,
		CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public MimeEntity GetBodyPart(int index, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<MimeEntity> GetBodyPartAsync(int index, BodyPart part,
		CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(UniqueId uid, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(UniqueId uid, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(int index, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(int index, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(UniqueId uid, int offset, int count, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(UniqueId uid, int offset, int count,
		CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(int index, int offset, int count, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(int index, int offset, int count,
		CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(UniqueId uid, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(UniqueId uid, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(int index, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(int index, BodyPart part, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(UniqueId uid, BodyPart part, int offset, int count,
		CancellationToken cancellationToken = new(), ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(UniqueId uid, BodyPart part, int offset, int count,
		CancellationToken cancellationToken = new(), ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(int index, BodyPart part, int offset, int count,
		CancellationToken cancellationToken = new(), ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(int index, BodyPart part, int offset, int count,
		CancellationToken cancellationToken = new(), ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(UniqueId uid, string section, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(UniqueId uid, string section, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(UniqueId uid, string section, int offset, int count,
		CancellationToken cancellationToken = new(), ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(UniqueId uid, string section, int offset, int count,
		CancellationToken cancellationToken = new(), ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(int index, string section, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(int index, string section, CancellationToken cancellationToken = new(),
		ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public Stream GetStream(int index, string section, int offset, int count,
		CancellationToken cancellationToken = new(), ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public async Task<Stream> GetStreamAsync(int index, string section, int offset, int count,
		CancellationToken cancellationToken = new(), ITransferProgress progress = null)
	{
		throw new NotImplementedException();
	}

	public bool Store(UniqueId uid, IStoreFlagsRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<bool> StoreAsync(UniqueId uid, IStoreFlagsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<UniqueId> Store(IList<UniqueId> uids, IStoreFlagsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UniqueId>> StoreAsync(
		IList<UniqueId> uids,
		IStoreFlagsRequest request,
		CancellationToken cancellationToken = new()
	)
	{
		var exchangeFlags = new List<PropertyDefinition>();
		var flags = request.Flags;

		Enum.GetValues(typeof(MessageFlags)).Cast<MessageFlags>().ToList().ForEach(flag =>
		{
			if (flags.HasFlag(flag))
				switch (flag)
				{
					case MessageFlags.Seen:
						exchangeFlags.Add(EmailMessageSchema.IsRead);
						break;
					case MessageFlags.Answered:
						exchangeFlags.Add(EmailMessageSchema.IsResponseRequested);
						break;
					case MessageFlags.Draft:
						exchangeFlags.Add(ItemSchema.IsDraft);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
		});

		var items = _service
			.FindItems(_folder.Value.Id, new UidSearchQuery(uids).ToExchangeSearchFilter(),
				new ItemView(int.MaxValue))
			.Items
			.AsEnumerable();

		var updatedItemsSource = items.Select(item =>
		{
			UpdateEmail(request, item, exchangeFlags);

			return item;
		}).ToList();

		ServiceResponseCollection<UpdateItemResponse>? updatedItems = _service.UpdateItems(
			updatedItemsSource,
			_folder.Value.Id,
			ConflictResolutionMode.AlwaysOverwrite,
			null,
			null
		);

		return updatedItems
			.Select(updatedItem => new UniqueId(uint.Parse(updatedItem.ReturnedItem.Id.UniqueId)))
			.ToList();
	}

	public bool Store(int index, IStoreFlagsRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<bool> StoreAsync(int index, IStoreFlagsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<int> Store(IList<int> indexes, IStoreFlagsRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<int>> StoreAsync(IList<int> indexes, IStoreFlagsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public bool Store(UniqueId uid, IStoreLabelsRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<bool> StoreAsync(UniqueId uid, IStoreLabelsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<UniqueId> Store(IList<UniqueId> uids, IStoreLabelsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UniqueId>> StoreAsync(IList<UniqueId> uids, IStoreLabelsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public bool Store(int index, IStoreLabelsRequest request, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<bool> StoreAsync(int index, IStoreLabelsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<int> Store(IList<int> indexes, IStoreLabelsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<int>> StoreAsync(IList<int> indexes, IStoreLabelsRequest request,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Store(UniqueId uid, IList<Annotation> annotations, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task StoreAsync(UniqueId uid, IList<Annotation> annotations,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Store(IList<UniqueId> uids, IList<Annotation> annotations, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task StoreAsync(IList<UniqueId> uids, IList<Annotation> annotations,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<UniqueId> Store(IList<UniqueId> uids, ulong modseq, IList<Annotation> annotations,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<UniqueId>> StoreAsync(IList<UniqueId> uids, ulong modseq, IList<Annotation> annotations,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Store(int index, IList<Annotation> annotations, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task StoreAsync(int index, IList<Annotation> annotations, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Store(IList<int> indexes, IList<Annotation> annotations, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task StoreAsync(IList<int> indexes, IList<Annotation> annotations,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<int> Store(IList<int> indexes, ulong modseq, IList<Annotation> annotations,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IEnumerator<MimeMessage> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Equals(IMailFolder other)
	{
		if (other is null)
			return false;

		return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
	}

	public void Open(FolderAccess access, CancellationToken cancellationToken)
	{
		_folder.Value.Load();
	}

	public Task OpenAsync(FolderAccess access, CancellationToken cancellationToken)
	{
		return Task.Run(() => Open(access, cancellationToken), cancellationToken);
	}

	public void AddFlags(IList<UniqueId> uids, MessageFlags flags, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
	}

	public Task AddFlagsAsync(IList<UniqueId> uids, MessageFlags flags, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.CompletedTask;
	}

	public void RemoveFlags(IList<UniqueId> uids, MessageFlags flags, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
	}

	public Task RemoveFlagsAsync(IList<UniqueId> uids, MessageFlags flags, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.CompletedTask;
	}

	public void SetFlags(IList<UniqueId> uids, MessageFlags flags, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
	}

	public Task SetFlagsAsync(IList<UniqueId> uids, MessageFlags flags, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.CompletedTask;
	}

	public void ReplaceFlags(IList<UniqueId> uids, MessageFlags flags, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
	}

	public Task ReplaceFlagsAsync(IList<UniqueId> uids, ItemId messageId, MessageFlags flags,
		CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.CompletedTask;
	}

	public IList<IMessageSummary> Fetch(int startIndex, int endIndex, MessageSummaryItems items,
		CancellationToken cancellationToken)
	{
		var itemIds = new List<ItemId>();
		for (var i = startIndex; i <= endIndex; i++)
			itemIds.Add(_folder.Value.FindItems(new ItemView(1) { Offset = i - 1 }).Items[0].Id);

		var propertySet = new PropertySet(items.ToExchangeProperties());
		var result = new List<IMessageSummary>();
		foreach (var item in itemIds)
			result.Add(new ExchangeMessageSummary(
				EmailMessage.Bind(_service, item, propertySet))
			);

		return result;
	}

	public Task<IList<IMessageSummary>> FetchAsync(int startIndex, int endIndex, MessageSummaryItems items,
		CancellationToken cancellationToken)
	{
		return Task.Run(() => Fetch(startIndex, endIndex, items, cancellationToken), cancellationToken);
	}

	private InternetAddress GetFromAddress(Item message)
	{
		return message switch
		{
			MeetingRequest meetingRequest => new MailboxAddress(
				meetingRequest.Organizer.Name,
				meetingRequest.Organizer.Address
			),
			EmailMessage emailMessage => new MailboxAddress(emailMessage.From.Name, emailMessage.From.Address),
			PostItem postItem => new MailboxAddress(postItem.From.Name, postItem.From.Address),
			_ => throw new NotImplementedException()
		};
	}


	private IEnumerable<InternetAddress> GetToAddress(Item message)
	{
		return message switch
		{
			// Get From Address from Item
			MeetingRequest meetingRequest => new List<InternetAddress>
			{
				new MailboxAddress(meetingRequest.Organizer.Name, meetingRequest.Organizer.Address)
			},
			EmailMessage emailMessage => emailMessage.ToRecipients
				.Select(item => new MailboxAddress(item.Name, item.Address)),
			PostItem postItem => new List<InternetAddress>
			{
				new MailboxAddress(postItem.From.Name, postItem.From.Address)
			},
			_ => throw new NotImplementedException()
		};
	}

	private static void UpdateEmail(IStoreFlagsRequest request, Item item, List<PropertyDefinition> exchangeFlags)
	{
		if (item is not EmailMessage emailMessage)
			return;

		if (exchangeFlags.Contains(EmailMessageSchema.IsRead)) emailMessage.IsRead = request.Action == StoreAction.Add;

		if (exchangeFlags.Contains(EmailMessageSchema.IsResponseRequested))
			emailMessage.IsResponseRequested = request.Action == StoreAction.Add;

		if (exchangeFlags.Contains(ItemSchema.IsAssociated))
			emailMessage.IsAssociated = request.Action == StoreAction.Add;

		if (exchangeFlags.Contains(EmailMessageSchema.IsReadReceiptRequested))
			emailMessage.IsReadReceiptRequested = request.Action == StoreAction.Add;

		if (exchangeFlags.Contains(EmailMessageSchema.IsDeliveryReceiptRequested))
			emailMessage.IsDeliveryReceiptRequested = request.Action == StoreAction.Add;
	}

	public void Fetch(
		IEnumerable<UniqueId> uids,
		MessageSummaryItems items,
		CancellationToken cancellationToken,
		ITransferProgress progress,
		Action<UniqueId> callback
	)
	{
		var itemIds = uids.ToList().ConvertAll(uid => new ItemId(uid.Id.ToString()));
		var propertySet = new PropertySet(items.ToExchangeProperties());
		foreach (var item in itemIds)
		{
			if (cancellationToken.IsCancellationRequested)
				break;

			var mailmessage = EmailMessage.Bind(_service, item, propertySet);
			var uniqueId = new UniqueId(uint.Parse(mailmessage.Id.UniqueId));
			callback?.Invoke(uniqueId);
			progress.Report(1, itemIds.Count);

			_fetcheItems.Add(uniqueId, mailmessage);
		}
	}

	public Task FetchAsync(IEnumerable<UniqueId> uids, MessageSummaryItems items, CancellationToken cancellationToken,
		ITransferProgress progress, Action<UniqueId> callback)
	{
		return Task.Run(() => Fetch(uids, items, cancellationToken, progress, callback), cancellationToken);
	}

	public void Fetch(
		UniqueId uid,
		MessageSummaryItems items,
		CancellationToken cancellationToken,
		ITransferProgress progress,
		Action<IMessageSummary> callback
	)
	{
		var item = EmailMessage.Bind(_service, new ItemId(uid.Id.ToString()),
			new PropertySet(items.ToExchangeProperties()));
		callback.Invoke(new ExchangeMessageSummary(item));
		progress.Report(1, 1);
	}

	public Task FetchAsync(UniqueId uid, MessageSummaryItems items, CancellationToken cancellationToken,
		ITransferProgress progress, Action<IMessageSummary> callback)
	{
		return Task.Run(() => Fetch(uid, items, cancellationToken, progress, callback), cancellationToken);
	}

	public void Append(MimeMessage message, MessageFlags flags, DateTimeOffset? internalDate,
		CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
	}

	public Task AppendAsync(MimeMessage message, MessageFlags flags, DateTimeOffset? internalDate,
		CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.CompletedTask;
	}
}