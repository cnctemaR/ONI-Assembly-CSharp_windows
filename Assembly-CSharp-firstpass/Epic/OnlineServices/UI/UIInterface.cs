using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UI
{
	public sealed class UIInterface : Handle
	{
		public UIInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void ShowFriends(ShowFriendsOptions options, object clientData, OnShowFriendsCallback completionDelegate)
		{
			ShowFriendsOptionsInternal showFriendsOptionsInternal = Helper.CopyProperties<ShowFriendsOptionsInternal>(options);
			OnShowFriendsCallbackInternal onShowFriendsCallbackInternal = new OnShowFriendsCallbackInternal(UIInterface.OnShowFriends);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onShowFriendsCallbackInternal, Array.Empty<Delegate>());
			UIInterface.EOS_UI_ShowFriends(base.InnerHandle, ref showFriendsOptionsInternal, zero, onShowFriendsCallbackInternal);
			Helper.TryMarshalDispose<ShowFriendsOptionsInternal>(ref showFriendsOptionsInternal);
		}

		public void HideFriends(HideFriendsOptions options, object clientData, OnHideFriendsCallback completionDelegate)
		{
			HideFriendsOptionsInternal hideFriendsOptionsInternal = Helper.CopyProperties<HideFriendsOptionsInternal>(options);
			OnHideFriendsCallbackInternal onHideFriendsCallbackInternal = new OnHideFriendsCallbackInternal(UIInterface.OnHideFriends);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onHideFriendsCallbackInternal, Array.Empty<Delegate>());
			UIInterface.EOS_UI_HideFriends(base.InnerHandle, ref hideFriendsOptionsInternal, zero, onHideFriendsCallbackInternal);
			Helper.TryMarshalDispose<HideFriendsOptionsInternal>(ref hideFriendsOptionsInternal);
		}

		public bool GetFriendsVisible(GetFriendsVisibleOptions options)
		{
			GetFriendsVisibleOptionsInternal getFriendsVisibleOptionsInternal = Helper.CopyProperties<GetFriendsVisibleOptionsInternal>(options);
			int num = UIInterface.EOS_UI_GetFriendsVisible(base.InnerHandle, ref getFriendsVisibleOptionsInternal);
			Helper.TryMarshalDispose<GetFriendsVisibleOptionsInternal>(ref getFriendsVisibleOptionsInternal);
			bool @default = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(num, out @default);
			return @default;
		}

		public ulong AddNotifyDisplaySettingsUpdated(AddNotifyDisplaySettingsUpdatedOptions options, object clientData, OnDisplaySettingsUpdatedCallback notificationFn)
		{
			AddNotifyDisplaySettingsUpdatedOptionsInternal addNotifyDisplaySettingsUpdatedOptionsInternal = Helper.CopyProperties<AddNotifyDisplaySettingsUpdatedOptionsInternal>(options);
			OnDisplaySettingsUpdatedCallbackInternal onDisplaySettingsUpdatedCallbackInternal = new OnDisplaySettingsUpdatedCallbackInternal(UIInterface.OnDisplaySettingsUpdated);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notificationFn, onDisplaySettingsUpdatedCallbackInternal, Array.Empty<Delegate>());
			ulong num = UIInterface.EOS_UI_AddNotifyDisplaySettingsUpdated(base.InnerHandle, ref addNotifyDisplaySettingsUpdatedOptionsInternal, zero, onDisplaySettingsUpdatedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyDisplaySettingsUpdatedOptionsInternal>(ref addNotifyDisplaySettingsUpdatedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyDisplaySettingsUpdated(ulong id)
		{
			Helper.TryRemoveCallbackByNotificationId(id);
			UIInterface.EOS_UI_RemoveNotifyDisplaySettingsUpdated(base.InnerHandle, id);
		}

		public Result SetToggleFriendsKey(SetToggleFriendsKeyOptions options)
		{
			SetToggleFriendsKeyOptionsInternal setToggleFriendsKeyOptionsInternal = Helper.CopyProperties<SetToggleFriendsKeyOptionsInternal>(options);
			Result result = UIInterface.EOS_UI_SetToggleFriendsKey(base.InnerHandle, ref setToggleFriendsKeyOptionsInternal);
			Helper.TryMarshalDispose<SetToggleFriendsKeyOptionsInternal>(ref setToggleFriendsKeyOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public KeyCombination GetToggleFriendsKey(GetToggleFriendsKeyOptions options)
		{
			GetToggleFriendsKeyOptionsInternal getToggleFriendsKeyOptionsInternal = Helper.CopyProperties<GetToggleFriendsKeyOptionsInternal>(options);
			KeyCombination keyCombination = UIInterface.EOS_UI_GetToggleFriendsKey(base.InnerHandle, ref getToggleFriendsKeyOptionsInternal);
			Helper.TryMarshalDispose<GetToggleFriendsKeyOptionsInternal>(ref getToggleFriendsKeyOptionsInternal);
			KeyCombination @default = Helper.GetDefault<KeyCombination>();
			Helper.TryMarshalGet<KeyCombination>(keyCombination, out @default);
			return @default;
		}

		public bool IsValidKeyCombination(KeyCombination keyCombination)
		{
			int num = UIInterface.EOS_UI_IsValidKeyCombination(base.InnerHandle, keyCombination);
			bool @default = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(num, out @default);
			return @default;
		}

		public Result SetDisplayPreference(SetDisplayPreferenceOptions options)
		{
			SetDisplayPreferenceOptionsInternal setDisplayPreferenceOptionsInternal = Helper.CopyProperties<SetDisplayPreferenceOptionsInternal>(options);
			Result result = UIInterface.EOS_UI_SetDisplayPreference(base.InnerHandle, ref setDisplayPreferenceOptionsInternal);
			Helper.TryMarshalDispose<SetDisplayPreferenceOptionsInternal>(ref setDisplayPreferenceOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public NotificationLocation GetNotificationLocationPreference()
		{
			NotificationLocation notificationLocation = UIInterface.EOS_UI_GetNotificationLocationPreference(base.InnerHandle);
			NotificationLocation @default = Helper.GetDefault<NotificationLocation>();
			Helper.TryMarshalGet<NotificationLocation>(notificationLocation, out @default);
			return @default;
		}

		public Result AcknowledgeEventId(AcknowledgeEventIdOptions options)
		{
			AcknowledgeEventIdOptionsInternal acknowledgeEventIdOptionsInternal = Helper.CopyProperties<AcknowledgeEventIdOptionsInternal>(options);
			Result result = UIInterface.EOS_UI_AcknowledgeEventId(base.InnerHandle, ref acknowledgeEventIdOptionsInternal);
			Helper.TryMarshalDispose<AcknowledgeEventIdOptionsInternal>(ref acknowledgeEventIdOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnDisplaySettingsUpdated(IntPtr address)
		{
			OnDisplaySettingsUpdatedCallback onDisplaySettingsUpdatedCallback = null;
			OnDisplaySettingsUpdatedCallbackInfo onDisplaySettingsUpdatedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDisplaySettingsUpdatedCallback, OnDisplaySettingsUpdatedCallbackInfoInternal, OnDisplaySettingsUpdatedCallbackInfo>(address, out onDisplaySettingsUpdatedCallback, out onDisplaySettingsUpdatedCallbackInfo))
			{
				onDisplaySettingsUpdatedCallback(onDisplaySettingsUpdatedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnHideFriends(IntPtr address)
		{
			OnHideFriendsCallback onHideFriendsCallback = null;
			HideFriendsCallbackInfo hideFriendsCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnHideFriendsCallback, HideFriendsCallbackInfoInternal, HideFriendsCallbackInfo>(address, out onHideFriendsCallback, out hideFriendsCallbackInfo))
			{
				onHideFriendsCallback(hideFriendsCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnShowFriends(IntPtr address)
		{
			OnShowFriendsCallback onShowFriendsCallback = null;
			ShowFriendsCallbackInfo showFriendsCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnShowFriendsCallback, ShowFriendsCallbackInfoInternal, ShowFriendsCallbackInfo>(address, out onShowFriendsCallback, out showFriendsCallbackInfo))
			{
				onShowFriendsCallback(showFriendsCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_UI_AcknowledgeEventId(IntPtr handle, ref AcknowledgeEventIdOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern NotificationLocation EOS_UI_GetNotificationLocationPreference(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_UI_SetDisplayPreference(IntPtr handle, ref SetDisplayPreferenceOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_UI_IsValidKeyCombination(IntPtr handle, KeyCombination keyCombination);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern KeyCombination EOS_UI_GetToggleFriendsKey(IntPtr handle, ref GetToggleFriendsKeyOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_UI_SetToggleFriendsKey(IntPtr handle, ref SetToggleFriendsKeyOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_UI_RemoveNotifyDisplaySettingsUpdated(IntPtr handle, ulong id);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_UI_AddNotifyDisplaySettingsUpdated(IntPtr handle, ref AddNotifyDisplaySettingsUpdatedOptionsInternal options, IntPtr clientData, OnDisplaySettingsUpdatedCallbackInternal notificationFn);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_UI_GetFriendsVisible(IntPtr handle, ref GetFriendsVisibleOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_UI_HideFriends(IntPtr handle, ref HideFriendsOptionsInternal options, IntPtr clientData, OnHideFriendsCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_UI_ShowFriends(IntPtr handle, ref ShowFriendsOptionsInternal options, IntPtr clientData, OnShowFriendsCallbackInternal completionDelegate);

		public const int AcknowledgecorrelationidApiLatest = 1;

		public const int AcknowledgeeventidApiLatest = 1;

		public const int SetdisplaypreferenceApiLatest = 1;

		public const int GettogglefriendskeyApiLatest = 1;

		public const int SettogglefriendskeyApiLatest = 1;

		public const int AddnotifydisplaysettingsupdatedApiLatest = 1;

		public const int GetfriendsvisibleApiLatest = 1;

		public const int HidefriendsApiLatest = 1;

		public const int ShowfriendsApiLatest = 1;

		public const int EventidInvalid = 0;
	}
}
