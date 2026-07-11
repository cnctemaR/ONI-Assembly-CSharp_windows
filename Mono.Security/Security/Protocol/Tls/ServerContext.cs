using System;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	internal class ServerContext : Context
	{
		public SslServerStream SslStream
		{
			get
			{
				return this.sslStream;
			}
		}

		public bool ClientCertificateRequired
		{
			get
			{
				return this.clientCertificateRequired;
			}
		}

		public bool RequestClientCertificate
		{
			get
			{
				return this.request_client_certificate;
			}
		}

		public ServerContext(SslServerStream stream, SecurityProtocolType securityProtocolType, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool requestClientCertificate)
			: base(securityProtocolType)
		{
			this.sslStream = stream;
			this.clientCertificateRequired = clientCertificateRequired;
			this.request_client_certificate = requestClientCertificate;
			Mono.Security.X509.X509Certificate x509Certificate = new Mono.Security.X509.X509Certificate(serverCertificate.GetRawCertData());
			base.ServerSettings.Certificates = new Mono.Security.X509.X509CertificateCollection();
			base.ServerSettings.Certificates.Add(x509Certificate);
			base.ServerSettings.UpdateCertificateRSA();
			if (CertificateValidationHelper.SupportsX509Chain)
			{
				Mono.Security.X509.X509Chain x509Chain = new Mono.Security.X509.X509Chain(X509StoreManager.IntermediateCACertificates);
				if (x509Chain.Build(x509Certificate))
				{
					for (int i = x509Chain.Chain.Count - 1; i > 0; i--)
					{
						base.ServerSettings.Certificates.Add(x509Chain.Chain[i]);
					}
				}
			}
			base.ServerSettings.CertificateTypes = new ClientCertificateType[base.ServerSettings.Certificates.Count];
			for (int j = 0; j < base.ServerSettings.CertificateTypes.Length; j++)
			{
				base.ServerSettings.CertificateTypes[j] = ClientCertificateType.RSA;
			}
			if (CertificateValidationHelper.SupportsX509Chain)
			{
				Mono.Security.X509.X509CertificateCollection trustedRootCertificates = X509StoreManager.TrustedRootCertificates;
				string[] array = new string[trustedRootCertificates.Count];
				int num = 0;
				foreach (Mono.Security.X509.X509Certificate x509Certificate2 in trustedRootCertificates)
				{
					array[num++] = x509Certificate2.IssuerName;
				}
				base.ServerSettings.DistinguisedNames = array;
			}
		}

		private SslServerStream sslStream;

		private bool request_client_certificate;

		private bool clientCertificateRequired;
	}
}
