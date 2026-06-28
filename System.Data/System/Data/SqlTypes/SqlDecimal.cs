using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlDecimal : IXmlSerializable, IComparable, INullable
	{
		public SqlDecimal(decimal value)
		{
			int[] bits = decimal.GetBits(value);
			this.precision = SqlDecimal.MaxPrecision;
			this.scale = (byte)((uint)bits[3] >> 16);
			if (this.scale > SqlDecimal.MaxScale || (bits[3] & 2130771967) != 0)
			{
				throw new ArgumentException(Locale.GetText("Invalid scale"));
			}
			this.value = new int[4];
			this.value[0] = bits[0];
			this.value[1] = bits[1];
			this.value[2] = bits[2];
			this.value[3] = 0;
			this.positive = value >= 0m;
			this.notNull = true;
			this.precision = this.GetPrecision(value);
		}

		public SqlDecimal(double dVal)
		{
			this = new SqlDecimal((decimal)dVal);
			SqlDecimal sqlDecimal = this;
			int num = (int)(17 - this.precision);
			if (num > 0)
			{
				sqlDecimal = SqlDecimal.AdjustScale(this, num, false);
			}
			else
			{
				sqlDecimal = SqlDecimal.Round(this, 17);
			}
			this.notNull = sqlDecimal.notNull;
			this.positive = sqlDecimal.positive;
			this.precision = sqlDecimal.precision;
			this.scale = sqlDecimal.scale;
			this.value = sqlDecimal.value;
		}

		public SqlDecimal(int value)
		{
			this = new SqlDecimal(value);
		}

		public SqlDecimal(long value)
		{
			this = new SqlDecimal(value);
		}

		public SqlDecimal(byte bPrecision, byte bScale, bool fPositive, int[] bits)
		{
			this = new SqlDecimal(bPrecision, bScale, fPositive, bits[0], bits[1], bits[2], bits[3]);
		}

		public SqlDecimal(byte bPrecision, byte bScale, bool fPositive, int data1, int data2, int data3, int data4)
		{
			this.precision = bPrecision;
			this.scale = bScale;
			this.positive = fPositive;
			this.value = new int[4];
			this.value[0] = data1;
			this.value[1] = data2;
			this.value[2] = data3;
			this.value[3] = data4;
			this.notNull = true;
			if (this.precision < this.scale)
			{
				throw new SqlTypeException(Locale.GetText("Invalid presicion/scale combination."));
			}
			if (this.precision > 38)
			{
				throw new SqlTypeException(Locale.GetText("Invalid precision/scale combination."));
			}
			if (this.ToDouble() > Math.Pow(10.0, 38.0) - 1.0 || this.ToDouble() < -Math.Pow(10.0, 38.0))
			{
				throw new OverflowException("Can't convert to SqlDecimal, Out of range ");
			}
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				return;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return;
			default:
				reader.MoveToContent();
				if (reader.EOF)
				{
					return;
				}
				reader.Read();
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					return;
				}
				if (reader.Value.Length > 0)
				{
					if (string.Compare("Null", reader.Value) == 0)
					{
						this.notNull = false;
						return;
					}
					SqlDecimal sqlDecimal = new SqlDecimal(decimal.Parse(reader.Value));
					this.value = sqlDecimal.Data;
					this.notNull = true;
					this.scale = sqlDecimal.Scale;
					this.precision = sqlDecimal.Precision;
					this.positive = sqlDecimal.IsPositive;
				}
				return;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(this.Value.ToString());
		}

		public byte[] BinData
		{
			get
			{
				byte[] array = new byte[this.value.Length * 4];
				int num = 0;
				for (int i = 0; i < this.value.Length; i++)
				{
					array[num++] = (byte)(255 & this.value[i]);
					array[num++] = (byte)(255 & (this.value[i] >> 8));
					array[num++] = (byte)(255 & (this.value[i] >> 16));
					array[num++] = (byte)(255 & (this.value[i] >> 24));
				}
				return array;
			}
		}

		public int[] Data
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				return new int[]
				{
					this.value[0],
					this.value[1],
					this.value[2],
					this.value[3]
				};
			}
		}

		public bool IsNull
		{
			get
			{
				return !this.notNull;
			}
		}

		public bool IsPositive
		{
			get
			{
				return this.positive;
			}
		}

		public byte Precision
		{
			get
			{
				return this.precision;
			}
		}

		public byte Scale
		{
			get
			{
				return this.scale;
			}
		}

		public decimal Value
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				if (this.value[3] > 0)
				{
					throw new OverflowException();
				}
				return new decimal(this.value[0], this.value[1], this.value[2], !this.positive, this.scale);
			}
		}

		public static SqlDecimal Abs(SqlDecimal n)
		{
			if (!n.notNull)
			{
				return n;
			}
			return new SqlDecimal(n.Precision, n.Scale, true, n.Data);
		}

		public static SqlDecimal Add(SqlDecimal x, SqlDecimal y)
		{
			return x + y;
		}

		public static SqlDecimal AdjustScale(SqlDecimal n, int digits, bool fRound)
		{
			byte b = n.Precision;
			if (n.IsNull)
			{
				throw new SqlNullValueException();
			}
			if (digits == 0)
			{
				return n;
			}
			byte b2;
			if (digits > 0)
			{
				b = (byte)((int)b + digits);
				b2 = (byte)((int)n.scale + digits);
				for (int i = 0; i < digits; i++)
				{
					n *= 10L;
				}
			}
			else
			{
				if ((int)n.Scale < Math.Abs(digits))
				{
					throw new SqlTruncateException();
				}
				if (fRound)
				{
					n = SqlDecimal.Round(n, digits + (int)n.scale);
				}
				else
				{
					n = SqlDecimal.Round(SqlDecimal.Truncate(n, digits + (int)n.scale), digits + (int)n.scale);
				}
				b2 = n.scale;
			}
			return new SqlDecimal(b, b2, n.positive, n.Data);
		}

		public static SqlDecimal Ceiling(SqlDecimal n)
		{
			if (!n.notNull)
			{
				return n;
			}
			return SqlDecimal.AdjustScale(n, (int)(-(int)n.Scale), true);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlDecimal))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Data.SqlTypes.SqlDecimal"));
			}
			return this.CompareTo((SqlDecimal)value);
		}

		public int CompareTo(SqlDecimal value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.Value.CompareTo(value.Value);
		}

		public static SqlDecimal ConvertToPrecScale(SqlDecimal n, int precision, int scale)
		{
			int num = (int)n.Precision;
			int num2 = (int)n.Scale;
			n = SqlDecimal.AdjustScale(n, scale - (int)n.scale, true);
			if ((int)n.Scale >= num2 && precision < (int)n.Precision)
			{
				throw new SqlTruncateException();
			}
			return new SqlDecimal((byte)precision, n.scale, n.IsPositive, n.Data);
		}

		public static SqlDecimal Divide(SqlDecimal x, SqlDecimal y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlDecimal))
			{
				return false;
			}
			if (this.IsNull)
			{
				return ((SqlDecimal)value).IsNull;
			}
			return !((SqlDecimal)value).IsNull && (bool)(this == (SqlDecimal)value);
		}

		public static SqlBoolean Equals(SqlDecimal x, SqlDecimal y)
		{
			return x == y;
		}

		public static SqlDecimal Floor(SqlDecimal n)
		{
			return SqlDecimal.AdjustScale(n, (int)(-(int)n.Scale), false);
		}

		internal static SqlDecimal FromTdsBigDecimal(TdsBigDecimal x)
		{
			if (x == null)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(x.Precision, x.Scale, !x.IsNegative, x.Data);
		}

		public override int GetHashCode()
		{
			int num = 10;
			num = 91 * num + this.Data[0];
			num = 91 * num + this.Data[1];
			num = 91 * num + this.Data[2];
			num = 91 * num + this.Data[3];
			num = 91 * num + (int)this.Scale;
			return 91 * num + (int)this.Precision;
		}

		public static SqlBoolean GreaterThan(SqlDecimal x, SqlDecimal y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlDecimal x, SqlDecimal y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlDecimal x, SqlDecimal y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlDecimal x, SqlDecimal y)
		{
			return x <= y;
		}

		public static SqlDecimal Multiply(SqlDecimal x, SqlDecimal y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlDecimal x, SqlDecimal y)
		{
			return x != y;
		}

		public static SqlDecimal Parse(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException(Locale.GetText("string s"));
			}
			return new SqlDecimal(decimal.Parse(s));
		}

		public static SqlDecimal Power(SqlDecimal n, double exp)
		{
			if (n.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(Math.Pow(n.ToDouble(), exp));
		}

		public static SqlDecimal Round(SqlDecimal n, int position)
		{
			if (n.IsNull)
			{
				throw new SqlNullValueException();
			}
			decimal num = n.Value;
			num = decimal.Round(num, position);
			return new SqlDecimal(num);
		}

		public static SqlInt32 Sign(SqlDecimal n)
		{
			if (n.IsNull)
			{
				return SqlInt32.Null;
			}
			return (!n.IsPositive) ? (-1) : 1;
		}

		public static SqlDecimal Subtract(SqlDecimal x, SqlDecimal y)
		{
			return x - y;
		}

		private byte GetPrecision(decimal value)
		{
			string text = value.ToString();
			byte b = 0;
			foreach (char c in text)
			{
				if (c >= '0' && c <= '9')
				{
					b += 1;
				}
			}
			return b;
		}

		public double ToDouble()
		{
			double num = this.Data[0];
			num += this.Data[1] * Math.Pow(2.0, 32.0);
			num += this.Data[2] * Math.Pow(2.0, 64.0);
			num += this.Data[3] * Math.Pow(2.0, 96.0);
			return num / Math.Pow(10.0, (double)this.scale);
		}

		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		public SqlDouble ToSqlDouble()
		{
			return this;
		}

		public SqlInt16 ToSqlInt16()
		{
			return (SqlInt16)this;
		}

		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
		}

		public SqlMoney ToSqlMoney()
		{
			return (SqlMoney)this;
		}

		public SqlSingle ToSqlSingle()
		{
			return this;
		}

		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		public override string ToString()
		{
			if (this.IsNull)
			{
				return "Null";
			}
			ulong num = (ulong)this.Data[0];
			num += (ulong)((ulong)((long)this.Data[1]) << 32);
			ulong num2 = (ulong)this.Data[2];
			num2 += (ulong)((ulong)((long)this.Data[3]) << 32);
			uint num3 = 0U;
			StringBuilder stringBuilder = new StringBuilder();
			int num4 = 0;
			while (num != 0UL || num2 != 0UL)
			{
				SqlDecimal.Div128By32(ref num2, ref num, 10U, ref num3);
				stringBuilder.Insert(0, num3.ToString());
				num4++;
			}
			while (stringBuilder.Length > (int)this.Precision)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			if (this.Scale > 0)
			{
				stringBuilder.Insert(stringBuilder.Length - (int)this.Scale, ".");
			}
			if (!this.positive)
			{
				stringBuilder.Insert(0, '-');
			}
			return stringBuilder.ToString();
		}

		private static int Div128By32(ref ulong hi, ref ulong lo, uint divider)
		{
			uint num = 0U;
			return SqlDecimal.Div128By32(ref hi, ref lo, divider, ref num);
		}

		private static int Div128By32(ref ulong hi, ref ulong lo, uint divider, ref uint rest)
		{
			ulong num = (ulong)((uint)(hi >> 32));
			ulong num2 = num / (ulong)divider;
			num -= num2 * (ulong)divider;
			num <<= 32;
			num |= (ulong)((uint)hi);
			ulong num3 = num / (ulong)divider;
			num -= num3 * (ulong)divider;
			num <<= 32;
			hi = (num2 << 32) | (ulong)((uint)num3);
			num |= (ulong)((uint)(lo >> 32));
			num2 = num / (ulong)divider;
			num -= num2 * (ulong)divider;
			num <<= 32;
			num |= (ulong)((uint)lo);
			num3 = num / (ulong)divider;
			num -= num3 * (ulong)divider;
			lo = (num2 << 32) | (ulong)((uint)num3);
			rest = (uint)num;
			num <<= 1;
			return (num <= (ulong)divider && (num != (ulong)divider || (num3 & 1UL) != 1UL)) ? 0 : 1;
		}

		[MonoTODO("Find out what is the right way to set scale and precision")]
		private static SqlDecimal DecimalDiv(SqlDecimal x, SqlDecimal y)
		{
			ulong num = 0UL;
			ulong num2 = 0UL;
			int num3 = 0;
			int num4 = 0;
			bool flag = !(x.positive ^ y.positive);
			byte b = ((x.Precision < y.Precision) ? y.Precision : x.Precision);
			SqlDecimal.DecimalDivSub(ref x, ref y, ref num, ref num2, ref num4);
			num3 = (int)(x.Scale - y.Scale);
			SqlDecimal.Rescale128(ref num, ref num2, ref num3, num4, 0, 38, 1);
			uint num5 = 0U;
			while ((int)b < num3)
			{
				SqlDecimal.Div128By32(ref num2, ref num, 10U, ref num5);
				num3--;
			}
			if (num5 >= 5U)
			{
				num += 1UL;
			}
			while (num2 * Math.Pow(2.0, 64.0) + num - Math.Pow(10.0, (double)b) > 0.0)
			{
				b += 1;
			}
			while ((int)b + num3 > (int)SqlDecimal.MaxScale)
			{
				SqlDecimal.Div128By32(ref num2, ref num, 10U, ref num5);
				num3--;
				if (num5 >= 5U)
				{
					num += 1UL;
				}
			}
			int num6 = (int)num;
			int num7 = (int)(num >> 32);
			int num8 = (int)num2;
			int num9 = (int)(num2 >> 32);
			return new SqlDecimal(b, (byte)num3, flag, num6, num7, num8, num9);
		}

		private static void Rescale128(ref ulong clo, ref ulong chi, ref int scale, int texp, int minScale, int maxScale, int roundFlag)
		{
			int i = 0;
			int num = 0;
			i = scale;
			if (texp > 0)
			{
				while (texp > 0 && i <= maxScale)
				{
					uint num2 = (uint)chi;
					while (texp > 0 && ((clo & 1UL) == 0UL || num2 > 0U))
					{
						if (--texp == 0)
						{
							num = (int)(clo & 1UL);
						}
						SqlDecimal.RShift128(ref clo, ref chi);
						num2 = (uint)(chi >> 32);
					}
					int num3;
					if (texp > 9)
					{
						num3 = 9;
					}
					else
					{
						num3 = texp;
					}
					if (i + num3 > maxScale)
					{
						num3 = maxScale - i;
					}
					if (num3 == 0)
					{
						break;
					}
					texp -= num3;
					i += num3;
					uint num4 = SqlDecimal.constantsDecadeInt32Factors[num3] >> num3;
					SqlDecimal.Mult128By32(ref clo, ref chi, num4, 0);
				}
				while (texp > 0)
				{
					if (--texp == 0)
					{
						num = (int)(clo & 1UL);
					}
					SqlDecimal.RShift128(ref clo, ref chi);
				}
			}
			while (i > maxScale)
			{
				int num3 = scale - maxScale;
				if (num3 > 9)
				{
					num3 = 9;
				}
				i -= num3;
				num = SqlDecimal.Div128By32(ref clo, ref chi, SqlDecimal.constantsDecadeInt32Factors[num3]);
			}
			while (i < minScale)
			{
				if (roundFlag == 0)
				{
					num = 0;
				}
				int num3 = minScale - i;
				if (num3 > 9)
				{
					num3 = 9;
				}
				i += num3;
				SqlDecimal.Mult128By32(ref clo, ref chi, SqlDecimal.constantsDecadeInt32Factors[num3], num);
				num = 0;
			}
			scale = i;
			SqlDecimal.Normalize128(ref clo, ref chi, ref i, roundFlag, num);
		}

		private static void Normalize128(ref ulong clo, ref ulong chi, ref int scale, int roundFlag, int roundBit)
		{
			int num = scale;
			scale = num;
			if (roundFlag != 0 && roundBit != 0)
			{
				SqlDecimal.RoundUp128(ref clo, ref chi);
			}
		}

		private static void RoundUp128(ref ulong lo, ref ulong hi)
		{
			if ((lo += 1UL) == 0UL)
			{
				hi += 1UL;
			}
		}

		private static void DecimalDivSub(ref SqlDecimal x, ref SqlDecimal y, ref ulong clo, ref ulong chi, ref int exp)
		{
			uint num = 0U;
			uint num2 = 0U;
			uint num3 = 0U;
			uint num4 = 0U;
			ulong num5 = (ulong)(((long)x.Data[3] << 32) | (long)x.Data[2]);
			ulong num6 = (ulong)(((long)x.Data[1] << 32) | (long)x.Data[0]);
			ulong num7 = 0UL;
			num = (uint)y.Data[0];
			num2 = (uint)y.Data[1];
			num3 = (uint)y.Data[2];
			num4 = (uint)y.Data[3];
			if (num == 0U && num2 == 0U && num3 == 0U && num4 == 0U)
			{
				throw new DivideByZeroException();
			}
			if (num6 == 0UL && num5 == 0UL)
			{
				clo = (chi = 0UL);
				return;
			}
			int num8 = 0;
			while ((num5 & 9223372036854775808UL) == 0UL)
			{
				SqlDecimal.LShift128(ref num6, ref num5);
				num8++;
			}
			int num9 = 0;
			while (((ulong)num4 & (ulong)(-2147483648)) == 0UL)
			{
				SqlDecimal.LShift128(ref num, ref num2, ref num3, ref num4);
				num9++;
			}
			ulong num10 = ((ulong)num4 << 32) | (ulong)num3;
			ulong num11 = ((ulong)num2 << 32) | (ulong)num;
			ulong num12 = 0UL;
			int num13;
			if (num5 > num10 || (num5 == num10 && num6 >= num11))
			{
				SqlDecimal.Sub192(num7, num6, num5, num12, num11, num10, ref num7, ref num6, ref num5);
				num13 = 1;
			}
			else
			{
				num13 = 0;
			}
			SqlDecimal.Div192By128To128(num7, num6, num5, num, num2, num3, num4, ref clo, ref chi);
			exp = 128 + num8 - num9;
			if (num13 != 0)
			{
				SqlDecimal.RShift128(ref clo, ref chi);
				chi += 9223372036854775808UL;
				exp--;
			}
			while (exp > 0 && (clo & 1UL) == 0UL)
			{
				SqlDecimal.RShift128(ref clo, ref chi);
				exp--;
			}
		}

		private static void RShift128(ref ulong lo, ref ulong hi)
		{
			lo >>= 1;
			if ((hi & 1UL) != 0UL)
			{
				lo |= 9223372036854775808UL;
			}
			hi >>= 1;
		}

		private static void LShift128(ref ulong lo, ref ulong hi)
		{
			hi <<= 1;
			if ((lo & 9223372036854775808UL) != 0UL)
			{
				hi += 1UL;
			}
			lo <<= 1;
		}

		private static void LShift128(ref uint lo, ref uint mi, ref uint mi2, ref uint hi)
		{
			hi <<= 1;
			if (((ulong)mi2 & (ulong)(-2147483648)) != 0UL)
			{
				hi += 1U;
			}
			mi2 <<= 1;
			if (((ulong)mi & (ulong)(-2147483648)) != 0UL)
			{
				mi2 += 1U;
			}
			mi <<= 1;
			if (((ulong)lo & (ulong)(-2147483648)) != 0UL)
			{
				mi += 1U;
			}
			lo <<= 1;
		}

		private static void Div192By128To128(ulong xlo, ulong xmi, ulong xhi, uint ylo, uint ymi, uint ymi2, uint yhi, ref ulong clo, ref ulong chi)
		{
			ulong num = xlo;
			ulong num2 = xmi;
			ulong num3 = xhi;
			uint num4 = SqlDecimal.Div192By128To32WithRest(ref num, ref num2, ref num3, ylo, ymi, ymi2, yhi);
			num3 = (num3 << 32) | (num2 >> 32);
			num2 = (num2 << 32) | (num >> 32);
			num <<= 32;
			chi = ((ulong)num4 << 32) | (ulong)SqlDecimal.Div192By128To32WithRest(ref num, ref num2, ref num3, ylo, ymi, ymi2, yhi);
			num3 = (num3 << 32) | (num2 >> 32);
			num2 = (num2 << 32) | (num >> 32);
			num <<= 32;
			num4 = SqlDecimal.Div192By128To32WithRest(ref num, ref num2, ref num3, ylo, ymi, ymi2, yhi);
			uint num5;
			if (num3 >= (ulong)yhi)
			{
				num5 = uint.MaxValue;
			}
			else
			{
				num3 <<= 32;
				num5 = (uint)(num3 / (ulong)yhi);
			}
			clo = ((ulong)num4 << 32) | (ulong)num5;
		}

		private static uint Div192By128To32WithRest(ref ulong xlo, ref ulong xmi, ref ulong xhi, uint ylo, uint ymi, uint ymi2, uint yhi)
		{
			ulong num = 0UL;
			ulong num2 = 0UL;
			ulong num3 = xlo;
			ulong num4 = xmi;
			ulong num5 = xhi;
			uint num6;
			if (num5 >= (ulong)yhi << 32)
			{
				num6 = uint.MaxValue;
			}
			else
			{
				num6 = (uint)(num5 / (ulong)yhi);
			}
			SqlDecimal.Mult128By32To128(ylo, ymi, ymi2, yhi, num6, ref num, ref num2);
			SqlDecimal.Sub192(num3, num4, num5, 0UL, num, num2, ref num3, ref num4, ref num5);
			while (num5 < 0UL)
			{
				num6 -= 1U;
				SqlDecimal.Add192(num3, num4, num5, 0UL, ((ulong)ymi << 32) | (ulong)ylo, (ulong)(yhi | ymi2), ref num3, ref num4, ref num5);
			}
			xlo = num3;
			xmi = num4;
			xhi = num5;
			return num6;
		}

		private static void Mult128By32(ref ulong clo, ref ulong chi, uint factor, int roundBit)
		{
			ulong num = (ulong)((uint)clo) * (ulong)factor;
			if (roundBit != 0)
			{
				num += (ulong)(factor / 2U);
			}
			uint num2 = (uint)num;
			num >>= 32;
			num += (clo >> 32) * (ulong)factor;
			uint num3 = (uint)num;
			clo = ((ulong)num3 << 32) | (ulong)num2;
			num >>= 32;
			num += (ulong)((uint)chi) * (ulong)factor;
			num2 = (uint)num;
			num >>= 32;
			num += (chi >> 32) * (ulong)factor;
			num3 = (uint)num;
			chi = ((ulong)num3 << 32) | (ulong)num2;
		}

		private static void Mult128By32To128(uint xlo, uint xmi, uint xmi2, uint xhi, uint factor, ref ulong clo, ref ulong chi)
		{
			ulong num = (ulong)xlo * (ulong)factor;
			uint num2 = (uint)num;
			num >>= 32;
			num += (ulong)xmi * (ulong)factor;
			uint num3 = (uint)num;
			num >>= 32;
			num += (ulong)xmi2 * (ulong)factor;
			uint num4 = (uint)num;
			num >>= 32;
			num += (ulong)xhi * (ulong)factor;
			clo = ((ulong)num3 << 32) | (ulong)num2;
			chi = num | (ulong)num4;
		}

		private static void Add192(ulong xlo, ulong xmi, ulong xhi, ulong ylo, ulong ymi, ulong yhi, ref ulong clo, ref ulong cmi, ref ulong chi)
		{
			xlo += ylo;
			if (xlo < ylo)
			{
				xmi += 1UL;
				if (xmi == 0UL)
				{
					xhi += 1UL;
				}
			}
			xmi += ymi;
			if (xmi < ymi)
			{
				xmi += 1UL;
			}
			xhi += yhi;
			clo = xlo;
			cmi = xmi;
			chi = xhi;
		}

		private static void Sub192(ulong xlo, ulong xmi, ulong xhi, ulong ylo, ulong ymi, ulong yhi, ref ulong lo, ref ulong mi, ref ulong hi)
		{
			ulong num = xlo - ylo;
			ulong num2 = xmi - ymi;
			ulong num3 = xhi - yhi;
			if (xlo < ylo)
			{
				if (num2 == 0UL)
				{
					num3 -= 1UL;
				}
				num2 -= 1UL;
			}
			if (xmi < ymi)
			{
				num3 -= 1UL;
			}
			lo = num;
			mi = num2;
			hi = num3;
		}

		public static SqlDecimal Truncate(SqlDecimal n, int position)
		{
			int num = (int)n.scale - position;
			if (num == 0)
			{
				return n;
			}
			int[] array = n.Data;
			decimal num2 = new decimal(array[0], array[1], array[2], !n.positive, 0);
			decimal num3 = 10m;
			int i = 0;
			while (i < num)
			{
				num2 -= num2 % num3;
				i++;
				num3 *= 10m;
			}
			array = decimal.GetBits(num2);
			array[3] = 0;
			return new SqlDecimal(n.precision, n.scale, n.positive, array);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "decimal";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlDecimal operator +(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDecimal.Null;
			}
			if (x.IsPositive && !y.IsPositive)
			{
				y = new SqlDecimal(y.Precision, y.Scale, !y.IsPositive, y.Data);
				return x - y;
			}
			if (!x.IsPositive && y.IsPositive)
			{
				x = new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
				return y - x;
			}
			if (!x.IsPositive && !y.IsPositive)
			{
				x = new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
				y = new SqlDecimal(y.Precision, y.Scale, !y.IsPositive, y.Data);
				x += y;
				return new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
			}
			if (x.scale > y.scale)
			{
				y = SqlDecimal.AdjustScale(y, (int)(x.scale - y.scale), false);
			}
			else if (y.scale > x.scale)
			{
				x = SqlDecimal.AdjustScale(x, (int)(y.scale - x.scale), false);
			}
			byte b = (byte)((int)Math.Max(x.Scale, y.Scale) + Math.Max((int)(x.Precision - x.Scale), (int)(y.Precision - y.Scale)) + 1);
			if (b > SqlDecimal.MaxPrecision)
			{
				b = SqlDecimal.MaxPrecision;
			}
			int[] data = x.Data;
			int[] data2 = y.Data;
			int[] array = new int[4];
			ulong num = 0UL;
			for (int i = 0; i < 4; i++)
			{
				ulong num2 = (ulong)data[i] + (ulong)data2[i] + num;
				array[i] = (int)(num2 & (ulong)(-1));
				num = num2 >> 32;
			}
			if (num > 0UL)
			{
				throw new OverflowException();
			}
			return new SqlDecimal(b, x.Scale, x.IsPositive, array);
		}

		public static SqlDecimal operator /(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDecimal.Null;
			}
			return SqlDecimal.DecimalDiv(x, y);
		}

		public static SqlBoolean operator ==(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return SqlBoolean.False;
			}
			if (x.Scale > y.Scale)
			{
				y = SqlDecimal.AdjustScale(y, (int)(x.Scale - y.Scale), false);
			}
			else if (y.Scale > x.Scale)
			{
				x = SqlDecimal.AdjustScale(y, (int)(y.Scale - x.Scale), false);
			}
			for (int i = 0; i < 4; i++)
			{
				if (x.Data[i] != y.Data[i])
				{
					return SqlBoolean.False;
				}
			}
			return SqlBoolean.True;
		}

		public static SqlBoolean operator >(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return new SqlBoolean(x.IsPositive);
			}
			if (x.Scale > y.Scale)
			{
				y = SqlDecimal.AdjustScale(y, (int)(x.Scale - y.Scale), false);
			}
			else if (y.Scale > x.Scale)
			{
				x = SqlDecimal.AdjustScale(x, (int)(y.Scale - x.Scale), false);
			}
			for (int i = 3; i >= 0; i--)
			{
				if (x.Data[i] != 0 || y.Data[i] != 0)
				{
					return new SqlBoolean(x.Data[i] > y.Data[i]);
				}
			}
			return new SqlBoolean(false);
		}

		public static SqlBoolean operator >=(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return new SqlBoolean(x.IsPositive);
			}
			if (x.Scale > y.Scale)
			{
				y = SqlDecimal.AdjustScale(y, (int)(x.Scale - y.Scale), true);
			}
			else if (y.Scale > x.Scale)
			{
				x = SqlDecimal.AdjustScale(x, (int)(y.Scale - x.Scale), true);
			}
			for (int i = 3; i >= 0; i--)
			{
				if (x.Data[i] != 0 || y.Data[i] != 0)
				{
					return new SqlBoolean(x.Data[i] >= y.Data[i]);
				}
			}
			return new SqlBoolean(true);
		}

		public static SqlBoolean operator !=(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return SqlBoolean.True;
			}
			if (x.Scale > y.Scale)
			{
				x = SqlDecimal.AdjustScale(x, (int)(y.Scale - x.Scale), true);
			}
			else if (y.Scale > x.Scale)
			{
				y = SqlDecimal.AdjustScale(y, (int)(x.Scale - y.Scale), true);
			}
			for (int i = 0; i < 4; i++)
			{
				if (x.Data[i] != y.Data[i])
				{
					return SqlBoolean.True;
				}
			}
			return SqlBoolean.False;
		}

		public static SqlBoolean operator <(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return new SqlBoolean(y.IsPositive);
			}
			if (x.Scale > y.Scale)
			{
				y = SqlDecimal.AdjustScale(y, (int)(x.Scale - y.Scale), true);
			}
			else if (y.Scale > x.Scale)
			{
				x = SqlDecimal.AdjustScale(x, (int)(y.Scale - x.Scale), true);
			}
			for (int i = 3; i >= 0; i--)
			{
				if (x.Data[i] != 0 || y.Data[i] != 0)
				{
					return new SqlBoolean(x.Data[i] < y.Data[i]);
				}
			}
			return new SqlBoolean(false);
		}

		public static SqlBoolean operator <=(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return new SqlBoolean(y.IsPositive);
			}
			if (x.Scale > y.Scale)
			{
				y = SqlDecimal.AdjustScale(y, (int)(x.Scale - y.Scale), true);
			}
			else if (y.Scale > x.Scale)
			{
				x = SqlDecimal.AdjustScale(x, (int)(y.Scale - x.Scale), true);
			}
			for (int i = 3; i >= 0; i--)
			{
				if (x.Data[i] != 0 || y.Data[i] != 0)
				{
					return new SqlBoolean(x.Data[i] <= y.Data[i]);
				}
			}
			return new SqlBoolean(true);
		}

		public static SqlDecimal operator *(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDecimal.Null;
			}
			byte b = x.Precision + y.Precision + 1;
			byte b2 = x.Scale + y.Scale;
			if (b > SqlDecimal.MaxPrecision)
			{
				b = SqlDecimal.MaxPrecision;
			}
			int[] data = x.Data;
			int[] data2 = y.Data;
			int[] array = new int[4];
			ulong num = 0UL;
			for (int i = 0; i < 4; i++)
			{
				ulong num2 = 0UL;
				for (int j = i; j <= i; j++)
				{
					num2 += (ulong)data[j] * (ulong)data2[i - j];
				}
				array[i] = (int)((num2 + num) & (ulong)(-1));
				num = num2 >> 32;
			}
			if (num > 0UL)
			{
				throw new OverflowException();
			}
			return new SqlDecimal(b, b2, x.IsPositive == y.IsPositive, array);
		}

		public static SqlDecimal operator -(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDecimal.Null;
			}
			if (x.IsPositive && !y.IsPositive)
			{
				y = new SqlDecimal(y.Precision, y.Scale, !y.IsPositive, y.Data);
				return x + y;
			}
			if (!x.IsPositive && y.IsPositive)
			{
				x = new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
				x += y;
				return new SqlDecimal(x.Precision, x.Scale, false, x.Data);
			}
			if (!x.IsPositive && !y.IsPositive)
			{
				y = new SqlDecimal(y.Precision, y.Scale, !y.IsPositive, y.Data);
				x = new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
				return y - x;
			}
			if (x.scale > y.scale)
			{
				y = SqlDecimal.AdjustScale(y, (int)(x.scale - y.scale), false);
			}
			else if (y.scale > x.scale)
			{
				x = SqlDecimal.AdjustScale(x, (int)(y.scale - x.scale), false);
			}
			byte b = (byte)((int)Math.Max(x.Scale, y.Scale) + Math.Max((int)(x.Precision - x.Scale), (int)(y.Precision - y.Scale)));
			int[] array;
			int[] array2;
			if (x >= y)
			{
				array = x.Data;
				array2 = y.Data;
			}
			else
			{
				array = y.Data;
				array2 = x.Data;
			}
			int num = 0;
			int[] array3 = new int[4];
			for (int i = 0; i < 4; i++)
			{
				ulong num2 = (ulong)array[i] - (ulong)array2[i] + (ulong)((long)num);
				num = 0;
				if (array2[i] > array[i])
				{
					num = -1;
				}
				array3[i] = (int)num2;
			}
			if (num > 0)
			{
				throw new OverflowException();
			}
			return new SqlDecimal(b, x.Scale, (x >= y).Value, array3);
		}

		public static SqlDecimal operator -(SqlDecimal x)
		{
			return new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
		}

		public static explicit operator SqlDecimal(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(x.ByteValue);
		}

		public static explicit operator decimal(SqlDecimal x)
		{
			return x.Value;
		}

		public static explicit operator SqlDecimal(SqlDouble x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(x.Value);
		}

		public static explicit operator SqlDecimal(SqlSingle x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal((double)x.Value);
		}

		public static explicit operator SqlDecimal(SqlString x)
		{
			return SqlDecimal.Parse(x.Value);
		}

		public static explicit operator SqlDecimal(double x)
		{
			return new SqlDecimal(x);
		}

		public static implicit operator SqlDecimal(long x)
		{
			return new SqlDecimal(x);
		}

		public static implicit operator SqlDecimal(decimal x)
		{
			return new SqlDecimal(x);
		}

		public static implicit operator SqlDecimal(SqlByte x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(x.Value);
		}

		public static implicit operator SqlDecimal(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(x.Value);
		}

		public static implicit operator SqlDecimal(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(x.Value);
		}

		public static implicit operator SqlDecimal(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(x.Value);
		}

		public static implicit operator SqlDecimal(SqlMoney x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			return new SqlDecimal(x.Value);
		}

		private const int SCALE_SHIFT = 16;

		private const int SIGN_SHIFT = 31;

		private const int RESERVED_SS32_BITS = 2130771967;

		private const ulong LIT_GUINT64_HIGHBIT = 9223372036854775808UL;

		private const ulong LIT_GUINT32_HIGHBIT = 2147483648UL;

		private const byte DECIMAL_MAX_INTFACTORS = 9;

		private int[] value;

		private byte precision;

		private byte scale;

		private bool positive;

		private bool notNull;

		private static uint[] constantsDecadeInt32Factors = new uint[] { 1U, 10U, 100U, 1000U, 10000U, 100000U, 1000000U, 10000000U, 100000000U, 1000000000U };

		public static readonly byte MaxPrecision = 38;

		public static readonly byte MaxScale = 38;

		public static readonly SqlDecimal MaxValue = new SqlDecimal(SqlDecimal.MaxPrecision, 0, true, -1, 160047679, 1518781562, 1262177448);

		public static readonly SqlDecimal MinValue = new SqlDecimal(SqlDecimal.MaxPrecision, 0, false, -1, 160047679, 1518781562, 1262177448);

		public static readonly SqlDecimal Null;
	}
}
