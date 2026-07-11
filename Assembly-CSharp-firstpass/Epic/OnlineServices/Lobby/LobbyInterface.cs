using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Lobby
{
	public sealed class LobbyInterface : Handle
	{
		public LobbyInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void CreateLobby(CreateLobbyOptions options, object clientData, OnCreateLobbyCallback completionDelegate)
		{
			CreateLobbyOptionsInternal createLobbyOptionsInternal = Helper.CopyProperties<CreateLobbyOptionsInternal>(options);
			OnCreateLobbyCallbackInternal onCreateLobbyCallbackInternal = new OnCreateLobbyCallbackInternal(LobbyInterface.OnCreateLobby);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onCreateLobbyCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_CreateLobby(base.InnerHandle, ref createLobbyOptionsInternal, zero, onCreateLobbyCallbackInternal);
			Helper.TryMarshalDispose<CreateLobbyOptionsInternal>(ref createLobbyOptionsInternal);
		}

		public void DestroyLobby(DestroyLobbyOptions options, object clientData, OnDestroyLobbyCallback completionDelegate)
		{
			DestroyLobbyOptionsInternal destroyLobbyOptionsInternal = Helper.CopyProperties<DestroyLobbyOptionsInternal>(options);
			OnDestroyLobbyCallbackInternal onDestroyLobbyCallbackInternal = new OnDestroyLobbyCallbackInternal(LobbyInterface.OnDestroyLobby);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onDestroyLobbyCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_DestroyLobby(base.InnerHandle, ref destroyLobbyOptionsInternal, zero, onDestroyLobbyCallbackInternal);
			Helper.TryMarshalDispose<DestroyLobbyOptionsInternal>(ref destroyLobbyOptionsInternal);
		}

		public void JoinLobby(JoinLobbyOptions options, object clientData, OnJoinLobbyCallback completionDelegate)
		{
			JoinLobbyOptionsInternal joinLobbyOptionsInternal = Helper.CopyProperties<JoinLobbyOptionsInternal>(options);
			OnJoinLobbyCallbackInternal onJoinLobbyCallbackInternal = new OnJoinLobbyCallbackInternal(LobbyInterface.OnJoinLobby);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onJoinLobbyCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_JoinLobby(base.InnerHandle, ref joinLobbyOptionsInternal, zero, onJoinLobbyCallbackInternal);
			Helper.TryMarshalDispose<JoinLobbyOptionsInternal>(ref joinLobbyOptionsInternal);
		}

		public void LeaveLobby(LeaveLobbyOptions options, object clientData, OnLeaveLobbyCallback completionDelegate)
		{
			LeaveLobbyOptionsInternal leaveLobbyOptionsInternal = Helper.CopyProperties<LeaveLobbyOptionsInternal>(options);
			OnLeaveLobbyCallbackInternal onLeaveLobbyCallbackInternal = new OnLeaveLobbyCallbackInternal(LobbyInterface.OnLeaveLobby);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onLeaveLobbyCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_LeaveLobby(base.InnerHandle, ref leaveLobbyOptionsInternal, zero, onLeaveLobbyCallbackInternal);
			Helper.TryMarshalDispose<LeaveLobbyOptionsInternal>(ref leaveLobbyOptionsInternal);
		}

		public Result UpdateLobbyModification(UpdateLobbyModificationOptions options, out LobbyModification outLobbyModificationHandle)
		{
			UpdateLobbyModificationOptionsInternal updateLobbyModificationOptionsInternal = Helper.CopyProperties<UpdateLobbyModificationOptionsInternal>(options);
			outLobbyModificationHandle = Helper.GetDefault<LobbyModification>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyInterface.EOS_Lobby_UpdateLobbyModification(base.InnerHandle, ref updateLobbyModificationOptionsInternal, ref zero);
			Helper.TryMarshalDispose<UpdateLobbyModificationOptionsInternal>(ref updateLobbyModificationOptionsInternal);
			Helper.TryMarshalGet<LobbyModification>(zero, out outLobbyModificationHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void UpdateLobby(UpdateLobbyOptions options, object clientData, OnUpdateLobbyCallback completionDelegate)
		{
			UpdateLobbyOptionsInternal updateLobbyOptionsInternal = Helper.CopyProperties<UpdateLobbyOptionsInternal>(options);
			OnUpdateLobbyCallbackInternal onUpdateLobbyCallbackInternal = new OnUpdateLobbyCallbackInternal(LobbyInterface.OnUpdateLobby);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onUpdateLobbyCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_UpdateLobby(base.InnerHandle, ref updateLobbyOptionsInternal, zero, onUpdateLobbyCallbackInternal);
			Helper.TryMarshalDispose<UpdateLobbyOptionsInternal>(ref updateLobbyOptionsInternal);
		}

		public void PromoteMember(PromoteMemberOptions options, object clientData, OnPromoteMemberCallback completionDelegate)
		{
			PromoteMemberOptionsInternal promoteMemberOptionsInternal = Helper.CopyProperties<PromoteMemberOptionsInternal>(options);
			OnPromoteMemberCallbackInternal onPromoteMemberCallbackInternal = new OnPromoteMemberCallbackInternal(LobbyInterface.OnPromoteMember);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onPromoteMemberCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_PromoteMember(base.InnerHandle, ref promoteMemberOptionsInternal, zero, onPromoteMemberCallbackInternal);
			Helper.TryMarshalDispose<PromoteMemberOptionsInternal>(ref promoteMemberOptionsInternal);
		}

		public void KickMember(KickMemberOptions options, object clientData, OnKickMemberCallback completionDelegate)
		{
			KickMemberOptionsInternal kickMemberOptionsInternal = Helper.CopyProperties<KickMemberOptionsInternal>(options);
			OnKickMemberCallbackInternal onKickMemberCallbackInternal = new OnKickMemberCallbackInternal(LobbyInterface.OnKickMember);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onKickMemberCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_KickMember(base.InnerHandle, ref kickMemberOptionsInternal, zero, onKickMemberCallbackInternal);
			Helper.TryMarshalDispose<KickMemberOptionsInternal>(ref kickMemberOptionsInternal);
		}

		public ulong AddNotifyLobbyUpdateReceived(AddNotifyLobbyUpdateReceivedOptions options, object clientData, OnLobbyUpdateReceivedCallback notificationFn)
		{
			AddNotifyLobbyUpdateReceivedOptionsInternal addNotifyLobbyUpdateReceivedOptionsInternal = Helper.CopyProperties<AddNotifyLobbyUpdateReceivedOptionsInternal>(options);
			OnLobbyUpdateReceivedCallbackInternal onLobbyUpdateReceivedCallbackInternal = new OnLobbyUpdateReceivedCallbackInternal(LobbyInterface.OnLobbyUpdateReceived);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onLobbyUpdateReceivedCallbackInternal, Array.Empty<Delegate>());
			ulong num = LobbyInterface.EOS_Lobby_AddNotifyLobbyUpdateReceived(base.InnerHandle, ref addNotifyLobbyUpdateReceivedOptionsInternal, zero, onLobbyUpdateReceivedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyLobbyUpdateReceivedOptionsInternal>(ref addNotifyLobbyUpdateReceivedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyLobbyUpdateReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			LobbyInterface.EOS_Lobby_RemoveNotifyLobbyUpdateReceived(base.InnerHandle, inId);
		}

		public ulong AddNotifyLobbyMemberUpdateReceived(AddNotifyLobbyMemberUpdateReceivedOptions options, object clientData, OnLobbyMemberUpdateReceivedCallback notificationFn)
		{
			AddNotifyLobbyMemberUpdateReceivedOptionsInternal addNotifyLobbyMemberUpdateReceivedOptionsInternal = Helper.CopyProperties<AddNotifyLobbyMemberUpdateReceivedOptionsInternal>(options);
			OnLobbyMemberUpdateReceivedCallbackInternal onLobbyMemberUpdateReceivedCallbackInternal = new OnLobbyMemberUpdateReceivedCallbackInternal(LobbyInterface.OnLobbyMemberUpdateReceived);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onLobbyMemberUpdateReceivedCallbackInternal, Array.Empty<Delegate>());
			ulong num = LobbyInterface.EOS_Lobby_AddNotifyLobbyMemberUpdateReceived(base.InnerHandle, ref addNotifyLobbyMemberUpdateReceivedOptionsInternal, zero, onLobbyMemberUpdateReceivedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyLobbyMemberUpdateReceivedOptionsInternal>(ref addNotifyLobbyMemberUpdateReceivedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyLobbyMemberUpdateReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			LobbyInterface.EOS_Lobby_RemoveNotifyLobbyMemberUpdateReceived(base.InnerHandle, inId);
		}

		public ulong AddNotifyLobbyMemberStatusReceived(AddNotifyLobbyMemberStatusReceivedOptions options, object clientData, OnLobbyMemberStatusReceivedCallback notificationFn)
		{
			AddNotifyLobbyMemberStatusReceivedOptionsInternal addNotifyLobbyMemberStatusReceivedOptionsInternal = Helper.CopyProperties<AddNotifyLobbyMemberStatusReceivedOptionsInternal>(options);
			OnLobbyMemberStatusReceivedCallbackInternal onLobbyMemberStatusReceivedCallbackInternal = new OnLobbyMemberStatusReceivedCallbackInternal(LobbyInterface.OnLobbyMemberStatusReceived);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onLobbyMemberStatusReceivedCallbackInternal, Array.Empty<Delegate>());
			ulong num = LobbyInterface.EOS_Lobby_AddNotifyLobbyMemberStatusReceived(base.InnerHandle, ref addNotifyLobbyMemberStatusReceivedOptionsInternal, zero, onLobbyMemberStatusReceivedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyLobbyMemberStatusReceivedOptionsInternal>(ref addNotifyLobbyMemberStatusReceivedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyLobbyMemberStatusReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			LobbyInterface.EOS_Lobby_RemoveNotifyLobbyMemberStatusReceived(base.InnerHandle, inId);
		}

		public void SendInvite(SendInviteOptions options, object clientData, OnSendInviteCallback completionDelegate)
		{
			SendInviteOptionsInternal sendInviteOptionsInternal = Helper.CopyProperties<SendInviteOptionsInternal>(options);
			OnSendInviteCallbackInternal onSendInviteCallbackInternal = new OnSendInviteCallbackInternal(LobbyInterface.OnSendInvite);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onSendInviteCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_SendInvite(base.InnerHandle, ref sendInviteOptionsInternal, zero, onSendInviteCallbackInternal);
			Helper.TryMarshalDispose<SendInviteOptionsInternal>(ref sendInviteOptionsInternal);
		}

		public void RejectInvite(RejectInviteOptions options, object clientData, OnRejectInviteCallback completionDelegate)
		{
			RejectInviteOptionsInternal rejectInviteOptionsInternal = Helper.CopyProperties<RejectInviteOptionsInternal>(options);
			OnRejectInviteCallbackInternal onRejectInviteCallbackInternal = new OnRejectInviteCallbackInternal(LobbyInterface.OnRejectInvite);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onRejectInviteCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_RejectInvite(base.InnerHandle, ref rejectInviteOptionsInternal, zero, onRejectInviteCallbackInternal);
			Helper.TryMarshalDispose<RejectInviteOptionsInternal>(ref rejectInviteOptionsInternal);
		}

		public void QueryInvites(QueryInvitesOptions options, object clientData, OnQueryInvitesCallback completionDelegate)
		{
			QueryInvitesOptionsInternal queryInvitesOptionsInternal = Helper.CopyProperties<QueryInvitesOptionsInternal>(options);
			OnQueryInvitesCallbackInternal onQueryInvitesCallbackInternal = new OnQueryInvitesCallbackInternal(LobbyInterface.OnQueryInvites);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryInvitesCallbackInternal, Array.Empty<Delegate>());
			LobbyInterface.EOS_Lobby_QueryInvites(base.InnerHandle, ref queryInvitesOptionsInternal, zero, onQueryInvitesCallbackInternal);
			Helper.TryMarshalDispose<QueryInvitesOptionsInternal>(ref queryInvitesOptionsInternal);
		}

		public uint GetInviteCount(GetInviteCountOptions options)
		{
			GetInviteCountOptionsInternal getInviteCountOptionsInternal = Helper.CopyProperties<GetInviteCountOptionsInternal>(options);
			uint num = LobbyInterface.EOS_Lobby_GetInviteCount(base.InnerHandle, ref getInviteCountOptionsInternal);
			Helper.TryMarshalDispose<GetInviteCountOptionsInternal>(ref getInviteCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result GetInviteIdByIndex(GetInviteIdByIndexOptions options, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			GetInviteIdByIndexOptionsInternal getInviteIdByIndexOptionsInternal = Helper.CopyProperties<GetInviteIdByIndexOptionsInternal>(options);
			Result result = LobbyInterface.EOS_Lobby_GetInviteIdByIndex(base.InnerHandle, ref getInviteIdByIndexOptionsInternal, outBuffer, ref inOutBufferLength);
			Helper.TryMarshalDispose<GetInviteIdByIndexOptionsInternal>(ref getInviteIdByIndexOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CreateLobbySearch(CreateLobbySearchOptions options, out LobbySearch outLobbySearchHandle)
		{
			CreateLobbySearchOptionsInternal createLobbySearchOptionsInternal = Helper.CopyProperties<CreateLobbySearchOptionsInternal>(options);
			outLobbySearchHandle = Helper.GetDefault<LobbySearch>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyInterface.EOS_Lobby_CreateLobbySearch(base.InnerHandle, ref createLobbySearchOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CreateLobbySearchOptionsInternal>(ref createLobbySearchOptionsInternal);
			Helper.TryMarshalGet<LobbySearch>(zero, out outLobbySearchHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public ulong AddNotifyLobbyInviteReceived(AddNotifyLobbyInviteReceivedOptions options, object clientData, OnLobbyInviteReceivedCallback notificationFn)
		{
			AddNotifyLobbyInviteReceivedOptionsInternal addNotifyLobbyInviteReceivedOptionsInternal = Helper.CopyProperties<AddNotifyLobbyInviteReceivedOptionsInternal>(options);
			OnLobbyInviteReceivedCallbackInternal onLobbyInviteReceivedCallbackInternal = new OnLobbyInviteReceivedCallbackInternal(LobbyInterface.OnLobbyInviteReceived);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onLobbyInviteReceivedCallbackInternal, Array.Empty<Delegate>());
			ulong num = LobbyInterface.EOS_Lobby_AddNotifyLobbyInviteReceived(base.InnerHandle, ref addNotifyLobbyInviteReceivedOptionsInternal, zero, onLobbyInviteReceivedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyLobbyInviteReceivedOptionsInternal>(ref addNotifyLobbyInviteReceivedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyLobbyInviteReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			LobbyInterface.EOS_Lobby_RemoveNotifyLobbyInviteReceived(base.InnerHandle, inId);
		}

		public ulong AddNotifyLobbyInviteAccepted(AddNotifyLobbyInviteAcceptedOptions options, object clientData, OnLobbyInviteAcceptedCallback notificationFn)
		{
			AddNotifyLobbyInviteAcceptedOptionsInternal addNotifyLobbyInviteAcceptedOptionsInternal = Helper.CopyProperties<AddNotifyLobbyInviteAcceptedOptionsInternal>(options);
			OnLobbyInviteAcceptedCallbackInternal onLobbyInviteAcceptedCallbackInternal = new OnLobbyInviteAcceptedCallbackInternal(LobbyInterface.OnLobbyInviteAccepted);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onLobbyInviteAcceptedCallbackInternal, Array.Empty<Delegate>());
			ulong num = LobbyInterface.EOS_Lobby_AddNotifyLobbyInviteAccepted(base.InnerHandle, ref addNotifyLobbyInviteAcceptedOptionsInternal, zero, onLobbyInviteAcceptedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyLobbyInviteAcceptedOptionsInternal>(ref addNotifyLobbyInviteAcceptedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyLobbyInviteAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			LobbyInterface.EOS_Lobby_RemoveNotifyLobbyInviteAccepted(base.InnerHandle, inId);
		}

		public ulong AddNotifyJoinLobbyAccepted(AddNotifyJoinLobbyAcceptedOptions options, object clientData, OnJoinLobbyAcceptedCallback notificationFn)
		{
			AddNotifyJoinLobbyAcceptedOptionsInternal addNotifyJoinLobbyAcceptedOptionsInternal = Helper.CopyProperties<AddNotifyJoinLobbyAcceptedOptionsInternal>(options);
			OnJoinLobbyAcceptedCallbackInternal onJoinLobbyAcceptedCallbackInternal = new OnJoinLobbyAcceptedCallbackInternal(LobbyInterface.OnJoinLobbyAccepted);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onJoinLobbyAcceptedCallbackInternal, Array.Empty<Delegate>());
			ulong num = LobbyInterface.EOS_Lobby_AddNotifyJoinLobbyAccepted(base.InnerHandle, ref addNotifyJoinLobbyAcceptedOptionsInternal, zero, onJoinLobbyAcceptedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyJoinLobbyAcceptedOptionsInternal>(ref addNotifyJoinLobbyAcceptedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyJoinLobbyAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			LobbyInterface.EOS_Lobby_RemoveNotifyJoinLobbyAccepted(base.InnerHandle, inId);
		}

		public Result CopyLobbyDetailsHandleByInviteId(CopyLobbyDetailsHandleByInviteIdOptions options, out LobbyDetails outLobbyDetailsHandle)
		{
			CopyLobbyDetailsHandleByInviteIdOptionsInternal copyLobbyDetailsHandleByInviteIdOptionsInternal = Helper.CopyProperties<CopyLobbyDetailsHandleByInviteIdOptionsInternal>(options);
			outLobbyDetailsHandle = Helper.GetDefault<LobbyDetails>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyInterface.EOS_Lobby_CopyLobbyDetailsHandleByInviteId(base.InnerHandle, ref copyLobbyDetailsHandleByInviteIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLobbyDetailsHandleByInviteIdOptionsInternal>(ref copyLobbyDetailsHandleByInviteIdOptionsInternal);
			Helper.TryMarshalGet<LobbyDetails>(zero, out outLobbyDetailsHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyLobbyDetailsHandleByUiEventId(CopyLobbyDetailsHandleByUiEventIdOptions options, out LobbyDetails outLobbyDetailsHandle)
		{
			CopyLobbyDetailsHandleByUiEventIdOptionsInternal copyLobbyDetailsHandleByUiEventIdOptionsInternal = Helper.CopyProperties<CopyLobbyDetailsHandleByUiEventIdOptionsInternal>(options);
			outLobbyDetailsHandle = Helper.GetDefault<LobbyDetails>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyInterface.EOS_Lobby_CopyLobbyDetailsHandleByUiEventId(base.InnerHandle, ref copyLobbyDetailsHandleByUiEventIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLobbyDetailsHandleByUiEventIdOptionsInternal>(ref copyLobbyDetailsHandleByUiEventIdOptionsInternal);
			Helper.TryMarshalGet<LobbyDetails>(zero, out outLobbyDetailsHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyLobbyDetailsHandle(CopyLobbyDetailsHandleOptions options, out LobbyDetails outLobbyDetailsHandle)
		{
			CopyLobbyDetailsHandleOptionsInternal copyLobbyDetailsHandleOptionsInternal = Helper.CopyProperties<CopyLobbyDetailsHandleOptionsInternal>(options);
			outLobbyDetailsHandle = Helper.GetDefault<LobbyDetails>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyInterface.EOS_Lobby_CopyLobbyDetailsHandle(base.InnerHandle, ref copyLobbyDetailsHandleOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyLobbyDetailsHandleOptionsInternal>(ref copyLobbyDetailsHandleOptionsInternal);
			Helper.TryMarshalGet<LobbyDetails>(zero, out outLobbyDetailsHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnJoinLobbyAccepted(IntPtr address)
		{
			OnJoinLobbyAcceptedCallback onJoinLobbyAcceptedCallback = null;
			JoinLobbyAcceptedCallbackInfo joinLobbyAcceptedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinLobbyAcceptedCallback, JoinLobbyAcceptedCallbackInfoInternal, JoinLobbyAcceptedCallbackInfo>(address, out onJoinLobbyAcceptedCallback, out joinLobbyAcceptedCallbackInfo))
			{
				onJoinLobbyAcceptedCallback(joinLobbyAcceptedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLobbyInviteAccepted(IntPtr address)
		{
			OnLobbyInviteAcceptedCallback onLobbyInviteAcceptedCallback = null;
			LobbyInviteAcceptedCallbackInfo lobbyInviteAcceptedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyInviteAcceptedCallback, LobbyInviteAcceptedCallbackInfoInternal, LobbyInviteAcceptedCallbackInfo>(address, out onLobbyInviteAcceptedCallback, out lobbyInviteAcceptedCallbackInfo))
			{
				onLobbyInviteAcceptedCallback(lobbyInviteAcceptedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLobbyInviteReceived(IntPtr address)
		{
			OnLobbyInviteReceivedCallback onLobbyInviteReceivedCallback = null;
			LobbyInviteReceivedCallbackInfo lobbyInviteReceivedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyInviteReceivedCallback, LobbyInviteReceivedCallbackInfoInternal, LobbyInviteReceivedCallbackInfo>(address, out onLobbyInviteReceivedCallback, out lobbyInviteReceivedCallbackInfo))
			{
				onLobbyInviteReceivedCallback(lobbyInviteReceivedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryInvites(IntPtr address)
		{
			OnQueryInvitesCallback onQueryInvitesCallback = null;
			QueryInvitesCallbackInfo queryInvitesCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryInvitesCallback, QueryInvitesCallbackInfoInternal, QueryInvitesCallbackInfo>(address, out onQueryInvitesCallback, out queryInvitesCallbackInfo))
			{
				onQueryInvitesCallback(queryInvitesCallbackInfo);
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
		internal static void OnLobbyMemberStatusReceived(IntPtr address)
		{
			OnLobbyMemberStatusReceivedCallback onLobbyMemberStatusReceivedCallback = null;
			LobbyMemberStatusReceivedCallbackInfo lobbyMemberStatusReceivedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyMemberStatusReceivedCallback, LobbyMemberStatusReceivedCallbackInfoInternal, LobbyMemberStatusReceivedCallbackInfo>(address, out onLobbyMemberStatusReceivedCallback, out lobbyMemberStatusReceivedCallbackInfo))
			{
				onLobbyMemberStatusReceivedCallback(lobbyMemberStatusReceivedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLobbyMemberUpdateReceived(IntPtr address)
		{
			OnLobbyMemberUpdateReceivedCallback onLobbyMemberUpdateReceivedCallback = null;
			LobbyMemberUpdateReceivedCallbackInfo lobbyMemberUpdateReceivedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyMemberUpdateReceivedCallback, LobbyMemberUpdateReceivedCallbackInfoInternal, LobbyMemberUpdateReceivedCallbackInfo>(address, out onLobbyMemberUpdateReceivedCallback, out lobbyMemberUpdateReceivedCallbackInfo))
			{
				onLobbyMemberUpdateReceivedCallback(lobbyMemberUpdateReceivedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLobbyUpdateReceived(IntPtr address)
		{
			OnLobbyUpdateReceivedCallback onLobbyUpdateReceivedCallback = null;
			LobbyUpdateReceivedCallbackInfo lobbyUpdateReceivedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLobbyUpdateReceivedCallback, LobbyUpdateReceivedCallbackInfoInternal, LobbyUpdateReceivedCallbackInfo>(address, out onLobbyUpdateReceivedCallback, out lobbyUpdateReceivedCallbackInfo))
			{
				onLobbyUpdateReceivedCallback(lobbyUpdateReceivedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnKickMember(IntPtr address)
		{
			OnKickMemberCallback onKickMemberCallback = null;
			KickMemberCallbackInfo kickMemberCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnKickMemberCallback, KickMemberCallbackInfoInternal, KickMemberCallbackInfo>(address, out onKickMemberCallback, out kickMemberCallbackInfo))
			{
				onKickMemberCallback(kickMemberCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnPromoteMember(IntPtr address)
		{
			OnPromoteMemberCallback onPromoteMemberCallback = null;
			PromoteMemberCallbackInfo promoteMemberCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnPromoteMemberCallback, PromoteMemberCallbackInfoInternal, PromoteMemberCallbackInfo>(address, out onPromoteMemberCallback, out promoteMemberCallbackInfo))
			{
				onPromoteMemberCallback(promoteMemberCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnUpdateLobby(IntPtr address)
		{
			OnUpdateLobbyCallback onUpdateLobbyCallback = null;
			UpdateLobbyCallbackInfo updateLobbyCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUpdateLobbyCallback, UpdateLobbyCallbackInfoInternal, UpdateLobbyCallbackInfo>(address, out onUpdateLobbyCallback, out updateLobbyCallbackInfo))
			{
				onUpdateLobbyCallback(updateLobbyCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLeaveLobby(IntPtr address)
		{
			OnLeaveLobbyCallback onLeaveLobbyCallback = null;
			LeaveLobbyCallbackInfo leaveLobbyCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLeaveLobbyCallback, LeaveLobbyCallbackInfoInternal, LeaveLobbyCallbackInfo>(address, out onLeaveLobbyCallback, out leaveLobbyCallbackInfo))
			{
				onLeaveLobbyCallback(leaveLobbyCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnJoinLobby(IntPtr address)
		{
			OnJoinLobbyCallback onJoinLobbyCallback = null;
			JoinLobbyCallbackInfo joinLobbyCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinLobbyCallback, JoinLobbyCallbackInfoInternal, JoinLobbyCallbackInfo>(address, out onJoinLobbyCallback, out joinLobbyCallbackInfo))
			{
				onJoinLobbyCallback(joinLobbyCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDestroyLobby(IntPtr address)
		{
			OnDestroyLobbyCallback onDestroyLobbyCallback = null;
			DestroyLobbyCallbackInfo destroyLobbyCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDestroyLobbyCallback, DestroyLobbyCallbackInfoInternal, DestroyLobbyCallbackInfo>(address, out onDestroyLobbyCallback, out destroyLobbyCallbackInfo))
			{
				onDestroyLobbyCallback(destroyLobbyCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnCreateLobby(IntPtr address)
		{
			OnCreateLobbyCallback onCreateLobbyCallback = null;
			CreateLobbyCallbackInfo createLobbyCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnCreateLobbyCallback, CreateLobbyCallbackInfoInternal, CreateLobbyCallbackInfo>(address, out onCreateLobbyCallback, out createLobbyCallbackInfo))
			{
				onCreateLobbyCallback(createLobbyCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_Attribute_Release(IntPtr lobbyAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Lobby_CopyLobbyDetailsHandle(IntPtr handle, ref CopyLobbyDetailsHandleOptionsInternal options, ref IntPtr outLobbyDetailsHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Lobby_CopyLobbyDetailsHandleByUiEventId(IntPtr handle, ref CopyLobbyDetailsHandleByUiEventIdOptionsInternal options, ref IntPtr outLobbyDetailsHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Lobby_CopyLobbyDetailsHandleByInviteId(IntPtr handle, ref CopyLobbyDetailsHandleByInviteIdOptionsInternal options, ref IntPtr outLobbyDetailsHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyJoinLobbyAccepted(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyJoinLobbyAccepted(IntPtr handle, ref AddNotifyJoinLobbyAcceptedOptionsInternal options, IntPtr clientData, OnJoinLobbyAcceptedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyInviteAccepted(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyInviteAccepted(IntPtr handle, ref AddNotifyLobbyInviteAcceptedOptionsInternal options, IntPtr clientData, OnLobbyInviteAcceptedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyInviteReceived(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyInviteReceived(IntPtr handle, ref AddNotifyLobbyInviteReceivedOptionsInternal options, IntPtr clientData, OnLobbyInviteReceivedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Lobby_CreateLobbySearch(IntPtr handle, ref CreateLobbySearchOptionsInternal options, ref IntPtr outLobbySearchHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Lobby_GetInviteIdByIndex(IntPtr handle, ref GetInviteIdByIndexOptionsInternal options, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Lobby_GetInviteCount(IntPtr handle, ref GetInviteCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_QueryInvites(IntPtr handle, ref QueryInvitesOptionsInternal options, IntPtr clientData, OnQueryInvitesCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_RejectInvite(IntPtr handle, ref RejectInviteOptionsInternal options, IntPtr clientData, OnRejectInviteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_SendInvite(IntPtr handle, ref SendInviteOptionsInternal options, IntPtr clientData, OnSendInviteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyMemberStatusReceived(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyMemberStatusReceived(IntPtr handle, ref AddNotifyLobbyMemberStatusReceivedOptionsInternal options, IntPtr clientData, OnLobbyMemberStatusReceivedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyMemberUpdateReceived(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyMemberUpdateReceived(IntPtr handle, ref AddNotifyLobbyMemberUpdateReceivedOptionsInternal options, IntPtr clientData, OnLobbyMemberUpdateReceivedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_RemoveNotifyLobbyUpdateReceived(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Lobby_AddNotifyLobbyUpdateReceived(IntPtr handle, ref AddNotifyLobbyUpdateReceivedOptionsInternal options, IntPtr clientData, OnLobbyUpdateReceivedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_KickMember(IntPtr handle, ref KickMemberOptionsInternal options, IntPtr clientData, OnKickMemberCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_PromoteMember(IntPtr handle, ref PromoteMemberOptionsInternal options, IntPtr clientData, OnPromoteMemberCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_UpdateLobby(IntPtr handle, ref UpdateLobbyOptionsInternal options, IntPtr clientData, OnUpdateLobbyCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Lobby_UpdateLobbyModification(IntPtr handle, ref UpdateLobbyModificationOptionsInternal options, ref IntPtr outLobbyModificationHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_LeaveLobby(IntPtr handle, ref LeaveLobbyOptionsInternal options, IntPtr clientData, OnLeaveLobbyCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_JoinLobby(IntPtr handle, ref JoinLobbyOptionsInternal options, IntPtr clientData, OnJoinLobbyCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_DestroyLobby(IntPtr handle, ref DestroyLobbyOptionsInternal options, IntPtr clientData, OnDestroyLobbyCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_CreateLobby(IntPtr handle, ref CreateLobbyOptionsInternal options, IntPtr clientData, OnCreateLobbyCallbackInternal completionDelegate);

		public const int LobbysearchCopysearchresultbyindexApiLatest = 1;

		public const int LobbysearchGetsearchresultcountApiLatest = 1;

		public const int LobbysearchSetmaxresultsApiLatest = 1;

		public const int LobbysearchRemoveparameterApiLatest = 1;

		public const int LobbysearchSetparameterApiLatest = 1;

		public const int LobbysearchSettargetuseridApiLatest = 1;

		public const int LobbysearchSetlobbyidApiLatest = 1;

		public const int LobbysearchFindApiLatest = 1;

		public const int LobbydetailsGetmemberbyindexApiLatest = 1;

		public const int LobbydetailsGetmembercountApiLatest = 1;

		public const int LobbydetailsCopymemberattributebykeyApiLatest = 1;

		public const int LobbydetailsCopymemberattributebyindexApiLatest = 1;

		public const int LobbydetailsGetmemberattributecountApiLatest = 1;

		public const int LobbydetailsCopyattributebykeyApiLatest = 1;

		public const int LobbydetailsCopyattributebyindexApiLatest = 1;

		public const int LobbydetailsGetattributecountApiLatest = 1;

		public const int LobbydetailsCopyinfoApiLatest = 1;

		public const int LobbydetailsGetlobbyownerApiLatest = 1;

		public const int LobbymodificationRemovememberattributeApiLatest = 1;

		public const int LobbymodificationAddmemberattributeApiLatest = 1;

		public const int LobbymodificationRemoveattributeApiLatest = 1;

		public const int LobbymodificationAddattributeApiLatest = 1;

		public const int LobbymodificationSetmaxmembersApiLatest = 1;

		public const int LobbymodificationSetpermissionlevelApiLatest = 1;

		public const int AttributeApiLatest = 1;

		public const int AttributedataApiLatest = 1;

		public const string SearchMinslotsavailable = "minslotsavailable";

		public const string SearchMincurrentmembers = "mincurrentmembers";

		public const int CopylobbydetailshandleApiLatest = 1;

		public const int GetinviteidbyindexApiLatest = 1;

		public const int GetinvitecountApiLatest = 1;

		public const int QueryinvitesApiLatest = 1;

		public const int RejectinviteApiLatest = 1;

		public const int SendinviteApiLatest = 1;

		public const int CreatelobbysearchApiLatest = 1;

		public const int CopylobbydetailshandlebyuieventidApiLatest = 1;

		public const int CopylobbydetailshandlebyinviteidApiLatest = 1;

		public const int AddnotifyjoinlobbyacceptedApiLatest = 1;

		public const int AddnotifylobbyinviteacceptedApiLatest = 1;

		public const int AddnotifylobbyinvitereceivedApiLatest = 1;

		public const int InviteidMaxLength = 64;

		public const int AddnotifylobbymemberstatusreceivedApiLatest = 1;

		public const int AddnotifylobbymemberupdatereceivedApiLatest = 1;

		public const int AddnotifylobbyupdatereceivedApiLatest = 1;

		public const int KickmemberApiLatest = 1;

		public const int PromotememberApiLatest = 1;

		public const int UpdatelobbyApiLatest = 1;

		public const int UpdatelobbymodificationApiLatest = 1;

		public const int LeavelobbyApiLatest = 1;

		public const int JoinlobbyApiLatest = 2;

		public const int DestroylobbyApiLatest = 1;

		public const int CreatelobbyApiLatest = 2;

		public const int LobbydetailsInfoApiLatest = 1;

		public const int LobbymodificationMaxAttributeLength = 64;

		public const int LobbymodificationMaxAttributes = 64;

		public const int MaxSearchResults = 200;

		public const int MaxLobbyMembers = 64;

		public const int MaxLobbies = 4;
	}
}
