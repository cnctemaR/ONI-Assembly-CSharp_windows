using System;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace System.Net
{
	[Serializable]
	public class IPAddress
	{
		public IPAddress(long newAddress)
		{
			if (newAddress < 0L || newAddress > (long)((ulong)(-1)))
			{
				throw new ArgumentOutOfRangeException("newAddress");
			}
			this.m_Address = newAddress;
		}

		public IPAddress(byte[] address, long scopeid)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Length != 16)
			{
				throw new ArgumentException(global::SR.GetString("An invalid IP address was specified."), "address");
			}
			this.m_Family = AddressFamily.InterNetworkV6;
			for (int i = 0; i < 8; i++)
			{
				this.m_Numbers[i] = (ushort)((int)address[i * 2] * 256 + (int)address[i * 2 + 1]);
			}
			if (scopeid < 0L || scopeid > (long)((ulong)(-1)))
			{
				throw new ArgumentOutOfRangeException("scopeid");
			}
			this.m_ScopeId = scopeid;
		}

		private IPAddress(ushort[] address, uint scopeid)
		{
			this.m_Family = AddressFamily.InterNetworkV6;
			this.m_Numbers = address;
			this.m_ScopeId = (long)((ulong)scopeid);
		}

		public IPAddress(byte[] address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Length != 4 && address.Length != 16)
			{
				throw new ArgumentException(global::SR.GetString("An invalid IP address was specified."), "address");
			}
			if (address.Length == 4)
			{
				this.m_Family = AddressFamily.InterNetwork;
				this.m_Address = (long)(((int)address[3] << 24) | ((int)address[2] << 16) | ((int)address[1] << 8) | (int)address[0]) & (long)((ulong)(-1));
				return;
			}
			this.m_Family = AddressFamily.InterNetworkV6;
			for (int i = 0; i < 8; i++)
			{
				this.m_Numbers[i] = (ushort)((int)address[i * 2] * 256 + (int)address[i * 2 + 1]);
			}
		}

		internal IPAddress(int newAddress)
		{
			this.m_Address = (long)newAddress & (long)((ulong)(-1));
		}

		public static bool TryParse(string ipString, out IPAddress address)
		{
			address = IPAddress.InternalParse(ipString, true);
			return address != null;
		}

		public static IPAddress Parse(string ipString)
		{
			return IPAddress.InternalParse(ipString, false);
		}

		private unsafe static IPAddress InternalParse(string ipString, bool tryParse)
		{
			if (ipString == null)
			{
				if (tryParse)
				{
					return null;
				}
				throw new ArgumentNullException("ipString");
			}
			else if (ipString.IndexOf(':') != -1)
			{
				int num = 0;
				if (ipString[0] != '[')
				{
					ipString += "]";
				}
				else
				{
					num = 1;
				}
				int length = ipString.Length;
				fixed (string text = ipString)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					if (IPv6AddressHelper.IsValidStrict(ptr, num, ref length) || length != ipString.Length)
					{
						ushort[] array = new ushort[8];
						string text2 = null;
						ushort[] array2;
						ushort* ptr2;
						if ((array2 = array) == null || array2.Length == 0)
						{
							ptr2 = null;
						}
						else
						{
							ptr2 = &array2[0];
						}
						IPv6AddressHelper.Parse(ipString, ptr2, 0, ref text2);
						array2 = null;
						if (text2 == null || text2.Length == 0)
						{
							return new IPAddress(array, 0U);
						}
						text2 = text2.Substring(1);
						uint num2;
						if (uint.TryParse(text2, NumberStyles.None, null, out num2))
						{
							return new IPAddress(array, num2);
						}
						return new IPAddress(array, 0U);
					}
					else
					{
						text = null;
						if (tryParse)
						{
							return null;
						}
						SocketException ex = new SocketException(SocketError.InvalidArgument);
						throw new FormatException(global::SR.GetString("An invalid IP address was specified."), ex);
					}
				}
			}
			else
			{
				int length2 = ipString.Length;
				long num3;
				fixed (string text = ipString)
				{
					char* ptr3 = text;
					if (ptr3 != null)
					{
						ptr3 += RuntimeHelpers.OffsetToStringData / 2;
					}
					num3 = IPv4AddressHelper.ParseNonCanonical(ptr3, 0, ref length2, true);
				}
				if (num3 != -1L && length2 == ipString.Length)
				{
					num3 = ((num3 & 255L) << 24) | (((num3 & 65280L) << 8) | (((num3 & 16711680L) >> 8) | ((num3 & (long)((ulong)(-16777216))) >> 24)));
					return new IPAddress(num3);
				}
				if (tryParse)
				{
					return null;
				}
				throw new FormatException(global::SR.GetString("An invalid IP address was specified."));
			}
		}

		[Obsolete("This property has been deprecated. It is address family dependent. Please use IPAddress.Equals method to perform comparisons. http://go.microsoft.com/fwlink/?linkid=14202")]
		public long Address
		{
			get
			{
				if (this.m_Family == AddressFamily.InterNetworkV6)
				{
					throw new SocketException(SocketError.OperationNotSupported);
				}
				return this.m_Address;
			}
			set
			{
				if (this.m_Family == AddressFamily.InterNetworkV6)
				{
					throw new SocketException(SocketError.OperationNotSupported);
				}
				if (this.m_Address != value)
				{
					this.m_ToString = null;
					this.m_Address = value;
				}
			}
		}

		public byte[] GetAddressBytes()
		{
			byte[] array;
			if (this.m_Family == AddressFamily.InterNetworkV6)
			{
				array = new byte[16];
				int num = 0;
				for (int i = 0; i < 8; i++)
				{
					array[num++] = (byte)((this.m_Numbers[i] >> 8) & 255);
					array[num++] = (byte)(this.m_Numbers[i] & 255);
				}
			}
			else
			{
				array = new byte[]
				{
					(byte)this.m_Address,
					(byte)(this.m_Address >> 8),
					(byte)(this.m_Address >> 16),
					(byte)(this.m_Address >> 24)
				};
			}
			return array;
		}

		public AddressFamily AddressFamily
		{
			get
			{
				return this.m_Family;
			}
		}

		public long ScopeId
		{
			get
			{
				if (this.m_Family == AddressFamily.InterNetwork)
				{
					throw new SocketException(SocketError.OperationNotSupported);
				}
				return this.m_ScopeId;
			}
			set
			{
				if (this.m_Family == AddressFamily.InterNetwork)
				{
					throw new SocketException(SocketError.OperationNotSupported);
				}
				if (value < 0L || value > (long)((ulong)(-1)))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.m_ScopeId != value)
				{
					this.m_Address = value;
					this.m_ScopeId = value;
				}
			}
		}

		public unsafe override string ToString()
		{
			if (this.m_ToString == null)
			{
				if (this.m_Family == AddressFamily.InterNetworkV6)
				{
					IPv6AddressFormatter pv6AddressFormatter = new IPv6AddressFormatter(this.m_Numbers, this.ScopeId);
					this.m_ToString = pv6AddressFormatter.ToString();
				}
				else
				{
					int num = 15;
					char* ptr = stackalloc char[(UIntPtr)30];
					int num2 = (int)((this.m_Address >> 24) & 255L);
					do
					{
						ptr[(IntPtr)(--num) * 2] = (char)(48 + num2 % 10);
						num2 /= 10;
					}
					while (num2 > 0);
					ptr[(IntPtr)(--num) * 2] = '.';
					num2 = (int)((this.m_Address >> 16) & 255L);
					do
					{
						ptr[(IntPtr)(--num) * 2] = (char)(48 + num2 % 10);
						num2 /= 10;
					}
					while (num2 > 0);
					ptr[(IntPtr)(--num) * 2] = '.';
					num2 = (int)((this.m_Address >> 8) & 255L);
					do
					{
						ptr[(IntPtr)(--num) * 2] = (char)(48 + num2 % 10);
						num2 /= 10;
					}
					while (num2 > 0);
					ptr[(IntPtr)(--num) * 2] = '.';
					num2 = (int)(this.m_Address & 255L);
					do
					{
						ptr[(IntPtr)(--num) * 2] = (char)(48 + num2 % 10);
						num2 /= 10;
					}
					while (num2 > 0);
					this.m_ToString = new string(ptr, num, 15 - num);
				}
			}
			return this.m_ToString;
		}

		public static long HostToNetworkOrder(long host)
		{
			return (((long)IPAddress.HostToNetworkOrder((int)host) & (long)((ulong)(-1))) << 32) | ((long)IPAddress.HostToNetworkOrder((int)(host >> 32)) & (long)((ulong)(-1)));
		}

		public static int HostToNetworkOrder(int host)
		{
			return (((int)IPAddress.HostToNetworkOrder((short)host) & 65535) << 16) | ((int)IPAddress.HostToNetworkOrder((short)(host >> 16)) & 65535);
		}

		public static short HostToNetworkOrder(short host)
		{
			return (short)(((int)(host & 255) << 8) | ((host >> 8) & 255));
		}

		public static long NetworkToHostOrder(long network)
		{
			return IPAddress.HostToNetworkOrder(network);
		}

		public static int NetworkToHostOrder(int network)
		{
			return IPAddress.HostToNetworkOrder(network);
		}

		public static short NetworkToHostOrder(short network)
		{
			return IPAddress.HostToNetworkOrder(network);
		}

		public static bool IsLoopback(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.m_Family == AddressFamily.InterNetworkV6)
			{
				return address.Equals(IPAddress.IPv6Loopback);
			}
			return (address.m_Address & 255L) == (IPAddress.Loopback.m_Address & 255L);
		}

		internal bool IsBroadcast
		{
			get
			{
				return this.m_Family != AddressFamily.InterNetworkV6 && this.m_Address == IPAddress.Broadcast.m_Address;
			}
		}

		public bool IsIPv6Multicast
		{
			get
			{
				return this.m_Family == AddressFamily.InterNetworkV6 && (this.m_Numbers[0] & 65280) == 65280;
			}
		}

		public bool IsIPv6LinkLocal
		{
			get
			{
				return this.m_Family == AddressFamily.InterNetworkV6 && (this.m_Numbers[0] & 65472) == 65152;
			}
		}

		public bool IsIPv6SiteLocal
		{
			get
			{
				return this.m_Family == AddressFamily.InterNetworkV6 && (this.m_Numbers[0] & 65472) == 65216;
			}
		}

		public bool IsIPv6Teredo
		{
			get
			{
				return this.m_Family == AddressFamily.InterNetworkV6 && this.m_Numbers[0] == 8193 && this.m_Numbers[1] == 0;
			}
		}

		public bool IsIPv4MappedToIPv6
		{
			get
			{
				if (this.AddressFamily != AddressFamily.InterNetworkV6)
				{
					return false;
				}
				for (int i = 0; i < 5; i++)
				{
					if (this.m_Numbers[i] != 0)
					{
						return false;
					}
				}
				return this.m_Numbers[5] == ushort.MaxValue;
			}
		}

		internal bool Equals(object comparandObj, bool compareScopeId)
		{
			IPAddress ipaddress = comparandObj as IPAddress;
			if (ipaddress == null)
			{
				return false;
			}
			if (this.m_Family != ipaddress.m_Family)
			{
				return false;
			}
			if (this.m_Family == AddressFamily.InterNetworkV6)
			{
				for (int i = 0; i < 8; i++)
				{
					if (ipaddress.m_Numbers[i] != this.m_Numbers[i])
					{
						return false;
					}
				}
				return ipaddress.m_ScopeId == this.m_ScopeId || !compareScopeId;
			}
			return ipaddress.m_Address == this.m_Address;
		}

		public override bool Equals(object comparand)
		{
			return this.Equals(comparand, true);
		}

		public override int GetHashCode()
		{
			if (this.m_Family == AddressFamily.InterNetworkV6)
			{
				if (this.m_HashCode == 0)
				{
					this.m_HashCode = StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.ToString());
				}
				return this.m_HashCode;
			}
			return (int)this.m_Address;
		}

		internal IPAddress Snapshot()
		{
			AddressFamily family = this.m_Family;
			if (family == AddressFamily.InterNetwork)
			{
				return new IPAddress(this.m_Address);
			}
			if (family != AddressFamily.InterNetworkV6)
			{
				throw new InternalException();
			}
			return new IPAddress(this.m_Numbers, (uint)this.m_ScopeId);
		}

		public IPAddress MapToIPv6()
		{
			if (this.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return this;
			}
			return new IPAddress(new ushort[]
			{
				0,
				0,
				0,
				0,
				0,
				ushort.MaxValue,
				(ushort)(((this.m_Address & 65280L) >> 8) | ((this.m_Address & 255L) << 8)),
				(ushort)(((this.m_Address & (long)((ulong)(-16777216))) >> 24) | ((this.m_Address & 16711680L) >> 8))
			}, 0U);
		}

		public IPAddress MapToIPv4()
		{
			if (this.AddressFamily == AddressFamily.InterNetwork)
			{
				return this;
			}
			return new IPAddress((long)((ulong)(((uint)(this.m_Numbers[6] & 65280) >> 8) | (uint)((uint)(this.m_Numbers[6] & 255) << 8) | ((((uint)(this.m_Numbers[7] & 65280) >> 8) | (uint)((uint)(this.m_Numbers[7] & 255) << 8)) << 16))));
		}

		public static readonly IPAddress Any = new IPAddress(0);

		public static readonly IPAddress Loopback = new IPAddress(16777343);

		public static readonly IPAddress Broadcast = new IPAddress((long)((ulong)(-1)));

		public static readonly IPAddress None = IPAddress.Broadcast;

		internal const long LoopbackMask = 255L;

		internal long m_Address;

		[NonSerialized]
		internal string m_ToString;

		public static readonly IPAddress IPv6Any = new IPAddress(new byte[16], 0L);

		public static readonly IPAddress IPv6Loopback = new IPAddress(new byte[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 1
		}, 0L);

		public static readonly IPAddress IPv6None = new IPAddress(new byte[16], 0L);

		private AddressFamily m_Family = AddressFamily.InterNetwork;

		private ushort[] m_Numbers = new ushort[8];

		private long m_ScopeId;

		private int m_HashCode;

		internal const int IPv4AddressBytes = 4;

		internal const int IPv6AddressBytes = 16;

		internal const int NumberOfLabels = 8;
	}
}
