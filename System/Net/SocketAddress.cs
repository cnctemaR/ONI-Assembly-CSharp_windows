using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace System.Net
{
	public class SocketAddress
	{
		public AddressFamily Family
		{
			get
			{
				return (AddressFamily)((int)this.m_Buffer[0] | ((int)this.m_Buffer[1] << 8));
			}
		}

		public int Size
		{
			get
			{
				return this.m_Size;
			}
		}

		public byte this[int offset]
		{
			get
			{
				if (offset < 0 || offset >= this.Size)
				{
					throw new IndexOutOfRangeException();
				}
				return this.m_Buffer[offset];
			}
			set
			{
				if (offset < 0 || offset >= this.Size)
				{
					throw new IndexOutOfRangeException();
				}
				if (this.m_Buffer[offset] != value)
				{
					this.m_changed = true;
				}
				this.m_Buffer[offset] = value;
			}
		}

		public SocketAddress(AddressFamily family)
			: this(family, 32)
		{
		}

		public SocketAddress(AddressFamily family, int size)
		{
			if (size < 2)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			this.m_Size = size;
			this.m_Buffer = new byte[(size / IntPtr.Size + 2) * IntPtr.Size];
			this.m_Buffer[0] = (byte)family;
			this.m_Buffer[1] = (byte)(family >> 8);
		}

		internal SocketAddress(IPAddress ipAddress)
			: this(ipAddress.AddressFamily, (ipAddress.AddressFamily == AddressFamily.InterNetwork) ? 16 : 28)
		{
			this.m_Buffer[2] = 0;
			this.m_Buffer[3] = 0;
			if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
			{
				this.m_Buffer[4] = 0;
				this.m_Buffer[5] = 0;
				this.m_Buffer[6] = 0;
				this.m_Buffer[7] = 0;
				long scopeId = ipAddress.ScopeId;
				this.m_Buffer[24] = (byte)scopeId;
				this.m_Buffer[25] = (byte)(scopeId >> 8);
				this.m_Buffer[26] = (byte)(scopeId >> 16);
				this.m_Buffer[27] = (byte)(scopeId >> 24);
				byte[] addressBytes = ipAddress.GetAddressBytes();
				for (int i = 0; i < addressBytes.Length; i++)
				{
					this.m_Buffer[8 + i] = addressBytes[i];
				}
				return;
			}
			this.m_Buffer[4] = (byte)ipAddress.m_Address;
			this.m_Buffer[5] = (byte)(ipAddress.m_Address >> 8);
			this.m_Buffer[6] = (byte)(ipAddress.m_Address >> 16);
			this.m_Buffer[7] = (byte)(ipAddress.m_Address >> 24);
		}

		internal SocketAddress(IPAddress ipaddress, int port)
			: this(ipaddress)
		{
			this.m_Buffer[2] = (byte)(port >> 8);
			this.m_Buffer[3] = (byte)port;
		}

		internal IPAddress GetIPAddress()
		{
			if (this.Family == AddressFamily.InterNetworkV6)
			{
				byte[] array = new byte[16];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.m_Buffer[i + 8];
				}
				long num = (long)(((int)this.m_Buffer[27] << 24) + ((int)this.m_Buffer[26] << 16) + ((int)this.m_Buffer[25] << 8) + (int)this.m_Buffer[24]);
				return new IPAddress(array, num);
			}
			if (this.Family == AddressFamily.InterNetwork)
			{
				return new IPAddress((long)((int)(this.m_Buffer[4] & byte.MaxValue) | (((int)this.m_Buffer[5] << 8) & 65280) | (((int)this.m_Buffer[6] << 16) & 16711680) | ((int)this.m_Buffer[7] << 24)) & (long)((ulong)(-1)));
			}
			throw new SocketException(SocketError.AddressFamilyNotSupported);
		}

		internal IPEndPoint GetIPEndPoint()
		{
			IPAddress ipaddress = this.GetIPAddress();
			int num = (((int)this.m_Buffer[2] << 8) & 65280) | (int)this.m_Buffer[3];
			return new IPEndPoint(ipaddress, num);
		}

		internal void CopyAddressSizeIntoBuffer()
		{
			this.m_Buffer[this.m_Buffer.Length - IntPtr.Size] = (byte)this.m_Size;
			this.m_Buffer[this.m_Buffer.Length - IntPtr.Size + 1] = (byte)(this.m_Size >> 8);
			this.m_Buffer[this.m_Buffer.Length - IntPtr.Size + 2] = (byte)(this.m_Size >> 16);
			this.m_Buffer[this.m_Buffer.Length - IntPtr.Size + 3] = (byte)(this.m_Size >> 24);
		}

		internal int GetAddressSizeOffset()
		{
			return this.m_Buffer.Length - IntPtr.Size;
		}

		internal unsafe void SetSize(IntPtr ptr)
		{
			this.m_Size = *(int*)(void*)ptr;
		}

		public override bool Equals(object comparand)
		{
			SocketAddress socketAddress = comparand as SocketAddress;
			if (socketAddress == null || this.Size != socketAddress.Size)
			{
				return false;
			}
			for (int i = 0; i < this.Size; i++)
			{
				if (this[i] != socketAddress[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (this.m_changed)
			{
				this.m_changed = false;
				this.m_hash = 0;
				int num = this.Size & -4;
				int i;
				for (i = 0; i < num; i += 4)
				{
					this.m_hash ^= (int)this.m_Buffer[i] | ((int)this.m_Buffer[i + 1] << 8) | ((int)this.m_Buffer[i + 2] << 16) | ((int)this.m_Buffer[i + 3] << 24);
				}
				if ((this.Size & 3) != 0)
				{
					int num2 = 0;
					int num3 = 0;
					while (i < this.Size)
					{
						num2 |= (int)this.m_Buffer[i] << num3;
						num3 += 8;
						i++;
					}
					this.m_hash ^= num2;
				}
			}
			return this.m_hash;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 2; i < this.Size; i++)
			{
				if (i > 2)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(this[i].ToString(NumberFormatInfo.InvariantInfo));
			}
			return string.Concat(new string[]
			{
				this.Family.ToString(),
				":",
				this.Size.ToString(NumberFormatInfo.InvariantInfo),
				":{",
				stringBuilder.ToString(),
				"}"
			});
		}

		internal const int IPv6AddressSize = 28;

		internal const int IPv4AddressSize = 16;

		internal int m_Size;

		internal byte[] m_Buffer;

		private const int WriteableOffset = 2;

		private const int MaxSize = 32;

		private bool m_changed = true;

		private int m_hash;
	}
}
