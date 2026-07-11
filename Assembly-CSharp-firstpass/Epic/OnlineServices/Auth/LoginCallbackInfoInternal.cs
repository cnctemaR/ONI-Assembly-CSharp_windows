using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LoginCallbackInfoInternal : ICallbackInfo
	{
		public Result ResultCode
		{
			get
			{
				Result @default = Helper.GetDefault<Result>();
				Helper.TryMarshalGet<Result>(this.m_ResultCode, out @default);
				return @default;
			}
		}

		public object ClientData
		{
			get
			{
				object @default = Helper.GetDefault<object>();
				Helper.TryMarshalGet(this.m_ClientData, out @default);
				return @default;
			}
		}

		public IntPtr ClientDataAddress
		{
			get
			{
				return this.m_ClientData;
			}
		}

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet<EpicAccountId>(this.m_LocalUserId, out @default);
				return @default;
			}
		}

		public PinGrantInfoInternal? PinGrantInfo
		{
			get
			{
				PinGrantInfoInternal? @default = Helper.GetDefault<PinGrantInfoInternal?>();
				Helper.TryMarshalGet<PinGrantInfoInternal>(this.m_PinGrantInfo, out @default);
				return @default;
			}
		}

		public ContinuanceToken ContinuanceToken
		{
			get
			{
				ContinuanceToken @default = Helper.GetDefault<ContinuanceToken>();
				Helper.TryMarshalGet<ContinuanceToken>(this.m_ContinuanceToken, out @default);
				return @default;
			}
		}

		public AccountFeatureRestrictedInfoInternal? AccountFeatureRestrictedInfo
		{
			get
			{
				AccountFeatureRestrictedInfoInternal? @default = Helper.GetDefault<AccountFeatureRestrictedInfoInternal?>();
				Helper.TryMarshalGet<AccountFeatureRestrictedInfoInternal>(this.m_AccountFeatureRestrictedInfo, out @default);
				return @default;
			}
		}

		private Result m_ResultCode;

		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private IntPtr m_PinGrantInfo;

		private IntPtr m_ContinuanceToken;

		private IntPtr m_AccountFeatureRestrictedInfo;
	}
}
