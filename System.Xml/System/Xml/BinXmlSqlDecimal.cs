using System;
using System.Diagnostics;
using System.IO;

namespace System.Xml
{
	internal struct BinXmlSqlDecimal
	{
		public bool IsPositive
		{
			get
			{
				return this.m_bSign == 0;
			}
		}

		public BinXmlSqlDecimal(byte[] data, int offset, bool trim)
		{
			byte b = data[offset];
			if (b <= 11)
			{
				if (b == 7)
				{
					this.m_bLen = 1;
					goto IL_0050;
				}
				if (b == 11)
				{
					this.m_bLen = 2;
					goto IL_0050;
				}
			}
			else
			{
				if (b == 15)
				{
					this.m_bLen = 3;
					goto IL_0050;
				}
				if (b == 19)
				{
					this.m_bLen = 4;
					goto IL_0050;
				}
			}
			throw new XmlException("Unable to parse data as SQL_DECIMAL.", null);
			IL_0050:
			this.m_bPrec = data[offset + 1];
			this.m_bScale = data[offset + 2];
			this.m_bSign = ((data[offset + 3] == 0) ? 1 : 0);
			this.m_data1 = BinXmlSqlDecimal.UIntFromByteArray(data, offset + 4);
			this.m_data2 = ((this.m_bLen > 1) ? BinXmlSqlDecimal.UIntFromByteArray(data, offset + 8) : 0U);
			this.m_data3 = ((this.m_bLen > 2) ? BinXmlSqlDecimal.UIntFromByteArray(data, offset + 12) : 0U);
			this.m_data4 = ((this.m_bLen > 3) ? BinXmlSqlDecimal.UIntFromByteArray(data, offset + 16) : 0U);
			if (this.m_bLen == 4 && this.m_data4 == 0U)
			{
				this.m_bLen = 3;
			}
			if (this.m_bLen == 3 && this.m_data3 == 0U)
			{
				this.m_bLen = 2;
			}
			if (this.m_bLen == 2 && this.m_data2 == 0U)
			{
				this.m_bLen = 1;
			}
			if (trim)
			{
				this.TrimTrailingZeros();
			}
		}

		public void Write(Stream strm)
		{
			strm.WriteByte(this.m_bLen * 4 + 3);
			strm.WriteByte(this.m_bPrec);
			strm.WriteByte(this.m_bScale);
			strm.WriteByte((this.m_bSign == 0) ? 1 : 0);
			this.WriteUI4(this.m_data1, strm);
			if (this.m_bLen > 1)
			{
				this.WriteUI4(this.m_data2, strm);
				if (this.m_bLen > 2)
				{
					this.WriteUI4(this.m_data3, strm);
					if (this.m_bLen > 3)
					{
						this.WriteUI4(this.m_data4, strm);
					}
				}
			}
		}

		private void WriteUI4(uint val, Stream strm)
		{
			strm.WriteByte((byte)(val & 255U));
			strm.WriteByte((byte)((val >> 8) & 255U));
			strm.WriteByte((byte)((val >> 16) & 255U));
			strm.WriteByte((byte)((val >> 24) & 255U));
		}

		private static uint UIntFromByteArray(byte[] data, int offset)
		{
			return (uint)((int)data[offset] | ((int)data[offset + 1] << 8) | ((int)data[offset + 2] << 16) | ((int)data[offset + 3] << 24));
		}

		private bool FZero()
		{
			return this.m_data1 == 0U && this.m_bLen <= 1;
		}

		private void StoreFromWorkingArray(uint[] rguiData)
		{
			this.m_data1 = rguiData[0];
			this.m_data2 = rguiData[1];
			this.m_data3 = rguiData[2];
			this.m_data4 = rguiData[3];
		}

		private bool FGt10_38(uint[] rglData)
		{
			return (ulong)rglData[3] >= 1262177448UL && ((ulong)rglData[3] > 1262177448UL || (ulong)rglData[2] > 1518781562UL || ((ulong)rglData[2] == 1518781562UL && (ulong)rglData[1] >= 160047680UL));
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
			BinXmlSqlDecimal.MpNormalize(rgulU, ref ciulU);
		}

		private static void MpNormalize(uint[] rgulU, ref int ciulU)
		{
			while (ciulU > 1 && rgulU[ciulU - 1] == 0U)
			{
				ciulU--;
			}
		}

