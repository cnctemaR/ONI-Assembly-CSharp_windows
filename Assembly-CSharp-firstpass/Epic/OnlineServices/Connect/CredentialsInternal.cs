using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
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

		public ExternalCredentialType Type
		{
			get
			{
				ExternalCredentialType @default = Helper.GetDefault<ExternalCredentialType>();
				Helper.TryMarshalGet<ExternalCredentialType>(this.m_Type, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ExternalCredentialType>(ref this.m_Type, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Token;

		private ExternalCredentialType m_Type;
	}
}
