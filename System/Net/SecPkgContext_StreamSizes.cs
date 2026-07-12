using System;
using System.Runtime.InteropServices;

namespace System.Net
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecPkgContext_StreamSizes
	{
		internal unsafe SecPkgContext_StreamSizes(byte[] memory)
		{
			fixed (byte[] array = memory)
			{
				void* ptr;
				if (memory == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = (void*)(&array[0]);
				}
				IntPtr intPtr = new IntPtr(ptr);
				checked
				{
					try
					{
						this.cbHeader = (int)((uint)Marshal.ReadInt32(intPtr));
						this.cbTrailer = (int)((uint)Marshal.ReadInt32(intPtr, 4));
						this.cbMaximumMessage = (int)((uint)Marshal.ReadInt32(intPtr, 8));
						this.cBuffers = (int)((uint)Marshal.ReadInt32(intPtr, 12));
						this.cbBlockSize = (int)((uint)Marshal.ReadInt32(intPtr, 16));
					}
					catch (OverflowException)
					{
						NetEventSource.Fail(this, "Negative size.", ".ctor");
						throw;
					}
				}
			}
		}

		public int cbHeader;

		public int cbTrailer;

		public int cbMaximumMessage;

		public int cBuffers;

		public int cbBlockSize;

		public static readonly int SizeOf = Marshal.SizeOf<SecPkgContext_StreamSizes>();
	}
}
