using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LinkAccountOptionsInternal : IDisposable
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

		public LinkAccountFlags LinkAccountFlags
		{
			get
			{
				LinkAccountFlags @default = Helper.GetDefault<LinkAccountFlags>();
				Helper.TryMarshalGet<LinkAccountFlags>(this.m_LinkAccountFlags, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<LinkAccountFlags>(ref this.m_LinkAccountFlags, value);
			}
		}

		public ContinuanceToken ContinuanceToken
		{
			get
			{
				ContinuanceToken @default = Helper.GetDefault<ContinuanceToken>();
				Helper.TryMarshalGet<ContinuanceToken>(this.m_ContinuanceToken, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_ContinuanceToken, value);
			}
		}

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet<EpicAccountId>(this.m_LocalUserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_LocalUserId, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private LinkAccountFlags m_LinkAccountFlags;

		private IntPtr m_ContinuanceToken;

		private IntPtr m_LocalUserId;
	}
}
