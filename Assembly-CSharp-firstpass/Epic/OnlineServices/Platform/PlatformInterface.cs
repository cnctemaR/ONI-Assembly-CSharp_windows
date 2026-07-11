using System;
using System.Runtime.InteropServices;
using System.Text;
using Epic.OnlineServices.Achievements;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Ecom;
using Epic.OnlineServices.Friends;
using Epic.OnlineServices.Leaderboards;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Metrics;
using Epic.OnlineServices.P2P;
using Epic.OnlineServices.PlayerDataStorage;
using Epic.OnlineServices.Presence;
using Epic.OnlineServices.Sessions;
using Epic.OnlineServices.Stats;
using Epic.OnlineServices.TitleStorage;
using Epic.OnlineServices.UI;
using Epic.OnlineServices.UserInfo;

namespace Epic.OnlineServices.Platform
{
	public sealed class PlatformInterface : Handle
	{
		public PlatformInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public static Result Initialize(InitializeOptions options)
		{
			InitializeOptionsInternal initializeOptionsInternal = Helper.CopyProperties<InitializeOptionsInternal>(options);
			int[] array = new int[] { 1, 1 };
			IntPtr zero = IntPtr.Zero;
			Helper.TryMarshalSet<int>(ref zero, array);
			initializeOptionsInternal.Reserved = zero;
			Result result = PlatformInterface.EOS_Initialize(ref initializeOptionsInternal);
			Helper.TryMarshalDispose<InitializeOptionsInternal>(ref initializeOptionsInternal);
			Helper.TryMarshalDispose(ref zero);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public static Result Shutdown()
		{
			Result result = PlatformInterface.EOS_Shutdown();
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public static PlatformInterface Create(Options options)
		{
			OptionsInternal optionsInternal = Helper.CopyProperties<OptionsInternal>(options);
			IntPtr intPtr = PlatformInterface.EOS_Platform_Create(ref optionsInternal);
			Helper.TryMarshalDispose<OptionsInternal>(ref optionsInternal);
			PlatformInterface @default = Helper.GetDefault<PlatformInterface>();
			Helper.TryMarshalGet<PlatformInterface>(intPtr, out @default);
			return @default;
		}

		public void Release()
		{
			PlatformInterface.EOS_Platform_Release(base.InnerHandle);
		}

		public void Tick()
		{
			PlatformInterface.EOS_Platform_Tick(base.InnerHandle);
		}

		public MetricsInterface GetMetricsInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetMetricsInterface(base.InnerHandle);
			MetricsInterface @default = Helper.GetDefault<MetricsInterface>();
			Helper.TryMarshalGet<MetricsInterface>(intPtr, out @default);
			return @default;
		}

		public AuthInterface GetAuthInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetAuthInterface(base.InnerHandle);
			AuthInterface @default = Helper.GetDefault<AuthInterface>();
			Helper.TryMarshalGet<AuthInterface>(intPtr, out @default);
			return @default;
		}

		public ConnectInterface GetConnectInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetConnectInterface(base.InnerHandle);
			ConnectInterface @default = Helper.GetDefault<ConnectInterface>();
			Helper.TryMarshalGet<ConnectInterface>(intPtr, out @default);
			return @default;
		}

