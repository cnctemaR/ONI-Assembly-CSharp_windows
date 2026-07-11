using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Presence
{
	public sealed class PresenceInterface : Handle
	{
		public PresenceInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryPresence(QueryPresenceOptions options, object clientData, OnQueryPresenceCompleteCallback completionDelegate)
		{
			QueryPresenceOptionsInternal queryPresenceOptionsInternal = Helper.CopyProperties<QueryPresenceOptionsInternal>(options);
			OnQueryPresenceCompleteCallbackInternal onQueryPresenceCompleteCallbackInternal = new OnQueryPresenceCompleteCallbackInternal(PresenceInterface.OnQueryPresenceComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryPresenceCompleteCallbackInternal, Array.Empty<Delegate>());
			PresenceInterface.EOS_Presence_QueryPresence(base.InnerHandle, ref queryPresenceOptionsInternal, zero, onQueryPresenceCompleteCallbackInternal);
			Helper.TryMarshalDispose<QueryPresenceOptionsInternal>(ref queryPresenceOptionsInternal);
		}

		public bool HasPresence(HasPresenceOptions options)
		{
			HasPresenceOptionsInternal hasPresenceOptionsInternal = Helper.CopyProperties<HasPresenceOptionsInternal>(options);
			int num = PresenceInterface.EOS_Presence_HasPresence(base.InnerHandle, ref hasPresenceOptionsInternal);
			Helper.TryMarshalDispose<HasPresenceOptionsInternal>(ref hasPresenceOptionsInternal);
			bool @default = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(num, out @default);
			return @default;
		}

		public Result CopyPresence(CopyPresenceOptions options, out Info outPresence)
		{
			CopyPresenceOptionsInternal copyPresenceOptionsInternal = Helper.CopyProperties<CopyPresenceOptionsInternal>(options);
			outPresence = Helper.GetDefault<Info>();
			IntPtr zero = IntPtr.Zero;
			Result result = PresenceInterface.EOS_Presence_CopyPresence(base.InnerHandle, ref copyPresenceOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyPresenceOptionsInternal>(ref copyPresenceOptionsInternal);
			if (Helper.TryMarshalGet<InfoInternal, Info>(zero, out outPresence))
			{
				PresenceInterface.EOS_Presence_Info_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CreatePresenceModification(CreatePresenceModificationOptions options, out PresenceModification outPresenceModificationHandle)
		{
			CreatePresenceModificationOptionsInternal createPresenceModificationOptionsInternal = Helper.CopyProperties<CreatePresenceModificationOptionsInternal>(options);
			outPresenceModificationHandle = Helper.GetDefault<PresenceModification>();
			IntPtr zero = IntPtr.Zero;
			Result result = PresenceInterface.EOS_Presence_CreatePresenceModification(base.InnerHandle, ref createPresenceModificationOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CreatePresenceModificationOptionsInternal>(ref createPresenceModificationOptionsInternal);
			Helper.TryMarshalGet<PresenceModification>(zero, out outPresenceModificationHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void SetPresence(SetPresenceOptions options, object clientData, SetPresenceCompleteCallback completionDelegate)
		{
			SetPresenceOptionsInternal setPresenceOptionsInternal = Helper.CopyProperties<SetPresenceOptionsInternal>(options);
			SetPresenceCompleteCallbackInternal setPresenceCompleteCallbackInternal = new SetPresenceCompleteCallbackInternal(PresenceInterface.SetPresenceComplete);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, setPresenceCompleteCallbackInternal, Array.Empty<Delegate>());
			PresenceInterface.EOS_Presence_SetPresence(base.InnerHandle, ref setPresenceOptionsInternal, zero, setPresenceCompleteCallbackInternal);
			Helper.TryMarshalDispose<SetPresenceOptionsInternal>(ref setPresenceOptionsInternal);
		}

		public ulong AddNotifyOnPresenceChanged(AddNotifyOnPresenceChangedOptions options, object clientData, OnPresenceChangedCallback notificationHandler)
		{
			AddNotifyOnPresenceChangedOptionsInternal addNotifyOnPresenceChangedOptionsInternal = Helper.CopyProperties<AddNotifyOnPresenceChangedOptionsInternal>(options);
			OnPresenceChangedCallbackInternal onPresenceChangedCallbackInternal = new OnPresenceChangedCallbackInternal(PresenceInterface.OnPresenceChanged);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationHandler, onPresenceChangedCallbackInternal, Array.Empty<Delegate>());
			ulong num = PresenceInterface.EOS_Presence_AddNotifyOnPresenceChanged(base.InnerHandle, ref addNotifyOnPresenceChangedOptionsInternal, zero, onPresenceChangedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyOnPresenceChangedOptionsInternal>(ref addNotifyOnPresenceChangedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyOnPresenceChanged(ulong notificationId)
		{
			Helper.TryRemoveCallbackByNotificationId(notificationId);
			PresenceInterface.EOS_Presence_RemoveNotifyOnPresenceChanged(base.InnerHandle, notificationId);
		}

		public ulong AddNotifyJoinGameAccepted(AddNotifyJoinGameAcceptedOptions options, object clientData, OnJoinGameAcceptedCallback notificationFn)
		{
			AddNotifyJoinGameAcceptedOptionsInternal addNotifyJoinGameAcceptedOptionsInternal = Helper.CopyProperties<AddNotifyJoinGameAcceptedOptionsInternal>(options);
			OnJoinGameAcceptedCallbackInternal onJoinGameAcceptedCallbackInternal = new OnJoinGameAcceptedCallbackInternal(PresenceInterface.OnJoinGameAccepted);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onJoinGameAcceptedCallbackInternal, Array.Empty<Delegate>());
			ulong num = PresenceInterface.EOS_Presence_AddNotifyJoinGameAccepted(base.InnerHandle, ref addNotifyJoinGameAcceptedOptionsInternal, zero, onJoinGameAcceptedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyJoinGameAcceptedOptionsInternal>(ref addNotifyJoinGameAcceptedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyJoinGameAccepted(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			PresenceInterface.EOS_Presence_RemoveNotifyJoinGameAccepted(base.InnerHandle, inId);
		}

		public Result GetJoinInfo(GetJoinInfoOptions options, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			GetJoinInfoOptionsInternal getJoinInfoOptionsInternal = Helper.CopyProperties<GetJoinInfoOptionsInternal>(options);
			Result result = PresenceInterface.EOS_Presence_GetJoinInfo(base.InnerHandle, ref getJoinInfoOptionsInternal, outBuffer, ref inOutBufferLength);
			Helper.TryMarshalDispose<GetJoinInfoOptionsInternal>(ref getJoinInfoOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnJoinGameAccepted(IntPtr address)
		{
			OnJoinGameAcceptedCallback onJoinGameAcceptedCallback = null;
			JoinGameAcceptedCallbackInfo joinGameAcceptedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnJoinGameAcceptedCallback, JoinGameAcceptedCallbackInfoInternal, JoinGameAcceptedCallbackInfo>(address, out onJoinGameAcceptedCallback, out joinGameAcceptedCallbackInfo))
			{
				onJoinGameAcceptedCallback(joinGameAcceptedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnPresenceChanged(IntPtr address)
		{
			OnPresenceChangedCallback onPresenceChangedCallback = null;
			PresenceChangedCallbackInfo presenceChangedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnPresenceChangedCallback, PresenceChangedCallbackInfoInternal, PresenceChangedCallbackInfo>(address, out onPresenceChangedCallback, out presenceChangedCallbackInfo))
			{
				onPresenceChangedCallback(presenceChangedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void SetPresenceComplete(IntPtr address)
		{
			SetPresenceCompleteCallback setPresenceCompleteCallback = null;
			SetPresenceCallbackInfo setPresenceCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<SetPresenceCompleteCallback, SetPresenceCallbackInfoInternal, SetPresenceCallbackInfo>(address, out setPresenceCompleteCallback, out setPresenceCallbackInfo))
			{
				setPresenceCompleteCallback(setPresenceCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryPresenceComplete(IntPtr address)
		{
			OnQueryPresenceCompleteCallback onQueryPresenceCompleteCallback = null;
			QueryPresenceCallbackInfo queryPresenceCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryPresenceCompleteCallback, QueryPresenceCallbackInfoInternal, QueryPresenceCallbackInfo>(address, out onQueryPresenceCompleteCallback, out queryPresenceCallbackInfo))
			{
				onQueryPresenceCompleteCallback(queryPresenceCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Presence_Info_Release(IntPtr presenceInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Presence_GetJoinInfo(IntPtr handle, ref GetJoinInfoOptionsInternal options, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Presence_RemoveNotifyJoinGameAccepted(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Presence_AddNotifyJoinGameAccepted(IntPtr handle, ref AddNotifyJoinGameAcceptedOptionsInternal options, IntPtr clientData, OnJoinGameAcceptedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Presence_RemoveNotifyOnPresenceChanged(IntPtr handle, ulong notificationId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Presence_AddNotifyOnPresenceChanged(IntPtr handle, ref AddNotifyOnPresenceChangedOptionsInternal options, IntPtr clientData, OnPresenceChangedCallbackInternal notificationHandler);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Presence_SetPresence(IntPtr handle, ref SetPresenceOptionsInternal options, IntPtr clientData, SetPresenceCompleteCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Presence_CreatePresenceModification(IntPtr handle, ref CreatePresenceModificationOptionsInternal options, ref IntPtr outPresenceModificationHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Presence_CopyPresence(IntPtr handle, ref CopyPresenceOptionsInternal options, ref IntPtr outPresence);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_Presence_HasPresence(IntPtr handle, ref HasPresenceOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Presence_QueryPresence(IntPtr handle, ref QueryPresenceOptionsInternal options, IntPtr clientData, OnQueryPresenceCompleteCallbackInternal completionDelegate);

		public const int DeletedataApiLatest = 1;

		public const int PresencemodificationDeletedataApiLatest = 1;

		public const int PresencemodificationDatarecordidApiLatest = 1;

		public const int SetdataApiLatest = 1;

		public const int PresencemodificationSetdataApiLatest = 1;

		public const int SetrawrichtextApiLatest = 1;

		public const int PresencemodificationSetrawrichtextApiLatest = 1;

		public const int SetstatusApiLatest = 1;

		public const int PresencemodificationSetstatusApiLatest = 1;

		public const int RichTextMaxValueLength = 255;

		public const int DataMaxValueLength = 255;

		public const int DataMaxKeyLength = 64;

		public const int DataMaxKeys = 32;

		public const int PresencemodificationSetjoininfoApiLatest = 1;

		public const int PresencemodificationJoininfoMaxLength = 255;

		public const int GetjoininfoApiLatest = 1;

		public const int AddnotifyjoingameacceptedApiLatest = 2;

		public const int AddnotifyonpresencechangedApiLatest = 1;

		public const int SetpresenceApiLatest = 1;

		public const int CreatepresencemodificationApiLatest = 1;

		public const int CopypresenceApiLatest = 2;

		public const int HaspresenceApiLatest = 1;

		public const int QuerypresenceApiLatest = 1;

		public const int InfoApiLatest = 2;

		public const int DatarecordApiLatest = 1;
	}
}
