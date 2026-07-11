using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetPortRangeOptionsInternal : IDisposable
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

		public ushort Port
		{
			get
			{
				ushort @default = Helper.GetDefault<ushort>();
				Helper.TryMarshalGet<ushort>(this.m_Port, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ushort>(ref this.m_Port, value);
			}
		}

		public ushort MaxAdditionalPortsToTry
		{
			get
			{
				ushort @default = Helper.GetDefault<ushort>();
				Helper.TryMarshalGet<ushort>(this.m_MaxAdditionalPortsToTry, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ushort>(ref this.m_MaxAdditionalPortsToTry, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private ushort m_Port;

		private ushort m_MaxAdditionalPortsToTry;
	}
}
