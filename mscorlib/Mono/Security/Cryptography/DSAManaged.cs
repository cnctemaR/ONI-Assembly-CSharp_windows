using System;
using System.Security.Cryptography;
using Mono.Math;

namespace Mono.Security.Cryptography
{
	internal class DSAManaged : DSA
	{
		public DSAManaged()
			: this(1024)
		{
		}

		public DSAManaged(int dwKeySize)
		{
			this.KeySizeValue = dwKeySize;
			this.LegalKeySizesValue = new KeySizes[1];
			this.LegalKeySizesValue[0] = new KeySizes(512, 1024, 64);
		}

		public event DSAManaged.KeyGeneratedEventHandler KeyGenerated;

		~DSAManaged()
		{
			this.Dispose(false);
		}

		private void Generate()
		{
			this.GenerateParams(base.KeySize);
			this.GenerateKeyPair();
			this.keypairGenerated = true;
			if (this.KeyGenerated != null)
			{
				this.KeyGenerated(this, null);
			}
		}

		private void GenerateKeyPair()
		{
			this.x = BigInteger.GenerateRandom(160);
			while (this.x == 0U || this.x >= this.q)
			{
				this.x.Randomize();
			}
			this.y = this.g.ModPow(this.x, this.p);
		}

		private void add(byte[] a, byte[] b, int value)
		{
			uint num = (uint)((int)(b[b.Length - 1] & byte.MaxValue) + value);
			a[b.Length - 1] = (byte)num;
			num >>= 8;
			for (int i = b.Length - 2; i >= 0; i--)
			{
				num += (uint)(b[i] & byte.MaxValue);
				a[i] = (byte)num;
				num >>= 8;
			}
		}

		private void GenerateParams(int keyLength)
		{
			byte[] array = new byte[20];
			byte[] array2 = new byte[20];
			byte[] array3 = new byte[20];
			byte[] array4 = new byte[20];
			SHA1 sha = SHA1.Create();
			int num = (keyLength - 1) / 160;
			byte[] array5 = new byte[keyLength / 8];
			bool flag = false;
			while (!flag)
			{
				do
				{
					this.Random.GetBytes(array);
					array2 = sha.ComputeHash(array);
					Array.Copy(array, 0, array3, 0, array.Length);
					this.add(array3, array, 1);
					array3 = sha.ComputeHash(array3);
					for (int num2 = 0; num2 != array4.Length; num2++)
					{
						array4[num2] = array2[num2] ^ array3[num2];
					}
					byte[] array6 = array4;
					int num3 = 0;
					array6[num3] |= 128;
					byte[] array7 = array4;
					int num4 = 19;
					array7[num4] |= 1;
					this.q = new BigInteger(array4);
				}
				while (!this.q.IsProbablePrime());
				this.counter = 0;
				int num5 = 2;
				while (this.counter < 4096)
				{
					for (int i = 0; i < num; i++)
					{
						this.add(array2, array, num5 + i);
						array2 = sha.ComputeHash(array2);
						Array.Copy(array2, 0, array5, array5.Length - (i + 1) * array2.Length, array2.Length);
					}
					this.add(array2, array, num5 + num);
					array2 = sha.ComputeHash(array2);
					Array.Copy(array2, array2.Length - (array5.Length - num * array2.Length), array5, 0, array5.Length - num * array2.Length);
					byte[] array8 = array5;
					int num6 = 0;
					array8[num6] |= 128;
					BigInteger bigInteger = new BigInteger(array5);
					BigInteger bigInteger2 = bigInteger % (this.q * 2);
					this.p = bigInteger - (bigInteger2 - 1);
					if (this.p.TestBit((uint)(keyLength - 1)) && this.p.IsProbablePrime())
					{
						flag = true;
						break;
					}
					this.counter++;
					num5 += num + 1;
				}
			}
			BigInteger bigInteger3 = (this.p - 1) / this.q;
			for (;;)
			{
				BigInteger bigInteger4 = BigInteger.GenerateRandom(keyLength);
				if (!(bigInteger4 <= 1) && !(bigInteger4 >= this.p - 1))
				{
					this.g = bigInteger4.ModPow(bigInteger3, this.p);
					if (!(this.g <= 1))
					{
						break;
					}
				}
			}
			this.seed = new BigInteger(array);
			this.j = (this.p - 1) / this.q;
		}

