using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	public sealed class SessionSearch : Handle
	{
		public SessionSearch(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result SetSessionId(SessionSearchSetSessionIdOptions options)
		{
			SessionSearchSetSessionIdOptionsInternal sessionSearchSetSessionIdOptionsInternal = Helper.CopyProperties<SessionSearchSetSessionIdOptionsInternal>(options);
			Result result = SessionSearch.EOS_SessionSearch_SetSessionId(base.InnerHandle, ref sessionSearchSetSessionIdOptionsInternal);
			Helper.TryMarshalDispose<SessionSearchSetSessionIdOptionsInternal>(ref sessionSearchSetSessionIdOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetTargetUserId(SessionSearchSetTargetUserIdOptions options)
		{
			SessionSearchSetTargetUserIdOptionsInternal sessionSearchSetTargetUserIdOptionsInternal = Helper.CopyProperties<SessionSearchSetTargetUserIdOptionsInternal>(options);
			Result result = SessionSearch.EOS_SessionSearch_SetTargetUserId(base.InnerHandle, ref sessionSearchSetTargetUserIdOptionsInternal);
			Helper.TryMarshalDispose<SessionSearchSetTargetUserIdOptionsInternal>(ref sessionSearchSetTargetUserIdOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetParameter(SessionSearchSetParameterOptions options)
		{
			SessionSearchSetParameterOptionsInternal sessionSearchSetParameterOptionsInternal = Helper.CopyProperties<SessionSearchSetParameterOptionsInternal>(options);
			Result result = SessionSearch.EOS_SessionSearch_SetParameter(base.InnerHandle, ref sessionSearchSetParameterOptionsInternal);
			Helper.TryMarshalDispose<SessionSearchSetParameterOptionsInternal>(ref sessionSearchSetParameterOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result RemoveParameter(SessionSearchRemoveParameterOptions options)
		{
			SessionSearchRemoveParameterOptionsInternal sessionSearchRemoveParameterOptionsInternal = Helper.CopyProperties<SessionSearchRemoveParameterOptionsInternal>(options);
			Result result = SessionSearch.EOS_SessionSearch_RemoveParameter(base.InnerHandle, ref sessionSearchRemoveParameterOptionsInternal);
			Helper.TryMarshalDispose<SessionSearchRemoveParameterOptionsInternal>(ref sessionSearchRemoveParameterOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetMaxResults(SessionSearchSetMaxResultsOptions options)
		{
			SessionSearchSetMaxResultsOptionsInternal sessionSearchSetMaxResultsOptionsInternal = Helper.CopyProperties<SessionSearchSetMaxResultsOptionsInternal>(options);
			Result result = SessionSearch.EOS_SessionSearch_SetMaxResults(base.InnerHandle, ref sessionSearchSetMaxResultsOptionsInternal);
			Helper.TryMarshalDispose<SessionSearchSetMaxResultsOptionsInternal>(ref sessionSearchSetMaxResultsOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Find(SessionSearchFindOptions options, object clientData, SessionSearchOnFindCallback completionDelegate)
		{
			SessionSearchFindOptionsInternal sessionSearchFindOptionsInternal = Helper.CopyProperties<SessionSearchFindOptionsInternal>(options);
			SessionSearchOnFindCallbackInternal sessionSearchOnFindCallbackInternal = new SessionSearchOnFindCallbackInternal(SessionSearch.SessionSearchOnFind);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, sessionSearchOnFindCallbackInternal, Array.Empty<Delegate>());
			SessionSearch.EOS_SessionSearch_Find(base.InnerHandle, ref sessionSearchFindOptionsInternal, zero, sessionSearchOnFindCallbackInternal);
			Helper.TryMarshalDispose<SessionSearchFindOptionsInternal>(ref sessionSearchFindOptionsInternal);
		}

		public uint GetSearchResultCount(SessionSearchGetSearchResultCountOptions options)
		{
			SessionSearchGetSearchResultCountOptionsInternal sessionSearchGetSearchResultCountOptionsInternal = Helper.CopyProperties<SessionSearchGetSearchResultCountOptionsInternal>(options);
			uint num = SessionSearch.EOS_SessionSearch_GetSearchResultCount(base.InnerHandle, ref sessionSearchGetSearchResultCountOptionsInternal);
			Helper.TryMarshalDispose<SessionSearchGetSearchResultCountOptionsInternal>(ref sessionSearchGetSearchResultCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopySearchResultByIndex(SessionSearchCopySearchResultByIndexOptions options, out SessionDetails outSessionHandle)
		{
			SessionSearchCopySearchResultByIndexOptionsInternal sessionSearchCopySearchResultByIndexOptionsInternal = Helper.CopyProperties<SessionSearchCopySearchResultByIndexOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<SessionDetails>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionSearch.EOS_SessionSearch_CopySearchResultByIndex(base.InnerHandle, ref sessionSearchCopySearchResultByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<SessionSearchCopySearchResultByIndexOptionsInternal>(ref sessionSearchCopySearchResultByIndexOptionsInternal);
			Helper.TryMarshalGet<SessionDetails>(zero, out outSessionHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			SessionSearch.EOS_SessionSearch_Release(base.InnerHandle);
		}

		[MonoPInvokeCallback]
		internal static void SessionSearchOnFind(IntPtr address)
		{
			SessionSearchOnFindCallback sessionSearchOnFindCallback = null;
			SessionSearchFindCallbackInfo sessionSearchFindCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<SessionSearchOnFindCallback, SessionSearchFindCallbackInfoInternal, SessionSearchFindCallbackInfo>(address, out sessionSearchOnFindCallback, out sessionSearchFindCallbackInfo))
			{
				sessionSearchOnFindCallback(sessionSearchFindCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_SessionSearch_Release(IntPtr sessionSearchHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionSearch_CopySearchResultByIndex(IntPtr handle, ref SessionSearchCopySearchResultByIndexOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_SessionSearch_GetSearchResultCount(IntPtr handle, ref SessionSearchGetSearchResultCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_SessionSearch_Find(IntPtr handle, ref SessionSearchFindOptionsInternal options, IntPtr clientData, SessionSearchOnFindCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionSearch_SetMaxResults(IntPtr handle, ref SessionSearchSetMaxResultsOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionSearch_RemoveParameter(IntPtr handle, ref SessionSearchRemoveParameterOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionSearch_SetParameter(IntPtr handle, ref SessionSearchSetParameterOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionSearch_SetTargetUserId(IntPtr handle, ref SessionSearchSetTargetUserIdOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionSearch_SetSessionId(IntPtr handle, ref SessionSearchSetSessionIdOptionsInternal options);
	}
}
