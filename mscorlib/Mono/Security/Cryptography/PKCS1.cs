using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	internal sealed class PKCS1
	{
		private PKCS1()
		{
		}

		private static bool Compare(byte[] array1, byte[] array2)
		{
			bool flag = array1.Length == array2.Length;
			if (flag)
			{
				for (int i = 0; i < array1.Length; i++)
				{
					if (array1[i] != array2[i])
					{
						return false;
					}
				}
			}
			return flag;
		}

		private static byte[] xor(byte[] array1, byte[] array2)
		{
			byte[] array3 = new byte[array1.Length];
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i] = array1[i] ^ array2[i];
			}
			return array3;
		}

		private static byte[] GetEmptyHash(HashAlgorithm hash)
		{
			if (hash is SHA1)
			{
				return PKCS1.emptySHA1;
			}
			if (hash is SHA256)
			{
				return PKCS1.emptySHA256;
			}
			if (hash is SHA384)
			{
				return PKCS1.emptySHA384;
			}
			if (hash is SHA512)
			{
				return PKCS1.emptySHA512;
			}
			return hash.ComputeHash(null);
		}

		public static byte[] I2OSP(int x, int size)
		{
			byte[] bytes = BitConverterLE.GetBytes(x);
			Array.Reverse(bytes, 0, bytes.Length);
			return PKCS1.I2OSP(bytes, size);
		}

		public static byte[] I2OSP(byte[] x, int size)
		{
			byte[] array = new byte[size];
			Buffer.BlockCopy(x, 0, array, array.Length - x.Length, x.Length);
			return array;
		}

		public static byte[] OS2IP(byte[] x)
		{
			int num = 0;
			while (x[num++] == 0 && num < x.Length)
			{
			}
			num--;
			if (num > 0)
			{
				byte[] array = new byte[x.Length - num];
				Buffer.BlockCopy(x, num, array, 0, array.Length);
				return array;
			}
			return x;
		}

		public static byte[] RSAEP(RSA rsa, byte[] m)
		{
			return rsa.EncryptValue(m);
		}

		public static byte[] RSADP(RSA rsa, byte[] c)
		{
			return rsa.DecryptValue(c);
		}

		public static byte[] RSASP1(RSA rsa, byte[] m)
		{
			return rsa.DecryptValue(m);
		}

		public static byte[] RSAVP1(RSA rsa, byte[] s)
		{
			return rsa.EncryptValue(s);
		}

		public static byte[] Encrypt_OAEP(RSA rsa, HashAlgorithm hash, RandomNumberGenerator rng, byte[] M)
		{
			int num = rsa.KeySize / 8;
			int num2 = hash.HashSize / 8;
			if (M.Length > num - 2 * num2 - 2)
			{
				throw new CryptographicException("message too long");
			}
			byte[] emptyHash = PKCS1.GetEmptyHash(hash);
			int num3 = num - M.Length - 2 * num2 - 2;
			byte[] array = new byte[emptyHash.Length + num3 + 1 + M.Length];
			Buffer.BlockCopy(emptyHash, 0, array, 0, emptyHash.Length);
			array[emptyHash.Length + num3] = 1;
			Buffer.BlockCopy(M, 0, array, array.Length - M.Length, M.Length);
			byte[] array2 = new byte[num2];
			rng.GetBytes(array2);
			byte[] array3 = PKCS1.MGF1(hash, array2, num - num2 - 1);
			byte[] array4 = PKCS1.xor(array, array3);
			byte[] array5 = PKCS1.MGF1(hash, array4, num2);
			byte[] array6 = PKCS1.xor(array2, array5);
			byte[] array7 = new byte[array6.Length + array4.Length + 1];
			Buffer.BlockCopy(array6, 0, array7, 1, array6.Length);
			Buffer.BlockCopy(array4, 0, array7, array6.Length + 1, array4.Length);
			byte[] array8 = PKCS1.OS2IP(array7);
			byte[] array9 = PKCS1.RSAEP(rsa, array8);
			return PKCS1.I2OSP(array9, num);
		}

		public static byte[] Decrypt_OAEP(RSA rsa, HashAlgorithm hash, byte[] C)
		{
			int num = rsa.KeySize / 8;
			int num2 = hash.HashSize / 8;
			if (num < 2 * num2 + 2 || C.Length != num)
			{
				throw new CryptographicException("decryption error");
			}
			byte[] array = PKCS1.OS2IP(C);
			byte[] array2 = PKCS1.RSADP(rsa, array);
			byte[] array3 = PKCS1.I2OSP(array2, num);
			byte[] array4 = new byte[num2];
			Buffer.BlockCopy(array3, 1, array4, 0, array4.Length);
			byte[] array5 = new byte[num - num2 - 1];
			Buffer.BlockCopy(array3, array3.Length - array5.Length, array5, 0, array5.Length);
			byte[] array6 = PKCS1.MGF1(hash, array5, num2);
			byte[] array7 = PKCS1.xor(array4, array6);
			byte[] array8 = PKCS1.MGF1(hash, array7, num - num2 - 1);
			byte[] array9 = PKCS1.xor(array5, array8);
			byte[] emptyHash = PKCS1.GetEmptyHash(hash);
			byte[] array10 = new byte[emptyHash.Length];
			Buffer.BlockCopy(array9, 0, array10, 0, array10.Length);
			bool flag = PKCS1.Compare(emptyHash, array10);
			int num3 = emptyHash.Length;
			while (array9[num3] == 0)
			{
				num3++;
			}
			int num4 = array9.Length - num3 - 1;
			byte[] array11 = new byte[num4];
			Buffer.BlockCopy(array9, num3 + 1, array11, 0, num4);
			if (array3[0] != 0 || !flag || array9[num3] != 1)
			{
				return null;
			}
			return array11;
		}

		public static byte[] Encrypt_v15(RSA rsa, RandomNumberGenerator rng, byte[] M)
		{
			int num = rsa.KeySize / 8;
			if (M.Length > num - 11)
			{
				throw new CryptographicException("message too long");
			}
			int num2 = Math.Max(8, num - M.Length - 3);
			byte[] array = new byte[num2];
			rng.GetNonZeroBytes(array);
			byte[] array2 = new byte[num];
			array2[1] = 2;
			Buffer.BlockCopy(array, 0, array2, 2, num2);
			Buffer.BlockCopy(M, 0, array2, num - M.Length, M.Length);
			byte[] array3 = PKCS1.OS2IP(array2);
			byte[] array4 = PKCS1.RSAEP(rsa, array3);
			return PKCS1.I2OSP(array4, num);
		}

		public static byte[] Decrypt_v15(RSA rsa, byte[] C)
		{
			int num = rsa.KeySize >> 3;
			if (num < 11 || C.Length > num)
			{
				throw new CryptographicException("decryption error");
			}
			byte[] array = PKCS1.OS2IP(C);
			byte[] array2 = PKCS1.RSADP(rsa, array);
			byte[] array3 = PKCS1.I2OSP(array2, num);
			if (array3[0] != 0 || array3[1] != 2)
			{
				return null;
			}
			int num2 = 10;
			while (array3[num2] != 0 && num2 < array3.Length)
			{
				num2++;
			}
			if (array3[num2] != 0)
			{
				return null;
			}
			num2++;
			byte[] array4 = new byte[array3.Length - num2];
			Buffer.BlockCopy(array3, num2, array4, 0, array4.Length);
			return array4;
		}

		public static byte[] Sign_v15(RSA rsa, HashAlgorithm hash, byte[] hashValue)
		{
			int num = rsa.KeySize >> 3;
			byte[] array = PKCS1.Encode_v15(hash, hashValue, num);
			byte[] array2 = PKCS1.OS2IP(array);
			byte[] array3 = PKCS1.RSASP1(rsa, array2);
			return PKCS1.I2OSP(array3, num);
		}

		public static bool Verify_v15(RSA rsa, HashAlgorithm hash, byte[] hashValue, byte[] signature)
		{
			return PKCS1.Verify_v15(rsa, hash, hashValue, signature, false);
		}

		public static bool Verify_v15(RSA rsa, HashAlgorithm hash, byte[] hashValue, byte[] signature, bool tryNonStandardEncoding)
		{
			int num = rsa.KeySize >> 3;
			byte[] array = PKCS1.OS2IP(signature);
			byte[] array2 = PKCS1.RSAVP1(rsa, array);
			byte[] array3 = PKCS1.I2OSP(array2, num);
			byte[] array4 = PKCS1.Encode_v15(hash, hashValue, num);
			bool flag = PKCS1.Compare(array4, array3);
			if (flag || !tryNonStandardEncoding)
			{
				return flag;
			}
			if (array3[0] != 0 || array3[1] != 1)
			{
				return false;
			}
			int i;
			for (i = 2; i < array3.Length - hashValue.Length - 1; i++)
			{
				if (array3[i] != 255)
				{
					return false;
				}
			}
			if (array3[i++] != 0)
			{
				return false;
			}
			byte[] array5 = new byte[hashValue.Length];
			Buffer.BlockCopy(array3, i, array5, 0, array5.Length);
			return PKCS1.Compare(array5, hashValue);
		}

		public static byte[] Encode_v15(HashAlgorithm hash, byte[] hashValue, int emLength)
		{
			if (hashValue.Length != hash.HashSize >> 3)
			{
				throw new CryptographicException("bad hash length for " + hash.ToString());
			}
			string text = CryptoConfig.MapNameToOID(hash.ToString());
			byte[] array;
			if (text != null)
			{
				ASN1 asn = new ASN1(48);
				asn.Add(new ASN1(CryptoConfig.EncodeOID(text)));
				asn.Add(new ASN1(5));
				ASN1 asn2 = new ASN1(4, hashValue);
				ASN1 asn3 = new ASN1(48);
				asn3.Add(asn);
				asn3.Add(asn2);
				array = asn3.GetBytes();
			}
			else
			{
				array = hashValue;
			}
			Buffer.BlockCopy(hashValue, 0, array, array.Length - hashValue.Length, hashValue.Length);
			int num = Math.Max(8, emLength - array.Length - 3);
			byte[] array2 = new byte[num + array.Length + 3];
			array2[1] = 1;
			for (int i = 2; i < num + 2; i++)
			{
				array2[i] = byte.MaxValue;
			}
			Buffer.BlockCopy(array, 0, array2, num + 3, array.Length);
			return array2;
		}

		public static byte[] MGF1(HashAlgorithm hash, byte[] mgfSeed, int maskLen)
		{
			if (maskLen < 0)
			{
				throw new OverflowException();
			}
			int num = mgfSeed.Length;
			int num2 = hash.HashSize >> 3;
			int num3 = maskLen / num2;
			if (maskLen % num2 != 0)
			{
				num3++;
			}
			byte[] array = new byte[num3 * num2];
			byte[] array2 = new byte[num + 4];
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				byte[] array3 = PKCS1.I2OSP(i, 4);
				Buffer.BlockCopy(mgfSeed, 0, array2, 0, num);
				Buffer.BlockCopy(array3, 0, array2, num, 4);
				byte[] array4 = hash.ComputeHash(array2);
				Buffer.BlockCopy(array4, 0, array, num4, num2);
				num4 += num;
			}
			byte[] array5 = new byte[maskLen];
			Buffer.BlockCopy(array, 0, array5, 0, maskLen);
			return array5;
		}

		private static byte[] emptySHA1 = new byte[]
		{
			218, 57, 163, 238, 94, 107, 75, 13, 50, 85,
			191, 239, 149, 96, 24, 144, 175, 216, 7, 9
		};

		private static byte[] emptySHA256 = new byte[]
		{
			227, 176, 196, 66, 152, 252, 28, 20, 154, 251,
			244, 200, 153, 111, 185, 36, 39, 174, 65, 228,
			100, 155, 147, 76, 164, 149, 153, 27, 120, 82,
			184, 85
		};

		private static byte[] emptySHA384 = new byte[]
		{
			56, 176, 96, 167, 81, 172, 150, 56, 76, 217,
			50, 126, 177, 177, 227, 106, 33, 253, 183, 17,
			20, 190, 7, 67, 76, 12, 199, 191, 99, 246,
			225, 218, 39, 78, 222, 191, 231, 111, 101, 251,
			213, 26, 210, 241, 72, 152, 185, 91
		};

		private static byte[] emptySHA512 = new byte[]
		{
			207, 131, 225, 53, 126, 239, 184, 189, 241, 84,
			40, 80, 214, 109, 128, 7, 214, 32, 228, 5,
			11, 87, 21, 220, 131, 244, 169, 33, 211, 108,
			233, 206, 71, 208, 209, 60, 93, 133, 242, 176,
			byte.MaxValue, 131, 24, 210, 135, 126, 236, 47, 99, 185,
			49, 189, 71, 65, 122, 129, 165, 56, 50, 122,
			249, 39, 218, 62
		};
	}
}
