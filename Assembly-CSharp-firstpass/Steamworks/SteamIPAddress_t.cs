using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SteamIPAddress_t
	{
		public SteamIPAddress_t(IPAddress iPAddress)
		{
			byte[] addressBytes = iPAddress.GetAddressBytes();
			AddressFamily addressFamily = iPAddress.AddressFamily;
			if (addressFamily != AddressFamily.InterNetwork)
			{
				if (addressFamily != AddressFamily.InterNetworkV6)
				{
					throw new TypeInitializationException("SteamIPAddress_t: Unexpected address family " + iPAddress.AddressFamily.ToString(), null);
				}
				if (addressBytes.Length != 16)
				{
					throw new TypeInitializationException("SteamIPAddress_t: Unexpected byte length for Ipv6: " + addressBytes.Length.ToString(), null);
				}
				this.m_ip0 = (long)(((int)addressBytes[1] << 24) | ((int)addressBytes[0] << 16) | ((int)addressBytes[3] << 8) | (int)addressBytes[2] | ((int)addressBytes[5] << 24) | ((int)addressBytes[4] << 16) | ((int)addressBytes[7] << 8) | (int)addressBytes[6]);
				this.m_ip1 = (long)(((int)addressBytes[9] << 24) | ((int)addressBytes[8] << 16) | ((int)addressBytes[11] << 8) | (int)addressBytes[10] | ((int)addressBytes[13] << 24) | ((int)addressBytes[12] << 16) | ((int)addressBytes[15] << 8) | (int)addressBytes[14]);
				this.m_eType = ESteamIPType.k_ESteamIPTypeIPv6;
				return;
			}
			else
			{
				if (addressBytes.Length != 4)
				{
					throw new TypeInitializationException("SteamIPAddress_t: Unexpected byte length for Ipv4." + addressBytes.Length.ToString(), null);
				}
				this.m_ip0 = (long)(((int)addressBytes[0] << 24) | ((int)addressBytes[1] << 16) | ((int)addressBytes[2] << 8) | (int)addressBytes[3]);
				this.m_ip1 = 0L;
				this.m_eType = ESteamIPType.k_ESteamIPTypeIPv4;
				return;
			}
		}

		public IPAddress ToIPAddress()
		{
			if (this.m_eType == ESteamIPType.k_ESteamIPTypeIPv4)
			{
				byte[] bytes = BitConverter.GetBytes(this.m_ip0);
				return new IPAddress(new byte[]
				{
					bytes[3],
					bytes[2],
					bytes[1],
					bytes[0]
				});
			}
			byte[] array = new byte[16];
			BitConverter.GetBytes(this.m_ip0).CopyTo(array, 0);
			BitConverter.GetBytes(this.m_ip1).CopyTo(array, 8);
			return new IPAddress(array);
		}

		public override string ToString()
		{
			return this.ToIPAddress().ToString();
		}

		public ESteamIPType GetIPType()
		{
			return this.m_eType;
		}

		public bool IsSet()
		{
			return this.m_ip0 != 0L || this.m_ip1 != 0L;
		}

		private long m_ip0;

		private long m_ip1;

		private ESteamIPType m_eType;
	}
}
