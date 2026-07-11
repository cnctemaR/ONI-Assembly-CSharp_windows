using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ClientCredentialsInternal : IDisposable
	{
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

		public string ClientSecret
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ClientSecret, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ClientSecret, value);
			}
		}

		public void Dispose()
		{
		}

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ClientId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ClientSecret;
	}
}
