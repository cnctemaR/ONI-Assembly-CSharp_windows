using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class ECDiffieHellmanCngPublicKey : ECDiffieHellmanPublicKey
	{
		internal ECDiffieHellmanCngPublicKey()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public CngKeyBlobFormat BlobFormat
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		[SecuritySafeCritical]
		public static ECDiffieHellmanPublicKey FromByteArray(byte[] publicKeyBlob, CngKeyBlobFormat format)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecuritySafeCritical]
		public static ECDiffieHellmanCngPublicKey FromXmlString(string xml)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public CngKey Import()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
