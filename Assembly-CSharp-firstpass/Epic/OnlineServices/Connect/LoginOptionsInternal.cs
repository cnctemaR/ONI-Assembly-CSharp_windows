using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LoginOptionsInternal : IDisposable
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

		public CredentialsInternal? Credentials
		{
			get
			{
				CredentialsInternal? @default = Helper.GetDefault<CredentialsInternal?>();
				Helper.TryMarshalGet<CredentialsInternal>(this.m_Credentials, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<CredentialsInternal>(ref this.m_Credentials, value);
			}
		}

		public UserLoginInfoInternal? UserLoginInfo
		{
			get
			{
				UserLoginInfoInternal? @default = Helper.GetDefault<UserLoginInfoInternal?>();
				Helper.TryMarshalGet<UserLoginInfoInternal>(this.m_UserLoginInfo, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<UserLoginInfoInternal>(ref this.m_UserLoginInfo, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_Credentials);
			Helper.TryMarshalDispose(ref this.m_UserLoginInfo);
		}

		private int m_ApiVersion;

		private IntPtr m_Credentials;

		private IntPtr m_UserLoginInfo;
	}
}