		internal void AdjustScale(int digits, bool fRound)
		{
			bool flag = false;
			int i = digits;
			if (i + (int)this.m_bScale < 0)
			{
				throw new XmlException("Numeric arithmetic causes truncation.", null);
			}
			if (i + (int)this.m_bScale > (int)BinXmlSqlDecimal.NUMERIC_MAX_PRECISION)
			{
				throw new XmlException("Arithmetic Overflow.", null);
			}
			byte b = (byte)(i + (int)this.m_bScale);
			byte b2 = (byte)Math.Min((int)BinXmlSqlDecimal.NUMERIC_MAX_PRECISION, Math.Max(1, i + (int)this.m_bPrec));
			if (i > 0)
			{
				this.m_bScale = b;
				this.m_bPrec = b2;
				while (i > 0)
				{
					uint num;
					if (i >= 9)
					{
						num = BinXmlSqlDecimal.x_rgulShiftBase[8];
						i -= 9;
					}
					else
					{
						num = BinXmlSqlDecimal.x_rgulShiftBase[i - 1];
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
						num = BinXmlSqlDecimal.x_rgulShiftBase[8];
						i += 9;
					}
					else
					{
						num = BinXmlSqlDecimal.x_rgulShiftBase[-i - 1];
						i = 0;
					}
					num2 = this.DivByULong(num);
				}
				while (i < 0);
				flag = num2 >= num / 2U;
				this.m_bScale = b;
				this.m_bPrec = b2;
			}
			if (flag && fRound)
			{
				this.AddULong(1U);
				return;
			}
			if (this.FZero())
			{
				this.m_bSign = 0;
			}
		}

		private void AddULong(uint ulAdd)
		{
			ulong num = (ulong)ulAdd;
			int bLen = (int)this.m_bLen;
			uint[] array = new uint[] { this.m_data1, this.m_data2, this.m_data3, this.m_data4 };
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
			if (num2 == BinXmlSqlDecimal.x_cNumeMax)
			{
				throw new XmlException("Arithmetic Overflow.", null);
			}
			array[num2] = (uint)num;
			this.m_bLen += 1;
			if (this.FGt10_38(array))
			{
				throw new XmlException("Arithmetic Overflow.", null);
			}
			this.StoreFromWorkingArray(array);
		}

