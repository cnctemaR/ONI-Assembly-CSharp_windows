using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography.X509Certificates
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class AuthenticodeSignatureInformation
	{
		internal AuthenticodeSignatureInformation()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public string Description
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public Uri DescriptionUrl
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
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

		public X509Chain SignatureChain
		{
			[SecuritySafeCritical]
			[StorePermission(SecurityAction.Demand, OpenStore = true, EnumerateCertificates = true)]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public X509Certificate2 SigningCertificate
		{
			[SecuritySafeCritical]
			[StorePermission(SecurityAction.Demand, OpenStore = true, EnumerateCertificates = true)]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public TimestampInformation Timestamp
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public TrustStatus TrustStatus
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return TrustStatus.Untrusted;
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
