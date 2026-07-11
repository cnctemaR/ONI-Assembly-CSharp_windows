using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public class RegionInfo
	{
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
				this.lcid = name.GetHashCode();
				return;
			}
			if (!this.GetByTerritory(CultureInfo.GetCultureInfo(name)))
			{
				throw new ArgumentException(string.Format("Region name {0} is not supported.", name), "name");
			}
		}

		public static RegionInfo CurrentRegion
		{
			get
			{
				if (RegionInfo.currentRegion == null)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					if (currentCulture == null || CultureInfo.BootstrapCultureID == 127)
					{
						return null;
					}
					RegionInfo.currentRegion = new RegionInfo(CultureInfo.BootstrapCultureID);
				}
				return RegionInfo.currentRegion;
			}
		}

		private bool GetByTerritory(CultureInfo ci)
		{
			if (ci == null)
			{
				throw new Exception("INTERNAL ERROR: should not happen.");
			}
			if (ci.IsNeutralCulture || ci.Territory == null)
			{
				return false;
			}
			this.lcid = ci.LCID;
			return this.construct_internal_region_from_name(ci.Territory.ToUpperInvariant());
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
				if (text != null)
				{
					if (RegionInfo.<>f__switch$map1D == null)
					{
						RegionInfo.<>f__switch$map1D = new Dictionary<string, int>(2)
						{
							{ "US", 0 },
							{ "UK", 0 }
						};
					}
					int num;
					if (RegionInfo.<>f__switch$map1D.TryGetValue(text, out num))
					{
						if (num == 0)
						{
							return false;
						}
					}
				}
				return true;
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
				return this.DisplayName;
			}
		}

		[MonoTODO("Not implemented")]
		[ComVisible(false)]
		public virtual string CurrencyNativeName
		{
			get
			{
				throw new NotImplementedException();
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
			return regionInfo != null && this.lcid == regionInfo.lcid;
		}

		public override int GetHashCode()
		{
			return (int)((ulong)int.MinValue + (ulong)((long)((long)this.regionId << 3)) + (ulong)((long)this.regionId));
		}

		public override string ToString()
		{
			return this.Name;
		}

		private static RegionInfo currentRegion;

		private int lcid;

		private int regionId;

		private string iso2Name;

		private string iso3Name;

		private string win3Name;

		private string englishName;

		private string currencySymbol;

		private string isoCurrencySymbol;

		private string currencyEnglishName;
	}
}
