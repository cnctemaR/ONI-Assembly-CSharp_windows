using System;
using System.Globalization;
using System.Net.Sockets;

namespace System.Net
{
	[Serializable]
	public class IPAddress
	{
		public IPAddress(long addr)
		{
			this.m_Address = addr;
			this.m_Family = global::System.Net.Sockets.AddressFamily.InterNetwork;
		}

		public IPAddress(byte[] address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			int num = address.Length;
			if (num != 16 && num != 4)
			{
				throw new ArgumentException("An invalid IP address was specified.", "address");
			}
			if (num == 16)
			{
				this.m_Numbers = new ushort[8];
				Buffer.BlockCopy(address, 0, this.m_Numbers, 0, 16);
				this.m_Family = global::System.Net.Sockets.AddressFamily.InterNetworkV6;
				this.m_ScopeId = 0L;
			}
			else
			{
				this.m_Address = (long)((ulong)((ulong)address[3] << 24) + (ulong)((long)((long)address[2] << 16)) + (ulong)((long)((long)address[1] << 8)) + (ulong)((long)address[0]));
				this.m_Family = global::System.Net.Sockets.AddressFamily.InterNetwork;
			}
		}

		public IPAddress(byte[] address, long scopeId)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Length != 16)
			{
				throw new ArgumentException("An invalid IP address was specified.", "address");
			}
			this.m_Numbers = new ushort[8];
			Buffer.BlockCopy(address, 0, this.m_Numbers, 0, 16);
			this.m_Family = global::System.Net.Sockets.AddressFamily.InterNetworkV6;
			this.m_ScopeId = scopeId;
		}

		internal IPAddress(ushort[] address, long scopeId)
		{
			this.m_Numbers = address;
			for (int i = 0; i < 8; i++)
			{
				this.m_Numbers[i] = (ushort)IPAddress.HostToNetworkOrder((short)this.m_Numbers[i]);
			}
			this.m_Family = global::System.Net.Sockets.AddressFamily.InterNetworkV6;
			this.m_ScopeId = scopeId;
		}

		private static short SwapShort(short number)
		{
			return (short)(((number >> 8) & 255) | (((int)number << 8) & 65280));
		}

		private static int SwapInt(int number)
		{
			return ((number >> 24) & 255) | ((number >> 8) & 65280) | ((number << 8) & 16711680) | (number << 24);
		}

		private static long SwapLong(long number)
		{
			return ((number >> 56) & 255L) | ((number >> 40) & 65280L) | ((number >> 24) & 16711680L) | ((number >> 8) & (long)((ulong)(-16777216))) | ((number << 8) & 1095216660480L) | ((number << 24) & 280375465082880L) | ((number << 40) & 71776119061217280L) | (number << 56);
		}

		public static short HostToNetworkOrder(short host)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return host;
			}
			return IPAddress.SwapShort(host);
		}

		public static int HostToNetworkOrder(int host)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return host;
			}
			return IPAddress.SwapInt(host);
		}

		public static long HostToNetworkOrder(long host)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return host;
			}
			return IPAddress.SwapLong(host);
		}

		public static short NetworkToHostOrder(short network)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return network;
			}
			return IPAddress.SwapShort(network);
		}

		public static int NetworkToHostOrder(int network)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return network;
			}
			return IPAddress.SwapInt(network);
		}

		public static long NetworkToHostOrder(long network)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return network;
			}
			return IPAddress.SwapLong(network);
		}

		public static IPAddress Parse(string ipString)
		{
			IPAddress ipaddress;
			if (IPAddress.TryParse(ipString, out ipaddress))
			{
				return ipaddress;
			}
			throw new FormatException("An invalid IP address was specified.");
		}

		public static bool TryParse(string ipString, out IPAddress address)
		{
			if (ipString == null)
			{
				throw new ArgumentNullException("ipString");
			}
			IPAddress ipaddress;
			address = (ipaddress = IPAddress.ParseIPV4(ipString));
			if (ipaddress == null)
			{
				address = (ipaddress = IPAddress.ParseIPV6(ipString));
				if (ipaddress == null)
				{
					return false;
				}
			}
			return true;
		}

		private static IPAddress ParseIPV4(string ip)
		{
			int num = ip.IndexOf(' ');
			if (num != -1)
			{
				string[] array = ip.Substring(num + 1).Split(new char[] { '.' });
				if (array.Length > 0)
				{
					string text = array[array.Length - 1];
					if (text.Length == 0)
					{
						return null;
					}
					foreach (char c in text)
					{
						if (!global::System.Uri.IsHexDigit(c))
						{
							return null;
						}
					}
				}
				ip = ip.Substring(0, num);
			}
			if (ip.Length == 0 || ip[ip.Length - 1] == '.')
			{
				return null;
			}
			string[] array2 = ip.Split(new char[] { '.' });
			if (array2.Length > 4)
			{
				return null;
			}
			IPAddress ipaddress;
			try
			{
				long num2 = 0L;
				long num3 = 0L;
				for (int j = 0; j < array2.Length; j++)
				{
					string text3 = array2[j];
					if (3 <= text3.Length && text3.Length <= 4 && text3[0] == '0' && (text3[1] == 'x' || text3[1] == 'X'))
					{
						if (text3.Length == 3)
						{
							num3 = (long)((byte)global::System.Uri.FromHex(text3[2]));
						}
						else
						{
							num3 = (long)((byte)((global::System.Uri.FromHex(text3[2]) << 4) | global::System.Uri.FromHex(text3[3])));
						}
					}
					else
					{
						if (text3.Length == 0)
						{
							return null;
						}
						if (text3[0] == '0')
						{
							num3 = 0L;
							for (int k = 1; k < text3.Length; k++)
							{
								if ('0' > text3[k] || text3[k] > '7')
								{
									return null;
								}
								num3 = (num3 << 3) + (long)text3[k] - 48L;
							}
						}
						else if (!long.TryParse(text3, NumberStyles.None, null, out num3))
						{
							return null;
						}
					}
					if (j == array2.Length - 1)
					{
						j = 3;
					}
					else if (num3 > 255L)
					{
						return null;
					}
					int num4 = 0;
					while (num3 > 0L)
					{
						num2 |= (num3 & 255L) << (j - num4 << 3);
						num4++;
						num3 /= 256L;
					}
				}
				ipaddress = new IPAddress(num2);
			}
			catch (Exception)
			{
				ipaddress = null;
			}
			return ipaddress;
		}

		private static IPAddress ParseIPV6(string ip)
		{
			IPv6Address pv6Address;
			if (IPv6Address.TryParse(ip, out pv6Address))
			{
				return new IPAddress(pv6Address.Address, pv6Address.ScopeId);
			}
			return null;
		}

		[Obsolete("This property is obsolete. Use GetAddressBytes.")]
		public long Address
		{
			get
			{
				if (this.m_Family != global::System.Net.Sockets.AddressFamily.InterNetwork)
				{
					throw new Exception("The attempted operation is not supported for the type of object referenced");
				}
				return this.m_Address;
			}
			set
			{
				if (this.m_Family != global::System.Net.Sockets.AddressFamily.InterNetwork)
				{
					throw new Exception("The attempted operation is not supported for the type of object referenced");
				}
				this.m_Address = value;
			}
		}

		internal long InternalIPv4Address
		{
			get
			{
				return this.m_Address;
			}
		}

		public bool IsIPv6LinkLocal
		{
			get
			{
				if (this.m_Family == global::System.Net.Sockets.AddressFamily.InterNetwork)
				{
					return false;
				}
				int num = (int)IPAddress.NetworkToHostOrder((short)this.m_Numbers[0]) & 65520;
				return 65152 <= num && num < 65216;
			}
		}

		public bool IsIPv6SiteLocal
		{
			get
			{
				if (this.m_Family == global::System.Net.Sockets.AddressFamily.InterNetwork)
				{
					return false;
				}
				int num = (int)IPAddress.NetworkToHostOrder((short)this.m_Numbers[0]) & 65520;
				return 65216 <= num && num < 65280;
			}
		}

		public bool IsIPv6Multicast
		{
			get
			{
				return this.m_Family != global::System.Net.Sockets.AddressFamily.InterNetwork && ((ushort)IPAddress.NetworkToHostOrder((short)this.m_Numbers[0]) & 65280) == 65280;
			}
		}

		public long ScopeId
		{
			get
			{
				if (this.m_Family != global::System.Net.Sockets.AddressFamily.InterNetworkV6)
				{
					throw new Exception("The attempted operation is not supported for the type of object referenced");
				}
				return this.m_ScopeId;
			}
			set
			{
				if (this.m_Family != global::System.Net.Sockets.AddressFamily.InterNetworkV6)
				{
					throw new Exception("The attempted operation is not supported for the type of object referenced");
				}
				this.m_ScopeId = value;
			}
		}

		public byte[] GetAddressBytes()
		{
			if (this.m_Family == global::System.Net.Sockets.AddressFamily.InterNetworkV6)
			{
				byte[] array = new byte[16];
				Buffer.BlockCopy(this.m_Numbers, 0, array, 0, 16);
				return array;
			}
			return new byte[]
			{
				(byte)(this.m_Address & 255L),
				(byte)((this.m_Address >> 8) & 255L),
				(byte)((this.m_Address >> 16) & 255L),
				(byte)(this.m_Address >> 24)
			};
		}

		public global::System.Net.Sockets.AddressFamily AddressFamily
		{
			get
			{
				return this.m_Family;
			}
		}

		public static bool IsLoopback(IPAddress addr)
		{
			if (addr.m_Family == global::System.Net.Sockets.AddressFamily.InterNetwork)
			{
				return (addr.m_Address & 255L) == 127L;
			}
			for (int i = 0; i < 6; i++)
			{
				if (addr.m_Numbers[i] != 0)
				{
					return false;
				}
			}
			return IPAddress.NetworkToHostOrder((short)addr.m_Numbers[7]) == 1;
		}

		public override string ToString()
		{
			if (this.m_Family == global::System.Net.Sockets.AddressFamily.InterNetwork)
			{
				return IPAddress.ToString(this.m_Address);
			}
			ushort[] array = this.m_Numbers.Clone() as ushort[];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (ushort)IPAddress.NetworkToHostOrder((short)array[i]);
			}
			return new IPv6Address(array)
			{
				ScopeId = this.ScopeId
			}.ToString();
		}

		private static string ToString(long addr)
		{
			return string.Concat(new string[]
			{
				(addr & 255L).ToString(),
				".",
				((addr >> 8) & 255L).ToString(),
				".",
				((addr >> 16) & 255L).ToString(),
				".",
				((addr >> 24) & 255L).ToString()
			});
		}

		public override bool Equals(object other)
		{
			IPAddress ipaddress = other as IPAddress;
			if (ipaddress == null)
			{
				return false;
			}
			if (this.AddressFamily != ipaddress.AddressFamily)
			{
				return false;
			}
			if (this.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetwork)
			{
				return this.m_Address == ipaddress.m_Address;
			}
			ushort[] numbers = ipaddress.m_Numbers;
			for (int i = 0; i < 8; i++)
			{
				if (this.m_Numbers[i] != numbers[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (this.m_Family == global::System.Net.Sockets.AddressFamily.InterNetwork)
			{
				return (int)this.m_Address;
			}
			return IPAddress.Hash(((int)this.m_Numbers[0] << 16) + (int)this.m_Numbers[1], ((int)this.m_Numbers[2] << 16) + (int)this.m_Numbers[3], ((int)this.m_Numbers[4] << 16) + (int)this.m_Numbers[5], ((int)this.m_Numbers[6] << 16) + (int)this.m_Numbers[7]);
		}

		private static int Hash(int i, int j, int k, int l)
		{
			return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25));
		}

		private long m_Address;

		private global::System.Net.Sockets.AddressFamily m_Family;

		private ushort[] m_Numbers;

		private long m_ScopeId;

		public static readonly IPAddress Any = new IPAddress(0L);

		public static readonly IPAddress Broadcast = IPAddress.Parse("255.255.255.255");

		public static readonly IPAddress Loopback = IPAddress.Parse("127.0.0.1");

		public static readonly IPAddress None = IPAddress.Parse("255.255.255.255");

		public static readonly IPAddress IPv6Any = IPAddress.ParseIPV6("::");

		public static readonly IPAddress IPv6Loopback = IPAddress.ParseIPV6("::1");

		public static readonly IPAddress IPv6None = IPAddress.ParseIPV6("::");

		private int m_HashCode;
	}
}
