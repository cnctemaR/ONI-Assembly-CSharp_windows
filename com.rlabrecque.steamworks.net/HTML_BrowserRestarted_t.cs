using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4527)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_BrowserRestarted_t
	{
		public const int k_iCallback = 4527;

		public HHTMLBrowser unBrowserHandle;

		public HHTMLBrowser unOldBrowserHandle;
	}
}
