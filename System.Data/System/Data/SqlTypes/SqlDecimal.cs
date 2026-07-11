using System;
using System.Data.Common;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlDecimal : INullable, IComparable, IXmlSerializable
	{
		private byte CalculatePrecision()
		{
			int num;
			uint[] array;
			uint num2;
			if (this._data4 != 0U)
			{
				num = 33;
				array = SqlDecimal.s_decimalHelpersHiHi;
				num2 = this._data4;
			}
			else if (this._data3 != 0U)
			{
				num = 24;
				array = SqlDecimal.s_decimalHelpersHi;
				num2 = this._data3;
			}
			else if (this._data2 != 0U)
			{
				num = 15;
				array = SqlDecimal.s_decimalHelpersMid;
				num2 = this._data2;
			}
			else
			{
				num = 5;
				array = SqlDecimal.s_decimalHelpersLo;
				num2 = this._data1;
			}
			if (num2 < array[num])
			{
				num -= 2;
				if (num2 < array[num])
				{
					num -= 2;
					if (num2 < array[num])
					{
						num--;
					}
					else
					{
						num++;
					}
				}
				else
				{
					num++;
				}
			}
			else
			{
				num += 2;
				if (num2 < array[num])
				{
					num--;
				}
				else
				{
					num++;
				}
			}
			if (num2 >= array[num])
			{
				num++;
				if (num == 37 && num2 >= array[num])
				{
					num++;
				}
			}
			byte b = (byte)(num + 1);
			if (b > 1 && this.VerifyPrecision(b - 1))
			{
				b -= 1;
			}
			return Math.Max(b, this._bScale);
		}

		private bool VerifyPrecision(byte precision)
		{
			int num = (int)(checked(precision - 1));
			if (this._data4 < SqlDecimal.s_decimalHelpersHiHi[num])
			{
				return true;
			}
			if (this._data4 == SqlDecimal.s_decimalHelpersHiHi[num])
			{
				if (this._data3 < SqlDecimal.s_decimalHelpersHi[num])
				{
					return true;
				}
				if (this._data3 == SqlDecimal.s_decimalHelpersHi[num])
				{
					if (this._data2 < SqlDecimal.s_decimalHelpersMid[num])
					{
						return true;
					}
					if (this._data2 == SqlDecimal.s_decimalHelpersMid[num] && this._data1 < SqlDecimal.s_decimalHelpersLo[num])
					{
						return true;
					}
				}
			}
			return false;
		}

		private SqlDecimal(bool fNull)
		{
			this._bLen = (this._bPrec = (this._bScale = 0));
			this._bStatus = 0;
			this._data1 = (this._data2 = (this._data3 = (this._data4 = SqlDecimal.s_uiZero)));
		}

		public SqlDecimal(decimal value)
		{
			this._bStatus = SqlDecimal.s_bNotNull;
			int[] bits = decimal.GetBits(value);
			uint num = (uint)bits[3];
			this._data1 = (uint)bits[0];
			this._data2 = (uint)bits[1];
			this._data3 = (uint)bits[2];
			this._data4 = SqlDecimal.s_uiZero;
			this._bStatus |= (((num & 2147483648U) == 2147483648U) ? SqlDecimal.s_bNegative : 0);
			if (this._data3 != 0U)
			{
				this._bLen = 3;
			}
			else if (this._data2 != 0U)
			{
				this._bLen = 2;
			}
			else
			{
				this._bLen = 1;
			}
			this._bScale = (byte)((int)(num & 16711680U) >> 16);
			this._bPrec = 0;
			this._bPrec = this.CalculatePrecision();
		}

		public SqlDecimal(int value)
		{
			this._bStatus = SqlDecimal.s_bNotNull;
			uint num = (uint)value;
			if (value < 0)
			{
				this._bStatus |= SqlDecimal.s_bNegative;
				if (value != -2147483648)
				{
					num = (uint)(-(uint)value);
				}
			}
			this._data1 = num;
			this._data2 = (this._data3 = (this._data4 = SqlDecimal.s_uiZero));
			this._bLen = 1;
			this._bPrec = SqlDecimal.BGetPrecUI4(this._data1);
			this._bScale = 0;
		}

		public SqlDecimal(long value)
		{
			this._bStatus = SqlDecimal.s_bNotNull;
			ulong num = (ulong)value;
			if (value < 0L)
			{
				this._bStatus |= SqlDecimal.s_bNegative;
				if (value != -9223372036854775808L)
				{
					num = (ulong)(-(ulong)value);
				}
			}
			this._data1 = (uint)num;
			this._data2 = (uint)(num >> 32);
			this._data3 = (this._data4 = 0U);
			this._bLen = ((this._data2 == 0U) ? 1 : 2);
			this._bPrec = SqlDecimal.BGetPrecUI8(num);
			this._bScale = 0;
		}

		public SqlDecimal(byte bPrecision, byte bScale, bool fPositive, int[] bits)
		{
			SqlDecimal.CheckValidPrecScale(bPrecision, bScale);
			if (bits == null)
			{
				throw new ArgumentNullException("bits");
			}
			if (bits.Length != 4)
			{
				throw new ArgumentException(SQLResource.InvalidArraySizeMessage, "bits");
			}
			this._bPrec = bPrecision;
			this._bScale = bScale;
			this._data1 = (uint)bits[0];
			this._data2 = (uint)bits[1];
			this._data3 = (uint)bits[2];
			this._data4 = (uint)bits[3];
			this._bLen = 1;
			for (int i = 3; i >= 0; i--)
			{
				if (bits[i] != 0)
				{
					this._bLen = (byte)(i + 1);
					break;
				}
			}
			this._bStatus = SqlDecimal.s_bNotNull;
			if (!fPositive)
			{
				this._bStatus |= SqlDecimal.s_bNegative;
			}
			if (this.FZero())
			{
				this.SetPositive();
			}
			if (bPrecision < this.CalculatePrecision())
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
		}

		public SqlDecimal(byte bPrecision, byte bScale, bool fPositive, int data1, int data2, int data3, int data4)
		{
			SqlDecimal.CheckValidPrecScale(bPrecision, bScale);
			this._bPrec = bPrecision;
			this._bScale = bScale;
			this._data1 = (uint)data1;
			this._data2 = (uint)data2;
			this._data3 = (uint)data3;
			this._data4 = (uint)data4;
			this._bLen = 1;
			if (data4 == 0)
			{
				if (data3 == 0)
				{
					if (data2 == 0)
					{
						this._bLen = 1;
					}
					else
					{
						this._bLen = 2;
					}
				}
				else
				{
					this._bLen = 3;
				}
			}
			else
			{
				this._bLen = 4;
			}
			this._bStatus = SqlDecimal.s_bNotNull;
			if (!fPositive)
			{
				this._bStatus |= SqlDecimal.s_bNegative;
			}
			if (this.FZero())
			{
				this.SetPositive();
			}
			if (bPrecision < this.CalculatePrecision())
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
		}

		public SqlDecimal(double dVal)
		{
			this = new SqlDecimal(false);
			this._bStatus = SqlDecimal.s_bNotNull;
			if (dVal < 0.0)
			{
				dVal = -dVal;
				this._bStatus |= SqlDecimal.s_bNegative;
			}
			if (dVal >= SqlDecimal.s_DMAX_NUME)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			double num = Math.Floor(dVal);
			double num2 = dVal - num;
			this._bPrec = SqlDecimal.s_NUMERIC_MAX_PRECISION;
			this._bLen = 1;
			if (num > 0.0)
			{
				dVal = Math.Floor(num / SqlDecimal.s_DUINT_BASE);
				this._data1 = (uint)(num - dVal * SqlDecimal.s_DUINT_BASE);
				num = dVal;
				if (num > 0.0)
				{
					dVal = Math.Floor(num / SqlDecimal.s_DUINT_BASE);
					this._data2 = (uint)(num - dVal * SqlDecimal.s_DUINT_BASE);
					num = dVal;
					this._bLen += 1;
					if (num > 0.0)
					{
						dVal = Math.Floor(num / SqlDecimal.s_DUINT_BASE);
						this._data3 = (uint)(num - dVal * SqlDecimal.s_DUINT_BASE);
						num = dVal;
						this._bLen += 1;
						if (num > 0.0)
						{
							dVal = Math.Floor(num / SqlDecimal.s_DUINT_BASE);
							this._data4 = (uint)(num - dVal * SqlDecimal.s_DUINT_BASE);
							this._bLen += 1;
						}
					}
				}
			}
			uint num3 = (uint)(this.FZero() ? 0 : this.CalculatePrecision());
			if (num3 > SqlDecimal.s_DBL_DIG)
			{
				uint num4 = num3 - SqlDecimal.s_DBL_DIG;
				uint num5;
				do
				{
					num5 = this.DivByULong(10U);
					num4 -= 1U;
				}
				while (num4 > 0U);
				num4 = num3 - SqlDecimal.s_DBL_DIG;
				if (num5 >= 5U)
				{
					this.AddULong(1U);
					num3 = (uint)this.CalculatePrecision() + num4;
				}
				do
				{
					this.MultByULong(10U);
					num4 -= 1U;
				}
				while (num4 > 0U);
			}
			this._bScale = (byte)((num3 < SqlDecimal.s_DBL_DIG) ? (SqlDecimal.s_DBL_DIG - num3) : 0U);
			this._bPrec = (byte)(num3 + (uint)this._bScale);
			if (this._bScale > 0)
			{
				num3 = (uint)this._bScale;
				do
				{
					uint num6 = ((num3 >= 9U) ? 9U : num3);
					num2 *= SqlDecimal.s_rgulShiftBase[(int)(num6 - 1U)];
					num3 -= num6;
					this.MultByULong(SqlDecimal.s_rgulShiftBase[(int)(num6 - 1U)]);
					this.AddULong((uint)num2);
					num2 -= Math.Floor(num2);
				}
				while (num3 > 0U);
			}
			if (num2 >= 0.5)
			{
				this.AddULong(1U);
			}
			if (this.FZero())
			{
				this.SetPositive();
			}
		}

		private SqlDecimal(uint[] rglData, byte bLen, byte bPrec, byte bScale, bool fPositive)
		{
			SqlDecimal.CheckValidPrecScale(bPrec, bScale);
			this._bLen = bLen;
			this._bPrec = bPrec;
			this._bScale = bScale;
			this._data1 = rglData[0];
			this._data2 = rglData[1];
			this._data3 = rglData[2];
			this._data4 = rglData[3];
			this._bStatus = SqlDecimal.s_bNotNull;
			if (!fPositive)
			{
				this._bStatus |= SqlDecimal.s_bNegative;
			}
			if (this.FZero())
			{
				this.SetPositive();
			}
		}

		public bool IsNull
		{
			get
			{
				return (this._bStatus & SqlDecimal.s_bNullMask) == SqlDecimal.s_bIsNull;
			}
		}

		public decimal Value
		{
			get
			{
				return this.ToDecimal();
			}
		}

		public bool IsPositive
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				return (this._bStatus & SqlDecimal.s_bSignMask) == SqlDecimal.s_bPositive;
			}
		}

		private void SetPositive()
		{
			this._bStatus &= SqlDecimal.s_bReverseSignMask;
		}

		private void SetSignBit(bool fPositive)
		{
			this._bStatus = (fPositive ? (this._bStatus & SqlDecimal.s_bReverseSignMask) : (this._bStatus | SqlDecimal.s_bNegative));
		}

		public byte Precision
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				return this._bPrec;
			}
		}

		public byte Scale
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				return this._bScale;
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
					(int)this._data1,
					(int)this._data2,
					(int)this._data3,
					(int)this._data4
				};
			}
		}

		public byte[] BinData
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				int num = (int)this._data1;
				int num2 = (int)this._data2;
				int num3 = (int)this._data3;
				int num4 = (int)this._data4;
				byte[] array = new byte[16];
				array[0] = (byte)(num & 255);
				num >>= 8;
				array[1] = (byte)(num & 255);
				num >>= 8;
				array[2] = (byte)(num & 255);
				num >>= 8;
				array[3] = (byte)(num & 255);
				array[4] = (byte)(num2 & 255);
				num2 >>= 8;
				array[5] = (byte)(num2 & 255);
				num2 >>= 8;
				array[6] = (byte)(num2 & 255);
				num2 >>= 8;
				array[7] = (byte)(num2 & 255);
				array[8] = (byte)(num3 & 255);
				num3 >>= 8;
				array[9] = (byte)(num3 & 255);
				num3 >>= 8;
				array[10] = (byte)(num3 & 255);
				num3 >>= 8;
				array[11] = (byte)(num3 & 255);
				array[12] = (byte)(num4 & 255);
				num4 >>= 8;
				array[13] = (byte)(num4 & 255);
				num4 >>= 8;
				array[14] = (byte)(num4 & 255);
				num4 >>= 8;
				array[15] = (byte)(num4 & 255);
				return array;
			}
		}

		public override string ToString()
		{
			if (this.IsNull)
			{
				return SQLResource.NullString;
			}
			uint[] array = new uint[] { this._data1, this._data2, this._data3, this._data4 };
			int bLen = (int)this._bLen;
			char[] array2 = new char[(int)(SqlDecimal.s_NUMERIC_MAX_PRECISION + 1)];
			int i = 0;
			while (bLen > 1 || array[0] != 0U)
			{
				uint num;
				SqlDecimal.MpDiv1(array, ref bLen, SqlDecimal.s_ulBase10, out num);
				array2[i++] = SqlDecimal.ChFromDigit(num);
			}
			while (i <= (int)this._bScale)
			{
				array2[i++] = SqlDecimal.ChFromDigit(0U);
			}
			int num2 = 0;
			int num3 = 0;
			if (this._bScale > 0)
			{
				num2 = 1;
			}
			char[] array3;
			if (this.IsPositive)
			{
				array3 = new char[num2 + i];
			}
			else
			{
				array3 = new char[num2 + i + 1];
				array3[num3++] = '-';
			}
			while (i > 0)
			{
				if (i-- == (int)this._bScale)
				{
					array3[num3++] = '.';
				}
				array3[num3++] = array2[i];
			}
			return new string(array3);
		}

		public static SqlDecimal Parse(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s == SQLResource.NullString)
			{
				return SqlDecimal.Null;
			}
			SqlDecimal @null = SqlDecimal.Null;
			char[] array = s.ToCharArray();
			int num = array.Length;
			int num2 = -1;
			int num3 = 0;
			@null._bPrec = 1;
			@null._bScale = 0;
			@null.SetToZero();
			while (num != 0 && array[num - 1] == ' ')
			{
				num--;
			}
			if (num == 0)
			{
				throw new FormatException(SQLResource.FormatMessage);
			}
			while (array[num3] == ' ')
			{
				num3++;
				num--;
			}
			if (array[num3] == '-')
			{
				@null.SetSignBit(false);
				num3++;
				num--;
			}
			else
			{
				@null.SetSignBit(true);
				if (array[num3] == '+')
				{
					num3++;
					num--;
				}
			}
			while (num > 2 && array[num3] == '0')
			{
				num3++;
				num--;
			}
			if (2 == num && '0' == array[num3] && '.' == array[num3 + 1])
			{
				array[num3] = '.';
				array[num3 + 1] = '0';
			}
			if (num == 0 || num > (int)(SqlDecimal.s_NUMERIC_MAX_PRECISION + 1))
			{
				throw new FormatException(SQLResource.FormatMessage);
			}
			while (num > 1 && array[num3] == '0')
			{
				num3++;
				num--;
			}
			int i;
			for (i = 0; i < num; i++)
			{
				char c = array[num3];
				num3++;
				if (c >= '0' && c <= '9')
				{
					c -= '0';
					@null.MultByULong(SqlDecimal.s_ulBase10);
					@null.AddULong((uint)c);
				}
				else
				{
					if (c != '.' || num2 >= 0)
					{
						throw new FormatException(SQLResource.FormatMessage);
					}
					num2 = i;
				}
			}
			if (num2 < 0)
			{
				@null._bPrec = (byte)i;
				@null._bScale = 0;
			}
			else
			{
				@null._bPrec = (byte)(i - 1);
				@null._bScale = (byte)((int)@null._bPrec - num2);
			}
			if (@null._bPrec > SqlDecimal.s_NUMERIC_MAX_PRECISION)
			{
				throw new FormatException(SQLResource.FormatMessage);
			}
			if (@null._bPrec == 0)
			{
				throw new FormatException(SQLResource.FormatMessage);
			}
			if (@null.FZero())
			{
				@null.SetPositive();
			}
			return @null;
		}

		public double ToDouble()
		{
			if (this.IsNull)
			{
				throw new SqlNullValueException();
			}
			double num = this._data4;
			num = num * (double)SqlDecimal.s_lInt32Base + this._data3;
			num = num * (double)SqlDecimal.s_lInt32Base + this._data2;
			num = num * (double)SqlDecimal.s_lInt32Base + this._data1;
			num /= Math.Pow(10.0, (double)this._bScale);
			if (!this.IsPositive)
			{
				return -num;
			}
			return num;
		}

		private decimal ToDecimal()
		{
			if (this.IsNull)
			{
				throw new SqlNullValueException();
			}
			if (this._data4 != 0U || this._bScale > 28)
			{
				throw new OverflowException(SQLResource.ConversionOverflowMessage);
			}
			return new decimal((int)this._data1, (int)this._data2, (int)this._data3, !this.IsPositive, this._bScale);
		}

		public static implicit operator SqlDecimal(decimal x)
		{
			return new SqlDecimal(x);
		}

		public static explicit operator SqlDecimal(double x)
		{
			return new SqlDecimal(x);
		}

		public static implicit operator SqlDecimal(long x)
		{
			return new SqlDecimal(new decimal(x));
		}

		public static explicit operator decimal(SqlDecimal x)
		{
			return x.Value;
		}

		public static SqlDecimal operator -(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return SqlDecimal.Null;
			}
			SqlDecimal sqlDecimal = x;
			if (sqlDecimal.FZero())
			{
				sqlDecimal.SetPositive();
			}
			else
			{
				sqlDecimal.SetSignBit(!sqlDecimal.IsPositive);
			}
			return sqlDecimal;
		}

		public static SqlDecimal operator +(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDecimal.Null;
			}
			bool flag = true;
			bool flag2 = x.IsPositive;
			bool flag3 = y.IsPositive;
			int bScale = (int)x._bScale;
			int bScale2 = (int)y._bScale;
			int num = Math.Max((int)x._bPrec - bScale, (int)y._bPrec - bScale2);
			int num2 = Math.Max(bScale, bScale2);
			int num3 = num + num2 + 1;
			num3 = Math.Min((int)SqlDecimal.MaxPrecision, num3);
			if (num3 - num < num2)
			{
				num2 = num3 - num;
			}
			if (bScale != num2)
			{
				x.AdjustScale(num2 - bScale, true);
			}
			if (bScale2 != num2)
			{
				y.AdjustScale(num2 - bScale2, true);
			}
			if (!flag2)
			{
				flag2 = !flag2;
				flag3 = !flag3;
				flag = !flag;
			}
			int num4 = (int)x._bLen;
			int num5 = (int)y._bLen;
			uint[] array = new uint[] { x._data1, x._data2, x._data3, x._data4 };
			uint[] array2 = new uint[] { y._data1, y._data2, y._data3, y._data4 };
			byte b;
			if (flag3)
			{
				ulong num6 = 0UL;
				int num7 = 0;
				while (num7 < num4 || num7 < num5)
				{
					if (num7 < num4)
					{
						num6 += (ulong)array[num7];
					}
					if (num7 < num5)
					{
						num6 += (ulong)array2[num7];
					}
					array[num7] = (uint)num6;
					num6 >>= 32;
					num7++;
				}
				if (num6 != 0UL)
				{
					if (num7 == SqlDecimal.s_cNumeMax)
					{
						throw new OverflowException(SQLResource.ArithOverflowMessage);
					}
					array[num7] = (uint)num6;
					num7++;
				}
				b = (byte)num7;
			}
			else
			{
				int num8 = 0;
				if (x.LAbsCmp(y) < 0)
				{
					flag = !flag;
					uint[] array3 = array2;
					array2 = array;
					array = array3;
					num4 = num5;
					num5 = (int)x._bLen;
				}
				ulong num6 = SqlDecimal.s_ulInt32Base;
				int num7 = 0;
				while (num7 < num4 || num7 < num5)
				{
					if (num7 < num4)
					{
						num6 += (ulong)array[num7];
					}
					if (num7 < num5)
					{
						num6 -= (ulong)array2[num7];
					}
					array[num7] = (uint)num6;
					if (array[num7] != 0U)
					{
						num8 = num7;
					}
					num6 >>= 32;
					num6 += SqlDecimal.s_ulInt32BaseForMod;
					num7++;
				}
				b = (byte)(num8 + 1);
			}
			SqlDecimal sqlDecimal = new SqlDecimal(array, b, (byte)num3, (byte)num2, flag);
			if (sqlDecimal.FGt10_38() || sqlDecimal.CalculatePrecision() > SqlDecimal.s_NUMERIC_MAX_PRECISION)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			if (sqlDecimal.FZero())
			{
				sqlDecimal.SetPositive();
			}
			return sqlDecimal;
		}

		public static SqlDecimal operator -(SqlDecimal x, SqlDecimal y)
		{
			return x + -y;
		}

		public static SqlDecimal operator *(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDecimal.Null;
			}
			int bLen = (int)y._bLen;
			int num = (int)(x._bScale + y._bScale);
			int num2 = num;
			int num3 = (int)(x._bPrec - x._bScale + (y._bPrec - y._bScale) + 1);
			int num4 = num2 + num3;
			if (num4 > (int)SqlDecimal.s_NUMERIC_MAX_PRECISION)
			{
				num4 = (int)SqlDecimal.s_NUMERIC_MAX_PRECISION;
			}
			if (num2 > (int)SqlDecimal.s_NUMERIC_MAX_PRECISION)
			{
				num2 = (int)SqlDecimal.s_NUMERIC_MAX_PRECISION;
			}
			num2 = Math.Min(num4 - num3, num2);
			num2 = Math.Max(num2, Math.Min(num, (int)SqlDecimal.s_cNumeDivScaleMin));
			int num5 = num2 - num;
			bool flag = x.IsPositive == y.IsPositive;
			uint[] array = new uint[] { x._data1, x._data2, x._data3, x._data4 };
			uint[] array2 = new uint[] { y._data1, y._data2, y._data3, y._data4 };
			uint[] array3 = new uint[9];
			int i = 0;
			for (int j = 0; j < (int)x._bLen; j++)
			{
				uint num6 = array[j];
				ulong num7 = 0UL;
				i = j;
				for (int k = 0; k < bLen; k++)
				{
					ulong num8 = num7 + (ulong)array3[i];
					ulong num9 = (ulong)array2[k];
					num7 = (ulong)num6 * num9;
					num7 += num8;
					if (num7 < num8)
					{
						num8 = SqlDecimal.s_ulInt32Base;
					}
					else
					{
						num8 = 0UL;
					}
					array3[i++] = (uint)num7;
					num7 = (num7 >> 32) + num8;
				}
				if (num7 != 0UL)
				{
					array3[i++] = (uint)num7;
				}
			}
			while (array3[i] == 0U && i > 0)
			{
				i--;
			}
			int num10 = i + 1;
			if (num5 != 0)
			{
				if (num5 < 0)
				{
					uint num11;
					uint num12;
					do
					{
						if (num5 <= -9)
						{
							num11 = SqlDecimal.s_rgulShiftBase[8];
							num5 += 9;
						}
						else
						{
							num11 = SqlDecimal.s_rgulShiftBase[-num5 - 1];
							num5 = 0;
						}
						SqlDecimal.MpDiv1(array3, ref num10, num11, out num12);
					}
					while (num5 != 0);
					if (num10 > SqlDecimal.s_cNumeMax)
					{
						throw new OverflowException(SQLResource.ArithOverflowMessage);
					}
					for (i = num10; i < SqlDecimal.s_cNumeMax; i++)
					{
						array3[i] = 0U;
					}
					SqlDecimal sqlDecimal = new SqlDecimal(array3, (byte)num10, (byte)num4, (byte)num2, flag);
					if (sqlDecimal.FGt10_38())
					{
						throw new OverflowException(SQLResource.ArithOverflowMessage);
					}
					if (num12 >= num11 / 2U)
					{
						sqlDecimal.AddULong(1U);
					}
					if (sqlDecimal.FZero())
					{
						sqlDecimal.SetPositive();
					}
					return sqlDecimal;
				}
				else
				{
					if (num10 > SqlDecimal.s_cNumeMax)
					{
						throw new OverflowException(SQLResource.ArithOverflowMessage);
					}
					for (i = num10; i < SqlDecimal.s_cNumeMax; i++)
					{
						array3[i] = 0U;
					}
					SqlDecimal sqlDecimal = new SqlDecimal(array3, (byte)num10, (byte)num4, (byte)num, flag);
					if (sqlDecimal.FZero())
					{
						sqlDecimal.SetPositive();
					}
					sqlDecimal.AdjustScale(num5, true);
					return sqlDecimal;
				}
			}
			else
			{
				if (num10 > SqlDecimal.s_cNumeMax)
				{
					throw new OverflowException(SQLResource.ArithOverflowMessage);
				}
				for (i = num10; i < SqlDecimal.s_cNumeMax; i++)
				{
					array3[i] = 0U;
				}
				SqlDecimal sqlDecimal = new SqlDecimal(array3, (byte)num10, (byte)num4, (byte)num2, flag);
				if (sqlDecimal.FGt10_38())
				{
					throw new OverflowException(SQLResource.ArithOverflowMessage);
				}
				if (sqlDecimal.FZero())
				{
					sqlDecimal.SetPositive();
				}
				return sqlDecimal;
			}
		}

		public static SqlDecimal operator /(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDecimal.Null;
			}
			if (y.FZero())
			{
				throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
			}
			bool flag = x.IsPositive == y.IsPositive;
			int num = Math.Max((int)(x._bScale + y._bPrec + 1), (int)SqlDecimal.s_cNumeDivScaleMin);
			int num2 = (int)(x._bPrec - x._bScale + y._bScale);
			int num3 = num + (int)x._bPrec + (int)y._bPrec + 1;
			int num4 = Math.Min(num, (int)SqlDecimal.s_cNumeDivScaleMin);
			num2 = Math.Min(num2, (int)SqlDecimal.s_NUMERIC_MAX_PRECISION);
			num3 = num2 + num;
			if (num3 > (int)SqlDecimal.s_NUMERIC_MAX_PRECISION)
			{
				num3 = (int)SqlDecimal.s_NUMERIC_MAX_PRECISION;
			}
			num = Math.Min(num3 - num2, num);
			num = Math.Max(num, num4);
			int num5 = num - (int)x._bScale + (int)y._bScale;
			x.AdjustScale(num5, true);
			uint[] array = new uint[] { x._data1, x._data2, x._data3, x._data4 };
			uint[] array2 = new uint[] { y._data1, y._data2, y._data3, y._data4 };
			uint[] array3 = new uint[SqlDecimal.s_cNumeMax + 1];
			uint[] array4 = new uint[SqlDecimal.s_cNumeMax];
			int num6;
			int num7;
			SqlDecimal.MpDiv(array, (int)x._bLen, array2, (int)y._bLen, array4, out num6, array3, out num7);
			SqlDecimal.ZeroToMaxLen(array4, num6);
			SqlDecimal sqlDecimal = new SqlDecimal(array4, (byte)num6, (byte)num3, (byte)num, flag);
			if (sqlDecimal.FZero())
			{
				sqlDecimal.SetPositive();
			}
			return sqlDecimal;
		}

		public static explicit operator SqlDecimal(SqlBoolean x)
		{
			if (!x.IsNull)
			{
				return new SqlDecimal((int)x.ByteValue);
			}
			return SqlDecimal.Null;
		}

		public static implicit operator SqlDecimal(SqlByte x)
		{
			if (!x.IsNull)
			{
				return new SqlDecimal((int)x.Value);
			}
			return SqlDecimal.Null;
		}

		public static implicit operator SqlDecimal(SqlInt16 x)
		{
			if (!x.IsNull)
			{
				return new SqlDecimal((int)x.Value);
			}
			return SqlDecimal.Null;
		}

		public static implicit operator SqlDecimal(SqlInt32 x)
		{
			if (!x.IsNull)
			{
				return new SqlDecimal(x.Value);
			}
			return SqlDecimal.Null;
		}

		public static implicit operator SqlDecimal(SqlInt64 x)
		{
			if (!x.IsNull)
			{
				return new SqlDecimal(x.Value);
			}
			return SqlDecimal.Null;
		}

		public static implicit operator SqlDecimal(SqlMoney x)
		{
			if (!x.IsNull)
			{
				return new SqlDecimal(x.ToDecimal());
			}
			return SqlDecimal.Null;
		}

		public static explicit operator SqlDecimal(SqlSingle x)
		{
			if (!x.IsNull)
			{
				return new SqlDecimal((double)x.Value);
			}
			return SqlDecimal.Null;
		}

		public static explicit operator SqlDecimal(SqlDouble x)
		{
			if (!x.IsNull)
			{
				return new SqlDecimal(x.Value);
			}
			return SqlDecimal.Null;
		}

		public static explicit operator SqlDecimal(SqlString x)
		{
			if (!x.IsNull)
			{
				return SqlDecimal.Parse(x.Value);
			}
			return SqlDecimal.Null;
		}

		[Conditional("DEBUG")]
		private void AssertValid()
		{
			if (this.IsNull)
			{
				return;
			}
			object obj = (new uint[] { this._data1, this._data2, this._data3, this._data4 })[(int)(this._bLen - 1)];
			for (int i = (int)this._bLen; i < SqlDecimal.s_cNumeMax; i++)
			{
			}
		}

		private static void ZeroToMaxLen(uint[] rgulData, int cUI4sCur)
		{
			switch (cUI4sCur)
			{
			case 1:
				rgulData[1] = (rgulData[2] = (rgulData[3] = 0U));
				return;
			case 2:
				rgulData[2] = (rgulData[3] = 0U);
				return;
			case 3:
				rgulData[3] = 0U;
				return;
			default:
				return;
			}
		}

		private static byte CLenFromPrec(byte bPrec)
		{
			return SqlDecimal.s_rgCLenFromPrec[(int)(bPrec - 1)];
		}

		private bool FZero()
		{
			return this._data1 == 0U && this._bLen <= 1;
		}

		private bool FGt10_38()
		{
			return (ulong)this._data4 >= 1262177448UL && this._bLen == 4 && ((ulong)this._data4 > 1262177448UL || (ulong)this._data3 > 1518781562UL || ((ulong)this._data3 == 1518781562UL && (ulong)this._data2 >= 160047680UL));
		}

		private bool FGt10_38(uint[] rglData)
		{
			return (ulong)rglData[3] >= 1262177448UL && ((ulong)rglData[3] > 1262177448UL || (ulong)rglData[2] > 1518781562UL || ((ulong)rglData[2] == 1518781562UL && (ulong)rglData[1] >= 160047680UL));
		}

		private static byte BGetPrecUI4(uint value)
		{
			int num;
			if (value < SqlDecimal.s_ulT4)
			{
				if (value < SqlDecimal.s_ulT2)
				{
					num = ((value >= SqlDecimal.s_ulT1) ? 2 : 1);
				}
				else
				{
					num = ((value >= SqlDecimal.s_ulT3) ? 4 : 3);
				}
			}
			else if (value < SqlDecimal.s_ulT8)
			{
				if (value < SqlDecimal.s_ulT6)
				{
					num = ((value >= SqlDecimal.s_ulT5) ? 6 : 5);
				}
				else
				{
					num = ((value >= SqlDecimal.s_ulT7) ? 8 : 7);
				}
			}
			else
			{
				num = ((value >= SqlDecimal.s_ulT9) ? 10 : 9);
			}
			return (byte)num;
		}

		private static byte BGetPrecUI8(uint ulU0, uint ulU1)
		{
			return SqlDecimal.BGetPrecUI8((ulong)ulU0 + ((ulong)ulU1 << 32));
		}

		private static byte BGetPrecUI8(ulong dwlVal)
		{
			int num2;
			if (dwlVal < (ulong)SqlDecimal.s_ulT8)
			{
				uint num = (uint)dwlVal;
				if (num < SqlDecimal.s_ulT4)
				{
					if (num < SqlDecimal.s_ulT2)
					{
						num2 = ((num >= SqlDecimal.s_ulT1) ? 2 : 1);
					}
					else
					{
						num2 = ((num >= SqlDecimal.s_ulT3) ? 4 : 3);
					}
				}
				else if (num < SqlDecimal.s_ulT6)
				{
					num2 = ((num >= SqlDecimal.s_ulT5) ? 6 : 5);
				}
				else
				{
					num2 = ((num >= SqlDecimal.s_ulT7) ? 8 : 7);
				}
			}
			else if (dwlVal < SqlDecimal.s_dwlT16)
			{
				if (dwlVal < SqlDecimal.s_dwlT12)
				{
					if (dwlVal < SqlDecimal.s_dwlT10)
					{
						num2 = ((dwlVal >= (ulong)SqlDecimal.s_ulT9) ? 10 : 9);
					}
					else
					{
						num2 = ((dwlVal >= SqlDecimal.s_dwlT11) ? 12 : 11);
					}
				}
				else if (dwlVal < SqlDecimal.s_dwlT14)
				{
					num2 = ((dwlVal >= SqlDecimal.s_dwlT13) ? 14 : 13);
				}
				else
				{
					num2 = ((dwlVal >= SqlDecimal.s_dwlT15) ? 16 : 15);
				}
			}
			else if (dwlVal < SqlDecimal.s_dwlT18)
			{
				num2 = ((dwlVal >= SqlDecimal.s_dwlT17) ? 18 : 17);
			}
			else
			{
				num2 = ((dwlVal >= SqlDecimal.s_dwlT19) ? 20 : 19);
			}
			return (byte)num2;
		}

		private void AddULong(uint ulAdd)
		{
			ulong num = (ulong)ulAdd;
			int bLen = (int)this._bLen;
			uint[] array = new uint[] { this._data1, this._data2, this._data3, this._data4 };
			int num2 = 0;
			for (;;)
			{
				num += (ulong)array[num2];
				array[num2] = (uint)num;
				num >>= 32;
				if (num == 0UL)
				{
					break;
				}
				num2++;
				if (num2 >= bLen)
				{
					goto Block_2;
				}
			}
			this.StoreFromWorkingArray(array);
			return;
			Block_2:
			if (num2 == SqlDecimal.s_cNumeMax)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			array[num2] = (uint)num;
			this._bLen += 1;
			if (this.FGt10_38(array))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			this.StoreFromWorkingArray(array);
		}

		private void MultByULong(uint uiMultiplier)
		{
			int bLen = (int)this._bLen;
			ulong num = 0UL;
			uint[] array = new uint[] { this._data1, this._data2, this._data3, this._data4 };
			for (int i = 0; i < bLen; i++)
			{
				ulong num2 = (ulong)array[i] * (ulong)uiMultiplier;
				num += num2;
				if (num < num2)
				{
					num2 = SqlDecimal.s_ulInt32Base;
				}
				else
				{
					num2 = 0UL;
				}
				array[i] = (uint)num;
				num = (num >> 32) + num2;
			}
			if (num != 0UL)
			{
				if (bLen == SqlDecimal.s_cNumeMax)
				{
					throw new OverflowException(SQLResource.ArithOverflowMessage);
				}
				array[bLen] = (uint)num;
				this._bLen += 1;
			}
			if (this.FGt10_38(array))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			this.StoreFromWorkingArray(array);
		}

		private uint DivByULong(uint iDivisor)
		{
			ulong num = (ulong)iDivisor;
			ulong num2 = 0UL;
			bool flag = true;
			if (num == 0UL)
			{
				throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
			}
			uint[] array = new uint[] { this._data1, this._data2, this._data3, this._data4 };
			for (int i = (int)this._bLen; i > 0; i--)
			{
				num2 = (num2 << 32) + (ulong)array[i - 1];
				uint num3 = (uint)(num2 / num);
				array[i - 1] = num3;
				num2 %= num;
				if (flag && num3 == 0U)
				{
					this._bLen -= 1;
				}
				else
				{
					flag = false;
				}
			}
			this.StoreFromWorkingArray(array);
			if (flag)
			{
				this._bLen = 1;
			}
			return (uint)num2;
		}

		internal void AdjustScale(int digits, bool fRound)
		{
			bool flag = false;
			int i = digits;
			if (i + (int)this._bScale < 0)
			{
				throw new SqlTruncateException();
			}
			if (i + (int)this._bScale > (int)SqlDecimal.s_NUMERIC_MAX_PRECISION)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			byte b = (byte)(i + (int)this._bScale);
			byte b2 = (byte)Math.Min((int)SqlDecimal.s_NUMERIC_MAX_PRECISION, Math.Max(1, i + (int)this._bPrec));
			if (i > 0)
			{
				this._bScale = b;
				this._bPrec = b2;
				while (i > 0)
				{
					uint num;
					if (i >= 9)
					{
						num = SqlDecimal.s_rgulShiftBase[8];
						i -= 9;
					}
					else
					{
						num = SqlDecimal.s_rgulShiftBase[i - 1];
						i = 0;
					}
					this.MultByULong(num);
				}
			}
			else if (i < 0)
			{
				uint num;
				uint num2;
				do
				{
					if (i <= -9)
					{
						num = SqlDecimal.s_rgulShiftBase[8];
						i += 9;
					}
					else
					{
						num = SqlDecimal.s_rgulShiftBase[-i - 1];
						i = 0;
					}
					num2 = this.DivByULong(num);
				}
				while (i < 0);
				flag = num2 >= num / 2U;
				this._bScale = b;
				this._bPrec = b2;
			}
			if (flag && fRound)
			{
				this.AddULong(1U);
				return;
			}
			if (this.FZero())
			{
				this.SetPositive();
			}
		}

		public static SqlDecimal AdjustScale(SqlDecimal n, int digits, bool fRound)
		{
			if (n.IsNull)
			{
				return SqlDecimal.Null;
			}
			SqlDecimal sqlDecimal = n;
			sqlDecimal.AdjustScale(digits, fRound);
			return sqlDecimal;
		}

		public static SqlDecimal ConvertToPrecScale(SqlDecimal n, int precision, int scale)
		{
			SqlDecimal.CheckValidPrecScale(precision, scale);
			if (n.IsNull)
			{
				return SqlDecimal.Null;
			}
			SqlDecimal sqlDecimal = n;
			int num = scale - (int)sqlDecimal._bScale;
			sqlDecimal.AdjustScale(num, true);
			byte b = SqlDecimal.CLenFromPrec((byte)precision);
			if (b < sqlDecimal._bLen)
			{
				throw new SqlTruncateException();
			}
			if (b == sqlDecimal._bLen && precision < (int)sqlDecimal.CalculatePrecision())
			{
				throw new SqlTruncateException();
			}
			sqlDecimal._bPrec = (byte)precision;
			return sqlDecimal;
		}

		private int LAbsCmp(SqlDecimal snumOp)
		{
			int bLen = (int)snumOp._bLen;
			int bLen2 = (int)this._bLen;
			if (bLen != bLen2)
			{
				if (bLen2 <= bLen)
				{
					return -1;
				}
				return 1;
			}
			else
			{
				uint[] array = new uint[] { this._data1, this._data2, this._data3, this._data4 };
				uint[] array2 = new uint[] { snumOp._data1, snumOp._data2, snumOp._data3, snumOp._data4 };
				int num = bLen - 1;
				while (array[num] == array2[num])
				{
					num--;
					if (num < 0)
					{
						return 0;
					}
				}
				if (array[num] <= array2[num])
				{
					return -1;
				}
				return 1;
			}
		}

		private static void MpMove(uint[] rgulS, int ciulS, uint[] rgulD, out int ciulD)
		{
			ciulD = ciulS;
			for (int i = 0; i < ciulS; i++)
			{
				rgulD[i] = rgulS[i];
			}
		}

		private static void MpSet(uint[] rgulD, out int ciulD, uint iulN)
		{
			ciulD = 1;
			rgulD[0] = iulN;
		}

		private static void MpNormalize(uint[] rgulU, ref int ciulU)
		{
			while (ciulU > 1 && rgulU[ciulU - 1] == 0U)
			{
				ciulU--;
			}
		}

		private static void MpMul1(uint[] piulD, ref int ciulD, uint iulX)
		{
			uint num = 0U;
			int i;
			for (i = 0; i < ciulD; i++)
			{
				ulong num2 = (ulong)piulD[i];
				ulong num3 = (ulong)num + num2 * (ulong)iulX;
				num = SqlDecimal.HI(num3);
				piulD[i] = SqlDecimal.LO(num3);
			}
			if (num != 0U)
			{
				piulD[i] = num;
				ciulD++;
			}
		}

		private static void MpDiv1(uint[] rgulU, ref int ciulU, uint iulD, out uint iulR)
		{
			uint num = 0U;
			ulong num2 = (ulong)iulD;
			int i = ciulU;
			while (i > 0)
			{
				i--;
				ulong num3 = ((ulong)num << 32) + (ulong)rgulU[i];
				rgulU[i] = (uint)(num3 / num2);
				num = (uint)(num3 - (ulong)rgulU[i] * num2);
			}
			iulR = num;
			SqlDecimal.MpNormalize(rgulU, ref ciulU);
		}

		internal static ulong DWL(uint lo, uint hi)
		{
			return (ulong)lo + ((ulong)hi << 32);
		}

		private static uint HI(ulong x)
		{
			return (uint)(x >> 32);
		}

		private static uint LO(ulong x)
		{
			return (uint)x;
		}

		private static void MpDiv(uint[] rgulU, int ciulU, uint[] rgulD, int ciulD, uint[] rgulQ, out int ciulQ, uint[] rgulR, out int ciulR)
		{
			if (ciulD == 1 && rgulD[0] == 0U)
			{
				ciulQ = (ciulR = 0);
				return;
			}
			if (ciulU == 1 && ciulD == 1)
			{
				SqlDecimal.MpSet(rgulQ, out ciulQ, rgulU[0] / rgulD[0]);
				SqlDecimal.MpSet(rgulR, out ciulR, rgulU[0] % rgulD[0]);
				return;
			}
			if (ciulD > ciulU)
			{
				SqlDecimal.MpMove(rgulU, ciulU, rgulR, out ciulR);
				SqlDecimal.MpSet(rgulQ, out ciulQ, 0U);
				return;
			}
			if (ciulU <= 2)
			{
				ulong num = SqlDecimal.DWL(rgulU[0], rgulU[1]);
				ulong num2 = (ulong)rgulD[0];
				if (ciulD > 1)
				{
					num2 += (ulong)rgulD[1] << 32;
				}
				ulong num3 = num / num2;
				rgulQ[0] = SqlDecimal.LO(num3);
				rgulQ[1] = SqlDecimal.HI(num3);
				ciulQ = ((SqlDecimal.HI(num3) != 0U) ? 2 : 1);
				num3 = num % num2;
				rgulR[0] = SqlDecimal.LO(num3);
				rgulR[1] = SqlDecimal.HI(num3);
				ciulR = ((SqlDecimal.HI(num3) != 0U) ? 2 : 1);
				return;
			}
			if (ciulD == 1)
			{
				SqlDecimal.MpMove(rgulU, ciulU, rgulQ, out ciulQ);
				uint num4;
				SqlDecimal.MpDiv1(rgulQ, ref ciulQ, rgulD[0], out num4);
				rgulR[0] = num4;
				ciulR = 1;
				return;
			}
			ciulQ = (ciulR = 0);
			if (rgulU != rgulR)
			{
				SqlDecimal.MpMove(rgulU, ciulU, rgulR, out ciulR);
			}
			ciulQ = ciulU - ciulD + 1;
			uint num5 = rgulD[ciulD - 1];
			rgulR[ciulU] = 0U;
			int num6 = ciulU;
			uint num7 = (uint)(SqlDecimal.s_ulInt32Base / ((ulong)num5 + 1UL));
			if (num7 > 1U)
			{
				SqlDecimal.MpMul1(rgulD, ref ciulD, num7);
				num5 = rgulD[ciulD - 1];
				SqlDecimal.MpMul1(rgulR, ref ciulR, num7);
			}
			uint num8 = rgulD[ciulD - 2];
			do
			{
				ulong num9 = SqlDecimal.DWL(rgulR[num6 - 1], rgulR[num6]);
				uint num10;
				if (num5 == rgulR[num6])
				{
					num10 = (uint)(SqlDecimal.s_ulInt32Base - 1UL);
				}
				else
				{
					num10 = (uint)(num9 / (ulong)num5);
				}
				ulong num11 = (ulong)num10;
				uint num12 = (uint)(num9 - num11 * (ulong)num5);
				while ((ulong)num8 * num11 > SqlDecimal.DWL(rgulR[num6 - 2], num12))
				{
					num10 -= 1U;
					if (num12 >= -num5)
					{
						break;
					}
					num12 += num5;
					num11 = (ulong)num10;
				}
				num9 = SqlDecimal.s_ulInt32Base;
				ulong num13 = 0UL;
				int i = 0;
				int num14 = num6 - ciulD;
				while (i < ciulD)
				{
					ulong num15 = (ulong)rgulD[i];
					num13 += (ulong)num10 * num15;
					num9 += (ulong)rgulR[num14] - (ulong)SqlDecimal.LO(num13);
					num13 = (ulong)SqlDecimal.HI(num13);
					rgulR[num14] = SqlDecimal.LO(num9);
					num9 = (ulong)SqlDecimal.HI(num9) + SqlDecimal.s_ulInt32Base - 1UL;
					i++;
					num14++;
				}
				num9 += (ulong)rgulR[num14] - num13;
				rgulR[num14] = SqlDecimal.LO(num9);
				rgulQ[num6 - ciulD] = num10;
				if (SqlDecimal.HI(num9) == 0U)
				{
					rgulQ[num6 - ciulD] = num10 - 1U;
					uint num16 = 0U;
					i = 0;
					num14 = num6 - ciulD;
					while (i < ciulD)
					{
						num9 = (ulong)rgulD[i] + (ulong)rgulR[num14] + (ulong)num16;
						num16 = SqlDecimal.HI(num9);
						rgulR[num14] = SqlDecimal.LO(num9);
						i++;
						num14++;
					}
					rgulR[num14] += num16;
				}
				num6--;
			}
			while (num6 >= ciulD);
			SqlDecimal.MpNormalize(rgulQ, ref ciulQ);
			ciulR = ciulD;
			SqlDecimal.MpNormalize(rgulR, ref ciulR);
			if (num7 > 1U)
			{
				uint num17;
				SqlDecimal.MpDiv1(rgulD, ref ciulD, num7, out num17);
				SqlDecimal.MpDiv1(rgulR, ref ciulR, num7, out num17);
			}
		}

		private EComparison CompareNm(SqlDecimal snumOp)
		{
			int num = (this.IsPositive ? 1 : (-1));
			int num2 = (snumOp.IsPositive ? 1 : (-1));
			if (num == num2)
			{
				SqlDecimal sqlDecimal = this;
				SqlDecimal sqlDecimal2 = snumOp;
				int num3 = (int)(this._bScale - snumOp._bScale);
				if (num3 < 0)
				{
					try
					{
						sqlDecimal.AdjustScale(-num3, true);
						goto IL_0078;
					}
					catch (OverflowException)
					{
						return (num > 0) ? EComparison.GT : EComparison.LT;
					}
				}
				if (num3 > 0)
				{
					try
					{
						sqlDecimal2.AdjustScale(num3, true);
					}
					catch (OverflowException)
					{
						return (num > 0) ? EComparison.LT : EComparison.GT;
					}
				}
				IL_0078:
				int num4 = sqlDecimal.LAbsCmp(sqlDecimal2);
				if (num4 == 0)
				{
					return EComparison.EQ;
				}
				if (num * num4 < 0)
				{
					return EComparison.LT;
				}
				return EComparison.GT;
			}
			if (num != 1)
			{
				return EComparison.LT;
			}
			return EComparison.GT;
		}

		private static void CheckValidPrecScale(byte bPrec, byte bScale)
		{
			if (bPrec < 1 || bPrec > SqlDecimal.MaxPrecision || bScale < 0 || bScale > SqlDecimal.MaxScale || bScale > bPrec)
			{
				throw new SqlTypeException(SQLResource.InvalidPrecScaleMessage);
			}
		}

		private static void CheckValidPrecScale(int iPrec, int iScale)
		{
			if (iPrec < 1 || iPrec > (int)SqlDecimal.MaxPrecision || iScale < 0 || iScale > (int)SqlDecimal.MaxScale || iScale > iPrec)
			{
				throw new SqlTypeException(SQLResource.InvalidPrecScaleMessage);
			}
		}

		public static SqlBoolean operator ==(SqlDecimal x, SqlDecimal y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.CompareNm(y) == EComparison.EQ);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator !=(SqlDecimal x, SqlDecimal y)
		{
			return !(x == y);
		}

		public static SqlBoolean operator <(SqlDecimal x, SqlDecimal y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.CompareNm(y) == EComparison.LT);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >(SqlDecimal x, SqlDecimal y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.CompareNm(y) == EComparison.GT);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator <=(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			EComparison ecomparison = x.CompareNm(y);
			return new SqlBoolean(ecomparison == EComparison.LT || ecomparison == EComparison.EQ);
		}

		public static SqlBoolean operator >=(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			EComparison ecomparison = x.CompareNm(y);
			return new SqlBoolean(ecomparison == EComparison.GT || ecomparison == EComparison.EQ);
		}

		public static SqlDecimal Add(SqlDecimal x, SqlDecimal y)
		{
			return x + y;
		}

		public static SqlDecimal Subtract(SqlDecimal x, SqlDecimal y)
		{
			return x - y;
		}

		public static SqlDecimal Multiply(SqlDecimal x, SqlDecimal y)
		{
			return x * y;
		}

		public static SqlDecimal Divide(SqlDecimal x, SqlDecimal y)
		{
			return x / y;
		}

		public static SqlBoolean Equals(SqlDecimal x, SqlDecimal y)
		{
			return x == y;
		}

		public static SqlBoolean NotEquals(SqlDecimal x, SqlDecimal y)
		{
			return x != y;
		}

		public static SqlBoolean LessThan(SqlDecimal x, SqlDecimal y)
		{
			return x < y;
		}

		public static SqlBoolean GreaterThan(SqlDecimal x, SqlDecimal y)
		{
			return x > y;
		}

		public static SqlBoolean LessThanOrEqual(SqlDecimal x, SqlDecimal y)
		{
			return x <= y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlDecimal x, SqlDecimal y)
		{
			return x >= y;
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

		private static char ChFromDigit(uint uiDigit)
		{
			return (char)(uiDigit + 48U);
		}

		private void StoreFromWorkingArray(uint[] rguiData)
		{
			this._data1 = rguiData[0];
			this._data2 = rguiData[1];
			this._data3 = rguiData[2];
			this._data4 = rguiData[3];
		}

		private void SetToZero()
		{
			this._bLen = 1;
			this._data1 = (this._data2 = (this._data3 = (this._data4 = 0U)));
			this._bStatus = SqlDecimal.s_bNotNull | SqlDecimal.s_bPositive;
		}

		private void MakeInteger(out bool fFraction)
		{
			int i = (int)this._bScale;
			fFraction = false;
			while (i > 0)
			{
				uint num;
				if (i >= 9)
				{
					num = this.DivByULong(SqlDecimal.s_rgulShiftBase[8]);
					i -= 9;
				}
				else
				{
					num = this.DivByULong(SqlDecimal.s_rgulShiftBase[i - 1]);
					i = 0;
				}
				if (num != 0U)
				{
					fFraction = true;
				}
			}
			this._bScale = 0;
		}

		public static SqlDecimal Abs(SqlDecimal n)
		{
			if (n.IsNull)
			{
				return SqlDecimal.Null;
			}
			n.SetPositive();
			return n;
		}

		public static SqlDecimal Ceiling(SqlDecimal n)
		{
			if (n.IsNull)
			{
				return SqlDecimal.Null;
			}
			if (n._bScale == 0)
			{
				return n;
			}
			bool flag;
			n.MakeInteger(out flag);
			if (flag && n.IsPositive)
			{
				n.AddULong(1U);
			}
			if (n.FZero())
			{
				n.SetPositive();
			}
			return n;
		}

		public static SqlDecimal Floor(SqlDecimal n)
		{
			if (n.IsNull)
			{
				return SqlDecimal.Null;
			}
			if (n._bScale == 0)
			{
				return n;
			}
			bool flag;
			n.MakeInteger(out flag);
			if (flag && !n.IsPositive)
			{
				n.AddULong(1U);
			}
			if (n.FZero())
			{
				n.SetPositive();
			}
			return n;
		}

		public static SqlInt32 Sign(SqlDecimal n)
		{
			if (n.IsNull)
			{
				return SqlInt32.Null;
			}
			if (n == new SqlDecimal(0))
			{
				return SqlInt32.Zero;
			}
			if (n.IsNull)
			{
				return SqlInt32.Null;
			}
			if (!n.IsPositive)
			{
				return new SqlInt32(-1);
			}
			return new SqlInt32(1);
		}

		private static SqlDecimal Round(SqlDecimal n, int lPosition, bool fTruncate)
		{
			if (n.IsNull)
			{
				return SqlDecimal.Null;
			}
			if (lPosition >= 0)
			{
				lPosition = Math.Min((int)SqlDecimal.s_NUMERIC_MAX_PRECISION, lPosition);
				if (lPosition >= (int)n._bScale)
				{
					return n;
				}
			}
			else
			{
				lPosition = Math.Max((int)(-(int)SqlDecimal.s_NUMERIC_MAX_PRECISION), lPosition);
				if (lPosition < (int)(n._bScale - n._bPrec))
				{
					n.SetToZero();
					return n;
				}
			}
			uint num = 0U;
			int i = Math.Abs(lPosition - (int)n._bScale);
			uint num2 = 1U;
			while (i > 0)
			{
				if (i >= 9)
				{
					num = n.DivByULong(SqlDecimal.s_rgulShiftBase[8]);
					num2 = SqlDecimal.s_rgulShiftBase[8];
					i -= 9;
				}
				else
				{
					num = n.DivByULong(SqlDecimal.s_rgulShiftBase[i - 1]);
					num2 = SqlDecimal.s_rgulShiftBase[i - 1];
					i = 0;
				}
			}
			if (num2 > 1U)
			{
				num /= num2 / 10U;
			}
			if (n.FZero() && (fTruncate || num < 5U))
			{
				n.SetPositive();
				return n;
			}
			if (num >= 5U && !fTruncate)
			{
				n.AddULong(1U);
			}
			i = Math.Abs(lPosition - (int)n._bScale);
			while (i-- > 0)
			{
				n.MultByULong(SqlDecimal.s_ulBase10);
			}
			return n;
		}

		public static SqlDecimal Round(SqlDecimal n, int position)
		{
			return SqlDecimal.Round(n, position, false);
		}

		public static SqlDecimal Truncate(SqlDecimal n, int position)
		{
			return SqlDecimal.Round(n, position, true);
		}

		public static SqlDecimal Power(SqlDecimal n, double exp)
		{
			if (n.IsNull)
			{
				return SqlDecimal.Null;
			}
			byte precision = n.Precision;
			int scale = (int)n.Scale;
			double num = n.ToDouble();
			n = new SqlDecimal(Math.Pow(num, exp));
			n.AdjustScale(scale - (int)n.Scale, true);
			n._bPrec = SqlDecimal.MaxPrecision;
			return n;
		}

		public int CompareTo(object value)
		{
			if (value is SqlDecimal)
			{
				SqlDecimal sqlDecimal = (SqlDecimal)value;
				return this.CompareTo(sqlDecimal);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlDecimal));
		}

		public int CompareTo(SqlDecimal value)
		{
			if (this.IsNull)
			{
				if (!value.IsNull)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (value.IsNull)
				{
					return 1;
				}
				if (this < value)
				{
					return -1;
				}
				if (this > value)
				{
					return 1;
				}
				return 0;
			}
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlDecimal))
			{
				return false;
			}
			SqlDecimal sqlDecimal = (SqlDecimal)value;
			if (sqlDecimal.IsNull || this.IsNull)
			{
				return sqlDecimal.IsNull && this.IsNull;
			}
			return (this == sqlDecimal).Value;
		}

		public override int GetHashCode()
		{
			if (this.IsNull)
			{
				return 0;
			}
			SqlDecimal sqlDecimal = this;
			int num = (int)sqlDecimal.CalculatePrecision();
			sqlDecimal.AdjustScale((int)SqlDecimal.s_NUMERIC_MAX_PRECISION - num, true);
			int bLen = (int)sqlDecimal._bLen;
			int num2 = 0;
			int[] data = sqlDecimal.Data;
			for (int i = 0; i < bLen; i++)
			{
				int num3 = (num2 >> 28) & 255;
				num2 <<= 4;
				num2 = num2 ^ data[i] ^ num3;
			}
			return num2;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string attribute = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute != null && XmlConvert.ToBoolean(attribute))
			{
				reader.ReadElementString();
				this._bStatus = SqlDecimal.s_bReverseNullMask & this._bStatus;
				return;
			}
			SqlDecimal sqlDecimal = SqlDecimal.Parse(reader.ReadElementString());
			this._bStatus = sqlDecimal._bStatus;
			this._bLen = sqlDecimal._bLen;
			this._bPrec = sqlDecimal._bPrec;
			this._bScale = sqlDecimal._bScale;
			this._data1 = sqlDecimal._data1;
			this._data2 = sqlDecimal._data2;
			this._data3 = sqlDecimal._data3;
			this._data4 = sqlDecimal._data4;
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			writer.WriteString(this.ToString());
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");
		}

		internal byte _bStatus;

		internal byte _bLen;

		internal byte _bPrec;

		internal byte _bScale;

		internal uint _data1;

		internal uint _data2;

		internal uint _data3;

		internal uint _data4;

		private static readonly byte s_NUMERIC_MAX_PRECISION = 38;

		public static readonly byte MaxPrecision = SqlDecimal.s_NUMERIC_MAX_PRECISION;

		public static readonly byte MaxScale = SqlDecimal.s_NUMERIC_MAX_PRECISION;

		private static readonly byte s_bNullMask = 1;

		private static readonly byte s_bIsNull = 0;

		private static readonly byte s_bNotNull = 1;

		private static readonly byte s_bReverseNullMask = ~SqlDecimal.s_bNullMask;

		private static readonly byte s_bSignMask = 2;

		private static readonly byte s_bPositive = 0;

		private static readonly byte s_bNegative = 2;

		private static readonly byte s_bReverseSignMask = ~SqlDecimal.s_bSignMask;

		private static readonly uint s_uiZero = 0U;

		private static readonly int s_cNumeMax = 4;

		private static readonly long s_lInt32Base = 4294967296L;

		private static readonly ulong s_ulInt32Base = 4294967296UL;

		private static readonly ulong s_ulInt32BaseForMod = SqlDecimal.s_ulInt32Base - 1UL;

		internal static readonly ulong s_llMax = 9223372036854775807UL;

		private static readonly uint s_ulBase10 = 10U;

		private static readonly double s_DUINT_BASE = (double)SqlDecimal.s_lInt32Base;

		private static readonly double s_DUINT_BASE2 = SqlDecimal.s_DUINT_BASE * SqlDecimal.s_DUINT_BASE;

		private static readonly double s_DUINT_BASE3 = SqlDecimal.s_DUINT_BASE2 * SqlDecimal.s_DUINT_BASE;

		private static readonly double s_DMAX_NUME = 1E+38;

		private static readonly uint s_DBL_DIG = 17U;

		private static readonly byte s_cNumeDivScaleMin = 6;

		private static readonly uint[] s_rgulShiftBase = new uint[] { 10U, 100U, 1000U, 10000U, 100000U, 1000000U, 10000000U, 100000000U, 1000000000U };

		private static readonly uint[] s_decimalHelpersLo = new uint[]
		{
			10U, 100U, 1000U, 10000U, 100000U, 1000000U, 10000000U, 100000000U, 1000000000U, 1410065408U,
			1215752192U, 3567587328U, 1316134912U, 276447232U, 2764472320U, 1874919424U, 1569325056U, 2808348672U, 2313682944U, 1661992960U,
			3735027712U, 2990538752U, 4135583744U, 2701131776U, 1241513984U, 3825205248U, 3892314112U, 268435456U, 2684354560U, 1073741824U,
			2147483648U, 0U, 0U, 0U, 0U, 0U, 0U, 0U
		};

		private static readonly uint[] s_decimalHelpersMid = new uint[]
		{
			0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 2U,
			23U, 232U, 2328U, 23283U, 232830U, 2328306U, 23283064U, 232830643U, 2328306436U, 1808227885U,
			902409669U, 434162106U, 46653770U, 466537709U, 370409800U, 3704098002U, 2681241660U, 1042612833U, 1836193738U, 1182068202U,
			3230747430U, 2242703233U, 952195850U, 932023908U, 730304488U, 3008077584U, 16004768U, 160047680U
		};

		private static readonly uint[] s_decimalHelpersHi = new uint[]
		{
			0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U,
			0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 5U,
			54U, 542U, 5421U, 54210U, 542101U, 5421010U, 54210108U, 542101086U, 1126043566U, 2670501072U,
			935206946U, 762134875U, 3326381459U, 3199043520U, 1925664130U, 2076772117U, 3587851993U, 1518781562U
		};

		private static readonly uint[] s_decimalHelpersHiHi = new uint[]
		{
			0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U,
			0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U,
			0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 1U, 12U,
			126U, 1262U, 12621U, 126217U, 1262177U, 12621774U, 126217744U, 1262177448U
		};

		private const int HelperTableStartIndexLo = 5;

		private const int HelperTableStartIndexMid = 15;

		private const int HelperTableStartIndexHi = 24;

		private const int HelperTableStartIndexHiHi = 33;

		private static readonly byte[] s_rgCLenFromPrec = new byte[]
		{
			1, 1, 1, 1, 1, 1, 1, 1, 1, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 4, 4,
			4, 4, 4, 4, 4, 4, 4, 4
		};

		private static readonly uint s_ulT1 = 10U;

		private static readonly uint s_ulT2 = 100U;

		private static readonly uint s_ulT3 = 1000U;

		private static readonly uint s_ulT4 = 10000U;

		private static readonly uint s_ulT5 = 100000U;

		private static readonly uint s_ulT6 = 1000000U;

		private static readonly uint s_ulT7 = 10000000U;

		private static readonly uint s_ulT8 = 100000000U;

		private static readonly uint s_ulT9 = 1000000000U;

		private static readonly ulong s_dwlT10 = 10000000000UL;

		private static readonly ulong s_dwlT11 = 100000000000UL;

		private static readonly ulong s_dwlT12 = 1000000000000UL;

		private static readonly ulong s_dwlT13 = 10000000000000UL;

		private static readonly ulong s_dwlT14 = 100000000000000UL;

		private static readonly ulong s_dwlT15 = 1000000000000000UL;

		private static readonly ulong s_dwlT16 = 10000000000000000UL;

		private static readonly ulong s_dwlT17 = 100000000000000000UL;

		private static readonly ulong s_dwlT18 = 1000000000000000000UL;

		private static readonly ulong s_dwlT19 = 10000000000000000000UL;

		public static readonly SqlDecimal Null = new SqlDecimal(true);

		public static readonly SqlDecimal MinValue = SqlDecimal.Parse("-99999999999999999999999999999999999999");

		public static readonly SqlDecimal MaxValue = SqlDecimal.Parse("99999999999999999999999999999999999999");
	}
}
