using System;
using System.IO;
using System.Security.Permissions;

namespace Microsoft.Win32.SafeHandles
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	public sealed class SafePipeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafePipeHandle(IntPtr preexistingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			this.handle = preexistingHandle;
		}

		protected override bool ReleaseHandle()
		{
			MonoIOError monoIOError;
			return MonoIO.Close(this.handle, out monoIOError);
		}
	}
}
