using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct VerifyUserAuthOptionsInternal : IDisposable
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

		public TokenInternal? AuthToken
		{
			get
			{
				TokenInternal? @default = Helper.GetDefault<TokenInternal?>();
				Helper.TryMarshalGet<TokenInternal>(this.m_AuthToken, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<TokenInternal>(ref this.m_AuthToken, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_AuthToken);
		}

		private int m_ApiVersion;

		private IntPtr m_AuthToken;
	}
}
