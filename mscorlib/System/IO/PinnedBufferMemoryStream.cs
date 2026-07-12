using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	internal sealed class PinnedBufferMemoryStream : UnmanagedMemoryStream
	{
		internal unsafe PinnedBufferMemoryStream(byte[] array)
		{
			this._array = array;
			this._pinningHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			int num = array.Length;
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(array))
			{
				byte* ptr = reference;
				base.Initialize(ptr, (long)num, (long)num, FileAccess.Read);
			}
		}

		public override int Read(Span<byte> buffer)
		{
			return base.ReadCore(buffer);
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			base.WriteCore(buffer);
		}

		~PinnedBufferMemoryStream()
		{
			this.Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (this._pinningHandle.IsAllocated)
			{
				this._pinningHandle.Free();
			}
			base.Dispose(disposing);
		}

		private byte[] _array;

		private GCHandle _pinningHandle;
	}
}
