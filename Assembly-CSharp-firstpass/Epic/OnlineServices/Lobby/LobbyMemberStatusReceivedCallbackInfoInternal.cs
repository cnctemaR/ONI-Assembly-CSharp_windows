using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyMemberStatusReceivedCallbackInfoInternal : ICallbackInfo
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

		public string LobbyId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_LobbyId, out @default);
				return @default;
			}
		}

		public ProductUserId TargetUserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_TargetUserId, out @default);
				return @default;
			}
		}

		public LobbyMemberStatus CurrentStatus
		{
			get
			{
				LobbyMemberStatus @default = Helper.GetDefault<LobbyMemberStatus>();
				Helper.TryMarshalGet<LobbyMemberStatus>(this.m_CurrentStatus, out @default);
				return @default;
			}
		}

		private IntPtr m_ClientData;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LobbyId;

		private IntPtr m_TargetUserId;

		private LobbyMemberStatus m_CurrentStatus;
	}
}
