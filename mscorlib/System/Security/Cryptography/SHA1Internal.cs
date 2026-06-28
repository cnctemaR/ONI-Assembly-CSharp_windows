using System;

namespace System.Security.Cryptography
{
	internal class SHA1Internal
	{
		public SHA1Internal()
		{
			this._H = new uint[5];
			this._ProcessingBuffer = new byte[64];
			this.buff = new uint[80];
			this.Initialize();
		}

		public void HashCore(byte[] rgb, int ibStart, int cbSize)
		{
			if (this._ProcessingBufferCount != 0)
			{
				if (cbSize < 64 - this._ProcessingBufferCount)
				{
					Buffer.BlockCopy(rgb, ibStart, this._ProcessingBuffer, this._ProcessingBufferCount, cbSize);
					this._ProcessingBufferCount += cbSize;
					return;
				}
				int i = 64 - this._ProcessingBufferCount;
				Buffer.BlockCopy(rgb, ibStart, this._ProcessingBuffer, this._ProcessingBufferCount, i);
				this.ProcessBlock(this._ProcessingBuffer, 0U);
				this._ProcessingBufferCount = 0;
				ibStart += i;
				cbSize -= i;
			}
			for (int i = 0; i < cbSize - cbSize % 64; i += 64)
			{
				this.ProcessBlock(rgb, (uint)(ibStart + i));
			}
			if (cbSize % 64 != 0)
			{
				Buffer.BlockCopy(rgb, cbSize - cbSize % 64 + ibStart, this._ProcessingBuffer, 0, cbSize % 64);
				this._ProcessingBufferCount = cbSize % 64;
			}
		}

