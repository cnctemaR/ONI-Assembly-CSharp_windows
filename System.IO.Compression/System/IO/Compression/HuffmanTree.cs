using System;

namespace System.IO.Compression
{
	internal sealed class HuffmanTree
	{
		public static HuffmanTree StaticLiteralLengthTree { get; } = new HuffmanTree(HuffmanTree.GetStaticLiteralTreeLength());

		public static HuffmanTree StaticDistanceTree { get; } = new HuffmanTree(HuffmanTree.GetStaticDistanceTreeLength());

		public HuffmanTree(byte[] codeLengths)
		{
			this._codeLengthArray = codeLengths;
			if (this._codeLengthArray.Length == 288)
			{
				this._tableBits = 9;
			}
			else
			{
				this._tableBits = 7;
			}
			this._tableMask = (1 << this._tableBits) - 1;
			this._table = new short[1 << this._tableBits];
			this._left = new short[2 * this._codeLengthArray.Length];
			this._right = new short[2 * this._codeLengthArray.Length];
			this.CreateTable();
		}

		private static byte[] GetStaticLiteralTreeLength()
		{
			byte[] array = new byte[288];
			for (int i = 0; i <= 143; i++)
			{
				array[i] = 8;
			}
			for (int j = 144; j <= 255; j++)
			{
				array[j] = 9;
			}
			for (int k = 256; k <= 279; k++)
			{
				array[k] = 7;
			}
			for (int l = 280; l <= 287; l++)
			{
				array[l] = 8;
			}
			return array;
		}

		private static byte[] GetStaticDistanceTreeLength()
		{
			byte[] array = new byte[32];
			for (int i = 0; i < 32; i++)
			{
				array[i] = 5;
			}
			return array;
		}

		private uint[] CalculateHuffmanCode()
		{
			uint[] array = new uint[17];
			foreach (int num in this._codeLengthArray)
			{
				array[num] += 1U;
			}
			array[0] = 0U;
			uint[] array2 = new uint[17];
			uint num2 = 0U;
			for (int j = 1; j <= 16; j++)
			{
				num2 = num2 + array[j - 1] << 1;
				array2[j] = num2;
			}
			uint[] array3 = new uint[288];
			for (int k = 0; k < this._codeLengthArray.Length; k++)
			{
				int num3 = (int)this._codeLengthArray[k];
				if (num3 > 0)
				{
					array3[k] = FastEncoderStatics.BitReverse(array2[num3], num3);
					array2[num3] += 1U;
				}
			}
			return array3;
		}

		private void CreateTable()
		{
			uint[] array = this.CalculateHuffmanCode();
			short num = (short)this._codeLengthArray.Length;
			for (int i = 0; i < this._codeLengthArray.Length; i++)
			{
				int num2 = (int)this._codeLengthArray[i];
				if (num2 > 0)
				{
					int num3 = (int)array[i];
					if (num2 > this._tableBits)
					{
						int num4 = num2 - this._tableBits;
						int num5 = 1 << this._tableBits;
						int num6 = num3 & ((1 << this._tableBits) - 1);
						short[] array2 = this._table;
						do
						{
							short num7 = array2[num6];
							if (num7 == 0)
							{
								array2[num6] = -num;
								num7 = -num;
								num += 1;
							}
							if (num7 > 0)
							{
								goto Block_6;
							}
							if ((num3 & num5) == 0)
							{
								array2 = this._left;
							}
							else
							{
								array2 = this._right;
							}
							num6 = (int)(-(int)num7);
							num5 <<= 1;
							num4--;
						}
						while (num4 != 0);
						array2[num6] = (short)i;
						goto IL_0119;
						Block_6:
						throw new InvalidDataException("Failed to construct a huffman tree using the length array. The stream might be corrupted.");
					}
					int num8 = 1 << num2;
					if (num3 >= num8)
					{
						throw new InvalidDataException("Failed to construct a huffman tree using the length array. The stream might be corrupted.");
					}
					int num9 = 1 << this._tableBits - num2;
					for (int j = 0; j < num9; j++)
					{
						this._table[num3] = (short)i;
						num3 += num8;
					}
				}
				IL_0119:;
			}
		}

		public int GetNextSymbol(InputBuffer input)
		{
			uint num = input.TryLoad16Bits();
			if (input.AvailableBits == 0)
			{
				return -1;
			}
			int num2;
			checked
			{
				num2 = (int)this._table[(int)((IntPtr)(unchecked((ulong)num & (ulong)((long)this._tableMask))))];
			}
			if (num2 < 0)
			{
				uint num3 = 1U << this._tableBits;
				do
				{
					num2 = -num2;
					if ((num & num3) == 0U)
					{
						num2 = (int)this._left[num2];
					}
					else
					{
						num2 = (int)this._right[num2];
					}
					num3 <<= 1;
				}
				while (num2 < 0);
			}
			int num4 = (int)this._codeLengthArray[num2];
			if (num4 <= 0)
			{
				throw new InvalidDataException("Failed to construct a huffman tree using the length array. The stream might be corrupted.");
			}
			if (num4 > input.AvailableBits)
			{
				return -1;
			}
			input.SkipBits(num4);
			return num2;
		}

		internal const int MaxLiteralTreeElements = 288;

		internal const int MaxDistTreeElements = 32;

		internal const int EndOfBlockCode = 256;

		internal const int NumberOfCodeLengthTreeElements = 19;

		private readonly int _tableBits;

		private readonly short[] _table;

		private readonly short[] _left;

		private readonly short[] _right;

		private readonly byte[] _codeLengthArray;

		private readonly int _tableMask;
	}
}
