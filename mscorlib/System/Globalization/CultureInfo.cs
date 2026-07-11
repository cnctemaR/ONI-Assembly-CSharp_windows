using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public class CultureInfo : ICloneable, IFormatProvider
	{
		public CultureInfo(int culture)
			: this(culture, true)
		{
		}

		public CultureInfo(int culture, bool useUserOverride)
			: this(culture, useUserOverride, false)
		{
		}

		private CultureInfo(int culture, bool useUserOverride, bool read_only)
		{
			if (culture < 0)
			{
				throw new ArgumentOutOfRangeException("culture", "Positive number required.");
			}
			this.constructed = true;
			this.m_isReadOnly = read_only;
			this.m_useUserOverride = useUserOverride;
			if (culture == 127)
			{
				this.ConstructInvariant(read_only);
				return;
			}
			if (!this.ConstructInternalLocaleFromLcid(culture))
			{
				throw new ArgumentException(string.Format("Culture ID {0} (0x{0:X4}) is not a supported culture.", culture), "culture");
			}
		}

		public CultureInfo(string name)
			: this(name, true)
		{
		}

		public CultureInfo(string name, bool useUserOverride)
			: this(name, useUserOverride, false)
		{
		}

		private CultureInfo(string name, bool useUserOverride, bool read_only)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.constructed = true;
			this.m_isReadOnly = read_only;
			this.m_useUserOverride = useUserOverride;
			if (name.Length == 0)
			{
				this.ConstructInvariant(read_only);
				return;
			}
			if (!this.ConstructInternalLocaleFromName(name.ToLowerInvariant()))
			{
				throw new ArgumentException("Culture name " + name + " is not supported.", "name");
			}
		}

		private CultureInfo()
		{
			this.constructed = true;
		}

		public static CultureInfo InvariantCulture
		{
			get
			{
				return CultureInfo.invariant_culture_info;
			}
		}

		public static CultureInfo CreateSpecificCulture(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				return CultureInfo.InvariantCulture;
			}
			CultureInfo cultureInfo = new CultureInfo();
			if (!CultureInfo.ConstructInternalLocaleFromSpecificName(cultureInfo, name.ToLowerInvariant()))
			{
				throw new ArgumentException("Culture name " + name + " is not supported.", name);
			}
			return cultureInfo;
		}

		public static CultureInfo CurrentCulture
		{
			get
			{
				return Thread.CurrentThread.CurrentCulture;
			}
		}

		public static CultureInfo CurrentUICulture
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture;
			}
		}

		internal static CultureInfo ConstructCurrentCulture()
		{
			CultureInfo cultureInfo = new CultureInfo();
			if (!CultureInfo.ConstructInternalLocaleFromCurrentLocale(cultureInfo))
			{
				cultureInfo = CultureInfo.InvariantCulture;
			}
			CultureInfo.BootstrapCultureID = cultureInfo.cultureID;
			return cultureInfo;
		}

		internal static CultureInfo ConstructCurrentUICulture()
		{
			return CultureInfo.ConstructCurrentCulture();
		}

		internal string Territory
		{
			get
			{
				return this.territory;
			}
		}

		[ComVisible(false)]
		public CultureTypes CultureTypes
		{
			get
			{
				CultureTypes cultureTypes = (CultureTypes)0;
				foreach (object obj in Enum.GetValues(typeof(CultureTypes)))
				{
					CultureTypes cultureTypes2 = (CultureTypes)((int)obj);
					if (Array.IndexOf<CultureInfo>(CultureInfo.GetCultures(cultureTypes2), this) >= 0)
					{
						cultureTypes |= cultureTypes2;
					}
				}
				return cultureTypes;
			}
		}

		[ComVisible(false)]
		public CultureInfo GetConsoleFallbackUICulture()
		{
			string name = this.Name;
			if (name != null)
			{
				if (CultureInfo.<>f__switch$map1A == null)
				{
					CultureInfo.<>f__switch$map1A = new Dictionary<string, int>(48)
					{
						{ "ar", 0 },
						{ "ar-BH", 0 },
						{ "ar-EG", 0 },
						{ "ar-IQ", 0 },
						{ "ar-JO", 0 },
						{ "ar-KW", 0 },
						{ "ar-LB", 0 },
						{ "ar-LY", 0 },
						{ "ar-QA", 0 },
						{ "ar-SA", 0 },
						{ "ar-SY", 0 },
						{ "ar-AE", 0 },
						{ "ar-YE", 0 },
						{ "dv", 0 },
						{ "dv-MV", 0 },
						{ "fa", 0 },
						{ "fa-IR", 0 },
						{ "gu", 0 },
						{ "gu-IN", 0 },
						{ "he", 0 },
						{ "he-IL", 0 },
						{ "hi", 0 },
						{ "hi-IN", 0 },
						{ "kn", 0 },
						{ "kn-IN", 0 },
						{ "kok", 0 },
						{ "kok-IN", 0 },
						{ "mr", 0 },
						{ "mr-IN", 0 },
						{ "pa", 0 },
						{ "pa-IN", 0 },
						{ "sa", 0 },
						{ "sa-IN", 0 },
						{ "syr", 0 },
						{ "syr-SY", 0 },
						{ "ta", 0 },
						{ "ta-IN", 0 },
						{ "te", 0 },
						{ "te-IN", 0 },
						{ "th", 0 },
						{ "th-TH", 0 },
						{ "ur", 0 },
						{ "ur-PK", 0 },
						{ "vi", 0 },
						{ "vi-VN", 0 },
						{ "ar-DZ", 1 },
						{ "ar-MA", 1 },
						{ "ar-TN", 1 }
					};
				}
				int num;
				if (CultureInfo.<>f__switch$map1A.TryGetValue(name, out num))
				{
					if (num == 0)
					{
						return CultureInfo.GetCultureInfo("en");
					}
					if (num == 1)
					{
						return CultureInfo.GetCultureInfo("fr");
					}
				}
			}
			return ((this.CultureTypes & CultureTypes.WindowsOnlyCultures) == (CultureTypes)0) ? this : CultureInfo.InvariantCulture;
		}

		[ComVisible(false)]
		public string IetfLanguageTag
		{
			get
			{
				string name = this.Name;
				if (name != null)
				{
					if (CultureInfo.<>f__switch$map19 == null)
					{
						CultureInfo.<>f__switch$map19 = new Dictionary<string, int>(2)
						{
							{ "zh-CHS", 0 },
							{ "zh-CHT", 1 }
						};
					}
					int num;
					if (CultureInfo.<>f__switch$map19.TryGetValue(name, out num))
					{
						if (num == 0)
						{
							return "zh-Hans";
						}
						if (num == 1)
						{
							return "zh-Hant";
						}
					}
				}
				return this.Name;
			}
		}

		[ComVisible(false)]
		public virtual int KeyboardLayoutId
		{
			get
			{
				int lcid = this.LCID;
				if (lcid == 4)
				{
					return 2052;
				}
				if (lcid == 1034)
				{
					return 3082;
				}
				if (lcid == 31748)
				{
					return 1028;
				}
				if (lcid != 31770)
				{
					return (this.LCID >= 1024) ? this.LCID : (this.LCID + 1024);
				}
				return 2074;
			}
		}

		public virtual int LCID
		{
			get
			{
				return this.cultureID;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public virtual string NativeName
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.nativename;
			}
		}

		public virtual Calendar Calendar
		{
			get
			{
				return this.DateTimeFormat.Calendar;
			}
		}

		public virtual Calendar[] OptionalCalendars
		{
			get
			{
				if (this.optional_calendars == null)
				{
					lock (this)
					{
						if (this.optional_calendars == null)
						{
							this.ConstructCalendars();
						}
					}
				}
				return this.optional_calendars;
			}
		}

		public virtual CultureInfo Parent
		{
			get
			{
				if (this.parent_culture == null)
				{
					if (!this.constructed)
					{
						this.Construct();
					}
					if (this.parent_lcid == this.cultureID)
					{
						return null;
					}
					if (this.parent_lcid == 127)
					{
						this.parent_culture = CultureInfo.InvariantCulture;
					}
					else if (this.cultureID == 127)
					{
						this.parent_culture = this;
					}
					else
					{
						this.parent_culture = new CultureInfo(this.parent_lcid);
					}
				}
				return this.parent_culture;
			}
		}

		public virtual TextInfo TextInfo
		{
			get
			{
				if (this.textInfo == null)
				{
					if (!this.constructed)
					{
						this.Construct();
					}
					lock (this)
					{
						if (this.textInfo == null)
						{
							this.textInfo = this.CreateTextInfo(this.m_isReadOnly);
						}
					}
				}
				return this.textInfo;
			}
		}

		public virtual string ThreeLetterISOLanguageName
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.iso3lang;
			}
		}

		public virtual string ThreeLetterWindowsLanguageName
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.win3lang;
			}
		}

		public virtual string TwoLetterISOLanguageName
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.iso2lang;
			}
		}

		public bool UseUserOverride
		{
			get
			{
				return this.m_useUserOverride;
			}
		}

		internal string IcuName
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.icu_name;
			}
		}

		public void ClearCachedData()
		{
			Thread.CurrentThread.CurrentCulture = null;
			Thread.CurrentThread.CurrentUICulture = null;
		}

		public virtual object Clone()
		{
			if (!this.constructed)
			{
				this.Construct();
			}
			CultureInfo cultureInfo = (CultureInfo)base.MemberwiseClone();
			cultureInfo.m_isReadOnly = false;
			cultureInfo.cached_serialized_form = null;
			if (!this.IsNeutralCulture)
			{
				cultureInfo.NumberFormat = (NumberFormatInfo)this.NumberFormat.Clone();
				cultureInfo.DateTimeFormat = (DateTimeFormatInfo)this.DateTimeFormat.Clone();
			}
			return cultureInfo;
		}

		public override bool Equals(object value)
		{
			CultureInfo cultureInfo = value as CultureInfo;
			return cultureInfo != null && cultureInfo.cultureID == this.cultureID;
		}

		public static CultureInfo[] GetCultures(CultureTypes types)
		{
			bool flag = (types & CultureTypes.NeutralCultures) != (CultureTypes)0;
			bool flag2 = (types & CultureTypes.SpecificCultures) != (CultureTypes)0;
			bool flag3 = (types & CultureTypes.InstalledWin32Cultures) != (CultureTypes)0;
			CultureInfo[] array = CultureInfo.internal_get_cultures(flag, flag2, flag3);
			if (flag && array.Length > 0 && array[0] == null)
			{
				array[0] = (CultureInfo)CultureInfo.InvariantCulture.Clone();
			}
			return array;
		}

		public override int GetHashCode()
		{
			return this.cultureID;
		}

		public static CultureInfo ReadOnly(CultureInfo ci)
		{
			if (ci == null)
			{
				throw new ArgumentNullException("ci");
			}
			if (ci.m_isReadOnly)
			{
				return ci;
			}
			CultureInfo cultureInfo = (CultureInfo)ci.Clone();
			cultureInfo.m_isReadOnly = true;
			if (cultureInfo.numInfo != null)
			{
				cultureInfo.numInfo = NumberFormatInfo.ReadOnly(cultureInfo.numInfo);
			}
			if (cultureInfo.dateTimeInfo != null)
			{
				cultureInfo.dateTimeInfo = DateTimeFormatInfo.ReadOnly(cultureInfo.dateTimeInfo);
			}
			if (cultureInfo.textInfo != null)
			{
				cultureInfo.textInfo = TextInfo.ReadOnly(cultureInfo.textInfo);
			}
			return cultureInfo;
		}

		public override string ToString()
		{
			return this.m_name;
		}

		public virtual CompareInfo CompareInfo
		{
			get
			{
				if (this.compareInfo == null)
				{
					if (!this.constructed)
					{
						this.Construct();
					}
					lock (this)
					{
						if (this.compareInfo == null)
						{
							this.compareInfo = new CompareInfo(this);
						}
					}
				}
				return this.compareInfo;
			}
		}

		internal static bool IsIDNeutralCulture(int lcid)
		{
			bool flag;
			if (!CultureInfo.internal_is_lcid_neutral(lcid, out flag))
			{
				throw new ArgumentException(string.Format("Culture id 0x{:x4} is not supported.", lcid));
			}
			return flag;
		}

		public virtual bool IsNeutralCulture
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.cultureID != 127 && ((this.cultureID & 65280) == 0 || this.specific_lcid == 0);
			}
		}

		internal void CheckNeutral()
		{
			if (this.IsNeutralCulture)
			{
				throw new NotSupportedException("Culture \"" + this.m_name + "\" is a neutral culture. It can not be used in formatting and parsing and therefore cannot be set as the thread's current culture.");
			}
		}

		public virtual NumberFormatInfo NumberFormat
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				this.CheckNeutral();
				if (this.numInfo == null)
				{
					lock (this)
					{
						if (this.numInfo == null)
						{
							this.numInfo = new NumberFormatInfo(this.m_isReadOnly);
							this.construct_number_format();
						}
					}
				}
				return this.numInfo;
			}
			set
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				if (this.m_isReadOnly)
				{
					throw new InvalidOperationException(CultureInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException("NumberFormat");
				}
				this.numInfo = value;
			}
		}

		public virtual DateTimeFormatInfo DateTimeFormat
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				this.CheckNeutral();
				if (this.dateTimeInfo == null)
				{
					lock (this)
					{
						if (this.dateTimeInfo == null)
						{
							this.dateTimeInfo = new DateTimeFormatInfo(this.m_isReadOnly);
							this.construct_datetime_format();
							if (this.optional_calendars != null)
							{
								this.dateTimeInfo.Calendar = this.optional_calendars[0];
							}
						}
					}
				}
				return this.dateTimeInfo;
			}
			set
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				if (this.m_isReadOnly)
				{
					throw new InvalidOperationException(CultureInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException("DateTimeFormat");
				}
				this.dateTimeInfo = value;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.displayname;
			}
		}

		public virtual string EnglishName
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.englishname;
			}
		}

		public static CultureInfo InstalledUICulture
		{
			get
			{
				return CultureInfo.GetCultureInfo(CultureInfo.BootstrapCultureID);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.m_isReadOnly;
			}
		}

		public virtual object GetFormat(Type formatType)
		{
			object obj = null;
			if (formatType == typeof(NumberFormatInfo))
			{
				obj = this.NumberFormat;
			}
			else if (formatType == typeof(DateTimeFormatInfo))
			{
				obj = this.DateTimeFormat;
			}
			return obj;
		}

		private void Construct()
		{
			this.construct_internal_locale_from_lcid(this.cultureID);
			this.constructed = true;
		}

		private bool ConstructInternalLocaleFromName(string locale)
		{
			string text = locale;
			if (text != null)
			{
				if (CultureInfo.<>f__switch$map1B == null)
				{
					CultureInfo.<>f__switch$map1B = new Dictionary<string, int>(2)
					{
						{ "zh-hans", 0 },
						{ "zh-hant", 1 }
					};
				}
				int num;
				if (CultureInfo.<>f__switch$map1B.TryGetValue(text, out num))
				{
					if (num != 0)
					{
						if (num == 1)
						{
							locale = "zh-cht";
						}
					}
					else
					{
						locale = "zh-chs";
					}
				}
			}
			return this.construct_internal_locale_from_name(locale);
		}

		private bool ConstructInternalLocaleFromLcid(int lcid)
		{
			return this.construct_internal_locale_from_lcid(lcid);
		}

		private static bool ConstructInternalLocaleFromSpecificName(CultureInfo ci, string name)
		{
			return CultureInfo.construct_internal_locale_from_specific_name(ci, name);
		}

		private static bool ConstructInternalLocaleFromCurrentLocale(CultureInfo ci)
		{
			return CultureInfo.construct_internal_locale_from_current_locale(ci);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool construct_internal_locale_from_lcid(int lcid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool construct_internal_locale_from_name(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool construct_internal_locale_from_specific_name(CultureInfo ci, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool construct_internal_locale_from_current_locale(CultureInfo ci);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CultureInfo[] internal_get_cultures(bool neutral, bool specific, bool installed);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void construct_datetime_format();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void construct_number_format();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool internal_is_lcid_neutral(int lcid, out bool is_neutral);

		private void ConstructInvariant(bool read_only)
		{
			this.cultureID = 127;
			this.numInfo = NumberFormatInfo.InvariantInfo;
			this.dateTimeInfo = DateTimeFormatInfo.InvariantInfo;
			if (!read_only)
			{
				this.numInfo = (NumberFormatInfo)this.numInfo.Clone();
				this.dateTimeInfo = (DateTimeFormatInfo)this.dateTimeInfo.Clone();
			}
			this.textInfo = this.CreateTextInfo(read_only);
			this.m_name = string.Empty;
			this.displayname = (this.englishname = (this.nativename = "Invariant Language (Invariant Country)"));
			this.iso3lang = "IVL";
			this.iso2lang = "iv";
			this.icu_name = "en_US_POSIX";
			this.win3lang = "IVL";
		}

		private TextInfo CreateTextInfo(bool readOnly)
		{
			return new TextInfo(this, this.cultureID, this.textinfo_data, readOnly);
		}

		private static void insert_into_shared_tables(CultureInfo c)
		{
			if (CultureInfo.shared_by_number == null)
			{
				CultureInfo.shared_by_number = new Hashtable();
				CultureInfo.shared_by_name = new Hashtable();
			}
			CultureInfo.shared_by_number[c.cultureID] = c;
			CultureInfo.shared_by_name[c.m_name] = c;
		}

		public static CultureInfo GetCultureInfo(int culture)
		{
			object obj = CultureInfo.shared_table_lock;
			CultureInfo cultureInfo2;
			lock (obj)
			{
				CultureInfo cultureInfo;
				if (CultureInfo.shared_by_number != null)
				{
					cultureInfo = CultureInfo.shared_by_number[culture] as CultureInfo;
					if (cultureInfo != null)
					{
						return cultureInfo;
					}
				}
				cultureInfo = new CultureInfo(culture, false, true);
				CultureInfo.insert_into_shared_tables(cultureInfo);
				cultureInfo2 = cultureInfo;
			}
			return cultureInfo2;
		}

		public static CultureInfo GetCultureInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			object obj = CultureInfo.shared_table_lock;
			CultureInfo cultureInfo2;
			lock (obj)
			{
				CultureInfo cultureInfo;
				if (CultureInfo.shared_by_name != null)
				{
					cultureInfo = CultureInfo.shared_by_name[name] as CultureInfo;
					if (cultureInfo != null)
					{
						return cultureInfo;
					}
				}
				cultureInfo = new CultureInfo(name, false, true);
				CultureInfo.insert_into_shared_tables(cultureInfo);
				cultureInfo2 = cultureInfo;
			}
			return cultureInfo2;
		}

		[MonoTODO("Currently it ignores the altName parameter")]
		public static CultureInfo GetCultureInfo(string name, string altName)
		{
			if (name == null)
			{
				throw new ArgumentNullException("null");
			}
			if (altName == null)
			{
				throw new ArgumentNullException("null");
			}
			return CultureInfo.GetCultureInfo(name);
		}

		public static CultureInfo GetCultureInfoByIetfLanguageTag(string name)
		{
			if (name != null)
			{
				if (CultureInfo.<>f__switch$map1C == null)
				{
					CultureInfo.<>f__switch$map1C = new Dictionary<string, int>(2)
					{
						{ "zh-Hans", 0 },
						{ "zh-Hant", 1 }
					};
				}
				int num;
				if (CultureInfo.<>f__switch$map1C.TryGetValue(name, out num))
				{
					if (num == 0)
					{
						return CultureInfo.GetCultureInfo("zh-CHS");
					}
					if (num == 1)
					{
						return CultureInfo.GetCultureInfo("zh-CHT");
					}
				}
			}
			return CultureInfo.GetCultureInfo(name);
		}

		internal static CultureInfo CreateCulture(string name, bool reference)
		{
			bool flag = name.Length == 0;
			bool flag2;
			bool flag3;
			if (reference)
			{
				flag2 = !flag;
				flag3 = false;
			}
			else
			{
				flag3 = false;
				flag2 = !flag;
			}
			return new CultureInfo(name, flag2, flag3);
		}

		internal unsafe void ConstructCalendars()
		{
			if (this.calendar_data == null)
			{
				this.optional_calendars = new Calendar[]
				{
					new GregorianCalendar(GregorianCalendarTypes.Localized)
				};
				return;
			}
			this.optional_calendars = new Calendar[5];
			for (int i = 0; i < 5; i++)
			{
				int num = this.calendar_data[i];
				Calendar calendar;
				switch (num >> 24)
				{
				case 0:
				{
					GregorianCalendarTypes gregorianCalendarTypes = (GregorianCalendarTypes)(num & 16777215);
					calendar = new GregorianCalendar(gregorianCalendarTypes);
					break;
				}
				case 1:
					calendar = new HijriCalendar();
					break;
				case 2:
					calendar = new ThaiBuddhistCalendar();
					break;
				default:
					throw new Exception("invalid calendar type:  " + num);
				}
				this.optional_calendars[i] = calendar;
			}
		}

		private const int NumOptionalCalendars = 5;

		private const int GregorianTypeMask = 16777215;

		private const int CalendarTypeBits = 24;

		private const int InvariantCultureId = 127;

		private static volatile CultureInfo invariant_culture_info = new CultureInfo(127, false, true);

		private static object shared_table_lock = new object();

		internal static int BootstrapCultureID;

		private bool m_isReadOnly;

		private int cultureID;

		[NonSerialized]
		private int parent_lcid;

		[NonSerialized]
		private int specific_lcid;

		[NonSerialized]
		private int datetime_index;

		[NonSerialized]
		private int number_index;

		private bool m_useUserOverride;

		[NonSerialized]
		private volatile NumberFormatInfo numInfo;

		private volatile DateTimeFormatInfo dateTimeInfo;

		private volatile TextInfo textInfo;

		private string m_name;

		[NonSerialized]
		private string displayname;

		[NonSerialized]
		private string englishname;

		[NonSerialized]
		private string nativename;

		[NonSerialized]
		private string iso3lang;

		[NonSerialized]
		private string iso2lang;

		[NonSerialized]
		private string icu_name;

		[NonSerialized]
		private string win3lang;

		[NonSerialized]
		private string territory;

		private volatile CompareInfo compareInfo;

		[NonSerialized]
		private unsafe readonly int* calendar_data;

		[NonSerialized]
		private unsafe readonly void* textinfo_data;

		[NonSerialized]
		private Calendar[] optional_calendars;

		[NonSerialized]
		private CultureInfo parent_culture;

		private int m_dataItem;

		private Calendar calendar;

		[NonSerialized]
		private bool constructed;

		[NonSerialized]
		internal byte[] cached_serialized_form;

		private static readonly string MSG_READONLY = "This instance is read only";

		private static Hashtable shared_by_number;

		private static Hashtable shared_by_name;
	}
}
