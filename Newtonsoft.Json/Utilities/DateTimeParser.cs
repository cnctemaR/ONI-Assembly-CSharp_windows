using System;

namespace Newtonsoft.Json.Utilities
{
	internal struct DateTimeParser
	{
		public bool Parse(string text)
		{
			this._text = text;
			this._length = text.Length;
			return this.ParseDate(0) && this.ParseChar(DateTimeParser.Lzyyyy_MM_dd, 'T') && this.ParseTimeAndZoneAndWhitespace(DateTimeParser.Lzyyyy_MM_ddT);
		}

		private bool ParseDate(int start)
		{
			return this.Parse4Digit(start, out this.Year) && 1 <= this.Year && this.ParseChar(start + DateTimeParser.Lzyyyy, '-') && this.Parse2Digit(start + DateTimeParser.Lzyyyy_, out this.Month) && 1 <= this.Month && this.Month <= 12 && this.ParseChar(start + DateTimeParser.Lzyyyy_MM, '-') && this.Parse2Digit(start + DateTimeParser.Lzyyyy_MM_, out this.Day) && 1 <= this.Day && this.Day <= DateTime.DaysInMonth(this.Year, this.Month);
		}

		private bool ParseTimeAndZoneAndWhitespace(int start)
		{
			return this.ParseTime(ref start) && this.ParseZone(start);
		}

		private bool ParseTime(ref int start)
		{
			if (!this.Parse2Digit(start, out this.Hour) || this.Hour >= 24 || !this.ParseChar(start + DateTimeParser.LzHH, ':') || !this.Parse2Digit(start + DateTimeParser.LzHH_, out this.Minute) || this.Minute >= 60 || !this.ParseChar(start + DateTimeParser.LzHH_mm, ':') || !this.Parse2Digit(start + DateTimeParser.LzHH_mm_, out this.Second) || this.Second >= 60)
			{
				return false;
			}
			start += DateTimeParser.LzHH_mm_ss;
			if (this.ParseChar(start, '.'))
			{
				this.Fraction = 0;
				int num = 0;
				while (++start < this._length && num < 7)
				{
					int num2 = (int)(this._text[start] - '0');
					if (num2 < 0 || num2 > 9)
					{
						break;
					}
					this.Fraction = this.Fraction * 10 + num2;
					num++;
				}
				if (num < 7)
				{
					if (num == 0)
					{
						return false;
					}
					this.Fraction *= DateTimeParser.Power10[7 - num];
				}
			}
			return true;
		}

		private bool ParseZone(int start)
		{
			if (start < this._length)
			{
				char c = this._text[start];
				if (c == 'Z' || c == 'z')
				{
					this.Zone = ParserTimeZone.Utc;
					start++;
				}
				else
				{
					if (start + 2 < this._length && this.Parse2Digit(start + DateTimeParser.Lz_, out this.ZoneHour) && this.ZoneHour <= 99)
					{
						switch (c)
						{
						case '+':
							this.Zone = ParserTimeZone.LocalEastOfUtc;
							start += DateTimeParser.Lz_zz;
							break;
						case '-':
							this.Zone = ParserTimeZone.LocalWestOfUtc;
							start += DateTimeParser.Lz_zz;
							break;
						}
					}
					if (start < this._length)
					{
						if (this.ParseChar(start, ':'))
						{
							start++;
							if (start + 1 < this._length && this.Parse2Digit(start, out this.ZoneMinute) && this.ZoneMinute <= 99)
							{
								start += 2;
							}
						}
						else if (start + 1 < this._length && this.Parse2Digit(start, out this.ZoneMinute) && this.ZoneMinute <= 99)
						{
							start += 2;
						}
					}
				}
			}
			return start == this._length;
		}

		private bool Parse4Digit(int start, out int num)
		{
			if (start + 3 < this._length)
			{
				int num2 = (int)(this._text[start] - '0');
				int num3 = (int)(this._text[start + 1] - '0');
				int num4 = (int)(this._text[start + 2] - '0');
				int num5 = (int)(this._text[start + 3] - '0');
				if (0 <= num2 && num2 < 10 && 0 <= num3 && num3 < 10 && 0 <= num4 && num4 < 10 && 0 <= num5 && num5 < 10)
				{
					num = ((num2 * 10 + num3) * 10 + num4) * 10 + num5;
					return true;
				}
			}
			num = 0;
			return false;
		}

		private bool Parse2Digit(int start, out int num)
		{
			if (start + 1 < this._length)
			{
				int num2 = (int)(this._text[start] - '0');
				int num3 = (int)(this._text[start + 1] - '0');
				if (0 <= num2 && num2 < 10 && 0 <= num3 && num3 < 10)
				{
					num = num2 * 10 + num3;
					return true;
				}
			}
			num = 0;
			return false;
		}

		private bool ParseChar(int start, char ch)
		{
			return start < this._length && this._text[start] == ch;
		}

		private const short MaxFractionDigits = 7;

		public int Year;

		public int Month;

		public int Day;

		public int Hour;

		public int Minute;

		public int Second;

		public int Fraction;

		public int ZoneHour;

		public int ZoneMinute;

		public ParserTimeZone Zone;

		private string _text;

		private int _length;

		private static readonly int[] Power10 = new int[] { -1, 10, 100, 1000, 10000, 100000, 1000000 };

		private static readonly int Lzyyyy = "yyyy".Length;

		private static readonly int Lzyyyy_ = "yyyy-".Length;

		private static readonly int Lzyyyy_MM = "yyyy-MM".Length;

		private static readonly int Lzyyyy_MM_ = "yyyy-MM-".Length;

		private static readonly int Lzyyyy_MM_dd = "yyyy-MM-dd".Length;

		private static readonly int Lzyyyy_MM_ddT = "yyyy-MM-ddT".Length;

		private static readonly int LzHH = "HH".Length;

		private static readonly int LzHH_ = "HH:".Length;

		private static readonly int LzHH_mm = "HH:mm".Length;

		private static readonly int LzHH_mm_ = "HH:mm:".Length;

		private static readonly int LzHH_mm_ss = "HH:mm:ss".Length;

		private static readonly int Lz_ = "-".Length;

		private static readonly int Lz_zz = "-zz".Length;
	}
}
