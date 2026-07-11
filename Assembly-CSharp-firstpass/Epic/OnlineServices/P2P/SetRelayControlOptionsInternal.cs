using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetRelayControlOptionsInternal : IDisposable
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

		public RelayControl RelayControl
		{
			get
			{
				RelayControl @default = Helper.GetDefault<RelayControl>();
				Helper.TryMarshalGet<RelayControl>(this.m_RelayControl, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<RelayControl>(ref this.m_RelayControl, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private RelayControl m_RelayControl;
	}
}
