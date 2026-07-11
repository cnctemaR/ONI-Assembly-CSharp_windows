using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
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
				throw new ArgumentException(global::SR.GetString("The algorithm group '{0}' is invalid.", new object[] { algorithmGroup }), "algorithmGroup");
			}
			this.m_algorithmGroup = algorithmGroup;
		}

		public string AlgorithmGroup
		{
			get
			{
				return this.m_algorithmGroup;
			}
		}

		public static bool operator ==(CngAlgorithmGroup left, CngAlgorithmGroup right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(CngAlgorithmGroup left, CngAlgorithmGroup right)
		{
			if (left == null)
			{
				return right != null;
			}
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CngAlgorithmGroup);
		}

		public bool Equals(CngAlgorithmGroup other)
		{
			return other != null && this.m_algorithmGroup.Equals(other.AlgorithmGroup);
		}

		public override int GetHashCode()
		{
			return this.m_algorithmGroup.GetHashCode();
		}

		public override string ToString()
		{
			return this.m_algorithmGroup;
		}

		public static CngAlgorithmGroup DiffieHellman
		{
			get
			{
				if (CngAlgorithmGroup.s_dh == null)
				{
					CngAlgorithmGroup.s_dh = new CngAlgorithmGroup("DH");
				}
				return CngAlgorithmGroup.s_dh;
			}
		}

		public static CngAlgorithmGroup Dsa
		{
			get
			{
				if (CngAlgorithmGroup.s_dsa == null)
				{
					CngAlgorithmGroup.s_dsa = new CngAlgorithmGroup("DSA");
				}
				return CngAlgorithmGroup.s_dsa;
			}
		}

		public static CngAlgorithmGroup ECDiffieHellman
		{
			get
			{
				if (CngAlgorithmGroup.s_ecdh == null)
				{
					CngAlgorithmGroup.s_ecdh = new CngAlgorithmGroup("ECDH");
				}
				return CngAlgorithmGroup.s_ecdh;
			}
		}

		public static CngAlgorithmGroup ECDsa
		{
			get
			{
				if (CngAlgorithmGroup.s_ecdsa == null)
				{
					CngAlgorithmGroup.s_ecdsa = new CngAlgorithmGroup("ECDSA");
				}
				return CngAlgorithmGroup.s_ecdsa;
			}
		}

		public static CngAlgorithmGroup Rsa
		{
			get
			{
				if (CngAlgorithmGroup.s_rsa == null)
				{
					CngAlgorithmGroup.s_rsa = new CngAlgorithmGroup("RSA");
				}
				return CngAlgorithmGroup.s_rsa;
			}
		}

		private static volatile CngAlgorithmGroup s_dh;

		private static volatile CngAlgorithmGroup s_dsa;

		private static volatile CngAlgorithmGroup s_ecdh;

		private static volatile CngAlgorithmGroup s_ecdsa;

		private static volatile CngAlgorithmGroup s_rsa;

		private string m_algorithmGroup;
	}
}
