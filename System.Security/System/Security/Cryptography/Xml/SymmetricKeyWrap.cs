using System;
using System.IO;

namespace System.Security.Cryptography.Xml
{
	internal class SymmetricKeyWrap
	{
		public static byte[] AESKeyWrapEncrypt(byte[] rgbKey, byte[] rgbWrappedKeyData)
		{
			SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
			symmetricAlgorithm.Mode = CipherMode.ECB;
			symmetricAlgorithm.IV = new byte[16];
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor(rgbKey, symmetricAlgorithm.IV);
			int num = rgbWrappedKeyData.Length / 8;
			byte[] array = new byte[16];
			byte[] array2 = new byte[8 * (num + 1)];
			if (num == 1)
			{
				byte[] array3 = new byte[] { 166, 166, 166, 166, 166, 166, 166, 166 };
				cryptoTransform.TransformBlock(SymmetricKeyWrap.Concatenate(array3, rgbWrappedKeyData), 0, 16, array, 0);
				Buffer.BlockCopy(SymmetricKeyWrap.MSB(array), 0, array2, 0, 8);
				Buffer.BlockCopy(SymmetricKeyWrap.LSB(array), 0, array2, 8, 8);
			}
			else
			{
				byte[] array3 = new byte[] { 166, 166, 166, 166, 166, 166, 166, 166 };
				byte[][] array4 = new byte[num + 1][];
				for (int i = 1; i <= num; i++)
				{
					array4[i] = new byte[8];
					Buffer.BlockCopy(rgbWrappedKeyData, 8 * (i - 1), array4[i], 0, 8);
				}
				for (int j = 0; j <= 5; j++)
				{
					for (int k = 1; k <= num; k++)
					{
						cryptoTransform.TransformBlock(SymmetricKeyWrap.Concatenate(array3, array4[k]), 0, 16, array, 0);
						byte[] bytes = BitConverter.GetBytes((long)(num * j + k));
						if (BitConverter.IsLittleEndian)
						{
							Array.Reverse(bytes);
						}
						array3 = SymmetricKeyWrap.Xor(bytes, SymmetricKeyWrap.MSB(array));
						array4[k] = SymmetricKeyWrap.LSB(array);
					}
				}
				Buffer.BlockCopy(array3, 0, array2, 0, 8);
				for (int l = 1; l <= num; l++)
				{
					Buffer.BlockCopy(array4[l], 0, array2, 8 * l, 8);
				}
			}
			return array2;
		}

