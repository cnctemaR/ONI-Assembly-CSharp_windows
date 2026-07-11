using System;

namespace System.Runtime.Serialization
{
	internal class BitFlagsGenerator
	{
		public BitFlagsGenerator(int bitCount)
		{
			this.bitCount = bitCount;
			int num = (bitCount + 7) / 8;
			this.locals = new byte[num];
		}

		public void Store(int bitIndex, bool value)
		{
			if (value)
			{
				byte[] array = this.locals;
				int byteIndex = BitFlagsGenerator.GetByteIndex(bitIndex);
				array[byteIndex] |= BitFlagsGenerator.GetBitValue(bitIndex);
				return;
			}
			byte[] array2 = this.locals;
			int byteIndex2 = BitFlagsGenerator.GetByteIndex(bitIndex);
			array2[byteIndex2] &= ~BitFlagsGenerator.GetBitValue(bitIndex);
		}

		public bool Load(int bitIndex)
		{
			byte b = this.locals[BitFlagsGenerator.GetByteIndex(bitIndex)];
			byte bitValue = BitFlagsGenerator.GetBitValue(bitIndex);
			return (b & bitValue) == bitValue;
		}

		public byte[] LoadArray()
		{
			return (byte[])this.locals.Clone();
		}

		public int GetLocalCount()
		{
			return this.locals.Length;
		}

		public int GetBitCount()
		{
			return this.bitCount;
		}

		public byte GetLocal(int i)
		{
			return this.locals[i];
		}

		public static bool IsBitSet(byte[] bytes, int bitIndex)
		{
			int byteIndex = BitFlagsGenerator.GetByteIndex(bitIndex);
			byte bitValue = BitFlagsGenerator.GetBitValue(bitIndex);
			return (bytes[byteIndex] & bitValue) == bitValue;
		}

		public static void SetBit(byte[] bytes, int bitIndex)
		{
			int byteIndex = BitFlagsGenerator.GetByteIndex(bitIndex);
			byte bitValue = BitFlagsGenerator.GetBitValue(bitIndex);
			int num = byteIndex;
			bytes[num] |= bitValue;
		}

		private static int GetByteIndex(int bitIndex)
		{
			return bitIndex >> 3;
		}

		private static byte GetBitValue(int bitIndex)
		{
			return (byte)(1 << (bitIndex & 7));
		}

		private int bitCount;

		private byte[] locals;
	}
}
