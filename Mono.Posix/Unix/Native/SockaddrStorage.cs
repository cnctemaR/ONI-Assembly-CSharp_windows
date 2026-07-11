using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class SockaddrStorage : Sockaddr, IEquatable<SockaddrStorage>
	{
		public byte[] data { get; set; }

		public long data_len { get; set; }

		internal override byte[] DynamicData()
		{
			return this.data;
		}

		internal override long GetDynamicLength()
		{
			return this.data_len;
		}

		internal override void SetDynamicLength(long value)
		{
			this.data_len = value;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_SockaddrStorage_get_size", SetLastError = true)]
		private static extern int get_size();

		public SockaddrStorage()
			: base((SockaddrType)32769, UnixAddressFamily.AF_UNSPEC)
		{
			this.data = new byte[SockaddrStorage.default_size];
			this.data_len = 0L;
		}

		public SockaddrStorage(int size)
			: base((SockaddrType)32769, UnixAddressFamily.AF_UNSPEC)
		{
			this.data = new byte[size];
			this.data_len = 0L;
		}

		public unsafe void SetTo(Sockaddr address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			long nativeSize = address.GetNativeSize();
			if (nativeSize > (long)this.data.Length)
			{
				this.data = new byte[nativeSize];
			}
			byte[] array;
			byte* ptr;
			if ((array = this.data) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			if (!NativeConvert.TryCopy(address, (IntPtr)((void*)ptr)))
			{
				throw new ArgumentException("Failed to convert to native struct", "address");
			}
			array = null;
			this.data_len = nativeSize;
			base.sa_family = address.sa_family;
		}

		public unsafe void CopyTo(Sockaddr address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (this.data_len < 0L || this.data_len > (long)this.data.Length)
			{
				throw new ArgumentException("data_len < 0 || data_len > data.Length", "this");
			}
			byte[] array;
			byte* ptr;
			if ((array = this.data) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			if (!NativeConvert.TryCopy((IntPtr)((void*)ptr), this.data_len, address))
			{
				throw new ArgumentException("Failed to convert from native struct", "this");
			}
			array = null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{{sa_family={0}, data_len={1}, data=(", base.sa_family, this.data_len);
			int num = 0;
			while ((long)num < this.data_len)
			{
				if (num != 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(this.data[num].ToString("x2"));
				num++;
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			int num = 4660;
			int num2 = 0;
			while ((long)num2 < this.data_len)
			{
				num += num2 ^ (int)this.data[num2];
				num2++;
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			return obj is SockaddrStorage && this.Equals((SockaddrStorage)obj);
		}

		public bool Equals(SockaddrStorage value)
		{
			if (value == null)
			{
				return false;
			}
			if (this.data_len != value.data_len)
			{
				return false;
			}
			int num = 0;
			while ((long)num < this.data_len)
			{
				if (this.data[num] != value.data[num])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		private static readonly int default_size = SockaddrStorage.get_size();
	}
}
