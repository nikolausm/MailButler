using System.Net;
using Microsoft.Exchange.WebServices.Data;

namespace MailButler.Core.Exchange.Extensions;

public static class CredentialsExtensions
{
	public static ExchangeCredentials ToExchangeCredentials(this ICredentials credentials, Uri uri)
	{
		if (credentials == null)
		{
			throw new ArgumentNullException(nameof(credentials));
		}

		if (uri == null)
		{
			throw new ArgumentNullException(nameof(uri));
		}

		var networkCredential = credentials.GetCredential(uri, "Basic");
		if (networkCredential == null)
		{
			throw new ArgumentException("Unable to extract NetworkCredential from ICredentials.", nameof(credentials));
		}

		return new WebCredentials(networkCredential.UserName, networkCredential.Password);	
	}
}