using System;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	internal class DESTransform : SymmetricTransform
	{
		internal DESTransform(SymmetricAlgorithm symmAlgo, bool encryption, byte[] key, byte[] iv)
			: base(symmAlgo, encryption, iv)
		{
			byte[] array = null;
			if (key == null)
			{
				key = DESTransform.GetStrongKey();
				array = key;
			}
			if (DES.IsWeakKey(key) || DES.IsSemiWeakKey(key))
			{
				string text = Locale.GetText("This is a known weak, or semi-weak, key.");
				throw new CryptographicException(text);
			}
			if (array == null)
			{
				array = (byte[])key.Clone();
			}
			this.keySchedule = new byte[DESTransform.KEY_BYTE_SIZE * 16];
			this.byteBuff = new byte[DESTransform.BLOCK_BYTE_SIZE];
			this.dwordBuff = new uint[DESTransform.BLOCK_BYTE_SIZE / 4];
			this.SetKey(array);
		}

		private uint CipherFunct(uint r, int n)
		{
			uint num = 0U;
			byte[] array = this.keySchedule;
			int num2 = n << 3;
			uint num3 = (r >> 1) | (r << 31);
			num |= DESTransform.spBoxes[(int)((UIntPtr)(0U + (((num3 >> 26) ^ (uint)array[num2++]) & 63U)))];
			num |= DESTransform.spBoxes[(int)((UIntPtr)(64U + (((num3 >> 22) ^ (uint)array[num2++]) & 63U)))];
			num |= DESTransform.spBoxes[(int)((UIntPtr)(128U + (((num3 >> 18) ^ (uint)array[num2++]) & 63U)))];
			num |= DESTransform.spBoxes[(int)((UIntPtr)(192U + (((num3 >> 14) ^ (uint)array[num2++]) & 63U)))];
			num |= DESTransform.spBoxes[(int)((UIntPtr)(256U + (((num3 >> 10) ^ (uint)array[num2++]) & 63U)))];
			num |= DESTransform.spBoxes[(int)((UIntPtr)(320U + (((num3 >> 6) ^ (uint)array[num2++]) & 63U)))];
			num |= DESTransform.spBoxes[(int)((UIntPtr)(384U + (((num3 >> 2) ^ (uint)array[num2++]) & 63U)))];
			num3 = (r << 1) | (r >> 31);
			return num | DESTransform.spBoxes[(int)((UIntPtr)(448U + ((num3 ^ (uint)array[num2]) & 63U)))];
		}

		internal static void Permutation(byte[] input, byte[] output, uint[] permTab, bool preSwap)
		{
			if (preSwap && BitConverter.IsLittleEndian)
			{
				DESTransform.BSwap(input);
			}
			int num = input[0] >> 4 << 1;
			int num2 = 32 + ((int)(input[0] & 15) << 1);
			uint num3 = permTab[num++] | permTab[num2++];
			uint num4 = permTab[num] | permTab[num2];
			int num5 = DESTransform.BLOCK_BYTE_SIZE << 1;
			int i = 2;
			int num6 = 1;
			while (i < num5)
			{
				int num7 = (int)input[num6];
				num = (i << 5) + (num7 >> 4 << 1);
				num2 = (i + 1 << 5) + ((num7 & 15) << 1);
				num3 |= permTab[num++] | permTab[num2++];
				num4 |= permTab[num] | permTab[num2];
				i += 2;
				num6++;
			}
			if (preSwap || !BitConverter.IsLittleEndian)
			{
				output[0] = (byte)num3;
				output[1] = (byte)(num3 >> 8);
				output[2] = (byte)(num3 >> 16);
				output[3] = (byte)(num3 >> 24);
				output[4] = (byte)num4;
				output[5] = (byte)(num4 >> 8);
				output[6] = (byte)(num4 >> 16);
				output[7] = (byte)(num4 >> 24);
			}
			else
			{
				output[0] = (byte)(num3 >> 24);
				output[1] = (byte)(num3 >> 16);
				output[2] = (byte)(num3 >> 8);
				output[3] = (byte)num3;
				output[4] = (byte)(num4 >> 24);
				output[5] = (byte)(num4 >> 16);
				output[6] = (byte)(num4 >> 8);
				output[7] = (byte)num4;
			}
		}

		private static void BSwap(byte[] byteBuff)
		{
			byte b = byteBuff[0];
			byteBuff[0] = byteBuff[3];
			byteBuff[3] = b;
			b = byteBuff[1];
			byteBuff[1] = byteBuff[2];
			byteBuff[2] = b;
			b = byteBuff[4];
			byteBuff[4] = byteBuff[7];
			byteBuff[7] = b;
			b = byteBuff[5];
			byteBuff[5] = byteBuff[6];
			byteBuff[6] = b;
		}

		internal void SetKey(byte[] key)
		{
			Array.Clear(this.keySchedule, 0, this.keySchedule.Length);
			int num = DESTransform.PC1.Length;
			byte[] array = new byte[num];
			byte[] array2 = new byte[num];
			int num2 = 0;
			foreach (byte b in DESTransform.PC1)
			{
				array[num2++] = (byte)((key[b >> 3] >> (int)(7 ^ (b & 7))) & 1);
			}
			for (int j = 0; j < DESTransform.KEY_BYTE_SIZE * 2; j++)
			{
				int num3 = num >> 1;
				int k;
				for (k = 0; k < num3; k++)
				{
					int num4 = k + (int)DESTransform.leftRotTotal[j];
					array2[k] = array[(num4 >= num3) ? (num4 - num3) : num4];
				}
				for (k = num3; k < num; k++)
				{
					int num5 = k + (int)DESTransform.leftRotTotal[j];
					array2[k] = array[(num5 >= num) ? (num5 - num3) : num5];
				}
				int num6 = j * DESTransform.KEY_BYTE_SIZE;
				k = 0;
				foreach (byte b2 in DESTransform.PC2)
				{
					if (array2[(int)b2] != 0)
					{
						byte[] array3 = this.keySchedule;
						int num7 = num6 + k / 6;
						array3[num7] |= (byte)(128 >> k % 6 + 2);
					}
					k++;
				}
			}
		}

		public void ProcessBlock(byte[] input, byte[] output)
		{
			Buffer.BlockCopy(input, 0, this.dwordBuff, 0, DESTransform.BLOCK_BYTE_SIZE);
			if (this.encrypt)
			{
				uint num = this.dwordBuff[0];
				uint num2 = this.dwordBuff[1];
				num ^= this.CipherFunct(num2, 0);
				num2 ^= this.CipherFunct(num, 1);
				num ^= this.CipherFunct(num2, 2);
				num2 ^= this.CipherFunct(num, 3);
				num ^= this.CipherFunct(num2, 4);
				num2 ^= this.CipherFunct(num, 5);
				num ^= this.CipherFunct(num2, 6);
				num2 ^= this.CipherFunct(num, 7);
				num ^= this.CipherFunct(num2, 8);
				num2 ^= this.CipherFunct(num, 9);
				num ^= this.CipherFunct(num2, 10);
				num2 ^= this.CipherFunct(num, 11);
				num ^= this.CipherFunct(num2, 12);
				num2 ^= this.CipherFunct(num, 13);
				num ^= this.CipherFunct(num2, 14);
				num2 ^= this.CipherFunct(num, 15);
				this.dwordBuff[0] = num2;
				this.dwordBuff[1] = num;
			}
			else
			{
				uint num3 = this.dwordBuff[0];
				uint num4 = this.dwordBuff[1];
				num3 ^= this.CipherFunct(num4, 15);
				num4 ^= this.CipherFunct(num3, 14);
				num3 ^= this.CipherFunct(num4, 13);
				num4 ^= this.CipherFunct(num3, 12);
				num3 ^= this.CipherFunct(num4, 11);
				num4 ^= this.CipherFunct(num3, 10);
				num3 ^= this.CipherFunct(num4, 9);
				num4 ^= this.CipherFunct(num3, 8);
				num3 ^= this.CipherFunct(num4, 7);
				num4 ^= this.CipherFunct(num3, 6);
				num3 ^= this.CipherFunct(num4, 5);
				num4 ^= this.CipherFunct(num3, 4);
				num3 ^= this.CipherFunct(num4, 3);
				num4 ^= this.CipherFunct(num3, 2);
				num3 ^= this.CipherFunct(num4, 1);
				num4 ^= this.CipherFunct(num3, 0);
				this.dwordBuff[0] = num4;
				this.dwordBuff[1] = num3;
			}
			Buffer.BlockCopy(this.dwordBuff, 0, output, 0, DESTransform.BLOCK_BYTE_SIZE);
		}

		protected override void ECB(byte[] input, byte[] output)
		{
			DESTransform.Permutation(input, output, DESTransform.ipTab, false);
			this.ProcessBlock(output, this.byteBuff);
			DESTransform.Permutation(this.byteBuff, output, DESTransform.fpTab, true);
		}

		internal static byte[] GetStrongKey()
		{
			byte[] array = KeyBuilder.Key(DESTransform.KEY_BYTE_SIZE);
			while (DES.IsWeakKey(array) || DES.IsSemiWeakKey(array))
			{
				array = KeyBuilder.Key(DESTransform.KEY_BYTE_SIZE);
			}
			return array;
		}

		internal static readonly int KEY_BIT_SIZE = 64;

		internal static readonly int KEY_BYTE_SIZE = DESTransform.KEY_BIT_SIZE / 8;

		internal static readonly int BLOCK_BIT_SIZE = 64;

		internal static readonly int BLOCK_BYTE_SIZE = DESTransform.BLOCK_BIT_SIZE / 8;

		private byte[] keySchedule;

		private byte[] byteBuff;

		private uint[] dwordBuff;

		private static readonly uint[] spBoxes = new uint[]
		{
			8421888U, 0U, 32768U, 8421890U, 8421378U, 33282U, 2U, 32768U, 512U, 8421888U,
			8421890U, 512U, 8389122U, 8421378U, 8388608U, 2U, 514U, 8389120U, 8389120U, 33280U,
			33280U, 8421376U, 8421376U, 8389122U, 32770U, 8388610U, 8388610U, 32770U, 0U, 514U,
			33282U, 8388608U, 32768U, 8421890U, 2U, 8421376U, 8421888U, 8388608U, 8388608U, 512U,
			8421378U, 32768U, 33280U, 8388610U, 512U, 2U, 8389122U, 33282U, 8421890U, 32770U,
			8421376U, 8389122U, 8388610U, 514U, 33282U, 8421888U, 514U, 8389120U, 8389120U, 0U,
			32770U, 33280U, 0U, 8421378U, 1074282512U, 1073758208U, 16384U, 540688U, 524288U, 16U,
			1074266128U, 1073758224U, 1073741840U, 1074282512U, 1074282496U, 1073741824U, 1073758208U, 524288U, 16U, 1074266128U,
			540672U, 524304U, 1073758224U, 0U, 1073741824U, 16384U, 540688U, 1074266112U, 524304U, 1073741840U,
			0U, 540672U, 16400U, 1074282496U, 1074266112U, 16400U, 0U, 540688U, 1074266128U, 524288U,
			1073758224U, 1074266112U, 1074282496U, 16384U, 1074266112U, 1073758208U, 16U, 1074282512U, 540688U, 16U,
			16384U, 1073741824U, 16400U, 1074282496U, 524288U, 1073741840U, 524304U, 1073758224U, 1073741840U, 524304U,
			540672U, 0U, 1073758208U, 16400U, 1073741824U, 1074266128U, 1074282512U, 540672U, 260U, 67174656U,
			0U, 67174404U, 67109120U, 0U, 65796U, 67109120U, 65540U, 67108868U, 67108868U, 65536U,
			67174660U, 65540U, 67174400U, 260U, 67108864U, 4U, 67174656U, 256U, 65792U, 67174400U,
			67174404U, 65796U, 67109124U, 65792U, 65536U, 67109124U, 4U, 67174660U, 256U, 67108864U,
			67174656U, 67108864U, 65540U, 260U, 65536U, 67174656U, 67109120U, 0U, 256U, 65540U,
			67174660U, 67109120U, 67108868U, 256U, 0U, 67174404U, 67109124U, 65536U, 67108864U, 67174660U,
			4U, 65796U, 65792U, 67108868U, 67174400U, 67109124U, 260U, 67174400U, 65796U, 4U,
			67174404U, 65792U, 2151682048U, 2147487808U, 2147487808U, 64U, 4198464U, 2151678016U, 2151677952U, 2147487744U,
			0U, 4198400U, 4198400U, 2151682112U, 2147483712U, 0U, 4194368U, 2151677952U, 2147483648U, 4096U,
			4194304U, 2151682048U, 64U, 4194304U, 2147487744U, 4160U, 2151678016U, 2147483648U, 4160U, 4194368U,
			4096U, 4198464U, 2151682112U, 2147483712U, 4194368U, 2151677952U, 4198400U, 2151682112U, 2147483712U, 0U,
			0U, 4198400U, 4160U, 4194368U, 2151678016U, 2147483648U, 2151682048U, 2147487808U, 2147487808U, 64U,
			2151682112U, 2147483712U, 2147483648U, 4096U, 2151677952U, 2147487744U, 4198464U, 2151678016U, 2147487744U, 4160U,
			4194304U, 2151682048U, 64U, 4194304U, 4096U, 4198464U, 128U, 17039488U, 17039360U, 553648256U,
			262144U, 128U, 536870912U, 17039360U, 537133184U, 262144U, 16777344U, 537133184U, 553648256U, 553910272U,
			262272U, 536870912U, 16777216U, 537133056U, 537133056U, 0U, 536871040U, 553910400U, 553910400U, 16777344U,
			553910272U, 536871040U, 0U, 553648128U, 17039488U, 16777216U, 553648128U, 262272U, 262144U, 553648256U,
			128U, 16777216U, 536870912U, 17039360U, 553648256U, 537133184U, 16777344U, 536870912U, 553910272U, 17039488U,
			537133184U, 128U, 16777216U, 553910272U, 553910400U, 262272U, 553648128U, 553910400U, 17039360U, 0U,
			537133056U, 553648128U, 262272U, 16777344U, 536871040U, 262144U, 0U, 537133056U, 17039488U, 536871040U,
			268435464U, 270532608U, 8192U, 270540808U, 270532608U, 8U, 270540808U, 2097152U, 268443648U, 2105352U,
			2097152U, 268435464U, 2097160U, 268443648U, 268435456U, 8200U, 0U, 2097160U, 268443656U, 8192U,
			2105344U, 268443656U, 8U, 270532616U, 270532616U, 0U, 2105352U, 270540800U, 8200U, 2105344U,
			270540800U, 268435456U, 268443648U, 8U, 270532616U, 2105344U, 270540808U, 2097152U, 8200U, 268435464U,
			2097152U, 268443648U, 268435456U, 8200U, 268435464U, 270540808U, 2105344U, 270532608U, 2105352U, 270540800U,
			0U, 270532616U, 8U, 8192U, 270532608U, 2105352U, 8192U, 2097160U, 268443656U, 0U,
			270540800U, 268435456U, 2097160U, 268443656U, 1048576U, 34603009U, 33555457U, 0U, 1024U, 33555457U,
			1049601U, 34604032U, 34604033U, 1048576U, 0U, 33554433U, 1U, 33554432U, 34603009U, 1025U,
			33555456U, 1049601U, 1048577U, 33555456U, 33554433U, 34603008U, 34604032U, 1048577U, 34603008U, 1024U,
			1025U, 34604033U, 1049600U, 1U, 33554432U, 1049600U, 33554432U, 1049600U, 1048576U, 33555457U,
			33555457U, 34603009U, 34603009U, 1U, 1048577U, 33554432U, 33555456U, 1048576U, 34604032U, 1025U,
			1049601U, 34604032U, 1025U, 33554433U, 34604033U, 34603008U, 1049600U, 0U, 1U, 34604033U,
			0U, 1049601U, 34603008U, 1024U, 33554433U, 33555456U, 1024U, 1048577U, 134219808U, 2048U,
			131072U, 134350880U, 134217728U, 134219808U, 32U, 134217728U, 131104U, 134348800U, 134350880U, 133120U,
			134350848U, 133152U, 2048U, 32U, 134348800U, 134217760U, 134219776U, 2080U, 133120U, 131104U,
			134348832U, 134350848U, 2080U, 0U, 0U, 134348832U, 134217760U, 134219776U, 133152U, 131072U,
			133152U, 131072U, 134350848U, 2048U, 32U, 134348832U, 2048U, 133152U, 134219776U, 32U,
			134217760U, 134348800U, 134348832U, 134217728U, 131072U, 134219808U, 0U, 134350880U, 131104U, 134217760U,
			134348800U, 134219776U, 134219808U, 0U, 134350880U, 133120U, 133120U, 2080U, 2080U, 131104U,
			134217728U, 134350848U
		};

		private static readonly byte[] PC1 = new byte[]
		{
			56, 48, 40, 32, 24, 16, 8, 0, 57, 49,
			41, 33, 25, 17, 9, 1, 58, 50, 42, 34,
			26, 18, 10, 2, 59, 51, 43, 35, 62, 54,
			46, 38, 30, 22, 14, 6, 61, 53, 45, 37,
			29, 21, 13, 5, 60, 52, 44, 36, 28, 20,
			12, 4, 27, 19, 11, 3
		};

		private static readonly byte[] leftRotTotal = new byte[]
		{
			1, 2, 4, 6, 8, 10, 12, 14, 15, 17,
			19, 21, 23, 25, 27, 28
		};

		private static readonly byte[] PC2 = new byte[]
		{
			13, 16, 10, 23, 0, 4, 2, 27, 14, 5,
			20, 9, 22, 18, 11, 3, 25, 7, 15, 6,
			26, 19, 12, 1, 40, 51, 30, 36, 46, 54,
			29, 39, 50, 44, 32, 47, 43, 48, 38, 55,
			33, 52, 45, 41, 49, 35, 28, 31
		};

		internal static readonly uint[] ipTab = new uint[]
		{
			0U, 0U, 256U, 0U, 0U, 256U, 256U, 256U, 1U, 0U,
			257U, 0U, 1U, 256U, 257U, 256U, 0U, 1U, 256U, 1U,
			0U, 257U, 256U, 257U, 1U, 1U, 257U, 1U, 1U, 257U,
			257U, 257U, 0U, 0U, 16777216U, 0U, 0U, 16777216U, 16777216U, 16777216U,
			65536U, 0U, 16842752U, 0U, 65536U, 16777216U, 16842752U, 16777216U, 0U, 65536U,
			16777216U, 65536U, 0U, 16842752U, 16777216U, 16842752U, 65536U, 65536U, 16842752U, 65536U,
			65536U, 16842752U, 16842752U, 16842752U, 0U, 0U, 512U, 0U, 0U, 512U,
			512U, 512U, 2U, 0U, 514U, 0U, 2U, 512U, 514U, 512U,
			0U, 2U, 512U, 2U, 0U, 514U, 512U, 514U, 2U, 2U,
			514U, 2U, 2U, 514U, 514U, 514U, 0U, 0U, 33554432U, 0U,
			0U, 33554432U, 33554432U, 33554432U, 131072U, 0U, 33685504U, 0U, 131072U, 33554432U,
			33685504U, 33554432U, 0U, 131072U, 33554432U, 131072U, 0U, 33685504U, 33554432U, 33685504U,
			131072U, 131072U, 33685504U, 131072U, 131072U, 33685504U, 33685504U, 33685504U, 0U, 0U,
			1024U, 0U, 0U, 1024U, 1024U, 1024U, 4U, 0U, 1028U, 0U,
			4U, 1024U, 1028U, 1024U, 0U, 4U, 1024U, 4U, 0U, 1028U,
			1024U, 1028U, 4U, 4U, 1028U, 4U, 4U, 1028U, 1028U, 1028U,
			0U, 0U, 67108864U, 0U, 0U, 67108864U, 67108864U, 67108864U, 262144U, 0U,
			67371008U, 0U, 262144U, 67108864U, 67371008U, 67108864U, 0U, 262144U, 67108864U, 262144U,
			0U, 67371008U, 67108864U, 67371008U, 262144U, 262144U, 67371008U, 262144U, 262144U, 67371008U,
			67371008U, 67371008U, 0U, 0U, 2048U, 0U, 0U, 2048U, 2048U, 2048U,
			8U, 0U, 2056U, 0U, 8U, 2048U, 2056U, 2048U, 0U, 8U,
			2048U, 8U, 0U, 2056U, 2048U, 2056U, 8U, 8U, 2056U, 8U,
			8U, 2056U, 2056U, 2056U, 0U, 0U, 134217728U, 0U, 0U, 134217728U,
			134217728U, 134217728U, 524288U, 0U, 134742016U, 0U, 524288U, 134217728U, 134742016U, 134217728U,
			0U, 524288U, 134217728U, 524288U, 0U, 134742016U, 134217728U, 134742016U, 524288U, 524288U,
			134742016U, 524288U, 524288U, 134742016U, 134742016U, 134742016U, 0U, 0U, 4096U, 0U,
			0U, 4096U, 4096U, 4096U, 16U, 0U, 4112U, 0U, 16U, 4096U,
			4112U, 4096U, 0U, 16U, 4096U, 16U, 0U, 4112U, 4096U, 4112U,
			16U, 16U, 4112U, 16U, 16U, 4112U, 4112U, 4112U, 0U, 0U,
			268435456U, 0U, 0U, 268435456U, 268435456U, 268435456U, 1048576U, 0U, 269484032U, 0U,
			1048576U, 268435456U, 269484032U, 268435456U, 0U, 1048576U, 268435456U, 1048576U, 0U, 269484032U,
			268435456U, 269484032U, 1048576U, 1048576U, 269484032U, 1048576U, 1048576U, 269484032U, 269484032U, 269484032U,
			0U, 0U, 8192U, 0U, 0U, 8192U, 8192U, 8192U, 32U, 0U,
			8224U, 0U, 32U, 8192U, 8224U, 8192U, 0U, 32U, 8192U, 32U,
			0U, 8224U, 8192U, 8224U, 32U, 32U, 8224U, 32U, 32U, 8224U,
			8224U, 8224U, 0U, 0U, 536870912U, 0U, 0U, 536870912U, 536870912U, 536870912U,
			2097152U, 0U, 538968064U, 0U, 2097152U, 536870912U, 538968064U, 536870912U, 0U, 2097152U,
			536870912U, 2097152U, 0U, 538968064U, 536870912U, 538968064U, 2097152U, 2097152U, 538968064U, 2097152U,
			2097152U, 538968064U, 538968064U, 538968064U, 0U, 0U, 16384U, 0U, 0U, 16384U,
			16384U, 16384U, 64U, 0U, 16448U, 0U, 64U, 16384U, 16448U, 16384U,
			0U, 64U, 16384U, 64U, 0U, 16448U, 16384U, 16448U, 64U, 64U,
			16448U, 64U, 64U, 16448U, 16448U, 16448U, 0U, 0U, 1073741824U, 0U,
			0U, 1073741824U, 1073741824U, 1073741824U, 4194304U, 0U, 1077936128U, 0U, 4194304U, 1073741824U,
			1077936128U, 1073741824U, 0U, 4194304U, 1073741824U, 4194304U, 0U, 1077936128U, 1073741824U, 1077936128U,
			4194304U, 4194304U, 1077936128U, 4194304U, 4194304U, 1077936128U, 1077936128U, 1077936128U, 0U, 0U,
			32768U, 0U, 0U, 32768U, 32768U, 32768U, 128U, 0U, 32896U, 0U,
			128U, 32768U, 32896U, 32768U, 0U, 128U, 32768U, 128U, 0U, 32896U,
			32768U, 32896U, 128U, 128U, 32896U, 128U, 128U, 32896U, 32896U, 32896U,
			0U, 0U, 2147483648U, 0U, 0U, 2147483648U, 2147483648U, 2147483648U, 8388608U, 0U,
			2155872256U, 0U, 8388608U, 2147483648U, 2155872256U, 2147483648U, 0U, 8388608U, 2147483648U, 8388608U,
			0U, 2155872256U, 2147483648U, 2155872256U, 8388608U, 8388608U, 2155872256U, 8388608U, 8388608U, 2155872256U,
			2155872256U, 2155872256U
		};

		internal static readonly uint[] fpTab = new uint[]
		{
			0U, 0U, 0U, 64U, 0U, 16384U, 0U, 16448U, 0U, 4194304U,
			0U, 4194368U, 0U, 4210688U, 0U, 4210752U, 0U, 1073741824U, 0U, 1073741888U,
			0U, 1073758208U, 0U, 1073758272U, 0U, 1077936128U, 0U, 1077936192U, 0U, 1077952512U,
			0U, 1077952576U, 0U, 0U, 64U, 0U, 16384U, 0U, 16448U, 0U,
			4194304U, 0U, 4194368U, 0U, 4210688U, 0U, 4210752U, 0U, 1073741824U, 0U,
			1073741888U, 0U, 1073758208U, 0U, 1073758272U, 0U, 1077936128U, 0U, 1077936192U, 0U,
			1077952512U, 0U, 1077952576U, 0U, 0U, 0U, 0U, 16U, 0U, 4096U,
			0U, 4112U, 0U, 1048576U, 0U, 1048592U, 0U, 1052672U, 0U, 1052688U,
			0U, 268435456U, 0U, 268435472U, 0U, 268439552U, 0U, 268439568U, 0U, 269484032U,
			0U, 269484048U, 0U, 269488128U, 0U, 269488144U, 0U, 0U, 16U, 0U,
			4096U, 0U, 4112U, 0U, 1048576U, 0U, 1048592U, 0U, 1052672U, 0U,
			1052688U, 0U, 268435456U, 0U, 268435472U, 0U, 268439552U, 0U, 268439568U, 0U,
			269484032U, 0U, 269484048U, 0U, 269488128U, 0U, 269488144U, 0U, 0U, 0U,
			0U, 4U, 0U, 1024U, 0U, 1028U, 0U, 262144U, 0U, 262148U,
			0U, 263168U, 0U, 263172U, 0U, 67108864U, 0U, 67108868U, 0U, 67109888U,
			0U, 67109892U, 0U, 67371008U, 0U, 67371012U, 0U, 67372032U, 0U, 67372036U,
			0U, 0U, 4U, 0U, 1024U, 0U, 1028U, 0U, 262144U, 0U,
			262148U, 0U, 263168U, 0U, 263172U, 0U, 67108864U, 0U, 67108868U, 0U,
			67109888U, 0U, 67109892U, 0U, 67371008U, 0U, 67371012U, 0U, 67372032U, 0U,
			67372036U, 0U, 0U, 0U, 0U, 1U, 0U, 256U, 0U, 257U,
			0U, 65536U, 0U, 65537U, 0U, 65792U, 0U, 65793U, 0U, 16777216U,
			0U, 16777217U, 0U, 16777472U, 0U, 16777473U, 0U, 16842752U, 0U, 16842753U,
			0U, 16843008U, 0U, 16843009U, 0U, 0U, 1U, 0U, 256U, 0U,
			257U, 0U, 65536U, 0U, 65537U, 0U, 65792U, 0U, 65793U, 0U,
			16777216U, 0U, 16777217U, 0U, 16777472U, 0U, 16777473U, 0U, 16842752U, 0U,
			16842753U, 0U, 16843008U, 0U, 16843009U, 0U, 0U, 0U, 0U, 128U,
			0U, 32768U, 0U, 32896U, 0U, 8388608U, 0U, 8388736U, 0U, 8421376U,
			0U, 8421504U, 0U, 2147483648U, 0U, 2147483776U, 0U, 2147516416U, 0U, 2147516544U,
			0U, 2155872256U, 0U, 2155872384U, 0U, 2155905024U, 0U, 2155905152U, 0U, 0U,
			128U, 0U, 32768U, 0U, 32896U, 0U, 8388608U, 0U, 8388736U, 0U,
			8421376U, 0U, 8421504U, 0U, 2147483648U, 0U, 2147483776U, 0U, 2147516416U, 0U,
			2147516544U, 0U, 2155872256U, 0U, 2155872384U, 0U, 2155905024U, 0U, 2155905152U, 0U,
			0U, 0U, 0U, 32U, 0U, 8192U, 0U, 8224U, 0U, 2097152U,
			0U, 2097184U, 0U, 2105344U, 0U, 2105376U, 0U, 536870912U, 0U, 536870944U,
			0U, 536879104U, 0U, 536879136U, 0U, 538968064U, 0U, 538968096U, 0U, 538976256U,
			0U, 538976288U, 0U, 0U, 32U, 0U, 8192U, 0U, 8224U, 0U,
			2097152U, 0U, 2097184U, 0U, 2105344U, 0U, 2105376U, 0U, 536870912U, 0U,
			536870944U, 0U, 536879104U, 0U, 536879136U, 0U, 538968064U, 0U, 538968096U, 0U,
			538976256U, 0U, 538976288U, 0U, 0U, 0U, 0U, 8U, 0U, 2048U,
			0U, 2056U, 0U, 524288U, 0U, 524296U, 0U, 526336U, 0U, 526344U,
			0U, 134217728U, 0U, 134217736U, 0U, 134219776U, 0U, 134219784U, 0U, 134742016U,
			0U, 134742024U, 0U, 134744064U, 0U, 134744072U, 0U, 0U, 8U, 0U,
			2048U, 0U, 2056U, 0U, 524288U, 0U, 524296U, 0U, 526336U, 0U,
			526344U, 0U, 134217728U, 0U, 134217736U, 0U, 134219776U, 0U, 134219784U, 0U,
			134742016U, 0U, 134742024U, 0U, 134744064U, 0U, 134744072U, 0U, 0U, 0U,
			0U, 2U, 0U, 512U, 0U, 514U, 0U, 131072U, 0U, 131074U,
			0U, 131584U, 0U, 131586U, 0U, 33554432U, 0U, 33554434U, 0U, 33554944U,
			0U, 33554946U, 0U, 33685504U, 0U, 33685506U, 0U, 33686016U, 0U, 33686018U,
			0U, 0U, 2U, 0U, 512U, 0U, 514U, 0U, 131072U, 0U,
			131074U, 0U, 131584U, 0U, 131586U, 0U, 33554432U, 0U, 33554434U, 0U,
			33554944U, 0U, 33554946U, 0U, 33685504U, 0U, 33685506U, 0U, 33686016U, 0U,
			33686018U, 0U
		};
	}
}
