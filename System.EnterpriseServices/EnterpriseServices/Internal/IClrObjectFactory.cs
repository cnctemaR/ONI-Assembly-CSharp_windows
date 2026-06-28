using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices.Internal
{
	[Guid("ecabafd2-7f19-11d2-978e-0000f8757e2a")]
	public interface IClrObjectFactory
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		object CreateFromAssembly(string assembly, string type, string mode);

		[DispId(4)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		object CreateFromMailbox(string Mailbox, string Mode);

		[DispId(2)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		object CreateFromVroot(string VrootUrl, string Mode);

		[DispId(3)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		object CreateFromWsdl(string WsdlUrl, string Mode);
	}
}
