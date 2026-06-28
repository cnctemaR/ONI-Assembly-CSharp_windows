using System;
using System.Runtime.Remoting.Messaging;

namespace Mono.Remoting.Channels.Unix
{
	internal class MethodCallHeaderHandler
	{
		public MethodCallHeaderHandler(string uri)
		{
			this._uri = uri;
		}

		public object HandleHeaders(Header[] headers)
		{
			return this._uri;
		}

		private string _uri;
	}
}
