using System;
using System.Globalization;
using System.Net.Sockets;

namespace System.Net
{
	[Serializable]
	public class IPEndPoint : EndPoint
	{
		public override AddressFamily AddressFamily
		{
			get
			{
				return this.m_Address.AddressFamily;
			}
		}

		public IPEndPoint(long address, int port)
		{
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.m_Port = port;
			this.m_Address = new IPAddress(address);
		}

		public IPEndPoint(IPAddress address, int port)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.m_Port = port;
			this.m_Address = address;
		}

		public IPAddress Address
		{
			get
			{
				return this.m_Address;
			}
			set
			{
				this.m_Address = value;
			}
		}

		public int Port
		{
			get
			{
				return this.m_Port;
			}
			set
			{
				if (!ValidationHelper.ValidateTcpPort(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.m_Port = value;
			}
		}

		public override string ToString()
		{
			string text;
			if (this.m_Address.AddressFamily == AddressFamily.InterNetworkV6)
			{
				text = "[{0}]:{1}";
			}
			else
			{
				text = "{0}:{1}";
			}
			return string.Format(text, this.m_Address.ToString(), this.Port.ToString(NumberFormatInfo.InvariantInfo));
		}

		public override SocketAddress Serialize()
		{
			return new SocketAddress(this.Address, this.Port);
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			if (socketAddress.Family != this.AddressFamily)
			{
				throw new ArgumentException(global::SR.GetString("The AddressFamily {0} is not valid for the {1} end point, use {2} instead.", new object[]
				{
					socketAddress.Family.ToString(),
					base.GetType().FullName,
					this.AddressFamily.ToString()
				}), "socketAddress");
			}
			if (socketAddress.Size < 8)
			{
				throw new ArgumentException(global::SR.GetString("The supplied {0} is an invalid size for the {1} end point.", new object[]
				{
					socketAddress.GetType().FullName,
					base.GetType().FullName
				}), "socketAddress");
			}
			return socketAddress.GetIPEndPoint();
		}

		public override bool Equals(object comparand)
		{
			return comparand is IPEndPoint && ((IPEndPoint)comparand).m_Address.Equals(this.m_Address) && ((IPEndPoint)comparand).m_Port == this.m_Port;
		}

		public override int GetHashCode()
		{
			return this.m_Address.GetHashCode() ^ this.m_Port;
		}

		internal IPEndPoint Snapshot()
		{
			return new IPEndPoint(this.Address.Snapshot(), this.Port);
		}

		public const int MinPort = 0;

		public const int MaxPort = 65535;

		private IPAddress m_Address;

		private int m_Port;

		internal const int AnyPort = 0;

		internal static IPEndPoint Any = new IPEndPoint(IPAddress.Any, 0);

		internal static IPEndPoint IPv6Any = new IPEndPoint(IPAddress.IPv6Any, 0);
	}
}
