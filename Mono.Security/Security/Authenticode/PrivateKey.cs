using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.Authenticode
{
	public class PrivateKey
	{
		public PrivateKey()
		{
			this.keyType = 2;
		}

		public PrivateKey(byte[] data, string password)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!this.Decode(data, password))
			{
				throw new CryptographicException(Locale.GetText("Invalid data and/or password"));
			}
		}

		public bool Encrypted
		{
			get
			{
				return this.encrypted;
			}
		}

		public int KeyType
		{
			get
			{
				return this.keyType;
			}
			set
			{
				this.keyType = value;
			}
		}

		public RSA RSA
		{
			get
			{
				return this.rsa;
			}
			set
			{
				this.rsa = value;
			}
		}

		public bool Weak
		{
			get
			{
				return !this.encrypted || this.weak;
			}
			set
			{
				this.weak = value;
			}
		}

		private byte[] DeriveKey(byte[] salt, string password)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(password);
			SHA1 sha = SHA1.Create();
			sha.TransformBlock(salt, 0, salt.Length, salt, 0);
			sha.TransformFinalBlock(bytes, 0, bytes.Length);
			byte[] array = new byte[16];
			Buffer.BlockCopy(sha.Hash, 0, array, 0, 16);
			sha.Clear();
			Array.Clear(bytes, 0, bytes.Length);
			return array;
		}

		private bool Decode(byte[] pvk, string password)
		{
			if (BitConverterLE.ToUInt32(pvk, 0) != 2964713758U)
			{
				return false;
			}
			if (BitConverterLE.ToUInt32(pvk, 4) != 0U)
			{
				return false;
			}
			this.keyType = BitConverterLE.ToInt32(pvk, 8);
			this.encrypted = BitConverterLE.ToUInt32(pvk, 12) == 1U;
			int num = BitConverterLE.ToInt32(pvk, 16);
			int num2 = BitConverterLE.ToInt32(pvk, 20);
			byte[] array = new byte[num2];
			Buffer.BlockCopy(pvk, 24 + num, array, 0, num2);
			if (num > 0)
			{
				if (password == null)
				{
					return false;
				}
				byte[] array2 = new byte[num];
				Buffer.BlockCopy(pvk, 24, array2, 0, num);
				byte[] array3 = this.DeriveKey(array2, password);
				RC4 rc = RC4.Create();
				ICryptoTransform cryptoTransform = rc.CreateDecryptor(array3, null);
				cryptoTransform.TransformBlock(array, 8, array.Length - 8, array, 8);
				try
				{
					this.rsa = CryptoConvert.FromCapiPrivateKeyBlob(array);
					this.weak = false;
				}
				catch (CryptographicException)
				{
					this.weak = true;
					Buffer.BlockCopy(pvk, 24 + num, array, 0, num2);
					Array.Clear(array3, 5, 11);
					RC4 rc2 = RC4.Create();
					cryptoTransform = rc2.CreateDecryptor(array3, null);
					cryptoTransform.TransformBlock(array, 8, array.Length - 8, array, 8);
					this.rsa = CryptoConvert.FromCapiPrivateKeyBlob(array);
				}
				Array.Clear(array3, 0, array3.Length);
			}
			else
			{
				this.weak = true;
				this.rsa = CryptoConvert.FromCapiPrivateKeyBlob(array);
				Array.Clear(array, 0, array.Length);
			}
			Array.Clear(pvk, 0, pvk.Length);
			return this.rsa != null;
		}

		public void Save(string filename)
		{
			this.Save(filename, null);
		}

		public void Save(string filename, string password)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			byte[] array = null;
			FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write);
			try
			{
				byte[] array2 = new byte[4];
				byte[] array3 = BitConverterLE.GetBytes(2964713758U);
				fileStream.Write(array3, 0, 4);
				fileStream.Write(array2, 0, 4);
				array3 = BitConverterLE.GetBytes(this.keyType);
				fileStream.Write(array3, 0, 4);
				this.encrypted = password != null;
				array = CryptoConvert.ToCapiPrivateKeyBlob(this.rsa);
				if (this.encrypted)
				{
					array3 = BitConverterLE.GetBytes(1);
					fileStream.Write(array3, 0, 4);
					array3 = BitConverterLE.GetBytes(16);
					fileStream.Write(array3, 0, 4);
					array3 = BitConverterLE.GetBytes(array.Length);
					fileStream.Write(array3, 0, 4);
					byte[] array4 = new byte[16];
					RC4 rc = RC4.Create();
					byte[] array5 = null;
					try
					{
						RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
						randomNumberGenerator.GetBytes(array4);
						fileStream.Write(array4, 0, array4.Length);
						array5 = this.DeriveKey(array4, password);
						if (this.Weak)
						{
							Array.Clear(array5, 5, 11);
						}
						ICryptoTransform cryptoTransform = rc.CreateEncryptor(array5, null);
						cryptoTransform.TransformBlock(array, 8, array.Length - 8, array, 8);
					}
					finally
					{
						Array.Clear(array4, 0, array4.Length);
						Array.Clear(array5, 0, array5.Length);
						rc.Clear();
					}
				}
				else
				{
					fileStream.Write(array2, 0, 4);
					fileStream.Write(array2, 0, 4);
					array3 = BitConverterLE.GetBytes(array.Length);
					fileStream.Write(array3, 0, 4);
				}
				fileStream.Write(array, 0, array.Length);
			}
			finally
			{
				Array.Clear(array, 0, array.Length);
				fileStream.Close();
			}
		}

		public static PrivateKey CreateFromFile(string filename)
		{
			return PrivateKey.CreateFromFile(filename, null);
		}

		public static PrivateKey CreateFromFile(string filename, string password)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			byte[] array = null;
			using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
			}
			return new PrivateKey(array, password);
		}

		private const uint magic = 2964713758U;

		private bool encrypted;

		private RSA rsa;

		private bool weak;

		private int keyType;
	}
}
