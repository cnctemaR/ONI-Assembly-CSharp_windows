using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Globalization
{
	[StructLayout(LayoutKind.Sequential)]
	internal class CultureData
	{
		private CultureData(string name)
		{
			this.sRealName = name;
		}

		public static CultureData Invariant
		{
			get
			{
				if (CultureData.s_Invariant == null)
				{
					CultureData cultureData = new CultureData("");
					cultureData.sISO639Language = "iv";
					cultureData.sAM1159 = "AM";
					cultureData.sPM2359 = "PM";
					cultureData.sTimeSeparator = ":";
					cultureData.saLongTimes = new string[] { "HH:mm:ss" };
					cultureData.saShortTimes = new string[] { "HH:mm", "hh:mm tt", "H:mm", "h:mm tt" };
					cultureData.iFirstDayOfWeek = 0;
					cultureData.iFirstWeekOfYear = 0;
					cultureData.waCalendars = new int[] { 1 };
					cultureData.calendars = new CalendarData[23];
					cultureData.calendars[0] = CalendarData.Invariant;
					cultureData.iDefaultAnsiCodePage = 1252;
					cultureData.iDefaultOemCodePage = 437;
					cultureData.iDefaultMacCodePage = 10000;
					cultureData.iDefaultEbcdicCodePage = 37;
					cultureData.sListSeparator = ",";
					Interlocked.CompareExchange<CultureData>(ref CultureData.s_Invariant, cultureData, null);
				}
				return CultureData.s_Invariant;
			}
		}

		public static CultureData GetCultureData(string cultureName, bool useUserOverride)
		{
			CultureData cultureData;
			try
			{
				cultureData = new CultureInfo(cultureName, useUserOverride).m_cultureData;
			}
			catch
			{
				cultureData = null;
			}
			return cultureData;
		}

		public static CultureData GetCultureData(string cultureName, bool useUserOverride, int datetimeIndex, int calendarId, int numberIndex, string iso2lang, int ansiCodePage, int oemCodePage, int macCodePage, int ebcdicCodePage, bool rightToLeft, string listSeparator)
		{
			if (string.IsNullOrEmpty(cultureName))
			{
				return CultureData.Invariant;
			}
			CultureData cultureData = new CultureData(cultureName);
			cultureData.fill_culture_data(datetimeIndex);
			cultureData.bUseOverrides = useUserOverride;
			cultureData.calendarId = calendarId;
			cultureData.numberIndex = numberIndex;
			cultureData.sISO639Language = iso2lang;
			cultureData.iDefaultAnsiCodePage = ansiCodePage;
			cultureData.iDefaultOemCodePage = oemCodePage;
			cultureData.iDefaultMacCodePage = macCodePage;
			cultureData.iDefaultEbcdicCodePage = ebcdicCodePage;
			cultureData.isRightToLeft = rightToLeft;
			cultureData.sListSeparator = listSeparator;
			return cultureData;
		}

		internal static CultureData GetCultureData(int culture, bool bUseUserOverride)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void fill_culture_data(int datetimeIndex);

		public CalendarData GetCalendar(int calendarId)
		{
			int num = calendarId - 1;
			if (this.calendars == null)
			{
				this.calendars = new CalendarData[23];
			}
			CalendarData calendarData = this.calendars[num];
			if (calendarData == null)
			{
				calendarData = new CalendarData(this.sRealName, calendarId, this.bUseOverrides);
				this.calendars[num] = calendarData;
			}
			return calendarData;
		}

		internal string[] LongTimes
		{
			get
			{
				return this.saLongTimes;
			}
		}

		internal string[] ShortTimes
		{
			get
			{
				return this.saShortTimes;
			}
		}

		internal string SISO639LANGNAME
		{
			get
			{
				return this.sISO639Language;
			}
		}

		internal int IFIRSTDAYOFWEEK
		{
			get
			{
				return this.iFirstDayOfWeek;
			}
		}

		internal int IFIRSTWEEKOFYEAR
		{
			get
			{
				return this.iFirstWeekOfYear;
			}
		}

		internal string SAM1159
		{
			get
			{
				return this.sAM1159;
			}
		}

		internal string SPM2359
		{
			get
			{
				return this.sPM2359;
			}
		}

		internal string TimeSeparator
		{
			get
			{
				return this.sTimeSeparator;
			}
		}

		internal int[] CalendarIds
		{
			get
			{
				if (this.waCalendars == null)
				{
					string text = this.sISO639Language;
					if (!(text == "ja"))
					{
						if (!(text == "zh"))
						{
							this.waCalendars = new int[] { this.calendarId };
						}
						else
						{
							this.waCalendars = new int[] { this.calendarId, 4 };
						}
					}
					else
					{
						this.waCalendars = new int[] { this.calendarId, 3 };
					}
				}
				return this.waCalendars;
			}
		}

		internal bool IsInvariantCulture
		{
			get
			{
				return string.IsNullOrEmpty(this.sRealName);
			}
		}

		internal string CultureName
		{
			get
			{
				return this.sRealName;
			}
		}

		internal string SCOMPAREINFO
		{
			get
			{
				return "";
			}
		}

		internal string STEXTINFO
		{
			get
			{
				return this.sRealName;
			}
		}

		internal int ILANGUAGE
		{
			get
			{
				return 0;
			}
		}

		internal int IDEFAULTANSICODEPAGE
		{
			get
			{
				return this.iDefaultAnsiCodePage;
			}
		}

		internal int IDEFAULTOEMCODEPAGE
		{
			get
			{
				return this.iDefaultOemCodePage;
			}
		}

		internal int IDEFAULTMACCODEPAGE
		{
			get
			{
				return this.iDefaultMacCodePage;
			}
		}

		internal int IDEFAULTEBCDICCODEPAGE
		{
			get
			{
				return this.iDefaultEbcdicCodePage;
			}
		}

		internal bool IsRightToLeft
		{
			get
			{
				return this.isRightToLeft;
			}
		}

		internal string SLIST
		{
			get
			{
				return this.sListSeparator;
			}
		}

		internal bool UseUserOverride
		{
			get
			{
				return this.bUseOverrides;
			}
		}

		internal string CalendarName(int calendarId)
		{
			return this.GetCalendar(calendarId).sNativeName;
		}

		internal string[] EraNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saEraNames;
		}

		internal string[] AbbrevEraNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevEraNames;
		}

		internal string[] AbbreviatedEnglishEraNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevEnglishEraNames;
		}

		internal string[] ShortDates(int calendarId)
		{
			return this.GetCalendar(calendarId).saShortDates;
		}

		internal string[] LongDates(int calendarId)
		{
			return this.GetCalendar(calendarId).saLongDates;
		}

		internal string[] YearMonths(int calendarId)
		{
			return this.GetCalendar(calendarId).saYearMonths;
		}

		internal string[] DayNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saDayNames;
		}

		internal string[] AbbreviatedDayNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevDayNames;
		}

		internal string[] SuperShortDayNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saSuperShortDayNames;
		}

		internal string[] MonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saMonthNames;
		}

		internal string[] GenitiveMonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saMonthGenitiveNames;
		}

		internal string[] AbbreviatedMonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevMonthNames;
		}

		internal string[] AbbreviatedGenitiveMonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saAbbrevMonthGenitiveNames;
		}

		internal string[] LeapYearMonthNames(int calendarId)
		{
			return this.GetCalendar(calendarId).saLeapYearMonthNames;
		}

		internal string MonthDay(int calendarId)
		{
			return this.GetCalendar(calendarId).sMonthDay;
		}

		internal string DateSeparator(int calendarId)
		{
			return CultureData.GetDateSeparator(this.ShortDates(calendarId)[0]);
		}

		private static string GetDateSeparator(string format)
		{
			return CultureData.GetSeparator(format, "dyM");
		}

		private static string GetSeparator(string format, string timeParts)
		{
			int num = CultureData.IndexOfTimePart(format, 0, timeParts);
			if (num != -1)
			{
				char c = format[num];
				do
				{
					num++;
				}
				while (num < format.Length && format[num] == c);
				int num2 = num;
				if (num2 < format.Length)
				{
					int num3 = CultureData.IndexOfTimePart(format, num2, timeParts);
					if (num3 != -1)
					{
						return CultureData.UnescapeNlsString(format, num2, num3 - 1);
					}
				}
			}
			return string.Empty;
		}

		private static int IndexOfTimePart(string format, int startIndex, string timeParts)
		{
			bool flag = false;
			for (int i = startIndex; i < format.Length; i++)
			{
				if (!flag && timeParts.IndexOf(format[i]) != -1)
				{
					return i;
				}
				char c = format[i];
				if (c != '\'')
				{
					if (c == '\\' && i + 1 < format.Length)
					{
						i++;
						c = format[i];
						if (c != '\'' && c != '\\')
						{
							i--;
						}
					}
				}
				else
				{
					flag = !flag;
				}
			}
			return -1;
		}

		private static string UnescapeNlsString(string str, int start, int end)
		{
			StringBuilder stringBuilder = null;
			int num = start;
			while (num < str.Length && num <= end)
			{
				char c = str[num];
				if (c != '\'')
				{
					if (c != '\\')
					{
						if (stringBuilder != null)
						{
							stringBuilder.Append(str[num]);
						}
					}
					else
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(str, start, num - start, str.Length);
						}
						num++;
						if (num < str.Length)
						{
							stringBuilder.Append(str[num]);
						}
					}
				}
				else if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(str, start, num - start, str.Length);
				}
				num++;
			}
			if (stringBuilder == null)
			{
				return str.Substring(start, end - start + 1);
			}
			return stringBuilder.ToString();
		}

		internal static string[] ReescapeWin32Strings(string[] array)
		{
			return array;
		}

		internal static string ReescapeWin32String(string str)
		{
			return str;
		}

		internal static bool IsCustomCultureId(int cultureId)
		{
			return false;
		}

		internal void GetNFIValues(NumberFormatInfo nfi)
		{
			if (!this.IsInvariantCulture)
			{
				CultureData.fill_number_data(nfi, this.numberIndex);
			}
			nfi.percentDecimalDigits = nfi.numberDecimalDigits;
			nfi.percentDecimalSeparator = nfi.numberDecimalSeparator;
			nfi.percentGroupSizes = nfi.numberGroupSizes;
			nfi.percentGroupSeparator = nfi.numberGroupSeparator;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void fill_number_data(NumberFormatInfo nfi, int numberIndex);

		private string sAM1159;

		private string sPM2359;

		private string sTimeSeparator;

		private volatile string[] saLongTimes;

		private volatile string[] saShortTimes;

		private int iFirstDayOfWeek;

		private int iFirstWeekOfYear;

		private volatile int[] waCalendars;

		private CalendarData[] calendars;

		private string sISO639Language;

		private readonly string sRealName;

		private bool bUseOverrides;

		private int calendarId;

		private int numberIndex;

		private int iDefaultAnsiCodePage;

		private int iDefaultOemCodePage;

		private int iDefaultMacCodePage;

		private int iDefaultEbcdicCodePage;

		private bool isRightToLeft;

		private string sListSeparator;

		private static CultureData s_Invariant;
	}
}
