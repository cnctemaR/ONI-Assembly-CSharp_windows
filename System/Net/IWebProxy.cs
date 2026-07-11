using System;

namespace System.Net
{
	public interface IWebProxy
	{
		Uri GetProxy(Uri destination);

		bool IsBypassed(Uri host);

		ICredentials Credentials { get; set; }
	}
}
