using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Stats
{
	public sealed class StatsInterface : Handle
	{
		public StatsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void IngestStat(IngestStatOptions options, object clientData, OnIngestStatCompleteCallback completionDelegate)
		{
			IngestStatOptionsInternal ingestStatOptionsInternal = Helper.CopyProperties<IngestStatOptionsInternal>(options);
			OnIngestStatCompleteCallbackInternal onIngestStatCompleteCallbackInternal = new OnIngestStatCompleteCallbackInternal(StatsInterface.OnIngestStatComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onIngestStatCompleteCallbackInternal, Array.Empty<Delegate>());
			StatsInterface.EOS_Stats_IngestStat(base.InnerHandle, ref ingestStatOptionsInternal, zero, onIngestStatCompleteCallbackInternal);
			Helper.TryMarshalDispose<IngestStatOptionsInternal>(ref ingestStatOptionsInternal);
		}

		public void QueryStats(QueryStatsOptions options, object clientData, OnQueryStatsCompleteCallback completionDelegate)
		{
			QueryStatsOptionsInternal queryStatsOptionsInternal = Helper.CopyProperties<QueryStatsOptionsInternal>(options);
			OnQueryStatsCompleteCallbackInternal onQueryStatsCompleteCallbackInternal = new OnQueryStatsCompleteCallbackInternal(StatsInterface.OnQueryStatsComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryStatsCompleteCallbackInternal, Array.Empty<Delegate>());
			StatsInterface.EOS_Stats_QueryStats(base.InnerHandle, ref queryStatsOptionsInternal, zero, onQueryStatsCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryStatsOptionsInternal>(ref queryStatsOptionsInternal);
		}

		public uint GetStatsCount(GetStatCountOptions options)
		{
			GetStatCountOptionsInternal getStatCountOptionsInternal = Helper.CopyProperties<GetStatCountOptionsInternal>(options);
			uint num = StatsInterface.EOS_Stats_GetStatsCount(base.InnerHandle, ref getStatCountOptionsInternal);
			Helper.TryMarshalDispose<GetStatCountOptionsInternal>(ref getStatCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyStatByIndex(CopyStatByIndexOptions options, out Stat outStat)
		{
			CopyStatByIndexOptionsInternal copyStatByIndexOptionsInternal = Helper.CopyProperties<CopyStatByIndexOptionsInternal>(options);
			outStat = Helper.GetDefault<Stat>();
			IntPtr zero = IntPtr.Zero;
			Result result = StatsInterface.EOS_Stats_CopyStatByIndex(base.InnerHandle, ref copyStatByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyStatByIndexOptionsInternal>(ref copyStatByIndexOptionsInternal);
			if (Helper.TryMarshalGet<StatInternal, Stat>(zero, out outStat))
			{
				StatsInterface.EOS_Stats_Stat_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyStatByName(CopyStatByNameOptions options, out Stat outStat)
		{
			CopyStatByNameOptionsInternal copyStatByNameOptionsInternal = Helper.CopyProperties<CopyStatByNameOptionsInternal>(options);
			outStat = Helper.GetDefault<Stat>();
			IntPtr zero = IntPtr.Zero;
			Result result = StatsInterface.EOS_Stats_CopyStatByName(base.InnerHandle, ref copyStatByNameOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyStatByNameOptionsInternal>(ref copyStatByNameOptionsInternal);
			if (Helper.TryMarshalGet<StatInternal, Stat>(zero, out outStat))
			{
				StatsInterface.EOS_Stats_Stat_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnQueryStatsComplete(IntPtr address)
		{
			OnQueryStatsCompleteCallback onQueryStatsCompleteCallback = null;
			OnQueryStatsCompleteCallbackInfo onQueryStatsCompleteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryStatsCompleteCallback, OnQueryStatsCompleteCallbackInfoInternal, OnQueryStatsCompleteCallbackInfo>(address, out onQueryStatsCompleteCallback, out onQueryStatsCompleteCallbackInfo))
			{
				onQueryStatsCompleteCallback(onQueryStatsCompleteCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnIngestStatComplete(IntPtr address)
		{
			OnIngestStatCompleteCallback onIngestStatCompleteCallback = null;
			IngestStatCompleteCallbackInfo ingestStatCompleteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnIngestStatCompleteCallback, IngestStatCompleteCallbackInfoInternal, IngestStatCompleteCallbackInfo>(address, out onIngestStatCompleteCallback, out ingestStatCompleteCallbackInfo))
			{
				onIngestStatCompleteCallback(ingestStatCompleteCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Stats_Stat_Release(IntPtr stat);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Stats_CopyStatByName(IntPtr handle, ref CopyStatByNameOptionsInternal options, ref IntPtr outStat);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Stats_CopyStatByIndex(IntPtr handle, ref CopyStatByIndexOptionsInternal options, ref IntPtr outStat);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Stats_GetStatsCount(IntPtr handle, ref GetStatCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Stats_QueryStats(IntPtr handle, ref QueryStatsOptionsInternal options, IntPtr clientData, OnQueryStatsCompleteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Stats_IngestStat(IntPtr handle, ref IngestStatOptionsInternal options, IntPtr clientData, OnIngestStatCompleteCallbackInternal completionDelegate);

		public const int CopystatbynameApiLatest = 1;

		public const int CopystatbyindexApiLatest = 1;

		public const int GetstatcountApiLatest = 1;

		public const int GetstatscountApiLatest = 1;

		public const int StatApiLatest = 1;

		public const int TimeUndefined = -1;

		public const int QuerystatsApiLatest = 2;

		public const int MaxQueryStats = 1000;

		public const int IngeststatApiLatest = 2;

		public const int MaxIngestStats = 3000;

		public const int IngestdataApiLatest = 1;
	}
}
