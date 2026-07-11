using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationSetInvitesAllowedOptionsInternal : IDisposable
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

		private int m_InvitesAllowed;
	}
}