		private void MultByULong(uint uiMultiplier)
		{
			int bLen = (int)this.m_bLen;
			ulong num = 0UL;
			uint[] array = new uint[] { this.m_data1, this.m_data2, this.m_data3, this.m_data4 };
			for (int i = 0; i < bLen; i++)
			{
				ulong num2 = (ulong)array[i] * (ulong)uiMultiplier;
				num += num2;
				if (num < num2)
				{
					num2 = BinXmlSqlDecimal.x_ulInt32Base;
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
				if (bLen == BinXmlSqlDecimal.x_cNumeMax)
				{
					throw new XmlException("Arithmetic Overflow.", null);
				}
				array[bLen] = (uint)num;
				this.m_bLen += 1;
			}
			if (this.FGt10_38(array))
			{
				throw new XmlException("Arithmetic Overflow.", null);
			}
			this.StoreFromWorkingArray(array);
		}

		internal uint DivByULong(uint iDivisor)
		{
			ulong num = (ulong)iDivisor;
			ulong num2 = 0UL;
			bool flag = true;
			if (num == 0UL)
			{
				throw new XmlException("Divide by zero error encountered.", null);
			}
			uint[] array = new uint[] { this.m_data1, this.m_data2, this.m_data3, this.m_data4 };
			for (int i = (int)this.m_bLen; i > 0; i--)
			{
				num2 = (num2 << 32) + (ulong)array[i - 1];
				uint num3 = (uint)(num2 / num);
				array[i - 1] = num3;
				num2 %= num;
				flag = flag && num3 == 0U;
				if (flag)
				{
					this.m_bLen -= 1;
				}
			}
			this.StoreFromWorkingArray(array);
			if (flag)
			{
				this.m_bLen = 1;
			}
			return (uint)num2;
		}

		private static byte CLenFromPrec(byte bPrec)
		{
			return BinXmlSqlDecimal.rgCLenFromPrec[(int)(bPrec - 1)];
		}

		private static char ChFromDigit(uint uiDigit)
		{
			return (char)(uiDigit + 48U);
		}

		public decimal ToDecimal()
		{
			if (this.m_data4 != 0U || this.m_bScale > 28)
			{
				throw new XmlException("Arithmetic Overflow.", null);
			}
			return new decimal((int)this.m_data1, (int)this.m_data2, (int)this.m_data3, !this.IsPositive, this.m_bScale);
		}

		private void TrimTrailingZeros()
		{
			uint[] array = new uint[] { this.m_data1, this.m_data2, this.m_data3, this.m_data4 };
			int bLen = (int)this.m_bLen;
			if (bLen == 1 && array[0] == 0U)
			{
				this.m_bScale = 0;
				return;
			}
			while (this.m_bScale > 0 && (bLen > 1 || array[0] != 0U))
			{
				uint num;
				BinXmlSqlDecimal.MpDiv1(array, ref bLen, 10U, out num);
				if (num != 0U)
				{
					break;
				}
				this.m_data1 = array[0];
				this.m_data2 = array[1];
				this.m_data3 = array[2];
				this.m_data4 = array[3];
				this.m_bScale -= 1;
			}
			if (this.m_bLen == 4 && this.m_data4 == 0U)
			{
				this.m_bLen = 3;
			}
			if (this.m_bLen == 3 && this.m_data3 == 0U)
			{
				this.m_bLen = 2;
			}
			if (this.m_bLen == 2 && this.m_data2 == 0U)
			{
				this.m_bLen = 1;
			}
		}

		public override string ToString()
		{
			uint[] array = new uint[] { this.m_data1, this.m_data2, this.m_data3, this.m_data4 };
			int bLen = (int)this.m_bLen;
			char[] array2 = new char[(int)(BinXmlSqlDecimal.NUMERIC_MAX_PRECISION + 1)];
			int i = 0;
			while (bLen > 1 || array[0] != 0U)
			{
				uint num;
				BinXmlSqlDecimal.MpDiv1(array, ref bLen, 10U, out num);
				array2[i++] = BinXmlSqlDecimal.ChFromDigit(num);
			}
			while (i <= (int)this.m_bScale)
			{
				array2[i++] = BinXmlSqlDecimal.ChFromDigit(0U);
			}
			bool isPositive = this.IsPositive;
			int num2 = (isPositive ? i : (i + 1));
			if (this.m_bScale > 0)
			{
				num2++;
			}
			char[] array3 = new char[num2];
			int num3 = 0;
			if (!isPositive)
			{
				array3[num3++] = '-';
			}
			while (i > 0)
			{
				if (i-- == (int)this.m_bScale)
				{
					array3[num3++] = '.';
				}
				array3[num3++] = array2[i];
			}
			return new string(array3);
		}

		[Conditional("DEBUG")]
		private void AssertValid()
		{
			object obj = (new uint[] { this.m_data1, this.m_data2, this.m_data3, this.m_data4 })[(int)(this.m_bLen - 1)];
			for (int i = (int)this.m_bLen; i < BinXmlSqlDecimal.x_cNumeMax; i++)
			{
			}
		}

		internal byte m_bLen;

		internal byte m_bPrec;

		internal byte m_bScale;

		internal byte m_bSign;

		internal uint m_data1;

		internal uint m_data2;

		internal uint m_data3;

		internal uint m_data4;

		private static readonly byte NUMERIC_MAX_PRECISION = 38;

		private static readonly byte MaxPrecision = BinXmlSqlDecimal.NUMERIC_MAX_PRECISION;

		private static readonly byte MaxScale = BinXmlSqlDecimal.NUMERIC_MAX_PRECISION;

		private static readonly int x_cNumeMax = 4;

		private static readonly long x_lInt32Base = 4294967296L;

		private static readonly ulong x_ulInt32Base = 4294967296UL;

		private static readonly ulong x_ulInt32BaseForMod = BinXmlSqlDecimal.x_ulInt32Base - 1UL;

		internal static readonly ulong x_llMax = 9223372036854775807UL;

		private static readonly double DUINT_BASE = (double)BinXmlSqlDecimal.x_lInt32Base;

		private static readonly double DUINT_BASE2 = BinXmlSqlDecimal.DUINT_BASE * BinXmlSqlDecimal.DUINT_BASE;

		private static readonly double DUINT_BASE3 = BinXmlSqlDecimal.DUINT_BASE2 * BinXmlSqlDecimal.DUINT_BASE;

		private static readonly uint[] x_rgulShiftBase = new uint[] { 10U, 100U, 1000U, 10000U, 100000U, 1000000U, 10000000U, 100000000U, 1000000000U };

		private static readonly byte[] rgCLenFromPrec = new byte[]
		{
			1, 1, 1, 1, 1, 1, 1, 1, 1, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 4, 4,
			4, 4, 4, 4, 4, 4, 4, 4
		};
	}
}
