using System;
using System.Net.Sockets;

namespace System.Net
{
	public class SocketAddress
	{
		public SocketAddress(global::System.Net.Sockets.AddressFamily family, int size)
		{
			if (size < 2)
			{
				throw new ArgumentOutOfRangeException("size is too small");
			}
			this.data = new byte[size];
			this.data[0] = (byte)family;
			this.data[1] = (byte)(family >> 8);
		}

		public SocketAddress(global::System.Net.Sockets.AddressFamily family)
			: this(family, 32)
		{
		}

		public global::System.Net.Sockets.AddressFamily Family
		{
			get
			{
				return (global::System.Net.Sockets.AddressFamily)((int)this.data[0] + ((int)this.data[1] << 8));
			}
		}

		public int Size
		{
			get
			{
				return this.data.Length;
			}
		}

		public byte this[int offset]
		{
			get
			{
				return this.data[offset];
			}
			set
			{
				this.data[offset] = value;
			}
		}

		public override string ToString()
		{
			string text = ((global::System.Net.Sockets.AddressFamily)this.data[0]).ToString();
			int num = this.data.Length;
			string text2 = string.Concat(new object[] { text, ":", num, ":{" });
			for (int i = 2; i < num; i++)
			{
				int num2 = (int)this.data[i];
				text2 += num2;
				if (i < num - 1)
				{
					text2 += ",";
				}
			}
			return text2 + "}";
		}

		public override bool Equals(object obj)
		{
			SocketAddress socketAddress = obj as SocketAddress;
			if (socketAddress != null && socketAddress.data.Length == this.data.Length)
			{
				byte[] array = socketAddress.data;
				for (int i = 0; i < this.data.Length; i++)
				{
					if (array[i] != this.data[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.data.Length; i++)
			{
				num += (int)this.data[i] + i;
			}
			return num;
		}

		private byte[] data;
	}
}
