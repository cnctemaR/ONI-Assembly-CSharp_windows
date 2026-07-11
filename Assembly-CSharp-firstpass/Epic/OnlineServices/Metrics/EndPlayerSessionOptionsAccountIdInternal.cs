using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Metrics
{
	[StructLayout(LayoutKind.Explicit, Pack = 4)]
	internal struct EndPlayerSessionOptionsAccountIdInternal : IDisposable
	{
		public MetricsAccountIdType AccountIdType
		{
			get
			{
				MetricsAccountIdType @default = Helper.GetDefault<MetricsAccountIdType>();
				Helper.TryMarshalGet<MetricsAccountIdType>(this.m_AccountIdType, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<MetricsAccountIdType>(ref this.m_AccountIdType, value);
			}
		}

		public EpicAccountId Epic
		{
			get
			{
				EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet<EpicAccountId, MetricsAccountIdType>(this.m_Epic, out @default, this.m_AccountIdType, MetricsAccountIdType.Epic);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<EpicAccountId, MetricsAccountIdType>(ref this.m_Epic, value, ref this.m_AccountIdType, MetricsAccountIdType.Epic, this);
			}
		}

		public string External
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<MetricsAccountIdType>(this.m_External, out @default, this.m_AccountIdType, MetricsAccountIdType.External);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<MetricsAccountIdType>(ref this.m_External, value, ref this.m_AccountIdType, MetricsAccountIdType.External, this);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose<MetricsAccountIdType>(ref this.m_External, this.m_AccountIdType, MetricsAccountIdType.External);
		}

		[FieldOffset(0)]
		private MetricsAccountIdType m_AccountIdType;

		[FieldOffset(4)]
		private IntPtr m_Epic;

		[FieldOffset(4)]
		private IntPtr m_External;
	}
}
