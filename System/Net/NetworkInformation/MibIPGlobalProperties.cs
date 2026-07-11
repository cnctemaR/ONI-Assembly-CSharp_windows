using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net.NetworkInformation
{
	internal class MibIPGlobalProperties : IPGlobalProperties
	{
		public MibIPGlobalProperties(string procDir)
		{
			this.StatisticsFile = Path.Combine(procDir, "net/snmp");
			this.StatisticsFileIPv6 = Path.Combine(procDir, "net/snmp6");
			this.TcpFile = Path.Combine(procDir, "net/tcp");
			this.Tcp6File = Path.Combine(procDir, "net/tcp6");
			this.UdpFile = Path.Combine(procDir, "net/udp");
			this.Udp6File = Path.Combine(procDir, "net/udp6");
		}

		[DllImport("libc")]
		private static extern int gethostname([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 1)] byte[] name, int len);

		[DllImport("libc")]
		private static extern int getdomainname([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 1)] byte[] name, int len);

		private global::System.Collections.Specialized.StringDictionary GetProperties4(string item)
		{
			string statisticsFile = this.StatisticsFile;
			string text = item + ": ";
			global::System.Collections.Specialized.StringDictionary stringDictionary2;
			using (StreamReader streamReader = new StreamReader(statisticsFile, Encoding.ASCII))
			{
				string[] array = null;
				string[] array2 = null;
				string text2 = string.Empty;
				for (;;)
				{
					text2 = streamReader.ReadLine();
					if (!string.IsNullOrEmpty(text2))
					{
						if (text2.Length > text.Length && string.CompareOrdinal(text2, 0, text, 0, text.Length) == 0)
						{
							if (array != null)
							{
								break;
							}
							array = text2.Substring(text.Length).Split(new char[] { ' ' });
						}
					}
					if (streamReader.EndOfStream)
					{
						goto IL_00E2;
					}
				}
				if (array2 != null)
				{
					throw this.CreateException(statisticsFile, string.Format("Found duplicate line for values for the same item '{0}'", item));
				}
				array2 = text2.Substring(text.Length).Split(new char[] { ' ' });
				IL_00E2:
				if (array2 == null)
				{
					throw this.CreateException(statisticsFile, string.Format("No corresponding line was not found for '{0}'", item));
				}
				if (array.Length != array2.Length)
				{
					throw this.CreateException(statisticsFile, string.Format("The counts in the header line and the value line do not match for '{0}'", item));
				}
				global::System.Collections.Specialized.StringDictionary stringDictionary = new global::System.Collections.Specialized.StringDictionary();
				for (int i = 0; i < array.Length; i++)
				{
					stringDictionary[array[i]] = array2[i];
				}
				stringDictionary2 = stringDictionary;
			}
			return stringDictionary2;
		}

		private global::System.Collections.Specialized.StringDictionary GetProperties6(string item)
		{
			if (!File.Exists(this.StatisticsFileIPv6))
			{
				throw new NetworkInformationException();
			}
			string statisticsFileIPv = this.StatisticsFileIPv6;
			global::System.Collections.Specialized.StringDictionary stringDictionary2;
			using (StreamReader streamReader = new StreamReader(statisticsFileIPv, Encoding.ASCII))
			{
				global::System.Collections.Specialized.StringDictionary stringDictionary = new global::System.Collections.Specialized.StringDictionary();
				string text = string.Empty;
				for (;;)
				{
					text = streamReader.ReadLine();
					if (!string.IsNullOrEmpty(text))
					{
						if (text.Length > item.Length && string.CompareOrdinal(text, 0, item, 0, item.Length) == 0)
						{
							int num = text.IndexOfAny(MibIPGlobalProperties.wsChars, item.Length);
							if (num < 0)
							{
								break;
							}
							stringDictionary[text.Substring(item.Length, num - item.Length)] = text.Substring(num + 1).Trim(MibIPGlobalProperties.wsChars);
						}
					}
					if (streamReader.EndOfStream)
					{
						goto Block_7;
					}
				}
				throw this.CreateException(statisticsFileIPv, null);
				Block_7:
				stringDictionary2 = stringDictionary;
			}
			return stringDictionary2;
		}

		private Exception CreateException(string file, string msg)
		{
			return new InvalidOperationException(string.Format("Unsupported (unexpected) '{0}' file format. ", file) + msg);
		}

		private IPEndPoint[] GetLocalAddresses(List<string[]> list)
		{
			IPEndPoint[] array = new IPEndPoint[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.ToEndpoint(list[i][1]);
			}
			return array;
		}

		private IPEndPoint ToEndpoint(string s)
		{
			int num = s.IndexOf(':');
			int num2 = int.Parse(s.Substring(num + 1), NumberStyles.HexNumber);
			if (s.Length == 13)
			{
				return new IPEndPoint(long.Parse(s.Substring(0, num), NumberStyles.HexNumber), num2);
			}
			byte[] array = new byte[16];
			int num3 = 0;
			while (num3 << 1 < num)
			{
				array[num3] = byte.Parse(s.Substring(num3 << 1, 2), NumberStyles.HexNumber);
				num3++;
			}
			return new IPEndPoint(new IPAddress(array), num2);
		}

		private void GetRows(string file, List<string[]> list)
		{
			if (!File.Exists(file))
			{
				return;
			}
			using (StreamReader streamReader = new StreamReader(file, Encoding.ASCII))
			{
				streamReader.ReadLine();
				while (!streamReader.EndOfStream)
				{
					string[] array = streamReader.ReadLine().Split(MibIPGlobalProperties.wsChars, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length < 4)
					{
						throw this.CreateException(file, null);
					}
					list.Add(array);
				}
			}
		}

		public override TcpConnectionInformation[] GetActiveTcpConnections()
		{
			List<string[]> list = new List<string[]>();
			this.GetRows(this.TcpFile, list);
			this.GetRows(this.Tcp6File, list);
			TcpConnectionInformation[] array = new TcpConnectionInformation[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				IPEndPoint ipendPoint = this.ToEndpoint(list[i][1]);
				IPEndPoint ipendPoint2 = this.ToEndpoint(list[i][2]);
				TcpState tcpState = (TcpState)int.Parse(list[i][3], NumberStyles.HexNumber);
				array[i] = new TcpConnectionInformationImpl(ipendPoint, ipendPoint2, tcpState);
			}
			return array;
		}

		public override IPEndPoint[] GetActiveTcpListeners()
		{
			List<string[]> list = new List<string[]>();
			this.GetRows(this.TcpFile, list);
			this.GetRows(this.Tcp6File, list);
			return this.GetLocalAddresses(list);
		}

		public override IPEndPoint[] GetActiveUdpListeners()
		{
			List<string[]> list = new List<string[]>();
			this.GetRows(this.UdpFile, list);
			this.GetRows(this.Udp6File, list);
			return this.GetLocalAddresses(list);
		}

		public override IcmpV4Statistics GetIcmpV4Statistics()
		{
			return new MibIcmpV4Statistics(this.GetProperties4("Icmp"));
		}

		public override IcmpV6Statistics GetIcmpV6Statistics()
		{
			return new MibIcmpV6Statistics(this.GetProperties6("Icmp6"));
		}

		public override IPGlobalStatistics GetIPv4GlobalStatistics()
		{
			return new MibIPGlobalStatistics(this.GetProperties4("Ip"));
		}

		public override IPGlobalStatistics GetIPv6GlobalStatistics()
		{
			return new MibIPGlobalStatistics(this.GetProperties6("Ip6"));
		}

		public override TcpStatistics GetTcpIPv4Statistics()
		{
			return new MibTcpStatistics(this.GetProperties4("Tcp"));
		}

		public override TcpStatistics GetTcpIPv6Statistics()
		{
			return new MibTcpStatistics(this.GetProperties4("Tcp"));
		}

		public override UdpStatistics GetUdpIPv4Statistics()
		{
			return new MibUdpStatistics(this.GetProperties4("Udp"));
		}

		public override UdpStatistics GetUdpIPv6Statistics()
		{
			return new MibUdpStatistics(this.GetProperties6("Udp6"));
		}

		public override string DhcpScopeName
		{
			get
			{
				return string.Empty;
			}
		}

		public override string DomainName
		{
			get
			{
				byte[] array = new byte[256];
				if (MibIPGlobalProperties.getdomainname(array, 256) != 0)
				{
					throw new NetworkInformationException();
				}
				int num = Array.IndexOf<byte>(array, 0);
				return Encoding.ASCII.GetString(array, 0, (num >= 0) ? num : 256);
			}
		}

		public override string HostName
		{
			get
			{
				byte[] array = new byte[256];
				if (MibIPGlobalProperties.gethostname(array, 256) != 0)
				{
					throw new NetworkInformationException();
				}
				int num = Array.IndexOf<byte>(array, 0);
				return Encoding.ASCII.GetString(array, 0, (num >= 0) ? num : 256);
			}
		}

		public override bool IsWinsProxy
		{
			get
			{
				return false;
			}
		}

		public override NetBiosNodeType NodeType
		{
			get
			{
				return NetBiosNodeType.Unknown;
			}
		}

		public const string ProcDir = "/proc";

		public const string CompatProcDir = "/usr/compat/linux/proc";

		public readonly string StatisticsFile;

		public readonly string StatisticsFileIPv6;

		public readonly string TcpFile;

		public readonly string Tcp6File;

		public readonly string UdpFile;

		public readonly string Udp6File;

		private static readonly char[] wsChars = new char[] { ' ', '\t' };
	}
}
