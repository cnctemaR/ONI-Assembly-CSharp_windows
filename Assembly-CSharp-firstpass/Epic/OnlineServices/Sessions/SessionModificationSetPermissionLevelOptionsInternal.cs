using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationSetPermissionLevelOptionsInternal : IDisposable
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

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private OnlineSessionPermissionLevel m_PermissionLevel;
	}
}
