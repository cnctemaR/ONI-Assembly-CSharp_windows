using System;

namespace System.Security.Cryptography.Xml
{
	internal static class SymmetricKeyWrap
	{
		internal static byte[] TripleDESKeyWrapEncrypt(byte[] rgbKey, byte[] rgbWrappedKeyData)
		{
			byte[] array;
			using (SHA1 sha = SHA1.Create())
			{
				array = sha.ComputeHash(rgbWrappedKeyData);
			}
			byte[] array2 = new byte[8];
			using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
			{
				randomNumberGenerator.GetBytes(array2);
			}
			byte[] array3 = new byte[rgbWrappedKeyData.Length + 8];
			TripleDES tripleDES = null;
			ICryptoTransform cryptoTransform = null;
			ICryptoTransform cryptoTransform2 = null;
			byte[] array6;
			try
			{
				tripleDES = TripleDES.Create();
				tripleDES.Padding = PaddingMode.None;
				cryptoTransform = tripleDES.CreateEncryptor(rgbKey, array2);
				cryptoTransform2 = tripleDES.CreateEncryptor(rgbKey, SymmetricKeyWrap.s_rgbTripleDES_KW_IV);
				Buffer.BlockCopy(rgbWrappedKeyData, 0, array3, 0, rgbWrappedKeyData.Length);
				Buffer.BlockCopy(array, 0, array3, rgbWrappedKeyData.Length, 8);
				byte[] array4 = cryptoTransform.TransformFinalBlock(array3, 0, array3.Length);
				byte[] array5 = new byte[array2.Length + array4.Length];
				Buffer.BlockCopy(array2, 0, array5, 0, array2.Length);
				Buffer.BlockCopy(array4, 0, array5, array2.Length, array4.Length);
				Array.Reverse<byte>(array5);
				array6 = cryptoTransform2.TransformFinalBlock(array5, 0, array5.Length);
			}
			finally
			{
				if (cryptoTransform2 != null)
				{
					cryptoTransform2.Dispose();
				}
				if (cryptoTransform != null)
				{
					cryptoTransform.Dispose();
				}
				if (tripleDES != null)
				{
					tripleDES.Dispose();
				}
			}
			return array6;
		}

		internal static byte[] TripleDESKeyWrapDecrypt(byte[] rgbKey, byte[] rgbEncryptedWrappedKeyData)
		{
			if (rgbEncryptedWrappedKeyData.Length != 32 && rgbEncryptedWrappedKeyData.Length != 40 && rgbEncryptedWrappedKeyData.Length != 48)
			{
				throw new CryptographicException("The length of the encrypted data in Key Wrap is either 32, 40 or 48 bytes.");
			}
			TripleDES tripleDES = null;
			ICryptoTransform cryptoTransform = null;
			ICryptoTransform cryptoTransform2 = null;
			byte[] array7;
			try
			{
				tripleDES = TripleDES.Create();
				tripleDES.Padding = PaddingMode.None;
				cryptoTransform = tripleDES.CreateDecryptor(rgbKey, SymmetricKeyWrap.s_rgbTripleDES_KW_IV);
				byte[] array = cryptoTransform.TransformFinalBlock(rgbEncryptedWrappedKeyData, 0, rgbEncryptedWrappedKeyData.Length);
				Array.Reverse<byte>(array);
				byte[] array2 = new byte[8];
				Buffer.BlockCopy(array, 0, array2, 0, 8);
				byte[] array3 = new byte[array.Length - array2.Length];
				Buffer.BlockCopy(array, 8, array3, 0, array3.Length);
				cryptoTransform2 = tripleDES.CreateDecryptor(rgbKey, array2);
				byte[] array4 = cryptoTransform2.TransformFinalBlock(array3, 0, array3.Length);
				byte[] array5 = new byte[array4.Length - 8];
				Buffer.BlockCopy(array4, 0, array5, 0, array5.Length);
				using (SHA1 sha = SHA1.Create())
				{
					byte[] array6 = sha.ComputeHash(array5);
					int i = array5.Length;
					int num = 0;
					while (i < array4.Length)
					{
						if (array4[i] != array6[num])
						{
							throw new CryptographicException("Bad wrapped key size.");
						}
						i++;
						num++;
					}
					array7 = array5;
				}
			}
			finally
			{
				if (cryptoTransform2 != null)
				{
					cryptoTransform2.Dispose();
				}
				if (cryptoTransform != null)
				{
					cryptoTransform.Dispose();
				}
				if (tripleDES != null)
				{
					tripleDES.Dispose();
				}
			}
			return array7;
		}

