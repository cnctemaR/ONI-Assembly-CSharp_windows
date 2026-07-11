using System;
using System.IO;
using System.Security.Permissions;

namespace Microsoft.Win32.SafeHandles
{
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
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
