using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net.Mime
{
	internal class SmtpDateTime
	{
		internal static Dictionary<string, TimeSpan> InitializeShortHandLookups()
		{
			return new Dictionary<string, TimeSpan>
			{
				{
					"UT",
					TimeSpan.Zero
				},
				{
					"GMT",
					TimeSpan.Zero
				},
				{
					"EDT",
					new TimeSpan(-4, 0, 0)
				},
				{
					"EST",
					new TimeSpan(-5, 0, 0)
				},
				{
					"CDT",
					new TimeSpan(-5, 0, 0)
				},
				{
					"CST",
					new TimeSpan(-6, 0, 0)
				},
				{
					"MDT",
					new TimeSpan(-6, 0, 0)
				},
				{
					"MST",
					new TimeSpan(-7, 0, 0)
				},
				{
					"PDT",
					new TimeSpan(-7, 0, 0)
				},
				{
					"PST",
					new TimeSpan(-8, 0, 0)
				}
			};
		}

		internal SmtpDateTime(DateTime value)
		{
			this._date = value;
			switch (value.Kind)
			{
			case DateTimeKind.Unspecified:
				this._unknownTimeZone = true;
				return;
			case DateTimeKind.Utc:
				this._timeZone = TimeSpan.Zero;
				return;
			case DateTimeKind.Local:
			{
				TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(value);
				this._timeZone = this.ValidateAndGetSanitizedTimeSpan(utcOffset);
				return;
			}
			default:
				return;
			}
		}

		internal SmtpDateTime(string value)
		{
			string text;
			this._date = this.ParseValue(value, out text);
			if (!this.TryParseTimeZoneString(text, out this._timeZone))
			{
				this._unknownTimeZone = true;
			}
		}

		internal DateTime Date
		{
			get
			{
				if (this._unknownTimeZone)
				{
					return DateTime.SpecifyKind(this._date, DateTimeKind.Unspecified);
				}
				DateTimeOffset dateTimeOffset = new DateTimeOffset(this._date, this._timeZone);
				return dateTimeOffset.LocalDateTime;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", this.FormatDate(this._date), this._unknownTimeZone ? "-0000" : this.TimeSpanToOffset(this._timeZone));
		}

		internal void ValidateAndGetTimeZoneOffsetValues(string offset, out bool positive, out int hours, out int minutes)
		{
			if (offset.Length != 5)
			{
				throw new FormatException("The date is in an invalid format.");
			}
			positive = offset.StartsWith("+", StringComparison.Ordinal);
			if (!int.TryParse(offset.Substring(1, 2), NumberStyles.None, CultureInfo.InvariantCulture, out hours))
			{
				throw new FormatException("The date is in an invalid format.");
			}
			if (!int.TryParse(offset.Substring(3, 2), NumberStyles.None, CultureInfo.InvariantCulture, out minutes))
			{
				throw new FormatException("The date is in an invalid format.");
			}
			if (minutes > 59)
			{
				throw new FormatException("The date is in an invalid format.");
			}
		}

		internal void ValidateTimeZoneShortHandValue(string value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsLetter(value, i))
				{
					throw new FormatException(SR.Format("An invalid character was found in the mail header: '{0}'.", value));
				}
			}
		}

		internal string FormatDate(DateTime value)
		{
			return value.ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
		}

		internal DateTime ParseValue(string data, out string timeZone)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new FormatException("The date is in an invalid format.");
			}
			int num = data.IndexOf(':');
			if (num == -1)
			{
				throw new FormatException(SR.Format("An invalid character was found in the mail header: '{0}'.", data));
			}
			int num2 = data.IndexOfAny(SmtpDateTime.s_allowedWhiteSpaceChars, num);
			if (num2 == -1)
			{
				throw new FormatException(SR.Format("An invalid character was found in the mail header: '{0}'.", data));
			}
			DateTime dateTime;
			if (!DateTime.TryParseExact(data.Substring(0, num2).Trim(), SmtpDateTime.s_validDateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime))
			{
				throw new FormatException("The date is in an invalid format.");
			}
			string text = data.Substring(num2).Trim();
			int num3 = text.IndexOfAny(SmtpDateTime.s_allowedWhiteSpaceChars);
			if (num3 != -1)
			{
				text = text.Substring(0, num3);
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new FormatException("The date is in an invalid format.");
			}
			timeZone = text;
			return dateTime;
		}

		internal bool TryParseTimeZoneString(string timeZoneString, out TimeSpan timeZone)
		{
			if (timeZoneString == "-0000")
			{
				timeZone = TimeSpan.Zero;
				return false;
			}
			if (timeZoneString[0] == '+' || timeZoneString[0] == '-')
			{
				bool flag;
				int num;
				int num2;
				this.ValidateAndGetTimeZoneOffsetValues(timeZoneString, out flag, out num, out num2);
				if (!flag)
				{
					if (num != 0)
					{
						num *= -1;
					}
					else if (num2 != 0)
					{
						num2 *= -1;
					}
				}
				timeZone = new TimeSpan(num, num2, 0);
				return true;
			}
			this.ValidateTimeZoneShortHandValue(timeZoneString);
			return SmtpDateTime.s_timeZoneOffsetLookup.TryGetValue(timeZoneString, out timeZone);
		}

		internal TimeSpan ValidateAndGetSanitizedTimeSpan(TimeSpan span)
		{
			TimeSpan timeSpan = new TimeSpan(span.Days, span.Hours, span.Minutes, 0, 0);
			if (Math.Abs(timeSpan.Ticks) > 3599400000000L)
			{
				throw new FormatException("The date is in an invalid format.");
			}
			return timeSpan;
		}

		internal string TimeSpanToOffset(TimeSpan span)
		{
			if (span.Ticks == 0L)
			{
				return "+0000";
			}
			uint num = (uint)Math.Abs(Math.Floor(span.TotalHours));
			uint num2 = (uint)Math.Abs(span.Minutes);
			string text = ((span.Ticks > 0L) ? "+" : "-");
			if (num < 10U)
			{
				text += "0";
			}
			text += num.ToString();
			if (num2 < 10U)
			{
				text += "0";
			}
			return text + num2.ToString();
		}

		internal const string UnknownTimeZoneDefaultOffset = "-0000";

		internal const string UtcDefaultTimeZoneOffset = "+0000";

		internal const int OffsetLength = 5;

		internal const int MaxMinuteValue = 59;

		internal const string DateFormatWithDayOfWeek = "ddd, dd MMM yyyy HH:mm:ss";

		internal const string DateFormatWithoutDayOfWeek = "dd MMM yyyy HH:mm:ss";

		internal const string DateFormatWithDayOfWeekAndNoSeconds = "ddd, dd MMM yyyy HH:mm";

		internal const string DateFormatWithoutDayOfWeekAndNoSeconds = "dd MMM yyyy HH:mm";

		internal static readonly string[] s_validDateTimeFormats = new string[] { "ddd, dd MMM yyyy HH:mm:ss", "dd MMM yyyy HH:mm:ss", "ddd, dd MMM yyyy HH:mm", "dd MMM yyyy HH:mm" };

		internal static readonly char[] s_allowedWhiteSpaceChars = new char[] { ' ', '\t' };

		internal static readonly Dictionary<string, TimeSpan> s_timeZoneOffsetLookup = SmtpDateTime.InitializeShortHandLookups();

		internal const long TimeSpanMaxTicks = 3599400000000L;

		internal const int OffsetMaxValue = 9959;

		private readonly DateTime _date;

		private readonly TimeSpan _timeZone;

		private readonly bool _unknownTimeZone;
	}
}