		internal static byte[] AESKeyWrapEncrypt(byte[] rgbKey, byte[] rgbWrappedKeyData)
		{
			int num = rgbWrappedKeyData.Length >> 3;
			if (rgbWrappedKeyData.Length % 8 != 0 || num <= 0)
			{
				throw new CryptographicException("The length of the encrypted data in Key Wrap is either 32, 40 or 48 bytes.");
			}
			Aes aes = null;
			ICryptoTransform cryptoTransform = null;
			byte[] array2;
			try
			{
				aes = Aes.Create();
				aes.Key = rgbKey;
				aes.Mode = CipherMode.ECB;
				aes.Padding = PaddingMode.None;
				cryptoTransform = aes.CreateEncryptor();
				if (num == 1)
				{
					byte[] array = new byte[SymmetricKeyWrap.s_rgbAES_KW_IV.Length + rgbWrappedKeyData.Length];
					Buffer.BlockCopy(SymmetricKeyWrap.s_rgbAES_KW_IV, 0, array, 0, SymmetricKeyWrap.s_rgbAES_KW_IV.Length);
					Buffer.BlockCopy(rgbWrappedKeyData, 0, array, SymmetricKeyWrap.s_rgbAES_KW_IV.Length, rgbWrappedKeyData.Length);
					array2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
				}
				else
				{
					byte[] array3 = new byte[num + 1 << 3];
					Buffer.BlockCopy(rgbWrappedKeyData, 0, array3, 8, rgbWrappedKeyData.Length);
					byte[] array4 = new byte[8];
					byte[] array5 = new byte[16];
					Buffer.BlockCopy(SymmetricKeyWrap.s_rgbAES_KW_IV, 0, array4, 0, 8);
					for (int i = 0; i <= 5; i++)
					{
						for (int j = 1; j <= num; j++)
						{
							long num2 = (long)(j + i * num);
							Buffer.BlockCopy(array4, 0, array5, 0, 8);
							Buffer.BlockCopy(array3, 8 * j, array5, 8, 8);
							byte[] array6 = cryptoTransform.TransformFinalBlock(array5, 0, 16);
							for (int k = 0; k < 8; k++)
							{
								byte b = (byte)((num2 >> 8 * (7 - k)) & 255L);
								array4[k] = b ^ array6[k];
							}
							Buffer.BlockCopy(array6, 8, array3, 8 * j, 8);
						}
					}
					Buffer.BlockCopy(array4, 0, array3, 0, 8);
					array2 = array3;
				}
			}
			finally
			{
				if (cryptoTransform != null)
				{
					cryptoTransform.Dispose();
				}
				if (aes != null)
				{
					aes.Dispose();
				}
			}
			return array2;
		}

		internal static byte[] AESKeyWrapDecrypt(byte[] rgbKey, byte[] rgbEncryptedWrappedKeyData)
		{
			int num = (rgbEncryptedWrappedKeyData.Length >> 3) - 1;
			if (rgbEncryptedWrappedKeyData.Length % 8 != 0 || num <= 0)
			{
				throw new CryptographicException("The length of the encrypted data in Key Wrap is either 32, 40 or 48 bytes.");
			}
			byte[] array = new byte[num << 3];
			Aes aes = null;
			ICryptoTransform cryptoTransform = null;
			byte[] array3;
			try
			{
				aes = Aes.Create();
				aes.Key = rgbKey;
				aes.Mode = CipherMode.ECB;
				aes.Padding = PaddingMode.None;
				cryptoTransform = aes.CreateDecryptor();
				if (num == 1)
				{
					byte[] array2 = cryptoTransform.TransformFinalBlock(rgbEncryptedWrappedKeyData, 0, rgbEncryptedWrappedKeyData.Length);
					for (int i = 0; i < 8; i++)
					{
						if (array2[i] != SymmetricKeyWrap.s_rgbAES_KW_IV[i])
						{
							throw new CryptographicException("Bad wrapped key size.");
						}
					}
					Buffer.BlockCopy(array2, 8, array, 0, 8);
					array3 = array;
				}
				else
				{
					Buffer.BlockCopy(rgbEncryptedWrappedKeyData, 8, array, 0, array.Length);
					byte[] array4 = new byte[8];
					byte[] array5 = new byte[16];
					Buffer.BlockCopy(rgbEncryptedWrappedKeyData, 0, array4, 0, 8);
					for (int j = 5; j >= 0; j--)
					{
						for (int k = num; k >= 1; k--)
						{
							long num2 = (long)(k + j * num);
							for (int l = 0; l < 8; l++)
							{
								byte b = (byte)((num2 >> 8 * (7 - l)) & 255L);
								byte[] array6 = array4;
								int num3 = l;
								array6[num3] ^= b;
							}
							Buffer.BlockCopy(array4, 0, array5, 0, 8);
							Buffer.BlockCopy(array, 8 * (k - 1), array5, 8, 8);
							byte[] array7 = cryptoTransform.TransformFinalBlock(array5, 0, 16);
							Buffer.BlockCopy(array7, 8, array, 8 * (k - 1), 8);
							Buffer.BlockCopy(array7, 0, array4, 0, 8);
						}
					}
					for (int m = 0; m < 8; m++)
					{
						if (array4[m] != SymmetricKeyWrap.s_rgbAES_KW_IV[m])
						{
							throw new CryptographicException("Bad wrapped key size.");
						}
					}
					array3 = array;
				}
			}
			finally
			{
				if (cryptoTransform != null)
				{
					cryptoTransform.Dispose();
				}
				if (aes != null)
				{
					aes.Dispose();
				}
			}
			return array3;
		}

		private static readonly byte[] s_rgbTripleDES_KW_IV = new byte[] { 74, 221, 162, 44, 121, 232, 33, 5 };

		private static readonly byte[] s_rgbAES_KW_IV = new byte[] { 166, 166, 166, 166, 166, 166, 166, 166 };
	}
}
