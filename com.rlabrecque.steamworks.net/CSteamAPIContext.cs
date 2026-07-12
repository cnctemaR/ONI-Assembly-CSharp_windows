using System;

namespace Steamworks
{
	internal static class CSteamAPIContext
	{
		internal static void Clear()
		{
			CSteamAPIContext.m_pSteamClient = IntPtr.Zero;
			CSteamAPIContext.m_pSteamUser = IntPtr.Zero;
			CSteamAPIContext.m_pSteamFriends = IntPtr.Zero;
			CSteamAPIContext.m_pSteamUtils = IntPtr.Zero;
			CSteamAPIContext.m_pSteamMatchmaking = IntPtr.Zero;
			CSteamAPIContext.m_pSteamUserStats = IntPtr.Zero;
			CSteamAPIContext.m_pSteamApps = IntPtr.Zero;
			CSteamAPIContext.m_pSteamMatchmakingServers = IntPtr.Zero;
			CSteamAPIContext.m_pSteamNetworking = IntPtr.Zero;
			CSteamAPIContext.m_pSteamRemoteStorage = IntPtr.Zero;
			CSteamAPIContext.m_pSteamHTTP = IntPtr.Zero;
			CSteamAPIContext.m_pSteamScreenshots = IntPtr.Zero;
			CSteamAPIContext.m_pSteamGameSearch = IntPtr.Zero;
			CSteamAPIContext.m_pSteamMusic = IntPtr.Zero;
			CSteamAPIContext.m_pController = IntPtr.Zero;
			CSteamAPIContext.m_pSteamUGC = IntPtr.Zero;
			CSteamAPIContext.m_pSteamAppList = IntPtr.Zero;
			CSteamAPIContext.m_pSteamMusic = IntPtr.Zero;
			CSteamAPIContext.m_pSteamMusicRemote = IntPtr.Zero;
			CSteamAPIContext.m_pSteamHTMLSurface = IntPtr.Zero;
			CSteamAPIContext.m_pSteamInventory = IntPtr.Zero;
			CSteamAPIContext.m_pSteamVideo = IntPtr.Zero;
			CSteamAPIContext.m_pSteamParentalSettings = IntPtr.Zero;
			CSteamAPIContext.m_pSteamInput = IntPtr.Zero;
			CSteamAPIContext.m_pSteamParties = IntPtr.Zero;
			CSteamAPIContext.m_pSteamRemotePlay = IntPtr.Zero;
			CSteamAPIContext.m_pSteamNetworkingUtils = IntPtr.Zero;
			CSteamAPIContext.m_pSteamNetworkingSockets = IntPtr.Zero;
			CSteamAPIContext.m_pSteamNetworkingMessages = IntPtr.Zero;
		}

