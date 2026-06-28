using System;

namespace System.Security.Cryptography
{
	[Serializable]
	public sealed class CngAlgorithmGroup : IEquatable<CngAlgorithmGroup>
	{
		public CngAlgorithmGroup(string algorithmGroup)
		{
			if (algorithmGroup == null)
			{
				throw new ArgumentNullException("algorithmGroup");
			}
			if (algorithmGroup.Length == 0)
			{
				throw new ArgumentException("algorithmGroup");
			}
			this.group = algorithmGroup;
		}

		public string AlgorithmGroup
		{
			get
			{
				return this.group;
			}
		}

		public bool Equals(CngAlgorithmGroup other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CngAlgorithmGroup);
		}

		public override int GetHashCode()
		{
			return this.group.GetHashCode();
		}

		public override string ToString()
		{
			return this.group;
		}

		public static CngAlgorithmGroup DiffieHellman
		{
			get
			{
				if (CngAlgorithmGroup.dh == null)
				{
					CngAlgorithmGroup.dh = new CngAlgorithmGroup("DH");
				}
				return CngAlgorithmGroup.dh;
			}
		}

		public static CngAlgorithmGroup Dsa
		{
			get
			{
				if (CngAlgorithmGroup.dsa == null)
				{
					CngAlgorithmGroup.dsa = new CngAlgorithmGroup("DSA");
				}
				return CngAlgorithmGroup.dsa;
			}
		}

		public static CngAlgorithmGroup ECDiffieHellman
		{
			get
			{
				if (CngAlgorithmGroup.ecdh == null)
				{
					CngAlgorithmGroup.ecdh = new CngAlgorithmGroup("ECDH");
				}
				return CngAlgorithmGroup.ecdh;
			}
		}

		public static CngAlgorithmGroup ECDsa
		{
			get
			{
				if (CngAlgorithmGroup.ecdsa == null)
				{
					CngAlgorithmGroup.ecdsa = new CngAlgorithmGroup("ECDSA");
				}
				return CngAlgorithmGroup.ecdsa;
			}
		}

		public static CngAlgorithmGroup Rsa
		{
			get
			{
				if (CngAlgorithmGroup.rsa == null)
				{
					CngAlgorithmGroup.rsa = new CngAlgorithmGroup("RSA");
				}
				return CngAlgorithmGroup.rsa;
			}
		}

		public static bool operator ==(CngAlgorithmGroup left, CngAlgorithmGroup right)
		{
			if (left == null)
			{
				return right == null;
			}
			return right != null && left.group == right.group;
		}

		public static bool operator !=(CngAlgorithmGroup left, CngAlgorithmGroup right)
		{
			if (left == null)
			{
				return right != null;
			}
			return right == null || left.group != right.group;
		}

		private string group;

		private static CngAlgorithmGroup dh;

		private static CngAlgorithmGroup dsa;

		private static CngAlgorithmGroup ecdh;

		private static CngAlgorithmGroup ecdsa;

		private static CngAlgorithmGroup rsa;
	}
}
