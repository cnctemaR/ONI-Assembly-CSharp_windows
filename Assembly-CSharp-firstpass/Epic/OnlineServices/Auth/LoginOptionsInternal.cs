using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
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

		public AuthScopeFlags ScopeFlags
		{
			get
			{
				AuthScopeFlags @default = Helper.GetDefault<AuthScopeFlags>();
				Helper.TryMarshalGet<AuthScopeFlags>(this.m_ScopeFlags, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<AuthScopeFlags>(ref this.m_ScopeFlags, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_Credentials);
		}

		private int m_ApiVersion;

		private IntPtr m_Credentials;

		private AuthScopeFlags m_ScopeFlags;
	}
}
