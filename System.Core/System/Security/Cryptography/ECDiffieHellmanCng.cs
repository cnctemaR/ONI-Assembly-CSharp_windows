using System;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using Unity;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ECDiffieHellmanCng : ECDiffieHellman
	{
		public ECDiffieHellmanCng()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public ECDiffieHellmanCng(int keySize)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecuritySafeCritical]
		public ECDiffieHellmanCng(CngKey key)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public ECDiffieHellmanCng(ECCurve curve)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public CngAlgorithm HashAlgorithm
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public byte[] HmacKey
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public CngKey Key
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public ECDiffieHellmanKeyDerivationFunction KeyDerivationFunction
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return ECDiffieHellmanKeyDerivationFunction.Hash;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public byte[] Label
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public override ECDiffieHellmanPublicKey PublicKey
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public byte[] SecretAppend
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public byte[] SecretPrepend
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public byte[] Seed
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public bool UseSecretAgreementAsHmacKey
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		[SecuritySafeCritical]
		public byte[] DeriveKeyMaterial(CngKey otherPartyPublicKey)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
		public SafeNCryptSecretHandle DeriveSecretAgreementHandle(CngKey otherPartyPublicKey)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public SafeNCryptSecretHandle DeriveSecretAgreementHandle(ECDiffieHellmanPublicKey otherPartyPublicKey)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public void FromXmlString(string xml, ECKeyXmlFormat format)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public string ToXmlString(ECKeyXmlFormat format)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
