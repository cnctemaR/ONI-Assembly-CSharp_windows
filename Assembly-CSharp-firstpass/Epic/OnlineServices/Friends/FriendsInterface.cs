using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Friends
{
	public sealed class FriendsInterface : Handle
	{
		public FriendsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryFriends(QueryFriendsOptions options, object clientData, OnQueryFriendsCallback completionDelegate)
		{
			QueryFriendsOptionsInternal queryFriendsOptionsInternal = Helper.CopyProperties<QueryFriendsOptionsInternal>(options);
			OnQueryFriendsCallbackInternal onQueryFriendsCallbackInternal = new OnQueryFriendsCallbackInternal(FriendsInterface.OnQueryFriends);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryFriendsCallbackInternal, Array.Empty<Delegate>());
			FriendsInterface.EOS_Friends_QueryFriends(base.InnerHandle, ref queryFriendsOptionsInternal, zero, onQueryFriendsCallbackInternal);
			Helper.TryMarshalDispose<QueryFriendsOptionsInternal>(ref queryFriendsOptionsInternal);
		}

		public void SendInvite(SendInviteOptions options, object clientData, OnSendInviteCallback completionDelegate)
		{
			SendInviteOptionsInternal sendInviteOptionsInternal = Helper.CopyProperties<SendInviteOptionsInternal>(options);
			OnSendInviteCallbackInternal onSendInviteCallbackInternal = new OnSendInviteCallbackInternal(FriendsInterface.OnSendInvite);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onSendInviteCallbackInternal, Array.Empty<Delegate>());
			FriendsInterface.EOS_Friends_SendInvite(base.InnerHandle, ref sendInviteOptionsInternal, zero, onSendInviteCallbackInternal);
			Helper.TryMarshalDispose<SendInviteOptionsInternal>(ref sendInviteOptionsInternal);
		}

		public void AcceptInvite(AcceptInviteOptions options, object clientData, OnAcceptInviteCallback completionDelegate)
		{
			AcceptInviteOptionsInternal acceptInviteOptionsInternal = Helper.CopyProperties<AcceptInviteOptionsInternal>(options);
			OnAcceptInviteCallbackInternal onAcceptInviteCallbackInternal = new OnAcceptInviteCallbackInternal(FriendsInterface.OnAcceptInvite);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onAcceptInviteCallbackInternal, Array.Empty<Delegate>());
			FriendsInterface.EOS_Friends_AcceptInvite(base.InnerHandle, ref acceptInviteOptionsInternal, zero, onAcceptInviteCallbackInternal);
			Helper.TryMarshalDispose<AcceptInviteOptionsInternal>(ref acceptInviteOptionsInternal);
		}

		public void RejectInvite(RejectInviteOptions options, object clientData, OnRejectInviteCallback completionDelegate)
		{
			RejectInviteOptionsInternal rejectInviteOptionsInternal = Helper.CopyProperties<RejectInviteOptionsInternal>(options);
			OnRejectInviteCallbackInternal onRejectInviteCallbackInternal = new OnRejectInviteCallbackInternal(FriendsInterface.OnRejectInvite);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onRejectInviteCallbackInternal, Array.Empty<Delegate>());
			FriendsInterface.EOS_Friends_RejectInvite(base.InnerHandle, ref rejectInviteOptionsInternal, zero, onRejectInviteCallbackInternal);
			Helper.TryMarshalDispose<RejectInviteOptionsInternal>(ref rejectInviteOptionsInternal);
		}

		public int GetFriendsCount(GetFriendsCountOptions options)
		{
			GetFriendsCountOptionsInternal getFriendsCountOptionsInternal = Helper.CopyProperties<GetFriendsCountOptionsInternal>(options);
			int num = FriendsInterface.EOS_Friends_GetFriendsCount(base.InnerHandle, ref getFriendsCountOptionsInternal);
			Helper.TryMarshalDispose<GetFriendsCountOptionsInternal>(ref getFriendsCountOptionsInternal);
			int @default = Helper.GetDefault<int>();
			Helper.TryMarshalGet<int>(num, out @default);
			return @default;
		}

		public EpicAccountId GetFriendAtIndex(GetFriendAtIndexOptions options)
		{
			GetFriendAtIndexOptionsInternal getFriendAtIndexOptionsInternal = Helper.CopyProperties<GetFriendAtIndexOptionsInternal>(options);
			IntPtr intPtr = FriendsInterface.EOS_Friends_GetFriendAtIndex(base.InnerHandle, ref getFriendAtIndexOptionsInternal);
			Helper.TryMarshalDispose<GetFriendAtIndexOptionsInternal>(ref getFriendAtIndexOptionsInternal);
			EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
			Helper.TryMarshalGet<EpicAccountId>(intPtr, out @default);
			return @default;
		}

		public FriendsStatus GetStatus(GetStatusOptions options)
		{
			GetStatusOptionsInternal getStatusOptionsInternal = Helper.CopyProperties<GetStatusOptionsInternal>(options);
			FriendsStatus friendsStatus = FriendsInterface.EOS_Friends_GetStatus(base.InnerHandle, ref getStatusOptionsInternal);
			Helper.TryMarshalDispose<GetStatusOptionsInternal>(ref getStatusOptionsInternal);
			FriendsStatus @default = Helper.GetDefault<FriendsStatus>();
			Helper.TryMarshalGet<FriendsStatus>(friendsStatus, out @default);
			return @default;
		}

		public ulong AddNotifyFriendsUpdate(AddNotifyFriendsUpdateOptions options, object clientData, OnFriendsUpdateCallback friendsUpdateHandler)
		{
			AddNotifyFriendsUpdateOptionsInternal addNotifyFriendsUpdateOptionsInternal = Helper.CopyProperties<AddNotifyFriendsUpdateOptionsInternal>(options);
			OnFriendsUpdateCallbackInternal onFriendsUpdateCallbackInternal = new OnFriendsUpdateCallbackInternal(FriendsInterface.OnFriendsUpdate);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, friendsUpdateHandler, onFriendsUpdateCallbackInternal, Array.Empty<Delegate>());
			ulong num = FriendsInterface.EOS_Friends_AddNotifyFriendsUpdate(base.InnerHandle, ref addNotifyFriendsUpdateOptionsInternal, zero, onFriendsUpdateCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyFriendsUpdateOptionsInternal>(ref addNotifyFriendsUpdateOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyFriendsUpdate(ulong notificationId)
		{
			Helper.TryRemoveCallbackByNotificationId(notificationId);
			FriendsInterface.EOS_Friends_RemoveNotifyFriendsUpdate(base.InnerHandle, notificationId);
		}

		[MonoPInvokeCallback]
		internal static void OnFriendsUpdate(IntPtr address)
		{
			OnFriendsUpdateCallback onFriendsUpdateCallback = null;
			OnFriendsUpdateInfo onFriendsUpdateInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnFriendsUpdateCallback, OnFriendsUpdateInfoInternal, OnFriendsUpdateInfo>(address, out onFriendsUpdateCallback, out onFriendsUpdateInfo))
			{
				onFriendsUpdateCallback(onFriendsUpdateInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnRejectInvite(IntPtr address)
		{
			OnRejectInviteCallback onRejectInviteCallback = null;
			RejectInviteCallbackInfo rejectInviteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnRejectInviteCallback, RejectInviteCallbackInfoInternal, RejectInviteCallbackInfo>(address, out onRejectInviteCallback, out rejectInviteCallbackInfo))
			{
				onRejectInviteCallback(rejectInviteCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnAcceptInvite(IntPtr address)
		{
			OnAcceptInviteCallback onAcceptInviteCallback = null;
			AcceptInviteCallbackInfo acceptInviteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnAcceptInviteCallback, AcceptInviteCallbackInfoInternal, AcceptInviteCallbackInfo>(address, out onAcceptInviteCallback, out acceptInviteCallbackInfo))
			{
				onAcceptInviteCallback(acceptInviteCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnSendInvite(IntPtr address)
		{
			OnSendInviteCallback onSendInviteCallback = null;
			SendInviteCallbackInfo sendInviteCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnSendInviteCallback, SendInviteCallbackInfoInternal, SendInviteCallbackInfo>(address, out onSendInviteCallback, out sendInviteCallbackInfo))
			{
				onSendInviteCallback(sendInviteCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryFriends(IntPtr address)
		{
			OnQueryFriendsCallback onQueryFriendsCallback = null;
			QueryFriendsCallbackInfo queryFriendsCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryFriendsCallback, QueryFriendsCallbackInfoInternal, QueryFriendsCallbackInfo>(address, out onQueryFriendsCallback, out queryFriendsCallbackInfo))
			{
				onQueryFriendsCallback(queryFriendsCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Friends_RemoveNotifyFriendsUpdate(IntPtr handle, ulong notificationId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Friends_AddNotifyFriendsUpdate(IntPtr handle, ref AddNotifyFriendsUpdateOptionsInternal options, IntPtr clientData, OnFriendsUpdateCallbackInternal friendsUpdateHandler);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern FriendsStatus EOS_Friends_GetStatus(IntPtr handle, ref GetStatusOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Friends_GetFriendAtIndex(IntPtr handle, ref GetFriendAtIndexOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_Friends_GetFriendsCount(IntPtr handle, ref GetFriendsCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Friends_RejectInvite(IntPtr handle, ref RejectInviteOptionsInternal options, IntPtr clientData, OnRejectInviteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Friends_AcceptInvite(IntPtr handle, ref AcceptInviteOptionsInternal options, IntPtr clientData, OnAcceptInviteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Friends_SendInvite(IntPtr handle, ref SendInviteOptionsInternal options, IntPtr clientData, OnSendInviteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Friends_QueryFriends(IntPtr handle, ref QueryFriendsOptionsInternal options, IntPtr clientData, OnQueryFriendsCallbackInternal completionDelegate);

		public const int AddnotifyfriendsupdateApiLatest = 1;

		public const int GetstatusApiLatest = 1;

		public const int GetfriendatindexApiLatest = 1;

		public const int GetfriendscountApiLatest = 1;

		public const int DeletefriendApiLatest = 1;

		public const int RejectinviteApiLatest = 1;

		public const int AcceptinviteApiLatest = 1;

		public const int SendinviteApiLatest = 1;

		public const int QueryfriendsApiLatest = 1;
	}
}
