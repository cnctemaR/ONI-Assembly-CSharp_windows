using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace System
{
	internal sealed class NumberFormatter
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetFormatterTables(out ulong* MantissaBitsTable, out int* TensExponentTable, out char* DigitLowerTable, out char* DigitUpperTable, out long* TenPowersList, out int* DecHexDigits);

		static NumberFormatter()
		{
			NumberFormatter.GetFormatterTables(out NumberFormatter.MantissaBitsTable, out NumberFormatter.TensExponentTable, out NumberFormatter.DigitLowerTable, out NumberFormatter.DigitUpperTable, out NumberFormatter.TenPowersList, out NumberFormatter.DecHexDigits);
		}

		private unsafe static long GetTenPowerOf(int i)
		{
			return NumberFormatter.TenPowersList[i];
		}

		private void InitDecHexDigits(uint value)
		{
			if (value >= 100000000U)
			{
				int num = (int)(value / 100000000U);
				value -= (uint)(100000000 * num);
				this._val2 = NumberFormatter.FastToDecHex(num);
			}
			this._val1 = NumberFormatter.ToDecHex((int)value);
		}

		private void InitDecHexDigits(ulong value)
		{
			if (value >= 100000000UL)
			{
				long num = (long)(value / 100000000UL);
				value -= (ulong)(100000000L * num);
				if (num >= 100000000L)
				{
					int num2 = (int)(num / 100000000L);
					num -= (long)num2 * 100000000L;
					this._val3 = NumberFormatter.ToDecHex(num2);
				}
				if (num != 0L)
				{
					this._val2 = NumberFormatter.ToDecHex((int)num);
				}
			}
			if (value != 0UL)
			{
				this._val1 = NumberFormatter.ToDecHex((int)value);
			}
		}

		private void InitDecHexDigits(uint hi, ulong lo)
		{
			if (hi == 0U)
			{
				this.InitDecHexDigits(lo);
				return;
			}
			uint num = hi / 100000000U;
			ulong num2 = (ulong)(hi - num * 100000000U);
			ulong num3 = lo / 100000000UL;
			ulong num4 = lo - num3 * 100000000UL + num2 * 9551616UL;
			hi = num;
			lo = num3 + num2 * 184467440737UL;
			num3 = num4 / 100000000UL;
			num4 -= num3 * 100000000UL;
			lo += num3;
			this._val1 = NumberFormatter.ToDecHex((int)num4);
			num3 = lo / 100000000UL;
			num4 = lo - num3 * 100000000UL;
			lo = num3;
			if (hi != 0U)
			{
				lo += (ulong)hi * 184467440737UL;
				num4 += (ulong)hi * 9551616UL;
				num3 = num4 / 100000000UL;
				lo += num3;
				num4 -= num3 * 100000000UL;
			}
			this._val2 = NumberFormatter.ToDecHex((int)num4);
			if (lo >= 100000000UL)
			{
				num3 = lo / 100000000UL;
				lo -= num3 * 100000000UL;
				this._val4 = NumberFormatter.ToDecHex((int)num3);
			}
			this._val3 = NumberFormatter.ToDecHex((int)lo);
		}

		private unsafe static uint FastToDecHex(int val)
		{
			if (val < 100)
			{
				return (uint)NumberFormatter.DecHexDigits[val];
			}
			int num = val * 5243 >> 19;
			return (uint)((NumberFormatter.DecHexDigits[num] << 8) | NumberFormatter.DecHexDigits[val - num * 100]);
		}

		private static uint ToDecHex(int val)
		{
			uint num = 0U;
			if (val >= 10000)
			{
				int num2 = val / 10000;
				val -= num2 * 10000;
				num = NumberFormatter.FastToDecHex(num2) << 16;
			}
			return num | NumberFormatter.FastToDecHex(val);
		}

		private static int FastDecHexLen(int val)
		{
			if (val < 256)
			{
				if (val < 16)
				{
					return 1;
				}
				return 2;
			}
			else
			{
				if (val < 4096)
				{
					return 3;
				}
				return 4;
			}
		}

		private static int DecHexLen(uint val)
		{
			if (val < 65536U)
			{
				return NumberFormatter.FastDecHexLen((int)val);
			}
			return 4 + NumberFormatter.FastDecHexLen((int)(val >> 16));
		}

		private int DecHexLen()
		{
			if (this._val4 != 0U)
			{
				return NumberFormatter.DecHexLen(this._val4) + 24;
			}
			if (this._val3 != 0U)
			{
				return NumberFormatter.DecHexLen(this._val3) + 16;
			}
			if (this._val2 != 0U)
			{
				return NumberFormatter.DecHexLen(this._val2) + 8;
			}
			if (this._val1 != 0U)
			{
				return NumberFormatter.DecHexLen(this._val1);
			}
			return 0;
		}

		private static int ScaleOrder(long hi)
		{
			for (int i = 18; i >= 0; i--)
			{
				if (hi >= NumberFormatter.GetTenPowerOf(i))
				{
					return i + 1;
				}
			}
			return 1;
		}

		private int InitialFloatingPrecision()
		{
			if (this._specifier == 'R')
			{
				return this._defPrecision + 2;
			}
			if (this._precision < this._defPrecision)
			{
				return this._defPrecision;
			}
			if (this._specifier == 'G')
			{
				return Math.Min(this._defPrecision + 2, this._precision);
			}
			if (this._specifier == 'E')
			{
				return Math.Min(this._defPrecision + 2, this._precision + 1);
			}
			return this._defPrecision;
		}

		private static int ParsePrecision(string format)
		{
			int num = 0;
			for (int i = 1; i < format.Length; i++)
			{
				int num2 = (int)(format[i] - '0');
				num = num * 10 + num2;
				if (num2 < 0 || num2 > 9 || num > 99)
				{
					return -2;
				}
			}
			return num;
		}

		private NumberFormatter(Thread current)
		{
			this._cbuf = EmptyArray<char>.Value;
			if (current == null)
			{
				return;
			}
			this.CurrentCulture = current.CurrentCulture;
		}

		private void Init(string format)
		{
			this._val1 = (this._val2 = (this._val3 = (this._val4 = 0U)));
			this._offset = 0;
			this._NaN = (this._infinity = false);
			this._isCustomFormat = false;
			this._specifierIsUpper = true;
			this._precision = -1;
			if (format == null || format.Length == 0)
			{
				this._specifier = 'G';
				return;
			}
			char c = format[0];
			if (c >= 'a' && c <= 'z')
			{
				c = c - 'a' + 'A';
				this._specifierIsUpper = false;
			}
			else if (c < 'A' || c > 'Z')
			{
				this._isCustomFormat = true;
				this._specifier = '0';
				return;
			}
			this._specifier = c;
			if (format.Length > 1)
			{
				this._precision = NumberFormatter.ParsePrecision(format);
				if (this._precision == -2)
				{
					this._isCustomFormat = true;
					this._specifier = '0';
					this._precision = -1;
				}
			}
		}

		private void InitHex(ulong value)
		{
			int defPrecision = this._defPrecision;
			if (defPrecision == 10)
			{
				value = (ulong)((uint)value);
			}
			this._val1 = (uint)value;
			this._val2 = (uint)(value >> 32);
			this._decPointPos = (this._digitsLen = this.DecHexLen());
			if (value == 0UL)
			{
				this._decPointPos = 1;
			}
		}

		private void Init(string format, int value, int defPrecision)
		{
			this.Init(format);
			this._defPrecision = defPrecision;
			this._positive = value >= 0;
			if (value == 0 || this._specifier == 'X')
			{
				this.InitHex((ulong)((long)value));
				return;
			}
			if (value < 0)
			{
				value = -value;
			}
			this.InitDecHexDigits((uint)value);
			this._decPointPos = (this._digitsLen = this.DecHexLen());
		}

		private void Init(string format, uint value, int defPrecision)
		{
			this.Init(format);
			this._defPrecision = defPrecision;
			this._positive = true;
			if (value == 0U || this._specifier == 'X')
			{
				this.InitHex((ulong)value);
				return;
			}
			this.InitDecHexDigits(value);
			this._decPointPos = (this._digitsLen = this.DecHexLen());
		}

		private void Init(string format, long value)
		{
			this.Init(format);
			this._defPrecision = 19;
			this._positive = value >= 0L;
			if (value == 0L || this._specifier == 'X')
			{
				this.InitHex((ulong)value);
				return;
			}
			if (value < 0L)
			{
				value = -value;
			}
			this.InitDecHexDigits((ulong)value);
			this._decPointPos = (this._digitsLen = this.DecHexLen());
		}

		private void Init(string format, ulong value)
		{
			this.Init(format);
			this._defPrecision = 20;
			this._positive = true;
			if (value == 0UL || this._specifier == 'X')
			{
				this.InitHex(value);
				return;
			}
			this.InitDecHexDigits(value);
			this._decPointPos = (this._digitsLen = this.DecHexLen());
		}

		private unsafe void Init(string format, double value, int defPrecision)
		{
			this.Init(format);
			this._defPrecision = defPrecision;
			long num = BitConverter.DoubleToInt64Bits(value);
			this._positive = num >= 0L;
			num &= long.MaxValue;
			if (num == 0L)
			{
				this._decPointPos = 1;
				this._digitsLen = 0;
				this._positive = true;
				return;
			}
			int num2 = (int)(num >> 52);
			long num3 = num & 4503599627370495L;
			if (num2 == 2047)
			{
				this._NaN = num3 != 0L;
				this._infinity = num3 == 0L;
				return;
			}
			int num4 = 0;
			if (num2 == 0)
			{
				num2 = 1;
				int num5 = NumberFormatter.ScaleOrder(num3);
				if (num5 < 15)
				{
					num4 = num5 - 15;
					num3 *= NumberFormatter.GetTenPowerOf(-num4);
				}
			}
			else
			{
				num3 = (num3 + 4503599627370495L + 1L) * 10L;
				num4 = -1;
			}
			ulong num6 = (ulong)((uint)num3);
			ulong num7 = (ulong)num3 >> 32;
			ulong num8 = NumberFormatter.MantissaBitsTable[num2];
			ulong num9 = num8 >> 32;
			num8 = (ulong)((uint)num8);
			ulong num10 = num7 * num8 + num6 * num9 + (num6 * num8 >> 32);
			long num11 = (long)(num7 * num9 + (num10 >> 32));
			while (num11 < 10000000000000000L)
			{
				num10 = (num10 & (ulong)(-1)) * 10UL;
				num11 = num11 * 10L + (long)(num10 >> 32);
				num4--;
			}
			if ((num10 & (ulong)(-2147483648)) != 0UL)
			{
				num11 += 1L;
			}
			int num12 = 17;
			this._decPointPos = NumberFormatter.TensExponentTable[num2] + num4 + num12;
			int num13 = this.InitialFloatingPrecision();
			if (num12 > num13)
			{
				long tenPowerOf = NumberFormatter.GetTenPowerOf(num12 - num13);
				num11 = (num11 + (tenPowerOf >> 1)) / tenPowerOf;
				num12 = num13;
			}
			if (num11 >= NumberFormatter.GetTenPowerOf(num12))
			{
				num12++;
				this._decPointPos++;
			}
			this.InitDecHexDigits((ulong)num11);
			this._offset = this.CountTrailingZeros();
			this._digitsLen = num12 - this._offset;
		}

		private void Init(string format, decimal value)
		{
			this.Init(format);
			this._defPrecision = 100;
			int[] bits = decimal.GetBits(value);
			int num = (bits[3] & 2031616) >> 16;
			this._positive = bits[3] >= 0;
			if (bits[0] == 0 && bits[1] == 0 && bits[2] == 0)
			{
				this._decPointPos = -num;
				this._positive = true;
				this._digitsLen = 0;
				return;
			}
			this.InitDecHexDigits((uint)bits[2], (ulong)(((long)bits[1] << 32) | (long)((ulong)bits[0])));
			this._digitsLen = this.DecHexLen();
			this._decPointPos = this._digitsLen - num;
			if (this._precision != -1 || this._specifier != 'G')
			{
				this._offset = this.CountTrailingZeros();
				this._digitsLen -= this._offset;
			}
		}

		private void ResetCharBuf(int size)
		{
			this._ind = 0;
			if (this._cbuf.Length < size)
			{
				this._cbuf = new char[size];
			}
		}

		private void Resize(int len)
		{
			Array.Resize<char>(ref this._cbuf, len);
		}

		private void Append(char c)
		{
			if (this._ind == this._cbuf.Length)
			{
				this.Resize(this._ind + 10);
			}
			char[] cbuf = this._cbuf;
			int ind = this._ind;
			this._ind = ind + 1;
			cbuf[ind] = c;
		}

		private void Append(char c, int cnt)
		{
			if (this._ind + cnt > this._cbuf.Length)
			{
				this.Resize(this._ind + cnt + 10);
			}
			while (cnt-- > 0)
			{
				char[] cbuf = this._cbuf;
				int ind = this._ind;
				this._ind = ind + 1;
				cbuf[ind] = c;
			}
		}

		private void Append(string s)
		{
			int length = s.Length;
			if (this._ind + length > this._cbuf.Length)
			{
				this.Resize(this._ind + length + 10);
			}
			for (int i = 0; i < length; i++)
			{
				char[] cbuf = this._cbuf;
				int ind = this._ind;
				this._ind = ind + 1;
				cbuf[ind] = s[i];
			}
		}

		private NumberFormatInfo GetNumberFormatInstance(IFormatProvider fp)
		{
			if (this._nfi != null && fp == null)
			{
				return this._nfi;
			}
			return NumberFormatInfo.GetInstance(fp);
		}

		private CultureInfo CurrentCulture
		{
			set
			{
				if (value != null && value.IsReadOnly)
				{
					this._nfi = value.NumberFormat;
					return;
				}
				this._nfi = null;
			}
		}

		private int IntegerDigits
		{
			get
			{
				if (this._decPointPos <= 0)
				{
					return 1;
				}
				return this._decPointPos;
			}
		}

		private int DecimalDigits
		{
			get
			{
				if (this._digitsLen <= this._decPointPos)
				{
					return 0;
				}
				return this._digitsLen - this._decPointPos;
			}
		}

		private bool IsFloatingSource
		{
			get
			{
				return this._defPrecision == 15 || this._defPrecision == 7;
			}
		}

		private bool IsZero
		{
			get
			{
				return this._digitsLen == 0;
			}
		}

		private bool IsZeroInteger
		{
			get
			{
				return this._digitsLen == 0 || this._decPointPos <= 0;
			}
		}

		private void RoundPos(int pos)
		{
			this.RoundBits(this._digitsLen - pos);
		}

		private bool RoundDecimal(int decimals)
		{
			return this.RoundBits(this._digitsLen - this._decPointPos - decimals);
		}

		private bool RoundBits(int shift)
		{
			if (shift <= 0)
			{
				return false;
			}
			if (shift > this._digitsLen)
			{
				this._digitsLen = 0;
				this._decPointPos = 1;
				this._val1 = (this._val2 = (this._val3 = (this._val4 = 0U)));
				this._positive = true;
				return false;
			}
			shift += this._offset;
			this._digitsLen += this._offset;
			while (shift > 8)
			{
				this._val1 = this._val2;
				this._val2 = this._val3;
				this._val3 = this._val4;
				this._val4 = 0U;
				this._digitsLen -= 8;
				shift -= 8;
			}
			shift = shift - 1 << 2;
			uint num = this._val1 >> shift;
			uint num2 = num & 15U;
			this._val1 = (num ^ num2) << shift;
			bool flag = false;
			if (num2 >= 5U)
			{
				this._val1 |= 2576980377U >> 28 - shift;
				this.AddOneToDecHex();
				int num3 = this.DecHexLen();
				flag = num3 != this._digitsLen;
				this._decPointPos = this._decPointPos + num3 - this._digitsLen;
				this._digitsLen = num3;
			}
			this.RemoveTrailingZeros();
			return flag;
		}

		private void RemoveTrailingZeros()
		{
			this._offset = this.CountTrailingZeros();
			this._digitsLen -= this._offset;
			if (this._digitsLen == 0)
			{
				this._offset = 0;
				this._decPointPos = 1;
				this._positive = true;
			}
		}

		private void AddOneToDecHex()
		{
			if (this._val1 != 2576980377U)
			{
				this._val1 = NumberFormatter.AddOneToDecHex(this._val1);
				return;
			}
			this._val1 = 0U;
			if (this._val2 != 2576980377U)
			{
				this._val2 = NumberFormatter.AddOneToDecHex(this._val2);
				return;
			}
			this._val2 = 0U;
			if (this._val3 == 2576980377U)
			{
				this._val3 = 0U;
				this._val4 = NumberFormatter.AddOneToDecHex(this._val4);
				return;
			}
			this._val3 = NumberFormatter.AddOneToDecHex(this._val3);
		}

		private static uint AddOneToDecHex(uint val)
		{
			if ((val & 65535U) == 39321U)
			{
				if ((val & 16777215U) == 10066329U)
				{
					if ((val & 268435455U) == 161061273U)
					{
						return val + 107374183U;
					}
					return val + 6710887U;
				}
				else
				{
					if ((val & 1048575U) == 629145U)
					{
						return val + 419431U;
					}
					return val + 26215U;
				}
			}
			else if ((val & 255U) == 153U)
			{
				if ((val & 4095U) == 2457U)
				{
					return val + 1639U;
				}
				return val + 103U;
			}
			else
			{
				if ((val & 15U) == 9U)
				{
					return val + 7U;
				}
				return val + 1U;
			}
		}

		private int CountTrailingZeros()
		{
			if (this._val1 != 0U)
			{
				return NumberFormatter.CountTrailingZeros(this._val1);
			}
			if (this._val2 != 0U)
			{
				return NumberFormatter.CountTrailingZeros(this._val2) + 8;
			}
			if (this._val3 != 0U)
			{
				return NumberFormatter.CountTrailingZeros(this._val3) + 16;
			}
			if (this._val4 != 0U)
			{
				return NumberFormatter.CountTrailingZeros(this._val4) + 24;
			}
			return this._digitsLen;
		}

		private static int CountTrailingZeros(uint val)
		{
			if ((val & 65535U) == 0U)
			{
				if ((val & 16777215U) == 0U)
				{
					if ((val & 268435455U) == 0U)
					{
						return 7;
					}
					return 6;
				}
				else
				{
					if ((val & 1048575U) == 0U)
					{
						return 5;
					}
					return 4;
				}
			}
			else if ((val & 255U) == 0U)
			{
				if ((val & 4095U) == 0U)
				{
					return 3;
				}
				return 2;
			}
			else
			{
				if ((val & 15U) == 0U)
				{
					return 1;
				}
				return 0;
			}
		}

		private static NumberFormatter GetInstance(IFormatProvider fp)
		{
			if (fp != null)
			{
				if (NumberFormatter.userFormatProvider == null)
				{
					Interlocked.CompareExchange<NumberFormatter>(ref NumberFormatter.userFormatProvider, new NumberFormatter(null), null);
				}
				return NumberFormatter.userFormatProvider;
			}
			NumberFormatter numberFormatter = NumberFormatter.threadNumberFormatter;
			NumberFormatter.threadNumberFormatter = null;
			if (numberFormatter == null)
			{
				return new NumberFormatter(Thread.CurrentThread);
			}
			numberFormatter.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			return numberFormatter;
		}

		private void Release()
		{
			if (this != NumberFormatter.userFormatProvider)
			{
				NumberFormatter.threadNumberFormatter = this;
			}
		}

		public static string NumberToString(string format, uint value, IFormatProvider fp)
		{
			NumberFormatter instance = NumberFormatter.GetInstance(fp);
			instance.Init(format, value, 10);
			string text = instance.IntegerToString(format, fp);
			instance.Release();
			return text;
		}

		public static string NumberToString(string format, int value, IFormatProvider fp)
		{
			NumberFormatter instance = NumberFormatter.GetInstance(fp);
			instance.Init(format, value, 10);
			string text = instance.IntegerToString(format, fp);
			instance.Release();
			return text;
		}

		public static string NumberToString(string format, ulong value, IFormatProvider fp)
		{
			NumberFormatter instance = NumberFormatter.GetInstance(fp);
			instance.Init(format, value);
			string text = instance.IntegerToString(format, fp);
			instance.Release();
			return text;
		}

		public static string NumberToString(string format, long value, IFormatProvider fp)
		{
			NumberFormatter instance = NumberFormatter.GetInstance(fp);
			instance.Init(format, value);
			string text = instance.IntegerToString(format, fp);
			instance.Release();
			return text;
		}

		public static string NumberToString(string format, float value, IFormatProvider fp)
		{
			NumberFormatter instance = NumberFormatter.GetInstance(fp);
			instance.Init(format, (double)value, 7);
			NumberFormatInfo numberFormatInstance = instance.GetNumberFormatInstance(fp);
			string text;
			if (instance._NaN)
			{
				text = numberFormatInstance.NaNSymbol;
			}
			else if (instance._infinity)
			{
				if (instance._positive)
				{
					text = numberFormatInstance.PositiveInfinitySymbol;
				}
				else
				{
					text = numberFormatInstance.NegativeInfinitySymbol;
				}
			}
			else if (instance._specifier == 'R')
			{
				text = instance.FormatRoundtrip(value, numberFormatInstance);
			}
			else
			{
				text = instance.NumberToString(format, numberFormatInstance);
			}
			instance.Release();
			return text;
		}

		public static string NumberToString(string format, double value, IFormatProvider fp)
		{
			NumberFormatter instance = NumberFormatter.GetInstance(fp);
			instance.Init(format, value, 15);
			NumberFormatInfo numberFormatInstance = instance.GetNumberFormatInstance(fp);
			string text;
			if (instance._NaN)
			{
				text = numberFormatInstance.NaNSymbol;
			}
			else if (instance._infinity)
			{
				if (instance._positive)
				{
					text = numberFormatInstance.PositiveInfinitySymbol;
				}
				else
				{
					text = numberFormatInstance.NegativeInfinitySymbol;
				}
			}
			else if (instance._specifier == 'R')
			{
				text = instance.FormatRoundtrip(value, numberFormatInstance);
			}
			else
			{
				text = instance.NumberToString(format, numberFormatInstance);
			}
			instance.Release();
			return text;
		}

		public static string NumberToString(string format, decimal value, IFormatProvider fp)
		{
			NumberFormatter instance = NumberFormatter.GetInstance(fp);
			instance.Init(format, value);
			string text = instance.NumberToString(format, instance.GetNumberFormatInstance(fp));
			instance.Release();
			return text;
		}

		private string IntegerToString(string format, IFormatProvider fp)
		{
			NumberFormatInfo numberFormatInstance = this.GetNumberFormatInstance(fp);
			char specifier = this._specifier;
			if (specifier <= 'N')
			{
				switch (specifier)
				{
				case 'C':
					return this.FormatCurrency(this._precision, numberFormatInstance);
				case 'D':
					return this.FormatDecimal(this._precision, numberFormatInstance);
				case 'E':
					return this.FormatExponential(this._precision, numberFormatInstance);
				case 'F':
					return this.FormatFixedPoint(this._precision, numberFormatInstance);
				case 'G':
					if (this._precision <= 0)
					{
						return this.FormatDecimal(-1, numberFormatInstance);
					}
					return this.FormatGeneral(this._precision, numberFormatInstance);
				default:
					if (specifier == 'N')
					{
						return this.FormatNumber(this._precision, numberFormatInstance);
					}
					break;
				}
			}
			else
			{
				if (specifier == 'P')
				{
					return this.FormatPercent(this._precision, numberFormatInstance);
				}
				if (specifier == 'X')
				{
					return this.FormatHexadecimal(this._precision);
				}
			}
			if (this._isCustomFormat)
			{
				return this.FormatCustom(format, numberFormatInstance);
			}
			throw new FormatException("The specified format '" + format + "' is invalid");
		}

		private string NumberToString(string format, NumberFormatInfo nfi)
		{
			char specifier = this._specifier;
			if (specifier <= 'N')
			{
				switch (specifier)
				{
				case 'C':
					return this.FormatCurrency(this._precision, nfi);
				case 'D':
					break;
				case 'E':
					return this.FormatExponential(this._precision, nfi);
				case 'F':
					return this.FormatFixedPoint(this._precision, nfi);
				case 'G':
					return this.FormatGeneral(this._precision, nfi);
				default:
					if (specifier == 'N')
					{
						return this.FormatNumber(this._precision, nfi);
					}
					break;
				}
			}
			else
			{
				if (specifier == 'P')
				{
					return this.FormatPercent(this._precision, nfi);
				}
				if (specifier != 'X')
				{
				}
			}
			if (this._isCustomFormat)
			{
				return this.FormatCustom(format, nfi);
			}
			throw new FormatException("The specified format '" + format + "' is invalid");
		}

		private string FormatCurrency(int precision, NumberFormatInfo nfi)
		{
			precision = ((precision >= 0) ? precision : nfi.CurrencyDecimalDigits);
			this.RoundDecimal(precision);
			this.ResetCharBuf(this.IntegerDigits * 2 + precision * 2 + 16);
			if (this._positive)
			{
				int num = nfi.CurrencyPositivePattern;
				if (num != 0)
				{
					if (num == 2)
					{
						this.Append(nfi.CurrencySymbol);
						this.Append(' ');
					}
				}
				else
				{
					this.Append(nfi.CurrencySymbol);
				}
			}
			else
			{
				switch (nfi.CurrencyNegativePattern)
				{
				case 0:
					this.Append('(');
					this.Append(nfi.CurrencySymbol);
					break;
				case 1:
					this.Append(nfi.NegativeSign);
					this.Append(nfi.CurrencySymbol);
					break;
				case 2:
					this.Append(nfi.CurrencySymbol);
					this.Append(nfi.NegativeSign);
					break;
				case 3:
					this.Append(nfi.CurrencySymbol);
					break;
				case 4:
					this.Append('(');
					break;
				case 5:
					this.Append(nfi.NegativeSign);
					break;
				case 8:
					this.Append(nfi.NegativeSign);
					break;
				case 9:
					this.Append(nfi.NegativeSign);
					this.Append(nfi.CurrencySymbol);
					this.Append(' ');
					break;
				case 11:
					this.Append(nfi.CurrencySymbol);
					this.Append(' ');
					break;
				case 12:
					this.Append(nfi.CurrencySymbol);
					this.Append(' ');
					this.Append(nfi.NegativeSign);
					break;
				case 14:
					this.Append('(');
					this.Append(nfi.CurrencySymbol);
					this.Append(' ');
					break;
				case 15:
					this.Append('(');
					break;
				}
			}
			this.AppendIntegerStringWithGroupSeparator(nfi.CurrencyGroupSizes, nfi.CurrencyGroupSeparator);
			if (precision > 0)
			{
				this.Append(nfi.CurrencyDecimalSeparator);
				this.AppendDecimalString(precision);
			}
			if (this._positive)
			{
				int num = nfi.CurrencyPositivePattern;
				if (num != 1)
				{
					if (num == 3)
					{
						this.Append(' ');
						this.Append(nfi.CurrencySymbol);
					}
				}
				else
				{
					this.Append(nfi.CurrencySymbol);
				}
			}
			else
			{
				switch (nfi.CurrencyNegativePattern)
				{
				case 0:
					this.Append(')');
					break;
				case 3:
					this.Append(nfi.NegativeSign);
					break;
				case 4:
					this.Append(nfi.CurrencySymbol);
					this.Append(')');
					break;
				case 5:
					this.Append(nfi.CurrencySymbol);
					break;
				case 6:
					this.Append(nfi.NegativeSign);
					this.Append(nfi.CurrencySymbol);
					break;
				case 7:
					this.Append(nfi.CurrencySymbol);
					this.Append(nfi.NegativeSign);
					break;
				case 8:
					this.Append(' ');
					this.Append(nfi.CurrencySymbol);
					break;
				case 10:
					this.Append(' ');
					this.Append(nfi.CurrencySymbol);
					this.Append(nfi.NegativeSign);
					break;
				case 11:
					this.Append(nfi.NegativeSign);
					break;
				case 13:
					this.Append(nfi.NegativeSign);
					this.Append(' ');
					this.Append(nfi.CurrencySymbol);
					break;
				case 14:
					this.Append(')');
					break;
				case 15:
					this.Append(' ');
					this.Append(nfi.CurrencySymbol);
					this.Append(')');
					break;
				}
			}
			return new string(this._cbuf, 0, this._ind);
		}

		private string FormatDecimal(int precision, NumberFormatInfo nfi)
		{
			if (precision < this._digitsLen)
			{
				precision = this._digitsLen;
			}
			if (precision == 0)
			{
				return "0";
			}
			this.ResetCharBuf(precision + 1);
			if (!this._positive)
			{
				this.Append(nfi.NegativeSign);
			}
			this.AppendDigits(0, precision);
			return new string(this._cbuf, 0, this._ind);
		}

		private unsafe string FormatHexadecimal(int precision)
		{
			int i = Math.Max(precision, this._decPointPos);
			char* ptr = (this._specifierIsUpper ? NumberFormatter.DigitUpperTable : NumberFormatter.DigitLowerTable);
			this.ResetCharBuf(i);
			this._ind = i;
			ulong num = (ulong)this._val1 | ((ulong)this._val2 << 32);
			while (i > 0)
			{
				this._cbuf[--i] = ptr[(num & 15UL) * 2UL / 2UL];
				num >>= 4;
			}
			return new string(this._cbuf, 0, this._ind);
		}

		private string FormatFixedPoint(int precision, NumberFormatInfo nfi)
		{
			if (precision == -1)
			{
				precision = nfi.NumberDecimalDigits;
			}
			this.RoundDecimal(precision);
			this.ResetCharBuf(this.IntegerDigits + precision + 2);
			if (!this._positive)
			{
				this.Append(nfi.NegativeSign);
			}
			this.AppendIntegerString(this.IntegerDigits);
			if (precision > 0)
			{
				this.Append(nfi.NumberDecimalSeparator);
				this.AppendDecimalString(precision);
			}
			return new string(this._cbuf, 0, this._ind);
		}

		private string FormatRoundtrip(double origval, NumberFormatInfo nfi)
		{
			NumberFormatter clone = this.GetClone();
			if (origval >= -1.79769313486231E+308 && origval <= 1.79769313486231E+308)
			{
				string text = this.FormatGeneral(this._defPrecision, nfi);
				if (origval == double.Parse(text, nfi))
				{
					return text;
				}
			}
			return clone.FormatGeneral(this._defPrecision + 2, nfi);
		}

		private string FormatRoundtrip(float origval, NumberFormatInfo nfi)
		{
			NumberFormatter clone = this.GetClone();
			string text = this.FormatGeneral(this._defPrecision, nfi);
			if (origval == float.Parse(text, nfi))
			{
				return text;
			}
			return clone.FormatGeneral(this._defPrecision + 2, nfi);
		}

		private string FormatGeneral(int precision, NumberFormatInfo nfi)
		{
			bool flag;
			if (precision == -1)
			{
				flag = this.IsFloatingSource;
				precision = this._defPrecision;
			}
			else
			{
				flag = true;
				if (precision == 0)
				{
					precision = this._defPrecision;
				}
				this.RoundPos(precision);
			}
			int num = this._decPointPos;
			int digitsLen = this._digitsLen;
			int num2 = digitsLen - num;
			if ((num > precision || num <= -4) && flag)
			{
				return this.FormatExponential(digitsLen - 1, nfi, 2);
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num < 0)
			{
				num = 0;
			}
			this.ResetCharBuf(num2 + num + 3);
			if (!this._positive)
			{
				this.Append(nfi.NegativeSign);
			}
			if (num == 0)
			{
				this.Append('0');
			}
			else
			{
				this.AppendDigits(digitsLen - num, digitsLen);
			}
			if (num2 > 0)
			{
				this.Append(nfi.NumberDecimalSeparator);
				this.AppendDigits(0, num2);
			}
			return new string(this._cbuf, 0, this._ind);
		}

		private string FormatNumber(int precision, NumberFormatInfo nfi)
		{
			precision = ((precision >= 0) ? precision : nfi.NumberDecimalDigits);
			this.ResetCharBuf(this.IntegerDigits * 3 + precision);
			this.RoundDecimal(precision);
			if (!this._positive)
			{
				switch (nfi.NumberNegativePattern)
				{
				case 0:
					this.Append('(');
					break;
				case 1:
					this.Append(nfi.NegativeSign);
					break;
				case 2:
					this.Append(nfi.NegativeSign);
					this.Append(' ');
					break;
				}
			}
			this.AppendIntegerStringWithGroupSeparator(nfi.NumberGroupSizes, nfi.NumberGroupSeparator);
			if (precision > 0)
			{
				this.Append(nfi.NumberDecimalSeparator);
				this.AppendDecimalString(precision);
			}
			if (!this._positive)
			{
				switch (nfi.NumberNegativePattern)
				{
				case 0:
					this.Append(')');
					break;
				case 3:
					this.Append(nfi.NegativeSign);
					break;
				case 4:
					this.Append(' ');
					this.Append(nfi.NegativeSign);
					break;
				}
			}
			return new string(this._cbuf, 0, this._ind);
		}

		private string FormatPercent(int precision, NumberFormatInfo nfi)
		{
			precision = ((precision >= 0) ? precision : nfi.PercentDecimalDigits);
			this.Multiply10(2);
			this.RoundDecimal(precision);
			this.ResetCharBuf(this.IntegerDigits * 2 + precision + 16);
			if (this._positive)
			{
				if (nfi.PercentPositivePattern == 2)
				{
					this.Append(nfi.PercentSymbol);
				}
			}
			else
			{
				switch (nfi.PercentNegativePattern)
				{
				case 0:
					this.Append(nfi.NegativeSign);
					break;
				case 1:
					this.Append(nfi.NegativeSign);
					break;
				case 2:
					this.Append(nfi.NegativeSign);
					this.Append(nfi.PercentSymbol);
					break;
				}
			}
			this.AppendIntegerStringWithGroupSeparator(nfi.PercentGroupSizes, nfi.PercentGroupSeparator);
			if (precision > 0)
			{
				this.Append(nfi.PercentDecimalSeparator);
				this.AppendDecimalString(precision);
			}
			if (this._positive)
			{
				int num = nfi.PercentPositivePattern;
				if (num != 0)
				{
					if (num == 1)
					{
						this.Append(nfi.PercentSymbol);
					}
				}
				else
				{
					this.Append(' ');
					this.Append(nfi.PercentSymbol);
				}
			}
			else
			{
				int num = nfi.PercentNegativePattern;
				if (num != 0)
				{
					if (num == 1)
					{
						this.Append(nfi.PercentSymbol);
					}
				}
				else
				{
					this.Append(' ');
					this.Append(nfi.PercentSymbol);
				}
			}
			return new string(this._cbuf, 0, this._ind);
		}

		private string FormatExponential(int precision, NumberFormatInfo nfi)
		{
			if (precision == -1)
			{
				precision = 6;
			}
			this.RoundPos(precision + 1);
			return this.FormatExponential(precision, nfi, 3);
		}

		private string FormatExponential(int precision, NumberFormatInfo nfi, int expDigits)
		{
			int decPointPos = this._decPointPos;
			int digitsLen = this._digitsLen;
			int num = decPointPos - 1;
			this._decPointPos = 1;
			this.ResetCharBuf(precision + 8);
			if (!this._positive)
			{
				this.Append(nfi.NegativeSign);
			}
			this.AppendOneDigit(digitsLen - 1);
			if (precision > 0)
			{
				this.Append(nfi.NumberDecimalSeparator);
				this.AppendDigits(digitsLen - precision - 1, digitsLen - this._decPointPos);
			}
			this.AppendExponent(nfi, num, expDigits);
			return new string(this._cbuf, 0, this._ind);
		}

		private string FormatCustom(string format, NumberFormatInfo nfi)
		{
			bool positive = this._positive;
			int num = 0;
			int num2 = 0;
			NumberFormatter.CustomInfo.GetActiveSection(format, ref positive, this.IsZero, ref num, ref num2);
			if (num2 != 0)
			{
				this._positive = positive;
				NumberFormatter.CustomInfo customInfo = NumberFormatter.CustomInfo.Parse(format, num, num2, nfi);
				StringBuilder stringBuilder = new StringBuilder(customInfo.IntegerDigits * 2);
				StringBuilder stringBuilder2 = new StringBuilder(customInfo.DecimalDigits * 2);
				StringBuilder stringBuilder3 = (customInfo.UseExponent ? new StringBuilder(customInfo.ExponentDigits * 2) : null);
				int num3 = 0;
				if (customInfo.Percents > 0)
				{
					this.Multiply10(2 * customInfo.Percents);
				}
				if (customInfo.Permilles > 0)
				{
					this.Multiply10(3 * customInfo.Permilles);
				}
				if (customInfo.DividePlaces > 0)
				{
					this.Divide10(customInfo.DividePlaces);
				}
				bool flag = true;
				if (customInfo.UseExponent && (customInfo.DecimalDigits > 0 || customInfo.IntegerDigits > 0))
				{
					if (!this.IsZero)
					{
						this.RoundPos(customInfo.DecimalDigits + customInfo.IntegerDigits);
						num3 -= this._decPointPos - customInfo.IntegerDigits;
						this._decPointPos = customInfo.IntegerDigits;
					}
					flag = num3 <= 0;
					NumberFormatter.AppendNonNegativeNumber(stringBuilder3, (num3 < 0) ? (-num3) : num3);
				}
				else
				{
					this.RoundDecimal(customInfo.DecimalDigits);
				}
				if (customInfo.IntegerDigits != 0 || !this.IsZeroInteger)
				{
					this.AppendIntegerString(this.IntegerDigits, stringBuilder);
				}
				this.AppendDecimalString(this.DecimalDigits, stringBuilder2);
				if (customInfo.UseExponent)
				{
					if (customInfo.DecimalDigits <= 0 && customInfo.IntegerDigits <= 0)
					{
						this._positive = true;
					}
					if (stringBuilder.Length < customInfo.IntegerDigits)
					{
						stringBuilder.Insert(0, "0", customInfo.IntegerDigits - stringBuilder.Length);
					}
					while (stringBuilder3.Length < customInfo.ExponentDigits - customInfo.ExponentTailSharpDigits)
					{
						stringBuilder3.Insert(0, '0');
					}
					if (flag && !customInfo.ExponentNegativeSignOnly)
					{
						stringBuilder3.Insert(0, nfi.PositiveSign);
					}
					else if (!flag)
					{
						stringBuilder3.Insert(0, nfi.NegativeSign);
					}
				}
				else
				{
					if (stringBuilder.Length < customInfo.IntegerDigits - customInfo.IntegerHeadSharpDigits)
					{
						stringBuilder.Insert(0, "0", customInfo.IntegerDigits - customInfo.IntegerHeadSharpDigits - stringBuilder.Length);
					}
					if (customInfo.IntegerDigits == customInfo.IntegerHeadSharpDigits && NumberFormatter.IsZeroOnly(stringBuilder))
					{
						stringBuilder.Remove(0, stringBuilder.Length);
					}
				}
				NumberFormatter.ZeroTrimEnd(stringBuilder2, true);
				while (stringBuilder2.Length < customInfo.DecimalDigits - customInfo.DecimalTailSharpDigits)
				{
					stringBuilder2.Append('0');
				}
				if (stringBuilder2.Length > customInfo.DecimalDigits)
				{
					stringBuilder2.Remove(customInfo.DecimalDigits, stringBuilder2.Length - customInfo.DecimalDigits);
				}
				return customInfo.Format(format, num, num2, nfi, this._positive, stringBuilder, stringBuilder2, stringBuilder3);
			}
			if (!this._positive)
			{
				return nfi.NegativeSign;
			}
			return string.Empty;
		}

		private static void ZeroTrimEnd(StringBuilder sb, bool canEmpty)
		{
			int num = 0;
			int num2 = sb.Length - 1;
			while ((canEmpty ? (num2 >= 0) : (num2 > 0)) && sb[num2] == '0')
			{
				num++;
				num2--;
			}
			if (num > 0)
			{
				sb.Remove(sb.Length - num, num);
			}
		}

		private static bool IsZeroOnly(StringBuilder sb)
		{
			for (int i = 0; i < sb.Length; i++)
			{
				if (char.IsDigit(sb[i]) && sb[i] != '0')
				{
					return false;
				}
			}
			return true;
		}

		private static void AppendNonNegativeNumber(StringBuilder sb, int v)
		{
			if (v < 0)
			{
				throw new ArgumentException();
			}
			int num = NumberFormatter.ScaleOrder((long)v) - 1;
			do
			{
				int num2 = v / (int)NumberFormatter.GetTenPowerOf(num);
				sb.Append((char)(48 | num2));
				v -= (int)NumberFormatter.GetTenPowerOf(num--) * num2;
			}
			while (num >= 0);
		}

		private void AppendIntegerString(int minLength, StringBuilder sb)
		{
			if (this._decPointPos <= 0)
			{
				sb.Append('0', minLength);
				return;
			}
			if (this._decPointPos < minLength)
			{
				sb.Append('0', minLength - this._decPointPos);
			}
			this.AppendDigits(this._digitsLen - this._decPointPos, this._digitsLen, sb);
		}

		private void AppendIntegerString(int minLength)
		{
			if (this._decPointPos <= 0)
			{
				this.Append('0', minLength);
				return;
			}
			if (this._decPointPos < minLength)
			{
				this.Append('0', minLength - this._decPointPos);
			}
			this.AppendDigits(this._digitsLen - this._decPointPos, this._digitsLen);
		}

		private void AppendDecimalString(int precision, StringBuilder sb)
		{
			this.AppendDigits(this._digitsLen - precision - this._decPointPos, this._digitsLen - this._decPointPos, sb);
		}

		private void AppendDecimalString(int precision)
		{
			this.AppendDigits(this._digitsLen - precision - this._decPointPos, this._digitsLen - this._decPointPos);
		}

		private void AppendIntegerStringWithGroupSeparator(int[] groups, string groupSeparator)
		{
			if (this.IsZeroInteger)
			{
				this.Append('0');
				return;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < groups.Length; i++)
			{
				num += groups[i];
				if (num > this._decPointPos)
				{
					break;
				}
				num2 = i;
			}
			if (groups.Length != 0 && num > 0)
			{
				int num3 = groups[num2];
				int num4 = ((this._decPointPos > num) ? (this._decPointPos - num) : 0);
				if (num3 == 0)
				{
					while (num2 >= 0 && groups[num2] == 0)
					{
						num2--;
					}
					num3 = ((num4 > 0) ? num4 : groups[num2]);
				}
				int num5;
				if (num4 == 0)
				{
					num5 = num3;
				}
				else
				{
					num2 += num4 / num3;
					num5 = num4 % num3;
					if (num5 == 0)
					{
						num5 = num3;
					}
					else
					{
						num2++;
					}
				}
				if (num >= this._decPointPos)
				{
					int num6 = groups[0];
					if (num > num6)
					{
						int num7 = -(num6 - this._decPointPos);
						int num8;
						if (num7 < num6)
						{
							num5 = num7;
						}
						else if (num6 > 0 && (num8 = this._decPointPos % num6) > 0)
						{
							num5 = num8;
						}
					}
				}
				int num9 = 0;
				while (this._decPointPos - num9 > num5 && num5 != 0)
				{
					this.AppendDigits(this._digitsLen - num9 - num5, this._digitsLen - num9);
					num9 += num5;
					this.Append(groupSeparator);
					if (--num2 < groups.Length && num2 >= 0)
					{
						num3 = groups[num2];
					}
					num5 = num3;
				}
				this.AppendDigits(this._digitsLen - this._decPointPos, this._digitsLen - num9);
				return;
			}
			this.AppendDigits(this._digitsLen - this._decPointPos, this._digitsLen);
		}

		private void AppendExponent(NumberFormatInfo nfi, int exponent, int minDigits)
		{
			if (this._specifierIsUpper || this._specifier == 'R')
			{
				this.Append('E');
			}
			else
			{
				this.Append('e');
			}
			if (exponent >= 0)
			{
				this.Append(nfi.PositiveSign);
			}
			else
			{
				this.Append(nfi.NegativeSign);
				exponent = -exponent;
			}
			if (exponent == 0)
			{
				this.Append('0', minDigits);
				return;
			}
			if (exponent < 10)
			{
				this.Append('0', minDigits - 1);
				this.Append((char)(48 | exponent));
				return;
			}
			uint num = NumberFormatter.FastToDecHex(exponent);
			if (exponent >= 100 || minDigits == 3)
			{
				this.Append((char)(48U | (num >> 8)));
			}
			this.Append((char)(48U | ((num >> 4) & 15U)));
			this.Append((char)(48U | (num & 15U)));
		}

		private void AppendOneDigit(int start)
		{
			if (this._ind == this._cbuf.Length)
			{
				this.Resize(this._ind + 10);
			}
			start += this._offset;
			uint num;
			if (start < 0)
			{
				num = 0U;
			}
			else if (start < 8)
			{
				num = this._val1;
			}
			else if (start < 16)
			{
				num = this._val2;
			}
			else if (start < 24)
			{
				num = this._val3;
			}
			else if (start < 32)
			{
				num = this._val4;
			}
			else
			{
				num = 0U;
			}
			num >>= (start & 7) << 2;
			char[] cbuf = this._cbuf;
			int ind = this._ind;
			this._ind = ind + 1;
			cbuf[ind] = (ushort)(48U | (num & 15U));
		}

		private void AppendDigits(int start, int end)
		{
			if (start >= end)
			{
				return;
			}
			int num = this._ind + (end - start);
			if (num > this._cbuf.Length)
			{
				this.Resize(num + 10);
			}
			this._ind = num;
			end += this._offset;
			start += this._offset;
			int num2 = start + 8 - (start & 7);
			for (;;)
			{
				uint num3;
				if (num2 == 8)
				{
					num3 = this._val1;
				}
				else if (num2 == 16)
				{
					num3 = this._val2;
				}
				else if (num2 == 24)
				{
					num3 = this._val3;
				}
				else if (num2 == 32)
				{
					num3 = this._val4;
				}
				else
				{
					num3 = 0U;
				}
				num3 >>= (start & 7) << 2;
				if (num2 > end)
				{
					num2 = end;
				}
				this._cbuf[--num] = (char)(48U | (num3 & 15U));
				switch (num2 - start)
				{
				case 1:
					goto IL_017F;
				case 2:
					goto IL_0167;
				case 3:
					goto IL_014F;
				case 4:
					goto IL_0137;
				case 5:
					goto IL_011F;
				case 6:
					goto IL_0107;
				case 7:
					goto IL_00EF;
				case 8:
					this._cbuf[--num] = (char)(48U | ((num3 >>= 4) & 15U));
					goto IL_00EF;
				}
				IL_0184:
				start = num2;
				num2 += 8;
				continue;
				IL_017F:
				if (num2 == end)
				{
					break;
				}
				goto IL_0184;
				IL_0167:
				this._cbuf[--num] = (char)(48U | ((num3 >> 4) & 15U));
				goto IL_017F;
				IL_014F:
				this._cbuf[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_0167;
				IL_0137:
				this._cbuf[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_014F;
				IL_011F:
				this._cbuf[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_0137;
				IL_0107:
				this._cbuf[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_011F;
				IL_00EF:
				this._cbuf[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_0107;
			}
		}

		private void AppendDigits(int start, int end, StringBuilder sb)
		{
			if (start >= end)
			{
				return;
			}
			int num = sb.Length + (end - start);
			sb.Length = num;
			end += this._offset;
			start += this._offset;
			int num2 = start + 8 - (start & 7);
			for (;;)
			{
				uint num3;
				if (num2 == 8)
				{
					num3 = this._val1;
				}
				else if (num2 == 16)
				{
					num3 = this._val2;
				}
				else if (num2 == 24)
				{
					num3 = this._val3;
				}
				else if (num2 == 32)
				{
					num3 = this._val4;
				}
				else
				{
					num3 = 0U;
				}
				num3 >>= (start & 7) << 2;
				if (num2 > end)
				{
					num2 = end;
				}
				sb[--num] = (char)(48U | (num3 & 15U));
				switch (num2 - start)
				{
				case 1:
					goto IL_0162;
				case 2:
					goto IL_014B;
				case 3:
					goto IL_0134;
				case 4:
					goto IL_011D;
				case 5:
					goto IL_0106;
				case 6:
					goto IL_00EF;
				case 7:
					goto IL_00D8;
				case 8:
					sb[--num] = (char)(48U | ((num3 >>= 4) & 15U));
					goto IL_00D8;
				}
				IL_0167:
				start = num2;
				num2 += 8;
				continue;
				IL_0162:
				if (num2 == end)
				{
					break;
				}
				goto IL_0167;
				IL_014B:
				sb[--num] = (char)(48U | ((num3 >> 4) & 15U));
				goto IL_0162;
				IL_0134:
				sb[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_014B;
				IL_011D:
				sb[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_0134;
				IL_0106:
				sb[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_011D;
				IL_00EF:
				sb[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_0106;
				IL_00D8:
				sb[--num] = (char)(48U | ((num3 >>= 4) & 15U));
				goto IL_00EF;
			}
		}

		private void Multiply10(int count)
		{
			if (count <= 0 || this._digitsLen == 0)
			{
				return;
			}
			this._decPointPos += count;
		}

		private void Divide10(int count)
		{
			if (count <= 0 || this._digitsLen == 0)
			{
				return;
			}
			this._decPointPos -= count;
		}

		private NumberFormatter GetClone()
		{
			return (NumberFormatter)base.MemberwiseClone();
		}

		private const int DefaultExpPrecision = 6;

		private const int HundredMillion = 100000000;

		private const long SeventeenDigitsThreshold = 10000000000000000L;

		private const ulong ULongDivHundredMillion = 184467440737UL;

		private const ulong ULongModHundredMillion = 9551616UL;

		private const int DoubleBitsExponentShift = 52;

		private const int DoubleBitsExponentMask = 2047;

		private const long DoubleBitsMantissaMask = 4503599627370495L;

		private const int DecimalBitsScaleMask = 2031616;

		private const int SingleDefPrecision = 7;

		private const int DoubleDefPrecision = 15;

		private const int Int32DefPrecision = 10;

		private const int UInt32DefPrecision = 10;

		private const int Int64DefPrecision = 19;

		private const int UInt64DefPrecision = 20;

		private const int DecimalDefPrecision = 100;

		private const int TenPowersListLength = 19;

		private const double MinRoundtripVal = -1.79769313486231E+308;

		private const double MaxRoundtripVal = 1.79769313486231E+308;

		private unsafe static readonly ulong* MantissaBitsTable;

		private unsafe static readonly int* TensExponentTable;

		private unsafe static readonly char* DigitLowerTable;

		private unsafe static readonly char* DigitUpperTable;

		private unsafe static readonly long* TenPowersList;

		private unsafe static readonly int* DecHexDigits;

		private NumberFormatInfo _nfi;

		private char[] _cbuf;

		private bool _NaN;

		private bool _infinity;

		private bool _isCustomFormat;

		private bool _specifierIsUpper;

		private bool _positive;

		private char _specifier;

		private int _precision;

		private int _defPrecision;

		private int _digitsLen;

		private int _offset;

		private int _decPointPos;

		private uint _val1;

		private uint _val2;

		private uint _val3;

		private uint _val4;

		private int _ind;

		[ThreadStatic]
		private static NumberFormatter threadNumberFormatter;

		[ThreadStatic]
		private static NumberFormatter userFormatProvider;

		private class CustomInfo
		{
			public static void GetActiveSection(string format, ref bool positive, bool zero, ref int offset, ref int length)
			{
				int[] array = new int[3];
				int num = 0;
				int num2 = 0;
				bool flag = false;
				for (int i = 0; i < format.Length; i++)
				{
					char c = format[i];
					if (c == '"' || c == '\'')
					{
						if (i == 0 || format[i - 1] != '\\')
						{
							flag = !flag;
						}
					}
					else if (c == ';' && !flag && (i == 0 || format[i - 1] != '\\'))
					{
						array[num++] = i - num2;
						num2 = i + 1;
						if (num == 3)
						{
							break;
						}
					}
				}
				if (num == 0)
				{
					offset = 0;
					length = format.Length;
					return;
				}
				if (num == 1)
				{
					if (positive || zero)
					{
						offset = 0;
						length = array[0];
						return;
					}
					if (array[0] + 1 < format.Length)
					{
						positive = true;
						offset = array[0] + 1;
						length = format.Length - offset;
						return;
					}
					offset = 0;
					length = array[0];
					return;
				}
				else if (zero)
				{
					if (num == 2)
					{
						if (format.Length - num2 == 0)
						{
							offset = 0;
							length = array[0];
							return;
						}
						offset = array[0] + array[1] + 2;
						length = format.Length - offset;
						return;
					}
					else
					{
						if (array[2] == 0)
						{
							offset = 0;
							length = array[0];
							return;
						}
						offset = array[0] + array[1] + 2;
						length = array[2];
						return;
					}
				}
				else
				{
					if (positive)
					{
						offset = 0;
						length = array[0];
						return;
					}
					if (array[1] > 0)
					{
						positive = true;
						offset = array[0] + 1;
						length = array[1];
						return;
					}
					offset = 0;
					length = array[0];
					return;
				}
			}

			public static NumberFormatter.CustomInfo Parse(string format, int offset, int length, NumberFormatInfo nfi)
			{
				char c = '\0';
				bool flag = true;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = true;
				NumberFormatter.CustomInfo customInfo = new NumberFormatter.CustomInfo();
				int num = 0;
				int num2 = offset;
				while (num2 - offset < length)
				{
					char c2 = format[num2];
					if (c2 == c && c2 != '\0')
					{
						c = '\0';
					}
					else if (c == '\0')
					{
						if (flag3 && c2 != '\0' && c2 != '0' && c2 != '#')
						{
							flag3 = false;
							flag = customInfo.DecimalPointPos < 0;
							flag2 = !flag;
							num2--;
						}
						else
						{
							if (c2 <= 'E')
							{
								switch (c2)
								{
								case '"':
								case '\'':
									if (c2 == '"' || c2 == '\'')
									{
										c = c2;
										goto IL_0292;
									}
									goto IL_0292;
								case '#':
									if (flag4 && flag)
									{
										customInfo.IntegerHeadSharpDigits++;
									}
									else if (flag2)
									{
										customInfo.DecimalTailSharpDigits++;
									}
									else if (flag3)
									{
										customInfo.ExponentTailSharpDigits++;
									}
									break;
								case '$':
								case '&':
									goto IL_0292;
								case '%':
									customInfo.Percents++;
									goto IL_0292;
								default:
									switch (c2)
									{
									case ',':
										if (flag && customInfo.IntegerDigits > 0)
										{
											num++;
											goto IL_0292;
										}
										goto IL_0292;
									case '-':
									case '/':
										goto IL_0292;
									case '.':
										flag = false;
										flag2 = true;
										flag3 = false;
										if (customInfo.DecimalPointPos == -1)
										{
											customInfo.DecimalPointPos = num2;
											goto IL_0292;
										}
										goto IL_0292;
									case '0':
										break;
									default:
										if (c2 != 'E')
										{
											goto IL_0292;
										}
										goto IL_01CC;
									}
									break;
								}
								if (c2 != '#')
								{
									flag4 = false;
									if (flag2)
									{
										customInfo.DecimalTailSharpDigits = 0;
									}
									else if (flag3)
									{
										customInfo.ExponentTailSharpDigits = 0;
									}
								}
								if (customInfo.IntegerHeadPos == -1)
								{
									customInfo.IntegerHeadPos = num2;
								}
								if (flag)
								{
									customInfo.IntegerDigits++;
									if (num > 0)
									{
										customInfo.UseGroup = true;
									}
									num = 0;
									goto IL_0292;
								}
								if (flag2)
								{
									customInfo.DecimalDigits++;
									goto IL_0292;
								}
								if (flag3)
								{
									customInfo.ExponentDigits++;
									goto IL_0292;
								}
								goto IL_0292;
							}
							else
							{
								if (c2 == '\\')
								{
									num2++;
									goto IL_0292;
								}
								if (c2 != 'e')
								{
									if (c2 != '‰')
									{
										goto IL_0292;
									}
									customInfo.Permilles++;
									goto IL_0292;
								}
							}
							IL_01CC:
							if (!customInfo.UseExponent)
							{
								customInfo.UseExponent = true;
								flag = false;
								flag2 = false;
								flag3 = true;
								if (num2 + 1 - offset < length)
								{
									char c3 = format[num2 + 1];
									if (c3 == '+')
									{
										customInfo.ExponentNegativeSignOnly = false;
									}
									if (c3 == '+' || c3 == '-')
									{
										num2++;
									}
									else if (c3 != '0' && c3 != '#')
									{
										customInfo.UseExponent = false;
										if (customInfo.DecimalPointPos < 0)
										{
											flag = true;
										}
									}
								}
							}
						}
					}
					IL_0292:
					num2++;
				}
				if (customInfo.ExponentDigits == 0)
				{
					customInfo.UseExponent = false;
				}
				else
				{
					customInfo.IntegerHeadSharpDigits = 0;
				}
				if (customInfo.DecimalDigits == 0)
				{
					customInfo.DecimalPointPos = -1;
				}
				customInfo.DividePlaces += num * 3;
				return customInfo;
			}

			public string Format(string format, int offset, int length, NumberFormatInfo nfi, bool positive, StringBuilder sb_int, StringBuilder sb_dec, StringBuilder sb_exp)
			{
				StringBuilder stringBuilder = new StringBuilder();
				char c = '\0';
				bool flag = true;
				bool flag2 = false;
				int num = 0;
				int i = 0;
				int num2 = 0;
				int[] numberGroupSizes = nfi.NumberGroupSizes;
				string numberGroupSeparator = nfi.NumberGroupSeparator;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				if (this.UseGroup && numberGroupSizes.Length != 0)
				{
					num3 = sb_int.Length;
					for (int j = 0; j < numberGroupSizes.Length; j++)
					{
						num4 += numberGroupSizes[j];
						if (num4 <= num3)
						{
							num5 = j;
						}
					}
					num7 = numberGroupSizes[num5];
					int num8 = ((num3 > num4) ? (num3 - num4) : 0);
					if (num7 == 0)
					{
						while (num5 >= 0 && numberGroupSizes[num5] == 0)
						{
							num5--;
						}
						num7 = ((num8 > 0) ? num8 : numberGroupSizes[num5]);
					}
					if (num8 == 0)
					{
						num6 = num7;
					}
					else
					{
						num5 += num8 / num7;
						num6 = num8 % num7;
						if (num6 == 0)
						{
							num6 = num7;
						}
						else
						{
							num5++;
						}
					}
				}
				else
				{
					this.UseGroup = false;
				}
				int num9 = offset;
				while (num9 - offset < length)
				{
					char c2 = format[num9];
					if (c2 == c && c2 != '\0')
					{
						c = '\0';
					}
					else if (c != '\0')
					{
						stringBuilder.Append(c2);
					}
					else
					{
						if (c2 <= 'E')
						{
							switch (c2)
							{
							case '"':
							case '\'':
								if (c2 == '"' || c2 == '\'')
								{
									c = c2;
									goto IL_03CC;
								}
								goto IL_03CC;
							case '#':
								break;
							case '$':
							case '&':
								goto IL_03C3;
							case '%':
								stringBuilder.Append(nfi.PercentSymbol);
								goto IL_03CC;
							default:
								switch (c2)
								{
								case ',':
									goto IL_03CC;
								case '-':
								case '/':
									goto IL_03C3;
								case '.':
									if (this.DecimalPointPos == num9)
									{
										if (this.DecimalDigits > 0)
										{
											while (i < sb_int.Length)
											{
												stringBuilder.Append(sb_int[i++]);
											}
										}
										if (sb_dec.Length > 0)
										{
											stringBuilder.Append(nfi.NumberDecimalSeparator);
										}
									}
									flag = false;
									flag2 = true;
									goto IL_03CC;
								case '0':
									break;
								default:
									if (c2 != 'E')
									{
										goto IL_03C3;
									}
									goto IL_02A3;
								}
								break;
							}
							if (flag)
							{
								num++;
								if (this.IntegerDigits - num >= sb_int.Length + i)
								{
									if (c2 != '0')
									{
										goto IL_03CC;
									}
								}
								while (this.IntegerDigits - num + i < sb_int.Length)
								{
									stringBuilder.Append(sb_int[i++]);
									if (this.UseGroup && --num3 > 0 && --num6 == 0)
									{
										stringBuilder.Append(numberGroupSeparator);
										if (--num5 < numberGroupSizes.Length && num5 >= 0)
										{
											num7 = numberGroupSizes[num5];
										}
										num6 = num7;
									}
								}
								goto IL_03CC;
							}
							if (!flag2)
							{
								stringBuilder.Append(c2);
								goto IL_03CC;
							}
							if (num2 < sb_dec.Length)
							{
								stringBuilder.Append(sb_dec[num2++]);
								goto IL_03CC;
							}
							goto IL_03CC;
						}
						else if (c2 != '\\')
						{
							if (c2 != 'e')
							{
								if (c2 != '‰')
								{
									goto IL_03C3;
								}
								stringBuilder.Append(nfi.PerMilleSymbol);
								goto IL_03CC;
							}
						}
						else
						{
							num9++;
							if (num9 - offset < length)
							{
								stringBuilder.Append(format[num9]);
								goto IL_03CC;
							}
							goto IL_03CC;
						}
						IL_02A3:
						if (sb_exp == null || !this.UseExponent)
						{
							stringBuilder.Append(c2);
							goto IL_03CC;
						}
						bool flag3 = true;
						bool flag4 = false;
						int num10 = num9 + 1;
						while (num10 - offset < length)
						{
							if (format[num10] == '0')
							{
								flag4 = true;
							}
							else if (num10 != num9 + 1 || (format[num10] != '+' && format[num10] != '-'))
							{
								if (!flag4)
								{
									flag3 = false;
									break;
								}
								break;
							}
							num10++;
						}
						if (flag3)
						{
							num9 = num10 - 1;
							flag = this.DecimalPointPos < 0;
							flag2 = !flag;
							stringBuilder.Append(c2);
							stringBuilder.Append(sb_exp);
							sb_exp = null;
							goto IL_03CC;
						}
						stringBuilder.Append(c2);
						goto IL_03CC;
						IL_03C3:
						stringBuilder.Append(c2);
					}
					IL_03CC:
					num9++;
				}
				if (!positive)
				{
					stringBuilder.Insert(0, nfi.NegativeSign);
				}
				return stringBuilder.ToString();
			}

			public bool UseGroup;

			public int DecimalDigits;

			public int DecimalPointPos = -1;

			public int DecimalTailSharpDigits;

			public int IntegerDigits;

			public int IntegerHeadSharpDigits;

			public int IntegerHeadPos;

			public bool UseExponent;

			public int ExponentDigits;

			public int ExponentTailSharpDigits;

			public bool ExponentNegativeSignOnly = true;

			public int DividePlaces;

			public int Percents;

			public int Permilles;
		}
	}
}
