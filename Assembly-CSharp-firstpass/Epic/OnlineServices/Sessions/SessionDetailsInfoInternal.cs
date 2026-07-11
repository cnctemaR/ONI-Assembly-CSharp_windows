using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionDetailsInfoInternal : IDisposable
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

		public string SessionId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_SessionId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_SessionId, value);
			}
		}

		public string HostAddress
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_HostAddress, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_HostAddress, value);
			}
		}

		public uint NumOpenPublicConnections
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_NumOpenPublicConnections, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_NumOpenPublicConnections, value);
			}
		}

		public SessionDetailsSettingsInternal? Settings
		{
			get
			{
				SessionDetailsSettingsInternal? @default = Helper.GetDefault<SessionDetailsSettingsInternal?>();
				Helper.TryMarshalGet<SessionDetailsSettingsInternal>(this.m_Settings, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<SessionDetailsSettingsInternal>(ref this.m_Settings, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_Settings);
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_HostAddress;

		private uint m_NumOpenPublicConnections;

		private IntPtr m_Settings;
	}
}
