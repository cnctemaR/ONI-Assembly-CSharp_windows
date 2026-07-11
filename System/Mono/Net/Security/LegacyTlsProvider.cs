using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Net.Security.Private;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal class LegacyTlsProvider : MonoTlsProvider
	{
		public override Guid ID
		{
			get
			{
				return MonoTlsProviderFactory.LegacyId;
			}
		}

		public override string Name
		{
			get
			{
				return "legacy";
			}
		}

		public override bool SupportsSslStream
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
				return false;
			}
		}

		public override bool SupportsMonoExtensions
		{
			get
			{
				return false;
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
				return SslProtocols.Tls;
			}
		}

		public override IMonoSslStream CreateSslStream(Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings = null)
		{
			return SslStream.CreateMonoSslStream(innerStream, leaveInnerStreamOpen, this, settings);
		}

		internal override IMonoSslStream CreateSslStreamInternal(SslStream sslStream, Stream innerStream, bool leaveInnerStreamOpen, MonoTlsSettings settings)
		{
			return new LegacySslStream(innerStream, leaveInnerStreamOpen, sslStream, this, settings);
		}

		internal override bool ValidateCertificate(ICertificateValidator2 validator, string targetHost, bool serverMode, X509CertificateCollection certificates, bool wantsChain, ref X509Chain chain, ref MonoSslPolicyErrors errors, ref int status11)
		{
			if (wantsChain)
			{
				chain = SystemCertificateValidator.CreateX509Chain(certificates);
			}
			SslPolicyErrors sslPolicyErrors = (SslPolicyErrors)errors;
			bool flag = SystemCertificateValidator.Evaluate(validator.Settings, targetHost, certificates, chain, ref sslPolicyErrors, ref status11);
			errors = (MonoSslPolicyErrors)sslPolicyErrors;
			return flag;
		}
	}
}
