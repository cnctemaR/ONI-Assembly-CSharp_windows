using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionDetailsSettingsInternal : IDisposable
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

		public string BucketId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_BucketId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_BucketId, value);
			}
		}

		public uint NumPublicConnections
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_NumPublicConnections, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_NumPublicConnections, value);
			}
		}

		public bool AllowJoinInProgress
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_AllowJoinInProgress, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_AllowJoinInProgress, value);
			}
		}

		public OnlineSessionPermissionLevel PermissionLevel
		{
			get
			{
				OnlineSessionPermissionLevel @default = Helper.GetDefault<OnlineSessionPermissionLevel>();
				Helper.TryMarshalGet<OnlineSessionPermissionLevel>(this.m_PermissionLevel, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<OnlineSessionPermissionLevel>(ref this.m_PermissionLevel, value);
			}
		}

		public bool InvitesAllowed
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_InvitesAllowed, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_InvitesAllowed, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_BucketId;

		private uint m_NumPublicConnections;

		private int m_AllowJoinInProgress;

		private OnlineSessionPermissionLevel m_PermissionLevel;

		private int m_InvitesAllowed;
	}
}
