using System;

namespace System.Security.Cryptography
{
	[Serializable]
	public sealed class CngAlgorithm : IEquatable<CngAlgorithm>
	{
		public CngAlgorithm(string algorithm)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			if (algorithm.Length == 0)
			{
				throw new ArgumentException("algorithm");
			}
			this.algo = algorithm;
		}

		public string Algorithm
		{
			get
			{
				return this.algo;
			}
		}

		public bool Equals(CngAlgorithm other)
		{
			return !(other == null) && this.algo == other.algo;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CngAlgorithm);
		}

		public override int GetHashCode()
		{
			return this.algo.GetHashCode();
		}

		public override string ToString()
		{
			return this.algo;
		}

		public static CngAlgorithm ECDiffieHellmanP256
		{
			get
			{
				if (CngAlgorithm.dh256 == null)
				{
					CngAlgorithm.dh256 = new CngAlgorithm("ECDH_P256");
				}
				return CngAlgorithm.dh256;
			}
		}

		public static CngAlgorithm ECDiffieHellmanP384
		{
			get
			{
				if (CngAlgorithm.dh384 == null)
				{
					CngAlgorithm.dh384 = new CngAlgorithm("ECDH_P384");
				}
				return CngAlgorithm.dh384;
			}
		}

		public static CngAlgorithm ECDiffieHellmanP521
		{
			get
			{
				if (CngAlgorithm.dh521 == null)
				{
					CngAlgorithm.dh521 = new CngAlgorithm("ECDH_P521");
				}
				return CngAlgorithm.dh521;
			}
		}

		public static CngAlgorithm ECDsaP256
		{
			get
			{
				if (CngAlgorithm.dsa256 == null)
				{
					CngAlgorithm.dsa256 = new CngAlgorithm("ECDSA_P256");
				}
				return CngAlgorithm.dsa256;
			}
		}

		public static CngAlgorithm ECDsaP384
		{
			get
			{
				if (CngAlgorithm.dsa384 == null)
				{
					CngAlgorithm.dsa384 = new CngAlgorithm("ECDSA_P384");
				}
				return CngAlgorithm.dsa384;
			}
		}

		public static CngAlgorithm ECDsaP521
		{
			get
			{
				if (CngAlgorithm.dsa521 == null)
				{
					CngAlgorithm.dsa521 = new CngAlgorithm("ECDSA_P521");
				}
				return CngAlgorithm.dsa521;
			}
		}

		public static CngAlgorithm MD5
		{
			get
			{
				if (CngAlgorithm.md5 == null)
				{
					CngAlgorithm.md5 = new CngAlgorithm("MD5");
				}
				return CngAlgorithm.md5;
			}
		}

		public static CngAlgorithm Sha1
		{
			get
			{
				if (CngAlgorithm.sha1 == null)
				{
					CngAlgorithm.sha1 = new CngAlgorithm("SHA1");
				}
				return CngAlgorithm.sha1;
			}
		}

		public static CngAlgorithm Sha256
		{
			get
			{
				if (CngAlgorithm.sha256 == null)
				{
					CngAlgorithm.sha256 = new CngAlgorithm("SHA256");
				}
				return CngAlgorithm.sha256;
			}
		}

		public static CngAlgorithm Sha384
		{
			get
			{
				if (CngAlgorithm.sha384 == null)
				{
					CngAlgorithm.sha384 = new CngAlgorithm("SHA384");
				}
				return CngAlgorithm.sha384;
			}
		}

		public static CngAlgorithm Sha512
		{
			get
			{
				if (CngAlgorithm.sha512 == null)
				{
					CngAlgorithm.sha512 = new CngAlgorithm("SHA512");
				}
				return CngAlgorithm.sha512;
			}
		}

		public static bool operator ==(CngAlgorithm left, CngAlgorithm right)
		{
			if (left == null)
			{
				return right == null;
			}
			return right != null && left.algo == right.algo;
		}

		public static bool operator !=(CngAlgorithm left, CngAlgorithm right)
		{
			if (left == null)
			{
				return right != null;
			}
			return right == null || left.algo != right.algo;
		}

		private string algo;

		private static CngAlgorithm dh256;

		private static CngAlgorithm dh384;

		private static CngAlgorithm dh521;

		private static CngAlgorithm dsa256;

		private static CngAlgorithm dsa384;

		private static CngAlgorithm dsa521;

		private static CngAlgorithm md5;

		private static CngAlgorithm sha1;

		private static CngAlgorithm sha256;

		private static CngAlgorithm sha384;

		private static CngAlgorithm sha512;
	}
}
