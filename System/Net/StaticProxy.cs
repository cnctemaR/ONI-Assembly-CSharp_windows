using System;

namespace System.Net
{
	internal class StaticProxy : ProxyChain
	{
		internal StaticProxy(Uri destination, Uri proxy)
			: base(destination)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}
			this.m_Proxy = proxy;
		}

		protected override bool GetNextProxy(out Uri proxy)
		{
			proxy = this.m_Proxy;
			if (proxy == null)
			{
				return false;
			}
			this.m_Proxy = null;
			return true;
		}

		private Uri m_Proxy;
	}
}
