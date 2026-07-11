using System;
using System.Collections.Generic;

namespace System.Net
{
	internal interface IWebProxyFinder : IDisposable
	{
		bool GetProxies(Uri destination, out IList<string> proxyList);

		void Abort();

		void Reset();

		bool IsValid { get; }
	}
}