		public EcomInterface GetEcomInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetEcomInterface(base.InnerHandle);
			EcomInterface @default = Helper.GetDefault<EcomInterface>();
			Helper.TryMarshalGet<EcomInterface>(intPtr, out @default);
			return @default;
		}

		public UIInterface GetUIInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetUIInterface(base.InnerHandle);
			UIInterface @default = Helper.GetDefault<UIInterface>();
			Helper.TryMarshalGet<UIInterface>(intPtr, out @default);
			return @default;
		}

		public FriendsInterface GetFriendsInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetFriendsInterface(base.InnerHandle);
			FriendsInterface @default = Helper.GetDefault<FriendsInterface>();
			Helper.TryMarshalGet<FriendsInterface>(intPtr, out @default);
			return @default;
		}

		public PresenceInterface GetPresenceInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetPresenceInterface(base.InnerHandle);
			PresenceInterface @default = Helper.GetDefault<PresenceInterface>();
			Helper.TryMarshalGet<PresenceInterface>(intPtr, out @default);
			return @default;
		}

		public SessionsInterface GetSessionsInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetSessionsInterface(base.InnerHandle);
			SessionsInterface @default = Helper.GetDefault<SessionsInterface>();
			Helper.TryMarshalGet<SessionsInterface>(intPtr, out @default);
			return @default;
		}

		public LobbyInterface GetLobbyInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetLobbyInterface(base.InnerHandle);
			LobbyInterface @default = Helper.GetDefault<LobbyInterface>();
			Helper.TryMarshalGet<LobbyInterface>(intPtr, out @default);
			return @default;
		}

		public UserInfoInterface GetUserInfoInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetUserInfoInterface(base.InnerHandle);
			UserInfoInterface @default = Helper.GetDefault<UserInfoInterface>();
			Helper.TryMarshalGet<UserInfoInterface>(intPtr, out @default);
			return @default;
		}

		public P2PInterface GetP2PInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetP2PInterface(base.InnerHandle);
			P2PInterface @default = Helper.GetDefault<P2PInterface>();
			Helper.TryMarshalGet<P2PInterface>(intPtr, out @default);
			return @default;
		}

		public PlayerDataStorageInterface GetPlayerDataStorageInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetPlayerDataStorageInterface(base.InnerHandle);
			PlayerDataStorageInterface @default = Helper.GetDefault<PlayerDataStorageInterface>();
			Helper.TryMarshalGet<PlayerDataStorageInterface>(intPtr, out @default);
			return @default;
		}

		public TitleStorageInterface GetTitleStorageInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetTitleStorageInterface(base.InnerHandle);
			TitleStorageInterface @default = Helper.GetDefault<TitleStorageInterface>();
			Helper.TryMarshalGet<TitleStorageInterface>(intPtr, out @default);
			return @default;
		}

		public AchievementsInterface GetAchievementsInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetAchievementsInterface(base.InnerHandle);
			AchievementsInterface @default = Helper.GetDefault<AchievementsInterface>();
			Helper.TryMarshalGet<AchievementsInterface>(intPtr, out @default);
			return @default;
		}

		public StatsInterface GetStatsInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetStatsInterface(base.InnerHandle);
			StatsInterface @default = Helper.GetDefault<StatsInterface>();
			Helper.TryMarshalGet<StatsInterface>(intPtr, out @default);
			return @default;
		}

		public LeaderboardsInterface GetLeaderboardsInterface()
		{
			IntPtr intPtr = PlatformInterface.EOS_Platform_GetLeaderboardsInterface(base.InnerHandle);
			LeaderboardsInterface @default = Helper.GetDefault<LeaderboardsInterface>();
			Helper.TryMarshalGet<LeaderboardsInterface>(intPtr, out @default);
			return @default;
		}

		public Result GetActiveCountryCode(EpicAccountId localUserId, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result result = PlatformInterface.EOS_Platform_GetActiveCountryCode(base.InnerHandle, localUserId.InnerHandle, outBuffer, ref inOutBufferLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetActiveLocaleCode(EpicAccountId localUserId, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result result = PlatformInterface.EOS_Platform_GetActiveLocaleCode(base.InnerHandle, localUserId.InnerHandle, outBuffer, ref inOutBufferLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetOverrideCountryCode(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result result = PlatformInterface.EOS_Platform_GetOverrideCountryCode(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetOverrideLocaleCode(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result result = PlatformInterface.EOS_Platform_GetOverrideLocaleCode(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetOverrideCountryCode(string newCountryCode)
		{
			Result result = PlatformInterface.EOS_Platform_SetOverrideCountryCode(base.InnerHandle, newCountryCode);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetOverrideLocaleCode(string newLocaleCode)
		{
			Result result = PlatformInterface.EOS_Platform_SetOverrideLocaleCode(base.InnerHandle, newLocaleCode);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CheckForLauncherAndRestart()
		{
			Result result = PlatformInterface.EOS_Platform_CheckForLauncherAndRestart(base.InnerHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Platform_CheckForLauncherAndRestart(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Platform_SetOverrideLocaleCode(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string newLocaleCode);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Platform_SetOverrideCountryCode(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string newCountryCode);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Platform_GetOverrideLocaleCode(IntPtr handle, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Platform_GetOverrideCountryCode(IntPtr handle, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Platform_GetActiveLocaleCode(IntPtr handle, IntPtr localUserId, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Platform_GetActiveCountryCode(IntPtr handle, IntPtr localUserId, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetLeaderboardsInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetStatsInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetAchievementsInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetTitleStorageInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetPlayerDataStorageInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetP2PInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetUserInfoInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetLobbyInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetSessionsInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetPresenceInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetFriendsInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetUIInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetEcomInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetConnectInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetAuthInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_GetMetricsInterface(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Platform_Tick(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Platform_Release(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Platform_Create(ref OptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Shutdown();

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Initialize(ref InitializeOptionsInternal options);

		public const int OptionsApiLatest = 8;

		public const int LocalecodeMaxBufferLen = 10;

		public const int LocalecodeMaxLength = 9;

		public const int CountrycodeMaxBufferLen = 5;

		public const int CountrycodeMaxLength = 4;

		public const int InitializeApiLatest = 3;
	}
}