		private RandomNumberGenerator Random
		{
			get
			{
				if (this.rng == null)
				{
					this.rng = RandomNumberGenerator.Create();
				}
				return this.rng;
			}
		}

		public override int KeySize
		{
			get
			{
				if (this.keypairGenerated)
				{
					return this.p.BitCount();
				}
				return base.KeySize;
			}
		}

		public override string KeyExchangeAlgorithm
		{
			get
			{
				return null;
			}
		}

		public bool PublicOnly
		{
			get
			{
				return this.keypairGenerated && this.x == null;
			}
		}

		public override string SignatureAlgorithm
		{
			get
			{
				return "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
			}
		}

		private byte[] NormalizeArray(byte[] array)
		{
			int num = array.Length % 4;
			if (num > 0)
			{
				byte[] array2 = new byte[array.Length + 4 - num];
				Array.Copy(array, 0, array2, 4 - num, array.Length);
				return array2;
			}
			return array;
		}

		public override DSAParameters ExportParameters(bool includePrivateParameters)
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException(Locale.GetText("Keypair was disposed"));
			}
			if (!this.keypairGenerated)
			{
				this.Generate();
			}
			if (includePrivateParameters && this.x == null)
			{
				throw new CryptographicException("no private key to export");
			}
			DSAParameters dsaparameters = default(DSAParameters);
			dsaparameters.P = this.NormalizeArray(this.p.GetBytes());
			dsaparameters.Q = this.NormalizeArray(this.q.GetBytes());
			dsaparameters.G = this.NormalizeArray(this.g.GetBytes());
			dsaparameters.Y = this.NormalizeArray(this.y.GetBytes());
			if (!this.j_missing)
			{
				dsaparameters.J = this.NormalizeArray(this.j.GetBytes());
			}
			if (this.seed != 0U)
			{
				dsaparameters.Seed = this.NormalizeArray(this.seed.GetBytes());
				dsaparameters.Counter = this.counter;
			}
			if (includePrivateParameters)
			{
				byte[] bytes = this.x.GetBytes();
				if (bytes.Length == 20)
				{
					dsaparameters.X = this.NormalizeArray(bytes);
				}
			}
			return dsaparameters;
		}

		public override void ImportParameters(DSAParameters parameters)
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException(Locale.GetText("Keypair was disposed"));
			}
			if (parameters.P == null || parameters.Q == null || parameters.G == null)
			{
				throw new CryptographicException(Locale.GetText("Missing mandatory DSA parameters (P, Q or G)."));
			}
			if (parameters.X == null && parameters.Y == null)
			{
				throw new CryptographicException(Locale.GetText("Missing both public (Y) and private (X) keys."));
			}
			this.p = new BigInteger(parameters.P);
			this.q = new BigInteger(parameters.Q);
			this.g = new BigInteger(parameters.G);
			if (parameters.X != null)
			{
				this.x = new BigInteger(parameters.X);
			}
			else
			{
				this.x = null;
			}
			if (parameters.Y != null)
			{
				this.y = new BigInteger(parameters.Y);
			}
			else
			{
				this.y = this.g.ModPow(this.x, this.p);
			}
			if (parameters.J != null)
			{
				this.j = new BigInteger(parameters.J);
			}
			else
			{
				this.j = (this.p - 1) / this.q;
				this.j_missing = true;
			}
			if (parameters.Seed != null)
			{
				this.seed = new BigInteger(parameters.Seed);
				this.counter = parameters.Counter;
			}
			else
			{
				this.seed = 0;
			}
			this.keypairGenerated = true;
		}

		public override byte[] CreateSignature(byte[] rgbHash)
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException(Locale.GetText("Keypair was disposed"));
			}
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			if (rgbHash.Length != 20)
			{
				throw new CryptographicException("invalid hash length");
			}
			if (!this.keypairGenerated)
			{
				this.Generate();
			}
			if (this.x == null)
			{
				throw new CryptographicException("no private key available for signature");
			}
			BigInteger bigInteger = new BigInteger(rgbHash);
			BigInteger bigInteger2 = BigInteger.GenerateRandom(160);
			while (bigInteger2 >= this.q)
			{
				bigInteger2.Randomize();
			}
			BigInteger bigInteger3 = this.g.ModPow(bigInteger2, this.p) % this.q;
			BigInteger bigInteger4 = bigInteger2.ModInverse(this.q) * (bigInteger + this.x * bigInteger3) % this.q;
			byte[] array = new byte[40];
			byte[] bytes = bigInteger3.GetBytes();
			byte[] bytes2 = bigInteger4.GetBytes();
			int num = 20 - bytes.Length;
			Array.Copy(bytes, 0, array, num, bytes.Length);
			num = 40 - bytes2.Length;
			Array.Copy(bytes2, 0, array, num, bytes2.Length);
			return array;
		}

		public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException(Locale.GetText("Keypair was disposed"));
			}
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			if (rgbSignature == null)
			{
				throw new ArgumentNullException("rgbSignature");
			}
			if (rgbHash.Length != 20)
			{
				throw new CryptographicException("invalid hash length");
			}
			if (rgbSignature.Length != 40)
			{
				throw new CryptographicException("invalid signature length");
			}
			if (!this.keypairGenerated)
			{
				return false;
			}
			bool flag;
			try
			{
				BigInteger bigInteger = new BigInteger(rgbHash);
				byte[] array = new byte[20];
				Array.Copy(rgbSignature, 0, array, 0, 20);
				BigInteger bigInteger2 = new BigInteger(array);
				Array.Copy(rgbSignature, 20, array, 0, 20);
				BigInteger bigInteger3 = new BigInteger(array);
				if (bigInteger2 < 0 || this.q <= bigInteger2)
				{
					flag = false;
				}
				else if (bigInteger3 < 0 || this.q <= bigInteger3)
				{
					flag = false;
				}
				else
				{
					BigInteger bigInteger4 = bigInteger3.ModInverse(this.q);
					BigInteger bigInteger5 = bigInteger * bigInteger4 % this.q;
					BigInteger bigInteger6 = bigInteger2 * bigInteger4 % this.q;
					bigInteger5 = this.g.ModPow(bigInteger5, this.p);
					bigInteger6 = this.y.ModPow(bigInteger6, this.p);
					BigInteger bigInteger7 = bigInteger5 * bigInteger6 % this.p % this.q;
					flag = bigInteger7 == bigInteger2;
				}
			}
			catch
			{
				throw new CryptographicException("couldn't compute signature verification");
			}
			return flag;
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (this.x != null)
				{
					this.x.Clear();
					this.x = null;
				}
				if (disposing)
				{
					if (this.p != null)
					{
						this.p.Clear();
						this.p = null;
					}
					if (this.q != null)
					{
						this.q.Clear();
						this.q = null;
					}
					if (this.g != null)
					{
						this.g.Clear();
						this.g = null;
					}
					if (this.j != null)
					{
						this.j.Clear();
						this.j = null;
					}
					if (this.seed != null)
					{
						this.seed.Clear();
						this.seed = null;
					}
					if (this.y != null)
					{
						this.y.Clear();
						this.y = null;
					}
				}
			}
			this.m_disposed = true;
		}

		private const int defaultKeySize = 1024;

		private bool keypairGenerated;

		private bool m_disposed;

		private BigInteger p;

		private BigInteger q;

		private BigInteger g;

		private BigInteger x;

		private BigInteger y;

		private BigInteger j;

		private BigInteger seed;

		private int counter;

		private bool j_missing;

		private RandomNumberGenerator rng;

		public delegate void KeyGeneratedEventHandler(object sender, EventArgs e);
	}
}
