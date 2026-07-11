using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CreateLobbyOptionsInternal : IDisposable
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

		public uint MaxLobbyMembers
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_MaxLobbyMembers, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_MaxLobbyMembers, value);
			}
		}

		public LobbyPermissionLevel PermissionLevel
		{
			get
			{
				LobbyPermissionLevel @default = Helper.GetDefault<LobbyPermissionLevel>();
				Helper.TryMarshalGet<LobbyPermissionLevel>(this.m_PermissionLevel, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<LobbyPermissionLevel>(ref this.m_PermissionLevel, value);
			}
		}

		public bool PresenceEnabled
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_PresenceEnabled, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_PresenceEnabled, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private uint m_MaxLobbyMembers;

		private LobbyPermissionLevel m_PermissionLevel;

		private int m_PresenceEnabled;
	}
}
