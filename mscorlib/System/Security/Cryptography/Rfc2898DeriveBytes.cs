using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class Rfc2898DeriveBytes : DeriveBytes
	{
		public Rfc2898DeriveBytes(string password, int saltSize)
			: this(password, saltSize, 1000)
		{
		}

		[SecuritySafeCritical]
		public Rfc2898DeriveBytes(string password, int saltSize, int iterations)
		{
			if (saltSize < 0)
			{
				throw new ArgumentOutOfRangeException("saltSize", Environment.GetResourceString("Non-negative number required."));
			}
			byte[] array = new byte[saltSize];
			Utils.StaticRandomNumberGenerator.GetBytes(array);
			this.Salt = array;
			this.IterationCount = iterations;
			this.m_password = new UTF8Encoding(false).GetBytes(password);
			this.m_hmacsha1 = new HMACSHA1(this.m_password);
			this.Initialize();
		}

		public Rfc2898DeriveBytes(string password, byte[] salt)
			: this(password, salt, 1000)
		{
		}

		public Rfc2898DeriveBytes(string password, byte[] salt, int iterations)
			: this(new UTF8Encoding(false).GetBytes(password), salt, iterations)
		{
		}

		[SecuritySafeCritical]
		public Rfc2898DeriveBytes(byte[] password, byte[] salt, int iterations)
		{
			this.Salt = salt;
			this.IterationCount = iterations;
			this.m_password = password;
			this.m_hmacsha1 = new HMACSHA1(password);
			this.Initialize();
		}

		public int IterationCount
		{
			get
			{
				return (int)this.m_iterations;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("Positive number required."));
				}
				this.m_iterations = (uint)value;
				this.Initialize();
			}
		}

		public byte[] Salt
		{
			get
			{
				return (byte[])this.m_salt.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length < 8)
				{
					throw new ArgumentException(Environment.GetResourceString("Salt is not at least eight bytes."));
				}
				this.m_salt = (byte[])value.Clone();
				this.Initialize();
			}
		}

		public override byte[] GetBytes(int cb)
		{
			if (cb <= 0)
			{
				throw new ArgumentOutOfRangeException("cb", Environment.GetResourceString("Positive number required."));
			}
			byte[] array = new byte[cb];
			int i = 0;
			int num = this.m_endIndex - this.m_startIndex;
			if (num > 0)
			{
				if (cb < num)
				{
					Buffer.InternalBlockCopy(this.m_buffer, this.m_startIndex, array, 0, cb);
					this.m_startIndex += cb;
					return array;
				}
				Buffer.InternalBlockCopy(this.m_buffer, this.m_startIndex, array, 0, num);
				this.m_startIndex = (this.m_endIndex = 0);
				i += num;
			}
			while (i < cb)
			{
				byte[] array2 = this.Func();
				int num2 = cb - i;
				if (num2 <= 20)
				{
					Buffer.InternalBlockCopy(array2, 0, array, i, num2);
					i += num2;
					Buffer.InternalBlockCopy(array2, num2, this.m_buffer, this.m_startIndex, 20 - num2);
					this.m_endIndex += 20 - num2;
					return array;
				}
				Buffer.InternalBlockCopy(array2, 0, array, i, 20);
				i += 20;
			}
			return array;
		}

		public override void Reset()
		{
			this.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (this.m_hmacsha1 != null)
				{
					((IDisposable)this.m_hmacsha1).Dispose();
				}
				if (this.m_buffer != null)
				{
					Array.Clear(this.m_buffer, 0, this.m_buffer.Length);
				}
				if (this.m_salt != null)
				{
					Array.Clear(this.m_salt, 0, this.m_salt.Length);
				}
			}
		}

		private void Initialize()
		{
			if (this.m_buffer != null)
			{
				Array.Clear(this.m_buffer, 0, this.m_buffer.Length);
			}
			this.m_buffer = new byte[20];
			this.m_block = 1U;
			this.m_startIndex = (this.m_endIndex = 0);
		}

		private byte[] Func()
		{
			byte[] array = Utils.Int(this.m_block);
			this.m_hmacsha1.TransformBlock(this.m_salt, 0, this.m_salt.Length, null, 0);
			this.m_hmacsha1.TransformBlock(array, 0, array.Length, null, 0);
			this.m_hmacsha1.TransformFinalBlock(EmptyArray<byte>.Value, 0, 0);
			byte[] array2 = this.m_hmacsha1.HashValue;
			this.m_hmacsha1.Initialize();
			byte[] array3 = array2;
			int num = 2;
			while ((long)num <= (long)((ulong)this.m_iterations))
			{
				this.m_hmacsha1.TransformBlock(array2, 0, array2.Length, null, 0);
				this.m_hmacsha1.TransformFinalBlock(EmptyArray<byte>.Value, 0, 0);
				array2 = this.m_hmacsha1.HashValue;
				for (int i = 0; i < 20; i++)
				{
					byte[] array4 = array3;
					int num2 = i;
					array4[num2] ^= array2[i];
				}
				this.m_hmacsha1.Initialize();
				num++;
			}
			this.m_block += 1U;
			return array3;
		}

		[SecuritySafeCritical]
		public byte[] CryptDeriveKey(string algname, string alghashname, int keySize, byte[] rgbIV)
		{
			if (keySize < 0)
			{
				throw new CryptographicException(Environment.GetResourceString("Specified key is not a valid size for this algorithm."));
			}
			throw new NotSupportedException("CspParameters are not supported by Mono");
		}

		private byte[] m_buffer;

		private byte[] m_salt;

		private HMACSHA1 m_hmacsha1;

		private byte[] m_password;

		private uint m_iterations;

		private uint m_block;

		private int m_startIndex;

		private int m_endIndex;

		private const int BlockSize = 20;
	}
}
