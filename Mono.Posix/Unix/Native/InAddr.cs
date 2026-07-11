using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public struct InAddr : IEquatable<InAddr>
	{
		public unsafe InAddr(byte b0, byte b1, byte b2, byte b3)
		{
			this.s_addr = 0U;
			fixed (uint* ptr = &this.s_addr)
			{
				byte* ptr2 = (byte*)ptr;
				*ptr2 = b0;
				ptr2[1] = b1;
				ptr2[2] = b2;
				ptr2[3] = b3;
			}
		}

		public unsafe InAddr(byte[] buffer)
		{
			if (buffer.Length != 4)
			{
				throw new ArgumentException("buffer.Length != 4", "buffer");
			}
			this.s_addr = 0U;
			fixed (uint* ptr = &this.s_addr)
			{
				uint* ptr2 = ptr;
				Marshal.Copy(buffer, 0, (IntPtr)((void*)ptr2), 4);
			}
		}

		public unsafe void CopyFrom(byte[] source, int startIndex)
		{
			fixed (uint* ptr = &this.s_addr)
			{
				uint* ptr2 = ptr;
				Marshal.Copy(source, startIndex, (IntPtr)((void*)ptr2), 4);
			}
		}

		public unsafe void CopyTo(byte[] destination, int startIndex)
		{
			fixed (uint* ptr = &this.s_addr)
			{
				Marshal.Copy((IntPtr)((void*)ptr), destination, startIndex, 4);
			}
		}

		public unsafe byte this[int index]
		{
			get
			{
				if (index < 0 || index >= 4)
				{
					throw new ArgumentOutOfRangeException("index", "index < 0 || index >= 4");
				}
				fixed (uint* ptr = &this.s_addr)
				{
					return ((byte*)ptr)[index];
				}
			}
			set
			{
				if (index < 0 || index >= 4)
				{
					throw new ArgumentOutOfRangeException("index", "index < 0 || index >= 4");
				}
				fixed (uint* ptr = &this.s_addr)
				{
					((byte*)ptr)[index] = value;
				}
			}
		}

		public override string ToString()
		{
			return NativeConvert.ToIPAddress(this).ToString();
		}

		public override int GetHashCode()
		{
			return this.s_addr.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is InAddr && this.Equals((InAddr)obj);
		}

		public bool Equals(InAddr value)
		{
			return this.s_addr == value.s_addr;
		}

		public uint s_addr;
	}
}
