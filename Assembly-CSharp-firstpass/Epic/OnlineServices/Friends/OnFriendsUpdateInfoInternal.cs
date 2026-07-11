using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Friends
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnFriendsUpdateInfoInternal : ICallbackInfo
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

		public EpicAccountId TargetUserId
		{
			get
			{
				EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet<EpicAccountId>(this.m_TargetUserId, out @default);
				return @default;
			}
		}

		public FriendsStatus PreviousStatus
		{
			get
			{
				FriendsStatus @default = Helper.GetDefault<FriendsStatus>();
				Helper.TryMarshalGet<FriendsStatus>(this.m_PreviousStatus, out @default);
				return @default;
			}
		}

		public FriendsStatus CurrentStatus
		{
			get
			{
				FriendsStatus @default = Helper.GetDefault<FriendsStatus>();
				Helper.TryMarshalGet<FriendsStatus>(this.m_CurrentStatus, out @default);
				return @default;
			}
		}

		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private IntPtr m_TargetUserId;

		private FriendsStatus m_PreviousStatus;

		private FriendsStatus m_CurrentStatus;
	}
}
