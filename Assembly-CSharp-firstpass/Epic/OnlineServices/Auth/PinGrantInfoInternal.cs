using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PinGrantInfoInternal : IDisposable
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

		public string UserCode
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_UserCode, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_UserCode, value);
			}
		}

		public string VerificationURI
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_VerificationURI, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_VerificationURI, value);
			}
		}

		public int ExpiresIn
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_ExpiresIn, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_ExpiresIn, value);
			}
		}

		public string VerificationURIComplete
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_VerificationURIComplete, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_VerificationURIComplete, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UserCode;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_VerificationURI;

		private int m_ExpiresIn;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_VerificationURIComplete;
	}
}
