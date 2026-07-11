using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
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

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_LocalUserId, out @default);
				return @default;
			}
		}

		public LoginStatus PreviousStatus
		{
			get
			{
				LoginStatus @default = Helper.GetDefault<LoginStatus>();
				Helper.TryMarshalGet<LoginStatus>(this.m_PreviousStatus, out @default);
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

		private LoginStatus m_PreviousStatus;

		private LoginStatus m_CurrentStatus;
	}
}
