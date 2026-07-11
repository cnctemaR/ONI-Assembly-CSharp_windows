using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ManifestSignatureInformation
	{
		internal ManifestSignatureInformation()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public AuthenticodeSignatureInformation AuthenticodeSignature
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public ManifestKinds Manifest
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return ManifestKinds.None;
			}
		}

		public StrongNameSignatureInformation StrongNameSignature
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public static ManifestSignatureInformationCollection VerifySignature(ActivationContext application)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public static ManifestSignatureInformationCollection VerifySignature(ActivationContext application, ManifestKinds manifests)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecuritySafeCritical]
		public static ManifestSignatureInformationCollection VerifySignature(ActivationContext application, ManifestKinds manifests, X509RevocationFlag revocationFlag, X509RevocationMode revocationMode)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
