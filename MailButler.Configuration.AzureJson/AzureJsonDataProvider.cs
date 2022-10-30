using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace MailButler.Configuration.AzureJson;

public sealed class AzureJsonDataProvider<T> : IDataProvider<T>
{
	private readonly string _containerRoot;
	private readonly string _fileName;

	public AzureJsonDataProvider(string containerRoot, string fileName)
	{
		_containerRoot = containerRoot;
		_fileName = fileName;
	}

	public T Data()
	{
		return JsonConvert.DeserializeObject<T>(
			ReadAllText(
				_fileName
			)
		)!;
	}

	private string ReadAllText(
		string fileName
	)
	{
		var target = Path.Combine(
			Path.GetTempPath(),
			fileName
		);

		new BlobClient(
			new Uri(
				$"{_containerRoot}/{fileName}"
			)
		).DownloadTo(
			target
		);
		try
		{
			return File.ReadAllText(target);
		}
		finally
		{
			File.Delete(target);
		}
	}
}