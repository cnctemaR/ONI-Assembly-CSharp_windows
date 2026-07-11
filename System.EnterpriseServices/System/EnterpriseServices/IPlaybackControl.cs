using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("51372AFD-CAE7-11CF-BE81-00AA00A2FA25")]
	[ComImport]
	public interface IPlaybackControl
	{
		void FinalClientRetry();

		void FinalServerRetry();
	}
}
