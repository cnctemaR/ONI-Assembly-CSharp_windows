using System;

namespace System.Net
{
	public interface ICredentials
	{
		NetworkCredential GetCredential(global::System.Uri uri, string authType);
	}
}