		public byte[] HashFinal()
		{
			byte[] array = new byte[20];
			this.ProcessFinalBlock(this._ProcessingBuffer, 0, this._ProcessingBufferCount);
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i * 4 + j] = (byte)(this._H[i] >> 8 * (3 - j));
				}
			}
			return array;
		}

		public void Initialize()
		{
			this.count = 0UL;
			this._ProcessingBufferCount = 0;
			this._H[0] = 1732584193U;
			this._H[1] = 4023233417U;
			this._H[2] = 2562383102U;
			this._H[3] = 271733878U;
			this._H[4] = 3285377520U;
		}

		private void ProcessBlock(byte[] inputBuffer, uint inputOffset)
		{
			this.count += 64UL;
			uint[] h = this._H;
			uint[] array = this.buff;
			SHA1Internal.InitialiseBuff(array, inputBuffer, inputOffset);
			SHA1Internal.FillBuff(array);
			uint num = h[0];
			uint num2 = h[1];
			uint num3 = h[2];
			uint num4 = h[3];
			uint num5 = h[4];
			int i;
			for (i = 0; i < 20; i += 5)
			{
				num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249U + array[i];
				num2 = (num2 << 30) | (num2 >> 2);
				num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249U + array[i + 1];
				num = (num << 30) | (num >> 2);
				num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249U + array[i + 2];
				num5 = (num5 << 30) | (num5 >> 2);
				num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249U + array[i + 3];
				num4 = (num4 << 30) | (num4 >> 2);
				num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249U + array[i + 4];
				num3 = (num3 << 30) | (num3 >> 2);
			}
			while (i < 40)
			{
				num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393U + array[i];
				num2 = (num2 << 30) | (num2 >> 2);
				num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393U + array[i + 1];
				num = (num << 30) | (num >> 2);
				num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393U + array[i + 2];
				num5 = (num5 << 30) | (num5 >> 2);
				num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393U + array[i + 3];
				num4 = (num4 << 30) | (num4 >> 2);
				num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393U + array[i + 4];
				num3 = (num3 << 30) | (num3 >> 2);
				i += 5;
			}
			while (i < 60)
			{
				num5 += ((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4)) + 2400959708U + array[i];
				num2 = (num2 << 30) | (num2 >> 2);
				num4 += ((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3)) + 2400959708U + array[i + 1];
				num = (num << 30) | (num >> 2);
				num3 += ((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2)) + 2400959708U + array[i + 2];
				num5 = (num5 << 30) | (num5 >> 2);
				num2 += ((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num)) + 2400959708U + array[i + 3];
				num4 = (num4 << 30) | (num4 >> 2);
				num += ((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5)) + 2400959708U + array[i + 4];
				num3 = (num3 << 30) | (num3 >> 2);
				i += 5;
			}
			while (i < 80)
			{
				num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 3395469782U + array[i];
				num2 = (num2 << 30) | (num2 >> 2);
				num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 3395469782U + array[i + 1];
				num = (num << 30) | (num >> 2);
				num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 3395469782U + array[i + 2];
				num5 = (num5 << 30) | (num5 >> 2);
				num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 3395469782U + array[i + 3];
				num4 = (num4 << 30) | (num4 >> 2);
				num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 3395469782U + array[i + 4];
				num3 = (num3 << 30) | (num3 >> 2);
				i += 5;
			}
			h[0] += num;
			h[1] += num2;
			h[2] += num3;
			h[3] += num4;
			h[4] += num5;
		}

		private static void InitialiseBuff(uint[] buff, byte[] input, uint inputOffset)
		{
			buff[0] = (uint)(((int)input[(int)((UIntPtr)inputOffset)] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 1U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 2U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 3U))]);
			buff[1] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 4U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 5U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 6U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 7U))]);
			buff[2] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 8U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 9U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 10U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 11U))]);
			buff[3] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 12U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 13U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 14U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 15U))]);
			buff[4] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 16U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 17U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 18U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 19U))]);
			buff[5] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 20U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 21U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 22U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 23U))]);
			buff[6] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 24U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 25U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 26U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 27U))]);
			buff[7] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 28U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 29U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 30U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 31U))]);
			buff[8] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 32U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 33U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 34U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 35U))]);
			buff[9] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 36U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 37U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 38U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 39U))]);
			buff[10] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 40U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 41U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 42U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 43U))]);
			buff[11] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 44U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 45U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 46U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 47U))]);
			buff[12] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 48U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 49U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 50U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 51U))]);
			buff[13] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 52U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 53U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 54U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 55U))]);
			buff[14] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 56U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 57U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 58U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 59U))]);
			buff[15] = (uint)(((int)input[(int)((UIntPtr)(inputOffset + 60U))] << 24) | ((int)input[(int)((UIntPtr)(inputOffset + 61U))] << 16) | ((int)input[(int)((UIntPtr)(inputOffset + 62U))] << 8) | (int)input[(int)((UIntPtr)(inputOffset + 63U))]);
		}

		private static void FillBuff(uint[] buff)
		{
			for (int i = 16; i < 80; i += 8)
			{
				uint num = buff[i - 3] ^ buff[i - 8] ^ buff[i - 14] ^ buff[i - 16];
				buff[i] = (num << 1) | (num >> 31);
				num = buff[i - 2] ^ buff[i - 7] ^ buff[i - 13] ^ buff[i - 15];
				buff[i + 1] = (num << 1) | (num >> 31);
				num = buff[i - 1] ^ buff[i - 6] ^ buff[i - 12] ^ buff[i - 14];
				buff[i + 2] = (num << 1) | (num >> 31);
				num = buff[i] ^ buff[i - 5] ^ buff[i - 11] ^ buff[i - 13];
				buff[i + 3] = (num << 1) | (num >> 31);
				num = buff[i + 1] ^ buff[i - 4] ^ buff[i - 10] ^ buff[i - 12];
				buff[i + 4] = (num << 1) | (num >> 31);
				num = buff[i + 2] ^ buff[i - 3] ^ buff[i - 9] ^ buff[i - 11];
				buff[i + 5] = (num << 1) | (num >> 31);
				num = buff[i + 3] ^ buff[i - 2] ^ buff[i - 8] ^ buff[i - 10];
				buff[i + 6] = (num << 1) | (num >> 31);
				num = buff[i + 4] ^ buff[i - 1] ^ buff[i - 7] ^ buff[i - 9];
				buff[i + 7] = (num << 1) | (num >> 31);
			}
		}

		private void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			ulong num = this.count + (ulong)((long)inputCount);
			int num2 = 56 - (int)(num % 64UL);
			if (num2 < 1)
			{
				num2 += 64;
			}
			int num3 = inputCount + num2 + 8;
			byte[] array = ((num3 != 64) ? new byte[num3] : this._ProcessingBuffer);
			for (int i = 0; i < inputCount; i++)
			{
				array[i] = inputBuffer[i + inputOffset];
			}
			array[inputCount] = 128;
			for (int j = inputCount + 1; j < inputCount + num2; j++)
			{
				array[j] = 0;
			}
			ulong num4 = num << 3;
			this.AddLength(num4, array, inputCount + num2);
			this.ProcessBlock(array, 0U);
			if (num3 == 128)
			{
				this.ProcessBlock(array, 64U);
			}
		}

		internal void AddLength(ulong length, byte[] buffer, int position)
		{
			buffer[position++] = (byte)(length >> 56);
			buffer[position++] = (byte)(length >> 48);
			buffer[position++] = (byte)(length >> 40);
			buffer[position++] = (byte)(length >> 32);
			buffer[position++] = (byte)(length >> 24);
			buffer[position++] = (byte)(length >> 16);
			buffer[position++] = (byte)(length >> 8);
			buffer[position] = (byte)length;
		}

		private const int BLOCK_SIZE_BYTES = 64;

		private const int HASH_SIZE_BYTES = 20;

		private uint[] _H;

		private ulong count;

		private byte[] _ProcessingBuffer;

		private int _ProcessingBufferCount;

		private uint[] buff;
	}
}
