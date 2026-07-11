using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CatalogItemInternal : IDisposable
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

		public string CatalogNamespace
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_CatalogNamespace, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_CatalogNamespace, value);
			}
		}

		public string Id
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Id, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_Id, value);
			}
		}

		public string EntitlementName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_EntitlementName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_EntitlementName, value);
			}
		}

		public string TitleText
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_TitleText, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_TitleText, value);
			}
		}

		public string DescriptionText
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_DescriptionText, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_DescriptionText, value);
			}
		}

		public string LongDescriptionText
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_LongDescriptionText, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_LongDescriptionText, value);
			}
		}

		public string TechnicalDetailsText
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_TechnicalDetailsText, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_TechnicalDetailsText, value);
			}
		}

		public string DeveloperText
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_DeveloperText, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_DeveloperText, value);
			}
		}

		public EcomItemType ItemType
		{
			get
			{
				EcomItemType @default = Helper.GetDefault<EcomItemType>();
				Helper.TryMarshalGet<EcomItemType>(this.m_ItemType, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<EcomItemType>(ref this.m_ItemType, value);
			}
		}

		public long EntitlementEndTimestamp
		{
			get
			{
				long @default = Helper.GetDefault<long>();
				Helper.TryMarshalGet<long>(this.m_EntitlementEndTimestamp, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<long>(ref this.m_EntitlementEndTimestamp, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CatalogNamespace;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Id;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_EntitlementName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TitleText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DescriptionText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LongDescriptionText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TechnicalDetailsText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DeveloperText;

		private EcomItemType m_ItemType;

		private long m_EntitlementEndTimestamp;
	}
}
