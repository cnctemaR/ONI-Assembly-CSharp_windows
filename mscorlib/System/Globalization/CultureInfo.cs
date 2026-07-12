using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Mono.Interop;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class CultureInfo : ICloneable, IFormatProvider
	{
		internal CultureData _cultureData
		{
			get
			{
				return this.m_cultureData;
			}
		}

		internal bool _isInherited
		{
			get
			{
				return this.m_isInherited;
			}
		}

		public static CultureInfo InvariantCulture
		{
			get
			{
				return CultureInfo.invariant_culture_info;
			}
		}

		public static CultureInfo CurrentCulture
		{
			get
			{
				return Thread.CurrentThread.CurrentCulture;
			}
			set
			{
				Thread.CurrentThread.CurrentCulture = value;
			}
		}

		public static CultureInfo CurrentUICulture
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture;
			}
			set
			{
				Thread.CurrentThread.CurrentUICulture = value;
			}
		}

		internal static CultureInfo ConstructCurrentCulture()
		{
			if (CultureInfo.default_current_culture != null)
			{
				return CultureInfo.default_current_culture;
			}
			if (GlobalizationMode.Invariant)
			{
				return CultureInfo.InvariantCulture;
			}
			string current_locale_name = CultureInfo.get_current_locale_name();
			CultureInfo cultureInfo = null;
			if (current_locale_name != null)
			{
				try
				{
					cultureInfo = CultureInfo.CreateSpecificCulture(current_locale_name);
				}
				catch
				{
				}
			}
			if (cultureInfo == null)
			{
				cultureInfo = CultureInfo.InvariantCulture;
			}
			else
			{
				cultureInfo.m_isReadOnly = true;
				cultureInfo.m_useUserOverride = true;
			}
			CultureInfo.default_current_culture = cultureInfo;
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

		internal string _name
		{
			get
			{
				return this.m_name;
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
					CultureTypes cultureTypes2 = (CultureTypes)obj;
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
			uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
			if (num <= 1260172255U)
			{
				if (num <= 939759947U)
				{
					if (num <= 249681006U)
					{
						if (num <= 198587497U)
						{
							if (num != 64366545U)
							{
								if (num != 77939050U)
								{
									if (num != 198587497U)
									{
										goto IL_06C2;
									}
									if (!(name == "ar-SA"))
									{
										goto IL_06C2;
									}
								}
								else if (!(name == "mr-IN"))
								{
									goto IL_06C2;
								}
							}
							else if (!(name == "ar-SY"))
							{
								goto IL_06C2;
							}
						}
						else if (num != 233820021U)
						{
							if (num != 236085687U)
							{
								if (num != 249681006U)
								{
									goto IL_06C2;
								}
								if (!(name == "hi-IN"))
								{
									goto IL_06C2;
								}
							}
							else if (!(name == "ar-KW"))
							{
								goto IL_06C2;
							}
						}
						else if (!(name == "ar-EG"))
						{
							goto IL_06C2;
						}
					}
					else if (num <= 469295067U)
					{
						if (num != 419506663U)
						{
							if (num != 434712723U)
							{
								if (num != 469295067U)
								{
									goto IL_06C2;
								}
								if (!(name == "ar-AE"))
								{
									goto IL_06C2;
								}
							}
							else if (!(name == "sa-IN"))
							{
								goto IL_06C2;
							}
						}
						else if (!(name == "ar-BH"))
						{
							goto IL_06C2;
						}
					}
					else if (num != 511763911U)
					{
						if (num != 907337542U)
						{
							if (num != 939759947U)
							{
								goto IL_06C2;
							}
							if (!(name == "ar-MA"))
							{
								goto IL_06C2;
							}
							goto IL_06B7;
						}
						else if (!(name == "ar-JO"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "vi-VN"))
					{
						goto IL_06C2;
					}
				}
				else if (num <= 1074569279U)
				{
					if (num <= 1011170994U)
					{
						if (num != 944060518U)
						{
							if (num != 944899161U)
							{
								if (num != 1011170994U)
								{
									goto IL_06C2;
								}
								if (!(name == "te"))
								{
									goto IL_06C2;
								}
							}
							else if (!(name == "sa"))
							{
								goto IL_06C2;
							}
						}
						else if (!(name == "ta"))
						{
							goto IL_06C2;
						}
					}
					else if (num != 1011465184U)
					{
						if (num != 1070729495U)
						{
							if (num != 1074569279U)
							{
								goto IL_06C2;
							}
							if (!(name == "ar-IQ"))
							{
								goto IL_06C2;
							}
						}
						else if (!(name == "ar-QA"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "vi"))
					{
						goto IL_06C2;
					}
				}
				else if (num <= 1123180923U)
				{
					if (num != 1094514636U)
					{
						if (num != 1095059089U)
						{
							if (num != 1123180923U)
							{
								goto IL_06C2;
							}
							if (!(name == "ar-DZ"))
							{
								goto IL_06C2;
							}
							goto IL_06B7;
						}
						else if (!(name == "th"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "kn"))
					{
						goto IL_06C2;
					}
				}
				else if (num != 1141238470U)
				{
					if (num != 1162022470U)
					{
						if (num != 1260172255U)
						{
							goto IL_06C2;
						}
						if (!(name == "dv"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "ur"))
					{
						goto IL_06C2;
					}
				}
				else if (!(name == "ar-LY"))
				{
					goto IL_06C2;
				}
			}
			else if (num <= 1756775346U)
			{
				if (num <= 1527123707U)
				{
					if (num <= 1429081278U)
					{
						if (num != 1277200137U)
						{
							if (num != 1347311754U)
							{
								if (num != 1429081278U)
								{
									goto IL_06C2;
								}
								if (!(name == "mr"))
								{
									goto IL_06C2;
								}
							}
							else if (!(name == "pa"))
							{
								goto IL_06C2;
							}
						}
						else if (!(name == "gu"))
						{
							goto IL_06C2;
						}
					}
					else if (num != 1456070279U)
					{
						if (num != 1458211363U)
						{
							if (num != 1527123707U)
							{
								goto IL_06C2;
							}
							if (!(name == "ar-LB"))
							{
								goto IL_06C2;
							}
						}
						else if (!(name == "gu-IN"))
						{
							goto IL_06C2;
						}
					}
					else
					{
						if (!(name == "ar-TN"))
						{
							goto IL_06C2;
						}
						goto IL_06B7;
					}
				}
				else if (num <= 1622153968U)
				{
					if (num != 1547363254U)
					{
						if (num != 1562713850U)
						{
							if (num != 1622153968U)
							{
								goto IL_06C2;
							}
							if (!(name == "kok-IN"))
							{
								goto IL_06C2;
							}
						}
						else if (!(name == "ar"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "he"))
					{
						goto IL_06C2;
					}
				}
				else if (num != 1680010088U)
				{
					if (num != 1748694682U)
					{
						if (num != 1756775346U)
						{
							goto IL_06C2;
						}
						if (!(name == "ta-IN"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "hi"))
					{
						goto IL_06C2;
					}
				}
				else if (!(name == "fa"))
				{
					goto IL_06C2;
				}
			}
			else if (num <= 3073845542U)
			{
				if (num <= 2153224060U)
				{
					if (num != 1846834581U)
					{
						if (num != 2046577884U)
						{
							if (num != 2153224060U)
							{
								goto IL_06C2;
							}
							if (!(name == "he-IL"))
							{
								goto IL_06C2;
							}
						}
						else if (!(name == "kok"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "dv-MV"))
					{
						goto IL_06C2;
					}
				}
				else if (num != 2902799296U)
				{
					if (num != 3060605246U)
					{
						if (num != 3073845542U)
						{
							goto IL_06C2;
						}
						if (!(name == "te-IN"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "pa-IN"))
					{
						goto IL_06C2;
					}
				}
				else if (!(name == "kn-IN"))
				{
					goto IL_06C2;
				}
			}
			else if (num <= 3477219856U)
			{
				if (num != 3294142633U)
				{
					if (num != 3311105148U)
					{
						if (num != 3477219856U)
						{
							goto IL_06C2;
						}
						if (!(name == "fa-IR"))
						{
							goto IL_06C2;
						}
					}
					else if (!(name == "syr-SY"))
					{
						goto IL_06C2;
					}
				}
				else if (!(name == "syr"))
				{
					goto IL_06C2;
				}
			}
			else if (num != 3957656723U)
			{
				if (num != 4027935912U)
				{
					if (num != 4091062904U)
					{
						goto IL_06C2;
					}
					if (!(name == "th-TH"))
					{
						goto IL_06C2;
					}
				}
				else if (!(name == "ur-PK"))
				{
					goto IL_06C2;
				}
			}
			else if (!(name == "ar-YE"))
			{
				goto IL_06C2;
			}
			return CultureInfo.GetCultureInfo("en");
			IL_06B7:
			return CultureInfo.GetCultureInfo("fr");
			IL_06C2:
			if ((this.CultureTypes & CultureTypes.WindowsOnlyCultures) == (CultureTypes)0)
			{
				return this;
			}
			return CultureInfo.InvariantCulture;
		}

		[ComVisible(false)]
		public string IetfLanguageTag
		{
			get
			{
				string name = this.Name;
				if (name == "zh-CHS")
				{
					return "zh-Hans";
				}
				if (!(name == "zh-CHT"))
				{
					return this.Name;
				}
				return "zh-Hant";
			}
		}

		[ComVisible(false)]
		public virtual int KeyboardLayoutId
		{
			get
			{
				int lcid = this.LCID;
				if (lcid <= 1034)
				{
					if (lcid == 4)
					{
						return 2052;
					}
					if (lcid == 1034)
					{
						return 3082;
					}
				}
				else
				{
					if (lcid == 31748)
					{
						return 1028;
					}
					if (lcid == 31770)
					{
						return 2074;
					}
				}
				if (this.LCID >= 1024)
				{
					return this.LCID;
				}
				return this.LCID + 1024;
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

		internal string NativeCalendarName
		{
			get
			{
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.native_calendar_names[(this.default_calendar_type >> 8) - 1];
			}
		}

		public virtual Calendar Calendar
		{
			get
			{
				if (this.calendar == null)
				{
					if (!this.constructed)
					{
						this.Construct();
					}
					this.calendar = CultureInfo.CreateCalendar(this.default_calendar_type);
				}
				return this.calendar;
			}
		}

		[MonoLimitation("Optional calendars are not supported only default calendar is returned")]
		public virtual Calendar[] OptionalCalendars
		{
			get
			{
				return new Calendar[] { this.Calendar };
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
						if (this.parent_lcid == 31748 && this.EnglishName[this.EnglishName.Length - 1] == 'y')
						{
							return this.parent_culture = new CultureInfo("zh-Hant");
						}
						if (this.parent_lcid == 4 && this.EnglishName[this.EnglishName.Length - 1] == 'y')
						{
							return this.parent_culture = new CultureInfo("zh-Hans");
						}
						return null;
					}
					else if (this.parent_lcid == 127)
					{
						this.parent_culture = CultureInfo.InvariantCulture;
					}
					else if (this.cultureID == 127)
					{
						this.parent_culture = this;
					}
					else if (this.cultureID == 1028)
					{
						this.parent_culture = new CultureInfo("zh-CHT");
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

		public void ClearCachedData()
		{
			object obj = CultureInfo.shared_table_lock;
			lock (obj)
			{
				CultureInfo.shared_by_number = null;
				CultureInfo.shared_by_name = null;
			}
			CultureInfo.default_current_culture = null;
			RegionInfo.ClearCachedData();
			TimeZone.ClearCachedData();
			TimeZoneInfo.ClearCachedData();
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
			return cultureInfo != null && cultureInfo.cultureID == this.cultureID && cultureInfo.m_name == this.m_name;
		}

		public static CultureInfo[] GetCultures(CultureTypes types)
		{
			bool flag = (types & CultureTypes.NeutralCultures) > (CultureTypes)0;
			bool flag2 = (types & CultureTypes.SpecificCultures) > (CultureTypes)0;
			bool flag3 = (types & CultureTypes.InstalledWin32Cultures) > (CultureTypes)0;
			CultureInfo[] array = CultureInfo.internal_get_cultures(flag, flag2, flag3);
			int i = 0;
			if (flag && array.Length != 0 && array[0] == null)
			{
				array[i++] = (CultureInfo)CultureInfo.InvariantCulture.Clone();
			}
			while (i < array.Length)
			{
				CultureInfo cultureInfo = array[i];
				CultureInfo.Data textInfoData = cultureInfo.GetTextInfoData();
				CultureInfo cultureInfo2 = array[i];
				string name = cultureInfo.m_name;
				bool flag4 = false;
				int num = cultureInfo.datetime_index;
				int calendarType = cultureInfo.CalendarType;
				int num2 = cultureInfo.number_index;
				string text = cultureInfo.iso2lang;
				int ansi = textInfoData.ansi;
				int oem = textInfoData.oem;
				int mac = textInfoData.mac;
				int ebcdic = textInfoData.ebcdic;
				bool right_to_left = textInfoData.right_to_left;
				char list_sep = (char)textInfoData.list_sep;
				cultureInfo2.m_cultureData = CultureData.GetCultureData(name, flag4, num, calendarType, num2, text, ansi, oem, mac, ebcdic, right_to_left, list_sep.ToString());
				i++;
			}
			return array;
		}

		private unsafe CultureInfo.Data GetTextInfoData()
		{
			return *(CultureInfo.Data*)this.textinfo_data;
		}

		public override int GetHashCode()
		{
			return this.cultureID.GetHashCode();
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

		public virtual bool IsNeutralCulture
		{
			get
			{
				if (this.cultureID == 127)
				{
					return false;
				}
				if (!this.constructed)
				{
					this.Construct();
				}
				return this.territory == null;
			}
		}

		private void CheckNeutral()
		{
		}

		public virtual NumberFormatInfo NumberFormat
		{
			get
			{
				if (this.numInfo == null)
				{
					this.numInfo = new NumberFormatInfo(this.m_cultureData)
					{
						isReadOnly = this.m_isReadOnly
					};
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
					throw new InvalidOperationException("This instance is read only");
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
				if (this.dateTimeInfo != null)
				{
					return this.dateTimeInfo;
				}
				if (!this.constructed)
				{
					this.Construct();
				}
				this.CheckNeutral();
				DateTimeFormatInfo dateTimeFormatInfo;
				if (GlobalizationMode.Invariant)
				{
					dateTimeFormatInfo = new DateTimeFormatInfo();
				}
				else
				{
					dateTimeFormatInfo = new DateTimeFormatInfo(this.m_cultureData, this.Calendar);
				}
				dateTimeFormatInfo._isReadOnly = this.m_isReadOnly;
				Thread.MemoryBarrier();
				this.dateTimeInfo = dateTimeFormatInfo;
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
					throw new InvalidOperationException("This instance is read only");
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
				return this.EnglishName;
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
				return CultureInfo.ConstructCurrentCulture();
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool construct_internal_locale_from_lcid(int lcid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool construct_internal_locale_from_name(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_current_locale_name();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CultureInfo[] internal_get_cultures(bool neutral, bool specific, bool installed);

		private void ConstructInvariant(bool read_only)
		{
			this.cultureID = 127;
			this.numInfo = NumberFormatInfo.InvariantInfo;
			if (!read_only)
			{
				this.numInfo = (NumberFormatInfo)this.numInfo.Clone();
			}
			this.textInfo = TextInfo.Invariant;
			this.m_name = string.Empty;
			this.englishname = (this.nativename = "Invariant Language (Invariant Country)");
			this.iso3lang = "IVL";
			this.iso2lang = "iv";
			this.win3lang = "IVL";
			this.default_calendar_type = 257;
		}

		private TextInfo CreateTextInfo(bool readOnly)
		{
			TextInfo textInfo = new TextInfo(this.m_cultureData);
			textInfo.SetReadOnlyState(readOnly);
			return textInfo;
		}

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
				this.m_cultureData = CultureData.Invariant;
				this.ConstructInvariant(read_only);
				return;
			}
			if (!this.construct_internal_locale_from_lcid(culture))
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Culture ID {0} (0x{1}) is not a supported culture.", culture.ToString(CultureInfo.InvariantCulture), culture.ToString("X4", CultureInfo.InvariantCulture));
				throw new CultureNotFoundException("culture", text);
			}
			CultureInfo.Data textInfoData = this.GetTextInfoData();
			string name = this.m_name;
			bool useUserOverride2 = this.m_useUserOverride;
			int num = this.datetime_index;
			int calendarType = this.CalendarType;
			int num2 = this.number_index;
			string text2 = this.iso2lang;
			int ansi = textInfoData.ansi;
			int oem = textInfoData.oem;
			int mac = textInfoData.mac;
			int ebcdic = textInfoData.ebcdic;
			bool right_to_left = textInfoData.right_to_left;
			char list_sep = (char)textInfoData.list_sep;
			this.m_cultureData = CultureData.GetCultureData(name, useUserOverride2, num, calendarType, num2, text2, ansi, oem, mac, ebcdic, right_to_left, list_sep.ToString());
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
			this.m_isInherited = base.GetType() != typeof(CultureInfo);
			if (name.Length == 0)
			{
				this.m_cultureData = CultureData.Invariant;
				this.ConstructInvariant(read_only);
				return;
			}
			if (!this.ConstructLocaleFromName(name.ToLowerInvariant()))
			{
				throw CultureInfo.CreateNotFoundException(name);
			}
			CultureInfo.Data textInfoData = this.GetTextInfoData();
			string name2 = this.m_name;
			int num = this.datetime_index;
			int calendarType = this.CalendarType;
			int num2 = this.number_index;
			string text = this.iso2lang;
			int ansi = textInfoData.ansi;
			int oem = textInfoData.oem;
			int mac = textInfoData.mac;
			int ebcdic = textInfoData.ebcdic;
			bool right_to_left = textInfoData.right_to_left;
			char list_sep = (char)textInfoData.list_sep;
			this.m_cultureData = CultureData.GetCultureData(name2, useUserOverride, num, calendarType, num2, text, ansi, oem, mac, ebcdic, right_to_left, list_sep.ToString());
		}

		private CultureInfo()
		{
			this.constructed = true;
		}

		private static void insert_into_shared_tables(CultureInfo c)
		{
			if (CultureInfo.shared_by_number == null)
			{
				CultureInfo.shared_by_number = new Dictionary<int, CultureInfo>();
				CultureInfo.shared_by_name = new Dictionary<string, CultureInfo>();
			}
			CultureInfo.shared_by_number[c.cultureID] = c;
			CultureInfo.shared_by_name[c.m_name] = c;
		}

		public static CultureInfo GetCultureInfo(int culture)
		{
			if (culture < 1)
			{
				throw new ArgumentOutOfRangeException("culture", "Positive number required.");
			}
			object obj = CultureInfo.shared_table_lock;
			CultureInfo cultureInfo2;
			lock (obj)
			{
				CultureInfo cultureInfo;
				if (CultureInfo.shared_by_number != null && CultureInfo.shared_by_number.TryGetValue(culture, out cultureInfo))
				{
					cultureInfo2 = cultureInfo;
				}
				else
				{
					cultureInfo = new CultureInfo(culture, false, true);
					CultureInfo.insert_into_shared_tables(cultureInfo);
					cultureInfo2 = cultureInfo;
				}
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
				if (CultureInfo.shared_by_name != null && CultureInfo.shared_by_name.TryGetValue(name, out cultureInfo))
				{
					cultureInfo2 = cultureInfo;
				}
				else
				{
					cultureInfo = new CultureInfo(name, false, true);
					CultureInfo.insert_into_shared_tables(cultureInfo);
					cultureInfo2 = cultureInfo;
				}
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
			if (name == "zh-Hans")
			{
				return CultureInfo.GetCultureInfo("zh-CHS");
			}
			if (!(name == "zh-Hant"))
			{
				return CultureInfo.GetCultureInfo(name);
			}
			return CultureInfo.GetCultureInfo("zh-CHT");
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

		public static CultureInfo CreateSpecificCulture(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				return CultureInfo.InvariantCulture;
			}
			string text = name;
			name = name.ToLowerInvariant();
			CultureInfo cultureInfo = new CultureInfo();
			if (!cultureInfo.ConstructLocaleFromName(name))
			{
				throw CultureInfo.CreateNotFoundException(text);
			}
			if (cultureInfo.IsNeutralCulture)
			{
				cultureInfo = CultureInfo.CreateSpecificCultureFromNeutral(cultureInfo.Name);
			}
			CultureInfo.Data textInfoData = cultureInfo.GetTextInfoData();
			CultureInfo cultureInfo2 = cultureInfo;
			string name2 = cultureInfo.m_name;
			bool flag = false;
			int num = cultureInfo.datetime_index;
			int calendarType = cultureInfo.CalendarType;
			int num2 = cultureInfo.number_index;
			string text2 = cultureInfo.iso2lang;
			int ansi = textInfoData.ansi;
			int oem = textInfoData.oem;
			int mac = textInfoData.mac;
			int ebcdic = textInfoData.ebcdic;
			bool right_to_left = textInfoData.right_to_left;
			char list_sep = (char)textInfoData.list_sep;
			cultureInfo2.m_cultureData = CultureData.GetCultureData(name2, flag, num, calendarType, num2, text2, ansi, oem, mac, ebcdic, right_to_left, list_sep.ToString());
			return cultureInfo;
		}

		private bool ConstructLocaleFromName(string name)
		{
			if (this.construct_internal_locale_from_name(name))
			{
				return true;
			}
			int num = name.Length - 1;
			if (num > 0)
			{
				while ((num = name.LastIndexOf('-', num - 1)) > 0)
				{
					if (this.construct_internal_locale_from_name(name.Substring(0, num)))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static CultureInfo CreateSpecificCultureFromNeutral(string name)
		{
			string text = name.ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			int num2;
			if (num <= 1344898993U)
			{
				if (num <= 1128614327U)
				{
					if (num <= 1025408520U)
					{
						if (num <= 975938470U)
						{
							if (num <= 926444256U)
							{
								if (num <= 896475900U)
								{
									if (num != 275533995U)
									{
										if (num == 896475900U)
										{
											if (text == "arn")
											{
												num2 = 1146;
												goto IL_1B49;
											}
										}
									}
									else if (text == "nso")
									{
										num2 = 1132;
										goto IL_1B49;
									}
								}
								else if (num != 925484199U)
								{
									if (num == 926444256U)
									{
										if (text == "id")
										{
											num2 = 1057;
											goto IL_1B49;
										}
									}
								}
								else if (text == "mn-cyrl")
								{
									num2 = 1104;
									goto IL_1B49;
								}
							}
							else if (num <= 944060518U)
							{
								if (num != 942383232U)
								{
									if (num == 944060518U)
									{
										if (text == "ta")
										{
											num2 = 1097;
											goto IL_1B49;
										}
									}
								}
								else if (text == "be")
								{
									num2 = 1059;
									goto IL_1B49;
								}
							}
							else if (num != 944899161U)
							{
								if (num == 975938470U)
								{
									if (text == "bg")
									{
										num2 = 1026;
										goto IL_1B49;
									}
								}
							}
							else if (text == "sa")
							{
								num2 = 1103;
								goto IL_1B49;
							}
						}
						else if (num <= 996684602U)
						{
							if (num <= 977615756U)
							{
								if (num != 976777113U)
								{
									if (num == 977615756U)
									{
										if (text == "tg")
										{
											num2 = 1064;
											goto IL_1B49;
										}
									}
								}
								else if (text == "ig")
								{
									num2 = 1136;
									goto IL_1B49;
								}
							}
							else if (num != 991980614U)
							{
								if (num == 996684602U)
								{
									if (text == "mn-mong")
									{
										num2 = 2128;
										goto IL_1B49;
									}
								}
							}
							else if (text == "gd")
							{
								num2 = 1169;
								goto IL_1B49;
							}
						}
						else if (num <= 1011170994U)
						{
							if (num != 1009493708U)
							{
								if (num == 1011170994U)
								{
									if (text == "te")
									{
										num2 = 1098;
										goto IL_1B49;
									}
								}
							}
							else if (text == "ba")
							{
								num2 = 1133;
								goto IL_1B49;
							}
						}
						else if (num != 1011465184U)
						{
							if (num != 1012009637U)
							{
								if (num == 1025408520U)
								{
									if (text == "tzm-latn")
									{
										num2 = 2143;
										goto IL_1B49;
									}
								}
							}
							else if (text == "se")
							{
								num2 = 1083;
								goto IL_1B49;
							}
						}
						else if (text == "vi")
						{
							num2 = 1066;
							goto IL_1B49;
						}
					}
					else if (num <= 1092248970U)
					{
						if (num <= 1058693732U)
						{
							if (num <= 1044726232U)
							{
								if (num != 1044181779U)
								{
									if (num == 1044726232U)
									{
										if (text == "tk")
										{
											num2 = 1090;
											goto IL_1B49;
										}
									}
								}
								else if (text == "kk")
								{
									num2 = 1087;
									goto IL_1B49;
								}
							}
							else if (num != 1045564875U)
							{
								if (num == 1058693732U)
								{
									if (text == "el")
									{
										num2 = 1032;
										goto IL_1B49;
									}
								}
							}
							else if (text == "sk")
							{
								num2 = 1051;
								goto IL_1B49;
							}
						}
						else if (num <= 1076162899U)
						{
							if (num != 1075868709U)
							{
								if (num == 1076162899U)
								{
									if (text == "am")
									{
										num2 = 1118;
										goto IL_1B49;
									}
								}
							}
							else if (text == "ga")
							{
								num2 = 2108;
								goto IL_1B49;
							}
						}
						else if (num != 1079120113U)
						{
							if (num != 1087741671U)
							{
								if (num == 1092248970U)
								{
									if (text == "en")
									{
										num2 = 1033;
										goto IL_1B49;
									}
								}
							}
							else if (text == "az-cyrl")
							{
								num2 = 2092;
								goto IL_1B49;
							}
						}
						else if (text == "si")
						{
							num2 = 1115;
							goto IL_1B49;
						}
					}
					else if (num <= 1110556780U)
					{
						if (num <= 1095059089U)
						{
							if (num != 1094514636U)
							{
								if (num == 1095059089U)
								{
									if (text == "th")
									{
										num2 = 1054;
										goto IL_1B49;
									}
								}
							}
							else if (text == "kn")
							{
								num2 = 1099;
								goto IL_1B49;
							}
						}
						else if (num != 1110159422U)
						{
							if (num == 1110556780U)
							{
								if (text == "lo")
								{
									num2 = 1108;
									goto IL_1B49;
								}
							}
						}
						else if (text == "bo")
						{
							num2 = 1105;
							goto IL_1B49;
						}
					}
					else if (num <= 1126201566U)
					{
						if (num != 1111292255U)
						{
							if (num == 1126201566U)
							{
								if (text == "gl")
								{
									num2 = 1110;
									goto IL_1B49;
								}
							}
						}
						else if (text == "ko")
						{
							num2 = 1042;
							goto IL_1B49;
						}
					}
					else if (num != 1126937041U)
					{
						if (num != 1128069874U)
						{
							if (num == 1128614327U)
							{
								if (text == "tn")
								{
									num2 = 1074;
									goto IL_1B49;
								}
							}
						}
						else if (text == "kl")
						{
							num2 = 1135;
							goto IL_1B49;
						}
					}
					else if (text == "bn")
					{
						num2 = 1093;
						goto IL_1B49;
					}
				}
				else if (num <= 1213341065U)
				{
					if (num <= 1177122803U)
					{
						if (num <= 1162022470U)
						{
							if (num <= 1144553303U)
							{
								if (num != 1129452970U)
								{
									if (num == 1144553303U)
									{
										if (text == "ii")
										{
											num2 = 1144;
											goto IL_1B49;
										}
									}
								}
								else if (text == "sl")
								{
									num2 = 1060;
									goto IL_1B49;
								}
							}
							else if (num != 1144847493U)
							{
								if (num == 1162022470U)
								{
									if (text == "ur")
									{
										num2 = 1056;
										goto IL_1B49;
									}
								}
							}
							else if (text == "km")
							{
								num2 = 1107;
								goto IL_1B49;
							}
						}
						else if (num <= 1163008208U)
						{
							if (num != 1162757945U)
							{
								if (num == 1163008208U)
								{
									if (text == "sr")
									{
										num2 = 9242;
										goto IL_1B49;
									}
								}
							}
							else if (text == "pl")
							{
								num2 = 1045;
								goto IL_1B49;
							}
						}
						else if (num != 1164435231U)
						{
							if (num != 1176137065U)
							{
								if (num == 1177122803U)
								{
									if (text == "cs")
									{
										num2 = 1029;
										goto IL_1B49;
									}
								}
							}
							else if (text == "es")
							{
								num2 = 3082;
								goto IL_1B49;
							}
						}
						else if (text == "zh")
						{
							num2 = 2052;
							goto IL_1B49;
						}
					}
					else if (num <= 1195724803U)
					{
						if (num <= 1194444875U)
						{
							if (num != 1192914684U)
							{
								if (num == 1194444875U)
								{
									if (text == "lb")
									{
										num2 = 1134;
										goto IL_1B49;
									}
								}
							}
							else if (text == "et")
							{
								num2 = 1061;
								goto IL_1B49;
							}
						}
						else if (num != 1194886160U)
						{
							if (num == 1195724803U)
							{
								if (text == "tr")
								{
									num2 = 1055;
									goto IL_1B49;
								}
							}
						}
						else if (text == "it")
						{
							num2 = 1040;
							goto IL_1B49;
						}
					}
					else if (num <= 1211324057U)
					{
						if (num != 1209692303U)
						{
							if (num == 1211324057U)
							{
								if (text == "iu-cans")
								{
									num2 = 1117;
									goto IL_1B49;
								}
							}
						}
						else if (text == "eu")
						{
							num2 = 1069;
							goto IL_1B49;
						}
					}
					else if (num != 1211663779U)
					{
						if (num != 1211957969U)
						{
							if (num == 1213341065U)
							{
								if (text == "sq")
								{
									num2 = 1052;
									goto IL_1B49;
								}
							}
						}
						else if (text == "ka")
						{
							num2 = 1079;
							goto IL_1B49;
						}
					}
					else if (text == "iu")
					{
						num2 = 2141;
						goto IL_1B49;
					}
				}
				else if (num <= 1277200137U)
				{
					if (num <= 1231251517U)
					{
						if (num <= 1227161470U)
						{
							if (num != 1213488160U)
							{
								if (num == 1227161470U)
								{
									if (text == "af")
									{
										num2 = 1078;
										goto IL_1B49;
									}
								}
							}
							else if (text == "ru")
							{
								num2 = 1049;
								goto IL_1B49;
							}
						}
						else if (num != 1230118684U)
						{
							if (num == 1231251517U)
							{
								if (text == "xh")
								{
									num2 = 1076;
									goto IL_1B49;
								}
							}
						}
						else if (text == "sv")
						{
							num2 = 1053;
							goto IL_1B49;
						}
					}
					else if (num <= 1246896303U)
					{
						if (num != 1237973804U)
						{
							if (num == 1246896303U)
							{
								if (text == "sw")
								{
									num2 = 1089;
									goto IL_1B49;
								}
							}
						}
						else if (text == "uz-latn")
						{
							num2 = 1091;
							goto IL_1B49;
						}
					}
					else if (num != 1247043398U)
					{
						if (num != 1260172255U)
						{
							if (num == 1277200137U)
							{
								if (text == "gu")
								{
									num2 = 1095;
									goto IL_1B49;
								}
							}
						}
						else if (text == "dv")
						{
							num2 = 1125;
							goto IL_1B49;
						}
					}
					else if (text == "rw")
					{
						num2 = 1159;
						goto IL_1B49;
					}
				}
				else if (num <= 1296390517U)
				{
					if (num <= 1278921350U)
					{
						if (num != 1277347232U)
						{
							if (num == 1278921350U)
							{
								if (text == "hu")
								{
									num2 = 1038;
									goto IL_1B49;
								}
							}
						}
						else if (text == "fy")
						{
							num2 = 1122;
							goto IL_1B49;
						}
					}
					else if (num != 1296243422U)
					{
						if (num == 1296390517U)
						{
							if (text == "tt")
							{
								num2 = 1092;
								goto IL_1B49;
							}
						}
					}
					else if (text == "uz")
					{
						num2 = 1091;
						goto IL_1B49;
					}
				}
				else if (num <= 1312329493U)
				{
					if (num != 1311490850U)
					{
						if (num == 1312329493U)
						{
							if (text == "is")
							{
								num2 = 1039;
								goto IL_1B49;
							}
						}
					}
					else if (text == "bs")
					{
						num2 = 5146;
						goto IL_1B49;
					}
				}
				else if (num != 1328268469U)
				{
					if (num != 1329254207U)
					{
						if (num == 1344898993U)
						{
							if (text == "cy")
							{
								num2 = 1106;
								goto IL_1B49;
							}
						}
					}
					else if (text == "hr")
					{
						num2 = 1050;
						goto IL_1B49;
					}
				}
				else if (text == "br")
				{
					num2 = 1150;
					goto IL_1B49;
				}
			}
			else if (num <= 1646454850U)
			{
				if (num <= 1545391778U)
				{
					if (num <= 1462636516U)
					{
						if (num <= 1428492898U)
						{
							if (num <= 1347311754U)
							{
								if (num != 1346178921U)
								{
									if (num == 1347311754U)
									{
										if (text == "pa")
										{
											num2 = 1094;
											goto IL_1B49;
										}
									}
								}
								else if (text == "ky")
								{
									num2 = 1088;
									goto IL_1B49;
								}
							}
							else if (num != 1424802581U)
							{
								if (num == 1428492898U)
								{
									if (text == "az")
									{
										num2 = 1068;
										goto IL_1B49;
									}
								}
							}
							else if (text == "tg-cyrl")
							{
								num2 = 1064;
								goto IL_1B49;
							}
						}
						else if (num <= 1429850248U)
						{
							if (num != 1429081278U)
							{
								if (num == 1429850248U)
								{
									if (text == "gsw")
									{
										num2 = 1156;
										goto IL_1B49;
									}
								}
							}
							else if (text == "mr")
							{
								num2 = 1102;
								goto IL_1B49;
							}
						}
						else if (num != 1445858897U)
						{
							if (num != 1461901041U)
							{
								if (num == 1462636516U)
								{
									if (text == "mt")
									{
										num2 = 1082;
										goto IL_1B49;
									}
								}
							}
							else if (text == "fr")
							{
								num2 = 1036;
								goto IL_1B49;
							}
						}
						else if (text == "ms")
						{
							num2 = 1086;
							goto IL_1B49;
						}
					}
					else if (num <= 1479958588U)
					{
						if (num <= 1478281302U)
						{
							if (num != 1463180969U)
							{
								if (num == 1478281302U)
								{
									if (text == "da")
									{
										num2 = 1030;
										goto IL_1B49;
									}
								}
							}
							else if (text == "nb")
							{
								num2 = 1044;
								goto IL_1B49;
							}
						}
						else if (num != 1479119945U)
						{
							if (num == 1479958588U)
							{
								if (text == "ne")
								{
									num2 = 1121;
									goto IL_1B49;
								}
							}
						}
						else if (text == "ca")
						{
							num2 = 1027;
							goto IL_1B49;
						}
					}
					else if (num <= 1483209992U)
					{
						if (num != 1480252778U)
						{
							if (num == 1483209992U)
							{
								if (text == "zu")
								{
									num2 = 1077;
									goto IL_1B49;
								}
							}
						}
						else if (text == "hy")
						{
							num2 = 1067;
							goto IL_1B49;
						}
					}
					else if (num != 1514352469U)
					{
						if (num != 1529997255U)
						{
							if (num == 1545391778U)
							{
								if (text == "de")
								{
									num2 = 1031;
									goto IL_1B49;
								}
							}
						}
						else if (text == "lv")
						{
							num2 = 1062;
							goto IL_1B49;
						}
					}
					else if (text == "ug")
					{
						num2 = 1152;
						goto IL_1B49;
					}
				}
				else if (num <= 1579491469U)
				{
					if (num <= 1551553596U)
					{
						if (num <= 1546524611U)
						{
							if (num != 1545789136U)
							{
								if (num == 1546524611U)
								{
									if (text == "mi")
									{
										num2 = 1153;
										goto IL_1B49;
									}
								}
							}
							else if (text == "fi")
							{
								num2 = 1035;
								goto IL_1B49;
							}
						}
						else if (num != 1547363254U)
						{
							if (num == 1551553596U)
							{
								if (text == "prs")
								{
									num2 = 1164;
									goto IL_1B49;
								}
							}
						}
						else if (text == "he")
						{
							num2 = 1037;
							goto IL_1B49;
						}
					}
					else if (num <= 1563552493U)
					{
						if (num != 1562713850U)
						{
							if (num == 1563552493U)
							{
								if (text == "lt")
								{
									num2 = 1063;
									goto IL_1B49;
								}
							}
						}
						else if (text == "ar")
						{
							num2 = 1025;
							goto IL_1B49;
						}
					}
					else if (num != 1563699588U)
					{
						if (num != 1565420801U)
						{
							if (num == 1579491469U)
							{
								if (text == "as")
								{
									num2 = 1101;
									goto IL_1B49;
								}
							}
						}
						else if (text == "pt")
						{
							num2 = 1046;
							goto IL_1B49;
						}
					}
					else if (text == "or")
					{
						num2 = 1096;
						goto IL_1B49;
					}
				}
				else if (num <= 1596857468U)
				{
					if (num <= 1581462945U)
					{
						if (num != 1580079849U)
						{
							if (num == 1581462945U)
							{
								if (text == "uk")
								{
									num2 = 1058;
									goto IL_1B49;
								}
							}
						}
						else if (text == "mk")
						{
							num2 = 1071;
							goto IL_1B49;
						}
					}
					else if (num != 1582198420U)
					{
						if (num == 1596857468U)
						{
							if (text == "ml")
							{
								num2 = 1100;
								goto IL_1B49;
							}
						}
					}
					else if (text == "ps")
					{
						num2 = 1123;
						goto IL_1B49;
					}
				}
				else if (num <= 1616151016U)
				{
					if (num != 1614473730U)
					{
						if (num == 1616151016U)
						{
							if (text == "rm")
							{
								num2 = 1047;
								goto IL_1B49;
							}
						}
					}
					else if (text == "ha")
					{
						num2 = 1128;
						goto IL_1B49;
					}
				}
				else if (num != 1630412706U)
				{
					if (num != 1630957159U)
					{
						if (num == 1646454850U)
						{
							if (text == "fo")
							{
								num2 = 1080;
								goto IL_1B49;
							}
						}
					}
					else if (text == "nl")
					{
						num2 = 1043;
						goto IL_1B49;
					}
				}
				else if (text == "mn")
				{
					num2 = 1104;
					goto IL_1B49;
				}
			}
			else if (num <= 3012500870U)
			{
				if (num <= 1748694682U)
				{
					if (num <= 1649706254U)
					{
						if (num <= 1647734778U)
						{
							if (num != 1646896135U)
							{
								if (num == 1647734778U)
								{
									if (text == "no")
									{
										num2 = 1044;
										goto IL_1B49;
									}
								}
							}
							else if (text == "co")
							{
								num2 = 1155;
								goto IL_1B49;
							}
						}
						else if (num != 1648867611U)
						{
							if (num == 1649706254U)
							{
								if (text == "ro")
								{
									num2 = 1048;
									goto IL_1B49;
								}
							}
						}
						else if (text == "wo")
						{
							num2 = 1160;
							goto IL_1B49;
						}
					}
					else if (num <= 1664512397U)
					{
						if (num != 1650441729U)
						{
							if (num == 1664512397U)
							{
								if (text == "nn")
								{
									num2 = 2068;
									goto IL_1B49;
								}
							}
						}
						else if (text == "yo")
						{
							num2 = 1130;
							goto IL_1B49;
						}
					}
					else if (num != 1680010088U)
					{
						if (num != 1680473867U)
						{
							if (num == 1748694682U)
							{
								if (text == "hi")
								{
									num2 = 1081;
									goto IL_1B49;
								}
							}
						}
						else if (text == "iu-latn")
						{
							num2 = 2141;
							goto IL_1B49;
						}
					}
					else if (text == "fa")
					{
						num2 = 1065;
						goto IL_1B49;
					}
				}
				else if (num <= 2046577884U)
				{
					if (num <= 1816099348U)
					{
						if (num != 1790977000U)
						{
							if (num == 1816099348U)
							{
								if (text == "ja")
								{
									num2 = 1041;
									goto IL_1B49;
								}
							}
						}
						else if (text == "bs-latn")
						{
							num2 = 5146;
							goto IL_1B49;
						}
					}
					else if (num != 1848919111U)
					{
						if (num == 2046577884U)
						{
							if (text == "kok")
							{
								num2 = 1111;
								goto IL_1B49;
							}
						}
					}
					else if (text == "oc")
					{
						num2 = 1154;
						goto IL_1B49;
					}
				}
				else
				{
					if (num <= 2197937899U)
					{
						if (num != 2180460995U)
						{
							if (num != 2197937899U)
							{
								goto IL_1B38;
							}
							if (!(text == "zh-hant"))
							{
								goto IL_1B38;
							}
						}
						else if (!(text == "zh-cht"))
						{
							goto IL_1B38;
						}
						num2 = 3076;
						goto IL_1B49;
					}
					if (num != 2264349090U)
					{
						if (num != 2281825994U)
						{
							if (num != 3012500870U)
							{
								goto IL_1B38;
							}
							if (!(text == "sr-latn"))
							{
								goto IL_1B38;
							}
							num2 = 9242;
							goto IL_1B49;
						}
						else if (!(text == "zh-hans"))
						{
							goto IL_1B38;
						}
					}
					else if (!(text == "zh-chs"))
					{
						goto IL_1B38;
					}
					num2 = 2052;
					goto IL_1B49;
				}
			}
			else if (num <= 3795602801U)
			{
				if (num <= 3294142633U)
				{
					if (num <= 3224459074U)
					{
						if (num != 3174420263U)
						{
							if (num == 3224459074U)
							{
								if (text == "tzm")
								{
									num2 = 2143;
									goto IL_1B49;
								}
							}
						}
						else if (text == "bs-cyrl")
						{
							num2 = 8218;
							goto IL_1B49;
						}
					}
					else if (num != 3240320582U)
					{
						if (num == 3294142633U)
						{
							if (text == "syr")
							{
								num2 = 1114;
								goto IL_1B49;
							}
						}
					}
					else if (text == "dsb")
					{
						num2 = 2094;
						goto IL_1B49;
					}
				}
				else if (num <= 3659307299U)
				{
					if (num != 3336872436U)
					{
						if (num == 3659307299U)
						{
							if (text == "sah")
							{
								num2 = 1157;
								goto IL_1B49;
							}
						}
					}
					else if (text == "fil")
					{
						num2 = 1124;
						goto IL_1B49;
					}
				}
				else if (num != 3678056394U)
				{
					if (num != 3761944489U)
					{
						if (num == 3795602801U)
						{
							if (text == "sr-cyrl")
							{
								num2 = 10266;
								goto IL_1B49;
							}
						}
					}
					else if (text == "smn")
					{
						num2 = 9275;
						goto IL_1B49;
					}
				}
				else if (text == "sms")
				{
					num2 = 8251;
					goto IL_1B49;
				}
			}
			else if (num <= 3953034599U)
			{
				if (num <= 3912943060U)
				{
					if (num != 3829054965U)
					{
						if (num == 3912943060U)
						{
							if (text == "sma")
							{
								num2 = 7227;
								goto IL_1B49;
							}
						}
					}
					else if (text == "smj")
					{
						num2 = 5179;
						goto IL_1B49;
					}
				}
				else if (num != 3918412059U)
				{
					if (num == 3953034599U)
					{
						if (text == "moh")
						{
							num2 = 1148;
							goto IL_1B49;
						}
					}
				}
				else if (text == "uz-cyrl")
				{
					num2 = 2115;
					goto IL_1B49;
				}
			}
			else if (num <= 4041297251U)
			{
				if (num != 3999162536U)
				{
					if (num == 4041297251U)
					{
						if (text == "quz")
						{
							num2 = 1131;
							goto IL_1B49;
						}
					}
				}
				else if (text == "az-latn")
				{
					num2 = 1068;
					goto IL_1B49;
				}
			}
			else if (num != 4103207754U)
			{
				if (num != 4276183917U)
				{
					if (num == 4280271688U)
					{
						if (text == "ha-latn")
						{
							num2 = 1128;
							goto IL_1B49;
						}
					}
				}
				else if (text == "qut")
				{
					num2 = 1158;
					goto IL_1B49;
				}
			}
			else if (text == "hsb")
			{
				num2 = 1070;
				goto IL_1B49;
			}
			IL_1B38:
			throw new NotImplementedException("Mapping for neutral culture " + name);
			IL_1B49:
			return new CultureInfo(num2);
		}

		internal int CalendarType
		{
			get
			{
				switch (this.default_calendar_type >> 8)
				{
				case 1:
					return 1;
				case 2:
					return 7;
				case 3:
					return 23;
				case 4:
					return 6;
				default:
					throw new NotImplementedException("CalendarType");
				}
			}
		}

		private static Calendar CreateCalendar(int calendarType)
		{
			string text;
			switch (calendarType >> 8)
			{
			case 1:
				return new GregorianCalendar((GregorianCalendarTypes)(calendarType & 255));
			case 2:
				text = "System.Globalization.ThaiBuddhistCalendar";
				break;
			case 3:
				text = "System.Globalization.UmAlQuraCalendar";
				break;
			case 4:
				text = "System.Globalization.HijriCalendar";
				break;
			default:
				throw new NotImplementedException("Unknown calendar type: " + calendarType.ToString());
			}
			Type type = Type.GetType(text, false);
			if (type == null)
			{
				return new GregorianCalendar(GregorianCalendarTypes.Localized);
			}
			return (Calendar)Activator.CreateInstance(type);
		}

		private static Exception CreateNotFoundException(string name)
		{
			return new CultureNotFoundException("name", "Culture name " + name + " is not supported.");
		}

		public static CultureInfo DefaultThreadCurrentCulture
		{
			get
			{
				return CultureInfo.s_DefaultThreadCurrentCulture;
			}
			set
			{
				CultureInfo.s_DefaultThreadCurrentCulture = value;
			}
		}

		public static CultureInfo DefaultThreadCurrentUICulture
		{
			get
			{
				return CultureInfo.s_DefaultThreadCurrentUICulture;
			}
			set
			{
				CultureInfo.s_DefaultThreadCurrentUICulture = value;
			}
		}

		internal string SortName
		{
			get
			{
				return this.m_name;
			}
		}

		internal static CultureInfo UserDefaultUICulture
		{
			get
			{
				return CultureInfo.ConstructCurrentUICulture();
			}
		}

		internal static CultureInfo UserDefaultCulture
		{
			get
			{
				return CultureInfo.ConstructCurrentCulture();
			}
		}

		[DllImport("__Internal")]
		private static extern void InitializeUserPreferredCultureInfoInAppX(CultureInfo.OnCultureInfoChangedDelegate onCultureInfoChangedInAppX);

		[DllImport("__Internal")]
		private static extern void SetUserPreferredCultureInfoInAppX([MarshalAs(UnmanagedType.LPWStr)] string name);

		[MonoPInvokeCallback(typeof(CultureInfo.OnCultureInfoChangedDelegate))]
		private static void OnCultureInfoChangedInAppX([MarshalAs(UnmanagedType.LPWStr)] string language)
		{
			if (language != null)
			{
				CultureInfo.s_UserPreferredCultureInfoInAppX = new CultureInfo(language);
				return;
			}
			CultureInfo.s_UserPreferredCultureInfoInAppX = null;
		}

		internal static CultureInfo GetCultureInfoForUserPreferredLanguageInAppX()
		{
			if (CultureInfo.s_UserPreferredCultureInfoInAppX == null)
			{
				CultureInfo.InitializeUserPreferredCultureInfoInAppX(new CultureInfo.OnCultureInfoChangedDelegate(CultureInfo.OnCultureInfoChangedInAppX));
			}
			return CultureInfo.s_UserPreferredCultureInfoInAppX;
		}

		internal static void SetCultureInfoForUserPreferredLanguageInAppX(CultureInfo cultureInfo)
		{
			if (CultureInfo.s_UserPreferredCultureInfoInAppX == null)
			{
				CultureInfo.InitializeUserPreferredCultureInfoInAppX(new CultureInfo.OnCultureInfoChangedDelegate(CultureInfo.OnCultureInfoChangedInAppX));
			}
			CultureInfo.SetUserPreferredCultureInfoInAppX(cultureInfo.Name);
			CultureInfo.s_UserPreferredCultureInfoInAppX = cultureInfo;
		}

		internal static void CheckDomainSafetyObject(object obj, object container)
		{
			if (obj.GetType().Assembly != typeof(CultureInfo).Assembly)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Cannot set sub-classed {0} object to {1} object."), obj.GetType(), container.GetType()));
			}
		}

		internal bool HasInvariantCultureName
		{
			get
			{
				return this.Name == CultureInfo.InvariantCulture.Name;
			}
		}

		internal static bool VerifyCultureName(string cultureName, bool throwException)
		{
			int i = 0;
			while (i < cultureName.Length)
			{
				char c = cultureName[i];
				if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
				{
					if (throwException)
					{
						throw new ArgumentException(Environment.GetResourceString("The given culture name '{0}' cannot be used to locate a resource file. Resource filenames must consist of only letters, numbers, hyphens or underscores.", new object[] { cultureName }));
					}
					return false;
				}
				else
				{
					i++;
				}
			}
			return true;
		}

		internal static bool VerifyCultureName(CultureInfo culture, bool throwException)
		{
			return !culture.m_isInherited || CultureInfo.VerifyCultureName(culture.Name, throwException);
		}

		private static volatile CultureInfo invariant_culture_info = new CultureInfo(127, false, true);

		private static object shared_table_lock = new object();

		private static CultureInfo default_current_culture;

		private bool m_isReadOnly;

		private int cultureID;

		[NonSerialized]
		private int parent_lcid;

		[NonSerialized]
		private int datetime_index;

		[NonSerialized]
		private int number_index;

		[NonSerialized]
		private int default_calendar_type;

		private bool m_useUserOverride;

		internal volatile NumberFormatInfo numInfo;

		internal volatile DateTimeFormatInfo dateTimeInfo;

		private volatile TextInfo textInfo;

		internal string m_name;

		[NonSerialized]
		private string englishname;

		[NonSerialized]
		private string nativename;

		[NonSerialized]
		private string iso3lang;

		[NonSerialized]
		private string iso2lang;

		[NonSerialized]
		private string win3lang;

		[NonSerialized]
		private string territory;

		[NonSerialized]
		private string[] native_calendar_names;

		private volatile CompareInfo compareInfo;

		[NonSerialized]
		private unsafe readonly void* textinfo_data;

		private int m_dataItem;

		private Calendar calendar;

		[NonSerialized]
		private CultureInfo parent_culture;

		[NonSerialized]
		private bool constructed;

		[NonSerialized]
		internal byte[] cached_serialized_form;

		[NonSerialized]
		internal CultureData m_cultureData;

		[NonSerialized]
		internal bool m_isInherited;

		internal const int InvariantCultureId = 127;

		private const int CalendarTypeBits = 8;

		internal const int LOCALE_INVARIANT = 127;

		private const string MSG_READONLY = "This instance is read only";

		private static volatile CultureInfo s_DefaultThreadCurrentUICulture;

		private static volatile CultureInfo s_DefaultThreadCurrentCulture;

		private static Dictionary<int, CultureInfo> shared_by_number;

		private static Dictionary<string, CultureInfo> shared_by_name;

		private static CultureInfo s_UserPreferredCultureInfoInAppX;

		internal static readonly bool IsTaiwanSku;

		private struct Data
		{
			public int ansi;

			public int ebcdic;

			public int mac;

			public int oem;

			public bool right_to_left;

			public byte list_sep;
		}

		private delegate void OnCultureInfoChangedDelegate([MarshalAs(UnmanagedType.LPWStr)] string language);
	}
}
