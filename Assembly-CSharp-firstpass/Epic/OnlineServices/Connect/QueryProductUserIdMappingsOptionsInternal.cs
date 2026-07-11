using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryProductUserIdMappingsOptionsInternal : IDisposable
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

		public ExternalAccountType AccountIdType_DEPRECATED
		{
			get
			{
				ExternalAccountType @default = Helper.GetDefault<ExternalAccountType>();
				Helper.TryMarshalGet<ExternalAccountType>(this.m_AccountIdType_DEPRECATED, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ExternalAccountType>(ref this.m_AccountIdType_DEPRECATED, value);
			}
		}

		public ProductUserId[] ProductUserIds
		{
			get
			{
				ProductUserId[] @default = Helper.GetDefault<ProductUserId[]>();
				Helper.TryMarshalGet<ProductUserId>(this.m_ProductUserIds, out @default, this.m_ProductUserIdCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ProductUserId>(ref this.m_ProductUserIds, value, out this.m_ProductUserIdCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_ProductUserIds);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private ExternalAccountType m_AccountIdType_DEPRECATED;

		private IntPtr m_ProductUserIds;

		private uint m_ProductUserIdCount;
	}
}
