using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[Guid("51372AFD-CAE7-11CF-BE81-00AA00A2FA25")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IPlaybackControl
	{
		void FinalClientRetry();

		void FinalServerRetry();
	}
}
