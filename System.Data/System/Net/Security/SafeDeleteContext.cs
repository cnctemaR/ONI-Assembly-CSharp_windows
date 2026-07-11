using System;
using System.Runtime.InteropServices;

namespace System.Net.Security
{
	internal abstract class SafeDeleteContext : SafeHandle
	{
		protected SafeDeleteContext(SafeFreeCredentials credential)
			: base(IntPtr.Zero, true)
		{
			bool flag = false;
			this._credential = credential;
			this._credential.DangerousAddRef(ref flag);
		}

		public override bool IsInvalid
		{
			get
			{
				return this._credential == null;
			}
		}

		protected override bool ReleaseHandle()
		{
			this._credential.DangerousRelease();
			this._credential = null;
			return true;
		}

		private SafeFreeCredentials _credential;
	}
}
