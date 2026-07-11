using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class MD5CryptoServiceProvider : MD5
	{
		public MD5CryptoServiceProvider()
		{
			this._H = new uint[4];
			this.buff = new uint[16];
			this._ProcessingBuffer = new byte[64];
			this.Initialize();
		}

		~MD5CryptoServiceProvider()
		{
			this.Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this._ProcessingBuffer != null)
				{
					Array.Clear(this._ProcessingBuffer, 0, this._ProcessingBuffer.Length);
					this._ProcessingBuffer = null;
				}
				if (this._H != null)
				{
					Array.Clear(this._H, 0, this._H.Length);
					this._H = null;
				}
				if (this.buff != null)
				{
					Array.Clear(this.buff, 0, this.buff.Length);
					this.buff = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
		{
			this.State = 1;
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
				this.ProcessBlock(this._ProcessingBuffer, 0);
				this._ProcessingBufferCount = 0;
				ibStart += i;
				cbSize -= i;
			}
			for (int i = 0; i < cbSize - cbSize % 64; i += 64)
			{
				this.ProcessBlock(rgb, ibStart + i);
			}
			if (cbSize % 64 != 0)
			{
				Buffer.BlockCopy(rgb, cbSize - cbSize % 64 + ibStart, this._ProcessingBuffer, 0, cbSize % 64);
				this._ProcessingBufferCount = cbSize % 64;
			}
		}

		protected override byte[] HashFinal()
		{
			byte[] array = new byte[16];
			this.ProcessFinalBlock(this._ProcessingBuffer, 0, this._ProcessingBufferCount);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i * 4 + j] = (byte)(this._H[i] >> j * 8);
				}
			}
			return array;
		}

		public override void Initialize()
		{
			this.count = 0UL;
			this._ProcessingBufferCount = 0;
			this._H[0] = 1732584193U;
			this._H[1] = 4023233417U;
			this._H[2] = 2562383102U;
			this._H[3] = 271733878U;
		}

		private void ProcessBlock(byte[] inputBuffer, int inputOffset)
		{
			this.count += 64UL;
			for (int i = 0; i < 16; i++)
			{
				this.buff[i] = (uint)((int)inputBuffer[inputOffset + 4 * i] | ((int)inputBuffer[inputOffset + 4 * i + 1] << 8) | ((int)inputBuffer[inputOffset + 4 * i + 2] << 16) | ((int)inputBuffer[inputOffset + 4 * i + 3] << 24));
			}
			uint num = this._H[0];
			uint num2 = this._H[1];
			uint num3 = this._H[2];
			uint num4 = this._H[3];
			num += (((num3 ^ num4) & num2) ^ num4) + MD5CryptoServiceProvider.K[0] + this.buff[0];
			num = (num << 7) | (num >> 25);
			num += num2;
			num4 += (((num2 ^ num3) & num) ^ num3) + MD5CryptoServiceProvider.K[1] + this.buff[1];
			num4 = (num4 << 12) | (num4 >> 20);
			num4 += num;
			num3 += (((num ^ num2) & num4) ^ num2) + MD5CryptoServiceProvider.K[2] + this.buff[2];
			num3 = (num3 << 17) | (num3 >> 15);
			num3 += num4;
			num2 += (((num4 ^ num) & num3) ^ num) + MD5CryptoServiceProvider.K[3] + this.buff[3];
			num2 = (num2 << 22) | (num2 >> 10);
			num2 += num3;
			num += (((num3 ^ num4) & num2) ^ num4) + MD5CryptoServiceProvider.K[4] + this.buff[4];
			num = (num << 7) | (num >> 25);
			num += num2;
			num4 += (((num2 ^ num3) & num) ^ num3) + MD5CryptoServiceProvider.K[5] + this.buff[5];
			num4 = (num4 << 12) | (num4 >> 20);
			num4 += num;
			num3 += (((num ^ num2) & num4) ^ num2) + MD5CryptoServiceProvider.K[6] + this.buff[6];
			num3 = (num3 << 17) | (num3 >> 15);
			num3 += num4;
			num2 += (((num4 ^ num) & num3) ^ num) + MD5CryptoServiceProvider.K[7] + this.buff[7];
			num2 = (num2 << 22) | (num2 >> 10);
			num2 += num3;
			num += (((num3 ^ num4) & num2) ^ num4) + MD5CryptoServiceProvider.K[8] + this.buff[8];
			num = (num << 7) | (num >> 25);
			num += num2;
			num4 += (((num2 ^ num3) & num) ^ num3) + MD5CryptoServiceProvider.K[9] + this.buff[9];
			num4 = (num4 << 12) | (num4 >> 20);
			num4 += num;
			num3 += (((num ^ num2) & num4) ^ num2) + MD5CryptoServiceProvider.K[10] + this.buff[10];
			num3 = (num3 << 17) | (num3 >> 15);
			num3 += num4;
			num2 += (((num4 ^ num) & num3) ^ num) + MD5CryptoServiceProvider.K[11] + this.buff[11];
			num2 = (num2 << 22) | (num2 >> 10);
			num2 += num3;
			num += (((num3 ^ num4) & num2) ^ num4) + MD5CryptoServiceProvider.K[12] + this.buff[12];
			num = (num << 7) | (num >> 25);
			num += num2;
			num4 += (((num2 ^ num3) & num) ^ num3) + MD5CryptoServiceProvider.K[13] + this.buff[13];
			num4 = (num4 << 12) | (num4 >> 20);
			num4 += num;
			num3 += (((num ^ num2) & num4) ^ num2) + MD5CryptoServiceProvider.K[14] + this.buff[14];
			num3 = (num3 << 17) | (num3 >> 15);
			num3 += num4;
			num2 += (((num4 ^ num) & num3) ^ num) + MD5CryptoServiceProvider.K[15] + this.buff[15];
			num2 = (num2 << 22) | (num2 >> 10);
			num2 += num3;
			num += (((num2 ^ num3) & num4) ^ num3) + MD5CryptoServiceProvider.K[16] + this.buff[1];
			num = (num << 5) | (num >> 27);
			num += num2;
			num4 += (((num ^ num2) & num3) ^ num2) + MD5CryptoServiceProvider.K[17] + this.buff[6];
			num4 = (num4 << 9) | (num4 >> 23);
			num4 += num;
			num3 += (((num4 ^ num) & num2) ^ num) + MD5CryptoServiceProvider.K[18] + this.buff[11];
			num3 = (num3 << 14) | (num3 >> 18);
			num3 += num4;
			num2 += (((num3 ^ num4) & num) ^ num4) + MD5CryptoServiceProvider.K[19] + this.buff[0];
			num2 = (num2 << 20) | (num2 >> 12);
			num2 += num3;
			num += (((num2 ^ num3) & num4) ^ num3) + MD5CryptoServiceProvider.K[20] + this.buff[5];
			num = (num << 5) | (num >> 27);
			num += num2;
			num4 += (((num ^ num2) & num3) ^ num2) + MD5CryptoServiceProvider.K[21] + this.buff[10];
			num4 = (num4 << 9) | (num4 >> 23);
			num4 += num;
			num3 += (((num4 ^ num) & num2) ^ num) + MD5CryptoServiceProvider.K[22] + this.buff[15];
			num3 = (num3 << 14) | (num3 >> 18);
			num3 += num4;
			num2 += (((num3 ^ num4) & num) ^ num4) + MD5CryptoServiceProvider.K[23] + this.buff[4];
			num2 = (num2 << 20) | (num2 >> 12);
			num2 += num3;
			num += (((num2 ^ num3) & num4) ^ num3) + MD5CryptoServiceProvider.K[24] + this.buff[9];
			num = (num << 5) | (num >> 27);
			num += num2;
			num4 += (((num ^ num2) & num3) ^ num2) + MD5CryptoServiceProvider.K[25] + this.buff[14];
			num4 = (num4 << 9) | (num4 >> 23);
			num4 += num;
			num3 += (((num4 ^ num) & num2) ^ num) + MD5CryptoServiceProvider.K[26] + this.buff[3];
			num3 = (num3 << 14) | (num3 >> 18);
			num3 += num4;
			num2 += (((num3 ^ num4) & num) ^ num4) + MD5CryptoServiceProvider.K[27] + this.buff[8];
			num2 = (num2 << 20) | (num2 >> 12);
			num2 += num3;
			num += (((num2 ^ num3) & num4) ^ num3) + MD5CryptoServiceProvider.K[28] + this.buff[13];
			num = (num << 5) | (num >> 27);
			num += num2;
			num4 += (((num ^ num2) & num3) ^ num2) + MD5CryptoServiceProvider.K[29] + this.buff[2];
			num4 = (num4 << 9) | (num4 >> 23);
			num4 += num;
			num3 += (((num4 ^ num) & num2) ^ num) + MD5CryptoServiceProvider.K[30] + this.buff[7];
			num3 = (num3 << 14) | (num3 >> 18);
			num3 += num4;
			num2 += (((num3 ^ num4) & num) ^ num4) + MD5CryptoServiceProvider.K[31] + this.buff[12];
			num2 = (num2 << 20) | (num2 >> 12);
			num2 += num3;
			num += (num2 ^ num3 ^ num4) + MD5CryptoServiceProvider.K[32] + this.buff[5];
			num = (num << 4) | (num >> 28);
			num += num2;
			num4 += (num ^ num2 ^ num3) + MD5CryptoServiceProvider.K[33] + this.buff[8];
			num4 = (num4 << 11) | (num4 >> 21);
			num4 += num;
			num3 += (num4 ^ num ^ num2) + MD5CryptoServiceProvider.K[34] + this.buff[11];
			num3 = (num3 << 16) | (num3 >> 16);
			num3 += num4;
			num2 += (num3 ^ num4 ^ num) + MD5CryptoServiceProvider.K[35] + this.buff[14];
			num2 = (num2 << 23) | (num2 >> 9);
			num2 += num3;
			num += (num2 ^ num3 ^ num4) + MD5CryptoServiceProvider.K[36] + this.buff[1];
			num = (num << 4) | (num >> 28);
			num += num2;
			num4 += (num ^ num2 ^ num3) + MD5CryptoServiceProvider.K[37] + this.buff[4];
			num4 = (num4 << 11) | (num4 >> 21);
			num4 += num;
			num3 += (num4 ^ num ^ num2) + MD5CryptoServiceProvider.K[38] + this.buff[7];
			num3 = (num3 << 16) | (num3 >> 16);
			num3 += num4;
			num2 += (num3 ^ num4 ^ num) + MD5CryptoServiceProvider.K[39] + this.buff[10];
			num2 = (num2 << 23) | (num2 >> 9);
			num2 += num3;
			num += (num2 ^ num3 ^ num4) + MD5CryptoServiceProvider.K[40] + this.buff[13];
			num = (num << 4) | (num >> 28);
			num += num2;
			num4 += (num ^ num2 ^ num3) + MD5CryptoServiceProvider.K[41] + this.buff[0];
			num4 = (num4 << 11) | (num4 >> 21);
			num4 += num;
			num3 += (num4 ^ num ^ num2) + MD5CryptoServiceProvider.K[42] + this.buff[3];
			num3 = (num3 << 16) | (num3 >> 16);
			num3 += num4;
			num2 += (num3 ^ num4 ^ num) + MD5CryptoServiceProvider.K[43] + this.buff[6];
			num2 = (num2 << 23) | (num2 >> 9);
			num2 += num3;
			num += (num2 ^ num3 ^ num4) + MD5CryptoServiceProvider.K[44] + this.buff[9];
			num = (num << 4) | (num >> 28);
			num += num2;
			num4 += (num ^ num2 ^ num3) + MD5CryptoServiceProvider.K[45] + this.buff[12];
			num4 = (num4 << 11) | (num4 >> 21);
			num4 += num;
			num3 += (num4 ^ num ^ num2) + MD5CryptoServiceProvider.K[46] + this.buff[15];
			num3 = (num3 << 16) | (num3 >> 16);
			num3 += num4;
			num2 += (num3 ^ num4 ^ num) + MD5CryptoServiceProvider.K[47] + this.buff[2];
			num2 = (num2 << 23) | (num2 >> 9);
			num2 += num3;
			num += ((~num4 | num2) ^ num3) + MD5CryptoServiceProvider.K[48] + this.buff[0];
			num = (num << 6) | (num >> 26);
			num += num2;
			num4 += ((~num3 | num) ^ num2) + MD5CryptoServiceProvider.K[49] + this.buff[7];
			num4 = (num4 << 10) | (num4 >> 22);
			num4 += num;
			num3 += ((~num2 | num4) ^ num) + MD5CryptoServiceProvider.K[50] + this.buff[14];
			num3 = (num3 << 15) | (num3 >> 17);
			num3 += num4;
			num2 += ((~num | num3) ^ num4) + MD5CryptoServiceProvider.K[51] + this.buff[5];
			num2 = (num2 << 21) | (num2 >> 11);
			num2 += num3;
			num += ((~num4 | num2) ^ num3) + MD5CryptoServiceProvider.K[52] + this.buff[12];
			num = (num << 6) | (num >> 26);
			num += num2;
			num4 += ((~num3 | num) ^ num2) + MD5CryptoServiceProvider.K[53] + this.buff[3];
			num4 = (num4 << 10) | (num4 >> 22);
			num4 += num;
			num3 += ((~num2 | num4) ^ num) + MD5CryptoServiceProvider.K[54] + this.buff[10];
			num3 = (num3 << 15) | (num3 >> 17);
			num3 += num4;
			num2 += ((~num | num3) ^ num4) + MD5CryptoServiceProvider.K[55] + this.buff[1];
			num2 = (num2 << 21) | (num2 >> 11);
			num2 += num3;
			num += ((~num4 | num2) ^ num3) + MD5CryptoServiceProvider.K[56] + this.buff[8];
			num = (num << 6) | (num >> 26);
			num += num2;
			num4 += ((~num3 | num) ^ num2) + MD5CryptoServiceProvider.K[57] + this.buff[15];
			num4 = (num4 << 10) | (num4 >> 22);
			num4 += num;
			num3 += ((~num2 | num4) ^ num) + MD5CryptoServiceProvider.K[58] + this.buff[6];
			num3 = (num3 << 15) | (num3 >> 17);
			num3 += num4;
			num2 += ((~num | num3) ^ num4) + MD5CryptoServiceProvider.K[59] + this.buff[13];
			num2 = (num2 << 21) | (num2 >> 11);
			num2 += num3;
			num += ((~num4 | num2) ^ num3) + MD5CryptoServiceProvider.K[60] + this.buff[4];
			num = (num << 6) | (num >> 26);
			num += num2;
			num4 += ((~num3 | num) ^ num2) + MD5CryptoServiceProvider.K[61] + this.buff[11];
			num4 = (num4 << 10) | (num4 >> 22);
			num4 += num;
			num3 += ((~num2 | num4) ^ num) + MD5CryptoServiceProvider.K[62] + this.buff[2];
			num3 = (num3 << 15) | (num3 >> 17);
			num3 += num4;
			num2 += ((~num | num3) ^ num4) + MD5CryptoServiceProvider.K[63] + this.buff[9];
			num2 = (num2 << 21) | (num2 >> 11);
			num2 += num3;
			this._H[0] += num;
			this._H[1] += num2;
			this._H[2] += num3;
			this._H[3] += num4;
		}

		private void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			ulong num = this.count + (ulong)((long)inputCount);
			int num2 = (int)(56UL - num % 64UL);
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
			buffer[position++] = (byte)length;
			buffer[position++] = (byte)(length >> 8);
			buffer[position++] = (byte)(length >> 16);
			buffer[position++] = (byte)(length >> 24);
			buffer[position++] = (byte)(length >> 32);
			buffer[position++] = (byte)(length >> 40);
			buffer[position++] = (byte)(length >> 48);
			buffer[position] = (byte)(length >> 56);
		}

		private const int BLOCK_SIZE_BYTES = 64;

		private const int HASH_SIZE_BYTES = 16;

		private uint[] _H;

		private uint[] buff;

		private ulong count;

		private byte[] _ProcessingBuffer;

		private int _ProcessingBufferCount;

		private static readonly uint[] K = new uint[]
		{
			3614090360U, 3905402710U, 606105819U, 3250441966U, 4118548399U, 1200080426U, 2821735955U, 4249261313U, 1770035416U, 2336552879U,
			4294925233U, 2304563134U, 1804603682U, 4254626195U, 2792965006U, 1236535329U, 4129170786U, 3225465664U, 643717713U, 3921069994U,
			3593408605U, 38016083U, 3634488961U, 3889429448U, 568446438U, 3275163606U, 4107603335U, 1163531501U, 2850285829U, 4243563512U,
			1735328473U, 2368359562U, 4294588738U, 2272392833U, 1839030562U, 4259657740U, 2763975236U, 1272893353U, 4139469664U, 3200236656U,
			681279174U, 3936430074U, 3572445317U, 76029189U, 3654602809U, 3873151461U, 530742520U, 3299628645U, 4096336452U, 1126891415U,
			2878612391U, 4237533241U, 1700485571U, 2399980690U, 4293915773U, 2240044497U, 1873313359U, 4264355552U, 2734768916U, 1309151649U,
			4149444226U, 3174756917U, 718787259U, 3951481745U
		};
	}
}