		internal static bool Init()
		{
			HSteamUser hsteamUser = SteamAPI.GetHSteamUser();
			HSteamPipe hsteamPipe = SteamAPI.GetHSteamPipe();
			if (hsteamPipe == (HSteamPipe)0)
			{
				return false;
			}
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle("SteamClient020"))
			{
				CSteamAPIContext.m_pSteamClient = NativeMethods.SteamInternal_CreateInterface(utf8StringHandle);
			}
			if (CSteamAPIContext.m_pSteamClient == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamUser = SteamClient.GetISteamUser(hsteamUser, hsteamPipe, "SteamUser021");
			if (CSteamAPIContext.m_pSteamUser == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamFriends = SteamClient.GetISteamFriends(hsteamUser, hsteamPipe, "SteamFriends017");
			if (CSteamAPIContext.m_pSteamFriends == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamUtils = SteamClient.GetISteamUtils(hsteamPipe, "SteamUtils010");
			if (CSteamAPIContext.m_pSteamUtils == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamMatchmaking = SteamClient.GetISteamMatchmaking(hsteamUser, hsteamPipe, "SteamMatchMaking009");
			if (CSteamAPIContext.m_pSteamMatchmaking == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamMatchmakingServers = SteamClient.GetISteamMatchmakingServers(hsteamUser, hsteamPipe, "SteamMatchMakingServers002");
			if (CSteamAPIContext.m_pSteamMatchmakingServers == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamUserStats = SteamClient.GetISteamUserStats(hsteamUser, hsteamPipe, "STEAMUSERSTATS_INTERFACE_VERSION012");
			if (CSteamAPIContext.m_pSteamUserStats == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamApps = SteamClient.GetISteamApps(hsteamUser, hsteamPipe, "STEAMAPPS_INTERFACE_VERSION008");
			if (CSteamAPIContext.m_pSteamApps == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamNetworking = SteamClient.GetISteamNetworking(hsteamUser, hsteamPipe, "SteamNetworking006");
			if (CSteamAPIContext.m_pSteamNetworking == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamRemoteStorage = SteamClient.GetISteamRemoteStorage(hsteamUser, hsteamPipe, "STEAMREMOTESTORAGE_INTERFACE_VERSION016");
			if (CSteamAPIContext.m_pSteamRemoteStorage == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamScreenshots = SteamClient.GetISteamScreenshots(hsteamUser, hsteamPipe, "STEAMSCREENSHOTS_INTERFACE_VERSION003");
			if (CSteamAPIContext.m_pSteamScreenshots == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamGameSearch = SteamClient.GetISteamGameSearch(hsteamUser, hsteamPipe, "SteamMatchGameSearch001");
			if (CSteamAPIContext.m_pSteamGameSearch == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamHTTP = SteamClient.GetISteamHTTP(hsteamUser, hsteamPipe, "STEAMHTTP_INTERFACE_VERSION003");
			if (CSteamAPIContext.m_pSteamHTTP == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamUGC = SteamClient.GetISteamUGC(hsteamUser, hsteamPipe, "STEAMUGC_INTERFACE_VERSION016");
			if (CSteamAPIContext.m_pSteamUGC == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamAppList = SteamClient.GetISteamAppList(hsteamUser, hsteamPipe, "STEAMAPPLIST_INTERFACE_VERSION001");
			if (CSteamAPIContext.m_pSteamAppList == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamMusic = SteamClient.GetISteamMusic(hsteamUser, hsteamPipe, "STEAMMUSIC_INTERFACE_VERSION001");
			if (CSteamAPIContext.m_pSteamMusic == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamMusicRemote = SteamClient.GetISteamMusicRemote(hsteamUser, hsteamPipe, "STEAMMUSICREMOTE_INTERFACE_VERSION001");
			if (CSteamAPIContext.m_pSteamMusicRemote == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamHTMLSurface = SteamClient.GetISteamHTMLSurface(hsteamUser, hsteamPipe, "STEAMHTMLSURFACE_INTERFACE_VERSION_005");
			if (CSteamAPIContext.m_pSteamHTMLSurface == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamInventory = SteamClient.GetISteamInventory(hsteamUser, hsteamPipe, "STEAMINVENTORY_INTERFACE_V003");
			if (CSteamAPIContext.m_pSteamInventory == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamVideo = SteamClient.GetISteamVideo(hsteamUser, hsteamPipe, "STEAMVIDEO_INTERFACE_V002");
			if (CSteamAPIContext.m_pSteamVideo == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamParentalSettings = SteamClient.GetISteamParentalSettings(hsteamUser, hsteamPipe, "STEAMPARENTALSETTINGS_INTERFACE_VERSION001");
			if (CSteamAPIContext.m_pSteamParentalSettings == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamInput = SteamClient.GetISteamInput(hsteamUser, hsteamPipe, "SteamInput006");
			if (CSteamAPIContext.m_pSteamInput == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamParties = SteamClient.GetISteamParties(hsteamUser, hsteamPipe, "SteamParties002");
			if (CSteamAPIContext.m_pSteamParties == IntPtr.Zero)
			{
				return false;
			}
			CSteamAPIContext.m_pSteamRemotePlay = SteamClient.GetISteamRemotePlay(hsteamUser, hsteamPipe, "STEAMREMOTEPLAY_INTERFACE_VERSION001");
			if (CSteamAPIContext.m_pSteamRemotePlay == IntPtr.Zero)
			{
				return false;
			}
			using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle("SteamNetworkingUtils004"))
			{
				CSteamAPIContext.m_pSteamNetworkingUtils = ((NativeMethods.SteamInternal_FindOrCreateUserInterface(hsteamUser, utf8StringHandle2) != IntPtr.Zero) ? NativeMethods.SteamInternal_FindOrCreateUserInterface(hsteamUser, utf8StringHandle2) : NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hsteamUser, utf8StringHandle2));
			}
			if (CSteamAPIContext.m_pSteamNetworkingUtils == IntPtr.Zero)
			{
				return false;
			}
			using (InteropHelp.UTF8StringHandle utf8StringHandle3 = new InteropHelp.UTF8StringHandle("SteamNetworkingSockets012"))
			{
				CSteamAPIContext.m_pSteamNetworkingSockets = NativeMethods.SteamInternal_FindOrCreateUserInterface(hsteamUser, utf8StringHandle3);
			}
			if (CSteamAPIContext.m_pSteamNetworkingSockets == IntPtr.Zero)
			{
				return false;
			}
			using (InteropHelp.UTF8StringHandle utf8StringHandle4 = new InteropHelp.UTF8StringHandle("SteamNetworkingMessages002"))
			{
				CSteamAPIContext.m_pSteamNetworkingMessages = NativeMethods.SteamInternal_FindOrCreateUserInterface(hsteamUser, utf8StringHandle4);
			}
			return !(CSteamAPIContext.m_pSteamNetworkingMessages == IntPtr.Zero);
		}

		internal static IntPtr GetSteamClient()
		{
			return CSteamAPIContext.m_pSteamClient;
		}

		internal static IntPtr GetSteamUser()
		{
			return CSteamAPIContext.m_pSteamUser;
		}

		internal static IntPtr GetSteamFriends()
		{
			return CSteamAPIContext.m_pSteamFriends;
		}

		internal static IntPtr GetSteamUtils()
		{
			return CSteamAPIContext.m_pSteamUtils;
		}

		internal static IntPtr GetSteamMatchmaking()
		{
			return CSteamAPIContext.m_pSteamMatchmaking;
		}

		internal static IntPtr GetSteamUserStats()
		{
			return CSteamAPIContext.m_pSteamUserStats;
		}

		internal static IntPtr GetSteamApps()
		{
			return CSteamAPIContext.m_pSteamApps;
		}

		internal static IntPtr GetSteamMatchmakingServers()
		{
			return CSteamAPIContext.m_pSteamMatchmakingServers;
		}

		internal static IntPtr GetSteamNetworking()
		{
			return CSteamAPIContext.m_pSteamNetworking;
		}

		internal static IntPtr GetSteamRemoteStorage()
		{
			return CSteamAPIContext.m_pSteamRemoteStorage;
		}

		internal static IntPtr GetSteamScreenshots()
		{
			return CSteamAPIContext.m_pSteamScreenshots;
		}

		internal static IntPtr GetSteamGameSearch()
		{
			return CSteamAPIContext.m_pSteamGameSearch;
		}

		internal static IntPtr GetSteamHTTP()
		{
			return CSteamAPIContext.m_pSteamHTTP;
		}

		internal static IntPtr GetSteamController()
		{
			return CSteamAPIContext.m_pController;
		}

		internal static IntPtr GetSteamUGC()
		{
			return CSteamAPIContext.m_pSteamUGC;
		}

		internal static IntPtr GetSteamAppList()
		{
			return CSteamAPIContext.m_pSteamAppList;
		}

		internal static IntPtr GetSteamMusic()
		{
			return CSteamAPIContext.m_pSteamMusic;
		}

		internal static IntPtr GetSteamMusicRemote()
		{
			return CSteamAPIContext.m_pSteamMusicRemote;
		}

		internal static IntPtr GetSteamHTMLSurface()
		{
			return CSteamAPIContext.m_pSteamHTMLSurface;
		}

		internal static IntPtr GetSteamInventory()
		{
			return CSteamAPIContext.m_pSteamInventory;
		}

		internal static IntPtr GetSteamVideo()
		{
			return CSteamAPIContext.m_pSteamVideo;
		}

		internal static IntPtr GetSteamParentalSettings()
		{
			return CSteamAPIContext.m_pSteamParentalSettings;
		}

		internal static IntPtr GetSteamInput()
		{
			return CSteamAPIContext.m_pSteamInput;
		}

		internal static IntPtr GetSteamParties()
		{
			return CSteamAPIContext.m_pSteamParties;
		}

		internal static IntPtr GetSteamRemotePlay()
		{
			return CSteamAPIContext.m_pSteamRemotePlay;
		}

		internal static IntPtr GetSteamNetworkingUtils()
		{
			return CSteamAPIContext.m_pSteamNetworkingUtils;
		}

		internal static IntPtr GetSteamNetworkingSockets()
		{
			return CSteamAPIContext.m_pSteamNetworkingSockets;
		}

		internal static IntPtr GetSteamNetworkingMessages()
		{
			return CSteamAPIContext.m_pSteamNetworkingMessages;
		}

		private static IntPtr m_pSteamClient;

		private static IntPtr m_pSteamUser;

		private static IntPtr m_pSteamFriends;

		private static IntPtr m_pSteamUtils;

		private static IntPtr m_pSteamMatchmaking;

		private static IntPtr m_pSteamUserStats;

		private static IntPtr m_pSteamApps;

		private static IntPtr m_pSteamMatchmakingServers;

		private static IntPtr m_pSteamNetworking;

		private static IntPtr m_pSteamRemoteStorage;

		private static IntPtr m_pSteamScreenshots;

		private static IntPtr m_pSteamGameSearch;

		private static IntPtr m_pSteamHTTP;

		private static IntPtr m_pController;

		private static IntPtr m_pSteamUGC;

		private static IntPtr m_pSteamAppList;

		private static IntPtr m_pSteamMusic;

		private static IntPtr m_pSteamMusicRemote;

		private static IntPtr m_pSteamHTMLSurface;

		private static IntPtr m_pSteamInventory;

		private static IntPtr m_pSteamVideo;

		private static IntPtr m_pSteamParentalSettings;

		private static IntPtr m_pSteamInput;

		private static IntPtr m_pSteamParties;

		private static IntPtr m_pSteamRemotePlay;

		private static IntPtr m_pSteamNetworkingUtils;

		private static IntPtr m_pSteamNetworkingSockets;

		private static IntPtr m_pSteamNetworkingMessages;
	}
}
