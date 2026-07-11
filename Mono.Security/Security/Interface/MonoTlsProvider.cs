using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Mono.Security.Interface
{
	public abstract class MonoTlsProvider
	{
		internal MonoTlsProvider()
		{
		}

		public abstract Guid ID { get; }

		public abstract string Name { get; }

		public abstract bool SupportsSslStream { get; }

		public abstract bool SupportsConnectionInfo { get; }

		public abstract bool SupportsMonoExtensions { get; }

		public abstract SslProtocols SupportedProtocols { get; }

		public abstract IMonoSslStream CreateSslStream(Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings = null);

		internal abstract IMonoSslStream CreateSslStreamInternal(SslStream sslStream, Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings);

		internal virtual bool HasNativeCertificates
		{
			get
			{
				return false;
			}
		}

		internal virtual X509Certificate2Impl GetNativeCertificate(byte[] data, string password, X509KeyStorageFlags flags)
		{
			throw new InvalidOperationException();
		}

		internal virtual X509Certificate2Impl GetNativeCertificate(X509Certificate certificate)
		{
			throw new InvalidOperationException();
		}

		internal abstract bool ValidateCertificate(ICertificateValidator2 validator, string targetHost, bool serverMode, X509CertificateCollection certificates, bool wantsChain, ref X509Chain chain, ref MonoSslPolicyErrors errors, ref int status11);

		internal abstract bool SupportsCleanShutdown { get; }
	}
}
