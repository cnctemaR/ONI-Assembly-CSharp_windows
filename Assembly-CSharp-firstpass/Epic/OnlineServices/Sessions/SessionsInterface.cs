using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Sessions
{
	public sealed class SessionsInterface : Handle
	{
		public SessionsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result CreateSessionModification(CreateSessionModificationOptions options, out SessionModification outSessionModificationHandle)
		{
			CreateSessionModificationOptionsInternal createSessionModificationOptionsInternal = Helper.CopyProperties<CreateSessionModificationOptionsInternal>(options);
			outSessionModificationHandle = Helper.GetDefault<SessionModification>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionsInterface.EOS_Sessions_CreateSessionModification(base.InnerHandle, ref createSessionModificationOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CreateSessionModificationOptionsInternal>(ref createSessionModificationOptionsInternal);
			Helper.TryMarshalGet<SessionModification>(zero, out outSessionModificationHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result UpdateSessionModification(UpdateSessionModificationOptions options, out SessionModification outSessionModificationHandle)
		{
			UpdateSessionModificationOptionsInternal updateSessionModificationOptionsInternal = Helper.CopyProperties<UpdateSessionModificationOptionsInternal>(options);
			outSessionModificationHandle = Helper.GetDefault<SessionModification>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionsInterface.EOS_Sessions_UpdateSessionModification(base.InnerHandle, ref updateSessionModificationOptionsInternal, ref zero);
			Helper.TryMarshalDispose<UpdateSessionModificationOptionsInternal>(ref updateSessionModificationOptionsInternal);
			Helper.TryMarshalGet<SessionModification>(zero, out outSessionModificationHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void UpdateSession(UpdateSessionOptions options, object clientData, OnUpdateSessionCallback completionDelegate)
		{
			UpdateSessionOptionsInternal updateSessionOptionsInternal = Helper.CopyProperties<UpdateSessionOptionsInternal>(options);
			OnUpdateSessionCallbackInternal onUpdateSessionCallbackInternal = new OnUpdateSessionCallbackInternal(SessionsInterface.OnUpdateSession);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onUpdateSessionCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_UpdateSession(base.InnerHandle, ref updateSessionOptionsInternal, zero, onUpdateSessionCallbackInternal);
			Helper.TryMarshalDispose<UpdateSessionOptionsInternal>(ref updateSessionOptionsInternal);
		}

		public void DestroySession(DestroySessionOptions options, object clientData, OnDestroySessionCallback completionDelegate)
		{
			DestroySessionOptionsInternal destroySessionOptionsInternal = Helper.CopyProperties<DestroySessionOptionsInternal>(options);
			OnDestroySessionCallbackInternal onDestroySessionCallbackInternal = new OnDestroySessionCallbackInternal(SessionsInterface.OnDestroySession);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onDestroySessionCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_DestroySession(base.InnerHandle, ref destroySessionOptionsInternal, zero, onDestroySessionCallbackInternal);
			Helper.TryMarshalDispose<DestroySessionOptionsInternal>(ref destroySessionOptionsInternal);
		}

		public void JoinSession(JoinSessionOptions options, object clientData, OnJoinSessionCallback completionDelegate)
		{
			JoinSessionOptionsInternal joinSessionOptionsInternal = Helper.CopyProperties<JoinSessionOptionsInternal>(options);
			OnJoinSessionCallbackInternal onJoinSessionCallbackInternal = new OnJoinSessionCallbackInternal(SessionsInterface.OnJoinSession);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onJoinSessionCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_JoinSession(base.InnerHandle, ref joinSessionOptionsInternal, zero, onJoinSessionCallbackInternal);
			Helper.TryMarshalDispose<JoinSessionOptionsInternal>(ref joinSessionOptionsInternal);
		}

		public void StartSession(StartSessionOptions options, object clientData, OnStartSessionCallback completionDelegate)
		{
			StartSessionOptionsInternal startSessionOptionsInternal = Helper.CopyProperties<StartSessionOptionsInternal>(options);
			OnStartSessionCallbackInternal onStartSessionCallbackInternal = new OnStartSessionCallbackInternal(SessionsInterface.OnStartSession);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onStartSessionCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_StartSession(base.InnerHandle, ref startSessionOptionsInternal, zero, onStartSessionCallbackInternal);
			Helper.TryMarshalDispose<StartSessionOptionsInternal>(ref startSessionOptionsInternal);
		}

		public void EndSession(EndSessionOptions options, object clientData, OnEndSessionCallback completionDelegate)
		{
			EndSessionOptionsInternal endSessionOptionsInternal = Helper.CopyProperties<EndSessionOptionsInternal>(options);
			OnEndSessionCallbackInternal onEndSessionCallbackInternal = new OnEndSessionCallbackInternal(SessionsInterface.OnEndSession);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onEndSessionCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_EndSession(base.InnerHandle, ref endSessionOptionsInternal, zero, onEndSessionCallbackInternal);
			Helper.TryMarshalDispose<EndSessionOptionsInternal>(ref endSessionOptionsInternal);
		}

		public void RegisterPlayers(RegisterPlayersOptions options, object clientData, OnRegisterPlayersCallback completionDelegate)
		{
			RegisterPlayersOptionsInternal registerPlayersOptionsInternal = Helper.CopyProperties<RegisterPlayersOptionsInternal>(options);
			OnRegisterPlayersCallbackInternal onRegisterPlayersCallbackInternal = new OnRegisterPlayersCallbackInternal(SessionsInterface.OnRegisterPlayers);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onRegisterPlayersCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_RegisterPlayers(base.InnerHandle, ref registerPlayersOptionsInternal, zero, onRegisterPlayersCallbackInternal);
			Helper.TryMarshalDispose<RegisterPlayersOptionsInternal>(ref registerPlayersOptionsInternal);
		}

		public void UnregisterPlayers(UnregisterPlayersOptions options, object clientData, OnUnregisterPlayersCallback completionDelegate)
		{
			UnregisterPlayersOptionsInternal unregisterPlayersOptionsInternal = Helper.CopyProperties<UnregisterPlayersOptionsInternal>(options);
			OnUnregisterPlayersCallbackInternal onUnregisterPlayersCallbackInternal = new OnUnregisterPlayersCallbackInternal(SessionsInterface.OnUnregisterPlayers);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onUnregisterPlayersCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_UnregisterPlayers(base.InnerHandle, ref unregisterPlayersOptionsInternal, zero, onUnregisterPlayersCallbackInternal);
			Helper.TryMarshalDispose<UnregisterPlayersOptionsInternal>(ref unregisterPlayersOptionsInternal);
		}

		public void SendInvite(SendInviteOptions options, object clientData, OnSendInviteCallback completionDelegate)
		{
			SendInviteOptionsInternal sendInviteOptionsInternal = Helper.CopyProperties<SendInviteOptionsInternal>(options);
			OnSendInviteCallbackInternal onSendInviteCallbackInternal = new OnSendInviteCallbackInternal(SessionsInterface.OnSendInvite);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onSendInviteCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_SendInvite(base.InnerHandle, ref sendInviteOptionsInternal, zero, onSendInviteCallbackInternal);
			Helper.TryMarshalDispose<SendInviteOptionsInternal>(ref sendInviteOptionsInternal);
		}

		public void RejectInvite(RejectInviteOptions options, object clientData, OnRejectInviteCallback completionDelegate)
		{
			RejectInviteOptionsInternal rejectInviteOptionsInternal = Helper.CopyProperties<RejectInviteOptionsInternal>(options);
			OnRejectInviteCallbackInternal onRejectInviteCallbackInternal = new OnRejectInviteCallbackInternal(SessionsInterface.OnRejectInvite);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onRejectInviteCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_RejectInvite(base.InnerHandle, ref rejectInviteOptionsInternal, zero, onRejectInviteCallbackInternal);
			Helper.TryMarshalDispose<RejectInviteOptionsInternal>(ref rejectInviteOptionsInternal);
		}

		public void QueryInvites(QueryInvitesOptions options, object clientData, OnQueryInvitesCallback completionDelegate)
		{
			QueryInvitesOptionsInternal queryInvitesOptionsInternal = Helper.CopyProperties<QueryInvitesOptionsInternal>(options);
			OnQueryInvitesCallbackInternal onQueryInvitesCallbackInternal = new OnQueryInvitesCallbackInternal(SessionsInterface.OnQueryInvites);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryInvitesCallbackInternal, Array.Empty<Delegate>());
			SessionsInterface.EOS_Sessions_QueryInvites(base.InnerHandle, ref queryInvitesOptionsInternal, zero, onQueryInvitesCallbackInternal);
			Helper.TryMarshalDispose<QueryInvitesOptionsInternal>(ref queryInvitesOptionsInternal);
		}

		public uint GetInviteCount(GetInviteCountOptions options)
		{
			GetInviteCountOptionsInternal getInviteCountOptionsInternal = Helper.CopyProperties<GetInviteCountOptionsInternal>(options);
			uint num = SessionsInterface.EOS_Sessions_GetInviteCount(base.InnerHandle, ref getInviteCountOptionsInternal);
			Helper.TryMarshalDispose<GetInviteCountOptionsInternal>(ref getInviteCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result GetInviteIdByIndex(GetInviteIdByIndexOptions options, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			GetInviteIdByIndexOptionsInternal getInviteIdByIndexOptionsInternal = Helper.CopyProperties<GetInviteIdByIndexOptionsInternal>(options);
			Result result = SessionsInterface.EOS_Sessions_GetInviteIdByIndex(base.InnerHandle, ref getInviteIdByIndexOptionsInternal, outBuffer, ref inOutBufferLength);
			Helper.TryMarshalDispose<GetInviteIdByIndexOptionsInternal>(ref getInviteIdByIndexOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CreateSessionSearch(CreateSessionSearchOptions options, out SessionSearch outSessionSearchHandle)
		{
			CreateSessionSearchOptionsInternal createSessionSearchOptionsInternal = Helper.CopyProperties<CreateSessionSearchOptionsInternal>(options);
			outSessionSearchHandle = Helper.GetDefault<SessionSearch>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionsInterface.EOS_Sessions_CreateSessionSearch(base.InnerHandle, ref createSessionSearchOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CreateSessionSearchOptionsInternal>(ref createSessionSearchOptionsInternal);
			Helper.TryMarshalGet<SessionSearch>(zero, out outSessionSearchHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyActiveSessionHandle(CopyActiveSessionHandleOptions options, out ActiveSession outSessionHandle)
		{
			CopyActiveSessionHandleOptionsInternal copyActiveSessionHandleOptionsInternal = Helper.CopyProperties<CopyActiveSessionHandleOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<ActiveSession>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionsInterface.EOS_Sessions_CopyActiveSessionHandle(base.InnerHandle, ref copyActiveSessionHandleOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyActiveSessionHandleOptionsInternal>(ref copyActiveSessionHandleOptionsInternal);
			Helper.TryMarshalGet<ActiveSession>(zero, out outSessionHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public ulong AddNotifySessionInviteReceived(AddNotifySessionInviteReceivedOptions options, object clientData, OnSessionInviteReceivedCallback notificationFn)
		{
			AddNotifySessionInviteReceivedOptionsInternal addNotifySessionInviteReceivedOptionsInternal = Helper.CopyProperties<AddNotifySessionInviteReceivedOptionsInternal>(options);
			OnSessionInviteReceivedCallbackInternal onSessionInviteReceivedCallbackInternal = new OnSessionInviteReceivedCallbackInternal(SessionsInterface.OnSessionInviteReceived);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onSessionInviteReceivedCallbackInternal, Array.Empty<Delegate>());
			ulong num = SessionsInterface.EOS_Sessions_AddNotifySessionInviteReceived(base.InnerHandle, ref addNotifySessionInviteReceivedOptionsInternal, zero, onSessionInviteReceivedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifySessionInviteReceivedOptionsInternal>(ref addNotifySessionInviteReceivedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifySessionInviteReceived(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			SessionsInterface.EOS_Sessions_RemoveNotifySessionInviteReceived(base.InnerHandle, inId);
		}

		public ulong AddNotifySessionInviteAccepted(AddNotifySessionInviteAcceptedOptions options, object clientData, OnSessionInviteAcceptedCallback notificationFn)
		{
			AddNotifySessionInviteAcceptedOptionsInternal addNotifySessionInviteAcceptedOptionsInternal = Helper.CopyProperties<AddNotifySessionInviteAcceptedOptionsInternal>(options);
			OnSessionInviteAcceptedCallbackInternal onSessionInviteAcceptedCallbackInternal = new OnSessionInviteAcceptedCallbackInternal(SessionsInterface.OnSessionInviteAccepted);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onSessionInviteAcceptedCallbackInternal, Array.Empty<Delegate>());
			ulong num = SessionsInterface.EOS_Sessions_AddNotifySessionInviteAccepted(base.InnerHandle, ref addNotifySessionInviteAcceptedOptionsInternal, zero, onSessionInviteAcceptedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifySessionInviteAcceptedOptionsInternal>(ref addNotifySessionInviteAcceptedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifySessionInviteAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			SessionsInterface.EOS_Sessions_RemoveNotifySessionInviteAccepted(base.InnerHandle, inId);
		}

		public ulong AddNotifyJoinSessionAccepted(AddNotifyJoinSessionAcceptedOptions options, object clientData, OnJoinSessionAcceptedCallback notificationFn)
		{
			AddNotifyJoinSessionAcceptedOptionsInternal addNotifyJoinSessionAcceptedOptionsInternal = Helper.CopyProperties<AddNotifyJoinSessionAcceptedOptionsInternal>(options);
			OnJoinSessionAcceptedCallbackInternal onJoinSessionAcceptedCallbackInternal = new OnJoinSessionAcceptedCallbackInternal(SessionsInterface.OnJoinSessionAccepted);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onJoinSessionAcceptedCallbackInternal, Array.Empty<Delegate>());
			ulong num = SessionsInterface.EOS_Sessions_AddNotifyJoinSessionAccepted(base.InnerHandle, ref addNotifyJoinSessionAcceptedOptionsInternal, zero, onJoinSessionAcceptedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyJoinSessionAcceptedOptionsInternal>(ref addNotifyJoinSessionAcceptedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyJoinSessionAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			SessionsInterface.EOS_Sessions_RemoveNotifyJoinSessionAccepted(base.InnerHandle, inId);
		}

		public Result CopySessionHandleByInviteId(CopySessionHandleByInviteIdOptions options, out SessionDetails outSessionHandle)
		{
			CopySessionHandleByInviteIdOptionsInternal copySessionHandleByInviteIdOptionsInternal = Helper.CopyProperties<CopySessionHandleByInviteIdOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<SessionDetails>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionsInterface.EOS_Sessions_CopySessionHandleByInviteId(base.InnerHandle, ref copySessionHandleByInviteIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopySessionHandleByInviteIdOptionsInternal>(ref copySessionHandleByInviteIdOptionsInternal);
			Helper.TryMarshalGet<SessionDetails>(zero, out outSessionHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopySessionHandleByUiEventId(CopySessionHandleByUiEventIdOptions options, out SessionDetails outSessionHandle)
		{
			CopySessionHandleByUiEventIdOptionsInternal copySessionHandleByUiEventIdOptionsInternal = Helper.CopyProperties<CopySessionHandleByUiEventIdOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<SessionDetails>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionsInterface.EOS_Sessions_CopySessionHandleByUiEventId(base.InnerHandle, ref copySessionHandleByUiEventIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopySessionHandleByUiEventIdOptionsInternal>(ref copySessionHandleByUiEventIdOptionsInternal);
			Helper.TryMarshalGet<SessionDetails>(zero, out outSessionHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopySessionHandleForPresence(CopySessionHandleForPresenceOptions options, out SessionDetails outSessionHandle)
		{
			CopySessionHandleForPresenceOptionsInternal copySessionHandleForPresenceOptionsInternal = Helper.CopyProperties<CopySessionHandleForPresenceOptionsInternal>(options);
			outSessionHandle = Helper.GetDefault<SessionDetails>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionsInterface.EOS_Sessions_CopySessionHandleForPresence(base.InnerHandle, ref copySessionHandleForPresenceOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopySessionHandleForPresenceOptionsInternal>(ref copySessionHandleForPresenceOptionsInternal);
			Helper.TryMarshalGet<SessionDetails>(zero, out outSessionHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result IsUserInSession(IsUserInSessionOptions options)
		{
			IsUserInSessionOptionsInternal isUserInSessionOptionsInternal = Helper.CopyProperties<IsUserInSessionOptionsInternal>(options);
			Result result = SessionsInterface.EOS_Sessions_IsUserInSession(base.InnerHandle, ref isUserInSessionOptionsInternal);
			Helper.TryMarshalDispose<IsUserInSessionOptionsInternal>(ref isUserInSessionOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result DumpSessionState(DumpSessionStateOptions options)
		{
			DumpSessionStateOptionsInternal dumpSessionStateOptionsInternal = Helper.CopyProperties<DumpSessionStateOptionsInternal>(options);
			Result result = SessionsInterface.EOS_Sessions_DumpSessionState(base.InnerHandle, ref dumpSessionStateOptionsInternal);
			Helper.TryMarshalDispose<DumpSessionStateOptionsInternal>(ref dumpSessionStateOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnJoinSessionAccepted(IntPtr address)
		{
			OnJoinSessionAcceptedCallback onJoinSessionAcceptedCallback = null;
			JoinSessionAcceptedCallbackInfo joinSessionAcceptedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinSessionAcceptedCallback, JoinSessionAcceptedCallbackInfoInternal, JoinSessionAcceptedCallbackInfo>(address, out onJoinSessionAcceptedCallback, out joinSessionAcceptedCallbackInfo))
			{
				onJoinSessionAcceptedCallback(joinSessionAcceptedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnSessionInviteAccepted(IntPtr address)
		{
			OnSessionInviteAcceptedCallback onSessionInviteAcceptedCallback = null;
			SessionInviteAcceptedCallbackInfo sessionInviteAcceptedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnSessionInviteAcceptedCallback, SessionInviteAcceptedCallbackInfoInternal, SessionInviteAcceptedCallbackInfo>(address, out onSessionInviteAcceptedCallback, out sessionInviteAcceptedCallbackInfo))
			{
				onSessionInviteAcceptedCallback(sessionInviteAcceptedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnSessionInviteReceived(IntPtr address)
		{
			OnSessionInviteReceivedCallback onSessionInviteReceivedCallback = null;
			SessionInviteReceivedCallbackInfo sessionInviteReceivedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnSessionInviteReceivedCallback, SessionInviteReceivedCallbackInfoInternal, SessionInviteReceivedCallbackInfo>(address, out onSessionInviteReceivedCallback, out sessionInviteReceivedCallbackInfo))
			{
				onSessionInviteReceivedCallback(sessionInviteReceivedCallbackInfo);
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
		internal static void OnUnregisterPlayers(IntPtr address)
		{
			OnUnregisterPlayersCallback onUnregisterPlayersCallback = null;
			UnregisterPlayersCallbackInfo unregisterPlayersCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUnregisterPlayersCallback, UnregisterPlayersCallbackInfoInternal, UnregisterPlayersCallbackInfo>(address, out onUnregisterPlayersCallback, out unregisterPlayersCallbackInfo))
			{
				onUnregisterPlayersCallback(unregisterPlayersCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnRegisterPlayers(IntPtr address)
		{
			OnRegisterPlayersCallback onRegisterPlayersCallback = null;
			RegisterPlayersCallbackInfo registerPlayersCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnRegisterPlayersCallback, RegisterPlayersCallbackInfoInternal, RegisterPlayersCallbackInfo>(address, out onRegisterPlayersCallback, out registerPlayersCallbackInfo))
			{
				onRegisterPlayersCallback(registerPlayersCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnEndSession(IntPtr address)
		{
			OnEndSessionCallback onEndSessionCallback = null;
			EndSessionCallbackInfo endSessionCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnEndSessionCallback, EndSessionCallbackInfoInternal, EndSessionCallbackInfo>(address, out onEndSessionCallback, out endSessionCallbackInfo))
			{
				onEndSessionCallback(endSessionCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnStartSession(IntPtr address)
		{
			OnStartSessionCallback onStartSessionCallback = null;
			StartSessionCallbackInfo startSessionCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnStartSessionCallback, StartSessionCallbackInfoInternal, StartSessionCallbackInfo>(address, out onStartSessionCallback, out startSessionCallbackInfo))
			{
				onStartSessionCallback(startSessionCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnJoinSession(IntPtr address)
		{
			OnJoinSessionCallback onJoinSessionCallback = null;
			JoinSessionCallbackInfo joinSessionCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinSessionCallback, JoinSessionCallbackInfoInternal, JoinSessionCallbackInfo>(address, out onJoinSessionCallback, out joinSessionCallbackInfo))
			{
				onJoinSessionCallback(joinSessionCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDestroySession(IntPtr address)
		{
			OnDestroySessionCallback onDestroySessionCallback = null;
			DestroySessionCallbackInfo destroySessionCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDestroySessionCallback, DestroySessionCallbackInfoInternal, DestroySessionCallbackInfo>(address, out onDestroySessionCallback, out destroySessionCallbackInfo))
			{
				onDestroySessionCallback(destroySessionCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnUpdateSession(IntPtr address)
		{
			OnUpdateSessionCallback onUpdateSessionCallback = null;
			UpdateSessionCallbackInfo updateSessionCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUpdateSessionCallback, UpdateSessionCallbackInfoInternal, UpdateSessionCallbackInfo>(address, out onUpdateSessionCallback, out updateSessionCallbackInfo))
			{
				onUpdateSessionCallback(updateSessionCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_DumpSessionState(IntPtr handle, ref DumpSessionStateOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_IsUserInSession(IntPtr handle, ref IsUserInSessionOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_CopySessionHandleForPresence(IntPtr handle, ref CopySessionHandleForPresenceOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_CopySessionHandleByUiEventId(IntPtr handle, ref CopySessionHandleByUiEventIdOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_CopySessionHandleByInviteId(IntPtr handle, ref CopySessionHandleByInviteIdOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_RemoveNotifyJoinSessionAccepted(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Sessions_AddNotifyJoinSessionAccepted(IntPtr handle, ref AddNotifyJoinSessionAcceptedOptionsInternal options, IntPtr clientData, OnJoinSessionAcceptedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_RemoveNotifySessionInviteAccepted(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Sessions_AddNotifySessionInviteAccepted(IntPtr handle, ref AddNotifySessionInviteAcceptedOptionsInternal options, IntPtr clientData, OnSessionInviteAcceptedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_RemoveNotifySessionInviteReceived(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Sessions_AddNotifySessionInviteReceived(IntPtr handle, ref AddNotifySessionInviteReceivedOptionsInternal options, IntPtr clientData, OnSessionInviteReceivedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_CopyActiveSessionHandle(IntPtr handle, ref CopyActiveSessionHandleOptionsInternal options, ref IntPtr outSessionHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_CreateSessionSearch(IntPtr handle, ref CreateSessionSearchOptionsInternal options, ref IntPtr outSessionSearchHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_GetInviteIdByIndex(IntPtr handle, ref GetInviteIdByIndexOptionsInternal options, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Sessions_GetInviteCount(IntPtr handle, ref GetInviteCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_QueryInvites(IntPtr handle, ref QueryInvitesOptionsInternal options, IntPtr clientData, OnQueryInvitesCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_RejectInvite(IntPtr handle, ref RejectInviteOptionsInternal options, IntPtr clientData, OnRejectInviteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_SendInvite(IntPtr handle, ref SendInviteOptionsInternal options, IntPtr clientData, OnSendInviteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_UnregisterPlayers(IntPtr handle, ref UnregisterPlayersOptionsInternal options, IntPtr clientData, OnUnregisterPlayersCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_RegisterPlayers(IntPtr handle, ref RegisterPlayersOptionsInternal options, IntPtr clientData, OnRegisterPlayersCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_EndSession(IntPtr handle, ref EndSessionOptionsInternal options, IntPtr clientData, OnEndSessionCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_StartSession(IntPtr handle, ref StartSessionOptionsInternal options, IntPtr clientData, OnStartSessionCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_JoinSession(IntPtr handle, ref JoinSessionOptionsInternal options, IntPtr clientData, OnJoinSessionCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_DestroySession(IntPtr handle, ref DestroySessionOptionsInternal options, IntPtr clientData, OnDestroySessionCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Sessions_UpdateSession(IntPtr handle, ref UpdateSessionOptionsInternal options, IntPtr clientData, OnUpdateSessionCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_UpdateSessionModification(IntPtr handle, ref UpdateSessionModificationOptionsInternal options, ref IntPtr outSessionModificationHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Sessions_CreateSessionModification(IntPtr handle, ref CreateSessionModificationOptionsInternal options, ref IntPtr outSessionModificationHandle);

		public const int DumpsessionstateApiLatest = 1;

		public const int IsuserinsessionApiLatest = 1;

		public const int CopysessionhandleforpresenceApiLatest = 1;

		public const int CopysessionhandlebyuieventidApiLatest = 1;

		public const int CopysessionhandlebyinviteidApiLatest = 1;

		public const int AddnotifyjoinsessionacceptedApiLatest = 1;

		public const int AddnotifysessioninviteacceptedApiLatest = 1;

		public const int AddnotifysessioninvitereceivedApiLatest = 1;

		public const int CopyactivesessionhandleApiLatest = 1;

		public const int ActivesessionInfoApiLatest = 1;

		public const int SessiondetailsCopysessionattributebykeyApiLatest = 1;

		public const int SessiondetailsCopysessionattributebyindexApiLatest = 1;

		public const int SessiondetailsGetsessionattributecountApiLatest = 1;

		public const int SessiondetailsCopyinfoApiLatest = 1;

		public const int SessiondetailsInfoApiLatest = 1;

		public const int SessiondetailsSettingsApiLatest = 2;

		public const int SessionsearchRemoveparameterApiLatest = 1;

		public const int SessionsearchSetparameterApiLatest = 1;

		public const int SessionsearchSettargetuseridApiLatest = 1;

		public const int SessionsearchSetsessionidApiLatest = 1;

		public const int SessionsearchCopysearchresultbyindexApiLatest = 1;

		public const int SessionsearchGetsearchresultcountApiLatest = 1;

		public const int SessionsearchFindApiLatest = 2;

		public const int SessionsearchSetmaxsearchresultsApiLatest = 1;

		public const int MaxSearchResults = 200;

		public const int SessionmodificationRemoveattributeApiLatest = 1;

		public const int SessionmodificationAddattributeApiLatest = 1;

		public const int SessionattributeApiLatest = 1;

		public const int SessiondetailsAttributeApiLatest = 1;

		public const int ActivesessionGetregisteredplayerbyindexApiLatest = 1;

		public const int ActivesessionGetregisteredplayercountApiLatest = 1;

		public const int ActivesessionCopyinfoApiLatest = 1;

		public const int SessionattributedataApiLatest = 1;

		public const int AttributedataApiLatest = 1;

		public const string SearchMinslotsavailable = "minslotsavailable";

		public const string SearchNonemptyServersOnly = "nonemptyonly";

		public const string SearchEmptyServersOnly = "emptyonly";

		public const string SearchBucketId = "bucket";

		public const int SessionmodificationSetinvitesallowedApiLatest = 1;

		public const int SessionmodificationSetmaxplayersApiLatest = 1;

		public const int Maxregisteredplayers = 1000;

		public const int SessionmodificationSetjoininprogressallowedApiLatest = 1;

		public const int SessionmodificationSetpermissionlevelApiLatest = 1;

		public const int SessionmodificationSethostaddressApiLatest = 1;

		public const int SessionmodificationSetbucketidApiLatest = 1;

		public const int UnregisterplayersApiLatest = 1;

		public const int RegisterplayersApiLatest = 1;

		public const int EndsessionApiLatest = 1;

		public const int StartsessionApiLatest = 1;

		public const int JoinsessionApiLatest = 2;

		public const int DestroysessionApiLatest = 1;

		public const int UpdatesessionApiLatest = 1;

		public const int CreatesessionsearchApiLatest = 1;

		public const int GetinviteidbyindexApiLatest = 1;

		public const int GetinvitecountApiLatest = 1;

		public const int QueryinvitesApiLatest = 1;

		public const int RejectinviteApiLatest = 1;

		public const int SendinviteApiLatest = 1;

		public const int InviteidMaxLength = 64;

		public const int UpdatesessionmodificationApiLatest = 1;

		public const int CreatesessionmodificationApiLatest = 3;

		public const int SessionmodificationMaxSessionidoverrideLength = 64;

		public const int SessionmodificationMinSessionidoverrideLength = 16;

		public const int SessionmodificationMaxSessionAttributeLength = 64;

		public const int SessionmodificationMaxSessionAttributes = 64;
	}
}
