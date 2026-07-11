using System;
using System.Text;

namespace System.Collections.Specialized
{
	public struct BitVector32
	{
		public BitVector32(BitVector32 source)
		{
			this.bits = source.bits;
		}

		public BitVector32(int init)
		{
			this.bits = init;
		}

		public int Data
		{
			get
			{
				return this.bits;
			}
		}

		public int this[BitVector32.Section section]
		{
			get
			{
				return (this.bits >> (int)section.Offset) & (int)section.Mask;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Section can't hold negative values");
				}
				if (value > (int)section.Mask)
				{
					throw new ArgumentException("Value too large to fit in section");
				}
				this.bits &= ~((int)section.Mask << (int)section.Offset);
				this.bits |= value << (int)section.Offset;
			}
		}

		public bool this[int mask]
		{
			get
			{
				return (this.bits & mask) == mask;
			}
			set
			{
				if (value)
				{
					this.bits |= mask;
				}
				else
				{
					this.bits &= ~mask;
				}
			}
		}

		public static int CreateMask()
		{
			return 1;
		}

		public static int CreateMask(int prev)
		{
			if (prev == 0)
			{
				return 1;
			}
			if (prev == -2147483648)
			{
				throw new InvalidOperationException("all bits set");
			}
			return prev << 1;
		}

		public static BitVector32.Section CreateSection(short maxValue)
		{
			return BitVector32.CreateSection(maxValue, new BitVector32.Section(0, 0));
		}

		public static BitVector32.Section CreateSection(short maxValue, BitVector32.Section previous)
		{
			if (maxValue < 1)
			{
				throw new ArgumentException("maxValue");
			}
			int num = BitVector32.HighestSetBit((int)maxValue);
			int num2 = (1 << num) - 1;
			int num3 = (int)previous.Offset + BitVector32.HighestSetBit((int)previous.Mask);
			if (num3 + num > 32)
			{
				throw new ArgumentException("Sections cannot exceed 32 bits in total");
			}
			return new BitVector32.Section((short)num2, (short)num3);
		}

		public override bool Equals(object o)
		{
			return o is BitVector32 && this.bits == ((BitVector32)o).bits;
		}

		public override int GetHashCode()
		{
			return this.bits.GetHashCode();
		}

		public override string ToString()
		{
			return BitVector32.ToString(this);
		}

		public static string ToString(BitVector32 value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BitVector32{");
			for (long num = (long)((ulong)int.MinValue); num > 0L; num >>= 1)
			{
				stringBuilder.Append((((long)value.bits & num) != 0L) ? '1' : '0');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		private static int HighestSetBit(int i)
		{
			int num = 0;
			while (i >> num != 0)
			{
				num++;
			}
			return num;
		}

		private int bits;

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

			public bool Equals(BitVector32.Section obj)
			{
				return this.mask == obj.mask && this.offset == obj.offset;
			}

			public override bool Equals(object o)
			{
				if (!(o is BitVector32.Section))
				{
					return false;
				}
				BitVector32.Section section = (BitVector32.Section)o;
				return this.mask == section.mask && this.offset == section.offset;
			}

			public override int GetHashCode()
			{
				return (int)this.mask << (int)this.offset;
			}

			public override string ToString()
			{
				return BitVector32.Section.ToString(this);
			}

			public static string ToString(BitVector32.Section value)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Section{0x");
				stringBuilder.Append(Convert.ToString(value.Mask, 16));
				stringBuilder.Append(", 0x");
				stringBuilder.Append(Convert.ToString(value.Offset, 16));
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}

			public static bool operator ==(BitVector32.Section v1, BitVector32.Section v2)
			{
				return v1.mask == v2.mask && v1.offset == v2.offset;
			}

			public static bool operator !=(BitVector32.Section v1, BitVector32.Section v2)
			{
				return v1.mask != v2.mask || v1.offset != v2.offset;
			}

			private short mask;

			private short offset;
		}
	}
}
