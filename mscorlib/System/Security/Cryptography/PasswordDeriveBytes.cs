using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class PasswordDeriveBytes : DeriveBytes
	{
		public PasswordDeriveBytes(string strPassword, byte[] rgbSalt)
		{
			this.Prepare(strPassword, rgbSalt, "SHA1", 100);
		}

		public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, CspParameters cspParams)
		{
			this.Prepare(strPassword, rgbSalt, "SHA1", 100);
			if (cspParams != null)
			{
				throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono for PasswordDeriveBytes."));
			}
		}

		public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, string strHashName, int iterations)
		{
			this.Prepare(strPassword, rgbSalt, strHashName, iterations);
		}

		public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, string strHashName, int iterations, CspParameters cspParams)
		{
			this.Prepare(strPassword, rgbSalt, strHashName, iterations);
			if (cspParams != null)
			{
				throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono for PasswordDeriveBytes."));
			}
		}

		public PasswordDeriveBytes(byte[] password, byte[] salt)
		{
			this.Prepare(password, salt, "SHA1", 100);
		}

		public PasswordDeriveBytes(byte[] password, byte[] salt, CspParameters cspParams)
		{
			this.Prepare(password, salt, "SHA1", 100);
			if (cspParams != null)
			{
				throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono for PasswordDeriveBytes."));
			}
		}

		public PasswordDeriveBytes(byte[] password, byte[] salt, string hashName, int iterations)
		{
			this.Prepare(password, salt, hashName, iterations);
		}

		public PasswordDeriveBytes(byte[] password, byte[] salt, string hashName, int iterations, CspParameters cspParams)
		{
			this.Prepare(password, salt, hashName, iterations);
			if (cspParams != null)
			{
				throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono for PasswordDeriveBytes."));
			}
		}

		~PasswordDeriveBytes()
		{
			if (this.initial != null)
			{
				Array.Clear(this.initial, 0, this.initial.Length);
				this.initial = null;
			}
			Array.Clear(this.password, 0, this.password.Length);
		}

		private void Prepare(string strPassword, byte[] rgbSalt, string strHashName, int iterations)
		{
			if (strPassword == null)
			{
				throw new ArgumentNullException("strPassword");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(strPassword);
			this.Prepare(bytes, rgbSalt, strHashName, iterations);
			Array.Clear(bytes, 0, bytes.Length);
		}

		private void Prepare(byte[] password, byte[] rgbSalt, string strHashName, int iterations)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			this.password = (byte[])password.Clone();
			this.Salt = rgbSalt;
			this.HashName = strHashName;
			this.IterationCount = iterations;
			this.state = 0;
		}

		public string HashName
		{
			get
			{
				return this.HashNameValue;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("HashName");
				}
				if (this.state != 0)
				{
					throw new CryptographicException(Locale.GetText("Can't change this property at this stage"));
				}
				this.HashNameValue = value;
			}
		}

		public int IterationCount
		{
			get
			{
				return this.IterationsValue;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("> 0", "IterationCount");
				}
				if (this.state != 0)
				{
					throw new CryptographicException(Locale.GetText("Can't change this property at this stage"));
				}
				this.IterationsValue = value;
			}
		}

		public byte[] Salt
		{
			get
			{
				if (this.SaltValue == null)
				{
					return null;
				}
				return (byte[])this.SaltValue.Clone();
			}
			set
			{
				if (this.state != 0)
				{
					throw new CryptographicException(Locale.GetText("Can't change this property at this stage"));
				}
				if (value != null)
				{
					this.SaltValue = (byte[])value.Clone();
				}
				else
				{
					this.SaltValue = null;
				}
			}
		}

		public byte[] CryptDeriveKey(string algname, string alghashname, int keySize, byte[] rgbIV)
		{
			if (keySize > 128)
			{
				throw new CryptographicException(Locale.GetText("Key Size can't be greater than 128 bits"));
			}
			throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono"));
		}

		[Obsolete("see Rfc2898DeriveBytes for PKCS#5 v2 support")]
		public override byte[] GetBytes(int cb)
		{
			if (cb < 1)
			{
				throw new IndexOutOfRangeException("cb");
			}
			if (this.state == 0)
			{
				this.Reset();
				this.state = 1;
			}
			byte[] array = new byte[cb];
			int i = 0;
			int num = Math.Max(1, this.IterationsValue - 1);
			if (this.output == null)
			{
				this.output = this.initial;
				for (int j = 0; j < num - 1; j++)
				{
					this.output = this.hash.ComputeHash(this.output);
				}
			}
			while (i < cb)
			{
				byte[] array2;
				if (this.hashnumber == 0)
				{
					array2 = this.hash.ComputeHash(this.output);
				}
				else
				{
					if (this.hashnumber >= 1000)
					{
						throw new CryptographicException(Locale.GetText("too long"));
					}
					string text = Convert.ToString(this.hashnumber);
					array2 = new byte[this.output.Length + text.Length];
					for (int k = 0; k < text.Length; k++)
					{
						array2[k] = (byte)text[k];
					}
					Buffer.BlockCopy(this.output, 0, array2, text.Length, this.output.Length);
					array2 = this.hash.ComputeHash(array2);
				}
				int num2 = array2.Length - this.position;
				int num3 = Math.Min(cb - i, num2);
				Buffer.BlockCopy(array2, this.position, array, i, num3);
				i += num3;
				this.position += num3;
				while (this.position >= array2.Length)
				{
					this.position -= array2.Length;
					this.hashnumber++;
				}
			}
			return array;
		}

		public override void Reset()
		{
			this.state = 0;
			this.position = 0;
			this.hashnumber = 0;
			this.hash = HashAlgorithm.Create(this.HashNameValue);
			if (this.SaltValue != null)
			{
				this.hash.TransformBlock(this.password, 0, this.password.Length, this.password, 0);
				this.hash.TransformFinalBlock(this.SaltValue, 0, this.SaltValue.Length);
				this.initial = this.hash.Hash;
			}
			else
			{
				this.initial = this.hash.ComputeHash(this.password);
			}
		}

		private string HashNameValue;

		private byte[] SaltValue;

		private int IterationsValue;

		private HashAlgorithm hash;

		private int state;

		private byte[] password;

		private byte[] initial;

		private byte[] output;

		private int position;

		private int hashnumber;
	}
}
