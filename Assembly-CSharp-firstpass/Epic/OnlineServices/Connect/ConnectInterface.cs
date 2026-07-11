using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Connect
{
	public sealed class ConnectInterface : Handle
	{
		public ConnectInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void Login(LoginOptions options, object clientData, OnLoginCallback completionDelegate)
		{
			LoginOptionsInternal loginOptionsInternal = Helper.CopyProperties<LoginOptionsInternal>(options);
			OnLoginCallbackInternal onLoginCallbackInternal = new OnLoginCallbackInternal(ConnectInterface.OnLogin);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onLoginCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_Login(base.InnerHandle, ref loginOptionsInternal, zero, onLoginCallbackInternal);
			Helper.TryMarshalDispose<LoginOptionsInternal>(ref loginOptionsInternal);
		}

		public void CreateUser(CreateUserOptions options, object clientData, OnCreateUserCallback completionDelegate)
		{
			CreateUserOptionsInternal createUserOptionsInternal = Helper.CopyProperties<CreateUserOptionsInternal>(options);
			OnCreateUserCallbackInternal onCreateUserCallbackInternal = new OnCreateUserCallbackInternal(ConnectInterface.OnCreateUser);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onCreateUserCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_CreateUser(base.InnerHandle, ref createUserOptionsInternal, zero, onCreateUserCallbackInternal);
			Helper.TryMarshalDispose<CreateUserOptionsInternal>(ref createUserOptionsInternal);
		}

		public void LinkAccount(LinkAccountOptions options, object clientData, OnLinkAccountCallback completionDelegate)
		{
			LinkAccountOptionsInternal linkAccountOptionsInternal = Helper.CopyProperties<LinkAccountOptionsInternal>(options);
			OnLinkAccountCallbackInternal onLinkAccountCallbackInternal = new OnLinkAccountCallbackInternal(ConnectInterface.OnLinkAccount);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onLinkAccountCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_LinkAccount(base.InnerHandle, ref linkAccountOptionsInternal, zero, onLinkAccountCallbackInternal);
			Helper.TryMarshalDispose<LinkAccountOptionsInternal>(ref linkAccountOptionsInternal);
		}

		public void UnlinkAccount(UnlinkAccountOptions options, object clientData, OnUnlinkAccountCallback completionDelegate)
		{
			UnlinkAccountOptionsInternal unlinkAccountOptionsInternal = Helper.CopyProperties<UnlinkAccountOptionsInternal>(options);
			OnUnlinkAccountCallbackInternal onUnlinkAccountCallbackInternal = new OnUnlinkAccountCallbackInternal(ConnectInterface.OnUnlinkAccount);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onUnlinkAccountCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_UnlinkAccount(base.InnerHandle, ref unlinkAccountOptionsInternal, zero, onUnlinkAccountCallbackInternal);
			Helper.TryMarshalDispose<UnlinkAccountOptionsInternal>(ref unlinkAccountOptionsInternal);
		}

		public void CreateDeviceId(CreateDeviceIdOptions options, object clientData, OnCreateDeviceIdCallback completionDelegate)
		{
			CreateDeviceIdOptionsInternal createDeviceIdOptionsInternal = Helper.CopyProperties<CreateDeviceIdOptionsInternal>(options);
			OnCreateDeviceIdCallbackInternal onCreateDeviceIdCallbackInternal = new OnCreateDeviceIdCallbackInternal(ConnectInterface.OnCreateDeviceId);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onCreateDeviceIdCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_CreateDeviceId(base.InnerHandle, ref createDeviceIdOptionsInternal, zero, onCreateDeviceIdCallbackInternal);
			Helper.TryMarshalDispose<CreateDeviceIdOptionsInternal>(ref createDeviceIdOptionsInternal);
		}

		public void DeleteDeviceId(DeleteDeviceIdOptions options, object clientData, OnDeleteDeviceIdCallback completionDelegate)
		{
			DeleteDeviceIdOptionsInternal deleteDeviceIdOptionsInternal = Helper.CopyProperties<DeleteDeviceIdOptionsInternal>(options);
			OnDeleteDeviceIdCallbackInternal onDeleteDeviceIdCallbackInternal = new OnDeleteDeviceIdCallbackInternal(ConnectInterface.OnDeleteDeviceId);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onDeleteDeviceIdCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_DeleteDeviceId(base.InnerHandle, ref deleteDeviceIdOptionsInternal, zero, onDeleteDeviceIdCallbackInternal);
			Helper.TryMarshalDispose<DeleteDeviceIdOptionsInternal>(ref deleteDeviceIdOptionsInternal);
		}

		public void TransferDeviceIdAccount(TransferDeviceIdAccountOptions options, object clientData, OnTransferDeviceIdAccountCallback completionDelegate)
		{
			TransferDeviceIdAccountOptionsInternal transferDeviceIdAccountOptionsInternal = Helper.CopyProperties<TransferDeviceIdAccountOptionsInternal>(options);
			OnTransferDeviceIdAccountCallbackInternal onTransferDeviceIdAccountCallbackInternal = new OnTransferDeviceIdAccountCallbackInternal(ConnectInterface.OnTransferDeviceIdAccount);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onTransferDeviceIdAccountCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_TransferDeviceIdAccount(base.InnerHandle, ref transferDeviceIdAccountOptionsInternal, zero, onTransferDeviceIdAccountCallbackInternal);
			Helper.TryMarshalDispose<TransferDeviceIdAccountOptionsInternal>(ref transferDeviceIdAccountOptionsInternal);
		}

		public void QueryExternalAccountMappings(QueryExternalAccountMappingsOptions options, object clientData, OnQueryExternalAccountMappingsCallback completionDelegate)
		{
			QueryExternalAccountMappingsOptionsInternal queryExternalAccountMappingsOptionsInternal = Helper.CopyProperties<QueryExternalAccountMappingsOptionsInternal>(options);
			OnQueryExternalAccountMappingsCallbackInternal onQueryExternalAccountMappingsCallbackInternal = new OnQueryExternalAccountMappingsCallbackInternal(ConnectInterface.OnQueryExternalAccountMappings);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryExternalAccountMappingsCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_QueryExternalAccountMappings(base.InnerHandle, ref queryExternalAccountMappingsOptionsInternal, zero, onQueryExternalAccountMappingsCallbackInternal);
			Helper.TryMarshalDispose<QueryExternalAccountMappingsOptionsInternal>(ref queryExternalAccountMappingsOptionsInternal);
		}

		public void QueryProductUserIdMappings(QueryProductUserIdMappingsOptions options, object clientData, OnQueryProductUserIdMappingsCallback completionDelegate)
		{
			QueryProductUserIdMappingsOptionsInternal queryProductUserIdMappingsOptionsInternal = Helper.CopyProperties<QueryProductUserIdMappingsOptionsInternal>(options);
			OnQueryProductUserIdMappingsCallbackInternal onQueryProductUserIdMappingsCallbackInternal = new OnQueryProductUserIdMappingsCallbackInternal(ConnectInterface.OnQueryProductUserIdMappings);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryProductUserIdMappingsCallbackInternal, Array.Empty<Delegate>());
			ConnectInterface.EOS_Connect_QueryProductUserIdMappings(base.InnerHandle, ref queryProductUserIdMappingsOptionsInternal, zero, onQueryProductUserIdMappingsCallbackInternal);
			Helper.TryMarshalDispose<QueryProductUserIdMappingsOptionsInternal>(ref queryProductUserIdMappingsOptionsInternal);
		}

		public ProductUserId GetExternalAccountMapping(GetExternalAccountMappingsOptions options)
		{
			GetExternalAccountMappingsOptionsInternal getExternalAccountMappingsOptionsInternal = Helper.CopyProperties<GetExternalAccountMappingsOptionsInternal>(options);
			IntPtr intPtr = ConnectInterface.EOS_Connect_GetExternalAccountMapping(base.InnerHandle, ref getExternalAccountMappingsOptionsInternal);
			Helper.TryMarshalDispose<GetExternalAccountMappingsOptionsInternal>(ref getExternalAccountMappingsOptionsInternal);
			ProductUserId @default = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet<ProductUserId>(intPtr, out @default);
			return @default;
		}

		public Result GetProductUserIdMapping(GetProductUserIdMappingOptions options, StringBuilder outBuffer, ref int inOutBufferLength)
		{
			GetProductUserIdMappingOptionsInternal getProductUserIdMappingOptionsInternal = Helper.CopyProperties<GetProductUserIdMappingOptionsInternal>(options);
			Result result = ConnectInterface.EOS_Connect_GetProductUserIdMapping(base.InnerHandle, ref getProductUserIdMappingOptionsInternal, outBuffer, ref inOutBufferLength);
			Helper.TryMarshalDispose<GetProductUserIdMappingOptionsInternal>(ref getProductUserIdMappingOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetProductUserExternalAccountCount(GetProductUserExternalAccountCountOptions options)
		{
			GetProductUserExternalAccountCountOptionsInternal getProductUserExternalAccountCountOptionsInternal = Helper.CopyProperties<GetProductUserExternalAccountCountOptionsInternal>(options);
			uint num = ConnectInterface.EOS_Connect_GetProductUserExternalAccountCount(base.InnerHandle, ref getProductUserExternalAccountCountOptionsInternal);
			Helper.TryMarshalDispose<GetProductUserExternalAccountCountOptionsInternal>(ref getProductUserExternalAccountCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyProductUserExternalAccountByIndex(CopyProductUserExternalAccountByIndexOptions options, out ExternalAccountInfo outExternalAccountInfo)
		{
			CopyProductUserExternalAccountByIndexOptionsInternal copyProductUserExternalAccountByIndexOptionsInternal = Helper.CopyProperties<CopyProductUserExternalAccountByIndexOptionsInternal>(options);
			outExternalAccountInfo = Helper.GetDefault<ExternalAccountInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = ConnectInterface.EOS_Connect_CopyProductUserExternalAccountByIndex(base.InnerHandle, ref copyProductUserExternalAccountByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyProductUserExternalAccountByIndexOptionsInternal>(ref copyProductUserExternalAccountByIndexOptionsInternal);
			if (Helper.TryMarshalGet<ExternalAccountInfoInternal, ExternalAccountInfo>(zero, out outExternalAccountInfo))
			{
				ConnectInterface.EOS_Connect_ExternalAccountInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyProductUserExternalAccountByAccountType(CopyProductUserExternalAccountByAccountTypeOptions options, out ExternalAccountInfo outExternalAccountInfo)
		{
			CopyProductUserExternalAccountByAccountTypeOptionsInternal copyProductUserExternalAccountByAccountTypeOptionsInternal = Helper.CopyProperties<CopyProductUserExternalAccountByAccountTypeOptionsInternal>(options);
			outExternalAccountInfo = Helper.GetDefault<ExternalAccountInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = ConnectInterface.EOS_Connect_CopyProductUserExternalAccountByAccountType(base.InnerHandle, ref copyProductUserExternalAccountByAccountTypeOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyProductUserExternalAccountByAccountTypeOptionsInternal>(ref copyProductUserExternalAccountByAccountTypeOptionsInternal);
			if (Helper.TryMarshalGet<ExternalAccountInfoInternal, ExternalAccountInfo>(zero, out outExternalAccountInfo))
			{
				ConnectInterface.EOS_Connect_ExternalAccountInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyProductUserExternalAccountByAccountId(CopyProductUserExternalAccountByAccountIdOptions options, out ExternalAccountInfo outExternalAccountInfo)
		{
			CopyProductUserExternalAccountByAccountIdOptionsInternal copyProductUserExternalAccountByAccountIdOptionsInternal = Helper.CopyProperties<CopyProductUserExternalAccountByAccountIdOptionsInternal>(options);
			outExternalAccountInfo = Helper.GetDefault<ExternalAccountInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = ConnectInterface.EOS_Connect_CopyProductUserExternalAccountByAccountId(base.InnerHandle, ref copyProductUserExternalAccountByAccountIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyProductUserExternalAccountByAccountIdOptionsInternal>(ref copyProductUserExternalAccountByAccountIdOptionsInternal);
			if (Helper.TryMarshalGet<ExternalAccountInfoInternal, ExternalAccountInfo>(zero, out outExternalAccountInfo))
			{
				ConnectInterface.EOS_Connect_ExternalAccountInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyProductUserInfo(CopyProductUserInfoOptions options, out ExternalAccountInfo outExternalAccountInfo)
		{
			CopyProductUserInfoOptionsInternal copyProductUserInfoOptionsInternal = Helper.CopyProperties<CopyProductUserInfoOptionsInternal>(options);
			outExternalAccountInfo = Helper.GetDefault<ExternalAccountInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = ConnectInterface.EOS_Connect_CopyProductUserInfo(base.InnerHandle, ref copyProductUserInfoOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyProductUserInfoOptionsInternal>(ref copyProductUserInfoOptionsInternal);
			if (Helper.TryMarshalGet<ExternalAccountInfoInternal, ExternalAccountInfo>(zero, out outExternalAccountInfo))
			{
				ConnectInterface.EOS_Connect_ExternalAccountInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public int GetLoggedInUsersCount()
		{
			int num = ConnectInterface.EOS_Connect_GetLoggedInUsersCount(base.InnerHandle);
			int @default = Helper.GetDefault<int>();
			Helper.TryMarshalGet<int>(num, out @default);
			return @default;
		}

		public ProductUserId GetLoggedInUserByIndex(int index)
		{
			IntPtr intPtr = ConnectInterface.EOS_Connect_GetLoggedInUserByIndex(base.InnerHandle, index);
			ProductUserId @default = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet<ProductUserId>(intPtr, out @default);
			return @default;
		}

		public LoginStatus GetLoginStatus(ProductUserId localUserId)
		{
			LoginStatus loginStatus = ConnectInterface.EOS_Connect_GetLoginStatus(base.InnerHandle, localUserId.InnerHandle);
			LoginStatus @default = Helper.GetDefault<LoginStatus>();
			Helper.TryMarshalGet<LoginStatus>(loginStatus, out @default);
			return @default;
		}

		public ulong AddNotifyAuthExpiration(AddNotifyAuthExpirationOptions options, object clientData, OnAuthExpirationCallback notification)
		{
			AddNotifyAuthExpirationOptionsInternal addNotifyAuthExpirationOptionsInternal = Helper.CopyProperties<AddNotifyAuthExpirationOptionsInternal>(options);
			OnAuthExpirationCallbackInternal onAuthExpirationCallbackInternal = new OnAuthExpirationCallbackInternal(ConnectInterface.OnAuthExpiration);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notification, onAuthExpirationCallbackInternal, Array.Empty<Delegate>());
			ulong num = ConnectInterface.EOS_Connect_AddNotifyAuthExpiration(base.InnerHandle, ref addNotifyAuthExpirationOptionsInternal, zero, onAuthExpirationCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyAuthExpirationOptionsInternal>(ref addNotifyAuthExpirationOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyAuthExpiration(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			ConnectInterface.EOS_Connect_RemoveNotifyAuthExpiration(base.InnerHandle, inId);
		}

		public ulong AddNotifyLoginStatusChanged(AddNotifyLoginStatusChangedOptions options, object clientData, OnLoginStatusChangedCallback notification)
		{
			AddNotifyLoginStatusChangedOptionsInternal addNotifyLoginStatusChangedOptionsInternal = Helper.CopyProperties<AddNotifyLoginStatusChangedOptionsInternal>(options);
			OnLoginStatusChangedCallbackInternal onLoginStatusChangedCallbackInternal = new OnLoginStatusChangedCallbackInternal(ConnectInterface.OnLoginStatusChanged);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notification, onLoginStatusChangedCallbackInternal, Array.Empty<Delegate>());
			ulong num = ConnectInterface.EOS_Connect_AddNotifyLoginStatusChanged(base.InnerHandle, ref addNotifyLoginStatusChangedOptionsInternal, zero, onLoginStatusChangedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyLoginStatusChangedOptionsInternal>(ref addNotifyLoginStatusChangedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyLoginStatusChanged(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			ConnectInterface.EOS_Connect_RemoveNotifyLoginStatusChanged(base.InnerHandle, inId);
		}

		[MonoPInvokeCallback]
		internal static void OnLoginStatusChanged(IntPtr address)
		{
			OnLoginStatusChangedCallback onLoginStatusChangedCallback = null;
			LoginStatusChangedCallbackInfo loginStatusChangedCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLoginStatusChangedCallback, LoginStatusChangedCallbackInfoInternal, LoginStatusChangedCallbackInfo>(address, out onLoginStatusChangedCallback, out loginStatusChangedCallbackInfo))
			{
				onLoginStatusChangedCallback(loginStatusChangedCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnAuthExpiration(IntPtr address)
		{
			OnAuthExpirationCallback onAuthExpirationCallback = null;
			AuthExpirationCallbackInfo authExpirationCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnAuthExpirationCallback, AuthExpirationCallbackInfoInternal, AuthExpirationCallbackInfo>(address, out onAuthExpirationCallback, out authExpirationCallbackInfo))
			{
				onAuthExpirationCallback(authExpirationCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryProductUserIdMappings(IntPtr address)
		{
			OnQueryProductUserIdMappingsCallback onQueryProductUserIdMappingsCallback = null;
			QueryProductUserIdMappingsCallbackInfo queryProductUserIdMappingsCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryProductUserIdMappingsCallback, QueryProductUserIdMappingsCallbackInfoInternal, QueryProductUserIdMappingsCallbackInfo>(address, out onQueryProductUserIdMappingsCallback, out queryProductUserIdMappingsCallbackInfo))
			{
				onQueryProductUserIdMappingsCallback(queryProductUserIdMappingsCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryExternalAccountMappings(IntPtr address)
		{
			OnQueryExternalAccountMappingsCallback onQueryExternalAccountMappingsCallback = null;
			QueryExternalAccountMappingsCallbackInfo queryExternalAccountMappingsCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryExternalAccountMappingsCallback, QueryExternalAccountMappingsCallbackInfoInternal, QueryExternalAccountMappingsCallbackInfo>(address, out onQueryExternalAccountMappingsCallback, out queryExternalAccountMappingsCallbackInfo))
			{
				onQueryExternalAccountMappingsCallback(queryExternalAccountMappingsCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnTransferDeviceIdAccount(IntPtr address)
		{
			OnTransferDeviceIdAccountCallback onTransferDeviceIdAccountCallback = null;
			TransferDeviceIdAccountCallbackInfo transferDeviceIdAccountCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnTransferDeviceIdAccountCallback, TransferDeviceIdAccountCallbackInfoInternal, TransferDeviceIdAccountCallbackInfo>(address, out onTransferDeviceIdAccountCallback, out transferDeviceIdAccountCallbackInfo))
			{
				onTransferDeviceIdAccountCallback(transferDeviceIdAccountCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDeleteDeviceId(IntPtr address)
		{
			OnDeleteDeviceIdCallback onDeleteDeviceIdCallback = null;
			DeleteDeviceIdCallbackInfo deleteDeviceIdCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDeleteDeviceIdCallback, DeleteDeviceIdCallbackInfoInternal, DeleteDeviceIdCallbackInfo>(address, out onDeleteDeviceIdCallback, out deleteDeviceIdCallbackInfo))
			{
				onDeleteDeviceIdCallback(deleteDeviceIdCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnCreateDeviceId(IntPtr address)
		{
			OnCreateDeviceIdCallback onCreateDeviceIdCallback = null;
			CreateDeviceIdCallbackInfo createDeviceIdCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnCreateDeviceIdCallback, CreateDeviceIdCallbackInfoInternal, CreateDeviceIdCallbackInfo>(address, out onCreateDeviceIdCallback, out createDeviceIdCallbackInfo))
			{
				onCreateDeviceIdCallback(createDeviceIdCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnUnlinkAccount(IntPtr address)
		{
			OnUnlinkAccountCallback onUnlinkAccountCallback = null;
			UnlinkAccountCallbackInfo unlinkAccountCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnUnlinkAccountCallback, UnlinkAccountCallbackInfoInternal, UnlinkAccountCallbackInfo>(address, out onUnlinkAccountCallback, out unlinkAccountCallbackInfo))
			{
				onUnlinkAccountCallback(unlinkAccountCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLinkAccount(IntPtr address)
		{
			OnLinkAccountCallback onLinkAccountCallback = null;
			LinkAccountCallbackInfo linkAccountCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLinkAccountCallback, LinkAccountCallbackInfoInternal, LinkAccountCallbackInfo>(address, out onLinkAccountCallback, out linkAccountCallbackInfo))
			{
				onLinkAccountCallback(linkAccountCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnCreateUser(IntPtr address)
		{
			OnCreateUserCallback onCreateUserCallback = null;
			CreateUserCallbackInfo createUserCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnCreateUserCallback, CreateUserCallbackInfoInternal, CreateUserCallbackInfo>(address, out onCreateUserCallback, out createUserCallbackInfo))
			{
				onCreateUserCallback(createUserCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnLogin(IntPtr address)
		{
			OnLoginCallback onLoginCallback = null;
			LoginCallbackInfo loginCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLoginCallback, LoginCallbackInfoInternal, LoginCallbackInfo>(address, out onLoginCallback, out loginCallbackInfo))
			{
				onLoginCallback(loginCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_ExternalAccountInfo_Release(IntPtr externalAccountInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_RemoveNotifyLoginStatusChanged(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Connect_AddNotifyLoginStatusChanged(IntPtr handle, ref AddNotifyLoginStatusChangedOptionsInternal options, IntPtr clientData, OnLoginStatusChangedCallbackInternal notification);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_RemoveNotifyAuthExpiration(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Connect_AddNotifyAuthExpiration(IntPtr handle, ref AddNotifyAuthExpirationOptionsInternal options, IntPtr clientData, OnAuthExpirationCallbackInternal notification);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern LoginStatus EOS_Connect_GetLoginStatus(IntPtr handle, IntPtr localUserId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Connect_GetLoggedInUserByIndex(IntPtr handle, int index);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_Connect_GetLoggedInUsersCount(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Connect_CopyProductUserInfo(IntPtr handle, ref CopyProductUserInfoOptionsInternal options, ref IntPtr outExternalAccountInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Connect_CopyProductUserExternalAccountByAccountId(IntPtr handle, ref CopyProductUserExternalAccountByAccountIdOptionsInternal options, ref IntPtr outExternalAccountInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Connect_CopyProductUserExternalAccountByAccountType(IntPtr handle, ref CopyProductUserExternalAccountByAccountTypeOptionsInternal options, ref IntPtr outExternalAccountInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Connect_CopyProductUserExternalAccountByIndex(IntPtr handle, ref CopyProductUserExternalAccountByIndexOptionsInternal options, ref IntPtr outExternalAccountInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Connect_GetProductUserExternalAccountCount(IntPtr handle, ref GetProductUserExternalAccountCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Connect_GetProductUserIdMapping(IntPtr handle, ref GetProductUserIdMappingOptionsInternal options, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Connect_GetExternalAccountMapping(IntPtr handle, ref GetExternalAccountMappingsOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_QueryProductUserIdMappings(IntPtr handle, ref QueryProductUserIdMappingsOptionsInternal options, IntPtr clientData, OnQueryProductUserIdMappingsCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_QueryExternalAccountMappings(IntPtr handle, ref QueryExternalAccountMappingsOptionsInternal options, IntPtr clientData, OnQueryExternalAccountMappingsCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_TransferDeviceIdAccount(IntPtr handle, ref TransferDeviceIdAccountOptionsInternal options, IntPtr clientData, OnTransferDeviceIdAccountCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_DeleteDeviceId(IntPtr handle, ref DeleteDeviceIdOptionsInternal options, IntPtr clientData, OnDeleteDeviceIdCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_CreateDeviceId(IntPtr handle, ref CreateDeviceIdOptionsInternal options, IntPtr clientData, OnCreateDeviceIdCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_UnlinkAccount(IntPtr handle, ref UnlinkAccountOptionsInternal options, IntPtr clientData, OnUnlinkAccountCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_LinkAccount(IntPtr handle, ref LinkAccountOptionsInternal options, IntPtr clientData, OnLinkAccountCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_CreateUser(IntPtr handle, ref CreateUserOptionsInternal options, IntPtr clientData, OnCreateUserCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Connect_Login(IntPtr handle, ref LoginOptionsInternal options, IntPtr clientData, OnLoginCallbackInternal completionDelegate);

		public const int AddnotifyloginstatuschangedApiLatest = 1;

		public const int OnauthexpirationcallbackApiLatest = 1;

		public const int AddnotifyauthexpirationApiLatest = 1;

		public const int ExternalaccountinfoApiLatest = 1;

		public const int TimeUndefined = -1;

		public const int CopyproductuserinfoApiLatest = 1;

		public const int CopyproductuserexternalaccountbyaccountidApiLatest = 1;

		public const int CopyproductuserexternalaccountbyaccounttypeApiLatest = 1;

		public const int CopyproductuserexternalaccountbyindexApiLatest = 1;

		public const int GetproductuserexternalaccountcountApiLatest = 1;

		public const int GetproductuseridmappingApiLatest = 1;

		public const int QueryproductuseridmappingsApiLatest = 2;

		public const int GetexternalaccountmappingsApiLatest = 1;

		public const int GetexternalaccountmappingApiLatest = 1;

		public const int QueryexternalaccountmappingsMaxAccountIds = 128;

		public const int QueryexternalaccountmappingsApiLatest = 1;

		public const int TransferdeviceidaccountApiLatest = 1;

		public const int DeletedeviceidApiLatest = 1;

		public const int CreatedeviceidDevicemodelMaxLength = 64;

		public const int CreatedeviceidApiLatest = 1;

		public const int UnlinkaccountApiLatest = 1;

		public const int LinkaccountApiLatest = 1;

		public const int CreateuserApiLatest = 1;

		public const int LoginApiLatest = 2;

		public const int UserlogininfoApiLatest = 1;

		public const int UserlogininfoDisplaynameMaxLength = 32;

		public const int CredentialsApiLatest = 1;

		public const int ExternalAccountIdMaxLength = 256;
	}
}
