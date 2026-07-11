using System;
using System.Text;

namespace System.Xml.Schema
{
	internal struct XsdDuration
	{
		public XsdDuration(bool isNegative, int years, int months, int days, int hours, int minutes, int seconds, int nanoseconds)
		{
			if (years < 0)
			{
				throw new ArgumentOutOfRangeException("years");
			}
			if (months < 0)
			{
				throw new ArgumentOutOfRangeException("months");
			}
			if (days < 0)
			{
				throw new ArgumentOutOfRangeException("days");
			}
			if (hours < 0)
			{
				throw new ArgumentOutOfRangeException("hours");
			}
			if (minutes < 0)
			{
				throw new ArgumentOutOfRangeException("minutes");
			}
			if (seconds < 0)
			{
				throw new ArgumentOutOfRangeException("seconds");
			}
			if (nanoseconds < 0 || nanoseconds > 999999999)
			{
				throw new ArgumentOutOfRangeException("nanoseconds");
			}
			this.years = years;
			this.months = months;
			this.days = days;
			this.hours = hours;
			this.minutes = minutes;
			this.seconds = seconds;
			this.nanoseconds = (uint)nanoseconds;
			if (isNegative)
			{
				this.nanoseconds |= 2147483648U;
			}
		}

		public XsdDuration(TimeSpan timeSpan)
		{
			this = new XsdDuration(timeSpan, XsdDuration.DurationType.Duration);
		}

		public XsdDuration(TimeSpan timeSpan, XsdDuration.DurationType durationType)
		{
			long ticks = timeSpan.Ticks;
			bool flag;
			ulong num;
			if (ticks < 0L)
			{
				flag = true;
				num = (ulong)(-(ulong)ticks);
			}
			else
			{
				flag = false;
				num = (ulong)ticks;
			}
			if (durationType == XsdDuration.DurationType.YearMonthDuration)
			{
				int num2 = (int)(num / 315360000000000UL);
				int num3 = (int)(num % 315360000000000UL / 25920000000000UL);
				if (num3 == 12)
				{
					num2++;
					num3 = 0;
				}
				this = new XsdDuration(flag, num2, num3, 0, 0, 0, 0, 0);
				return;
			}
			this.nanoseconds = (uint)(num % 10000000UL) * 100U;
			if (flag)
			{
				this.nanoseconds |= 2147483648U;
			}
			this.years = 0;
			this.months = 0;
			this.days = (int)(num / 864000000000UL);
			this.hours = (int)(num / 36000000000UL % 24UL);
			this.minutes = (int)(num / 600000000UL % 60UL);
			this.seconds = (int)(num / 10000000UL % 60UL);
		}

		public XsdDuration(string s)
		{
			this = new XsdDuration(s, XsdDuration.DurationType.Duration);
		}

		public XsdDuration(string s, XsdDuration.DurationType durationType)
		{
			XsdDuration xsdDuration;
			Exception ex = XsdDuration.TryParse(s, durationType, out xsdDuration);
			if (ex != null)
			{
				throw ex;
			}
			this.years = xsdDuration.Years;
			this.months = xsdDuration.Months;
			this.days = xsdDuration.Days;
			this.hours = xsdDuration.Hours;
			this.minutes = xsdDuration.Minutes;
			this.seconds = xsdDuration.Seconds;
			this.nanoseconds = (uint)xsdDuration.Nanoseconds;
			if (xsdDuration.IsNegative)
			{
				this.nanoseconds |= 2147483648U;
			}
		}

		public bool IsNegative
		{
			get
			{
				return (this.nanoseconds & 2147483648U) > 0U;
			}
		}

		public int Years
		{
			get
			{
				return this.years;
			}
		}

		public int Months
		{
			get
			{
				return this.months;
			}
		}

		public int Days
		{
			get
			{
				return this.days;
			}
		}

		public int Hours
		{
			get
			{
				return this.hours;
			}
		}

		public int Minutes
		{
			get
			{
				return this.minutes;
			}
		}

		public int Seconds
		{
			get
			{
				return this.seconds;
			}
		}

