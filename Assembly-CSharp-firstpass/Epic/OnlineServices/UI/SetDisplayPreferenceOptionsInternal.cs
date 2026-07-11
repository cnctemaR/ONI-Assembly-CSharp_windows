using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UI
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetDisplayPreferenceOptionsInternal : IDisposable
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

		public NotificationLocation NotificationLocation
		{
			get
			{
				NotificationLocation @default = Helper.GetDefault<NotificationLocation>();
				Helper.TryMarshalGet<NotificationLocation>(this.m_NotificationLocation, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<NotificationLocation>(ref this.m_NotificationLocation, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private NotificationLocation m_NotificationLocation;
	}
}
