using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafeBrotliEncoderHandle : SafeHandle
	{
		public SafeBrotliEncoderHandle()
			: base(IntPtr.Zero, true)
		{
		}

		protected override bool ReleaseHandle()
		{
			Interop.Brotli.BrotliEncoderDestroyInstance(this.handle);
			return true;
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}
	}
}
