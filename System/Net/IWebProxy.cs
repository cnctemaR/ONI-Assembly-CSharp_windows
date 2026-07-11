using System;

namespace System.Net
{
	public interface IWebProxy
	{
		ICredentials Credentials { get; set; }

		global::System.Uri GetProxy(global::System.Uri destination);

		bool IsBypassed(global::System.Uri host);
	}
}
