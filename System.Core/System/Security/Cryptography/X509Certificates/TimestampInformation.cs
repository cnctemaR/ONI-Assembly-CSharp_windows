using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography.X509Certificates
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class TimestampInformation
	{
		internal TimestampInformation()
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

		public DateTime Timestamp
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(DateTime);
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
