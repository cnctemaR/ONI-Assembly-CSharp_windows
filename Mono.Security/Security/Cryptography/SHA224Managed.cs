using System;

namespace Mono.Security.Cryptography
{
	public class SHA224Managed : SHA224
	{
		public SHA224Managed()
		{
			this._H = new uint[8];
			this._ProcessingBuffer = new byte[64];
			this.buff = new uint[64];
			this.Initialize();
		}

		private uint Ch(uint u, uint v, uint w)
		{
			return (u & v) ^ (~u & w);
		}

		private uint Maj(uint u, uint v, uint w)
		{
			return (u & v) ^ (u & w) ^ (v & w);
		}

		private uint Ro0(uint x)
		{
			return ((x >> 7) | (x << 25)) ^ ((x >> 18) | (x << 14)) ^ (x >> 3);
		}

		private uint Ro1(uint x)
		{
			return ((x >> 17) | (x << 15)) ^ ((x >> 19) | (x << 13)) ^ (x >> 10);
		}

		private uint Sig0(uint x)
		{
			return ((x >> 2) | (x << 30)) ^ ((x >> 13) | (x << 19)) ^ ((x >> 22) | (x << 10));
		}

		private uint Sig1(uint x)
		{
			return ((x >> 6) | (x << 26)) ^ ((x >> 11) | (x << 21)) ^ ((x >> 25) | (x << 7));
		}

		protected override void HashCore(byte[] rgb, int start, int size)
		{
			this.State = 1;
			if (this._ProcessingBufferCount != 0)
			{
				if (size < 64 - this._ProcessingBufferCount)
				{
					Buffer.BlockCopy(rgb, start, this._ProcessingBuffer, this._ProcessingBufferCount, size);
					this._ProcessingBufferCount += size;
					return;
				}
				int i = 64 - this._ProcessingBufferCount;
				Buffer.BlockCopy(rgb, start, this._ProcessingBuffer, this._ProcessingBufferCount, i);
				this.ProcessBlock(this._ProcessingBuffer, 0);
				this._ProcessingBufferCount = 0;
				start += i;
				size -= i;
			}
			for (int i = 0; i < size - size % 64; i += 64)
			{
				this.ProcessBlock(rgb, start + i);
			}
			if (size % 64 != 0)
			{
				Buffer.BlockCopy(rgb, size - size % 64 + start, this._ProcessingBuffer, 0, size % 64);
				this._ProcessingBufferCount = size % 64;
			}
		}

		protected override byte[] HashFinal()
		{
			byte[] array = new byte[28];
			this.ProcessFinalBlock(this._ProcessingBuffer, 0, this._ProcessingBufferCount);
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i * 4 + j] = (byte)(this._H[i] >> 24 - j * 8);
				}
			}
			this.State = 0;
			return array;
		}

		public override void Initialize()
		{
			this.count = 0UL;
			this._ProcessingBufferCount = 0;
			this._H[0] = 3238371032U;
			this._H[1] = 914150663U;
			this._H[2] = 812702999U;
			this._H[3] = 4144912697U;
			this._H[4] = 4290775857U;
			this._H[5] = 1750603025U;
			this._H[6] = 1694076839U;
			this._H[7] = 3204075428U;
		}

		private void ProcessBlock(byte[] inputBuffer, int inputOffset)
		{
			uint[] k = SHAConstants.K1;
			uint[] array = this.buff;
			this.count += 64UL;
			for (int i = 0; i < 16; i++)
			{
				array[i] = (uint)(((int)inputBuffer[inputOffset + 4 * i] << 24) | ((int)inputBuffer[inputOffset + 4 * i + 1] << 16) | ((int)inputBuffer[inputOffset + 4 * i + 2] << 8) | (int)inputBuffer[inputOffset + 4 * i + 3]);
			}
			for (int i = 16; i < 64; i++)
			{
				uint num = array[i - 15];
				num = ((num >> 7) | (num << 25)) ^ ((num >> 18) | (num << 14)) ^ (num >> 3);
				uint num2 = array[i - 2];
				num2 = ((num2 >> 17) | (num2 << 15)) ^ ((num2 >> 19) | (num2 << 13)) ^ (num2 >> 10);
				array[i] = num2 + array[i - 7] + num + array[i - 16];
			}
			uint num3 = this._H[0];
			uint num4 = this._H[1];
			uint num5 = this._H[2];
			uint num6 = this._H[3];
			uint num7 = this._H[4];
			uint num8 = this._H[5];
			uint num9 = this._H[6];
			uint num10 = this._H[7];
			for (int i = 0; i < 64; i++)
			{
				uint num = num10 + (((num7 >> 6) | (num7 << 26)) ^ ((num7 >> 11) | (num7 << 21)) ^ ((num7 >> 25) | (num7 << 7))) + ((num7 & num8) ^ (~num7 & num9)) + k[i] + array[i];
				uint num2 = ((num3 >> 2) | (num3 << 30)) ^ ((num3 >> 13) | (num3 << 19)) ^ ((num3 >> 22) | (num3 << 10));
				num2 += (num3 & num4) ^ (num3 & num5) ^ (num4 & num5);
				num10 = num9;
				num9 = num8;
				num8 = num7;
				num7 = num6 + num;
				num6 = num5;
				num5 = num4;
				num4 = num3;
				num3 = num + num2;
			}
			this._H[0] += num3;
			this._H[1] += num4;
			this._H[2] += num5;
			this._H[3] += num6;
			this._H[4] += num7;
			this._H[5] += num8;
			this._H[6] += num9;
			this._H[7] += num10;
		}

		private void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			ulong num = this.count + (ulong)((long)inputCount);
			int num2 = 56 - (int)(num % 64UL);
			if (num2 < 1)
			{
				num2 += 64;
			}
			byte[] array = new byte[inputCount + num2 + 8];
			for (int i = 0; i < inputCount; i++)
			{
				array[i] = inputBuffer[i + inputOffset];
			}
			array[inputCount] = 128;
			for (int j = inputCount + 1; j < inputCount + num2; j++)
			{
				array[j] = 0;
			}
			ulong num3 = num << 3;
			this.AddLength(num3, array, inputCount + num2);
			this.ProcessBlock(array, 0);
			if (inputCount + num2 + 8 == 128)
			{
				this.ProcessBlock(array, 64);
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

		private uint[] _H;

		private ulong count;

		private byte[] _ProcessingBuffer;

		private int _ProcessingBufferCount;

		private uint[] buff;
	}
}
