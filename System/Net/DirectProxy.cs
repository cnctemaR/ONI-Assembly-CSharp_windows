using System;

namespace System.Net
{
	internal class DirectProxy : ProxyChain
	{
		internal DirectProxy(Uri destination)
			: base(destination)
		{
		}

		protected override bool GetNextProxy(out Uri proxy)
		{
			proxy = null;
			if (this.m_ProxyRetrieved)
			{
				return false;
			}
			this.m_ProxyRetrieved = true;
			return true;
		}

		private bool m_ProxyRetrieved;
	}
}
