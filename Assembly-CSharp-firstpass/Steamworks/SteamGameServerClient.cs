using System;

namespace Steamworks
{
	public static class SteamGameServerClient
	{
		public static HSteamPipe CreateSteamPipe()
		{
			InteropHelp.TestIfAvailableGameServer();
			return (HSteamPipe)NativeMethods.ISteamClient_CreateSteamPipe(CSteamGameServerAPIContext.GetSteamClient());
		}

		public static bool BReleaseSteamPipe(HSteamPipe hSteamPipe)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamClient_BReleaseSteamPipe(CSteamGameServerAPIContext.GetSteamClient(), hSteamPipe);
		}

		public static HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe)
		{
			InteropHelp.TestIfAvailableGameServer();
			return (HSteamUser)NativeMethods.ISteamClient_ConnectToGlobalUser(CSteamGameServerAPIContext.GetSteamClient(), hSteamPipe);
		}

		public static HSteamUser CreateLocalUser(out HSteamPipe phSteamPipe, EAccountType eAccountType)
		{
			InteropHelp.TestIfAvailableGameServer();
			return (HSteamUser)NativeMethods.ISteamClient_CreateLocalUser(CSteamGameServerAPIContext.GetSteamClient(), out phSteamPipe, eAccountType);
		}

		public static void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
		{
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamClient_ReleaseUser(CSteamGameServerAPIContext.GetSteamClient(), hSteamPipe, hUser);
		}

		public static IntPtr GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamUser(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamGameServer(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static void SetLocalIPBinding(uint unIP, ushort usPort)
		{
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamClient_SetLocalIPBinding(CSteamGameServerAPIContext.GetSteamClient(), unIP, usPort);
		}

		public static IntPtr GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamFriends(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamUtils(CSteamGameServerAPIContext.GetSteamClient(), hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamMatchmaking(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamMatchmakingServers(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamGenericInterface(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamUserStats(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamGameServerStats(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamApps(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamNetworking(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamRemoteStorage(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamScreenshots(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamGameSearch(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamGameSearch(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static uint GetIPCCallCount()
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamClient_GetIPCCallCount(CSteamGameServerAPIContext.GetSteamClient());
		}

		public static void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction)
		{
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamClient_SetWarningMessageHook(CSteamGameServerAPIContext.GetSteamClient(), pFunction);
		}

		public static bool BShutdownIfAllPipesClosed()
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamClient_BShutdownIfAllPipesClosed(CSteamGameServerAPIContext.GetSteamClient());
		}

		public static IntPtr GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamHTTP(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamController(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamUGC(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamAppList(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamMusic(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamMusicRemote(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamHTMLSurface(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamInventory(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamVideo(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamParentalSettings(CSteamGameServerAPIContext.GetSteamClient(), hSteamuser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamInput(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamInput(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamParties(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamParties(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}

		public static IntPtr GetISteamRemotePlay(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				intPtr = NativeMethods.ISteamClient_GetISteamRemotePlay(CSteamGameServerAPIContext.GetSteamClient(), hSteamUser, hSteamPipe, utf8StringHandle);
			}
			return intPtr;
		}
	}
}
