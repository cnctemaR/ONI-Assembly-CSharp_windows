using System;

namespace Steamworks
{
	internal static class CSteamGameServerAPIContext
	{
		internal static void Clear()
		{
			CSteamGameServerAPIContext.m_pSteamClient = IntPtr.Zero;
			CSteamGameServerAPIContext.m_pSteamGameServer = IntPtr.Zero;
			CSteamGameServerAPIContext.m_pSteamUtils = IntPtr.Zero;
			CSteamGameServerAPIContext.m_pSteamNetworking = IntPtr.Zero;
			CSteamGameServerAPIContext.m_pSteamGameServerStats = IntPtr.Zero;
			CSteamGameServerAPIContext.m_pSteamHTTP = IntPtr.Zero;
			CSteamGameServerAPIContext.m_pSteamInventory = IntPtr.Zero;
			CSteamGameServerAPIContext.m_pSteamUGC = IntPtr.Zero;
			CSteamGameServerAPIContext.m_pSteamApps = IntPtr.Zero;
		}

		internal static bool Init()
		{
			HSteamUser hsteamUser = GameServer.GetHSteamUser();
			HSteamPipe hsteamPipe = GameServer.GetHSteamPipe();
			if (hsteamPipe == (HSteamPipe)0)
			{
				return false;
			}
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle("SteamClient019"))
			{
				CSteamGameServerAPIContext.m_pSteamClient = NativeMethods.SteamInternal_CreateInterface(utf8StringHandle);
			}
			if (CSteamGameServerAPIContext.m_pSteamClient == IntPtr.Zero)
			{
				return false;
			}
			CSteamGameServerAPIContext.m_pSteamGameServer = SteamGameServerClient.GetISteamGameServer(hsteamUser, hsteamPipe, "SteamGameServer012");
			if (CSteamGameServerAPIContext.m_pSteamGameServer == IntPtr.Zero)
			{
				return false;
			}
			CSteamGameServerAPIContext.m_pSteamUtils = SteamGameServerClient.GetISteamUtils(hsteamPipe, "SteamUtils009");
			if (CSteamGameServerAPIContext.m_pSteamUtils == IntPtr.Zero)
			{
				return false;
			}
			CSteamGameServerAPIContext.m_pSteamNetworking = SteamGameServerClient.GetISteamNetworking(hsteamUser, hsteamPipe, "SteamNetworking005");
			if (CSteamGameServerAPIContext.m_pSteamNetworking == IntPtr.Zero)
			{
				return false;
			}
			CSteamGameServerAPIContext.m_pSteamGameServerStats = SteamGameServerClient.GetISteamGameServerStats(hsteamUser, hsteamPipe, "SteamGameServerStats001");
			if (CSteamGameServerAPIContext.m_pSteamGameServerStats == IntPtr.Zero)
			{
				return false;
			}
			CSteamGameServerAPIContext.m_pSteamHTTP = SteamGameServerClient.GetISteamHTTP(hsteamUser, hsteamPipe, "STEAMHTTP_INTERFACE_VERSION003");
			if (CSteamGameServerAPIContext.m_pSteamHTTP == IntPtr.Zero)
			{
				return false;
			}
			CSteamGameServerAPIContext.m_pSteamInventory = SteamGameServerClient.GetISteamInventory(hsteamUser, hsteamPipe, "STEAMINVENTORY_INTERFACE_V003");
			if (CSteamGameServerAPIContext.m_pSteamInventory == IntPtr.Zero)
			{
				return false;
			}
			CSteamGameServerAPIContext.m_pSteamUGC = SteamGameServerClient.GetISteamUGC(hsteamUser, hsteamPipe, "STEAMUGC_INTERFACE_VERSION013");
			if (CSteamGameServerAPIContext.m_pSteamUGC == IntPtr.Zero)
			{
				return false;
			}
			CSteamGameServerAPIContext.m_pSteamApps = SteamGameServerClient.GetISteamApps(hsteamUser, hsteamPipe, "STEAMAPPS_INTERFACE_VERSION008");
			return !(CSteamGameServerAPIContext.m_pSteamApps == IntPtr.Zero);
		}

		internal static IntPtr GetSteamClient()
		{
			return CSteamGameServerAPIContext.m_pSteamClient;
		}

		internal static IntPtr GetSteamGameServer()
		{
			return CSteamGameServerAPIContext.m_pSteamGameServer;
		}

		internal static IntPtr GetSteamUtils()
		{
			return CSteamGameServerAPIContext.m_pSteamUtils;
		}

		internal static IntPtr GetSteamNetworking()
		{
			return CSteamGameServerAPIContext.m_pSteamNetworking;
		}

		internal static IntPtr GetSteamGameServerStats()
		{
			return CSteamGameServerAPIContext.m_pSteamGameServerStats;
		}

		internal static IntPtr GetSteamHTTP()
		{
			return CSteamGameServerAPIContext.m_pSteamHTTP;
		}

		internal static IntPtr GetSteamInventory()
		{
			return CSteamGameServerAPIContext.m_pSteamInventory;
		}

		internal static IntPtr GetSteamUGC()
		{
			return CSteamGameServerAPIContext.m_pSteamUGC;
		}

		internal static IntPtr GetSteamApps()
		{
			return CSteamGameServerAPIContext.m_pSteamApps;
		}

		private static IntPtr m_pSteamClient;

		private static IntPtr m_pSteamGameServer;

		private static IntPtr m_pSteamUtils;

		private static IntPtr m_pSteamNetworking;

		private static IntPtr m_pSteamGameServerStats;

		private static IntPtr m_pSteamHTTP;

		private static IntPtr m_pSteamInventory;

		private static IntPtr m_pSteamUGC;

		private static IntPtr m_pSteamApps;
	}
}
