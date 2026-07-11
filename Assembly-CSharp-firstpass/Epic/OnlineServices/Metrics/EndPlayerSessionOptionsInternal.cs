using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Metrics
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct EndPlayerSessionOptionsInternal : IDisposable
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

		public EndPlayerSessionOptionsAccountIdInternal AccountId
		{
			get
			{
				EndPlayerSessionOptionsAccountIdInternal @default = Helper.GetDefault<EndPlayerSessionOptionsAccountIdInternal>();
				Helper.TryMarshalGet<EndPlayerSessionOptionsAccountIdInternal>(this.m_AccountId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<EndPlayerSessionOptionsAccountIdInternal>(ref this.m_AccountId, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose<EndPlayerSessionOptionsAccountIdInternal>(ref this.m_AccountId);
		}

		private int m_ApiVersion;

		private EndPlayerSessionOptionsAccountIdInternal m_AccountId;
	}
}
