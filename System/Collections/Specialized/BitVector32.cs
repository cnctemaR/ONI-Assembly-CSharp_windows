using System;
using System.Text;

namespace System.Collections.Specialized
{
	public struct BitVector32
	{
		public BitVector32(int data)
		{
			this.data = (uint)data;
		}

		public BitVector32(BitVector32 value)
		{
			this.data = value.data;
		}

		public bool this[int bit]
		{
			get
			{
				return ((ulong)this.data & (ulong)((long)bit)) == (ulong)bit;
			}
			set
			{
				if (value)
				{
					this.data |= (uint)bit;
					return;
				}
				this.data &= (uint)(~(uint)bit);
			}
		}

		public int this[BitVector32.Section section]
		{
			get
			{
				return (int)((this.data & (uint)((uint)section.Mask << (int)section.Offset)) >> (int)section.Offset);
			}
			set
			{
				value <<= (int)section.Offset;
				int num = (65535 & (int)section.Mask) << (int)section.Offset;
				this.data = (this.data & (uint)(~(uint)num)) | (uint)(value & num);
			}
		}

		public int Data
		{
			get
			{
				return (int)this.data;
			}
		}

		private static short CountBitsSet(short mask)
		{
			short num = 0;
			while ((mask & 1) != 0)
			{
				num += 1;
				mask = (short)(mask >> 1);
			}
			return num;
		}

		public static int CreateMask()
		{
			return BitVector32.CreateMask(0);
		}

		public static int CreateMask(int previous)
		{
			if (previous == 0)
			{
				return 1;
			}
			if (previous == -2147483648)
			{
				throw new InvalidOperationException(global::SR.GetString("Bit vector is full."));
			}
			return previous << 1;
		}

		private static short CreateMaskFromHighValue(short highValue)
		{
			short num = 16;
			while (((int)highValue & 32768) == 0)
			{
				num -= 1;
				highValue = (short)(highValue << 1);
			}
			ushort num2 = 0;
			while (num > 0)
			{
				num -= 1;
				num2 = (ushort)(num2 << 1);
				num2 |= 1;
			}
			return (short)num2;
		}

		public static BitVector32.Section CreateSection(short maxValue)
		{
			return BitVector32.CreateSectionHelper(maxValue, 0, 0);
		}

		public static BitVector32.Section CreateSection(short maxValue, BitVector32.Section previous)
		{
			return BitVector32.CreateSectionHelper(maxValue, previous.Mask, previous.Offset);
		}

		private static BitVector32.Section CreateSectionHelper(short maxValue, short priorMask, short priorOffset)
		{
			if (maxValue < 1)
			{
				throw new ArgumentException(global::SR.GetString("Argument {0} should be larger than {1}.", new object[] { "maxValue", 0 }), "maxValue");
			}
			short num = priorOffset + BitVector32.CountBitsSet(priorMask);
			if (num >= 32)
			{
				throw new InvalidOperationException(global::SR.GetString("Bit vector is full."));
			}
			return new BitVector32.Section(BitVector32.CreateMaskFromHighValue(maxValue), num);
		}

		public override bool Equals(object o)
		{
			return o is BitVector32 && this.data == ((BitVector32)o).data;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static string ToString(BitVector32 value)
		{
			StringBuilder stringBuilder = new StringBuilder(45);
			stringBuilder.Append("BitVector32{");
			int num = (int)value.data;
			for (int i = 0; i < 32; i++)
			{
				if (((long)num & (long)((ulong)(-2147483648))) != 0L)
				{
					stringBuilder.Append("1");
				}
				else
				{
					stringBuilder.Append("0");
				}
				num <<= 1;
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return BitVector32.ToString(this);
		}

		private uint data;

		public struct Section
		{
			internal Section(short mask, short offset)
			{
				this.mask = mask;
				this.offset = offset;
			}

			public short Mask
			{
				get
				{
					return this.mask;
				}
			}

			public short Offset
			{
				get
				{
					return this.offset;
				}
			}

			public override bool Equals(object o)
			{
				return o is BitVector32.Section && this.Equals((BitVector32.Section)o);
			}

			public bool Equals(BitVector32.Section obj)
			{
				return obj.mask == this.mask && obj.offset == this.offset;
			}

			public static bool operator ==(BitVector32.Section a, BitVector32.Section b)
			{
				return a.Equals(b);
			}

			public static bool operator !=(BitVector32.Section a, BitVector32.Section b)
			{
				return !(a == b);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public static string ToString(BitVector32.Section value)
			{
				return string.Concat(new string[]
				{
					"Section{0x",
					Convert.ToString(value.Mask, 16),
					", 0x",
					Convert.ToString(value.Offset, 16),
					"}"
				});
			}

			public override string ToString()
			{
				return BitVector32.Section.ToString(this);
			}

			private readonly short mask;

			private readonly short offset;
		}
	}
}
