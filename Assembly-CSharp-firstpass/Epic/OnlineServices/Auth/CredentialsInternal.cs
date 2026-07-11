using System;
using System.Runtime.InteropServices;
using Epic.OnlineServices.Connect;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CredentialsInternal : IDisposable
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

		public string Id
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Id, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_Id, value);
			}
		}

		public string Token
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Token, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_Token, value);
			}
		}

		public LoginCredentialType Type
		{
			get
			{
				LoginCredentialType @default = Helper.GetDefault<LoginCredentialType>();
				Helper.TryMarshalGet<LoginCredentialType>(this.m_Type, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<LoginCredentialType>(ref this.m_Type, value);
			}
		}

		public IntPtr SystemAuthCredentialsOptions
		{
			get
			{
				IntPtr @default = Helper.GetDefault<IntPtr>();
				Helper.TryMarshalGet(this.m_SystemAuthCredentialsOptions, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<IntPtr>(ref this.m_SystemAuthCredentialsOptions, value);
			}
		}

		public ExternalCredentialType ExternalType
		{
			get
			{
				ExternalCredentialType @default = Helper.GetDefault<ExternalCredentialType>();
				Helper.TryMarshalGet<ExternalCredentialType>(this.m_ExternalType, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ExternalCredentialType>(ref this.m_ExternalType, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Id;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Token;

		private LoginCredentialType m_Type;

		private IntPtr m_SystemAuthCredentialsOptions;

		private ExternalCredentialType m_ExternalType;
	}
}
