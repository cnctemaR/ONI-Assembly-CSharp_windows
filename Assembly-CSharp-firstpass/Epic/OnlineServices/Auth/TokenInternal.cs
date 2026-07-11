using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct TokenInternal : IDisposable
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

		public string App
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_App, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_App, value);
			}
		}

		public string ClientId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ClientId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ClientId, value);
			}
		}

		public EpicAccountId AccountId
		{
			get
			{
				EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet<EpicAccountId>(this.m_AccountId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_AccountId, value);
			}
		}

		public string AccessToken
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_AccessToken, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_AccessToken, value);
			}
		}

		public double ExpiresIn
		{
			get
			{
				double @default = Helper.GetDefault<double>();
				Helper.TryMarshalGet<double>(this.m_ExpiresIn, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<double>(ref this.m_ExpiresIn, value);
			}
		}

		public string ExpiresAt
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ExpiresAt, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ExpiresAt, value);
			}
		}

		public AuthTokenType AuthType
		{
			get
			{
				AuthTokenType @default = Helper.GetDefault<AuthTokenType>();
				Helper.TryMarshalGet<AuthTokenType>(this.m_AuthType, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<AuthTokenType>(ref this.m_AuthType, value);
			}
		}

		public string RefreshToken
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_RefreshToken, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_RefreshToken, value);
			}
		}

		public double RefreshExpiresIn
		{
			get
			{
				double @default = Helper.GetDefault<double>();
				Helper.TryMarshalGet<double>(this.m_RefreshExpiresIn, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<double>(ref this.m_RefreshExpiresIn, value);
			}
		}

		public string RefreshExpiresAt
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_RefreshExpiresAt, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_RefreshExpiresAt, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_App;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ClientId;

		private IntPtr m_AccountId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AccessToken;

		private double m_ExpiresIn;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ExpiresAt;

		private AuthTokenType m_AuthType;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_RefreshToken;

		private double m_RefreshExpiresIn;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_RefreshExpiresAt;
	}
}
