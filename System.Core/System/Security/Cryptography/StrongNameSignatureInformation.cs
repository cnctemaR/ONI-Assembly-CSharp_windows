using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class StrongNameSignatureInformation
	{
		internal StrongNameSignatureInformation()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public string HashAlgorithm
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public int HResult
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public bool IsValid
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		public AsymmetricAlgorithm PublicKey
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public SignatureVerificationResult VerificationResult
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return SignatureVerificationResult.Valid;
			}
		}
	}
}
