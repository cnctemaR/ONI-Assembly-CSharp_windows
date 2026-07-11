using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public sealed class DateTimeFormatInfo : ICloneable, IFormatProvider
	{
		internal DateTimeFormatInfo(bool read_only)
		{
			this.m_isReadOnly = read_only;
			this.amDesignator = "AM";
			this.pmDesignator = "PM";
			this.dateSeparator = "/";
			this.timeSeparator = ":";
			this.shortDatePattern = "MM/dd/yyyy";
			this.longDatePattern = "dddd, dd MMMM yyyy";
			this.shortTimePattern = "HH:mm";
			this.longTimePattern = "HH:mm:ss";
			this.monthDayPattern = "MMMM dd";
			this.yearMonthPattern = "yyyy MMMM";
			this.fullDateTimePattern = "dddd, dd MMMM yyyy HH:mm:ss";
			this._RFC1123Pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
			this._SortableDateTimePattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
			this._UniversalSortableDateTimePattern = "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";
			this.firstDayOfWeek = 0;
			this.calendar = new GregorianCalendar();
			this.calendarWeekRule = 0;
			this.abbreviatedDayNames = DateTimeFormatInfo.INVARIANT_ABBREVIATED_DAY_NAMES;
			this.dayNames = DateTimeFormatInfo.INVARIANT_DAY_NAMES;
			this.abbreviatedMonthNames = DateTimeFormatInfo.INVARIANT_ABBREVIATED_MONTH_NAMES;
			this.monthNames = DateTimeFormatInfo.INVARIANT_MONTH_NAMES;
			this.m_genitiveAbbreviatedMonthNames = DateTimeFormatInfo.INVARIANT_ABBREVIATED_MONTH_NAMES;
			this.genitiveMonthNames = DateTimeFormatInfo.INVARIANT_MONTH_NAMES;
			this.shortDayNames = DateTimeFormatInfo.INVARIANT_SHORT_DAY_NAMES;
		}

		public DateTimeFormatInfo()
			: this(false)
		{
		}

		public static DateTimeFormatInfo GetInstance(IFormatProvider provider)
		{
			if (provider != null)
			{
				DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)provider.GetFormat(typeof(DateTimeFormatInfo));
				if (dateTimeFormatInfo != null)
				{
					return dateTimeFormatInfo;
				}
			}
			return DateTimeFormatInfo.CurrentInfo;
		}

		public bool IsReadOnly
		{
			get
			{
				return this.m_isReadOnly;
			}
		}

		public static DateTimeFormatInfo ReadOnly(DateTimeFormatInfo dtfi)
		{
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)dtfi.Clone();
			dateTimeFormatInfo.m_isReadOnly = true;
			return dateTimeFormatInfo;
		}

		public object Clone()
		{
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)base.MemberwiseClone();
			dateTimeFormatInfo.m_isReadOnly = false;
			return dateTimeFormatInfo;
		}

		public object GetFormat(Type formatType)
		{
			return (formatType != base.GetType()) ? null : this;
		}

		public string GetAbbreviatedEraName(int era)
		{
			if (era < 0 || era >= this.calendar.AbbreviatedEraNames.Length)
			{
				throw new ArgumentOutOfRangeException("era", era.ToString());
			}
			return this.calendar.AbbreviatedEraNames[era];
		}

		public string GetAbbreviatedMonthName(int month)
		{
			if (month < 1 || month > 13)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.abbreviatedMonthNames[month - 1];
		}

		public int GetEra(string eraName)
		{
			if (eraName == null)
			{
				throw new ArgumentNullException();
			}
			string[] array = this.calendar.EraNames;
			for (int i = 0; i < array.Length; i++)
			{
				if (CultureInfo.InvariantCulture.CompareInfo.Compare(eraName, array[i], CompareOptions.IgnoreCase) == 0)
				{
					return this.calendar.Eras[i];
				}
			}
			array = this.calendar.AbbreviatedEraNames;
			for (int j = 0; j < array.Length; j++)
			{
				if (CultureInfo.InvariantCulture.CompareInfo.Compare(eraName, array[j], CompareOptions.IgnoreCase) == 0)
				{
					return this.calendar.Eras[j];
				}
			}
			return -1;
		}

		public string GetEraName(int era)
		{
			if (era < 0 || era > this.calendar.EraNames.Length)
			{
				throw new ArgumentOutOfRangeException("era", era.ToString());
			}
			return this.calendar.EraNames[era - 1];
		}

		public string GetMonthName(int month)
		{
			if (month < 1 || month > 13)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.monthNames[month - 1];
		}

		public string[] AbbreviatedDayNames
		{
			get
			{
				return (string[])this.RawAbbreviatedDayNames.Clone();
			}
			set
			{
				this.RawAbbreviatedDayNames = value;
			}
		}

		internal string[] RawAbbreviatedDayNames
		{
			get
			{
				return this.abbreviatedDayNames;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.GetLength(0) != 7)
				{
					throw new ArgumentException(DateTimeFormatInfo.MSG_ARRAYSIZE_DAY);
				}
				this.abbreviatedDayNames = (string[])value.Clone();
			}
		}

		public string[] AbbreviatedMonthNames
		{
			get
			{
				return (string[])this.RawAbbreviatedMonthNames.Clone();
			}
			set
			{
				this.RawAbbreviatedMonthNames = value;
			}
		}

		internal string[] RawAbbreviatedMonthNames
		{
			get
			{
				return this.abbreviatedMonthNames;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.GetLength(0) != 13)
				{
					throw new ArgumentException(DateTimeFormatInfo.MSG_ARRAYSIZE_MONTH);
				}
				this.abbreviatedMonthNames = (string[])value.Clone();
			}
		}

		public string[] DayNames
		{
			get
			{
				return (string[])this.RawDayNames.Clone();
			}
			set
			{
				this.RawDayNames = value;
			}
		}

		internal string[] RawDayNames
		{
			get
			{
				return this.dayNames;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.GetLength(0) != 7)
				{
					throw new ArgumentException(DateTimeFormatInfo.MSG_ARRAYSIZE_DAY);
				}
				this.dayNames = (string[])value.Clone();
			}
		}

		public string[] MonthNames
		{
			get
			{
				return (string[])this.RawMonthNames.Clone();
			}
			set
			{
				this.RawMonthNames = value;
			}
		}

		internal string[] RawMonthNames
		{
			get
			{
				return this.monthNames;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.GetLength(0) != 13)
				{
					throw new ArgumentException(DateTimeFormatInfo.MSG_ARRAYSIZE_MONTH);
				}
				this.monthNames = (string[])value.Clone();
			}
		}

		public string AMDesignator
		{
			get
			{
				return this.amDesignator;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.amDesignator = value;
			}
		}

		public string PMDesignator
		{
			get
			{
				return this.pmDesignator;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.pmDesignator = value;
			}
		}

		public string DateSeparator
		{
			get
			{
				return this.dateSeparator;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.dateSeparator = value;
			}
		}

		public string TimeSeparator
		{
			get
			{
				return this.timeSeparator;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.timeSeparator = value;
			}
		}

		public string LongDatePattern
		{
			get
			{
				return this.longDatePattern;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.longDatePattern = value;
			}
		}

		public string ShortDatePattern
		{
			get
			{
				return this.shortDatePattern;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.shortDatePattern = value;
			}
		}

		public string ShortTimePattern
		{
			get
			{
				return this.shortTimePattern;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.shortTimePattern = value;
			}
		}

		public string LongTimePattern
		{
			get
			{
				return this.longTimePattern;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.longTimePattern = value;
			}
		}

		public string MonthDayPattern
		{
			get
			{
				return this.monthDayPattern;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.monthDayPattern = value;
			}
		}

		public string YearMonthPattern
		{
			get
			{
				return this.yearMonthPattern;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.yearMonthPattern = value;
			}
		}

		public string FullDateTimePattern
		{
			get
			{
				if (this.fullDateTimePattern != null)
				{
					return this.fullDateTimePattern;
				}
				return this.longDatePattern + " " + this.longTimePattern;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.fullDateTimePattern = value;
			}
		}

		public static DateTimeFormatInfo CurrentInfo
		{
			get
			{
				return Thread.CurrentThread.CurrentCulture.DateTimeFormat;
			}
		}

		public static DateTimeFormatInfo InvariantInfo
		{
			get
			{
				if (DateTimeFormatInfo.theInvariantDateTimeFormatInfo == null)
				{
					DateTimeFormatInfo.theInvariantDateTimeFormatInfo = DateTimeFormatInfo.ReadOnly(new DateTimeFormatInfo());
					DateTimeFormatInfo.theInvariantDateTimeFormatInfo.FillInvariantPatterns();
				}
				return DateTimeFormatInfo.theInvariantDateTimeFormatInfo;
			}
		}

		public DayOfWeek FirstDayOfWeek
		{
			get
			{
				return (DayOfWeek)this.firstDayOfWeek;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value < DayOfWeek.Sunday || value > DayOfWeek.Saturday)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.firstDayOfWeek = (int)value;
			}
		}

		public Calendar Calendar
		{
			get
			{
				return this.calendar;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.calendar = value;
			}
		}

		public CalendarWeekRule CalendarWeekRule
		{
			get
			{
				return (CalendarWeekRule)this.calendarWeekRule;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new InvalidOperationException(DateTimeFormatInfo.MSG_READONLY);
				}
				this.calendarWeekRule = (int)value;
			}
		}

		public string RFC1123Pattern
		{
			get
			{
				return this._RFC1123Pattern;
			}
		}

		internal string RoundtripPattern
		{
			get
			{
				return "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";
			}
		}

		public string SortableDateTimePattern
		{
			get
			{
				return this._SortableDateTimePattern;
			}
		}

		public string UniversalSortableDateTimePattern
		{
			get
			{
				return this._UniversalSortableDateTimePattern;
			}
		}

		public string[] GetAllDateTimePatterns()
		{
			return (string[])this.GetAllDateTimePatternsInternal().Clone();
		}

		internal string[] GetAllDateTimePatternsInternal()
		{
			this.FillAllDateTimePatterns();
			return this.all_date_time_patterns;
		}

		private void FillAllDateTimePatterns()
		{
			if (this.all_date_time_patterns != null)
			{
				return;
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(this.GetAllRawDateTimePatterns('d'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('D'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('g'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('G'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('f'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('F'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('m'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('M'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('r'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('R'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('s'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('t'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('T'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('u'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('U'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('y'));
			arrayList.AddRange(this.GetAllRawDateTimePatterns('Y'));
			this.all_date_time_patterns = (string[])arrayList.ToArray(typeof(string));
		}

		public string[] GetAllDateTimePatterns(char format)
		{
			return (string[])this.GetAllRawDateTimePatterns(format).Clone();
		}

		internal string[] GetAllRawDateTimePatterns(char format)
		{
			string[] array;
			switch (format)
			{
			case 'R':
				goto IL_02CB;
			default:
				switch (format)
				{
				case 'r':
					goto IL_02CB;
				case 's':
					return new string[] { this.SortableDateTimePattern };
				case 't':
					if (this.allShortTimePatterns != null && this.allShortTimePatterns.Length > 0)
					{
						return this.allShortTimePatterns;
					}
					return new string[] { this.ShortTimePattern };
				case 'u':
					return new string[] { this.UniversalSortableDateTimePattern };
				default:
					switch (format)
					{
					case 'D':
						if (this.allLongDatePatterns != null && this.allLongDatePatterns.Length > 0)
						{
							return this.allLongDatePatterns;
						}
						return new string[] { this.LongDatePattern };
					default:
						switch (format)
						{
						case 'd':
							if (this.allShortDatePatterns != null && this.allShortDatePatterns.Length > 0)
							{
								return this.allShortDatePatterns;
							}
							return new string[] { this.ShortDatePattern };
						default:
							if (format != 'M' && format != 'm')
							{
								throw new ArgumentException("Format specifier was invalid.");
							}
							if (this.monthDayPatterns != null && this.monthDayPatterns.Length > 0)
							{
								return this.monthDayPatterns;
							}
							return new string[] { this.MonthDayPattern };
						case 'f':
							array = this.PopulateCombinedList(this.allLongDatePatterns, this.allShortTimePatterns);
							if (array != null && array.Length > 0)
							{
								return array;
							}
							return new string[] { this.LongDatePattern + " " + this.ShortTimePattern };
						case 'g':
							array = this.PopulateCombinedList(this.allShortDatePatterns, this.allShortTimePatterns);
							if (array != null && array.Length > 0)
							{
								return array;
							}
							return new string[] { this.ShortDatePattern + " " + this.ShortTimePattern };
						}
						break;
					case 'F':
						break;
					case 'G':
						array = this.PopulateCombinedList(this.allShortDatePatterns, this.allLongTimePatterns);
						if (array != null && array.Length > 0)
						{
							return array;
						}
						return new string[] { this.ShortDatePattern + " " + this.LongTimePattern };
					}
					break;
				case 'y':
					goto IL_029B;
				}
				break;
			case 'T':
				if (this.allLongTimePatterns != null && this.allLongTimePatterns.Length > 0)
				{
					return this.allLongTimePatterns;
				}
				return new string[] { this.LongTimePattern };
			case 'U':
				break;
			case 'Y':
				goto IL_029B;
			}
			array = this.PopulateCombinedList(this.allLongDatePatterns, this.allLongTimePatterns);
			if (array != null && array.Length > 0)
			{
				return array;
			}
			return new string[] { this.LongDatePattern + " " + this.LongTimePattern };
			IL_029B:
			if (this.yearMonthPatterns != null && this.yearMonthPatterns.Length > 0)
			{
				return this.yearMonthPatterns;
			}
			return new string[] { this.YearMonthPattern };
			IL_02CB:
			return new string[] { this.RFC1123Pattern };
		}

		public string GetDayName(DayOfWeek dayofweek)
		{
			if (dayofweek < DayOfWeek.Sunday || dayofweek > DayOfWeek.Saturday)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.dayNames[(int)dayofweek];
		}

		public string GetAbbreviatedDayName(DayOfWeek dayofweek)
		{
			if (dayofweek < DayOfWeek.Sunday || dayofweek > DayOfWeek.Saturday)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.abbreviatedDayNames[(int)dayofweek];
		}

		private void FillInvariantPatterns()
		{
			this.allShortDatePatterns = new string[] { "MM/dd/yyyy" };
			this.allLongDatePatterns = new string[] { "dddd, dd MMMM yyyy" };
			this.allLongTimePatterns = new string[] { "HH:mm:ss" };
			this.allShortTimePatterns = new string[] { "HH:mm", "hh:mm tt", "H:mm", "h:mm tt" };
			this.monthDayPatterns = new string[] { "MMMM dd" };
			this.yearMonthPatterns = new string[] { "yyyy MMMM" };
		}

		private string[] PopulateCombinedList(string[] dates, string[] times)
		{
			if (dates != null && times != null)
			{
				string[] array = new string[dates.Length * times.Length];
				int num = 0;
				foreach (string text in dates)
				{
					foreach (string text2 in times)
					{
						array[num++] = text + " " + text2;
					}
				}
				return array;
			}
			return null;
		}

		[ComVisible(false)]
		[MonoTODO("Returns only the English month abbreviated names")]
		public string[] AbbreviatedMonthGenitiveNames
		{
			get
			{
				return this.m_genitiveAbbreviatedMonthNames;
			}
			set
			{
				this.m_genitiveAbbreviatedMonthNames = value;
			}
		}

		[MonoTODO("Returns only the English moth names")]
		[ComVisible(false)]
		public string[] MonthGenitiveNames
		{
			get
			{
				return this.genitiveMonthNames;
			}
			set
			{
				this.genitiveMonthNames = value;
			}
		}

		[ComVisible(false)]
		[MonoTODO("Returns an empty string as if the calendar name wasn't available")]
		public string NativeCalendarName
		{
			get
			{
				return string.Empty;
			}
		}

		[ComVisible(false)]
		public string[] ShortestDayNames
		{
			get
			{
				return this.shortDayNames;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.Length != 7)
				{
					throw new ArgumentException("Array must have 7 entries");
				}
				for (int i = 0; i < 7; i++)
				{
					if (value[i] == null)
					{
						throw new ArgumentNullException(string.Format("Element {0} is null", i));
					}
				}
				this.shortDayNames = value;
			}
		}

		[ComVisible(false)]
		public string GetShortestDayName(DayOfWeek dayOfWeek)
		{
			if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.shortDayNames[(int)dayOfWeek];
		}

		[ComVisible(false)]
		public void SetAllDateTimePatterns(string[] patterns, char format)
		{
			if (patterns == null)
			{
				throw new ArgumentNullException("patterns");
			}
			if (patterns.Length == 0)
			{
				throw new ArgumentException("patterns", "The argument patterns must not be of zero-length");
			}
			if (format != 'D')
			{
				if (format != 'M')
				{
					if (format != 'T')
					{
						if (format != 'Y')
						{
							if (format == 'd')
							{
								this.allShortDatePatterns = patterns;
								return;
							}
							if (format == 'm')
							{
								goto IL_007C;
							}
							if (format == 't')
							{
								this.allShortTimePatterns = patterns;
								return;
							}
							if (format != 'y')
							{
								throw new ArgumentException("format", "Format specifier is invalid");
							}
						}
						this.yearMonthPatterns = patterns;
						return;
					}
					this.allLongTimePatterns = patterns;
					return;
				}
				IL_007C:
				this.monthDayPatterns = patterns;
			}
			else
			{
				this.allLongDatePatterns = patterns;
			}
		}

		private const string _RoundtripPattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

		private static readonly string MSG_READONLY = "This instance is read only";

		private static readonly string MSG_ARRAYSIZE_MONTH = "An array with exactly 13 elements is needed";

		private static readonly string MSG_ARRAYSIZE_DAY = "An array with exactly 7 elements is needed";

		private static readonly string[] INVARIANT_ABBREVIATED_DAY_NAMES = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

		private static readonly string[] INVARIANT_DAY_NAMES = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

		private static readonly string[] INVARIANT_ABBREVIATED_MONTH_NAMES = new string[]
		{
			"Jan",
			"Feb",
			"Mar",
			"Apr",
			"May",
			"Jun",
			"Jul",
			"Aug",
			"Sep",
			"Oct",
			"Nov",
			"Dec",
			string.Empty
		};

		private static readonly string[] INVARIANT_MONTH_NAMES = new string[]
		{
			"January",
			"February",
			"March",
			"April",
			"May",
			"June",
			"July",
			"August",
			"September",
			"October",
			"November",
			"December",
			string.Empty
		};

		private static readonly string[] INVARIANT_SHORT_DAY_NAMES = new string[] { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };

		private static DateTimeFormatInfo theInvariantDateTimeFormatInfo;

		private bool m_isReadOnly;

		private string amDesignator;

		private string pmDesignator;

		private string dateSeparator;

		private string timeSeparator;

		private string shortDatePattern;

		private string longDatePattern;

		private string shortTimePattern;

		private string longTimePattern;

		private string monthDayPattern;

		private string yearMonthPattern;

		private string fullDateTimePattern;

		private string _RFC1123Pattern;

		private string _SortableDateTimePattern;

		private string _UniversalSortableDateTimePattern;

		private int firstDayOfWeek;

		private Calendar calendar;

		private int calendarWeekRule;

		private string[] abbreviatedDayNames;

		private string[] dayNames;

		private string[] monthNames;

		private string[] abbreviatedMonthNames;

		private string[] allShortDatePatterns;

		private string[] allLongDatePatterns;

		private string[] allShortTimePatterns;

		private string[] allLongTimePatterns;

		private string[] monthDayPatterns;

		private string[] yearMonthPatterns;

		private string[] shortDayNames;

		private int nDataItem;

		private bool m_useUserOverride;

		private bool m_isDefaultCalendar;

		private int CultureID;

		private bool bUseCalendarInfo;

		private string generalShortTimePattern;

		private string generalLongTimePattern;

		private string[] m_eraNames;

		private string[] m_abbrevEraNames;

		private string[] m_abbrevEnglishEraNames;

		private string[] m_dateWords;

		private int[] optionalCalendars;

		private string[] m_superShortDayNames;

		private string[] genitiveMonthNames;

		private string[] m_genitiveAbbreviatedMonthNames;

		private string[] leapYearMonthNames;

		private DateTimeFormatFlags formatFlags;

		private string m_name;

		private volatile string[] all_date_time_patterns;
	}
}
