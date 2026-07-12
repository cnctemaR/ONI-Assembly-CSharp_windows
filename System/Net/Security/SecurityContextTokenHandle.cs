using System;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
	internal sealed class SecurityContextTokenHandle : CriticalHandleZeroOrMinusOneIsInvalid
	{
		private SecurityContextTokenHandle()
		{
		}

		internal IntPtr DangerousGetHandle()
		{
			return this.handle;
		}

		protected override bool ReleaseHandle()
		{
			return this.IsInvalid || Interlocked.Increment(ref this._disposed) != 1 || global::Interop.Kernel32.CloseHandle(this.handle);
		}

		private int _disposed;
	}
}
