using System;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
	internal sealed class SafeSspiAuthDataHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeSspiAuthDataHandle()
			: base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			return global::Interop.SspiCli.SspiFreeAuthIdentity(this.handle) == global::Interop.SECURITY_STATUS.OK;
		}
	}
}
