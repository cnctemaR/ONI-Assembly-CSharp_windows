using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	public sealed class LobbySearch : Handle
	{
		public LobbySearch(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void Find(LobbySearchFindOptions options, object clientData, LobbySearchOnFindCallback completionDelegate)
		{
			LobbySearchFindOptionsInternal lobbySearchFindOptionsInternal = Helper.CopyProperties<LobbySearchFindOptionsInternal>(options);
			LobbySearchOnFindCallbackInternal lobbySearchOnFindCallbackInternal = new LobbySearchOnFindCallbackInternal(LobbySearch.LobbySearchOnFind);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, lobbySearchOnFindCallbackInternal, Array.Empty<Delegate>());
			LobbySearch.EOS_LobbySearch_Find(base.InnerHandle, ref lobbySearchFindOptionsInternal, zero, lobbySearchOnFindCallbackInternal);
			Helper.TryMarshalDispose<LobbySearchFindOptionsInternal>(ref lobbySearchFindOptionsInternal);
		}

		public Result SetLobbyId(LobbySearchSetLobbyIdOptions options)
		{
			LobbySearchSetLobbyIdOptionsInternal lobbySearchSetLobbyIdOptionsInternal = Helper.CopyProperties<LobbySearchSetLobbyIdOptionsInternal>(options);
			Result result = LobbySearch.EOS_LobbySearch_SetLobbyId(base.InnerHandle, ref lobbySearchSetLobbyIdOptionsInternal);
			Helper.TryMarshalDispose<LobbySearchSetLobbyIdOptionsInternal>(ref lobbySearchSetLobbyIdOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetTargetUserId(LobbySearchSetTargetUserIdOptions options)
		{
			LobbySearchSetTargetUserIdOptionsInternal lobbySearchSetTargetUserIdOptionsInternal = Helper.CopyProperties<LobbySearchSetTargetUserIdOptionsInternal>(options);
			Result result = LobbySearch.EOS_LobbySearch_SetTargetUserId(base.InnerHandle, ref lobbySearchSetTargetUserIdOptionsInternal);
			Helper.TryMarshalDispose<LobbySearchSetTargetUserIdOptionsInternal>(ref lobbySearchSetTargetUserIdOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetParameter(LobbySearchSetParameterOptions options)
		{
			LobbySearchSetParameterOptionsInternal lobbySearchSetParameterOptionsInternal = Helper.CopyProperties<LobbySearchSetParameterOptionsInternal>(options);
			Result result = LobbySearch.EOS_LobbySearch_SetParameter(base.InnerHandle, ref lobbySearchSetParameterOptionsInternal);
			Helper.TryMarshalDispose<LobbySearchSetParameterOptionsInternal>(ref lobbySearchSetParameterOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result RemoveParameter(LobbySearchRemoveParameterOptions options)
		{
			LobbySearchRemoveParameterOptionsInternal lobbySearchRemoveParameterOptionsInternal = Helper.CopyProperties<LobbySearchRemoveParameterOptionsInternal>(options);
			Result result = LobbySearch.EOS_LobbySearch_RemoveParameter(base.InnerHandle, ref lobbySearchRemoveParameterOptionsInternal);
			Helper.TryMarshalDispose<LobbySearchRemoveParameterOptionsInternal>(ref lobbySearchRemoveParameterOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetMaxResults(LobbySearchSetMaxResultsOptions options)
		{
			LobbySearchSetMaxResultsOptionsInternal lobbySearchSetMaxResultsOptionsInternal = Helper.CopyProperties<LobbySearchSetMaxResultsOptionsInternal>(options);
			Result result = LobbySearch.EOS_LobbySearch_SetMaxResults(base.InnerHandle, ref lobbySearchSetMaxResultsOptionsInternal);
			Helper.TryMarshalDispose<LobbySearchSetMaxResultsOptionsInternal>(ref lobbySearchSetMaxResultsOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetSearchResultCount(LobbySearchGetSearchResultCountOptions options)
		{
			LobbySearchGetSearchResultCountOptionsInternal lobbySearchGetSearchResultCountOptionsInternal = Helper.CopyProperties<LobbySearchGetSearchResultCountOptionsInternal>(options);
			uint num = LobbySearch.EOS_LobbySearch_GetSearchResultCount(base.InnerHandle, ref lobbySearchGetSearchResultCountOptionsInternal);
			Helper.TryMarshalDispose<LobbySearchGetSearchResultCountOptionsInternal>(ref lobbySearchGetSearchResultCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopySearchResultByIndex(LobbySearchCopySearchResultByIndexOptions options, out LobbyDetails outLobbyDetailsHandle)
		{
			LobbySearchCopySearchResultByIndexOptionsInternal lobbySearchCopySearchResultByIndexOptionsInternal = Helper.CopyProperties<LobbySearchCopySearchResultByIndexOptionsInternal>(options);
			outLobbyDetailsHandle = Helper.GetDefault<LobbyDetails>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbySearch.EOS_LobbySearch_CopySearchResultByIndex(base.InnerHandle, ref lobbySearchCopySearchResultByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<LobbySearchCopySearchResultByIndexOptionsInternal>(ref lobbySearchCopySearchResultByIndexOptionsInternal);
			Helper.TryMarshalGet<LobbyDetails>(zero, out outLobbyDetailsHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			LobbySearch.EOS_LobbySearch_Release(base.InnerHandle);
		}

		[MonoPInvokeCallback]
		internal static void LobbySearchOnFind(IntPtr address)
		{
			LobbySearchOnFindCallback lobbySearchOnFindCallback = null;
			LobbySearchFindCallbackInfo lobbySearchFindCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<LobbySearchOnFindCallback, LobbySearchFindCallbackInfoInternal, LobbySearchFindCallbackInfo>(address, out lobbySearchOnFindCallback, out lobbySearchFindCallbackInfo))
			{
				lobbySearchOnFindCallback(lobbySearchFindCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_LobbySearch_Release(IntPtr lobbySearchHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbySearch_CopySearchResultByIndex(IntPtr handle, ref LobbySearchCopySearchResultByIndexOptionsInternal options, ref IntPtr outLobbyDetailsHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_LobbySearch_GetSearchResultCount(IntPtr handle, ref LobbySearchGetSearchResultCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbySearch_SetMaxResults(IntPtr handle, ref LobbySearchSetMaxResultsOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbySearch_RemoveParameter(IntPtr handle, ref LobbySearchRemoveParameterOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbySearch_SetParameter(IntPtr handle, ref LobbySearchSetParameterOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbySearch_SetTargetUserId(IntPtr handle, ref LobbySearchSetTargetUserIdOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbySearch_SetLobbyId(IntPtr handle, ref LobbySearchSetLobbyIdOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_LobbySearch_Find(IntPtr handle, ref LobbySearchFindOptionsInternal options, IntPtr clientData, LobbySearchOnFindCallbackInternal completionDelegate);
	}
}
