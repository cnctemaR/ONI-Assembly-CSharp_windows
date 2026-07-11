using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Metrics
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct BeginPlayerSessionOptionsInternal : IDisposable
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

		public BeginPlayerSessionOptionsAccountIdInternal AccountId
		{
			get
			{
				BeginPlayerSessionOptionsAccountIdInternal @default = Helper.GetDefault<BeginPlayerSessionOptionsAccountIdInternal>();
				Helper.TryMarshalGet<BeginPlayerSessionOptionsAccountIdInternal>(this.m_AccountId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<BeginPlayerSessionOptionsAccountIdInternal>(ref this.m_AccountId, value);
			}
		}

		public string DisplayName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_DisplayName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_DisplayName, value);
			}
		}

		public UserControllerType ControllerType
		{
			get
			{
				UserControllerType @default = Helper.GetDefault<UserControllerType>();
				Helper.TryMarshalGet<UserControllerType>(this.m_ControllerType, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<UserControllerType>(ref this.m_ControllerType, value);
			}
		}

		public string ServerIp
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ServerIp, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ServerIp, value);
			}
		}

		public string GameSessionId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_GameSessionId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_GameSessionId, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose<BeginPlayerSessionOptionsAccountIdInternal>(ref this.m_AccountId);
		}

		private int m_ApiVersion;

		private BeginPlayerSessionOptionsAccountIdInternal m_AccountId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DisplayName;

		private UserControllerType m_ControllerType;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ServerIp;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_GameSessionId;
	}
}
