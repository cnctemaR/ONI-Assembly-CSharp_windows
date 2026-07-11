using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net.Mime
{
	internal class SmtpDateTime
	{
		internal static IDictionary<string, TimeSpan> InitializeShortHandLookups()
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
			this.date = value;
			switch (value.Kind)
			{
			case DateTimeKind.Unspecified:
				this.unknownTimeZone = true;
				return;
			case DateTimeKind.Utc:
				this.timeZone = TimeSpan.Zero;
				return;
			case DateTimeKind.Local:
			{
				TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(value);
				this.timeZone = this.ValidateAndGetSanitizedTimeSpan(utcOffset);
				return;
			}
			default:
				return;
			}
		}

		internal SmtpDateTime(string value)
		{
			string text;
			this.date = this.ParseValue(value, out text);
			if (!this.TryParseTimeZoneString(text, out this.timeZone))
			{
				this.unknownTimeZone = true;
			}
		}

		internal DateTime Date
		{
			get
			{
				if (this.unknownTimeZone)
				{
					return DateTime.SpecifyKind(this.date, DateTimeKind.Unspecified);
				}
				DateTimeOffset dateTimeOffset = new DateTimeOffset(this.date, this.timeZone);
				return dateTimeOffset.LocalDateTime;
			}
		}

		public override string ToString()
		{
			if (this.unknownTimeZone)
			{
				return string.Format("{0} {1}", this.FormatDate(this.date), "-0000");
			}
			return string.Format("{0} {1}", this.FormatDate(this.date), this.TimeSpanToOffset(this.timeZone));
		}

		internal void ValidateAndGetTimeZoneOffsetValues(string offset, out bool positive, out int hours, out int minutes)
		{
			if (offset.Length != 5)
			{
				throw new FormatException(global::SR.GetString("The date is in an invalid format."));
			}
			positive = offset.StartsWith("+");
			if (!int.TryParse(offset.Substring(1, 2), NumberStyles.None, CultureInfo.InvariantCulture, out hours))
			{
				throw new FormatException(global::SR.GetString("The date is in an invalid format."));
			}
			if (!int.TryParse(offset.Substring(3, 2), NumberStyles.None, CultureInfo.InvariantCulture, out minutes))
			{
				throw new FormatException(global::SR.GetString("The date is in an invalid format."));
			}
			if (minutes > 59)
			{
				throw new FormatException(global::SR.GetString("The date is in an invalid format."));
			}
		}

		internal void ValidateTimeZoneShortHandValue(string value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsLetter(value, i))
				{
					throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'."));
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
				throw new FormatException(global::SR.GetString("The date is in an invalid format."));
			}
			int num = data.IndexOf(':');
			if (num == -1)
			{
				throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'."));
			}
			int num2 = data.IndexOfAny(SmtpDateTime.allowedWhiteSpaceChars, num);
			if (num2 == -1)
			{
				throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'."));
			}
			DateTime dateTime;
			if (!DateTime.TryParseExact(data.Substring(0, num2).Trim(), SmtpDateTime.validDateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime))
			{
				throw new FormatException(global::SR.GetString("The date is in an invalid format."));
			}
			string text = data.Substring(num2).Trim();
			int num3 = text.IndexOfAny(SmtpDateTime.allowedWhiteSpaceChars);
			if (num3 != -1)
			{
				text = text.Substring(0, num3);
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new FormatException(global::SR.GetString("The date is in an invalid format."));
			}
			timeZone = text;
			return dateTime;
		}

		internal bool TryParseTimeZoneString(string timeZoneString, out TimeSpan timeZone)
		{
			timeZone = TimeSpan.Zero;
			if (timeZoneString == "-0000")
			{
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
			if (SmtpDateTime.timeZoneOffsetLookup.ContainsKey(timeZoneString))
			{
				timeZone = SmtpDateTime.timeZoneOffsetLookup[timeZoneString];
				return true;
			}
			return false;
		}

		internal TimeSpan ValidateAndGetSanitizedTimeSpan(TimeSpan span)
		{
			TimeSpan timeSpan = new TimeSpan(span.Days, span.Hours, span.Minutes, 0, 0);
			if (Math.Abs(timeSpan.Ticks) > SmtpDateTime.timeSpanMaxTicks)
			{
				throw new FormatException(global::SR.GetString("The date is in an invalid format."));
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

		internal const string unknownTimeZoneDefaultOffset = "-0000";

		internal const string utcDefaultTimeZoneOffset = "+0000";

		internal const int offsetLength = 5;

		internal const int maxMinuteValue = 59;

		internal const string dateFormatWithDayOfWeek = "ddd, dd MMM yyyy HH:mm:ss";

		internal const string dateFormatWithoutDayOfWeek = "dd MMM yyyy HH:mm:ss";

		internal const string dateFormatWithDayOfWeekAndNoSeconds = "ddd, dd MMM yyyy HH:mm";

		internal const string dateFormatWithoutDayOfWeekAndNoSeconds = "dd MMM yyyy HH:mm";

		internal static readonly string[] validDateTimeFormats = new string[] { "ddd, dd MMM yyyy HH:mm:ss", "dd MMM yyyy HH:mm:ss", "ddd, dd MMM yyyy HH:mm", "dd MMM yyyy HH:mm" };

		internal static readonly char[] allowedWhiteSpaceChars = new char[] { ' ', '\t' };

		internal static readonly IDictionary<string, TimeSpan> timeZoneOffsetLookup = SmtpDateTime.InitializeShortHandLookups();

		internal static readonly long timeSpanMaxTicks = 3599400000000L;

		internal static readonly int offsetMaxValue = 9959;

		private readonly DateTime date;

		private readonly TimeSpan timeZone;

		private readonly bool unknownTimeZone;
	}
}
