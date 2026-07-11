using System;

namespace System.Security.Cryptography.X509Certificates
{
	internal abstract class X509Certificate2Impl : X509CertificateImpl
	{
		public abstract bool Archived { get; set; }

		public abstract X509ExtensionCollection Extensions { get; }

		public abstract bool HasPrivateKey { get; }

		public abstract X500DistinguishedName IssuerName { get; }

		public abstract AsymmetricAlgorithm PrivateKey { get; set; }

		public abstract PublicKey PublicKey { get; }

		public abstract Oid SignatureAlgorithm { get; }

		public abstract X500DistinguishedName SubjectName { get; }

		public abstract int Version { get; }

		internal abstract X509CertificateImplCollection IntermediateCertificates { get; }

		internal abstract X509Certificate2Impl FallbackImpl { get; }

		public abstract string GetNameInfo(X509NameType nameType, bool forIssuer);

		public abstract void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags);

		public abstract byte[] Export(X509ContentType contentType, string password);

		public abstract bool Verify(X509Certificate2 thisCertificate);

		public abstract void Reset();
	}
}
