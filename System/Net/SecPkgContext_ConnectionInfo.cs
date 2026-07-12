using System;
using System.Runtime.InteropServices;

namespace System.Net
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecPkgContext_ConnectionInfo
	{
		internal unsafe SecPkgContext_ConnectionInfo(byte[] nativeBuffer)
		{
			fixed (byte[] array = nativeBuffer)
			{
				void* ptr;
				if (nativeBuffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = (void*)(&array[0]);
				}
				try
				{
					IntPtr intPtr = new IntPtr(ptr);
					this.Protocol = Marshal.ReadInt32(intPtr);
					this.DataCipherAlg = Marshal.ReadInt32(intPtr, 4);
					this.DataKeySize = Marshal.ReadInt32(intPtr, 8);
					this.DataHashAlg = Marshal.ReadInt32(intPtr, 12);
					this.DataHashKeySize = Marshal.ReadInt32(intPtr, 16);
					this.KeyExchangeAlg = Marshal.ReadInt32(intPtr, 20);
					this.KeyExchKeySize = Marshal.ReadInt32(intPtr, 24);
				}
				catch (OverflowException)
				{
					NetEventSource.Fail(this, "Negative size", ".ctor");
					throw;
				}
			}
		}

		public readonly int Protocol;

		public readonly int DataCipherAlg;

		public readonly int DataKeySize;

		public readonly int DataHashAlg;

		public readonly int DataHashKeySize;

		public readonly int KeyExchangeAlg;

		public readonly int KeyExchKeySize;
	}
}
