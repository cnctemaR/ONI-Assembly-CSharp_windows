using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PresenceModificationSetStatusOptionsInternal : IDisposable
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

		public Status Status
		{
			get
			{
				Status @default = Helper.GetDefault<Status>();
				Helper.TryMarshalGet<Status>(this.m_Status, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<Status>(ref this.m_Status, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private Status m_Status;
	}
}
