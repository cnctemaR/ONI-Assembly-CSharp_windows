using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CatalogOfferInternal : IDisposable
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

		public int ServerIndex
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_ServerIndex, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_ServerIndex, value);
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

		public string TechnicalDetailsText_DEPRECATED
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_TechnicalDetailsText_DEPRECATED, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_TechnicalDetailsText_DEPRECATED, value);
			}
		}

		public string CurrencyCode
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_CurrencyCode, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_CurrencyCode, value);
			}
		}

		public Result PriceResult
		{
			get
			{
				Result @default = Helper.GetDefault<Result>();
				Helper.TryMarshalGet<Result>(this.m_PriceResult, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<Result>(ref this.m_PriceResult, value);
			}
		}

		public uint OriginalPrice
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_OriginalPrice, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_OriginalPrice, value);
			}
		}

		public uint CurrentPrice
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_CurrentPrice, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_CurrentPrice, value);
			}
		}

		public byte DiscountPercentage
		{
			get
			{
				byte @default = Helper.GetDefault<byte>();
				Helper.TryMarshalGet<byte>(this.m_DiscountPercentage, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<byte>(ref this.m_DiscountPercentage, value);
			}
		}

		public long ExpirationTimestamp
		{
			get
			{
				long @default = Helper.GetDefault<long>();
				Helper.TryMarshalGet<long>(this.m_ExpirationTimestamp, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<long>(ref this.m_ExpirationTimestamp, value);
			}
		}

		public uint PurchasedCount
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_PurchasedCount, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_PurchasedCount, value);
			}
		}

		public int PurchaseLimit
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_PurchaseLimit, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_PurchaseLimit, value);
			}
		}

		public bool AvailableForPurchase
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_AvailableForPurchase, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_AvailableForPurchase, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private int m_ServerIndex;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CatalogNamespace;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Id;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TitleText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DescriptionText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LongDescriptionText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TechnicalDetailsText_DEPRECATED;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CurrencyCode;

		private Result m_PriceResult;

		private uint m_OriginalPrice;

		private uint m_CurrentPrice;

		private byte m_DiscountPercentage;

		private long m_ExpirationTimestamp;

		private uint m_PurchasedCount;

		private int m_PurchaseLimit;

		private int m_AvailableForPurchase;
	}
}
