using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	public sealed class AchievementsInterface : Handle
	{
		public AchievementsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryDefinitions(QueryDefinitionsOptions options, object clientData, OnQueryDefinitionsCompleteCallback completionDelegate)
		{
			QueryDefinitionsOptionsInternal queryDefinitionsOptionsInternal = Helper.CopyProperties<QueryDefinitionsOptionsInternal>(options);
			OnQueryDefinitionsCompleteCallbackInternal onQueryDefinitionsCompleteCallbackInternal = new OnQueryDefinitionsCompleteCallbackInternal(AchievementsInterface.OnQueryDefinitionsComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryDefinitionsCompleteCallbackInternal, Array.Empty<Delegate>());
			AchievementsInterface.EOS_Achievements_QueryDefinitions(base.InnerHandle, ref queryDefinitionsOptionsInternal, zero, onQueryDefinitionsCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryDefinitionsOptionsInternal>(ref queryDefinitionsOptionsInternal);
		}

		public uint GetAchievementDefinitionCount(GetAchievementDefinitionCountOptions options)
		{
			GetAchievementDefinitionCountOptionsInternal getAchievementDefinitionCountOptionsInternal = Helper.CopyProperties<GetAchievementDefinitionCountOptionsInternal>(options);
			uint num = AchievementsInterface.EOS_Achievements_GetAchievementDefinitionCount(base.InnerHandle, ref getAchievementDefinitionCountOptionsInternal);
			Helper.TryMarshalDispose<GetAchievementDefinitionCountOptionsInternal>(ref getAchievementDefinitionCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyAchievementDefinitionV2ByIndex(CopyAchievementDefinitionV2ByIndexOptions options, out DefinitionV2 outDefinition)
		{
			CopyAchievementDefinitionV2ByIndexOptionsInternal copyAchievementDefinitionV2ByIndexOptionsInternal = Helper.CopyProperties<CopyAchievementDefinitionV2ByIndexOptionsInternal>(options);
			outDefinition = Helper.GetDefault<DefinitionV2>();
			IntPtr zero = IntPtr.Zero;
			Result result = AchievementsInterface.EOS_Achievements_CopyAchievementDefinitionV2ByIndex(base.InnerHandle, ref copyAchievementDefinitionV2ByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyAchievementDefinitionV2ByIndexOptionsInternal>(ref copyAchievementDefinitionV2ByIndexOptionsInternal);
			if (Helper.TryMarshalGet<DefinitionV2Internal, DefinitionV2>(zero, out outDefinition))
			{
				AchievementsInterface.EOS_Achievements_DefinitionV2_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyAchievementDefinitionV2ByAchievementId(CopyAchievementDefinitionV2ByAchievementIdOptions options, out DefinitionV2 outDefinition)
		{
			CopyAchievementDefinitionV2ByAchievementIdOptionsInternal copyAchievementDefinitionV2ByAchievementIdOptionsInternal = Helper.CopyProperties<CopyAchievementDefinitionV2ByAchievementIdOptionsInternal>(options);
			outDefinition = Helper.GetDefault<DefinitionV2>();
			IntPtr zero = IntPtr.Zero;
			Result result = AchievementsInterface.EOS_Achievements_CopyAchievementDefinitionV2ByAchievementId(base.InnerHandle, ref copyAchievementDefinitionV2ByAchievementIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyAchievementDefinitionV2ByAchievementIdOptionsInternal>(ref copyAchievementDefinitionV2ByAchievementIdOptionsInternal);
			if (Helper.TryMarshalGet<DefinitionV2Internal, DefinitionV2>(zero, out outDefinition))
			{
				AchievementsInterface.EOS_Achievements_DefinitionV2_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void QueryPlayerAchievements(QueryPlayerAchievementsOptions options, object clientData, OnQueryPlayerAchievementsCompleteCallback completionDelegate)
		{
			QueryPlayerAchievementsOptionsInternal queryPlayerAchievementsOptionsInternal = Helper.CopyProperties<QueryPlayerAchievementsOptionsInternal>(options);
			OnQueryPlayerAchievementsCompleteCallbackInternal onQueryPlayerAchievementsCompleteCallbackInternal = new OnQueryPlayerAchievementsCompleteCallbackInternal(AchievementsInterface.OnQueryPlayerAchievementsComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryPlayerAchievementsCompleteCallbackInternal, Array.Empty<Delegate>());
			AchievementsInterface.EOS_Achievements_QueryPlayerAchievements(base.InnerHandle, ref queryPlayerAchievementsOptionsInternal, zero, onQueryPlayerAchievementsCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryPlayerAchievementsOptionsInternal>(ref queryPlayerAchievementsOptionsInternal);
		}

		public uint GetPlayerAchievementCount(GetPlayerAchievementCountOptions options)
		{
			GetPlayerAchievementCountOptionsInternal getPlayerAchievementCountOptionsInternal = Helper.CopyProperties<GetPlayerAchievementCountOptionsInternal>(options);
			uint num = AchievementsInterface.EOS_Achievements_GetPlayerAchievementCount(base.InnerHandle, ref getPlayerAchievementCountOptionsInternal);
			Helper.TryMarshalDispose<GetPlayerAchievementCountOptionsInternal>(ref getPlayerAchievementCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyPlayerAchievementByIndex(CopyPlayerAchievementByIndexOptions options, out PlayerAchievement outAchievement)
		{
			CopyPlayerAchievementByIndexOptionsInternal copyPlayerAchievementByIndexOptionsInternal = Helper.CopyProperties<CopyPlayerAchievementByIndexOptionsInternal>(options);
			outAchievement = Helper.GetDefault<PlayerAchievement>();
			IntPtr zero = IntPtr.Zero;
			Result result = AchievementsInterface.EOS_Achievements_CopyPlayerAchievementByIndex(base.InnerHandle, ref copyPlayerAchievementByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyPlayerAchievementByIndexOptionsInternal>(ref copyPlayerAchievementByIndexOptionsInternal);
			if (Helper.TryMarshalGet<PlayerAchievementInternal, PlayerAchievement>(zero, out outAchievement))
			{
				AchievementsInterface.EOS_Achievements_PlayerAchievement_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyPlayerAchievementByAchievementId(CopyPlayerAchievementByAchievementIdOptions options, out PlayerAchievement outAchievement)
		{
			CopyPlayerAchievementByAchievementIdOptionsInternal copyPlayerAchievementByAchievementIdOptionsInternal = Helper.CopyProperties<CopyPlayerAchievementByAchievementIdOptionsInternal>(options);
			outAchievement = Helper.GetDefault<PlayerAchievement>();
			IntPtr zero = IntPtr.Zero;
			Result result = AchievementsInterface.EOS_Achievements_CopyPlayerAchievementByAchievementId(base.InnerHandle, ref copyPlayerAchievementByAchievementIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyPlayerAchievementByAchievementIdOptionsInternal>(ref copyPlayerAchievementByAchievementIdOptionsInternal);
			if (Helper.TryMarshalGet<PlayerAchievementInternal, PlayerAchievement>(zero, out outAchievement))
			{
				AchievementsInterface.EOS_Achievements_PlayerAchievement_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void UnlockAchievements(UnlockAchievementsOptions options, object clientData, OnUnlockAchievementsCompleteCallback completionDelegate)
		{
			UnlockAchievementsOptionsInternal unlockAchievementsOptionsInternal = Helper.CopyProperties<UnlockAchievementsOptionsInternal>(options);
			OnUnlockAchievementsCompleteCallbackInternal onUnlockAchievementsCompleteCallbackInternal = new OnUnlockAchievementsCompleteCallbackInternal(AchievementsInterface.OnUnlockAchievementsComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onUnlockAchievementsCompleteCallbackInternal, Array.Empty<Delegate>());
			AchievementsInterface.EOS_Achievements_UnlockAchievements(base.InnerHandle, ref unlockAchievementsOptionsInternal, zero, onUnlockAchievementsCompleteCallbackInternal);
			Helper.TryMarshalDispose<UnlockAchievementsOptionsInternal>(ref unlockAchievementsOptionsInternal);
		}

		public ulong AddNotifyAchievementsUnlockedV2(AddNotifyAchievementsUnlockedV2Options options, object clientData, OnAchievementsUnlockedCallbackV2 notificationFn)
		{
			AddNotifyAchievementsUnlockedV2OptionsInternal addNotifyAchievementsUnlockedV2OptionsInternal = Helper.CopyProperties<AddNotifyAchievementsUnlockedV2OptionsInternal>(options);
			OnAchievementsUnlockedCallbackV2Internal onAchievementsUnlockedCallbackV2Internal = new OnAchievementsUnlockedCallbackV2Internal(AchievementsInterface.OnAchievementsUnlockedV2);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onAchievementsUnlockedCallbackV2Internal, Array.Empty<Delegate>());
			ulong num = AchievementsInterface.EOS_Achievements_AddNotifyAchievementsUnlockedV2(base.InnerHandle, ref addNotifyAchievementsUnlockedV2OptionsInternal, zero, onAchievementsUnlockedCallbackV2Internal);
			Helper.TryMarshalDispose<AddNotifyAchievementsUnlockedV2OptionsInternal>(ref addNotifyAchievementsUnlockedV2OptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyAchievementsUnlocked(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			AchievementsInterface.EOS_Achievements_RemoveNotifyAchievementsUnlocked(base.InnerHandle, inId);
		}

		public Result CopyAchievementDefinitionByIndex(CopyAchievementDefinitionByIndexOptions options, out Definition outDefinition)
		{
			CopyAchievementDefinitionByIndexOptionsInternal copyAchievementDefinitionByIndexOptionsInternal = Helper.CopyProperties<CopyAchievementDefinitionByIndexOptionsInternal>(options);
			outDefinition = Helper.GetDefault<Definition>();
			IntPtr zero = IntPtr.Zero;
			Result result = AchievementsInterface.EOS_Achievements_CopyAchievementDefinitionByIndex(base.InnerHandle, ref copyAchievementDefinitionByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyAchievementDefinitionByIndexOptionsInternal>(ref copyAchievementDefinitionByIndexOptionsInternal);
			if (Helper.TryMarshalGet<DefinitionInternal, Definition>(zero, out outDefinition))
			{
				AchievementsInterface.EOS_Achievements_Definition_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyAchievementDefinitionByAchievementId(CopyAchievementDefinitionByAchievementIdOptions options, out Definition outDefinition)
		{
			CopyAchievementDefinitionByAchievementIdOptionsInternal copyAchievementDefinitionByAchievementIdOptionsInternal = Helper.CopyProperties<CopyAchievementDefinitionByAchievementIdOptionsInternal>(options);
			outDefinition = Helper.GetDefault<Definition>();
			IntPtr zero = IntPtr.Zero;
			Result result = AchievementsInterface.EOS_Achievements_CopyAchievementDefinitionByAchievementId(base.InnerHandle, ref copyAchievementDefinitionByAchievementIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyAchievementDefinitionByAchievementIdOptionsInternal>(ref copyAchievementDefinitionByAchievementIdOptionsInternal);
			if (Helper.TryMarshalGet<DefinitionInternal, Definition>(zero, out outDefinition))
			{
				AchievementsInterface.EOS_Achievements_Definition_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetUnlockedAchievementCount(GetUnlockedAchievementCountOptions options)
		{
			GetUnlockedAchievementCountOptionsInternal getUnlockedAchievementCountOptionsInternal = Helper.CopyProperties<GetUnlockedAchievementCountOptionsInternal>(options);
			uint num = AchievementsInterface.EOS_Achievements_GetUnlockedAchievementCount(base.InnerHandle, ref getUnlockedAchievementCountOptionsInternal);
			Helper.TryMarshalDispose<GetUnlockedAchievementCountOptionsInternal>(ref getUnlockedAchievementCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyUnlockedAchievementByIndex(CopyUnlockedAchievementByIndexOptions options, out UnlockedAchievement outAchievement)
		{
			CopyUnlockedAchievementByIndexOptionsInternal copyUnlockedAchievementByIndexOptionsInternal = Helper.CopyProperties<CopyUnlockedAchievementByIndexOptionsInternal>(options);
			outAchievement = Helper.GetDefault<UnlockedAchievement>();
			IntPtr zero = IntPtr.Zero;
			Result result = AchievementsInterface.EOS_Achievements_CopyUnlockedAchievementByIndex(base.InnerHandle, ref copyUnlockedAchievementByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyUnlockedAchievementByIndexOptionsInternal>(ref copyUnlockedAchievementByIndexOptionsInternal);
			if (Helper.TryMarshalGet<UnlockedAchievementInternal, UnlockedAchievement>(zero, out outAchievement))
			{
				AchievementsInterface.EOS_Achievements_UnlockedAchievement_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyUnlockedAchievementByAchievementId(CopyUnlockedAchievementByAchievementIdOptions options, out UnlockedAchievement outAchievement)
		{
			CopyUnlockedAchievementByAchievementIdOptionsInternal copyUnlockedAchievementByAchievementIdOptionsInternal = Helper.CopyProperties<CopyUnlockedAchievementByAchievementIdOptionsInternal>(options);
			outAchievement = Helper.GetDefault<UnlockedAchievement>();
			IntPtr zero = IntPtr.Zero;
			Result result = AchievementsInterface.EOS_Achievements_CopyUnlockedAchievementByAchievementId(base.InnerHandle, ref copyUnlockedAchievementByAchievementIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyUnlockedAchievementByAchievementIdOptionsInternal>(ref copyUnlockedAchievementByAchievementIdOptionsInternal);
			if (Helper.TryMarshalGet<UnlockedAchievementInternal, UnlockedAchievement>(zero, out outAchievement))
			{
				AchievementsInterface.EOS_Achievements_UnlockedAchievement_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public ulong AddNotifyAchievementsUnlocked(AddNotifyAchievementsUnlockedOptions options, object clientData, OnAchievementsUnlockedCallback notificationFn)
		{
			AddNotifyAchievementsUnlockedOptionsInternal addNotifyAchievementsUnlockedOptionsInternal = Helper.CopyProperties<AddNotifyAchievementsUnlockedOptionsInternal>(options);
			OnAchievementsUnlockedCallbackInternal onAchievementsUnlockedCallbackInternal = new OnAchievementsUnlockedCallbackInternal(AchievementsInterface.OnAchievementsUnlocked);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onAchievementsUnlockedCallbackInternal, Array.Empty<Delegate>());
			ulong num = AchievementsInterface.EOS_Achievements_AddNotifyAchievementsUnlocked(base.InnerHandle, ref addNotifyAchievementsUnlockedOptionsInternal, zero, onAchievementsUnlockedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyAchievementsUnlockedOptionsInternal>(ref addNotifyAchievementsUnlockedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnAchievementsUnlocked(IntPtr address)
		{
			OnAchievementsUnlockedCallback onAchievementsUnlockedCallback = null;
			OnAchievementsUnlockedCallbackInfo onAchievementsUnlockedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnAchievementsUnlockedCallback, OnAchievementsUnlockedCallbackInfoInternal, OnAchievementsUnlockedCallbackInfo>(address, out onAchievementsUnlockedCallback, out onAchievementsUnlockedCallbackInfo))
			{
				onAchievementsUnlockedCallback(onAchievementsUnlockedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnAchievementsUnlockedV2(IntPtr address)
		{
			OnAchievementsUnlockedCallbackV2 onAchievementsUnlockedCallbackV = null;
			OnAchievementsUnlockedCallbackV2Info onAchievementsUnlockedCallbackV2Info = null;
			if (Helper.TryGetAndRemoveCallback<OnAchievementsUnlockedCallbackV2, OnAchievementsUnlockedCallbackV2InfoInternal, OnAchievementsUnlockedCallbackV2Info>(address, out onAchievementsUnlockedCallbackV, out onAchievementsUnlockedCallbackV2Info))
			{
				onAchievementsUnlockedCallbackV(onAchievementsUnlockedCallbackV2Info);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnUnlockAchievementsComplete(IntPtr address)
		{
			OnUnlockAchievementsCompleteCallback onUnlockAchievementsCompleteCallback = null;
			OnUnlockAchievementsCompleteCallbackInfo onUnlockAchievementsCompleteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUnlockAchievementsCompleteCallback, OnUnlockAchievementsCompleteCallbackInfoInternal, OnUnlockAchievementsCompleteCallbackInfo>(address, out onUnlockAchievementsCompleteCallback, out onUnlockAchievementsCompleteCallbackInfo))
			{
				onUnlockAchievementsCompleteCallback(onUnlockAchievementsCompleteCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryPlayerAchievementsComplete(IntPtr address)
		{
			OnQueryPlayerAchievementsCompleteCallback onQueryPlayerAchievementsCompleteCallback = null;
			OnQueryPlayerAchievementsCompleteCallbackInfo onQueryPlayerAchievementsCompleteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryPlayerAchievementsCompleteCallback, OnQueryPlayerAchievementsCompleteCallbackInfoInternal, OnQueryPlayerAchievementsCompleteCallbackInfo>(address, out onQueryPlayerAchievementsCompleteCallback, out onQueryPlayerAchievementsCompleteCallbackInfo))
			{
				onQueryPlayerAchievementsCompleteCallback(onQueryPlayerAchievementsCompleteCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryDefinitionsComplete(IntPtr address)
		{
			OnQueryDefinitionsCompleteCallback onQueryDefinitionsCompleteCallback = null;
			OnQueryDefinitionsCompleteCallbackInfo onQueryDefinitionsCompleteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryDefinitionsCompleteCallback, OnQueryDefinitionsCompleteCallbackInfoInternal, OnQueryDefinitionsCompleteCallbackInfo>(address, out onQueryDefinitionsCompleteCallback, out onQueryDefinitionsCompleteCallbackInfo))
			{
				onQueryDefinitionsCompleteCallback(onQueryDefinitionsCompleteCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Achievements_UnlockedAchievement_Release(IntPtr achievement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Achievements_Definition_Release(IntPtr achievementDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Achievements_PlayerAchievement_Release(IntPtr achievement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Achievements_DefinitionV2_Release(IntPtr achievementDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Achievements_AddNotifyAchievementsUnlocked(IntPtr handle, ref AddNotifyAchievementsUnlockedOptionsInternal options, IntPtr clientData, OnAchievementsUnlockedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Achievements_CopyUnlockedAchievementByAchievementId(IntPtr handle, ref CopyUnlockedAchievementByAchievementIdOptionsInternal options, ref IntPtr outAchievement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Achievements_CopyUnlockedAchievementByIndex(IntPtr handle, ref CopyUnlockedAchievementByIndexOptionsInternal options, ref IntPtr outAchievement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Achievements_GetUnlockedAchievementCount(IntPtr handle, ref GetUnlockedAchievementCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Achievements_CopyAchievementDefinitionByAchievementId(IntPtr handle, ref CopyAchievementDefinitionByAchievementIdOptionsInternal options, ref IntPtr outDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Achievements_CopyAchievementDefinitionByIndex(IntPtr handle, ref CopyAchievementDefinitionByIndexOptionsInternal options, ref IntPtr outDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Achievements_RemoveNotifyAchievementsUnlocked(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Achievements_AddNotifyAchievementsUnlockedV2(IntPtr handle, ref AddNotifyAchievementsUnlockedV2OptionsInternal options, IntPtr clientData, OnAchievementsUnlockedCallbackV2Internal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Achievements_UnlockAchievements(IntPtr handle, ref UnlockAchievementsOptionsInternal options, IntPtr clientData, OnUnlockAchievementsCompleteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Achievements_CopyPlayerAchievementByAchievementId(IntPtr handle, ref CopyPlayerAchievementByAchievementIdOptionsInternal options, ref IntPtr outAchievement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Achievements_CopyPlayerAchievementByIndex(IntPtr handle, ref CopyPlayerAchievementByIndexOptionsInternal options, ref IntPtr outAchievement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Achievements_GetPlayerAchievementCount(IntPtr handle, ref GetPlayerAchievementCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Achievements_QueryPlayerAchievements(IntPtr handle, ref QueryPlayerAchievementsOptionsInternal options, IntPtr clientData, OnQueryPlayerAchievementsCompleteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Achievements_CopyAchievementDefinitionV2ByAchievementId(IntPtr handle, ref CopyAchievementDefinitionV2ByAchievementIdOptionsInternal options, ref IntPtr outDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Achievements_CopyAchievementDefinitionV2ByIndex(IntPtr handle, ref CopyAchievementDefinitionV2ByIndexOptionsInternal options, ref IntPtr outDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Achievements_GetAchievementDefinitionCount(IntPtr handle, ref GetAchievementDefinitionCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Achievements_QueryDefinitions(IntPtr handle, ref QueryDefinitionsOptionsInternal options, IntPtr clientData, OnQueryDefinitionsCompleteCallbackInternal completionDelegate);

		public const int AddnotifyachievementsunlockedApiLatest = 1;

		public const int CopyunlockedachievementbyachievementidApiLatest = 1;

		public const int CopyunlockedachievementbyindexApiLatest = 1;

		public const int GetunlockedachievementcountApiLatest = 1;

		public const int UnlockedachievementApiLatest = 1;

		public const int CopydefinitionbyachievementidApiLatest = 1;

		public const int CopydefinitionbyindexApiLatest = 1;

		public const int DefinitionApiLatest = 1;

		public const int Addnotifyachievementsunlockedv2ApiLatest = 2;

		public const int UnlockachievementsApiLatest = 1;

		public const int CopyplayerachievementbyachievementidApiLatest = 1;

		public const int CopyplayerachievementbyindexApiLatest = 1;

		public const int GetplayerachievementcountApiLatest = 1;

		public const int PlayerachievementApiLatest = 2;

		public const int AchievementUnlocktimeUndefined = -1;

		public const int QueryplayerachievementsApiLatest = 1;

		public const int Copydefinitionv2ByachievementidApiLatest = 2;

		public const int Copyachievementdefinitionv2ByachievementidApiLatest = 2;

		public const int Copydefinitionv2ByindexApiLatest = 2;

		public const int Copyachievementdefinitionv2ByindexApiLatest = 2;

		public const int GetachievementdefinitioncountApiLatest = 1;

		public const int Definitionv2ApiLatest = 2;

		public const int PlayerstatinfoApiLatest = 1;

		public const int StatthresholdApiLatest = 1;

		public const int StatthresholdsApiLatest = 1;

		public const int QuerydefinitionsApiLatest = 2;
	}
}