		public static byte[] AESKeyWrapDecrypt(byte[] rgbKey, byte[] rgbEncryptedWrappedKeyData)
		{
			SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
			symmetricAlgorithm.Mode = CipherMode.ECB;
			symmetricAlgorithm.Key = rgbKey;
			int num = rgbEncryptedWrappedKeyData.Length / 8 - 1;
			byte[] array = new byte[8];
			Buffer.BlockCopy(rgbEncryptedWrappedKeyData, 0, array, 0, 8);
			byte[] array2 = new byte[num * 8];
			Buffer.BlockCopy(rgbEncryptedWrappedKeyData, 8, array2, 0, rgbEncryptedWrappedKeyData.Length - 8);
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
			for (int i = 5; i >= 0; i--)
			{
				for (int j = num; j >= 1; j--)
				{
					byte[] bytes = BitConverter.GetBytes((long)num * (long)i + (long)j);
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytes);
					}
					byte[] array3 = new byte[16];
					byte[] array4 = new byte[8];
					Buffer.BlockCopy(array2, 8 * (j - 1), array4, 0, 8);
					byte[] array5 = SymmetricKeyWrap.Concatenate(SymmetricKeyWrap.Xor(array, bytes), array4);
					cryptoTransform.TransformBlock(array5, 0, 16, array3, 0);
					array = SymmetricKeyWrap.MSB(array3);
					Buffer.BlockCopy(SymmetricKeyWrap.LSB(array3), 0, array2, 8 * (j - 1), 8);
				}
			}
			return array2;
		}

		public static byte[] TripleDESKeyWrapEncrypt(byte[] rgbKey, byte[] rgbWrappedKeyData)
		{
			SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("TripleDES");
			byte[] array = SymmetricKeyWrap.ComputeCMSKeyChecksum(rgbWrappedKeyData);
			byte[] array2 = SymmetricKeyWrap.Concatenate(rgbWrappedKeyData, array);
			symmetricAlgorithm.GenerateIV();
			symmetricAlgorithm.Mode = CipherMode.CBC;
			symmetricAlgorithm.Padding = PaddingMode.None;
			symmetricAlgorithm.Key = rgbKey;
			byte[] array3 = SymmetricKeyWrap.Transform(array2, symmetricAlgorithm.CreateEncryptor());
			byte[] array4 = SymmetricKeyWrap.Concatenate(symmetricAlgorithm.IV, array3);
			Array.Reverse(array4);
			symmetricAlgorithm.IV = new byte[] { 74, 221, 162, 44, 121, 232, 33, 5 };
			return SymmetricKeyWrap.Transform(array4, symmetricAlgorithm.CreateEncryptor());
		}

		public static byte[] TripleDESKeyWrapDecrypt(byte[] rgbKey, byte[] rgbEncryptedWrappedKeyData)
		{
			SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("TripleDES");
			symmetricAlgorithm.Mode = CipherMode.CBC;
			symmetricAlgorithm.Padding = PaddingMode.None;
			symmetricAlgorithm.Key = rgbKey;
			symmetricAlgorithm.IV = new byte[] { 74, 221, 162, 44, 121, 232, 33, 5 };
			byte[] array = SymmetricKeyWrap.Transform(rgbEncryptedWrappedKeyData, symmetricAlgorithm.CreateDecryptor());
			Array.Reverse(array);
			byte[] array2 = new byte[array.Length - 8];
			byte[] array3 = new byte[8];
			Buffer.BlockCopy(array, 0, array3, 0, 8);
			Buffer.BlockCopy(array, 8, array2, 0, array2.Length);
			symmetricAlgorithm.IV = array3;
			byte[] array4 = SymmetricKeyWrap.Transform(array2, symmetricAlgorithm.CreateDecryptor());
			byte[] array5 = new byte[8];
			byte[] array6 = new byte[array4.Length - 8];
			Buffer.BlockCopy(array4, 0, array6, 0, array6.Length);
			Buffer.BlockCopy(array4, array6.Length, array5, 0, 8);
			return array6;
		}

		private static byte[] Transform(byte[] data, ICryptoTransform t)
		{
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, t, CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();
			byte[] array = memoryStream.ToArray();
			memoryStream.Close();
			cryptoStream.Close();
			return array;
		}

		private static byte[] ComputeCMSKeyChecksum(byte[] data)
		{
			byte[] array = HashAlgorithm.Create("SHA1").ComputeHash(data);
			byte[] array2 = new byte[8];
			Buffer.BlockCopy(array, 0, array2, 0, 8);
			return array2;
		}

		private static byte[] Concatenate(byte[] buf1, byte[] buf2)
		{
			byte[] array = new byte[buf1.Length + buf2.Length];
			Buffer.BlockCopy(buf1, 0, array, 0, buf1.Length);
			Buffer.BlockCopy(buf2, 0, array, buf1.Length, buf2.Length);
			return array;
		}

		private static byte[] MSB(byte[] input)
		{
			return SymmetricKeyWrap.MSB(input, 8);
		}

		private static byte[] MSB(byte[] input, int bytes)
		{
			byte[] array = new byte[bytes];
			Buffer.BlockCopy(input, 0, array, 0, bytes);
			return array;
		}

		private static byte[] LSB(byte[] input)
		{
			return SymmetricKeyWrap.LSB(input, 8);
		}

		private static byte[] LSB(byte[] input, int bytes)
		{
			byte[] array = new byte[bytes];
			Buffer.BlockCopy(input, bytes, array, 0, bytes);
			return array;
		}

		private static byte[] Xor(byte[] x, byte[] y)
		{
			if (x.Length != y.Length)
			{
				throw new CryptographicException("Error performing Xor: arrays different length.");
			}
			byte[] array = new byte[x.Length];
			for (int i = 0; i < x.Length; i++)
			{
				array[i] = x[i] ^ y[i];
			}
			return array;
		}
	}
}
