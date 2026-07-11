using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UserInfo
{
	public sealed class UserInfoInterface : Handle
	{
		public UserInfoInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryUserInfo(QueryUserInfoOptions options, object clientData, OnQueryUserInfoCallback completionDelegate)
		{
			QueryUserInfoOptionsInternal queryUserInfoOptionsInternal = Helper.CopyProperties<QueryUserInfoOptionsInternal>(options);
			OnQueryUserInfoCallbackInternal onQueryUserInfoCallbackInternal = new OnQueryUserInfoCallbackInternal(UserInfoInterface.OnQueryUserInfo);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryUserInfoCallbackInternal, Array.Empty<Delegate>());
			UserInfoInterface.EOS_UserInfo_QueryUserInfo(base.InnerHandle, ref queryUserInfoOptionsInternal, zero, onQueryUserInfoCallbackInternal);
			Helper.TryMarshalDispose<QueryUserInfoOptionsInternal>(ref queryUserInfoOptionsInternal);
		}

		public void QueryUserInfoByDisplayName(QueryUserInfoByDisplayNameOptions options, object clientData, OnQueryUserInfoByDisplayNameCallback completionDelegate)
		{
			QueryUserInfoByDisplayNameOptionsInternal queryUserInfoByDisplayNameOptionsInternal = Helper.CopyProperties<QueryUserInfoByDisplayNameOptionsInternal>(options);
			OnQueryUserInfoByDisplayNameCallbackInternal onQueryUserInfoByDisplayNameCallbackInternal = new OnQueryUserInfoByDisplayNameCallbackInternal(UserInfoInterface.OnQueryUserInfoByDisplayName);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryUserInfoByDisplayNameCallbackInternal, Array.Empty<Delegate>());
			UserInfoInterface.EOS_UserInfo_QueryUserInfoByDisplayName(base.InnerHandle, ref queryUserInfoByDisplayNameOptionsInternal, zero, onQueryUserInfoByDisplayNameCallbackInternal);
			Helper.TryMarshalDispose<QueryUserInfoByDisplayNameOptionsInternal>(ref queryUserInfoByDisplayNameOptionsInternal);
		}

		public void QueryUserInfoByExternalAccount(QueryUserInfoByExternalAccountOptions options, object clientData, OnQueryUserInfoByExternalAccountCallback completionDelegate)
		{
			QueryUserInfoByExternalAccountOptionsInternal queryUserInfoByExternalAccountOptionsInternal = Helper.CopyProperties<QueryUserInfoByExternalAccountOptionsInternal>(options);
			OnQueryUserInfoByExternalAccountCallbackInternal onQueryUserInfoByExternalAccountCallbackInternal = new OnQueryUserInfoByExternalAccountCallbackInternal(UserInfoInterface.OnQueryUserInfoByExternalAccount);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryUserInfoByExternalAccountCallbackInternal, Array.Empty<Delegate>());
			UserInfoInterface.EOS_UserInfo_QueryUserInfoByExternalAccount(base.InnerHandle, ref queryUserInfoByExternalAccountOptionsInternal, zero, onQueryUserInfoByExternalAccountCallbackInternal);
			Helper.TryMarshalDispose<QueryUserInfoByExternalAccountOptionsInternal>(ref queryUserInfoByExternalAccountOptionsInternal);
		}

		public Result CopyUserInfo(CopyUserInfoOptions options, out UserInfoData outUserInfo)
		{
			CopyUserInfoOptionsInternal copyUserInfoOptionsInternal = Helper.CopyProperties<CopyUserInfoOptionsInternal>(options);
			outUserInfo = Helper.GetDefault<UserInfoData>();
			IntPtr zero = IntPtr.Zero;
			Result result = UserInfoInterface.EOS_UserInfo_CopyUserInfo(base.InnerHandle, ref copyUserInfoOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyUserInfoOptionsInternal>(ref copyUserInfoOptionsInternal);
			if (Helper.TryMarshalGet<UserInfoDataInternal, UserInfoData>(zero, out outUserInfo))
			{
				UserInfoInterface.EOS_UserInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetExternalUserInfoCount(GetExternalUserInfoCountOptions options)
		{
			GetExternalUserInfoCountOptionsInternal getExternalUserInfoCountOptionsInternal = Helper.CopyProperties<GetExternalUserInfoCountOptionsInternal>(options);
			uint num = UserInfoInterface.EOS_UserInfo_GetExternalUserInfoCount(base.InnerHandle, ref getExternalUserInfoCountOptionsInternal);
			Helper.TryMarshalDispose<GetExternalUserInfoCountOptionsInternal>(ref getExternalUserInfoCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyExternalUserInfoByIndex(CopyExternalUserInfoByIndexOptions options, out ExternalUserInfo outExternalUserInfo)
		{
			CopyExternalUserInfoByIndexOptionsInternal copyExternalUserInfoByIndexOptionsInternal = Helper.CopyProperties<CopyExternalUserInfoByIndexOptionsInternal>(options);
			outExternalUserInfo = Helper.GetDefault<ExternalUserInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = UserInfoInterface.EOS_UserInfo_CopyExternalUserInfoByIndex(base.InnerHandle, ref copyExternalUserInfoByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyExternalUserInfoByIndexOptionsInternal>(ref copyExternalUserInfoByIndexOptionsInternal);
			if (Helper.TryMarshalGet<ExternalUserInfoInternal, ExternalUserInfo>(zero, out outExternalUserInfo))
			{
				UserInfoInterface.EOS_UserInfo_ExternalUserInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyExternalUserInfoByAccountType(CopyExternalUserInfoByAccountTypeOptions options, out ExternalUserInfo outExternalUserInfo)
		{
			CopyExternalUserInfoByAccountTypeOptionsInternal copyExternalUserInfoByAccountTypeOptionsInternal = Helper.CopyProperties<CopyExternalUserInfoByAccountTypeOptionsInternal>(options);
			outExternalUserInfo = Helper.GetDefault<ExternalUserInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = UserInfoInterface.EOS_UserInfo_CopyExternalUserInfoByAccountType(base.InnerHandle, ref copyExternalUserInfoByAccountTypeOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyExternalUserInfoByAccountTypeOptionsInternal>(ref copyExternalUserInfoByAccountTypeOptionsInternal);
			if (Helper.TryMarshalGet<ExternalUserInfoInternal, ExternalUserInfo>(zero, out outExternalUserInfo))
			{
				UserInfoInterface.EOS_UserInfo_ExternalUserInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyExternalUserInfoByAccountId(CopyExternalUserInfoByAccountIdOptions options, out ExternalUserInfo outExternalUserInfo)
		{
			CopyExternalUserInfoByAccountIdOptionsInternal copyExternalUserInfoByAccountIdOptionsInternal = Helper.CopyProperties<CopyExternalUserInfoByAccountIdOptionsInternal>(options);
			outExternalUserInfo = Helper.GetDefault<ExternalUserInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = UserInfoInterface.EOS_UserInfo_CopyExternalUserInfoByAccountId(base.InnerHandle, ref copyExternalUserInfoByAccountIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyExternalUserInfoByAccountIdOptionsInternal>(ref copyExternalUserInfoByAccountIdOptionsInternal);
			if (Helper.TryMarshalGet<ExternalUserInfoInternal, ExternalUserInfo>(zero, out outExternalUserInfo))
			{
				UserInfoInterface.EOS_UserInfo_ExternalUserInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnQueryUserInfoByExternalAccount(IntPtr address)
		{
			OnQueryUserInfoByExternalAccountCallback onQueryUserInfoByExternalAccountCallback = null;
			QueryUserInfoByExternalAccountCallbackInfo queryUserInfoByExternalAccountCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryUserInfoByExternalAccountCallback, QueryUserInfoByExternalAccountCallbackInfoInternal, QueryUserInfoByExternalAccountCallbackInfo>(address, out onQueryUserInfoByExternalAccountCallback, out queryUserInfoByExternalAccountCallbackInfo))
			{
				onQueryUserInfoByExternalAccountCallback(queryUserInfoByExternalAccountCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryUserInfoByDisplayName(IntPtr address)
		{
			OnQueryUserInfoByDisplayNameCallback onQueryUserInfoByDisplayNameCallback = null;
			QueryUserInfoByDisplayNameCallbackInfo queryUserInfoByDisplayNameCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryUserInfoByDisplayNameCallback, QueryUserInfoByDisplayNameCallbackInfoInternal, QueryUserInfoByDisplayNameCallbackInfo>(address, out onQueryUserInfoByDisplayNameCallback, out queryUserInfoByDisplayNameCallbackInfo))
			{
				onQueryUserInfoByDisplayNameCallback(queryUserInfoByDisplayNameCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryUserInfo(IntPtr address)
		{
			OnQueryUserInfoCallback onQueryUserInfoCallback = null;
			QueryUserInfoCallbackInfo queryUserInfoCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryUserInfoCallback, QueryUserInfoCallbackInfoInternal, QueryUserInfoCallbackInfo>(address, out onQueryUserInfoCallback, out queryUserInfoCallbackInfo))
			{
				onQueryUserInfoCallback(queryUserInfoCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_UserInfo_ExternalUserInfo_Release(IntPtr externalUserInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_UserInfo_Release(IntPtr userInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_UserInfo_CopyExternalUserInfoByAccountId(IntPtr handle, ref CopyExternalUserInfoByAccountIdOptionsInternal options, ref IntPtr outExternalUserInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_UserInfo_CopyExternalUserInfoByAccountType(IntPtr handle, ref CopyExternalUserInfoByAccountTypeOptionsInternal options, ref IntPtr outExternalUserInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_UserInfo_CopyExternalUserInfoByIndex(IntPtr handle, ref CopyExternalUserInfoByIndexOptionsInternal options, ref IntPtr outExternalUserInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_UserInfo_GetExternalUserInfoCount(IntPtr handle, ref GetExternalUserInfoCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_UserInfo_CopyUserInfo(IntPtr handle, ref CopyUserInfoOptionsInternal options, ref IntPtr outUserInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_UserInfo_QueryUserInfoByExternalAccount(IntPtr handle, ref QueryUserInfoByExternalAccountOptionsInternal options, IntPtr clientData, OnQueryUserInfoByExternalAccountCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_UserInfo_QueryUserInfoByDisplayName(IntPtr handle, ref QueryUserInfoByDisplayNameOptionsInternal options, IntPtr clientData, OnQueryUserInfoByDisplayNameCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_UserInfo_QueryUserInfo(IntPtr handle, ref QueryUserInfoOptionsInternal options, IntPtr clientData, OnQueryUserInfoCallbackInternal completionDelegate);

		public const int CopyexternaluserinfobyaccountidApiLatest = 1;

		public const int CopyexternaluserinfobyaccounttypeApiLatest = 1;

		public const int CopyexternaluserinfobyindexApiLatest = 1;

		public const int GetexternaluserinfocountApiLatest = 1;

		public const int ExternaluserinfoApiLatest = 1;

		public const int CopyuserinfoApiLatest = 2;

		public const int MaxDisplaynameUtf8Length = 64;

		public const int MaxDisplaynameCharacters = 16;

		public const int QueryuserinfobyexternalaccountApiLatest = 1;

		public const int QueryuserinfobydisplaynameApiLatest = 1;

		public const int QueryuserinfoApiLatest = 1;
	}
}