		public int Nanoseconds
		{
			get
			{
				return (int)(this.nanoseconds & 2147483647U);
			}
		}

		public int Microseconds
		{
			get
			{
				return this.Nanoseconds / 1000;
			}
		}

		public int Milliseconds
		{
			get
			{
				return this.Nanoseconds / 1000000;
			}
		}

		public XsdDuration Normalize()
		{
			int num = this.Years;
			int num2 = this.Months;
			int num3 = this.Days;
			int num4 = this.Hours;
			int num5 = this.Minutes;
			int num6 = this.Seconds;
			checked
			{
				try
				{
					if (num2 >= 12)
					{
						num += num2 / 12;
						num2 %= 12;
					}
					if (num6 >= 60)
					{
						num5 += num6 / 60;
						num6 %= 60;
					}
					if (num5 >= 60)
					{
						num4 += num5 / 60;
						num5 %= 60;
					}
					if (num4 >= 24)
					{
						num3 += num4 / 24;
						num4 %= 24;
					}
				}
				catch (OverflowException)
				{
					throw new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new object[]
					{
						this.ToString(),
						"Duration"
					}));
				}
				return new XsdDuration(this.IsNegative, num, num2, num3, num4, num5, num6, this.Nanoseconds);
			}
		}

		public TimeSpan ToTimeSpan()
		{
			return this.ToTimeSpan(XsdDuration.DurationType.Duration);
		}

		public TimeSpan ToTimeSpan(XsdDuration.DurationType durationType)
		{
			TimeSpan timeSpan;
			Exception ex = this.TryToTimeSpan(durationType, out timeSpan);
			if (ex != null)
			{
				throw ex;
			}
			return timeSpan;
		}

		internal Exception TryToTimeSpan(out TimeSpan result)
		{
			return this.TryToTimeSpan(XsdDuration.DurationType.Duration, out result);
		}

		internal Exception TryToTimeSpan(XsdDuration.DurationType durationType, out TimeSpan result)
		{
			Exception ex = null;
			ulong num = 0UL;
			checked
			{
				try
				{
					if (durationType != XsdDuration.DurationType.DayTimeDuration)
					{
						num += ((ulong)this.years + (ulong)this.months / 12UL) * 365UL;
						num += (ulong)this.months % 12UL * 30UL;
					}
					if (durationType != XsdDuration.DurationType.YearMonthDuration)
					{
						num += (ulong)this.days;
						num *= 24UL;
						num += (ulong)this.hours;
						num *= 60UL;
						num += (ulong)this.minutes;
						num *= 60UL;
						num += (ulong)this.seconds;
						num *= 10000000UL;
						num += (ulong)this.Nanoseconds / 100UL;
					}
					else
					{
						num *= 864000000000UL;
					}
					if (this.IsNegative)
					{
						if (num == 9223372036854775808UL)
						{
							result = new TimeSpan(long.MinValue);
						}
						else
						{
							result = new TimeSpan(0L - (long)num);
						}
					}
					else
					{
						result = new TimeSpan((long)num);
					}
					return null;
				}
				catch (OverflowException)
				{
					result = TimeSpan.MinValue;
					ex = new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new object[] { durationType, "TimeSpan" }));
				}
				return ex;
			}
		}

		public override string ToString()
		{
			return this.ToString(XsdDuration.DurationType.Duration);
		}

		internal string ToString(XsdDuration.DurationType durationType)
		{
			StringBuilder stringBuilder = new StringBuilder(20);
			if (this.IsNegative)
			{
				stringBuilder.Append('-');
			}
			stringBuilder.Append('P');
			if (durationType != XsdDuration.DurationType.DayTimeDuration)
			{
				if (this.years != 0)
				{
					stringBuilder.Append(XmlConvert.ToString(this.years));
					stringBuilder.Append('Y');
				}
				if (this.months != 0)
				{
					stringBuilder.Append(XmlConvert.ToString(this.months));
					stringBuilder.Append('M');
				}
			}
			if (durationType != XsdDuration.DurationType.YearMonthDuration)
			{
				if (this.days != 0)
				{
					stringBuilder.Append(XmlConvert.ToString(this.days));
					stringBuilder.Append('D');
				}
				if (this.hours != 0 || this.minutes != 0 || this.seconds != 0 || this.Nanoseconds != 0)
				{
					stringBuilder.Append('T');
					if (this.hours != 0)
					{
						stringBuilder.Append(XmlConvert.ToString(this.hours));
						stringBuilder.Append('H');
					}
					if (this.minutes != 0)
					{
						stringBuilder.Append(XmlConvert.ToString(this.minutes));
						stringBuilder.Append('M');
					}
					int num = this.Nanoseconds;
					if (this.seconds != 0 || num != 0)
					{
						stringBuilder.Append(XmlConvert.ToString(this.seconds));
						if (num != 0)
						{
							stringBuilder.Append('.');
							int length = stringBuilder.Length;
							stringBuilder.Length += 9;
							int num2 = stringBuilder.Length - 1;
							for (int i = num2; i >= length; i--)
							{
								int num3 = num % 10;
								stringBuilder[i] = (char)(num3 + 48);
								if (num2 == i && num3 == 0)
								{
									num2--;
								}
								num /= 10;
							}
							stringBuilder.Length = num2 + 1;
						}
						stringBuilder.Append('S');
					}
				}
				if (stringBuilder[stringBuilder.Length - 1] == 'P')
				{
					stringBuilder.Append("T0S");
				}
			}
			else if (stringBuilder[stringBuilder.Length - 1] == 'P')
			{
				stringBuilder.Append("0M");
			}
			return stringBuilder.ToString();
		}

		internal static Exception TryParse(string s, out XsdDuration result)
		{
			return XsdDuration.TryParse(s, XsdDuration.DurationType.Duration, out result);
		}

		internal static Exception TryParse(string s, XsdDuration.DurationType durationType, out XsdDuration result)
		{
			XsdDuration.Parts parts = XsdDuration.Parts.HasNone;
			result = default(XsdDuration);
			s = s.Trim();
			int length = s.Length;
			int num = 0;
			int i = 0;
			if (num < length)
			{
				if (s[num] == '-')
				{
					num++;
					result.nanoseconds = 2147483648U;
				}
				else
				{
					result.nanoseconds = 0U;
				}
				if (num < length && s[num++] == 'P')
				{
					int num2;
					if (XsdDuration.TryParseDigits(s, ref num, false, out num2, out i) == null)
					{
						if (num >= length)
						{
							goto IL_02B5;
						}
						if (s[num] == 'Y')
						{
							if (i == 0)
							{
								goto IL_02B5;
							}
							parts |= XsdDuration.Parts.HasYears;
							result.years = num2;
							if (++num == length)
							{
								goto IL_0298;
							}
							if (XsdDuration.TryParseDigits(s, ref num, false, out num2, out i) != null)
							{
								goto IL_02D8;
							}
							if (num >= length)
							{
								goto IL_02B5;
							}
						}
						if (s[num] == 'M')
						{
							if (i == 0)
							{
								goto IL_02B5;
							}
							parts |= XsdDuration.Parts.HasMonths;
							result.months = num2;
							if (++num == length)
							{
								goto IL_0298;
							}
							if (XsdDuration.TryParseDigits(s, ref num, false, out num2, out i) != null)
							{
								goto IL_02D8;
							}
							if (num >= length)
							{
								goto IL_02B5;
							}
						}
						if (s[num] == 'D')
						{
							if (i == 0)
							{
								goto IL_02B5;
							}
							parts |= XsdDuration.Parts.HasDays;
							result.days = num2;
							if (++num == length)
							{
								goto IL_0298;
							}
							if (XsdDuration.TryParseDigits(s, ref num, false, out num2, out i) != null)
							{
								goto IL_02D8;
							}
							if (num >= length)
							{
								goto IL_02B5;
							}
						}
						if (s[num] == 'T')
						{
							if (i != 0)
							{
								goto IL_02B5;
							}
							num++;
							if (XsdDuration.TryParseDigits(s, ref num, false, out num2, out i) != null)
							{
								goto IL_02D8;
							}
							if (num >= length)
							{
								goto IL_02B5;
							}
							if (s[num] == 'H')
							{
								if (i == 0)
								{
									goto IL_02B5;
								}
								parts |= XsdDuration.Parts.HasHours;
								result.hours = num2;
								if (++num == length)
								{
									goto IL_0298;
								}
								if (XsdDuration.TryParseDigits(s, ref num, false, out num2, out i) != null)
								{
									goto IL_02D8;
								}
								if (num >= length)
								{
									goto IL_02B5;
								}
							}
							if (s[num] == 'M')
							{
								if (i == 0)
								{
									goto IL_02B5;
								}
								parts |= XsdDuration.Parts.HasMinutes;
								result.minutes = num2;
								if (++num == length)
								{
									goto IL_0298;
								}
								if (XsdDuration.TryParseDigits(s, ref num, false, out num2, out i) != null)
								{
									goto IL_02D8;
								}
								if (num >= length)
								{
									goto IL_02B5;
								}
							}
							if (s[num] == '.')
							{
								num++;
								parts |= XsdDuration.Parts.HasSeconds;
								result.seconds = num2;
								if (XsdDuration.TryParseDigits(s, ref num, true, out num2, out i) != null)
								{
									goto IL_02D8;
								}
								if (i == 0)
								{
									num2 = 0;
								}
								while (i > 9)
								{
									num2 /= 10;
									i--;
								}
								while (i < 9)
								{
									num2 *= 10;
									i++;
								}
								result.nanoseconds |= (uint)num2;
								if (num >= length || s[num] != 'S')
								{
									goto IL_02B5;
								}
								if (++num == length)
								{
									goto IL_0298;
								}
							}
							else if (s[num] == 'S')
							{
								if (i == 0)
								{
									goto IL_02B5;
								}
								parts |= XsdDuration.Parts.HasSeconds;
								result.seconds = num2;
								if (++num == length)
								{
									goto IL_0298;
								}
							}
						}
						if (i != 0 || num != length)
						{
							goto IL_02B5;
						}
						IL_0298:
						if (parts != XsdDuration.Parts.HasNone)
						{
							if (durationType == XsdDuration.DurationType.DayTimeDuration)
							{
								if ((parts & (XsdDuration.Parts)3) != XsdDuration.Parts.HasNone)
								{
									goto IL_02B5;
								}
							}
							else if (durationType == XsdDuration.DurationType.YearMonthDuration && (parts & (XsdDuration.Parts)(-4)) != XsdDuration.Parts.HasNone)
							{
								goto IL_02B5;
							}
							return null;
						}
						goto IL_02B5;
					}
					IL_02D8:
					return new OverflowException(Res.GetString("Value '{0}' was either too large or too small for {1}.", new object[] { s, durationType }));
				}
			}
			IL_02B5:
			return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[] { s, durationType }));
		}

		private static string TryParseDigits(string s, ref int offset, bool eatDigits, out int result, out int numDigits)
		{
			int num = offset;
			int length = s.Length;
			result = 0;
			numDigits = 0;
			while (offset < length && s[offset] >= '0' && s[offset] <= '9')
			{
				int num2 = (int)(s[offset] - '0');
				if (result > (2147483647 - num2) / 10)
				{
					if (!eatDigits)
					{
						return "Value '{0}' was either too large or too small for {1}.";
					}
					numDigits = offset - num;
					while (offset < length && s[offset] >= '0' && s[offset] <= '9')
					{
						offset++;
					}
					return null;
				}
				else
				{
					result = result * 10 + num2;
					offset++;
				}
			}
			numDigits = offset - num;
			return null;
		}

		private int years;

		private int months;

		private int days;

		private int hours;

		private int minutes;

		private int seconds;

		private uint nanoseconds;

		private const uint NegativeBit = 2147483648U;

		private enum Parts
		{
			HasNone,
			HasYears,
			HasMonths,
			HasDays = 4,
			HasHours = 8,
			HasMinutes = 16,
			HasSeconds = 32
		}

		public enum DurationType
		{
			Duration,
			YearMonthDuration,
			DayTimeDuration
		}
	}
}
