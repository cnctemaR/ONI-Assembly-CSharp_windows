using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	public sealed class LeaderboardsInterface : Handle
	{
		public LeaderboardsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryLeaderboardDefinitions(QueryLeaderboardDefinitionsOptions options, object clientData, OnQueryLeaderboardDefinitionsCompleteCallback completionDelegate)
		{
			QueryLeaderboardDefinitionsOptionsInternal queryLeaderboardDefinitionsOptionsInternal = Helper.CopyProperties<QueryLeaderboardDefinitionsOptionsInternal>(options);
			OnQueryLeaderboardDefinitionsCompleteCallbackInternal onQueryLeaderboardDefinitionsCompleteCallbackInternal = new OnQueryLeaderboardDefinitionsCompleteCallbackInternal(LeaderboardsInterface.OnQueryLeaderboardDefinitionsComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryLeaderboardDefinitionsCompleteCallbackInternal, Array.Empty<Delegate>());
			LeaderboardsInterface.EOS_Leaderboards_QueryLeaderboardDefinitions(base.InnerHandle, ref queryLeaderboardDefinitionsOptionsInternal, zero, onQueryLeaderboardDefinitionsCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryLeaderboardDefinitionsOptionsInternal>(ref queryLeaderboardDefinitionsOptionsInternal);
		}

		public uint GetLeaderboardDefinitionCount(GetLeaderboardDefinitionCountOptions options)
		{
			GetLeaderboardDefinitionCountOptionsInternal getLeaderboardDefinitionCountOptionsInternal = Helper.CopyProperties<GetLeaderboardDefinitionCountOptionsInternal>(options);
			uint num = LeaderboardsInterface.EOS_Leaderboards_GetLeaderboardDefinitionCount(base.InnerHandle, ref getLeaderboardDefinitionCountOptionsInternal);
			Helper.TryMarshalDispose<GetLeaderboardDefinitionCountOptionsInternal>(ref getLeaderboardDefinitionCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyLeaderboardDefinitionByIndex(CopyLeaderboardDefinitionByIndexOptions options, out Definition outLeaderboardDefinition)
		{
			CopyLeaderboardDefinitionByIndexOptionsInternal copyLeaderboardDefinitionByIndexOptionsInternal = Helper.CopyProperties<CopyLeaderboardDefinitionByIndexOptionsInternal>(options);
			outLeaderboardDefinition = Helper.GetDefault<Definition>();
			IntPtr zero = IntPtr.Zero;
			Result result = LeaderboardsInterface.EOS_Leaderboards_CopyLeaderboardDefinitionByIndex(base.InnerHandle, ref copyLeaderboardDefinitionByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLeaderboardDefinitionByIndexOptionsInternal>(ref copyLeaderboardDefinitionByIndexOptionsInternal);
			if (Helper.TryMarshalGet<DefinitionInternal, Definition>(zero, out outLeaderboardDefinition))
			{
				LeaderboardsInterface.EOS_Leaderboards_LeaderboardDefinition_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyLeaderboardDefinitionByLeaderboardId(CopyLeaderboardDefinitionByLeaderboardIdOptions options, out Definition outLeaderboardDefinition)
		{
			CopyLeaderboardDefinitionByLeaderboardIdOptionsInternal copyLeaderboardDefinitionByLeaderboardIdOptionsInternal = Helper.CopyProperties<CopyLeaderboardDefinitionByLeaderboardIdOptionsInternal>(options);
			outLeaderboardDefinition = Helper.GetDefault<Definition>();
			IntPtr zero = IntPtr.Zero;
			Result result = LeaderboardsInterface.EOS_Leaderboards_CopyLeaderboardDefinitionByLeaderboardId(base.InnerHandle, ref copyLeaderboardDefinitionByLeaderboardIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLeaderboardDefinitionByLeaderboardIdOptionsInternal>(ref copyLeaderboardDefinitionByLeaderboardIdOptionsInternal);
			if (Helper.TryMarshalGet<DefinitionInternal, Definition>(zero, out outLeaderboardDefinition))
			{
				LeaderboardsInterface.EOS_Leaderboards_LeaderboardDefinition_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void QueryLeaderboardRanks(QueryLeaderboardRanksOptions options, object clientData, OnQueryLeaderboardRanksCompleteCallback completionDelegate)
		{
			QueryLeaderboardRanksOptionsInternal queryLeaderboardRanksOptionsInternal = Helper.CopyProperties<QueryLeaderboardRanksOptionsInternal>(options);
			OnQueryLeaderboardRanksCompleteCallbackInternal onQueryLeaderboardRanksCompleteCallbackInternal = new OnQueryLeaderboardRanksCompleteCallbackInternal(LeaderboardsInterface.OnQueryLeaderboardRanksComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryLeaderboardRanksCompleteCallbackInternal, Array.Empty<Delegate>());
			LeaderboardsInterface.EOS_Leaderboards_QueryLeaderboardRanks(base.InnerHandle, ref queryLeaderboardRanksOptionsInternal, zero, onQueryLeaderboardRanksCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryLeaderboardRanksOptionsInternal>(ref queryLeaderboardRanksOptionsInternal);
		}

		public uint GetLeaderboardRecordCount(GetLeaderboardRecordCountOptions options)
		{
			GetLeaderboardRecordCountOptionsInternal getLeaderboardRecordCountOptionsInternal = Helper.CopyProperties<GetLeaderboardRecordCountOptionsInternal>(options);
			uint num = LeaderboardsInterface.EOS_Leaderboards_GetLeaderboardRecordCount(base.InnerHandle, ref getLeaderboardRecordCountOptionsInternal);
			Helper.TryMarshalDispose<GetLeaderboardRecordCountOptionsInternal>(ref getLeaderboardRecordCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyLeaderboardRecordByIndex(CopyLeaderboardRecordByIndexOptions options, out LeaderboardRecord outLeaderboardRecord)
		{
			CopyLeaderboardRecordByIndexOptionsInternal copyLeaderboardRecordByIndexOptionsInternal = Helper.CopyProperties<CopyLeaderboardRecordByIndexOptionsInternal>(options);
			outLeaderboardRecord = Helper.GetDefault<LeaderboardRecord>();
			IntPtr zero = IntPtr.Zero;
			Result result = LeaderboardsInterface.EOS_Leaderboards_CopyLeaderboardRecordByIndex(base.InnerHandle, ref copyLeaderboardRecordByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLeaderboardRecordByIndexOptionsInternal>(ref copyLeaderboardRecordByIndexOptionsInternal);
			if (Helper.TryMarshalGet<LeaderboardRecordInternal, LeaderboardRecord>(zero, out outLeaderboardRecord))
			{
				LeaderboardsInterface.EOS_Leaderboards_LeaderboardRecord_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyLeaderboardRecordByUserId(CopyLeaderboardRecordByUserIdOptions options, out LeaderboardRecord outLeaderboardRecord)
		{
			CopyLeaderboardRecordByUserIdOptionsInternal copyLeaderboardRecordByUserIdOptionsInternal = Helper.CopyProperties<CopyLeaderboardRecordByUserIdOptionsInternal>(options);
			outLeaderboardRecord = Helper.GetDefault<LeaderboardRecord>();
			IntPtr zero = IntPtr.Zero;
			Result result = LeaderboardsInterface.EOS_Leaderboards_CopyLeaderboardRecordByUserId(base.InnerHandle, ref copyLeaderboardRecordByUserIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLeaderboardRecordByUserIdOptionsInternal>(ref copyLeaderboardRecordByUserIdOptionsInternal);
			if (Helper.TryMarshalGet<LeaderboardRecordInternal, LeaderboardRecord>(zero, out outLeaderboardRecord))
			{
				LeaderboardsInterface.EOS_Leaderboards_LeaderboardRecord_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void QueryLeaderboardUserScores(QueryLeaderboardUserScoresOptions options, object clientData, OnQueryLeaderboardUserScoresCompleteCallback completionDelegate)
		{
			QueryLeaderboardUserScoresOptionsInternal queryLeaderboardUserScoresOptionsInternal = Helper.CopyProperties<QueryLeaderboardUserScoresOptionsInternal>(options);
			OnQueryLeaderboardUserScoresCompleteCallbackInternal onQueryLeaderboardUserScoresCompleteCallbackInternal = new OnQueryLeaderboardUserScoresCompleteCallbackInternal(LeaderboardsInterface.OnQueryLeaderboardUserScoresComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryLeaderboardUserScoresCompleteCallbackInternal, Array.Empty<Delegate>());
			LeaderboardsInterface.EOS_Leaderboards_QueryLeaderboardUserScores(base.InnerHandle, ref queryLeaderboardUserScoresOptionsInternal, zero, onQueryLeaderboardUserScoresCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryLeaderboardUserScoresOptionsInternal>(ref queryLeaderboardUserScoresOptionsInternal);
		}

		public uint GetLeaderboardUserScoreCount(GetLeaderboardUserScoreCountOptions options)
		{
			GetLeaderboardUserScoreCountOptionsInternal getLeaderboardUserScoreCountOptionsInternal = Helper.CopyProperties<GetLeaderboardUserScoreCountOptionsInternal>(options);
			uint num = LeaderboardsInterface.EOS_Leaderboards_GetLeaderboardUserScoreCount(base.InnerHandle, ref getLeaderboardUserScoreCountOptionsInternal);
			Helper.TryMarshalDispose<GetLeaderboardUserScoreCountOptionsInternal>(ref getLeaderboardUserScoreCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyLeaderboardUserScoreByIndex(CopyLeaderboardUserScoreByIndexOptions options, out LeaderboardUserScore outLeaderboardUserScore)
		{
			CopyLeaderboardUserScoreByIndexOptionsInternal copyLeaderboardUserScoreByIndexOptionsInternal = Helper.CopyProperties<CopyLeaderboardUserScoreByIndexOptionsInternal>(options);
			outLeaderboardUserScore = Helper.GetDefault<LeaderboardUserScore>();
			IntPtr zero = IntPtr.Zero;
			Result result = LeaderboardsInterface.EOS_Leaderboards_CopyLeaderboardUserScoreByIndex(base.InnerHandle, ref copyLeaderboardUserScoreByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLeaderboardUserScoreByIndexOptionsInternal>(ref copyLeaderboardUserScoreByIndexOptionsInternal);
			if (Helper.TryMarshalGet<LeaderboardUserScoreInternal, LeaderboardUserScore>(zero, out outLeaderboardUserScore))
			{
				LeaderboardsInterface.EOS_Leaderboards_LeaderboardUserScore_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyLeaderboardUserScoreByUserId(CopyLeaderboardUserScoreByUserIdOptions options, out LeaderboardUserScore outLeaderboardUserScore)
		{
			CopyLeaderboardUserScoreByUserIdOptionsInternal copyLeaderboardUserScoreByUserIdOptionsInternal = Helper.CopyProperties<CopyLeaderboardUserScoreByUserIdOptionsInternal>(options);
			outLeaderboardUserScore = Helper.GetDefault<LeaderboardUserScore>();
			IntPtr zero = IntPtr.Zero;
			Result result = LeaderboardsInterface.EOS_Leaderboards_CopyLeaderboardUserScoreByUserId(base.InnerHandle, ref copyLeaderboardUserScoreByUserIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLeaderboardUserScoreByUserIdOptionsInternal>(ref copyLeaderboardUserScoreByUserIdOptionsInternal);
			if (Helper.TryMarshalGet<LeaderboardUserScoreInternal, LeaderboardUserScore>(zero, out outLeaderboardUserScore))
			{
				LeaderboardsInterface.EOS_Leaderboards_LeaderboardUserScore_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnQueryLeaderboardUserScoresComplete(IntPtr address)
		{
			OnQueryLeaderboardUserScoresCompleteCallback onQueryLeaderboardUserScoresCompleteCallback = null;
			OnQueryLeaderboardUserScoresCompleteCallbackInfo onQueryLeaderboardUserScoresCompleteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryLeaderboardUserScoresCompleteCallback, OnQueryLeaderboardUserScoresCompleteCallbackInfoInternal, OnQueryLeaderboardUserScoresCompleteCallbackInfo>(address, out onQueryLeaderboardUserScoresCompleteCallback, out onQueryLeaderboardUserScoresCompleteCallbackInfo))
			{
				onQueryLeaderboardUserScoresCompleteCallback(onQueryLeaderboardUserScoresCompleteCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryLeaderboardRanksComplete(IntPtr address)
		{
			OnQueryLeaderboardRanksCompleteCallback onQueryLeaderboardRanksCompleteCallback = null;
			OnQueryLeaderboardRanksCompleteCallbackInfo onQueryLeaderboardRanksCompleteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryLeaderboardRanksCompleteCallback, OnQueryLeaderboardRanksCompleteCallbackInfoInternal, OnQueryLeaderboardRanksCompleteCallbackInfo>(address, out onQueryLeaderboardRanksCompleteCallback, out onQueryLeaderboardRanksCompleteCallbackInfo))
			{
				onQueryLeaderboardRanksCompleteCallback(onQueryLeaderboardRanksCompleteCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryLeaderboardDefinitionsComplete(IntPtr address)
		{
			OnQueryLeaderboardDefinitionsCompleteCallback onQueryLeaderboardDefinitionsCompleteCallback = null;
			OnQueryLeaderboardDefinitionsCompleteCallbackInfo onQueryLeaderboardDefinitionsCompleteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryLeaderboardDefinitionsCompleteCallback, OnQueryLeaderboardDefinitionsCompleteCallbackInfoInternal, OnQueryLeaderboardDefinitionsCompleteCallbackInfo>(address, out onQueryLeaderboardDefinitionsCompleteCallback, out onQueryLeaderboardDefinitionsCompleteCallbackInfo))
			{
				onQueryLeaderboardDefinitionsCompleteCallback(onQueryLeaderboardDefinitionsCompleteCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Leaderboards_LeaderboardDefinition_Release(IntPtr leaderboardDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Leaderboards_LeaderboardRecord_Release(IntPtr leaderboardRecord);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Leaderboards_LeaderboardUserScore_Release(IntPtr leaderboardUserScore);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Leaderboards_Definition_Release(IntPtr leaderboardDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardUserScoreByUserId(IntPtr handle, ref CopyLeaderboardUserScoreByUserIdOptionsInternal options, ref IntPtr outLeaderboardUserScore);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardUserScoreByIndex(IntPtr handle, ref CopyLeaderboardUserScoreByIndexOptionsInternal options, ref IntPtr outLeaderboardUserScore);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Leaderboards_GetLeaderboardUserScoreCount(IntPtr handle, ref GetLeaderboardUserScoreCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Leaderboards_QueryLeaderboardUserScores(IntPtr handle, ref QueryLeaderboardUserScoresOptionsInternal options, IntPtr clientData, OnQueryLeaderboardUserScoresCompleteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardRecordByUserId(IntPtr handle, ref CopyLeaderboardRecordByUserIdOptionsInternal options, ref IntPtr outLeaderboardRecord);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardRecordByIndex(IntPtr handle, ref CopyLeaderboardRecordByIndexOptionsInternal options, ref IntPtr outLeaderboardRecord);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Leaderboards_GetLeaderboardRecordCount(IntPtr handle, ref GetLeaderboardRecordCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Leaderboards_QueryLeaderboardRanks(IntPtr handle, ref QueryLeaderboardRanksOptionsInternal options, IntPtr clientData, OnQueryLeaderboardRanksCompleteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardDefinitionByLeaderboardId(IntPtr handle, ref CopyLeaderboardDefinitionByLeaderboardIdOptionsInternal options, ref IntPtr outLeaderboardDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Leaderboards_CopyLeaderboardDefinitionByIndex(IntPtr handle, ref CopyLeaderboardDefinitionByIndexOptionsInternal options, ref IntPtr outLeaderboardDefinition);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Leaderboards_GetLeaderboardDefinitionCount(IntPtr handle, ref GetLeaderboardDefinitionCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Leaderboards_QueryLeaderboardDefinitions(IntPtr handle, ref QueryLeaderboardDefinitionsOptionsInternal options, IntPtr clientData, OnQueryLeaderboardDefinitionsCompleteCallbackInternal completionDelegate);

		public const int CopyleaderboardrecordbyuseridApiLatest = 2;

		public const int CopyleaderboardrecordbyindexApiLatest = 2;

		public const int GetleaderboardrecordcountApiLatest = 1;

		public const int LeaderboardrecordApiLatest = 2;

		public const int QueryleaderboardranksApiLatest = 1;

		public const int CopyleaderboarduserscorebyuseridApiLatest = 1;

		public const int CopyleaderboarduserscorebyindexApiLatest = 1;

		public const int GetleaderboarduserscorecountApiLatest = 1;

		public const int LeaderboarduserscoreApiLatest = 1;

		public const int QueryleaderboarduserscoresApiLatest = 1;

		public const int UserscoresquerystatinfoApiLatest = 1;

		public const int CopyleaderboarddefinitionbyleaderboardidApiLatest = 1;

		public const int CopyleaderboarddefinitionbyindexApiLatest = 1;

		public const int GetleaderboarddefinitioncountApiLatest = 1;

		public const int DefinitionApiLatest = 1;

		public const int QueryleaderboarddefinitionsApiLatest = 1;

		public const int TimeUndefined = -1;
	}
}
