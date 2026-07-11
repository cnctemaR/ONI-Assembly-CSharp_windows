using System;
using System.Reflection;

namespace System.Runtime.InteropServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("f1c3bf76-c3e4-11d3-88e7-00902754c43a")]
	[ComVisible(true)]
	public interface ITypeLibImporterNotifySink
	{
		void ReportEvent(ImporterEventKind eventKind, int eventCode, string eventMsg);

		Assembly ResolveRef([MarshalAs(UnmanagedType.Interface)] object typeLib);
	}
}
