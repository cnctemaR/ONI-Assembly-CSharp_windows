using System;

namespace System.Net.Security
{
	internal sealed class SafeDeleteContext_SECURITY : SafeDeleteContext
	{
		internal SafeDeleteContext_SECURITY()
		{
		}

		protected override bool ReleaseHandle()
		{
			if (this._EffectiveCredential != null)
			{
				this._EffectiveCredential.DangerousRelease();
			}
			return global::Interop.SspiCli.DeleteSecurityContext(ref this._handle) == 0;
		}
	}
}
