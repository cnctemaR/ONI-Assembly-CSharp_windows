using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LoginStatusChangedCallbackInfoInternal : ICallbackInfo
	{
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

		public LoginStatus PrevStatus
		{
			get
			{
				LoginStatus @default = Helper.GetDefault<LoginStatus>();
				Helper.TryMarshalGet<LoginStatus>(this.m_PrevStatus, out @default);
				return @default;
			}
		}

		public LoginStatus CurrentStatus
		{
			get
			{
				LoginStatus @default = Helper.GetDefault<LoginStatus>();
				Helper.TryMarshalGet<LoginStatus>(this.m_CurrentStatus, out @default);
				return @default;
			}
		}

		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private LoginStatus m_PrevStatus;

		private LoginStatus m_CurrentStatus;
	}
}
