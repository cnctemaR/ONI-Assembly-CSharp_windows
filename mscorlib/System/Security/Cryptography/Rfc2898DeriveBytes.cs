using System;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class Rfc2898DeriveBytes : DeriveBytes
	{
		public Rfc2898DeriveBytes(string password, byte[] salt)
			: this(password, salt, 1000)
		{
		}

		public Rfc2898DeriveBytes(string password, byte[] salt, int iterations)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			this.Salt = salt;
			this.IterationCount = iterations;
			this._hmac = new HMACSHA1(Encoding.UTF8.GetBytes(password));
		}

		public Rfc2898DeriveBytes(byte[] password, byte[] salt, int iterations)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			this.Salt = salt;
			this.IterationCount = iterations;
			this._hmac = new HMACSHA1(password);
		}

		public Rfc2898DeriveBytes(string password, int saltSize)
			: this(password, saltSize, 1000)
		{
		}

		public Rfc2898DeriveBytes(string password, int saltSize, int iterations)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			if (saltSize < 0)
			{
				throw new ArgumentOutOfRangeException("invalid salt length");
			}
			this.Salt = KeyBuilder.Key(saltSize);
			this.IterationCount = iterations;
			this._hmac = new HMACSHA1(Encoding.UTF8.GetBytes(password));
		}

		public int IterationCount
		{
			get
			{
				return this._iteration;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("IterationCount < 1");
				}
				this._iteration = value;
			}
		}

		public byte[] Salt
		{
			get
			{
				return (byte[])this._salt.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Salt");
				}
				if (value.Length < 8)
				{
					throw new ArgumentException("Salt < 8 bytes");
				}
				this._salt = (byte[])value.Clone();
			}
		}

		private byte[] F(byte[] s, int c, int i)
		{
			s[s.Length - 4] = (byte)(i >> 24);
			s[s.Length - 3] = (byte)(i >> 16);
			s[s.Length - 2] = (byte)(i >> 8);
			s[s.Length - 1] = (byte)i;
			byte[] array = this._hmac.ComputeHash(s);
			byte[] array2 = array;
			for (int j = 1; j < c; j++)
			{
				byte[] array3 = this._hmac.ComputeHash(array2);
				for (int k = 0; k < 20; k++)
				{
					array[k] ^= array3[k];
				}
				array2 = array3;
			}
			return array;
		}

		public override byte[] GetBytes(int cb)
		{
			if (cb < 1)
			{
				throw new ArgumentOutOfRangeException("cb");
			}
			int num = cb / 20;
			int num2 = cb % 20;
			if (num2 != 0)
			{
				num++;
			}
			byte[] array = new byte[cb];
			int num3 = 0;
			if (this._pos > 0)
			{
				int num4 = Math.Min(20 - this._pos, cb);
				Buffer.BlockCopy(this._buffer, this._pos, array, 0, num4);
				if (num4 >= cb)
				{
					return array;
				}
				this._pos = 0;
				num3 = num4;
			}
			byte[] array2 = new byte[this._salt.Length + 4];
			Buffer.BlockCopy(this._salt, 0, array2, 0, this._salt.Length);
			for (int i = 1; i <= num; i++)
			{
				this._buffer = this.F(array2, this._iteration, ++this._f);
				int num5 = ((i != num) ? 20 : (array.Length - num3));
				Buffer.BlockCopy(this._buffer, this._pos, array, num3, num5);
				num3 += this._pos + num5;
				this._pos = ((num5 != 20) ? num5 : 0);
			}
			return array;
		}

		public override void Reset()
		{
			this._buffer = null;
			this._pos = 0;
			this._f = 0;
		}

		private const int defaultIterations = 1000;

		private int _iteration;

		private byte[] _salt;

		private HMACSHA1 _hmac;

		private byte[] _buffer;

		private int _pos;

		private int _f;
	}
}
