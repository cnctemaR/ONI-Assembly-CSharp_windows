using System;
using System.Runtime.InteropServices;

namespace System.Net
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecPkgContext_Sizes
	{
		internal unsafe SecPkgContext_Sizes(byte[] memory)
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
						this.cbMaxToken = (int)((uint)Marshal.ReadInt32(intPtr));
						this.cbMaxSignature = (int)((uint)Marshal.ReadInt32(intPtr, 4));
						this.cbBlockSize = (int)((uint)Marshal.ReadInt32(intPtr, 8));
						this.cbSecurityTrailer = (int)((uint)Marshal.ReadInt32(intPtr, 12));
					}
					catch (OverflowException)
					{
						NetEventSource.Fail(this, "Negative size.", ".ctor");
						throw;
					}
				}
			}
		}

		public readonly int cbMaxToken;

		public readonly int cbMaxSignature;

		public readonly int cbBlockSize;

		public readonly int cbSecurityTrailer;

		public static readonly int SizeOf = Marshal.SizeOf<SecPkgContext_Sizes>();
	}
}
