using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Net.Security;
using Mono.Security.Interface;

namespace Mono.AppleTls
{
	internal class AppleTlsProvider : MonoTlsProvider
	{
		public override string Name
		{
			get
			{
				return "apple-tls";
			}
		}

		public override Guid ID
		{
			get
			{
				return Mono.Net.Security.MonoTlsProviderFactory.AppleTlsId;
			}
		}

		public override IMonoSslStream CreateSslStream(Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings = null)
		{
			return SslStream.CreateMonoSslStream(innerStream, leaveInnerStreamOpen, this, settings);
		}

		internal override IMonoSslStream CreateSslStreamInternal(SslStream sslStream, Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings)
		{
			return new AppleTlsStream(innerStream, leaveInnerStreamOpen, sslStream, settings, this);
		}

		public override bool SupportsSslStream
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsMonoExtensions
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsConnectionInfo
		{
			get
			{
				return true;
			}
		}

		internal override bool SupportsCleanShutdown
		{
			get
			{
				return false;
			}
		}

		public override SslProtocols SupportedProtocols
		{
			get
			{
				return SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
			}
		}

		internal override bool ValidateCertificate(ICertificateValidator2 validator, string targetHost, bool serverMode, X509CertificateCollection certificates, bool wantsChain, ref X509Chain chain, ref MonoSslPolicyErrors errors, ref int status11)
		{
			if (wantsChain)
			{
				chain = SystemCertificateValidator.CreateX509Chain(certificates);
			}
			return AppleCertificateHelper.InvokeSystemCertificateValidator(validator, targetHost, serverMode, certificates, ref errors, ref status11);
		}
	}
}
