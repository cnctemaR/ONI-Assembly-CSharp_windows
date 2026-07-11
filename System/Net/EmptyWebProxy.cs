using System;

namespace System.Net
{
	[Serializable]
	internal sealed class EmptyWebProxy : IAutoWebProxy, IWebProxy
	{
		public Uri GetProxy(Uri uri)
		{
			return uri;
		}

		public bool IsBypassed(Uri uri)
		{
			return true;
		}

		public ICredentials Credentials
		{
			get
			{
				return this.m_credentials;
			}
			set
			{
				this.m_credentials = value;
			}
		}

		ProxyChain IAutoWebProxy.GetProxies(Uri destination)
		{
			return new DirectProxy(destination);
		}

		[NonSerialized]
		private ICredentials m_credentials;
	}
}
