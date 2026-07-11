using System;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	internal class AesTransform : SymmetricTransform
	{
		public AesTransform(Aes algo, bool encryption, byte[] key, byte[] iv)
			: base(algo, encryption, iv)
		{
			if (key == null)
			{
				throw new CryptographicException("key is null");
			}
			if (iv != null && iv.Length != algo.BlockSize >> 3)
			{
				string text = Locale.GetText("IV length is invalid ({0} bytes), it should be {1} bytes long.", new object[]
				{
					iv.Length,
					algo.BlockSize >> 3
				});
				throw new CryptographicException(text);
			}
			int num = key.Length;
			if (num != 16 && num != 24 && num != 32)
			{
				string text2 = Locale.GetText("Key is too small ({0} bytes), it should be {1}, {2} or {3} bytes long.", new object[] { num, 16, 24, 32 });
				throw new CryptographicException(text2);
			}
			num <<= 3;
			this.Nk = num >> 5;
			if (this.Nk == 8)
			{
				this.Nr = 14;
			}
			else if (this.Nk == 6)
			{
				this.Nr = 12;
			}
			else
			{
				this.Nr = 10;
			}
			int num2 = 4 * (this.Nr + 1);
			uint[] array = new uint[num2];
			int num3 = 0;
			for (int i = 0; i < this.Nk; i++)
			{
				uint num4 = (uint)((uint)key[num3++] << 24);
				num4 |= (uint)((uint)key[num3++] << 16);
				num4 |= (uint)((uint)key[num3++] << 8);
				num4 |= (uint)key[num3++];
				array[i] = num4;
			}
			for (int j = this.Nk; j < num2; j++)
			{
				uint num5 = array[j - 1];
				if (j % this.Nk == 0)
				{
					uint num6 = (num5 << 8) | ((num5 >> 24) & 255U);
					num5 = this.SubByte(num6) ^ AesTransform.Rcon[j / this.Nk];
				}
				else if (this.Nk > 6 && j % this.Nk == 4)
				{
					num5 = this.SubByte(num5);
				}
				array[j] = array[j - this.Nk] ^ num5;
			}
			if (!encryption && (algo.Mode == CipherMode.ECB || algo.Mode == CipherMode.CBC))
			{
				int k = 0;
				int num7 = num2 - 4;
				while (k < num7)
				{
					for (int l = 0; l < 4; l++)
					{
						uint num8 = array[k + l];
						array[k + l] = array[num7 + l];
						array[num7 + l] = num8;
					}
					k += 4;
					num7 -= 4;
				}
				for (int m = 4; m < array.Length - 4; m++)
				{
					array[m] = AesTransform.iT0[(int)AesTransform.SBox[(int)((UIntPtr)(array[m] >> 24))]] ^ AesTransform.iT1[(int)AesTransform.SBox[(int)((byte)(array[m] >> 16))]] ^ AesTransform.iT2[(int)AesTransform.SBox[(int)((byte)(array[m] >> 8))]] ^ AesTransform.iT3[(int)AesTransform.SBox[(int)((byte)array[m])]];
				}
			}
			this.expandedKey = array;
		}

		public void Clear()
		{
			this.Dispose(true);
		}

		protected override void ECB(byte[] input, byte[] output)
		{
			if (this.encrypt)
			{
				this.Encrypt128(input, output, this.expandedKey);
			}
			else
			{
				this.Decrypt128(input, output, this.expandedKey);
			}
		}

		private uint SubByte(uint a)
		{
			uint num = 255U & a;
			uint num2 = (uint)AesTransform.SBox[(int)((UIntPtr)num)];
			num = 255U & (a >> 8);
			num2 |= (uint)((uint)AesTransform.SBox[(int)((UIntPtr)num)] << 8);
			num = 255U & (a >> 16);
			num2 |= (uint)((uint)AesTransform.SBox[(int)((UIntPtr)num)] << 16);
			num = 255U & (a >> 24);
			return num2 | (uint)((uint)AesTransform.SBox[(int)((UIntPtr)num)] << 24);
		}

		private void Encrypt128(byte[] indata, byte[] outdata, uint[] ekey)
		{
			int num = 40;
			uint num2 = (uint)((((int)indata[0] << 24) | ((int)indata[1] << 16) | ((int)indata[2] << 8) | (int)indata[3]) ^ (int)ekey[0]);
			uint num3 = (uint)((((int)indata[4] << 24) | ((int)indata[5] << 16) | ((int)indata[6] << 8) | (int)indata[7]) ^ (int)ekey[1]);
			uint num4 = (uint)((((int)indata[8] << 24) | ((int)indata[9] << 16) | ((int)indata[10] << 8) | (int)indata[11]) ^ (int)ekey[2]);
			uint num5 = (uint)((((int)indata[12] << 24) | ((int)indata[13] << 16) | ((int)indata[14] << 8) | (int)indata[15]) ^ (int)ekey[3]);
			uint num6 = AesTransform.T0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.T1[(int)((byte)(num3 >> 16))] ^ AesTransform.T2[(int)((byte)(num4 >> 8))] ^ AesTransform.T3[(int)((byte)num5)] ^ ekey[4];
			uint num7 = AesTransform.T0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.T1[(int)((byte)(num4 >> 16))] ^ AesTransform.T2[(int)((byte)(num5 >> 8))] ^ AesTransform.T3[(int)((byte)num2)] ^ ekey[5];
			uint num8 = AesTransform.T0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.T1[(int)((byte)(num5 >> 16))] ^ AesTransform.T2[(int)((byte)(num2 >> 8))] ^ AesTransform.T3[(int)((byte)num3)] ^ ekey[6];
			uint num9 = AesTransform.T0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.T1[(int)((byte)(num2 >> 16))] ^ AesTransform.T2[(int)((byte)(num3 >> 8))] ^ AesTransform.T3[(int)((byte)num4)] ^ ekey[7];
			num2 = AesTransform.T0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.T1[(int)((byte)(num7 >> 16))] ^ AesTransform.T2[(int)((byte)(num8 >> 8))] ^ AesTransform.T3[(int)((byte)num9)] ^ ekey[8];
			num3 = AesTransform.T0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.T1[(int)((byte)(num8 >> 16))] ^ AesTransform.T2[(int)((byte)(num9 >> 8))] ^ AesTransform.T3[(int)((byte)num6)] ^ ekey[9];
			num4 = AesTransform.T0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.T1[(int)((byte)(num9 >> 16))] ^ AesTransform.T2[(int)((byte)(num6 >> 8))] ^ AesTransform.T3[(int)((byte)num7)] ^ ekey[10];
			num5 = AesTransform.T0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.T1[(int)((byte)(num6 >> 16))] ^ AesTransform.T2[(int)((byte)(num7 >> 8))] ^ AesTransform.T3[(int)((byte)num8)] ^ ekey[11];
			num6 = AesTransform.T0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.T1[(int)((byte)(num3 >> 16))] ^ AesTransform.T2[(int)((byte)(num4 >> 8))] ^ AesTransform.T3[(int)((byte)num5)] ^ ekey[12];
			num7 = AesTransform.T0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.T1[(int)((byte)(num4 >> 16))] ^ AesTransform.T2[(int)((byte)(num5 >> 8))] ^ AesTransform.T3[(int)((byte)num2)] ^ ekey[13];
			num8 = AesTransform.T0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.T1[(int)((byte)(num5 >> 16))] ^ AesTransform.T2[(int)((byte)(num2 >> 8))] ^ AesTransform.T3[(int)((byte)num3)] ^ ekey[14];
			num9 = AesTransform.T0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.T1[(int)((byte)(num2 >> 16))] ^ AesTransform.T2[(int)((byte)(num3 >> 8))] ^ AesTransform.T3[(int)((byte)num4)] ^ ekey[15];
			num2 = AesTransform.T0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.T1[(int)((byte)(num7 >> 16))] ^ AesTransform.T2[(int)((byte)(num8 >> 8))] ^ AesTransform.T3[(int)((byte)num9)] ^ ekey[16];
			num3 = AesTransform.T0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.T1[(int)((byte)(num8 >> 16))] ^ AesTransform.T2[(int)((byte)(num9 >> 8))] ^ AesTransform.T3[(int)((byte)num6)] ^ ekey[17];
			num4 = AesTransform.T0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.T1[(int)((byte)(num9 >> 16))] ^ AesTransform.T2[(int)((byte)(num6 >> 8))] ^ AesTransform.T3[(int)((byte)num7)] ^ ekey[18];
			num5 = AesTransform.T0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.T1[(int)((byte)(num6 >> 16))] ^ AesTransform.T2[(int)((byte)(num7 >> 8))] ^ AesTransform.T3[(int)((byte)num8)] ^ ekey[19];
			num6 = AesTransform.T0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.T1[(int)((byte)(num3 >> 16))] ^ AesTransform.T2[(int)((byte)(num4 >> 8))] ^ AesTransform.T3[(int)((byte)num5)] ^ ekey[20];
			num7 = AesTransform.T0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.T1[(int)((byte)(num4 >> 16))] ^ AesTransform.T2[(int)((byte)(num5 >> 8))] ^ AesTransform.T3[(int)((byte)num2)] ^ ekey[21];
			num8 = AesTransform.T0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.T1[(int)((byte)(num5 >> 16))] ^ AesTransform.T2[(int)((byte)(num2 >> 8))] ^ AesTransform.T3[(int)((byte)num3)] ^ ekey[22];
			num9 = AesTransform.T0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.T1[(int)((byte)(num2 >> 16))] ^ AesTransform.T2[(int)((byte)(num3 >> 8))] ^ AesTransform.T3[(int)((byte)num4)] ^ ekey[23];
			num2 = AesTransform.T0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.T1[(int)((byte)(num7 >> 16))] ^ AesTransform.T2[(int)((byte)(num8 >> 8))] ^ AesTransform.T3[(int)((byte)num9)] ^ ekey[24];
			num3 = AesTransform.T0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.T1[(int)((byte)(num8 >> 16))] ^ AesTransform.T2[(int)((byte)(num9 >> 8))] ^ AesTransform.T3[(int)((byte)num6)] ^ ekey[25];
			num4 = AesTransform.T0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.T1[(int)((byte)(num9 >> 16))] ^ AesTransform.T2[(int)((byte)(num6 >> 8))] ^ AesTransform.T3[(int)((byte)num7)] ^ ekey[26];
			num5 = AesTransform.T0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.T1[(int)((byte)(num6 >> 16))] ^ AesTransform.T2[(int)((byte)(num7 >> 8))] ^ AesTransform.T3[(int)((byte)num8)] ^ ekey[27];
			num6 = AesTransform.T0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.T1[(int)((byte)(num3 >> 16))] ^ AesTransform.T2[(int)((byte)(num4 >> 8))] ^ AesTransform.T3[(int)((byte)num5)] ^ ekey[28];
			num7 = AesTransform.T0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.T1[(int)((byte)(num4 >> 16))] ^ AesTransform.T2[(int)((byte)(num5 >> 8))] ^ AesTransform.T3[(int)((byte)num2)] ^ ekey[29];
			num8 = AesTransform.T0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.T1[(int)((byte)(num5 >> 16))] ^ AesTransform.T2[(int)((byte)(num2 >> 8))] ^ AesTransform.T3[(int)((byte)num3)] ^ ekey[30];
			num9 = AesTransform.T0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.T1[(int)((byte)(num2 >> 16))] ^ AesTransform.T2[(int)((byte)(num3 >> 8))] ^ AesTransform.T3[(int)((byte)num4)] ^ ekey[31];
			num2 = AesTransform.T0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.T1[(int)((byte)(num7 >> 16))] ^ AesTransform.T2[(int)((byte)(num8 >> 8))] ^ AesTransform.T3[(int)((byte)num9)] ^ ekey[32];
			num3 = AesTransform.T0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.T1[(int)((byte)(num8 >> 16))] ^ AesTransform.T2[(int)((byte)(num9 >> 8))] ^ AesTransform.T3[(int)((byte)num6)] ^ ekey[33];
			num4 = AesTransform.T0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.T1[(int)((byte)(num9 >> 16))] ^ AesTransform.T2[(int)((byte)(num6 >> 8))] ^ AesTransform.T3[(int)((byte)num7)] ^ ekey[34];
			num5 = AesTransform.T0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.T1[(int)((byte)(num6 >> 16))] ^ AesTransform.T2[(int)((byte)(num7 >> 8))] ^ AesTransform.T3[(int)((byte)num8)] ^ ekey[35];
			num6 = AesTransform.T0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.T1[(int)((byte)(num3 >> 16))] ^ AesTransform.T2[(int)((byte)(num4 >> 8))] ^ AesTransform.T3[(int)((byte)num5)] ^ ekey[36];
			num7 = AesTransform.T0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.T1[(int)((byte)(num4 >> 16))] ^ AesTransform.T2[(int)((byte)(num5 >> 8))] ^ AesTransform.T3[(int)((byte)num2)] ^ ekey[37];
			num8 = AesTransform.T0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.T1[(int)((byte)(num5 >> 16))] ^ AesTransform.T2[(int)((byte)(num2 >> 8))] ^ AesTransform.T3[(int)((byte)num3)] ^ ekey[38];
			num9 = AesTransform.T0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.T1[(int)((byte)(num2 >> 16))] ^ AesTransform.T2[(int)((byte)(num3 >> 8))] ^ AesTransform.T3[(int)((byte)num4)] ^ ekey[39];
			if (this.Nr > 10)
			{
				num2 = AesTransform.T0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.T1[(int)((byte)(num7 >> 16))] ^ AesTransform.T2[(int)((byte)(num8 >> 8))] ^ AesTransform.T3[(int)((byte)num9)] ^ ekey[40];
				num3 = AesTransform.T0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.T1[(int)((byte)(num8 >> 16))] ^ AesTransform.T2[(int)((byte)(num9 >> 8))] ^ AesTransform.T3[(int)((byte)num6)] ^ ekey[41];
				num4 = AesTransform.T0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.T1[(int)((byte)(num9 >> 16))] ^ AesTransform.T2[(int)((byte)(num6 >> 8))] ^ AesTransform.T3[(int)((byte)num7)] ^ ekey[42];
				num5 = AesTransform.T0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.T1[(int)((byte)(num6 >> 16))] ^ AesTransform.T2[(int)((byte)(num7 >> 8))] ^ AesTransform.T3[(int)((byte)num8)] ^ ekey[43];
				num6 = AesTransform.T0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.T1[(int)((byte)(num3 >> 16))] ^ AesTransform.T2[(int)((byte)(num4 >> 8))] ^ AesTransform.T3[(int)((byte)num5)] ^ ekey[44];
				num7 = AesTransform.T0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.T1[(int)((byte)(num4 >> 16))] ^ AesTransform.T2[(int)((byte)(num5 >> 8))] ^ AesTransform.T3[(int)((byte)num2)] ^ ekey[45];
				num8 = AesTransform.T0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.T1[(int)((byte)(num5 >> 16))] ^ AesTransform.T2[(int)((byte)(num2 >> 8))] ^ AesTransform.T3[(int)((byte)num3)] ^ ekey[46];
				num9 = AesTransform.T0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.T1[(int)((byte)(num2 >> 16))] ^ AesTransform.T2[(int)((byte)(num3 >> 8))] ^ AesTransform.T3[(int)((byte)num4)] ^ ekey[47];
				num = 48;
				if (this.Nr > 12)
				{
					num2 = AesTransform.T0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.T1[(int)((byte)(num7 >> 16))] ^ AesTransform.T2[(int)((byte)(num8 >> 8))] ^ AesTransform.T3[(int)((byte)num9)] ^ ekey[48];
					num3 = AesTransform.T0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.T1[(int)((byte)(num8 >> 16))] ^ AesTransform.T2[(int)((byte)(num9 >> 8))] ^ AesTransform.T3[(int)((byte)num6)] ^ ekey[49];
					num4 = AesTransform.T0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.T1[(int)((byte)(num9 >> 16))] ^ AesTransform.T2[(int)((byte)(num6 >> 8))] ^ AesTransform.T3[(int)((byte)num7)] ^ ekey[50];
					num5 = AesTransform.T0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.T1[(int)((byte)(num6 >> 16))] ^ AesTransform.T2[(int)((byte)(num7 >> 8))] ^ AesTransform.T3[(int)((byte)num8)] ^ ekey[51];
					num6 = AesTransform.T0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.T1[(int)((byte)(num3 >> 16))] ^ AesTransform.T2[(int)((byte)(num4 >> 8))] ^ AesTransform.T3[(int)((byte)num5)] ^ ekey[52];
					num7 = AesTransform.T0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.T1[(int)((byte)(num4 >> 16))] ^ AesTransform.T2[(int)((byte)(num5 >> 8))] ^ AesTransform.T3[(int)((byte)num2)] ^ ekey[53];
					num8 = AesTransform.T0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.T1[(int)((byte)(num5 >> 16))] ^ AesTransform.T2[(int)((byte)(num2 >> 8))] ^ AesTransform.T3[(int)((byte)num3)] ^ ekey[54];
					num9 = AesTransform.T0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.T1[(int)((byte)(num2 >> 16))] ^ AesTransform.T2[(int)((byte)(num3 >> 8))] ^ AesTransform.T3[(int)((byte)num4)] ^ ekey[55];
					num = 56;
				}
			}
			outdata[0] = AesTransform.SBox[(int)((UIntPtr)(num6 >> 24))] ^ (byte)(ekey[num] >> 24);
			outdata[1] = AesTransform.SBox[(int)((byte)(num7 >> 16))] ^ (byte)(ekey[num] >> 16);
			outdata[2] = AesTransform.SBox[(int)((byte)(num8 >> 8))] ^ (byte)(ekey[num] >> 8);
			outdata[3] = AesTransform.SBox[(int)((byte)num9)] ^ (byte)ekey[num++];
			outdata[4] = AesTransform.SBox[(int)((UIntPtr)(num7 >> 24))] ^ (byte)(ekey[num] >> 24);
			outdata[5] = AesTransform.SBox[(int)((byte)(num8 >> 16))] ^ (byte)(ekey[num] >> 16);
			outdata[6] = AesTransform.SBox[(int)((byte)(num9 >> 8))] ^ (byte)(ekey[num] >> 8);
			outdata[7] = AesTransform.SBox[(int)((byte)num6)] ^ (byte)ekey[num++];
			outdata[8] = AesTransform.SBox[(int)((UIntPtr)(num8 >> 24))] ^ (byte)(ekey[num] >> 24);
			outdata[9] = AesTransform.SBox[(int)((byte)(num9 >> 16))] ^ (byte)(ekey[num] >> 16);
			outdata[10] = AesTransform.SBox[(int)((byte)(num6 >> 8))] ^ (byte)(ekey[num] >> 8);
			outdata[11] = AesTransform.SBox[(int)((byte)num7)] ^ (byte)ekey[num++];
			outdata[12] = AesTransform.SBox[(int)((UIntPtr)(num9 >> 24))] ^ (byte)(ekey[num] >> 24);
			outdata[13] = AesTransform.SBox[(int)((byte)(num6 >> 16))] ^ (byte)(ekey[num] >> 16);
			outdata[14] = AesTransform.SBox[(int)((byte)(num7 >> 8))] ^ (byte)(ekey[num] >> 8);
			outdata[15] = AesTransform.SBox[(int)((byte)num8)] ^ (byte)ekey[num++];
		}

		private void Decrypt128(byte[] indata, byte[] outdata, uint[] ekey)
		{
			int num = 40;
			uint num2 = (uint)((((int)indata[0] << 24) | ((int)indata[1] << 16) | ((int)indata[2] << 8) | (int)indata[3]) ^ (int)ekey[0]);
			uint num3 = (uint)((((int)indata[4] << 24) | ((int)indata[5] << 16) | ((int)indata[6] << 8) | (int)indata[7]) ^ (int)ekey[1]);
			uint num4 = (uint)((((int)indata[8] << 24) | ((int)indata[9] << 16) | ((int)indata[10] << 8) | (int)indata[11]) ^ (int)ekey[2]);
			uint num5 = (uint)((((int)indata[12] << 24) | ((int)indata[13] << 16) | ((int)indata[14] << 8) | (int)indata[15]) ^ (int)ekey[3]);
			uint num6 = AesTransform.iT0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.iT1[(int)((byte)(num5 >> 16))] ^ AesTransform.iT2[(int)((byte)(num4 >> 8))] ^ AesTransform.iT3[(int)((byte)num3)] ^ ekey[4];
			uint num7 = AesTransform.iT0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.iT1[(int)((byte)(num2 >> 16))] ^ AesTransform.iT2[(int)((byte)(num5 >> 8))] ^ AesTransform.iT3[(int)((byte)num4)] ^ ekey[5];
			uint num8 = AesTransform.iT0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.iT1[(int)((byte)(num3 >> 16))] ^ AesTransform.iT2[(int)((byte)(num2 >> 8))] ^ AesTransform.iT3[(int)((byte)num5)] ^ ekey[6];
			uint num9 = AesTransform.iT0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.iT1[(int)((byte)(num4 >> 16))] ^ AesTransform.iT2[(int)((byte)(num3 >> 8))] ^ AesTransform.iT3[(int)((byte)num2)] ^ ekey[7];
			num2 = AesTransform.iT0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.iT1[(int)((byte)(num9 >> 16))] ^ AesTransform.iT2[(int)((byte)(num8 >> 8))] ^ AesTransform.iT3[(int)((byte)num7)] ^ ekey[8];
			num3 = AesTransform.iT0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.iT1[(int)((byte)(num6 >> 16))] ^ AesTransform.iT2[(int)((byte)(num9 >> 8))] ^ AesTransform.iT3[(int)((byte)num8)] ^ ekey[9];
			num4 = AesTransform.iT0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.iT1[(int)((byte)(num7 >> 16))] ^ AesTransform.iT2[(int)((byte)(num6 >> 8))] ^ AesTransform.iT3[(int)((byte)num9)] ^ ekey[10];
			num5 = AesTransform.iT0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.iT1[(int)((byte)(num8 >> 16))] ^ AesTransform.iT2[(int)((byte)(num7 >> 8))] ^ AesTransform.iT3[(int)((byte)num6)] ^ ekey[11];
			num6 = AesTransform.iT0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.iT1[(int)((byte)(num5 >> 16))] ^ AesTransform.iT2[(int)((byte)(num4 >> 8))] ^ AesTransform.iT3[(int)((byte)num3)] ^ ekey[12];
			num7 = AesTransform.iT0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.iT1[(int)((byte)(num2 >> 16))] ^ AesTransform.iT2[(int)((byte)(num5 >> 8))] ^ AesTransform.iT3[(int)((byte)num4)] ^ ekey[13];
			num8 = AesTransform.iT0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.iT1[(int)((byte)(num3 >> 16))] ^ AesTransform.iT2[(int)((byte)(num2 >> 8))] ^ AesTransform.iT3[(int)((byte)num5)] ^ ekey[14];
			num9 = AesTransform.iT0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.iT1[(int)((byte)(num4 >> 16))] ^ AesTransform.iT2[(int)((byte)(num3 >> 8))] ^ AesTransform.iT3[(int)((byte)num2)] ^ ekey[15];
			num2 = AesTransform.iT0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.iT1[(int)((byte)(num9 >> 16))] ^ AesTransform.iT2[(int)((byte)(num8 >> 8))] ^ AesTransform.iT3[(int)((byte)num7)] ^ ekey[16];
			num3 = AesTransform.iT0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.iT1[(int)((byte)(num6 >> 16))] ^ AesTransform.iT2[(int)((byte)(num9 >> 8))] ^ AesTransform.iT3[(int)((byte)num8)] ^ ekey[17];
			num4 = AesTransform.iT0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.iT1[(int)((byte)(num7 >> 16))] ^ AesTransform.iT2[(int)((byte)(num6 >> 8))] ^ AesTransform.iT3[(int)((byte)num9)] ^ ekey[18];
			num5 = AesTransform.iT0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.iT1[(int)((byte)(num8 >> 16))] ^ AesTransform.iT2[(int)((byte)(num7 >> 8))] ^ AesTransform.iT3[(int)((byte)num6)] ^ ekey[19];
			num6 = AesTransform.iT0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.iT1[(int)((byte)(num5 >> 16))] ^ AesTransform.iT2[(int)((byte)(num4 >> 8))] ^ AesTransform.iT3[(int)((byte)num3)] ^ ekey[20];
			num7 = AesTransform.iT0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.iT1[(int)((byte)(num2 >> 16))] ^ AesTransform.iT2[(int)((byte)(num5 >> 8))] ^ AesTransform.iT3[(int)((byte)num4)] ^ ekey[21];
			num8 = AesTransform.iT0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.iT1[(int)((byte)(num3 >> 16))] ^ AesTransform.iT2[(int)((byte)(num2 >> 8))] ^ AesTransform.iT3[(int)((byte)num5)] ^ ekey[22];
			num9 = AesTransform.iT0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.iT1[(int)((byte)(num4 >> 16))] ^ AesTransform.iT2[(int)((byte)(num3 >> 8))] ^ AesTransform.iT3[(int)((byte)num2)] ^ ekey[23];
			num2 = AesTransform.iT0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.iT1[(int)((byte)(num9 >> 16))] ^ AesTransform.iT2[(int)((byte)(num8 >> 8))] ^ AesTransform.iT3[(int)((byte)num7)] ^ ekey[24];
			num3 = AesTransform.iT0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.iT1[(int)((byte)(num6 >> 16))] ^ AesTransform.iT2[(int)((byte)(num9 >> 8))] ^ AesTransform.iT3[(int)((byte)num8)] ^ ekey[25];
			num4 = AesTransform.iT0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.iT1[(int)((byte)(num7 >> 16))] ^ AesTransform.iT2[(int)((byte)(num6 >> 8))] ^ AesTransform.iT3[(int)((byte)num9)] ^ ekey[26];
			num5 = AesTransform.iT0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.iT1[(int)((byte)(num8 >> 16))] ^ AesTransform.iT2[(int)((byte)(num7 >> 8))] ^ AesTransform.iT3[(int)((byte)num6)] ^ ekey[27];
			num6 = AesTransform.iT0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.iT1[(int)((byte)(num5 >> 16))] ^ AesTransform.iT2[(int)((byte)(num4 >> 8))] ^ AesTransform.iT3[(int)((byte)num3)] ^ ekey[28];
			num7 = AesTransform.iT0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.iT1[(int)((byte)(num2 >> 16))] ^ AesTransform.iT2[(int)((byte)(num5 >> 8))] ^ AesTransform.iT3[(int)((byte)num4)] ^ ekey[29];
			num8 = AesTransform.iT0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.iT1[(int)((byte)(num3 >> 16))] ^ AesTransform.iT2[(int)((byte)(num2 >> 8))] ^ AesTransform.iT3[(int)((byte)num5)] ^ ekey[30];
			num9 = AesTransform.iT0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.iT1[(int)((byte)(num4 >> 16))] ^ AesTransform.iT2[(int)((byte)(num3 >> 8))] ^ AesTransform.iT3[(int)((byte)num2)] ^ ekey[31];
			num2 = AesTransform.iT0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.iT1[(int)((byte)(num9 >> 16))] ^ AesTransform.iT2[(int)((byte)(num8 >> 8))] ^ AesTransform.iT3[(int)((byte)num7)] ^ ekey[32];
			num3 = AesTransform.iT0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.iT1[(int)((byte)(num6 >> 16))] ^ AesTransform.iT2[(int)((byte)(num9 >> 8))] ^ AesTransform.iT3[(int)((byte)num8)] ^ ekey[33];
			num4 = AesTransform.iT0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.iT1[(int)((byte)(num7 >> 16))] ^ AesTransform.iT2[(int)((byte)(num6 >> 8))] ^ AesTransform.iT3[(int)((byte)num9)] ^ ekey[34];
			num5 = AesTransform.iT0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.iT1[(int)((byte)(num8 >> 16))] ^ AesTransform.iT2[(int)((byte)(num7 >> 8))] ^ AesTransform.iT3[(int)((byte)num6)] ^ ekey[35];
			num6 = AesTransform.iT0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.iT1[(int)((byte)(num5 >> 16))] ^ AesTransform.iT2[(int)((byte)(num4 >> 8))] ^ AesTransform.iT3[(int)((byte)num3)] ^ ekey[36];
			num7 = AesTransform.iT0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.iT1[(int)((byte)(num2 >> 16))] ^ AesTransform.iT2[(int)((byte)(num5 >> 8))] ^ AesTransform.iT3[(int)((byte)num4)] ^ ekey[37];
			num8 = AesTransform.iT0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.iT1[(int)((byte)(num3 >> 16))] ^ AesTransform.iT2[(int)((byte)(num2 >> 8))] ^ AesTransform.iT3[(int)((byte)num5)] ^ ekey[38];
			num9 = AesTransform.iT0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.iT1[(int)((byte)(num4 >> 16))] ^ AesTransform.iT2[(int)((byte)(num3 >> 8))] ^ AesTransform.iT3[(int)((byte)num2)] ^ ekey[39];
			if (this.Nr > 10)
			{
				num2 = AesTransform.iT0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.iT1[(int)((byte)(num9 >> 16))] ^ AesTransform.iT2[(int)((byte)(num8 >> 8))] ^ AesTransform.iT3[(int)((byte)num7)] ^ ekey[40];
				num3 = AesTransform.iT0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.iT1[(int)((byte)(num6 >> 16))] ^ AesTransform.iT2[(int)((byte)(num9 >> 8))] ^ AesTransform.iT3[(int)((byte)num8)] ^ ekey[41];
				num4 = AesTransform.iT0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.iT1[(int)((byte)(num7 >> 16))] ^ AesTransform.iT2[(int)((byte)(num6 >> 8))] ^ AesTransform.iT3[(int)((byte)num9)] ^ ekey[42];
				num5 = AesTransform.iT0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.iT1[(int)((byte)(num8 >> 16))] ^ AesTransform.iT2[(int)((byte)(num7 >> 8))] ^ AesTransform.iT3[(int)((byte)num6)] ^ ekey[43];
				num6 = AesTransform.iT0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.iT1[(int)((byte)(num5 >> 16))] ^ AesTransform.iT2[(int)((byte)(num4 >> 8))] ^ AesTransform.iT3[(int)((byte)num3)] ^ ekey[44];
				num7 = AesTransform.iT0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.iT1[(int)((byte)(num2 >> 16))] ^ AesTransform.iT2[(int)((byte)(num5 >> 8))] ^ AesTransform.iT3[(int)((byte)num4)] ^ ekey[45];
				num8 = AesTransform.iT0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.iT1[(int)((byte)(num3 >> 16))] ^ AesTransform.iT2[(int)((byte)(num2 >> 8))] ^ AesTransform.iT3[(int)((byte)num5)] ^ ekey[46];
				num9 = AesTransform.iT0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.iT1[(int)((byte)(num4 >> 16))] ^ AesTransform.iT2[(int)((byte)(num3 >> 8))] ^ AesTransform.iT3[(int)((byte)num2)] ^ ekey[47];
				num = 48;
				if (this.Nr > 12)
				{
					num2 = AesTransform.iT0[(int)((UIntPtr)(num6 >> 24))] ^ AesTransform.iT1[(int)((byte)(num9 >> 16))] ^ AesTransform.iT2[(int)((byte)(num8 >> 8))] ^ AesTransform.iT3[(int)((byte)num7)] ^ ekey[48];
					num3 = AesTransform.iT0[(int)((UIntPtr)(num7 >> 24))] ^ AesTransform.iT1[(int)((byte)(num6 >> 16))] ^ AesTransform.iT2[(int)((byte)(num9 >> 8))] ^ AesTransform.iT3[(int)((byte)num8)] ^ ekey[49];
					num4 = AesTransform.iT0[(int)((UIntPtr)(num8 >> 24))] ^ AesTransform.iT1[(int)((byte)(num7 >> 16))] ^ AesTransform.iT2[(int)((byte)(num6 >> 8))] ^ AesTransform.iT3[(int)((byte)num9)] ^ ekey[50];
					num5 = AesTransform.iT0[(int)((UIntPtr)(num9 >> 24))] ^ AesTransform.iT1[(int)((byte)(num8 >> 16))] ^ AesTransform.iT2[(int)((byte)(num7 >> 8))] ^ AesTransform.iT3[(int)((byte)num6)] ^ ekey[51];
					num6 = AesTransform.iT0[(int)((UIntPtr)(num2 >> 24))] ^ AesTransform.iT1[(int)((byte)(num5 >> 16))] ^ AesTransform.iT2[(int)((byte)(num4 >> 8))] ^ AesTransform.iT3[(int)((byte)num3)] ^ ekey[52];
					num7 = AesTransform.iT0[(int)((UIntPtr)(num3 >> 24))] ^ AesTransform.iT1[(int)((byte)(num2 >> 16))] ^ AesTransform.iT2[(int)((byte)(num5 >> 8))] ^ AesTransform.iT3[(int)((byte)num4)] ^ ekey[53];
					num8 = AesTransform.iT0[(int)((UIntPtr)(num4 >> 24))] ^ AesTransform.iT1[(int)((byte)(num3 >> 16))] ^ AesTransform.iT2[(int)((byte)(num2 >> 8))] ^ AesTransform.iT3[(int)((byte)num5)] ^ ekey[54];
					num9 = AesTransform.iT0[(int)((UIntPtr)(num5 >> 24))] ^ AesTransform.iT1[(int)((byte)(num4 >> 16))] ^ AesTransform.iT2[(int)((byte)(num3 >> 8))] ^ AesTransform.iT3[(int)((byte)num2)] ^ ekey[55];
					num = 56;
				}
			}
			outdata[0] = AesTransform.iSBox[(int)((UIntPtr)(num6 >> 24))] ^ (byte)(ekey[num] >> 24);
			outdata[1] = AesTransform.iSBox[(int)((byte)(num9 >> 16))] ^ (byte)(ekey[num] >> 16);
			outdata[2] = AesTransform.iSBox[(int)((byte)(num8 >> 8))] ^ (byte)(ekey[num] >> 8);
			outdata[3] = AesTransform.iSBox[(int)((byte)num7)] ^ (byte)ekey[num++];
			outdata[4] = AesTransform.iSBox[(int)((UIntPtr)(num7 >> 24))] ^ (byte)(ekey[num] >> 24);
			outdata[5] = AesTransform.iSBox[(int)((byte)(num6 >> 16))] ^ (byte)(ekey[num] >> 16);
			outdata[6] = AesTransform.iSBox[(int)((byte)(num9 >> 8))] ^ (byte)(ekey[num] >> 8);
			outdata[7] = AesTransform.iSBox[(int)((byte)num8)] ^ (byte)ekey[num++];
			outdata[8] = AesTransform.iSBox[(int)((UIntPtr)(num8 >> 24))] ^ (byte)(ekey[num] >> 24);
			outdata[9] = AesTransform.iSBox[(int)((byte)(num7 >> 16))] ^ (byte)(ekey[num] >> 16);
			outdata[10] = AesTransform.iSBox[(int)((byte)(num6 >> 8))] ^ (byte)(ekey[num] >> 8);
			outdata[11] = AesTransform.iSBox[(int)((byte)num9)] ^ (byte)ekey[num++];
			outdata[12] = AesTransform.iSBox[(int)((UIntPtr)(num9 >> 24))] ^ (byte)(ekey[num] >> 24);
			outdata[13] = AesTransform.iSBox[(int)((byte)(num8 >> 16))] ^ (byte)(ekey[num] >> 16);
			outdata[14] = AesTransform.iSBox[(int)((byte)(num7 >> 8))] ^ (byte)(ekey[num] >> 8);
			outdata[15] = AesTransform.iSBox[(int)((byte)num6)] ^ (byte)ekey[num++];
		}

		private const int Nb = 4;

		private uint[] expandedKey;

		private int Nk;

		private int Nr;

		private static readonly uint[] Rcon = new uint[]
		{
			0U, 16777216U, 33554432U, 67108864U, 134217728U, 268435456U, 536870912U, 1073741824U, 2147483648U, 452984832U,
			905969664U, 1811939328U, 3623878656U, 2868903936U, 1291845632U, 2583691264U, 788529152U, 1577058304U, 3154116608U, 1660944384U,
			3321888768U, 2533359616U, 889192448U, 1778384896U, 3556769792U, 3003121664U, 2097152000U, 4194304000U, 4009754624U, 3305111552U
		};

		private static readonly byte[] SBox = new byte[]
		{
			99, 124, 119, 123, 242, 107, 111, 197, 48, 1,
			103, 43, 254, 215, 171, 118, 202, 130, 201, 125,
			250, 89, 71, 240, 173, 212, 162, 175, 156, 164,
			114, 192, 183, 253, 147, 38, 54, 63, 247, 204,
			52, 165, 229, 241, 113, 216, 49, 21, 4, 199,
			35, 195, 24, 150, 5, 154, 7, 18, 128, 226,
			235, 39, 178, 117, 9, 131, 44, 26, 27, 110,
			90, 160, 82, 59, 214, 179, 41, 227, 47, 132,
			83, 209, 0, 237, 32, 252, 177, 91, 106, 203,
			190, 57, 74, 76, 88, 207, 208, 239, 170, 251,
			67, 77, 51, 133, 69, 249, 2, 127, 80, 60,
			159, 168, 81, 163, 64, 143, 146, 157, 56, 245,
			188, 182, 218, 33, 16, byte.MaxValue, 243, 210, 205, 12,
			19, 236, 95, 151, 68, 23, 196, 167, 126, 61,
			100, 93, 25, 115, 96, 129, 79, 220, 34, 42,
			144, 136, 70, 238, 184, 20, 222, 94, 11, 219,
			224, 50, 58, 10, 73, 6, 36, 92, 194, 211,
			172, 98, 145, 149, 228, 121, 231, 200, 55, 109,
			141, 213, 78, 169, 108, 86, 244, 234, 101, 122,
			174, 8, 186, 120, 37, 46, 28, 166, 180, 198,
			232, 221, 116, 31, 75, 189, 139, 138, 112, 62,
			181, 102, 72, 3, 246, 14, 97, 53, 87, 185,
			134, 193, 29, 158, 225, 248, 152, 17, 105, 217,
			142, 148, 155, 30, 135, 233, 206, 85, 40, 223,
			140, 161, 137, 13, 191, 230, 66, 104, 65, 153,
			45, 15, 176, 84, 187, 22
		};

		private static readonly byte[] iSBox = new byte[]
		{
			82, 9, 106, 213, 48, 54, 165, 56, 191, 64,
			163, 158, 129, 243, 215, 251, 124, 227, 57, 130,
			155, 47, byte.MaxValue, 135, 52, 142, 67, 68, 196, 222,
			233, 203, 84, 123, 148, 50, 166, 194, 35, 61,
			238, 76, 149, 11, 66, 250, 195, 78, 8, 46,
			161, 102, 40, 217, 36, 178, 118, 91, 162, 73,
			109, 139, 209, 37, 114, 248, 246, 100, 134, 104,
			152, 22, 212, 164, 92, 204, 93, 101, 182, 146,
			108, 112, 72, 80, 253, 237, 185, 218, 94, 21,
			70, 87, 167, 141, 157, 132, 144, 216, 171, 0,
			140, 188, 211, 10, 247, 228, 88, 5, 184, 179,
			69, 6, 208, 44, 30, 143, 202, 63, 15, 2,
			193, 175, 189, 3, 1, 19, 138, 107, 58, 145,
			17, 65, 79, 103, 220, 234, 151, 242, 207, 206,
			240, 180, 230, 115, 150, 172, 116, 34, 231, 173,
			53, 133, 226, 249, 55, 232, 28, 117, 223, 110,
			71, 241, 26, 113, 29, 41, 197, 137, 111, 183,
			98, 14, 170, 24, 190, 27, 252, 86, 62, 75,
			198, 210, 121, 32, 154, 219, 192, 254, 120, 205,
			90, 244, 31, 221, 168, 51, 136, 7, 199, 49,
			177, 18, 16, 89, 39, 128, 236, 95, 96, 81,
			127, 169, 25, 181, 74, 13, 45, 229, 122, 159,
			147, 201, 156, 239, 160, 224, 59, 77, 174, 42,
			245, 176, 200, 235, 187, 60, 131, 83, 153, 97,
			23, 43, 4, 126, 186, 119, 214, 38, 225, 105,
			20, 99, 85, 33, 12, 125
		};

		private static readonly uint[] T0 = new uint[]
		{
			3328402341U, 4168907908U, 4000806809U, 4135287693U, 4294111757U, 3597364157U, 3731845041U, 2445657428U, 1613770832U, 33620227U,
			3462883241U, 1445669757U, 3892248089U, 3050821474U, 1303096294U, 3967186586U, 2412431941U, 528646813U, 2311702848U, 4202528135U,
			4026202645U, 2992200171U, 2387036105U, 4226871307U, 1101901292U, 3017069671U, 1604494077U, 1169141738U, 597466303U, 1403299063U,
			3832705686U, 2613100635U, 1974974402U, 3791519004U, 1033081774U, 1277568618U, 1815492186U, 2118074177U, 4126668546U, 2211236943U,
			1748251740U, 1369810420U, 3521504564U, 4193382664U, 3799085459U, 2883115123U, 1647391059U, 706024767U, 134480908U, 2512897874U,
			1176707941U, 2646852446U, 806885416U, 932615841U, 168101135U, 798661301U, 235341577U, 605164086U, 461406363U, 3756188221U,
			3454790438U, 1311188841U, 2142417613U, 3933566367U, 302582043U, 495158174U, 1479289972U, 874125870U, 907746093U, 3698224818U,
			3025820398U, 1537253627U, 2756858614U, 1983593293U, 3084310113U, 2108928974U, 1378429307U, 3722699582U, 1580150641U, 327451799U,
			2790478837U, 3117535592U, 0U, 3253595436U, 1075847264U, 3825007647U, 2041688520U, 3059440621U, 3563743934U, 2378943302U,
			1740553945U, 1916352843U, 2487896798U, 2555137236U, 2958579944U, 2244988746U, 3151024235U, 3320835882U, 1336584933U, 3992714006U,
			2252555205U, 2588757463U, 1714631509U, 293963156U, 2319795663U, 3925473552U, 67240454U, 4269768577U, 2689618160U, 2017213508U,
			631218106U, 1269344483U, 2723238387U, 1571005438U, 2151694528U, 93294474U, 1066570413U, 563977660U, 1882732616U, 4059428100U,
			1673313503U, 2008463041U, 2950355573U, 1109467491U, 537923632U, 3858759450U, 4260623118U, 3218264685U, 2177748300U, 403442708U,
			638784309U, 3287084079U, 3193921505U, 899127202U, 2286175436U, 773265209U, 2479146071U, 1437050866U, 4236148354U, 2050833735U,
			3362022572U, 3126681063U, 840505643U, 3866325909U, 3227541664U, 427917720U, 2655997905U, 2749160575U, 1143087718U, 1412049534U,
			999329963U, 193497219U, 2353415882U, 3354324521U, 1807268051U, 672404540U, 2816401017U, 3160301282U, 369822493U, 2916866934U,
			3688947771U, 1681011286U, 1949973070U, 336202270U, 2454276571U, 201721354U, 1210328172U, 3093060836U, 2680341085U, 3184776046U,
			1135389935U, 3294782118U, 965841320U, 831886756U, 3554993207U, 4068047243U, 3588745010U, 2345191491U, 1849112409U, 3664604599U,
			26054028U, 2983581028U, 2622377682U, 1235855840U, 3630984372U, 2891339514U, 4092916743U, 3488279077U, 3395642799U, 4101667470U,
			1202630377U, 268961816U, 1874508501U, 4034427016U, 1243948399U, 1546530418U, 941366308U, 1470539505U, 1941222599U, 2546386513U,
			3421038627U, 2715671932U, 3899946140U, 1042226977U, 2521517021U, 1639824860U, 227249030U, 260737669U, 3765465232U, 2084453954U,
			1907733956U, 3429263018U, 2420656344U, 100860677U, 4160157185U, 470683154U, 3261161891U, 1781871967U, 2924959737U, 1773779408U,
			394692241U, 2579611992U, 974986535U, 664706745U, 3655459128U, 3958962195U, 731420851U, 571543859U, 3530123707U, 2849626480U,
			126783113U, 865375399U, 765172662U, 1008606754U, 361203602U, 3387549984U, 2278477385U, 2857719295U, 1344809080U, 2782912378U,
			59542671U, 1503764984U, 160008576U, 437062935U, 1707065306U, 3622233649U, 2218934982U, 3496503480U, 2185314755U, 697932208U,
			1512910199U, 504303377U, 2075177163U, 2824099068U, 1841019862U, 739644986U
		};

		private static readonly uint[] T1 = new uint[]
		{
			2781242211U, 2230877308U, 2582542199U, 2381740923U, 234877682U, 3184946027U, 2984144751U, 1418839493U, 1348481072U, 50462977U,
			2848876391U, 2102799147U, 434634494U, 1656084439U, 3863849899U, 2599188086U, 1167051466U, 2636087938U, 1082771913U, 2281340285U,
			368048890U, 3954334041U, 3381544775U, 201060592U, 3963727277U, 1739838676U, 4250903202U, 3930435503U, 3206782108U, 4149453988U,
			2531553906U, 1536934080U, 3262494647U, 484572669U, 2923271059U, 1783375398U, 1517041206U, 1098792767U, 49674231U, 1334037708U,
			1550332980U, 4098991525U, 886171109U, 150598129U, 2481090929U, 1940642008U, 1398944049U, 1059722517U, 201851908U, 1385547719U,
			1699095331U, 1587397571U, 674240536U, 2704774806U, 252314885U, 3039795866U, 151914247U, 908333586U, 2602270848U, 1038082786U,
			651029483U, 1766729511U, 3447698098U, 2682942837U, 454166793U, 2652734339U, 1951935532U, 775166490U, 758520603U, 3000790638U,
			4004797018U, 4217086112U, 4137964114U, 1299594043U, 1639438038U, 3464344499U, 2068982057U, 1054729187U, 1901997871U, 2534638724U,
			4121318227U, 1757008337U, 0U, 750906861U, 1614815264U, 535035132U, 3363418545U, 3988151131U, 3201591914U, 1183697867U,
			3647454910U, 1265776953U, 3734260298U, 3566750796U, 3903871064U, 1250283471U, 1807470800U, 717615087U, 3847203498U, 384695291U,
			3313910595U, 3617213773U, 1432761139U, 2484176261U, 3481945413U, 283769337U, 100925954U, 2180939647U, 4037038160U, 1148730428U,
			3123027871U, 3813386408U, 4087501137U, 4267549603U, 3229630528U, 2315620239U, 2906624658U, 3156319645U, 1215313976U, 82966005U,
			3747855548U, 3245848246U, 1974459098U, 1665278241U, 807407632U, 451280895U, 251524083U, 1841287890U, 1283575245U, 337120268U,
			891687699U, 801369324U, 3787349855U, 2721421207U, 3431482436U, 959321879U, 1469301956U, 4065699751U, 2197585534U, 1199193405U,
			2898814052U, 3887750493U, 724703513U, 2514908019U, 2696962144U, 2551808385U, 3516813135U, 2141445340U, 1715741218U, 2119445034U,
			2872807568U, 2198571144U, 3398190662U, 700968686U, 3547052216U, 1009259540U, 2041044702U, 3803995742U, 487983883U, 1991105499U,
			1004265696U, 1449407026U, 1316239930U, 504629770U, 3683797321U, 168560134U, 1816667172U, 3837287516U, 1570751170U, 1857934291U,
			4014189740U, 2797888098U, 2822345105U, 2754712981U, 936633572U, 2347923833U, 852879335U, 1133234376U, 1500395319U, 3084545389U,
			2348912013U, 1689376213U, 3533459022U, 3762923945U, 3034082412U, 4205598294U, 133428468U, 634383082U, 2949277029U, 2398386810U,
			3913789102U, 403703816U, 3580869306U, 2297460856U, 1867130149U, 1918643758U, 607656988U, 4049053350U, 3346248884U, 1368901318U,
			600565992U, 2090982877U, 2632479860U, 557719327U, 3717614411U, 3697393085U, 2249034635U, 2232388234U, 2430627952U, 1115438654U,
			3295786421U, 2865522278U, 3633334344U, 84280067U, 33027830U, 303828494U, 2747425121U, 1600795957U, 4188952407U, 3496589753U,
			2434238086U, 1486471617U, 658119965U, 3106381470U, 953803233U, 334231800U, 3005978776U, 857870609U, 3151128937U, 1890179545U,
			2298973838U, 2805175444U, 3056442267U, 574365214U, 2450884487U, 550103529U, 1233637070U, 4289353045U, 2018519080U, 2057691103U,
			2399374476U, 4166623649U, 2148108681U, 387583245U, 3664101311U, 836232934U, 3330556482U, 3100665960U, 3280093505U, 2955516313U,
			2002398509U, 287182607U, 3413881008U, 4238890068U, 3597515707U, 975967766U
		};

		private static readonly uint[] T2 = new uint[]
		{
			1671808611U, 2089089148U, 2006576759U, 2072901243U, 4061003762U, 1807603307U, 1873927791U, 3310653893U, 810573872U, 16974337U,
			1739181671U, 729634347U, 4263110654U, 3613570519U, 2883997099U, 1989864566U, 3393556426U, 2191335298U, 3376449993U, 2106063485U,
			4195741690U, 1508618841U, 1204391495U, 4027317232U, 2917941677U, 3563566036U, 2734514082U, 2951366063U, 2629772188U, 2767672228U,
			1922491506U, 3227229120U, 3082974647U, 4246528509U, 2477669779U, 644500518U, 911895606U, 1061256767U, 4144166391U, 3427763148U,
			878471220U, 2784252325U, 3845444069U, 4043897329U, 1905517169U, 3631459288U, 827548209U, 356461077U, 67897348U, 3344078279U,
			593839651U, 3277757891U, 405286936U, 2527147926U, 84871685U, 2595565466U, 118033927U, 305538066U, 2157648768U, 3795705826U,
			3945188843U, 661212711U, 2999812018U, 1973414517U, 152769033U, 2208177539U, 745822252U, 439235610U, 455947803U, 1857215598U,
			1525593178U, 2700827552U, 1391895634U, 994932283U, 3596728278U, 3016654259U, 695947817U, 3812548067U, 795958831U, 2224493444U,
			1408607827U, 3513301457U, 0U, 3979133421U, 543178784U, 4229948412U, 2982705585U, 1542305371U, 1790891114U, 3410398667U,
			3201918910U, 961245753U, 1256100938U, 1289001036U, 1491644504U, 3477767631U, 3496721360U, 4012557807U, 2867154858U, 4212583931U,
			1137018435U, 1305975373U, 861234739U, 2241073541U, 1171229253U, 4178635257U, 33948674U, 2139225727U, 1357946960U, 1011120188U,
			2679776671U, 2833468328U, 1374921297U, 2751356323U, 1086357568U, 2408187279U, 2460827538U, 2646352285U, 944271416U, 4110742005U,
			3168756668U, 3066132406U, 3665145818U, 560153121U, 271589392U, 4279952895U, 4077846003U, 3530407890U, 3444343245U, 202643468U,
			322250259U, 3962553324U, 1608629855U, 2543990167U, 1154254916U, 389623319U, 3294073796U, 2817676711U, 2122513534U, 1028094525U,
			1689045092U, 1575467613U, 422261273U, 1939203699U, 1621147744U, 2174228865U, 1339137615U, 3699352540U, 577127458U, 712922154U,
			2427141008U, 2290289544U, 1187679302U, 3995715566U, 3100863416U, 339486740U, 3732514782U, 1591917662U, 186455563U, 3681988059U,
			3762019296U, 844522546U, 978220090U, 169743370U, 1239126601U, 101321734U, 611076132U, 1558493276U, 3260915650U, 3547250131U,
			2901361580U, 1655096418U, 2443721105U, 2510565781U, 3828863972U, 2039214713U, 3878868455U, 3359869896U, 928607799U, 1840765549U,
			2374762893U, 3580146133U, 1322425422U, 2850048425U, 1823791212U, 1459268694U, 4094161908U, 3928346602U, 1706019429U, 2056189050U,
			2934523822U, 135794696U, 3134549946U, 2022240376U, 628050469U, 779246638U, 472135708U, 2800834470U, 3032970164U, 3327236038U,
			3894660072U, 3715932637U, 1956440180U, 522272287U, 1272813131U, 3185336765U, 2340818315U, 2323976074U, 1888542832U, 1044544574U,
			3049550261U, 1722469478U, 1222152264U, 50660867U, 4127324150U, 236067854U, 1638122081U, 895445557U, 1475980887U, 3117443513U,
			2257655686U, 3243809217U, 489110045U, 2662934430U, 3778599393U, 4162055160U, 2561878936U, 288563729U, 1773916777U, 3648039385U,
			2391345038U, 2493985684U, 2612407707U, 505560094U, 2274497927U, 3911240169U, 3460925390U, 1442818645U, 678973480U, 3749357023U,
			2358182796U, 2717407649U, 2306869641U, 219617805U, 3218761151U, 3862026214U, 1120306242U, 1756942440U, 1103331905U, 2578459033U,
			762796589U, 252780047U, 2966125488U, 1425844308U, 3151392187U, 372911126U
		};

		private static readonly uint[] T3 = new uint[]
		{
			1667474886U, 2088535288U, 2004326894U, 2071694838U, 4075949567U, 1802223062U, 1869591006U, 3318043793U, 808472672U, 16843522U,
			1734846926U, 724270422U, 4278065639U, 3621216949U, 2880169549U, 1987484396U, 3402253711U, 2189597983U, 3385409673U, 2105378810U,
			4210693615U, 1499065266U, 1195886990U, 4042263547U, 2913856577U, 3570689971U, 2728590687U, 2947541573U, 2627518243U, 2762274643U,
			1920112356U, 3233831835U, 3082273397U, 4261223649U, 2475929149U, 640051788U, 909531756U, 1061110142U, 4160160501U, 3435941763U,
			875846760U, 2779116625U, 3857003729U, 4059105529U, 1903268834U, 3638064043U, 825316194U, 353713962U, 67374088U, 3351728789U,
			589522246U, 3284360861U, 404236336U, 2526454071U, 84217610U, 2593830191U, 117901582U, 303183396U, 2155911963U, 3806477791U,
			3958056653U, 656894286U, 2998062463U, 1970642922U, 151591698U, 2206440989U, 741110872U, 437923380U, 454765878U, 1852748508U,
			1515908788U, 2694904667U, 1381168804U, 993742198U, 3604373943U, 3014905469U, 690584402U, 3823320797U, 791638366U, 2223281939U,
			1398011302U, 3520161977U, 0U, 3991743681U, 538992704U, 4244381667U, 2981218425U, 1532751286U, 1785380564U, 3419096717U,
			3200178535U, 960056178U, 1246420628U, 1280103576U, 1482221744U, 3486468741U, 3503319995U, 4025428677U, 2863326543U, 4227536621U,
			1128514950U, 1296947098U, 859002214U, 2240123921U, 1162203018U, 4193849577U, 33687044U, 2139062782U, 1347481760U, 1010582648U,
			2678045221U, 2829640523U, 1364325282U, 2745433693U, 1077985408U, 2408548869U, 2459086143U, 2644360225U, 943212656U, 4126475505U,
			3166494563U, 3065430391U, 3671750063U, 555836226U, 269496352U, 4294908645U, 4092792573U, 3537006015U, 3452783745U, 202118168U,
			320025894U, 3974901699U, 1600119230U, 2543297077U, 1145359496U, 387397934U, 3301201811U, 2812801621U, 2122220284U, 1027426170U,
			1684319432U, 1566435258U, 421079858U, 1936954854U, 1616945344U, 2172753945U, 1330631070U, 3705438115U, 572679748U, 707427924U,
			2425400123U, 2290647819U, 1179044492U, 4008585671U, 3099120491U, 336870440U, 3739122087U, 1583276732U, 185277718U, 3688593069U,
			3772791771U, 842159716U, 976899700U, 168435220U, 1229577106U, 101059084U, 606366792U, 1549591736U, 3267517855U, 3553849021U,
			2897014595U, 1650632388U, 2442242105U, 2509612081U, 3840161747U, 2038008818U, 3890688725U, 3368567691U, 926374254U, 1835907034U,
			2374863873U, 3587531953U, 1313788572U, 2846482505U, 1819063512U, 1448540844U, 4109633523U, 3941213647U, 1701162954U, 2054852340U,
			2930698567U, 134748176U, 3132806511U, 2021165296U, 623210314U, 774795868U, 471606328U, 2795958615U, 3031746419U, 3334885783U,
			3907527627U, 3722280097U, 1953799400U, 522133822U, 1263263126U, 3183336545U, 2341176845U, 2324333839U, 1886425312U, 1044267644U,
			3048588401U, 1718004428U, 1212733584U, 50529542U, 4143317495U, 235803164U, 1633788866U, 892690282U, 1465383342U, 3115962473U,
			2256965911U, 3250673817U, 488449850U, 2661202215U, 3789633753U, 4177007595U, 2560144171U, 286339874U, 1768537042U, 3654906025U,
			2391705863U, 2492770099U, 2610673197U, 505291324U, 2273808917U, 3924369609U, 3469625735U, 1431699370U, 673740880U, 3755965093U,
			2358021891U, 2711746649U, 2307489801U, 218961690U, 3217021541U, 3873845719U, 1111672452U, 1751693520U, 1094828930U, 2576986153U,
			757954394U, 252645662U, 2964376443U, 1414855848U, 3149649517U, 370555436U
		};

		private static readonly uint[] iT0 = new uint[]
		{
			1374988112U, 2118214995U, 437757123U, 975658646U, 1001089995U, 530400753U, 2902087851U, 1273168787U, 540080725U, 2910219766U,
			2295101073U, 4110568485U, 1340463100U, 3307916247U, 641025152U, 3043140495U, 3736164937U, 632953703U, 1172967064U, 1576976609U,
			3274667266U, 2169303058U, 2370213795U, 1809054150U, 59727847U, 361929877U, 3211623147U, 2505202138U, 3569255213U, 1484005843U,
			1239443753U, 2395588676U, 1975683434U, 4102977912U, 2572697195U, 666464733U, 3202437046U, 4035489047U, 3374361702U, 2110667444U,
			1675577880U, 3843699074U, 2538681184U, 1649639237U, 2976151520U, 3144396420U, 4269907996U, 4178062228U, 1883793496U, 2403728665U,
			2497604743U, 1383856311U, 2876494627U, 1917518562U, 3810496343U, 1716890410U, 3001755655U, 800440835U, 2261089178U, 3543599269U,
			807962610U, 599762354U, 33778362U, 3977675356U, 2328828971U, 2809771154U, 4077384432U, 1315562145U, 1708848333U, 101039829U,
			3509871135U, 3299278474U, 875451293U, 2733856160U, 92987698U, 2767645557U, 193195065U, 1080094634U, 1584504582U, 3178106961U,
			1042385657U, 2531067453U, 3711829422U, 1306967366U, 2438237621U, 1908694277U, 67556463U, 1615861247U, 429456164U, 3602770327U,
			2302690252U, 1742315127U, 2968011453U, 126454664U, 3877198648U, 2043211483U, 2709260871U, 2084704233U, 4169408201U, 0U,
			159417987U, 841739592U, 504459436U, 1817866830U, 4245618683U, 260388950U, 1034867998U, 908933415U, 168810852U, 1750902305U,
			2606453969U, 607530554U, 202008497U, 2472011535U, 3035535058U, 463180190U, 2160117071U, 1641816226U, 1517767529U, 470948374U,
			3801332234U, 3231722213U, 1008918595U, 303765277U, 235474187U, 4069246893U, 766945465U, 337553864U, 1475418501U, 2943682380U,
			4003061179U, 2743034109U, 4144047775U, 1551037884U, 1147550661U, 1543208500U, 2336434550U, 3408119516U, 3069049960U, 3102011747U,
			3610369226U, 1113818384U, 328671808U, 2227573024U, 2236228733U, 3535486456U, 2935566865U, 3341394285U, 496906059U, 3702665459U,
			226906860U, 2009195472U, 733156972U, 2842737049U, 294930682U, 1206477858U, 2835123396U, 2700099354U, 1451044056U, 573804783U,
			2269728455U, 3644379585U, 2362090238U, 2564033334U, 2801107407U, 2776292904U, 3669462566U, 1068351396U, 742039012U, 1350078989U,
			1784663195U, 1417561698U, 4136440770U, 2430122216U, 775550814U, 2193862645U, 2673705150U, 1775276924U, 1876241833U, 3475313331U,
			3366754619U, 270040487U, 3902563182U, 3678124923U, 3441850377U, 1851332852U, 3969562369U, 2203032232U, 3868552805U, 2868897406U,
			566021896U, 4011190502U, 3135740889U, 1248802510U, 3936291284U, 699432150U, 832877231U, 708780849U, 3332740144U, 899835584U,
			1951317047U, 4236429990U, 3767586992U, 866637845U, 4043610186U, 1106041591U, 2144161806U, 395441711U, 1984812685U, 1139781709U,
			3433712980U, 3835036895U, 2664543715U, 1282050075U, 3240894392U, 1181045119U, 2640243204U, 25965917U, 4203181171U, 4211818798U,
			3009879386U, 2463879762U, 3910161971U, 1842759443U, 2597806476U, 933301370U, 1509430414U, 3943906441U, 3467192302U, 3076639029U,
			3776767469U, 2051518780U, 2631065433U, 1441952575U, 404016761U, 1942435775U, 1408749034U, 1610459739U, 3745345300U, 2017778566U,
			3400528769U, 3110650942U, 941896748U, 3265478751U, 371049330U, 3168937228U, 675039627U, 4279080257U, 967311729U, 135050206U,
			3635733660U, 1683407248U, 2076935265U, 3576870512U, 1215061108U, 3501741890U
		};

		private static readonly uint[] iT1 = new uint[]
		{
			1347548327U, 1400783205U, 3273267108U, 2520393566U, 3409685355U, 4045380933U, 2880240216U, 2471224067U, 1428173050U, 4138563181U,
			2441661558U, 636813900U, 4233094615U, 3620022987U, 2149987652U, 2411029155U, 1239331162U, 1730525723U, 2554718734U, 3781033664U,
			46346101U, 310463728U, 2743944855U, 3328955385U, 3875770207U, 2501218972U, 3955191162U, 3667219033U, 768917123U, 3545789473U,
			692707433U, 1150208456U, 1786102409U, 2029293177U, 1805211710U, 3710368113U, 3065962831U, 401639597U, 1724457132U, 3028143674U,
			409198410U, 2196052529U, 1620529459U, 1164071807U, 3769721975U, 2226875310U, 486441376U, 2499348523U, 1483753576U, 428819965U,
			2274680428U, 3075636216U, 598438867U, 3799141122U, 1474502543U, 711349675U, 129166120U, 53458370U, 2592523643U, 2782082824U,
			4063242375U, 2988687269U, 3120694122U, 1559041666U, 730517276U, 2460449204U, 4042459122U, 2706270690U, 3446004468U, 3573941694U,
			533804130U, 2328143614U, 2637442643U, 2695033685U, 839224033U, 1973745387U, 957055980U, 2856345839U, 106852767U, 1371368976U,
			4181598602U, 1033297158U, 2933734917U, 1179510461U, 3046200461U, 91341917U, 1862534868U, 4284502037U, 605657339U, 2547432937U,
			3431546947U, 2003294622U, 3182487618U, 2282195339U, 954669403U, 3682191598U, 1201765386U, 3917234703U, 3388507166U, 0U,
			2198438022U, 1211247597U, 2887651696U, 1315723890U, 4227665663U, 1443857720U, 507358933U, 657861945U, 1678381017U, 560487590U,
			3516619604U, 975451694U, 2970356327U, 261314535U, 3535072918U, 2652609425U, 1333838021U, 2724322336U, 1767536459U, 370938394U,
			182621114U, 3854606378U, 1128014560U, 487725847U, 185469197U, 2918353863U, 3106780840U, 3356761769U, 2237133081U, 1286567175U,
			3152976349U, 4255350624U, 2683765030U, 3160175349U, 3309594171U, 878443390U, 1988838185U, 3704300486U, 1756818940U, 1673061617U,
			3403100636U, 272786309U, 1075025698U, 545572369U, 2105887268U, 4174560061U, 296679730U, 1841768865U, 1260232239U, 4091327024U,
			3960309330U, 3497509347U, 1814803222U, 2578018489U, 4195456072U, 575138148U, 3299409036U, 446754879U, 3629546796U, 4011996048U,
			3347532110U, 3252238545U, 4270639778U, 915985419U, 3483825537U, 681933534U, 651868046U, 2755636671U, 3828103837U, 223377554U,
			2607439820U, 1649704518U, 3270937875U, 3901806776U, 1580087799U, 4118987695U, 3198115200U, 2087309459U, 2842678573U, 3016697106U,
			1003007129U, 2802849917U, 1860738147U, 2077965243U, 164439672U, 4100872472U, 32283319U, 2827177882U, 1709610350U, 2125135846U,
			136428751U, 3874428392U, 3652904859U, 3460984630U, 3572145929U, 3593056380U, 2939266226U, 824852259U, 818324884U, 3224740454U,
			930369212U, 2801566410U, 2967507152U, 355706840U, 1257309336U, 4148292826U, 243256656U, 790073846U, 2373340630U, 1296297904U,
			1422699085U, 3756299780U, 3818836405U, 457992840U, 3099667487U, 2135319889U, 77422314U, 1560382517U, 1945798516U, 788204353U,
			1521706781U, 1385356242U, 870912086U, 325965383U, 2358957921U, 2050466060U, 2388260884U, 2313884476U, 4006521127U, 901210569U,
			3990953189U, 1014646705U, 1503449823U, 1062597235U, 2031621326U, 3212035895U, 3931371469U, 1533017514U, 350174575U, 2256028891U,
			2177544179U, 1052338372U, 741876788U, 1606591296U, 1914052035U, 213705253U, 2334669897U, 1107234197U, 1899603969U, 3725069491U,
			2631447780U, 2422494913U, 1635502980U, 1893020342U, 1950903388U, 1120974935U
		};

		private static readonly uint[] iT2 = new uint[]
		{
			2807058932U, 1699970625U, 2764249623U, 1586903591U, 1808481195U, 1173430173U, 1487645946U, 59984867U, 4199882800U, 1844882806U,
			1989249228U, 1277555970U, 3623636965U, 3419915562U, 1149249077U, 2744104290U, 1514790577U, 459744698U, 244860394U, 3235995134U,
			1963115311U, 4027744588U, 2544078150U, 4190530515U, 1608975247U, 2627016082U, 2062270317U, 1507497298U, 2200818878U, 567498868U,
			1764313568U, 3359936201U, 2305455554U, 2037970062U, 1047239000U, 1910319033U, 1337376481U, 2904027272U, 2892417312U, 984907214U,
			1243112415U, 830661914U, 861968209U, 2135253587U, 2011214180U, 2927934315U, 2686254721U, 731183368U, 1750626376U, 4246310725U,
			1820824798U, 4172763771U, 3542330227U, 48394827U, 2404901663U, 2871682645U, 671593195U, 3254988725U, 2073724613U, 145085239U,
			2280796200U, 2779915199U, 1790575107U, 2187128086U, 472615631U, 3029510009U, 4075877127U, 3802222185U, 4107101658U, 3201631749U,
			1646252340U, 4270507174U, 1402811438U, 1436590835U, 3778151818U, 3950355702U, 3963161475U, 4020912224U, 2667994737U, 273792366U,
			2331590177U, 104699613U, 95345982U, 3175501286U, 2377486676U, 1560637892U, 3564045318U, 369057872U, 4213447064U, 3919042237U,
			1137477952U, 2658625497U, 1119727848U, 2340947849U, 1530455833U, 4007360968U, 172466556U, 266959938U, 516552836U, 0U,
			2256734592U, 3980931627U, 1890328081U, 1917742170U, 4294704398U, 945164165U, 3575528878U, 958871085U, 3647212047U, 2787207260U,
			1423022939U, 775562294U, 1739656202U, 3876557655U, 2530391278U, 2443058075U, 3310321856U, 547512796U, 1265195639U, 437656594U,
			3121275539U, 719700128U, 3762502690U, 387781147U, 218828297U, 3350065803U, 2830708150U, 2848461854U, 428169201U, 122466165U,
			3720081049U, 1627235199U, 648017665U, 4122762354U, 1002783846U, 2117360635U, 695634755U, 3336358691U, 4234721005U, 4049844452U,
			3704280881U, 2232435299U, 574624663U, 287343814U, 612205898U, 1039717051U, 840019705U, 2708326185U, 793451934U, 821288114U,
			1391201670U, 3822090177U, 376187827U, 3113855344U, 1224348052U, 1679968233U, 2361698556U, 1058709744U, 752375421U, 2431590963U,
			1321699145U, 3519142200U, 2734591178U, 188127444U, 2177869557U, 3727205754U, 2384911031U, 3215212461U, 2648976442U, 2450346104U,
			3432737375U, 1180849278U, 331544205U, 3102249176U, 4150144569U, 2952102595U, 2159976285U, 2474404304U, 766078933U, 313773861U,
			2570832044U, 2108100632U, 1668212892U, 3145456443U, 2013908262U, 418672217U, 3070356634U, 2594734927U, 1852171925U, 3867060991U,
			3473416636U, 3907448597U, 2614737639U, 919489135U, 164948639U, 2094410160U, 2997825956U, 590424639U, 2486224549U, 1723872674U,
			3157750862U, 3399941250U, 3501252752U, 3625268135U, 2555048196U, 3673637356U, 1343127501U, 4130281361U, 3599595085U, 2957853679U,
			1297403050U, 81781910U, 3051593425U, 2283490410U, 532201772U, 1367295589U, 3926170974U, 895287692U, 1953757831U, 1093597963U,
			492483431U, 3528626907U, 1446242576U, 1192455638U, 1636604631U, 209336225U, 344873464U, 1015671571U, 669961897U, 3375740769U,
			3857572124U, 2973530695U, 3747192018U, 1933530610U, 3464042516U, 935293895U, 3454686199U, 2858115069U, 1863638845U, 3683022916U,
			4085369519U, 3292445032U, 875313188U, 1080017571U, 3279033885U, 621591778U, 1233856572U, 2504130317U, 24197544U, 3017672716U,
			3835484340U, 3247465558U, 2220981195U, 3060847922U, 1551124588U, 1463996600U
		};

		private static readonly uint[] iT3 = new uint[]
		{
			4104605777U, 1097159550U, 396673818U, 660510266U, 2875968315U, 2638606623U, 4200115116U, 3808662347U, 821712160U, 1986918061U,
			3430322568U, 38544885U, 3856137295U, 718002117U, 893681702U, 1654886325U, 2975484382U, 3122358053U, 3926825029U, 4274053469U,
			796197571U, 1290801793U, 1184342925U, 3556361835U, 2405426947U, 2459735317U, 1836772287U, 1381620373U, 3196267988U, 1948373848U,
			3764988233U, 3385345166U, 3263785589U, 2390325492U, 1480485785U, 3111247143U, 3780097726U, 2293045232U, 548169417U, 3459953789U,
			3746175075U, 439452389U, 1362321559U, 1400849762U, 1685577905U, 1806599355U, 2174754046U, 137073913U, 1214797936U, 1174215055U,
			3731654548U, 2079897426U, 1943217067U, 1258480242U, 529487843U, 1437280870U, 3945269170U, 3049390895U, 3313212038U, 923313619U,
			679998000U, 3215307299U, 57326082U, 377642221U, 3474729866U, 2041877159U, 133361907U, 1776460110U, 3673476453U, 96392454U,
			878845905U, 2801699524U, 777231668U, 4082475170U, 2330014213U, 4142626212U, 2213296395U, 1626319424U, 1906247262U, 1846563261U,
			562755902U, 3708173718U, 1040559837U, 3871163981U, 1418573201U, 3294430577U, 114585348U, 1343618912U, 2566595609U, 3186202582U,
			1078185097U, 3651041127U, 3896688048U, 2307622919U, 425408743U, 3371096953U, 2081048481U, 1108339068U, 2216610296U, 0U,
			2156299017U, 736970802U, 292596766U, 1517440620U, 251657213U, 2235061775U, 2933202493U, 758720310U, 265905162U, 1554391400U,
			1532285339U, 908999204U, 174567692U, 1474760595U, 4002861748U, 2610011675U, 3234156416U, 3693126241U, 2001430874U, 303699484U,
			2478443234U, 2687165888U, 585122620U, 454499602U, 151849742U, 2345119218U, 3064510765U, 514443284U, 4044981591U, 1963412655U,
			2581445614U, 2137062819U, 19308535U, 1928707164U, 1715193156U, 4219352155U, 1126790795U, 600235211U, 3992742070U, 3841024952U,
			836553431U, 1669664834U, 2535604243U, 3323011204U, 1243905413U, 3141400786U, 4180808110U, 698445255U, 2653899549U, 2989552604U,
			2253581325U, 3252932727U, 3004591147U, 1891211689U, 2487810577U, 3915653703U, 4237083816U, 4030667424U, 2100090966U, 865136418U,
			1229899655U, 953270745U, 3399679628U, 3557504664U, 4118925222U, 2061379749U, 3079546586U, 2915017791U, 983426092U, 2022837584U,
			1607244650U, 2118541908U, 2366882550U, 3635996816U, 972512814U, 3283088770U, 1568718495U, 3499326569U, 3576539503U, 621982671U,
			2895723464U, 410887952U, 2623762152U, 1002142683U, 645401037U, 1494807662U, 2595684844U, 1335535747U, 2507040230U, 4293295786U,
			3167684641U, 367585007U, 3885750714U, 1865862730U, 2668221674U, 2960971305U, 2763173681U, 1059270954U, 2777952454U, 2724642869U,
			1320957812U, 2194319100U, 2429595872U, 2815956275U, 77089521U, 3973773121U, 3444575871U, 2448830231U, 1305906550U, 4021308739U,
			2857194700U, 2516901860U, 3518358430U, 1787304780U, 740276417U, 1699839814U, 1592394909U, 2352307457U, 2272556026U, 188821243U,
			1729977011U, 3687994002U, 274084841U, 3594982253U, 3613494426U, 2701949495U, 4162096729U, 322734571U, 2837966542U, 1640576439U,
			484830689U, 1202797690U, 3537852828U, 4067639125U, 349075736U, 3342319475U, 4157467219U, 4255800159U, 1030690015U, 1155237496U,
			2951971274U, 1757691577U, 607398968U, 2738905026U, 499347990U, 3794078908U, 1011452712U, 227885567U, 2818666809U, 213114376U,
			3034881240U, 1455525988U, 3414450555U, 850817237U, 1817998408U, 3092726480U
		};
	}
}
