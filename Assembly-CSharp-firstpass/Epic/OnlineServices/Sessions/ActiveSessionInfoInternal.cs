using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ActiveSessionInfoInternal : IDisposable
	{
		public int ApiVersion
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_ApiVersion, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_ApiVersion, value);
			}
		}

		public string SessionName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_SessionName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_SessionName, value);
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
			set
			{
				Helper.TryMarshalSet(ref this.m_LocalUserId, value);
			}
		}

		public OnlineSessionState State
		{
			get
			{
				OnlineSessionState @default = Helper.GetDefault<OnlineSessionState>();
				Helper.TryMarshalGet<OnlineSessionState>(this.m_State, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<OnlineSessionState>(ref this.m_State, value);
			}
		}

		public SessionDetailsInfoInternal? SessionDetails
		{
			get
			{
				SessionDetailsInfoInternal? @default = Helper.GetDefault<SessionDetailsInfoInternal?>();
				Helper.TryMarshalGet<SessionDetailsInfoInternal>(this.m_SessionDetails, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<SessionDetailsInfoInternal>(ref this.m_SessionDetails, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_SessionDetails);
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionName;

		private IntPtr m_LocalUserId;

		private OnlineSessionState m_State;

		private IntPtr m_SessionDetails;
	}
}
