using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Stats
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct IngestStatOptionsInternal : IDisposable
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

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_LocalUserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_LocalUserId, value);
			}
		}

		public IngestDataInternal[] Stats
		{
			get
			{
				IngestDataInternal[] @default = Helper.GetDefault<IngestDataInternal[]>();
				Helper.TryMarshalGet<IngestDataInternal>(this.m_Stats, out @default, this.m_StatsCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<IngestDataInternal>(ref this.m_Stats, value, out this.m_StatsCount);
			}
		}

		public ProductUserId TargetUserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_TargetUserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_TargetUserId, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_Stats);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_Stats;

		private uint m_StatsCount;

		private IntPtr m_TargetUserId;
	}
}
