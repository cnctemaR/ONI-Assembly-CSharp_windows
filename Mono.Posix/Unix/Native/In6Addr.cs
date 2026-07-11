using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[Map]
	public struct In6Addr : IEquatable<In6Addr>
	{
		public unsafe In6Addr(byte[] buffer)
		{
			if (buffer.Length != 16)
			{
				throw new ArgumentException("buffer.Length != 16", "buffer");
			}
			this.addr0 = (this.addr1 = 0UL);
			fixed (ulong* ptr = &this.addr0)
			{
				ulong* ptr2 = ptr;
				Marshal.Copy(buffer, 0, (IntPtr)((void*)ptr2), 16);
			}
		}

		public unsafe void CopyFrom(byte[] source, int startIndex)
		{
			fixed (ulong* ptr = &this.addr0)
			{
				ulong* ptr2 = ptr;
				Marshal.Copy(source, startIndex, (IntPtr)((void*)ptr2), 16);
			}
		}

		public unsafe void CopyTo(byte[] destination, int startIndex)
		{
			fixed (ulong* ptr = &this.addr0)
			{
				Marshal.Copy((IntPtr)((void*)ptr), destination, startIndex, 16);
			}
		}

		public unsafe byte this[int index]
		{
			get
			{
				if (index < 0 || index >= 16)
				{
					throw new ArgumentOutOfRangeException("index", "index < 0 || index >= 16");
				}
				fixed (ulong* ptr = &this.addr0)
				{
					return ((byte*)ptr)[index];
				}
			}
			set
			{
				if (index < 0 || index >= 16)
				{
					throw new ArgumentOutOfRangeException("index", "index < 0 || index >= 16");
				}
				fixed (ulong* ptr = &this.addr0)
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
			return this.addr0.GetHashCode() ^ this.addr1.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is In6Addr && this.Equals((In6Addr)obj);
		}

		public bool Equals(In6Addr value)
		{
			return this.addr0 == value.addr0 && this.addr1 == value.addr1;
		}

		private ulong addr0;

		private ulong addr1;
	}
}
