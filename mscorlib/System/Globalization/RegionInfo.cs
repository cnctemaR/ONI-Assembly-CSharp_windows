using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class RegionInfo
	{
		public static RegionInfo CurrentRegion
		{
			get
			{
				RegionInfo regionInfo = RegionInfo.currentRegion;
				if (regionInfo == null)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					if (currentCulture != null)
					{
						regionInfo = new RegionInfo(currentCulture);
					}
					if (Interlocked.CompareExchange<RegionInfo>(ref RegionInfo.currentRegion, regionInfo, null) != null)
					{
						regionInfo = RegionInfo.currentRegion;
					}
				}
				return regionInfo;
			}
		}

		public RegionInfo(int culture)
		{
			if (!this.GetByTerritory(CultureInfo.GetCultureInfo(culture)))
			{
				throw new ArgumentException(string.Format("Region ID {0} (0x{0:X4}) is not a supported region.", culture), "culture");
			}
		}

		public RegionInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			if (this.construct_internal_region_from_name(name.ToUpperInvariant()))
			{
				return;
			}
			if (!this.GetByTerritory(CultureInfo.GetCultureInfo(name)))
			{
				throw new ArgumentException(string.Format("Region name {0} is not supported.", name), "name");
			}
		}

		private RegionInfo(CultureInfo ci)
		{
			if (ci.LCID == 127)
			{
				this.regionId = 244;
				this.iso2Name = "IV";
				this.iso3Name = "ivc";
				this.win3Name = "IVC";
				this.nativeName = (this.englishName = "Invariant Country");
				this.currencySymbol = "¤";
				this.isoCurrencySymbol = "XDR";
				this.currencyEnglishName = (this.currencyNativeName = "International Monetary Fund");
				return;
			}
			if (ci.Territory == null)
			{
				throw new NotImplementedException("Neutral region info");
			}
			this.construct_internal_region_from_name(ci.Territory.ToUpperInvariant());
		}

		private bool GetByTerritory(CultureInfo ci)
		{
			if (ci == null)
			{
				throw new Exception("INTERNAL ERROR: should not happen.");
			}
			return !ci.IsNeutralCulture && ci.Territory != null && this.construct_internal_region_from_name(ci.Territory.ToUpperInvariant());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool construct_internal_region_from_name(string name);

		[ComVisible(false)]
		public virtual string CurrencyEnglishName
		{
			get
			{
				return this.currencyEnglishName;
			}
		}

		public virtual string CurrencySymbol
		{
			get
			{
				return this.currencySymbol;
			}
		}

		[MonoTODO("DisplayName currently only returns the EnglishName")]
		public virtual string DisplayName
		{
			get
			{
				return this.englishName;
			}
		}

		public virtual string EnglishName
		{
			get
			{
				return this.englishName;
			}
		}

		[ComVisible(false)]
		public virtual int GeoId
		{
			get
			{
				return this.regionId;
			}
		}

		public virtual bool IsMetric
		{
			get
			{
				string text = this.iso2Name;
				return !(text == "US") && !(text == "UK");
			}
		}

		public virtual string ISOCurrencySymbol
		{
			get
			{
				return this.isoCurrencySymbol;
			}
		}

		[ComVisible(false)]
		public virtual string NativeName
		{
			get
			{
				return this.nativeName;
			}
		}

		[ComVisible(false)]
		public virtual string CurrencyNativeName
		{
			get
			{
				return this.currencyNativeName;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.iso2Name;
			}
		}

		public virtual string ThreeLetterISORegionName
		{
			get
			{
				return this.iso3Name;
			}
		}

		public virtual string ThreeLetterWindowsRegionName
		{
			get
			{
				return this.win3Name;
			}
		}

		public virtual string TwoLetterISORegionName
		{
			get
			{
				return this.iso2Name;
			}
		}

		public override bool Equals(object value)
		{
			RegionInfo regionInfo = value as RegionInfo;
			return regionInfo != null && this.Name == regionInfo.Name;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal static void ClearCachedData()
		{
			RegionInfo.currentRegion = null;
		}

		private static RegionInfo currentRegion;

		private int regionId;

		private string iso2Name;

		private string iso3Name;

		private string win3Name;

		private string englishName;

		private string nativeName;

		private string currencySymbol;

		private string isoCurrencySymbol;

		private string currencyEnglishName;

		private string currencyNativeName;
	}
}
