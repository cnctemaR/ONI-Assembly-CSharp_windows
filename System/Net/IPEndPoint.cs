using System;
using System.Net.Sockets;

namespace System.Net
{
	[Serializable]
	public class IPEndPoint : EndPoint
	{
		public IPEndPoint(IPAddress address, int port)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			this.Address = address;
			this.Port = port;
		}

		public IPEndPoint(long iaddr, int port)
		{
			this.Address = new IPAddress(iaddr);
			this.Port = port;
		}

		public IPAddress Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		public override global::System.Net.Sockets.AddressFamily AddressFamily
		{
			get
			{
				return this.address.AddressFamily;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new ArgumentOutOfRangeException("Invalid port");
				}
				this.port = value;
			}
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			if (socketAddress == null)
			{
				throw new ArgumentNullException("socketAddress");
			}
			if (socketAddress.Family != this.AddressFamily)
			{
				throw new ArgumentException(string.Concat(new object[] { "The IPEndPoint was created using ", this.AddressFamily, " AddressFamily but SocketAddress contains ", socketAddress.Family, " instead, please use the same type." }));
			}
			int size = socketAddress.Size;
			global::System.Net.Sockets.AddressFamily family = socketAddress.Family;
			global::System.Net.Sockets.AddressFamily addressFamily = family;
			IPEndPoint ipendPoint;
			if (addressFamily != global::System.Net.Sockets.AddressFamily.InterNetwork)
			{
				if (addressFamily != global::System.Net.Sockets.AddressFamily.InterNetworkV6)
				{
					return null;
				}
				if (size < 28)
				{
					return null;
				}
				int num = ((int)socketAddress[2] << 8) + (int)socketAddress[3];
				int num2 = (int)socketAddress[24] + ((int)socketAddress[25] << 8) + ((int)socketAddress[26] << 16) + ((int)socketAddress[27] << 24);
				ushort[] array = new ushort[8];
				for (int i = 0; i < 8; i++)
				{
					array[i] = (ushort)(((int)socketAddress[8 + i * 2] << 8) + (int)socketAddress[8 + i * 2 + 1]);
				}
				ipendPoint = new IPEndPoint(new IPAddress(array, (long)num2), num);
			}
			else
			{
				if (size < 8)
				{
					return null;
				}
				int num = ((int)socketAddress[2] << 8) + (int)socketAddress[3];
				long num3 = ((long)socketAddress[7] << 24) + ((long)socketAddress[6] << 16) + ((long)socketAddress[5] << 8) + (long)socketAddress[4];
				ipendPoint = new IPEndPoint(num3, num);
			}
			return ipendPoint;
		}

		public override SocketAddress Serialize()
		{
			SocketAddress socketAddress = null;
			global::System.Net.Sockets.AddressFamily addressFamily = this.address.AddressFamily;
			if (addressFamily != global::System.Net.Sockets.AddressFamily.InterNetwork)
			{
				if (addressFamily == global::System.Net.Sockets.AddressFamily.InterNetworkV6)
				{
					socketAddress = new SocketAddress(global::System.Net.Sockets.AddressFamily.InterNetworkV6, 28);
					socketAddress[2] = (byte)((this.port >> 8) & 255);
					socketAddress[3] = (byte)(this.port & 255);
					byte[] addressBytes = this.address.GetAddressBytes();
					for (int i = 0; i < 16; i++)
					{
						socketAddress[8 + i] = addressBytes[i];
					}
					socketAddress[24] = (byte)(this.address.ScopeId & 255L);
					socketAddress[25] = (byte)((this.address.ScopeId >> 8) & 255L);
					socketAddress[26] = (byte)((this.address.ScopeId >> 16) & 255L);
					socketAddress[27] = (byte)((this.address.ScopeId >> 24) & 255L);
				}
			}
			else
			{
				socketAddress = new SocketAddress(global::System.Net.Sockets.AddressFamily.InterNetwork, 16);
				socketAddress[2] = (byte)((this.port >> 8) & 255);
				socketAddress[3] = (byte)(this.port & 255);
				long internalIPv4Address = this.address.InternalIPv4Address;
				socketAddress[4] = (byte)(internalIPv4Address & 255L);
				socketAddress[5] = (byte)((internalIPv4Address >> 8) & 255L);
				socketAddress[6] = (byte)((internalIPv4Address >> 16) & 255L);
				socketAddress[7] = (byte)((internalIPv4Address >> 24) & 255L);
			}
			return socketAddress;
		}

		public override string ToString()
		{
			return this.address.ToString() + ":" + this.port;
		}

		public override bool Equals(object obj)
		{
			IPEndPoint ipendPoint = obj as IPEndPoint;
			return ipendPoint != null && ipendPoint.port == this.port && ipendPoint.address.Equals(this.address);
		}

		public override int GetHashCode()
		{
			return this.address.GetHashCode() + this.port;
		}

		public const int MaxPort = 65535;

		public const int MinPort = 0;

		private IPAddress address;

		private int port;
	}
}
