using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationAddAttributeOptionsInternal : IDisposable
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

		public AttributeDataInternal? SessionAttribute
		{
			get
			{
				AttributeDataInternal? @default = Helper.GetDefault<AttributeDataInternal?>();
				Helper.TryMarshalGet<AttributeDataInternal>(this.m_SessionAttribute, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<AttributeDataInternal>(ref this.m_SessionAttribute, value);
			}
		}

		public SessionAttributeAdvertisementType AdvertisementType
		{
			get
			{
				SessionAttributeAdvertisementType @default = Helper.GetDefault<SessionAttributeAdvertisementType>();
				Helper.TryMarshalGet<SessionAttributeAdvertisementType>(this.m_AdvertisementType, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<SessionAttributeAdvertisementType>(ref this.m_AdvertisementType, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_SessionAttribute);
		}

		private int m_ApiVersion;

		private IntPtr m_SessionAttribute;

		private SessionAttributeAdvertisementType m_AdvertisementType;
	}
}
