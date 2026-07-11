using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct DateTimeOffset : IFormattable, IComparable, ISerializable, IComparable<DateTimeOffset>, IEquatable<DateTimeOffset>, IDeserializationCallback
	{
		public DateTimeOffset(DateTime dateTime)
		{
			this.dt = dateTime;
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				this.utc_offset = TimeSpan.Zero;
			}
			else
			{
				this.utc_offset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
			}
			if (this.UtcDateTime < DateTime.MinValue || this.UtcDateTime > DateTime.MaxValue)
			{
				throw new ArgumentOutOfRangeException("The UTC date and time that results from applying the offset is earlier than MinValue or later than MaxValue.");
			}
		}

		public DateTimeOffset(DateTime dateTime, TimeSpan offset)
		{
			if (dateTime.Kind == DateTimeKind.Utc && offset != TimeSpan.Zero)
			{
				throw new ArgumentException("dateTime.Kind equals Utc and offset does not equal zero.");
			}
			if (dateTime.Kind == DateTimeKind.Local && offset != TimeZone.CurrentTimeZone.GetUtcOffset(dateTime))
			{
				throw new ArgumentException("dateTime.Kind equals Local and offset does not equal the offset of the system's local time zone.");
			}
			if (offset.Ticks % 600000000L != 0L)
			{
				throw new ArgumentException("offset is not specified in whole minutes.");
			}
			if (offset < new TimeSpan(-14, 0, 0) || offset > new TimeSpan(14, 0, 0))
			{
				throw new ArgumentOutOfRangeException("offset is less than -14 hours or greater than 14 hours.");
			}
			this.dt = dateTime;
			this.utc_offset = offset;
			if (this.UtcDateTime < DateTime.MinValue || this.UtcDateTime > DateTime.MaxValue)
			{
				throw new ArgumentOutOfRangeException("The UtcDateTime property is earlier than MinValue or later than MaxValue.");
			}
		}

		public DateTimeOffset(long ticks, TimeSpan offset)
		{
			this = new DateTimeOffset(new DateTime(ticks), offset);
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, TimeSpan offset)
		{
			this = new DateTimeOffset(new DateTime(year, month, day, hour, minute, second), offset);
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeSpan offset)
		{
			this = new DateTimeOffset(new DateTime(year, month, day, hour, minute, second, millisecond), offset);
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, TimeSpan offset)
		{
			this = new DateTimeOffset(new DateTime(year, month, day, hour, minute, second, millisecond, calendar), offset);
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		private DateTimeOffset(SerializationInfo info, StreamingContext context)
		{
			DateTime dateTime = (DateTime)info.GetValue("DateTime", typeof(DateTime));
			short @int = info.GetInt16("OffsetMinutes");
			this.utc_offset = TimeSpan.FromMinutes((double)@int);
			this.dt = dateTime.Add(this.utc_offset);
		}

		int IComparable.CompareTo(object obj)
		{
			return this.CompareTo((DateTimeOffset)obj);
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			DateTime dateTime = new DateTime(this.dt.Ticks);
			DateTime dateTime2 = dateTime.Subtract(this.utc_offset);
			info.AddValue("DateTime", dateTime2);
			info.AddValue("OffsetMinutes", (short)this.utc_offset.TotalMinutes);
		}

		[MonoTODO]
		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		public DateTimeOffset Add(TimeSpan timeSpan)
		{
			return new DateTimeOffset(this.dt.Add(timeSpan), this.utc_offset);
		}

		public DateTimeOffset AddDays(double days)
		{
			return new DateTimeOffset(this.dt.AddDays(days), this.utc_offset);
		}

		public DateTimeOffset AddHours(double hours)
		{
			return new DateTimeOffset(this.dt.AddHours(hours), this.utc_offset);
		}

		public DateTimeOffset AddMilliseconds(double milliseconds)
		{
			return new DateTimeOffset(this.dt.AddMilliseconds(milliseconds), this.utc_offset);
		}

		public DateTimeOffset AddMinutes(double minutes)
		{
			return new DateTimeOffset(this.dt.AddMinutes(minutes), this.utc_offset);
		}

		public DateTimeOffset AddMonths(int months)
		{
			return new DateTimeOffset(this.dt.AddMonths(months), this.utc_offset);
		}

		public DateTimeOffset AddSeconds(double seconds)
		{
			return new DateTimeOffset(this.dt.AddSeconds(seconds), this.utc_offset);
		}

		public DateTimeOffset AddTicks(long ticks)
		{
			return new DateTimeOffset(this.dt.AddTicks(ticks), this.utc_offset);
		}

		public DateTimeOffset AddYears(int years)
		{
			return new DateTimeOffset(this.dt.AddYears(years), this.utc_offset);
		}

		public static int Compare(DateTimeOffset first, DateTimeOffset second)
		{
			return first.CompareTo(second);
		}

		public int CompareTo(DateTimeOffset other)
		{
			return this.UtcDateTime.CompareTo(other.UtcDateTime);
		}

		public bool Equals(DateTimeOffset other)
		{
			return this.UtcDateTime == other.UtcDateTime;
		}

		public override bool Equals(object obj)
		{
			return obj is DateTimeOffset && this.UtcDateTime == ((DateTimeOffset)obj).UtcDateTime;
		}

		public static bool Equals(DateTimeOffset first, DateTimeOffset second)
		{
			return first.Equals(second);
		}

		public bool EqualsExact(DateTimeOffset other)
		{
			return this.dt == other.dt && this.utc_offset == other.utc_offset;
		}

		public static DateTimeOffset FromFileTime(long fileTime)
		{
			if (fileTime < 0L || fileTime > DateTimeOffset.MaxValue.Ticks)
			{
				throw new ArgumentOutOfRangeException("fileTime is less than zero or greater than DateTimeOffset.MaxValue.Ticks.");
			}
			return new DateTimeOffset(DateTime.FromFileTime(fileTime), TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.FromFileTime(fileTime)));
		}

		public override int GetHashCode()
		{
			return this.dt.GetHashCode() ^ this.utc_offset.GetHashCode();
		}

		public static DateTimeOffset Parse(string input)
		{
			return DateTimeOffset.Parse(input, null);
		}

		public static DateTimeOffset Parse(string input, IFormatProvider formatProvider)
		{
			return DateTimeOffset.Parse(input, formatProvider, DateTimeStyles.AllowWhiteSpaces);
		}

		public static DateTimeOffset Parse(string input, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			Exception ex = null;
			DateTime dateTime;
			DateTimeOffset dateTimeOffset;
			if (!DateTime.CoreParse(input, formatProvider, styles, out dateTime, out dateTimeOffset, true, ref ex))
			{
				throw ex;
			}
			if (dateTime.Ticks != 0L && dateTimeOffset.Ticks == 0L)
			{
				throw new ArgumentOutOfRangeException("The UTC representation falls outside the 1-9999 year range");
			}
			return dateTimeOffset;
		}

		public static DateTimeOffset ParseExact(string input, string format, IFormatProvider formatProvider)
		{
			return DateTimeOffset.ParseExact(input, format, formatProvider, DateTimeStyles.AssumeLocal);
		}

		public static DateTimeOffset ParseExact(string input, string format, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (format == string.Empty)
			{
				throw new FormatException("format is an empty string");
			}
			return DateTimeOffset.ParseExact(input, new string[] { format }, formatProvider, styles);
		}

		public static DateTimeOffset ParseExact(string input, string[] formats, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (input == string.Empty)
			{
				throw new FormatException("input is an empty string");
			}
			if (formats == null)
			{
				throw new ArgumentNullException("formats");
			}
			if (formats.Length == 0)
			{
				throw new FormatException("Invalid format specifier");
			}
			if ((styles & DateTimeStyles.AssumeLocal) != DateTimeStyles.None && (styles & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None)
			{
				throw new ArgumentException("styles parameter contains incompatible flags");
			}
			DateTimeOffset dateTimeOffset;
			if (!DateTimeOffset.ParseExact(input, formats, DateTimeFormatInfo.GetInstance(formatProvider), styles, out dateTimeOffset))
			{
				throw new FormatException("Invalid format string");
			}
			return dateTimeOffset;
		}

		private static bool ParseExact(string input, string[] formats, DateTimeFormatInfo dfi, DateTimeStyles styles, out DateTimeOffset ret)
		{
			foreach (string text in formats)
			{
				if (text == null || text == string.Empty)
				{
					throw new FormatException("Invalid format string");
				}
				DateTimeOffset dateTimeOffset;
				if (DateTimeOffset.DoParse(input, text, false, out dateTimeOffset, dfi, styles))
				{
					ret = dateTimeOffset;
					return true;
				}
			}
			ret = DateTimeOffset.MinValue;
			return false;
		}

		private static bool DoParse(string input, string format, bool exact, out DateTimeOffset result, DateTimeFormatInfo dfi, DateTimeStyles styles)
		{
			if ((styles & DateTimeStyles.AllowLeadingWhite) != DateTimeStyles.None)
			{
				format = format.TrimStart(null);
				input = input.TrimStart(null);
			}
			if ((styles & DateTimeStyles.AllowTrailingWhite) != DateTimeStyles.None)
			{
				format = format.TrimEnd(null);
				input = input.TrimEnd(null);
			}
			bool flag = false;
			if ((styles & DateTimeStyles.AllowInnerWhite) != DateTimeStyles.None)
			{
				flag = true;
			}
			bool flag2 = false;
			bool flag3 = false;
			if (format.Length == 1)
			{
				format = DateTimeUtils.GetStandardPattern(format[0], dfi, out flag2, out flag3, true);
			}
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			double num8 = -1.0;
			int num9 = -1;
			TimeSpan timeSpan = TimeSpan.MinValue;
			result = DateTimeOffset.MinValue;
			int i = 0;
			int num10 = 0;
			while (i < format.Length)
			{
				char c = format[i];
				char c2 = c;
				int num11;
				switch (c2)
				{
				case 's':
					num11 = DateTimeUtils.CountRepeat(format, i, c);
					if (num7 != -1 || num11 > 2)
					{
						return false;
					}
					num10 += DateTimeOffset.ParseNumber(input, num10, 2, num11 == 2, flag, out num7);
					break;
				case 't':
				{
					num11 = DateTimeUtils.CountRepeat(format, i, c);
					if (num5 != -1 || num11 > 2)
					{
						return false;
					}
					int num12 = num10;
					string text = input;
					int num13 = num10;
					string[] array2;
					if (num11 == 1)
					{
						string[] array = new string[2];
						array[0] = new string(dfi.AMDesignator[0], 1);
						array2 = array;
						array[1] = new string(dfi.PMDesignator[0], 0);
					}
					else
					{
						string[] array3 = new string[2];
						array3[0] = dfi.AMDesignator;
						array2 = array3;
						array3[1] = dfi.PMDesignator;
					}
					num10 = num12 + DateTimeOffset.ParseEnum(text, num13, array2, flag, out num9);
					if (num9 == -1)
					{
						return false;
					}
					if (num4 == -1)
					{
						num4 = num9 * 12;
					}
					else
					{
						num5 = num4 + num9 * 12;
					}
					break;
				}
				default:
					switch (c2)
					{
					case 'd':
						num11 = DateTimeUtils.CountRepeat(format, i, c);
						if (num3 != -1 || num11 > 4)
						{
							return false;
						}
						if (num11 <= 2)
						{
							num10 += DateTimeOffset.ParseNumber(input, num10, 2, num11 == 2, flag, out num3);
						}
						else
						{
							num10 += DateTimeOffset.ParseEnum(input, num10, (num11 != 3) ? dfi.DayNames : dfi.AbbreviatedDayNames, flag, out num9);
						}
						break;
					default:
						switch (c2)
						{
						case 'F':
						{
							num11 = DateTimeUtils.CountRepeat(format, i, c);
							int num15;
							int num14 = DateTimeOffset.ParseNumber(input, num10, num11, true, flag, out num9, out num15);
							if (num9 == -1)
							{
								num10 += DateTimeOffset.ParseNumber(input, num10, num15, true, flag, out num9);
							}
							else
							{
								num10 += num14;
							}
							if (num8 >= 0.0 || num11 > 7 || num9 == -1)
							{
								return false;
							}
							num8 = (double)num9 / Math.Pow(10.0, (double)num15);
							break;
						}
						default:
							if (c2 != ' ')
							{
								if (c2 != '%')
								{
									if (c2 != '/')
									{
										if (c2 != ':')
										{
											if (c2 != 'M')
											{
												if (c2 != '\\')
												{
													if (c2 != 'm')
													{
														num11 = 1;
														num10 += DateTimeOffset.ParseChar(input, num10, format[i], flag, out num9);
														if (num9 == -1)
														{
															return false;
														}
													}
													else
													{
														num11 = DateTimeUtils.CountRepeat(format, i, c);
														if (num6 != -1 || num11 > 2)
														{
															return false;
														}
														num10 += DateTimeOffset.ParseNumber(input, num10, 2, num11 == 2, flag, out num6);
													}
												}
												else
												{
													num11 = 2;
													num10 += DateTimeOffset.ParseChar(input, num10, format[i + 1], flag, out num9);
													if (num9 == -1)
													{
														return false;
													}
												}
											}
											else
											{
												num11 = DateTimeUtils.CountRepeat(format, i, c);
												if (num2 != -1 || num11 > 4)
												{
													return false;
												}
												if (num11 <= 2)
												{
													num10 += DateTimeOffset.ParseNumber(input, num10, 2, num11 == 2, flag, out num2);
												}
												else
												{
													num10 += DateTimeOffset.ParseEnum(input, num10, (num11 != 3) ? dfi.MonthNames : dfi.AbbreviatedMonthNames, flag, out num2);
													num2++;
												}
											}
										}
										else
										{
											num11 = 1;
											num10 += DateTimeOffset.ParseEnum(input, num10, new string[] { dfi.TimeSeparator }, false, out num9);
											if (num9 == -1)
											{
												return false;
											}
										}
									}
									else
									{
										num11 = 1;
										num10 += DateTimeOffset.ParseEnum(input, num10, new string[] { dfi.DateSeparator }, false, out num9);
										if (num9 == -1)
										{
											return false;
										}
									}
								}
								else
								{
									num11 = 1;
									if (i != 0)
									{
										return false;
									}
								}
							}
							else
							{
								num11 = 1;
								num10 += DateTimeOffset.ParseChar(input, num10, ' ', false, out num9);
								if (num9 == -1)
								{
									return false;
								}
							}
							break;
						case 'H':
							num11 = DateTimeUtils.CountRepeat(format, i, c);
							if (num5 != -1 || num11 > 2)
							{
								return false;
							}
							num10 += DateTimeOffset.ParseNumber(input, num10, 2, num11 == 2, flag, out num5);
							break;
						}
						break;
					case 'f':
						num11 = DateTimeUtils.CountRepeat(format, i, c);
						num10 += DateTimeOffset.ParseNumber(input, num10, num11, true, flag, out num9);
						if (num8 >= 0.0 || num11 > 7 || num9 == -1)
						{
							return false;
						}
						num8 = (double)num9 / Math.Pow(10.0, (double)num11);
						break;
					case 'h':
						num11 = DateTimeUtils.CountRepeat(format, i, c);
						if (num5 != -1 || num11 > 2)
						{
							return false;
						}
						num10 += DateTimeOffset.ParseNumber(input, num10, 2, num11 == 2, flag, out num9);
						if (num9 == -1)
						{
							return false;
						}
						if (num4 == -1)
						{
							num4 = num9;
						}
						else
						{
							num5 = num4 + num9;
						}
						break;
					}
					break;
				case 'y':
					if (num != -1)
					{
						return false;
					}
					num11 = DateTimeUtils.CountRepeat(format, i, c);
					if (num11 <= 2)
					{
						num10 += DateTimeOffset.ParseNumber(input, num10, 2, num11 == 2, flag, out num);
						if (num != -1)
						{
							num += DateTime.Now.Year - DateTime.Now.Year % 100;
						}
					}
					else if (num11 <= 4)
					{
						int num16;
						num10 += DateTimeOffset.ParseNumber(input, num10, 5, false, flag, out num, out num16);
						if (num16 < num11 || (num16 > num11 && (double)num / Math.Pow(10.0, (double)(num16 - 1)) < 1.0))
						{
							return false;
						}
					}
					else
					{
						num10 += DateTimeOffset.ParseNumber(input, num10, num11, true, flag, out num);
					}
					break;
				case 'z':
				{
					num11 = DateTimeUtils.CountRepeat(format, i, c);
					if (timeSpan != TimeSpan.MinValue || num11 > 3)
					{
						return false;
					}
					int num17 = 0;
					num9 = 0;
					int num18;
					num10 += DateTimeOffset.ParseEnum(input, num10, new string[] { "-", "+" }, flag, out num18);
					int num19;
					num10 += DateTimeOffset.ParseNumber(input, num10, 2, num11 != 1, false, out num19);
					if (num11 == 3)
					{
						num10 += DateTimeOffset.ParseEnum(input, num10, new string[] { dfi.TimeSeparator }, false, out num9);
						num10 += DateTimeOffset.ParseNumber(input, num10, 2, true, false, out num17);
					}
					if (num19 == -1 || num17 == -1 || num18 == -1)
					{
						return false;
					}
					if (num18 == 0)
					{
						num18 = -1;
					}
					timeSpan = new TimeSpan(num18 * num19, num18 * num17, 0);
					break;
				}
				}
				i += num11;
			}
			if (timeSpan == TimeSpan.MinValue && (styles & DateTimeStyles.AssumeLocal) != DateTimeStyles.None)
			{
				timeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
			}
			if (timeSpan == TimeSpan.MinValue && (styles & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None)
			{
				timeSpan = TimeSpan.Zero;
			}
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 < 0)
			{
				num6 = 0;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 < 0.0)
			{
				num8 = 0.0;
			}
			if (num > 0 && num2 > 0 && num3 > 0)
			{
				result = new DateTimeOffset(num, num2, num3, num5, num6, num7, 0, timeSpan);
				result = result.AddSeconds(num8);
				if ((styles & DateTimeStyles.AdjustToUniversal) != DateTimeStyles.None)
				{
					result = result.ToUniversalTime();
				}
				return true;
			}
			return false;
		}

		private static int ParseNumber(string input, int pos, int digits, bool leading_zero, bool allow_leading_white, out int result)
		{
			int num;
			return DateTimeOffset.ParseNumber(input, pos, digits, leading_zero, allow_leading_white, out result, out num);
		}

		private static int ParseNumber(string input, int pos, int digits, bool leading_zero, bool allow_leading_white, out int result, out int digit_parsed)
		{
			int num = 0;
			digit_parsed = 0;
			result = 0;
			while (allow_leading_white && pos < input.Length && input[pos] == ' ')
			{
				num++;
				pos++;
			}
			while (pos < input.Length && char.IsDigit(input[pos]) && digits > 0)
			{
				result = 10 * result + (int)((byte)(input[pos] - '0'));
				pos++;
				num++;
				digit_parsed++;
				digits--;
			}
			if (leading_zero && digits > 0)
			{
				result = -1;
			}
			if (digit_parsed == 0)
			{
				result = -1;
			}
			return num;
		}

		private static int ParseEnum(string input, int pos, string[] enums, bool allow_leading_white, out int result)
		{
			int num = 0;
			result = -1;
			while (allow_leading_white && pos < input.Length && input[pos] == ' ')
			{
				num++;
				pos++;
			}
			for (int i = 0; i < enums.Length; i++)
			{
				if (input.Substring(pos).StartsWith(enums[i]))
				{
					result = i;
					break;
				}
			}
			if (result >= 0)
			{
				num += enums[result].Length;
			}
			return num;
		}

		private static int ParseChar(string input, int pos, char c, bool allow_leading_white, out int result)
		{
			int num = 0;
			result = -1;
			while (allow_leading_white && pos < input.Length && input[pos] == ' ')
			{
				pos++;
				num++;
			}
			if (pos < input.Length && input[pos] == c)
			{
				result = (int)c;
				num++;
			}
			return num;
		}

		public TimeSpan Subtract(DateTimeOffset value)
		{
			return this.UtcDateTime - value.UtcDateTime;
		}

		public DateTimeOffset Subtract(TimeSpan value)
		{
			return this.Add(-value);
		}

		public long ToFileTime()
		{
			return this.UtcDateTime.ToFileTime();
		}

		public DateTimeOffset ToLocalTime()
		{
			return new DateTimeOffset(this.UtcDateTime.ToLocalTime(), TimeZone.CurrentTimeZone.GetUtcOffset(this.UtcDateTime.ToLocalTime()));
		}

		public DateTimeOffset ToOffset(TimeSpan offset)
		{
			return new DateTimeOffset(this.dt - this.utc_offset + offset, offset);
		}

		public override string ToString()
		{
			return this.ToString(null, null);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return this.ToString(null, formatProvider);
		}

		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(formatProvider);
			if (format == null || format == string.Empty)
			{
				format = instance.ShortDatePattern + " " + instance.LongTimePattern + " zzz";
			}
			bool flag = false;
			bool flag2 = false;
			if (format.Length == 1)
			{
				char c = format[0];
				try
				{
					format = DateTimeUtils.GetStandardPattern(c, instance, out flag, out flag2, true);
				}
				catch
				{
					format = null;
				}
				if (format == null)
				{
					throw new FormatException("format is not one of the format specifier characters defined for DateTimeFormatInfo");
				}
			}
			return (!flag) ? DateTimeUtils.ToString(this.DateTime, new TimeSpan?(this.Offset), format, instance) : DateTimeUtils.ToString(this.UtcDateTime, new TimeSpan?(TimeSpan.Zero), format, instance);
		}

		public DateTimeOffset ToUniversalTime()
		{
			return new DateTimeOffset(this.UtcDateTime, TimeSpan.Zero);
		}

		public static bool TryParse(string input, out DateTimeOffset result)
		{
			bool flag;
			try
			{
				result = DateTimeOffset.Parse(input);
				flag = true;
			}
			catch
			{
				result = DateTimeOffset.MinValue;
				flag = false;
			}
			return flag;
		}

		public static bool TryParse(string input, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			bool flag;
			try
			{
				result = DateTimeOffset.Parse(input, formatProvider, styles);
				flag = true;
			}
			catch
			{
				result = DateTimeOffset.MinValue;
				flag = false;
			}
			return flag;
		}

		public static bool TryParseExact(string input, string format, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			bool flag;
			try
			{
				result = DateTimeOffset.ParseExact(input, format, formatProvider, styles);
				flag = true;
			}
			catch
			{
				result = DateTimeOffset.MinValue;
				flag = false;
			}
			return flag;
		}

		public static bool TryParseExact(string input, string[] formats, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			bool flag;
			try
			{
				result = DateTimeOffset.ParseExact(input, formats, formatProvider, styles);
				flag = true;
			}
			catch
			{
				result = DateTimeOffset.MinValue;
				flag = false;
			}
			return flag;
		}

		public DateTime Date
		{
			get
			{
				return DateTime.SpecifyKind(this.dt.Date, DateTimeKind.Unspecified);
			}
		}

		public DateTime DateTime
		{
			get
			{
				return DateTime.SpecifyKind(this.dt, DateTimeKind.Unspecified);
			}
		}

		public int Day
		{
			get
			{
				return this.dt.Day;
			}
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return this.dt.DayOfWeek;
			}
		}

		public int DayOfYear
		{
			get
			{
				return this.dt.DayOfYear;
			}
		}

		public int Hour
		{
			get
			{
				return this.dt.Hour;
			}
		}

		public DateTime LocalDateTime
		{
			get
			{
				return this.UtcDateTime.ToLocalTime();
			}
		}

		public int Millisecond
		{
			get
			{
				return this.dt.Millisecond;
			}
		}

		public int Minute
		{
			get
			{
				return this.dt.Minute;
			}
		}

		public int Month
		{
			get
			{
				return this.dt.Month;
			}
		}

		public static DateTimeOffset Now
		{
			get
			{
				return new DateTimeOffset(DateTime.Now);
			}
		}

		public TimeSpan Offset
		{
			get
			{
				return this.utc_offset;
			}
		}

		public int Second
		{
			get
			{
				return this.dt.Second;
			}
		}

		public long Ticks
		{
			get
			{
				return this.dt.Ticks;
			}
		}

		public TimeSpan TimeOfDay
		{
			get
			{
				return this.dt.TimeOfDay;
			}
		}

		public DateTime UtcDateTime
		{
			get
			{
				return DateTime.SpecifyKind(this.dt - this.utc_offset, DateTimeKind.Utc);
			}
		}

		public static DateTimeOffset UtcNow
		{
			get
			{
				return new DateTimeOffset(DateTime.UtcNow);
			}
		}

		public long UtcTicks
		{
			get
			{
				return this.UtcDateTime.Ticks;
			}
		}

		public int Year
		{
			get
			{
				return this.dt.Year;
			}
		}

		public static DateTimeOffset operator +(DateTimeOffset dateTimeTz, TimeSpan timeSpan)
		{
			return dateTimeTz.Add(timeSpan);
		}

		public static bool operator ==(DateTimeOffset left, DateTimeOffset right)
		{
			return left.Equals(right);
		}

		public static bool operator >(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime > right.UtcDateTime;
		}

		public static bool operator >=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime >= right.UtcDateTime;
		}

		public static implicit operator DateTimeOffset(DateTime dateTime)
		{
			return new DateTimeOffset(dateTime);
		}

		public static bool operator !=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime != right.UtcDateTime;
		}

		public static bool operator <(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime < right.UtcDateTime;
		}

		public static bool operator <=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime <= right.UtcDateTime;
		}

		public static TimeSpan operator -(DateTimeOffset left, DateTimeOffset right)
		{
			return left.Subtract(right);
		}

		public static DateTimeOffset operator -(DateTimeOffset dateTimeTz, TimeSpan timeSpan)
		{
			return dateTimeTz.Subtract(timeSpan);
		}

		public static readonly DateTimeOffset MaxValue = new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero);

		public static readonly DateTimeOffset MinValue = new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero);

		private DateTime dt;

		private TimeSpan utc_offset;
	}
}
