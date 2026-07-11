using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net.NetworkInformation
{
	internal abstract class CommonUnixIPGlobalProperties : IPGlobalProperties
	{
		[DllImport("libc")]
		private static extern int gethostname([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] name, int len);

		[DllImport("libc")]
		private static extern int getdomainname([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] name, int len);

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
				try
				{
					if (CommonUnixIPGlobalProperties.getdomainname(array, 256) != 0)
					{
						throw new NetworkInformationException();
					}
				}
				catch (EntryPointNotFoundException)
				{
					return string.Empty;
				}
				int num = Array.IndexOf<byte>(array, 0);
				return Encoding.ASCII.GetString(array, 0, (num < 0) ? 256 : num);
			}
		}

		public override string HostName
		{
			get
			{
				byte[] array = new byte[256];
				if (CommonUnixIPGlobalProperties.gethostname(array, 256) != 0)
				{
					throw new NetworkInformationException();
				}
				int num = Array.IndexOf<byte>(array, 0);
				return Encoding.ASCII.GetString(array, 0, (num < 0) ? 256 : num);
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
	}
}
