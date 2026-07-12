using System;

namespace System.Net.Security
{
	internal sealed class SafeFreeCredential_SECURITY : SafeFreeCredentials
	{
		protected override bool ReleaseHandle()
		{
			return global::Interop.SspiCli.FreeCredentialsHandle(ref this._handle) == 0;
		}
	}
}
