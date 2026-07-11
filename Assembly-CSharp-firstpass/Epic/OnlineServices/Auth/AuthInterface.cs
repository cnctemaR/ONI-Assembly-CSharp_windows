using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	public sealed class AuthInterface : Handle
	{
		public AuthInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void Login(LoginOptions options, object clientData, OnLoginCallback completionDelegate)
		{
			LoginOptionsInternal loginOptionsInternal = Helper.CopyProperties<LoginOptionsInternal>(options);
			OnLoginCallbackInternal onLoginCallbackInternal = new OnLoginCallbackInternal(AuthInterface.OnLogin);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onLoginCallbackInternal, Array.Empty<Delegate>());
			AuthInterface.EOS_Auth_Login(base.InnerHandle, ref loginOptionsInternal, zero, onLoginCallbackInternal);
			Helper.TryMarshalDispose<LoginOptionsInternal>(ref loginOptionsInternal);
		}

		public void Logout(LogoutOptions options, object clientData, OnLogoutCallback completionDelegate)
		{
			LogoutOptionsInternal logoutOptionsInternal = Helper.CopyProperties<LogoutOptionsInternal>(options);
			OnLogoutCallbackInternal onLogoutCallbackInternal = new OnLogoutCallbackInternal(AuthInterface.OnLogout);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onLogoutCallbackInternal, Array.Empty<Delegate>());
			AuthInterface.EOS_Auth_Logout(base.InnerHandle, ref logoutOptionsInternal, zero, onLogoutCallbackInternal);
			Helper.TryMarshalDispose<LogoutOptionsInternal>(ref logoutOptionsInternal);
		}

		public void LinkAccount(LinkAccountOptions options, object clientData, OnLinkAccountCallback completionDelegate)
		{
			LinkAccountOptionsInternal linkAccountOptionsInternal = Helper.CopyProperties<LinkAccountOptionsInternal>(options);
			OnLinkAccountCallbackInternal onLinkAccountCallbackInternal = new OnLinkAccountCallbackInternal(AuthInterface.OnLinkAccount);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onLinkAccountCallbackInternal, Array.Empty<Delegate>());
			AuthInterface.EOS_Auth_LinkAccount(base.InnerHandle, ref linkAccountOptionsInternal, zero, onLinkAccountCallbackInternal);
			Helper.TryMarshalDispose<LinkAccountOptionsInternal>(ref linkAccountOptionsInternal);
		}

		public void DeletePersistentAuth(DeletePersistentAuthOptions options, object clientData, OnDeletePersistentAuthCallback completionDelegate)
		{
			DeletePersistentAuthOptionsInternal deletePersistentAuthOptionsInternal = Helper.CopyProperties<DeletePersistentAuthOptionsInternal>(options);
			OnDeletePersistentAuthCallbackInternal onDeletePersistentAuthCallbackInternal = new OnDeletePersistentAuthCallbackInternal(AuthInterface.OnDeletePersistentAuth);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onDeletePersistentAuthCallbackInternal, Array.Empty<Delegate>());
			AuthInterface.EOS_Auth_DeletePersistentAuth(base.InnerHandle, ref deletePersistentAuthOptionsInternal, zero, onDeletePersistentAuthCallbackInternal);
			Helper.TryMarshalDispose<DeletePersistentAuthOptionsInternal>(ref deletePersistentAuthOptionsInternal);
		}

		public void VerifyUserAuth(VerifyUserAuthOptions options, object clientData, OnVerifyUserAuthCallback completionDelegate)
		{
			VerifyUserAuthOptionsInternal verifyUserAuthOptionsInternal = Helper.CopyProperties<VerifyUserAuthOptionsInternal>(options);
			OnVerifyUserAuthCallbackInternal onVerifyUserAuthCallbackInternal = new OnVerifyUserAuthCallbackInternal(AuthInterface.OnVerifyUserAuth);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onVerifyUserAuthCallbackInternal, Array.Empty<Delegate>());
			AuthInterface.EOS_Auth_VerifyUserAuth(base.InnerHandle, ref verifyUserAuthOptionsInternal, zero, onVerifyUserAuthCallbackInternal);
			Helper.TryMarshalDispose<VerifyUserAuthOptionsInternal>(ref verifyUserAuthOptionsInternal);
		}

		public int GetLoggedInAccountsCount()
		{
			int num = AuthInterface.EOS_Auth_GetLoggedInAccountsCount(base.InnerHandle);
			int @default = Helper.GetDefault<int>();
			Helper.TryMarshalGet<int>(num, out @default);
			return @default;
		}

		public EpicAccountId GetLoggedInAccountByIndex(int index)
		{
			IntPtr intPtr = AuthInterface.EOS_Auth_GetLoggedInAccountByIndex(base.InnerHandle, index);
			EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
			Helper.TryMarshalGet<EpicAccountId>(intPtr, out @default);
			return @default;
		}

		public LoginStatus GetLoginStatus(EpicAccountId localUserId)
		{
			LoginStatus loginStatus = AuthInterface.EOS_Auth_GetLoginStatus(base.InnerHandle, localUserId.InnerHandle);
			LoginStatus @default = Helper.GetDefault<LoginStatus>();
			Helper.TryMarshalGet<LoginStatus>(loginStatus, out @default);
			return @default;
		}

		public Result CopyUserAuthToken(CopyUserAuthTokenOptions options, EpicAccountId localUserId, out Token outUserAuthToken)
		{
			CopyUserAuthTokenOptionsInternal copyUserAuthTokenOptionsInternal = Helper.CopyProperties<CopyUserAuthTokenOptionsInternal>(options);
			outUserAuthToken = Helper.GetDefault<Token>();
			IntPtr zero = IntPtr.Zero;
			Result result = AuthInterface.EOS_Auth_CopyUserAuthToken(base.InnerHandle, ref copyUserAuthTokenOptionsInternal, localUserId.InnerHandle, ref zero);
			Helper.TryMarshalDispose<CopyUserAuthTokenOptionsInternal>(ref copyUserAuthTokenOptionsInternal);
			if (Helper.TryMarshalGet<TokenInternal, Token>(zero, out outUserAuthToken))
			{
				AuthInterface.EOS_Auth_Token_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public ulong AddNotifyLoginStatusChanged(AddNotifyLoginStatusChangedOptions options, object clientData, OnLoginStatusChangedCallback notification)
		{
			AddNotifyLoginStatusChangedOptionsInternal addNotifyLoginStatusChangedOptionsInternal = Helper.CopyProperties<AddNotifyLoginStatusChangedOptionsInternal>(options);
			OnLoginStatusChangedCallbackInternal onLoginStatusChangedCallbackInternal = new OnLoginStatusChangedCallbackInternal(AuthInterface.OnLoginStatusChanged);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, notification, onLoginStatusChangedCallbackInternal, Array.Empty<Delegate>());
			ulong num = AuthInterface.EOS_Auth_AddNotifyLoginStatusChanged(base.InnerHandle, ref addNotifyLoginStatusChangedOptionsInternal, zero, onLoginStatusChangedCallbackInternal);
			Helper.TryMarshalDispose<AddNotifyLoginStatusChangedOptionsInternal>(ref addNotifyLoginStatusChangedOptionsInternal);
			Helper.TryAssignNotificationIdToCallback(zero, num);
			ulong @default = Helper.GetDefault<ulong>();
			Helper.TryMarshalGet<ulong>(num, out @default);
			return @default;
		}

		public void RemoveNotifyLoginStatusChanged(ulong inId)
		{
			Helper.TryRemoveCallbackByNotificationId(inId);
			AuthInterface.EOS_Auth_RemoveNotifyLoginStatusChanged(base.InnerHandle, inId);
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
		internal static void OnVerifyUserAuth(IntPtr address)
		{
			OnVerifyUserAuthCallback onVerifyUserAuthCallback = null;
			VerifyUserAuthCallbackInfo verifyUserAuthCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnVerifyUserAuthCallback, VerifyUserAuthCallbackInfoInternal, VerifyUserAuthCallbackInfo>(address, out onVerifyUserAuthCallback, out verifyUserAuthCallbackInfo))
			{
				onVerifyUserAuthCallback(verifyUserAuthCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDeletePersistentAuth(IntPtr address)
		{
			OnDeletePersistentAuthCallback onDeletePersistentAuthCallback = null;
			DeletePersistentAuthCallbackInfo deletePersistentAuthCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDeletePersistentAuthCallback, DeletePersistentAuthCallbackInfoInternal, DeletePersistentAuthCallbackInfo>(address, out onDeletePersistentAuthCallback, out deletePersistentAuthCallbackInfo))
			{
				onDeletePersistentAuthCallback(deletePersistentAuthCallbackInfo);
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
		internal static void OnLogout(IntPtr address)
		{
			OnLogoutCallback onLogoutCallback = null;
			LogoutCallbackInfo logoutCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnLogoutCallback, LogoutCallbackInfoInternal, LogoutCallbackInfo>(address, out onLogoutCallback, out logoutCallbackInfo))
			{
				onLogoutCallback(logoutCallbackInfo);
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
		private static extern void EOS_Auth_Token_Release(IntPtr authToken);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Auth_RemoveNotifyLoginStatusChanged(IntPtr handle, ulong inId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern ulong EOS_Auth_AddNotifyLoginStatusChanged(IntPtr handle, ref AddNotifyLoginStatusChangedOptionsInternal options, IntPtr clientData, OnLoginStatusChangedCallbackInternal notification);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Auth_CopyUserAuthToken(IntPtr handle, ref CopyUserAuthTokenOptionsInternal options, IntPtr localUserId, ref IntPtr outUserAuthToken);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern LoginStatus EOS_Auth_GetLoginStatus(IntPtr handle, IntPtr localUserId);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_Auth_GetLoggedInAccountByIndex(IntPtr handle, int index);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_Auth_GetLoggedInAccountsCount(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Auth_VerifyUserAuth(IntPtr handle, ref VerifyUserAuthOptionsInternal options, IntPtr clientData, OnVerifyUserAuthCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Auth_DeletePersistentAuth(IntPtr handle, ref DeletePersistentAuthOptionsInternal options, IntPtr clientData, OnDeletePersistentAuthCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Auth_LinkAccount(IntPtr handle, ref LinkAccountOptionsInternal options, IntPtr clientData, OnLinkAccountCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Auth_Logout(IntPtr handle, ref LogoutOptionsInternal options, IntPtr clientData, OnLogoutCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Auth_Login(IntPtr handle, ref LoginOptionsInternal options, IntPtr clientData, OnLoginCallbackInternal completionDelegate);

		public const int DeletepersistentauthApiLatest = 2;

		public const int AddnotifyloginstatuschangedApiLatest = 1;

		public const int CopyuserauthtokenApiLatest = 1;

		public const int VerifyuserauthApiLatest = 1;

		public const int LinkaccountApiLatest = 1;

		public const int LogoutApiLatest = 1;

		public const int LoginApiLatest = 2;

		public const int AccountfeaturerestrictedinfoApiLatest = 1;

		public const int PingrantinfoApiLatest = 2;

		public const int CredentialsApiLatest = 3;

		public const int TokenApiLatest = 2;
	}
}
