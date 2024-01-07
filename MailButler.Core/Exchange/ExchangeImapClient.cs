using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MailButler.Core.Exchange.Extensions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Proxy;
using MailKit.Security;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace MailButler.Core.Exchange;

public sealed class ExchangeImapClient : IImapClient
{
	private readonly ILogger<ExchangeImapClient> _logger;
	private readonly string _username;
	private readonly string _password;
	private readonly string _serverUrl;
	private readonly ExchangeService _service;
	private readonly CancellationTokenSource _cancellationTokenSource = new();
	private string? _host;
	private int _port;
	private SecureSocketOptions? _options;
	private IMailFolder? _inbox;

	public ExchangeImapClient(ILogger<ExchangeImapClient> logger, string username, string password, string serverUrl,
		ExchangeVersion exchangeVersion = ExchangeVersion.Exchange2013_SP1)
	{
		_logger = logger;
		_username = username;
		_password = password;
		_serverUrl = serverUrl;
		_service = new ExchangeService(exchangeVersion);
		_service.Credentials = new WebCredentials(username, password);
		_service.Url = new Uri(serverUrl);
	}

	public void Connect(string host, int port, bool useSsl, CancellationToken cancellationToken = new())
	{
		Connect(host, port, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable, cancellationToken);
	}

	public async Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = new())
	{
		await ConnectAsync(host, port, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable, cancellationToken);
	}

	public void Connect(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken)
	{
		_host = host;
		_port = port;
		_options = options;
	}

	public Task ConnectAsync(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken)
	{
		Connect(host, port, options, cancellationToken);
		return Task.CompletedTask;
	}

	public void Connect(Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto,
		CancellationToken cancellationToken = default)
	{
		// No-op for Exchange implementation
	}

	public async Task ConnectAsync(Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto,
		CancellationToken cancellationToken = default)
	{
		// No-op for Exchange implementation
	}

	public void Connect(Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto,
		CancellationToken cancellationToken = default)
	{
		// No-op for Exchange implementation
	}

	public async Task ConnectAsync(Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto,
		CancellationToken cancellationToken = new())
	{
		// No-op for Exchange implementation
	}

	public void Authenticate(ICredentials credentials, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task AuthenticateAsync(ICredentials credentials, CancellationToken cancellationToken = new())
	{

		await Task.Run(() => Authenticate(Encoding.UTF8, credentials, cancellationToken));
	}

	public void Authenticate(Encoding encoding, ICredentials credentials,
		CancellationToken cancellationToken = new())
	{
		try
		{
			var service = new ExchangeService();
			service.Credentials = credentials.ToExchangeCredentials(new Uri(_serverUrl));
			service.Url = new Uri(_serverUrl);
			service.FindFolders(WellKnownFolderName.Inbox, new FolderView(1));
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error authenticating with Exchange");
			throw;
		}
	}

	public async Task AuthenticateAsync(Encoding encoding, ICredentials credentials,
		CancellationToken cancellationToken = new())
	{
		await Task.Run(() => Authenticate(Encoding.UTF8, credentials, cancellationToken));
	}

	public void Authenticate(Encoding encoding, string userName, string password,
		CancellationToken cancellationToken = new())
	{
		Authenticate(Encoding.UTF8, new NetworkCredential(userName, password), cancellationToken);
	}

	public async Task AuthenticateAsync(Encoding encoding, string userName, string password,
		CancellationToken cancellationToken = new())
	{
		await Task.Run(() => Authenticate(Encoding.UTF8, new NetworkCredential(userName, password), cancellationToken));
	}

	public void Authenticate(string userName, string password, CancellationToken cancellationToken)
	{
		Authenticate(Encoding.UTF8, new NetworkCredential(userName, password), cancellationToken);
	}

	public async Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
	{
		await Task.Run(() => Authenticate(Encoding.UTF8, new NetworkCredential(userName, password), cancellationToken));
	}

	public void Authenticate(SaslMechanism mechanism, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task AuthenticateAsync(SaslMechanism mechanism, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Disconnect(bool quit, CancellationToken cancellationToken)
	{
		_cancellationTokenSource.Cancel();
	}

	public Task DisconnectAsync(bool quit, CancellationToken cancellationToken)
	{
		_cancellationTokenSource.Cancel();
		return Task.CompletedTask;
	}

	public IMailFolder GetFolder(SpecialFolder folder, CancellationToken cancellationToken)
	{
		return folder switch
		{
			SpecialFolder.Trash => new ExchangeMailFolder(_service, WellKnownFolderName.DeletedItems),
			SpecialFolder.Sent => new ExchangeMailFolder(_service, WellKnownFolderName.SentItems),
			SpecialFolder.Drafts => new ExchangeMailFolder(_service, WellKnownFolderName.Drafts),
			SpecialFolder.All => new ExchangeMailFolder(_service, WellKnownFolderName.MsgFolderRoot),
			SpecialFolder.Archive => new ExchangeMailFolder(_service, WellKnownFolderName.ArchiveRoot),
			SpecialFolder.Junk => new ExchangeMailFolder(_service, WellKnownFolderName.JunkEmail),
			_ => throw new NotSupportedException()
		};
	}

	public IList<IMailFolder> GetFolders(SpecialFolder folder, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Array.Empty<IMailFolder>();
	}

	public Task<IList<IMailFolder>> GetFoldersAsync(SpecialFolder folder, CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.FromResult<IList<IMailFolder>>(new IMailFolder[0]);
	}

	public void Idle(CancellationToken cancellationToken)
	{
		var inbox = Folder.Bind(_service, WellKnownFolderName.Inbox);
		var view = new ItemView(1);
		var searchFilter = new SearchFilter.SearchFilterCollection(
			LogicalOperator.And,
			new SearchFilter.IsEqualTo(
				EmailMessageSchema.IsRead, 
				false
			)
		);
		var count = inbox.FindItems(searchFilter, view).TotalCount;
		while (!cancellationToken.IsCancellationRequested)
		{
			if (count > 0)
			{
				OnNewMessage();
			}

			// Should be an configuration option
			Thread.Sleep(10000);
			count = inbox.FindItems(searchFilter, view).TotalCount;
		}
	}

	private void OnNewMessage()
	{
		NewMessage?.Invoke(this, EventArgs.Empty);
	}

	public Task IdleAsync(CancellationToken cancellationToken)
	{
		return Task.Run(() => Idle(cancellationToken), cancellationToken);
	}

	public void NoOp
		(CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
	}

	public Task NoOpAsync(CancellationToken cancellationToken)
	{
		// No-op for Exchange implementation
		return Task.CompletedTask;
	}

	public object SyncRoot { get; }
	public SslProtocols SslProtocols { get; set; }
	public CipherSuitesPolicy SslCipherSuitesPolicy { get; set; }
	public TlsCipherSuite? SslCipherSuite { get; }
	public X509CertificateCollection ClientCertificates { get; set; }
	public bool CheckCertificateRevocation { get; set; }
	public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
	public IPEndPoint LocalEndPoint { get; set; }
	public IProxyClient ProxyClient { get; set; }
	public HashSet<string> AuthenticationMechanisms { get; }
	public bool IsAuthenticated { get; }
	public bool IsConnected { get; }
	public bool IsSecure { get; }
	public bool IsEncrypted { get; }
	public bool IsSigned { get; }
	public SslProtocols SslProtocol { get; }
	public CipherAlgorithmType? SslCipherAlgorithm { get; }
	public int? SslCipherStrength { get; }
	public HashAlgorithmType? SslHashAlgorithm { get; }
	public int? SslHashStrength { get; }
	public ExchangeAlgorithmType? SslKeyExchangeAlgorithm { get; }
	public int? SslKeyExchangeStrength { get; }
	public int Timeout { get; set; }
	public event EventHandler<ConnectedEventArgs>? Connected;

	public void Dispose()
	{
		_cancellationTokenSource.Dispose();
	}

	public event EventHandler<DisconnectedEventArgs> Disconnected;
	public event EventHandler<AuthenticatedEventArgs>? Authenticated;


	public event EventHandler<EventArgs> NewMessage;

	protected void OnDisconnected()
	{
		//Disconnected?.Invoke(this, new DisconnectedEventArgs());
		Disconnected?.Invoke(this, new DisconnectedEventArgs(
				_host,
				_port,
				_options ?? SecureSocketOptions.Auto,
				true
			)
		);
	}

	public void EnableQuickResync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task EnableQuickResyncAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IMailFolder GetFolder(SpecialFolder folder)
	{
		// Map the SpecialFolder enum value to a WellKnownFolderName enum value
		WellKnownFolderName wellKnownFolderName;
		switch (folder)
		{
			case SpecialFolder.All:
				wellKnownFolderName = WellKnownFolderName.Inbox;
				break;
			case SpecialFolder.Archive:
				wellKnownFolderName = WellKnownFolderName.ArchiveMsgFolderRoot;
				break;
			case SpecialFolder.Drafts:
				wellKnownFolderName = WellKnownFolderName.Drafts;
				break;
			case SpecialFolder.Junk:
				wellKnownFolderName = WellKnownFolderName.JunkEmail;
				break;
			case SpecialFolder.Sent:
				wellKnownFolderName = WellKnownFolderName.SentItems;
				break;
			case SpecialFolder.Trash:
				wellKnownFolderName = WellKnownFolderName.DeletedItems;
				break;
			default:
				throw new NotSupportedException($"The {folder} special folder is not supported.");
		}

		// Bind to the folder using the current IMAP client instance
		var folderName = Enum.GetName(typeof(SpecialFolder), folder);
		var exchangeFolder = _service.FindFolders(wellKnownFolderName, new FolderView(100)).FirstOrDefault();
		if (exchangeFolder == null)
		{
			throw new InvalidOperationException($"The {folder} folder could not be found.");
		}
		
		// Open the folder and return it
		return new ExchangeMailFolder(_service, wellKnownFolderName);
	}
	

	public IMailFolder GetFolder(FolderNamespace @namespace)
	{
		throw new NotImplementedException();
	}

	public IList<IMailFolder> GetFolders(FolderNamespace @namespace, bool subscribedOnly,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<IMailFolder>> GetFoldersAsync(FolderNamespace @namespace, bool subscribedOnly,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IList<IMailFolder> GetFolders(FolderNamespace @namespace, StatusItems items = StatusItems.None, bool subscribedOnly = false,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IList<IMailFolder>> GetFoldersAsync(FolderNamespace @namespace, StatusItems items = StatusItems.None, bool subscribedOnly = false,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public IMailFolder GetFolder(string path, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<IMailFolder> GetFolderAsync(string path, CancellationToken cancellationToken = new())
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

	public async Task<MetadataCollection> GetMetadataAsync(IEnumerable<MetadataTag> tags, CancellationToken cancellationToken = new())
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

	public FolderNamespaceCollection PersonalNamespaces { get; }
	public FolderNamespaceCollection SharedNamespaces { get; }
	public FolderNamespaceCollection OtherNamespaces { get; }
	public bool SupportsQuotas { get; }
	public HashSet<ThreadingAlgorithm> ThreadingAlgorithms { get; }

	public IMailFolder Inbox => _inbox ??= new ExchangeMailFolder(_service, WellKnownFolderName.Inbox);

	public event EventHandler<AlertEventArgs>? Alert;
	public event EventHandler<FolderCreatedEventArgs>? FolderCreated;
	public event EventHandler<MetadataChangedEventArgs>? MetadataChanged;
	public void Compress(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task CompressAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void EnableUTF8(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task EnableUTF8Async(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public ImapImplementation Identify(ImapImplementation clientImplementation,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task<ImapImplementation> IdentifyAsync(ImapImplementation clientImplementation,
		CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Idle(CancellationToken doneToken, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task IdleAsync(CancellationToken doneToken, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void Notify(bool status, IList<ImapEventGroup> eventGroups, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task NotifyAsync(bool status, IList<ImapEventGroup> eventGroups, CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public void DisableNotify(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public async Task DisableNotifyAsync(CancellationToken cancellationToken = new())
	{
		throw new NotImplementedException();
	}

	public ImapCapabilities Capabilities { get; set; }
	public uint? AppendLimit { get; }
	public int InternationalizationLevel { get; }
	public AccessRights Rights { get; }
	public bool IsIdle { get; }
}