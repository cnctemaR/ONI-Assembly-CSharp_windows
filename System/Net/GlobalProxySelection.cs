using System;

namespace System.Net
{
	[Obsolete("Use WebRequest.DefaultProxy instead")]
	public class GlobalProxySelection
	{
		public static IWebProxy Select
		{
			get
			{
				return WebRequest.DefaultWebProxy;
			}
			set
			{
				WebRequest.DefaultWebProxy = value;
			}
		}

		public static IWebProxy GetEmptyWebProxy()
		{
			return new GlobalProxySelection.EmptyWebProxy();
		}

		internal class EmptyWebProxy : IWebProxy
		{
			internal EmptyWebProxy()
			{
			}

			public ICredentials Credentials
			{
				get
				{
					return this.credentials;
				}
				set
				{
					this.credentials = value;
				}
			}

			public global::System.Uri GetProxy(global::System.Uri destination)
			{
				return destination;
			}

			public bool IsBypassed(global::System.Uri host)
			{
				return true;
			}

			private ICredentials credentials;
		}
	}
}
