using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SteamNetworkingIdentity : IEquatable<SteamNetworkingIdentity>
	{
		public void Clear()
		{
			NativeMethods.SteamAPI_SteamNetworkingIdentity_Clear(ref this);
		}

		public bool IsInvalid()
		{
			return NativeMethods.SteamAPI_SteamNetworkingIdentity_IsInvalid(ref this);
		}

		public void SetSteamID(CSteamID steamID)
		{
			NativeMethods.SteamAPI_SteamNetworkingIdentity_SetSteamID(ref this, (ulong)steamID);
		}

		public CSteamID GetSteamID()
		{
			return (CSteamID)NativeMethods.SteamAPI_SteamNetworkingIdentity_GetSteamID(ref this);
		}

		public void SetSteamID64(ulong steamID)
		{
			NativeMethods.SteamAPI_SteamNetworkingIdentity_SetSteamID64(ref this, steamID);
		}

		public ulong GetSteamID64()
		{
			return NativeMethods.SteamAPI_SteamNetworkingIdentity_GetSteamID64(ref this);
		}

		public void SetIPAddr(SteamNetworkingIPAddr addr)
		{
			NativeMethods.SteamAPI_SteamNetworkingIdentity_SetIPAddr(ref this, ref addr);
		}

		public SteamNetworkingIPAddr GetIPAddr()
		{
			throw new NotImplementedException();
		}

		public void SetIPv4Addr(uint nIPv4, ushort nPort)
		{
			NativeMethods.SteamAPI_SteamNetworkingIdentity_SetIPv4Addr(ref this, nIPv4, nPort);
		}

		public uint GetIPv4()
		{
			return NativeMethods.SteamAPI_SteamNetworkingIdentity_GetIPv4(ref this);
		}

		public ESteamNetworkingFakeIPType GetFakeIPType()
		{
			return NativeMethods.SteamAPI_SteamNetworkingIdentity_GetFakeIPType(ref this);
		}

		public bool IsFakeIP()
		{
			return this.GetFakeIPType() > ESteamNetworkingFakeIPType.k_ESteamNetworkingFakeIPType_NotFake;
		}

		public void SetLocalHost()
		{
			NativeMethods.SteamAPI_SteamNetworkingIdentity_SetLocalHost(ref this);
		}

		public bool IsLocalHost()
		{
			return NativeMethods.SteamAPI_SteamNetworkingIdentity_IsLocalHost(ref this);
		}

		public bool SetGenericString(string pszString)
		{
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszString))
			{
				flag = NativeMethods.SteamAPI_SteamNetworkingIdentity_SetGenericString(ref this, utf8StringHandle);
			}
			return flag;
		}

		public string GetGenericString()
		{
			return InteropHelp.PtrToStringUTF8(NativeMethods.SteamAPI_SteamNetworkingIdentity_GetGenericString(ref this));
		}

		public bool SetGenericBytes(byte[] data, uint cbLen)
		{
			return NativeMethods.SteamAPI_SteamNetworkingIdentity_SetGenericBytes(ref this, data, cbLen);
		}

		public byte[] GetGenericBytes(out int cbLen)
		{
			throw new NotImplementedException();
		}

		public bool Equals(SteamNetworkingIdentity x)
		{
			return NativeMethods.SteamAPI_SteamNetworkingIdentity_IsEqualTo(ref this, ref x);
		}

		public void ToString(out string buf)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(128);
			NativeMethods.SteamAPI_SteamNetworkingIdentity_ToString(ref this, intPtr, 128U);
			buf = InteropHelp.PtrToStringUTF8(intPtr);
			Marshal.FreeHGlobal(intPtr);
		}

		public bool ParseString(string pszStr)
		{
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pszStr))
			{
				flag = NativeMethods.SteamAPI_SteamNetworkingIdentity_ParseString(ref this, utf8StringHandle);
			}
			return flag;
		}

		public ESteamNetworkingIdentityType m_eType;

		private int m_cbSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private uint[] m_reserved;

		public const int k_cchMaxString = 128;

		public const int k_cchMaxGenericString = 32;

		public const int k_cbMaxGenericBytes = 32;
	}
}
