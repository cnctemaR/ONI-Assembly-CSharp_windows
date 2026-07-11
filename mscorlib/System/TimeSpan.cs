using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public struct TimeSpan : IComparable, IComparable<TimeSpan>, IEquatable<TimeSpan>
	{
		public TimeSpan(long ticks)
		{
			this._ticks = ticks;
		}

		public TimeSpan(int hours, int minutes, int seconds)
		{
			this._ticks = TimeSpan.CalculateTicks(0, hours, minutes, seconds, 0);
		}

		public TimeSpan(int days, int hours, int minutes, int seconds)
		{
			this._ticks = TimeSpan.CalculateTicks(days, hours, minutes, seconds, 0);
		}

		public TimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
		{
			this._ticks = TimeSpan.CalculateTicks(days, hours, minutes, seconds, milliseconds);
		}

		internal static long CalculateTicks(int days, int hours, int minutes, int seconds, int milliseconds)
		{
			int num = hours * 3600;
			int num2 = minutes * 60;
			long num3 = (long)(num + num2 + seconds) * 1000L + (long)milliseconds;
			num3 *= 10000L;
			bool flag = false;
			if (days > 0)
			{
				long num4 = 864000000000L * (long)days;
				if (num3 < 0L)
				{
					long num5 = num3;
					num3 += num4;
					flag = num5 > num3;
				}
				else
				{
					num3 += num4;
					flag = num3 < 0L;
				}
			}
			else if (days < 0)
			{
				long num6 = 864000000000L * (long)days;
				if (num3 <= 0L)
				{
					num3 += num6;
					flag = num3 > 0L;
				}
				else
				{
					long num7 = num3;
					num3 += num6;
					flag = num3 > num7;
				}
			}
			if (flag)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("The timespan is too big or too small."));
			}
			return num3;
		}

		public int Days
		{
			get
			{
				return (int)(this._ticks / 864000000000L);
			}
		}

		public int Hours
		{
			get
			{
				return (int)(this._ticks % 864000000000L / 36000000000L);
			}
		}

		public int Milliseconds
		{
			get
			{
				return (int)(this._ticks % 10000000L / 10000L);
			}
		}

		public int Minutes
		{
			get
			{
				return (int)(this._ticks % 36000000000L / 600000000L);
			}
		}

		public int Seconds
		{
			get
			{
				return (int)(this._ticks % 600000000L / 10000000L);
			}
		}

		public long Ticks
		{
			get
			{
				return this._ticks;
			}
		}

		public double TotalDays
		{
			get
			{
				return (double)this._ticks / 864000000000.0;
			}
		}

		public double TotalHours
		{
			get
			{
				return (double)this._ticks / 36000000000.0;
			}
		}

		public double TotalMilliseconds
		{
			get
			{
				return (double)this._ticks / 10000.0;
			}
		}

		public double TotalMinutes
		{
			get
			{
				return (double)this._ticks / 600000000.0;
			}
		}

		public double TotalSeconds
		{
			get
			{
				return (double)this._ticks / 10000000.0;
			}
		}

		public TimeSpan Add(TimeSpan ts)
		{
			TimeSpan timeSpan;
			try
			{
				timeSpan = new TimeSpan(checked(this._ticks + ts.Ticks));
			}
			catch (OverflowException)
			{
				throw new OverflowException(Locale.GetText("Resulting timespan is too big."));
			}
			return timeSpan;
		}

		public static int Compare(TimeSpan t1, TimeSpan t2)
		{
			if (t1._ticks < t2._ticks)
			{
				return -1;
			}
			if (t1._ticks > t2._ticks)
			{
				return 1;
			}
			return 0;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is TimeSpan))
			{
				throw new ArgumentException(Locale.GetText("Argument has to be a TimeSpan."), "value");
			}
			return TimeSpan.Compare(this, (TimeSpan)value);
		}

		public int CompareTo(TimeSpan value)
		{
			return TimeSpan.Compare(this, value);
		}

		public bool Equals(TimeSpan obj)
		{
			return obj._ticks == this._ticks;
		}

		public TimeSpan Duration()
		{
			TimeSpan timeSpan;
			try
			{
				timeSpan = new TimeSpan(Math.Abs(this._ticks));
			}
			catch (OverflowException)
			{
				throw new OverflowException(Locale.GetText("This TimeSpan value is MinValue so you cannot get the duration."));
			}
			return timeSpan;
		}

		public override bool Equals(object value)
		{
			return value is TimeSpan && this._ticks == ((TimeSpan)value)._ticks;
		}

		public static bool Equals(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks == t2._ticks;
		}

		public static TimeSpan FromDays(double value)
		{
			return TimeSpan.From(value, 864000000000L);
		}

		public static TimeSpan FromHours(double value)
		{
			return TimeSpan.From(value, 36000000000L);
		}

		public static TimeSpan FromMinutes(double value)
		{
			return TimeSpan.From(value, 600000000L);
		}

		public static TimeSpan FromSeconds(double value)
		{
			return TimeSpan.From(value, 10000000L);
		}

		public static TimeSpan FromMilliseconds(double value)
		{
			return TimeSpan.From(value, 10000L);
		}

		private static TimeSpan From(double value, long tickMultiplicator)
		{
			if (double.IsNaN(value))
			{
				throw new ArgumentException(Locale.GetText("Value cannot be NaN."), "value");
			}
			if (double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value) || value < (double)TimeSpan.MinValue.Ticks || value > (double)TimeSpan.MaxValue.Ticks)
			{
				throw new OverflowException(Locale.GetText("Outside range [MinValue,MaxValue]"));
			}
			TimeSpan timeSpan;
			try
			{
				value *= (double)(tickMultiplicator / 10000L);
				checked
				{
					long num = (long)Math.Round(value);
					timeSpan = new TimeSpan(num * 10000L);
				}
			}
			catch (OverflowException)
			{
				throw new OverflowException(Locale.GetText("Resulting timespan is too big."));
			}
			return timeSpan;
		}

		public static TimeSpan FromTicks(long value)
		{
			return new TimeSpan(value);
		}

		public override int GetHashCode()
		{
			return this._ticks.GetHashCode();
		}

		public TimeSpan Negate()
		{
			if (this._ticks == TimeSpan.MinValue._ticks)
			{
				throw new OverflowException(Locale.GetText("This TimeSpan value is MinValue and cannot be negated."));
			}
			return new TimeSpan(-this._ticks);
		}

		public static TimeSpan Parse(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			TimeSpan.Parser parser = new TimeSpan.Parser(s);
			return parser.Execute();
		}

		public static bool TryParse(string s, out TimeSpan result)
		{
			if (s == null)
			{
				result = TimeSpan.Zero;
				return false;
			}
			bool flag;
			try
			{
				result = TimeSpan.Parse(s);
				flag = true;
			}
			catch
			{
				result = TimeSpan.Zero;
				flag = false;
			}
			return flag;
		}

		public TimeSpan Subtract(TimeSpan ts)
		{
			TimeSpan timeSpan;
			try
			{
				timeSpan = new TimeSpan(checked(this._ticks - ts.Ticks));
			}
			catch (OverflowException)
			{
				throw new OverflowException(Locale.GetText("Resulting timespan is too big."));
			}
			return timeSpan;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(14);
			if (this._ticks < 0L)
			{
				stringBuilder.Append('-');
			}
			if (this.Days != 0)
			{
				stringBuilder.Append(Math.Abs(this.Days));
				stringBuilder.Append('.');
			}
			stringBuilder.Append(Math.Abs(this.Hours).ToString("D2"));
			stringBuilder.Append(':');
			stringBuilder.Append(Math.Abs(this.Minutes).ToString("D2"));
			stringBuilder.Append(':');
			stringBuilder.Append(Math.Abs(this.Seconds).ToString("D2"));
			int num = (int)Math.Abs(this._ticks % 10000000L);
			if (num != 0)
			{
				stringBuilder.Append('.');
				stringBuilder.Append(num.ToString("D7"));
			}
			return stringBuilder.ToString();
		}

		public static TimeSpan operator +(TimeSpan t1, TimeSpan t2)
		{
			return t1.Add(t2);
		}

		public static bool operator ==(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks == t2._ticks;
		}

		public static bool operator >(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks > t2._ticks;
		}

		public static bool operator >=(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks >= t2._ticks;
		}

		public static bool operator !=(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks != t2._ticks;
		}

		public static bool operator <(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks < t2._ticks;
		}

		public static bool operator <=(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks <= t2._ticks;
		}

		public static TimeSpan operator -(TimeSpan t1, TimeSpan t2)
		{
			return t1.Subtract(t2);
		}

		public static TimeSpan operator -(TimeSpan t)
		{
			return t.Negate();
		}

		public static TimeSpan operator +(TimeSpan t)
		{
			return t;
		}

		public const long TicksPerDay = 864000000000L;

		public const long TicksPerHour = 36000000000L;

		public const long TicksPerMillisecond = 10000L;

		public const long TicksPerMinute = 600000000L;

		public const long TicksPerSecond = 10000000L;

		public static readonly TimeSpan MaxValue = new TimeSpan(long.MaxValue);

		public static readonly TimeSpan MinValue = new TimeSpan(long.MinValue);

		public static readonly TimeSpan Zero = new TimeSpan(0L);

		private long _ticks;

		private class Parser
		{
			public Parser(string src)
			{
				this._src = src;
				this._length = this._src.Length;
			}

			public bool AtEnd
			{
				get
				{
					return this._cur >= this._length;
				}
			}

			private void ParseWhiteSpace()
			{
				while (!this.AtEnd && char.IsWhiteSpace(this._src, this._cur))
				{
					this._cur++;
				}
			}

			private bool ParseSign()
			{
				bool flag = false;
				if (!this.AtEnd && this._src[this._cur] == '-')
				{
					flag = true;
					this._cur++;
				}
				return flag;
			}

			private int ParseInt(bool optional)
			{
				if (optional && this.AtEnd)
				{
					return 0;
				}
				int num = 0;
				int num2 = 0;
				while (!this.AtEnd && char.IsDigit(this._src, this._cur))
				{
					num = checked(num * 10 + (int)this._src[this._cur] - 48);
					this._cur++;
					num2++;
				}
				if (!optional && num2 == 0)
				{
					this.formatError = true;
				}
				return num;
			}

			private bool ParseOptDot()
			{
				if (this.AtEnd)
				{
					return false;
				}
				if (this._src[this._cur] == '.')
				{
					this._cur++;
					return true;
				}
				return false;
			}

			private void ParseOptColon()
			{
				if (!this.AtEnd)
				{
					if (this._src[this._cur] == ':')
					{
						this._cur++;
					}
					else
					{
						this.formatError = true;
					}
				}
			}

			private long ParseTicks()
			{
				long num = 1000000L;
				long num2 = 0L;
				bool flag = false;
				while (num > 0L && !this.AtEnd && char.IsDigit(this._src, this._cur))
				{
					num2 += (long)(this._src[this._cur] - '0') * num;
					this._cur++;
					num /= 10L;
					flag = true;
				}
				if (!flag)
				{
					this.formatError = true;
				}
				return num2;
			}

			public TimeSpan Execute()
			{
				int num = 0;
				this.ParseWhiteSpace();
				bool flag = this.ParseSign();
				int num2 = this.ParseInt(false);
				if (this.ParseOptDot())
				{
					num = this.ParseInt(true);
				}
				else if (!this.AtEnd)
				{
					num = num2;
					num2 = 0;
				}
				this.ParseOptColon();
				int num3 = this.ParseInt(true);
				this.ParseOptColon();
				int num4 = this.ParseInt(true);
				long num5;
				if (this.ParseOptDot())
				{
					num5 = this.ParseTicks();
				}
				else
				{
					num5 = 0L;
				}
				this.ParseWhiteSpace();
				if (!this.AtEnd)
				{
					this.formatError = true;
				}
				if (num > 23 || num3 > 59 || num4 > 59)
				{
					throw new OverflowException(Locale.GetText("Invalid time data."));
				}
				if (this.formatError)
				{
					throw new FormatException(Locale.GetText("Invalid format for TimeSpan.Parse."));
				}
				long num6 = TimeSpan.CalculateTicks(num2, num, num3, num4, 0);
				checked
				{
					num6 = ((!flag) ? (num6 + num5) : ((long)(unchecked((ulong)0) - (ulong)num6 - (ulong)num5)));
					return new TimeSpan(num6);
				}
			}

			private string _src;

			private int _cur;

			private int _length;

			private bool formatError;
		}
	}
}
