using System;

namespace System.Net
{
	internal class SystemNetworkCredential : NetworkCredential
	{
		private SystemNetworkCredential()
			: base(string.Empty, string.Empty, string.Empty)
		{
		}

		internal static readonly SystemNetworkCredential defaultCredential = new SystemNetworkCredential();
	}
}
