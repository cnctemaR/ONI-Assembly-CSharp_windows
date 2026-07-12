using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	internal sealed class SafeBrotliDecoderHandle : SafeHandle
	{
		public SafeBrotliDecoderHandle()
			: base(IntPtr.Zero, true)
		{
		}

		protected override bool ReleaseHandle()
		{
			Interop.Brotli.BrotliDecoderDestroyInstance(this.handle);
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
