using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security
{
	internal class SslAuthenticationOptions
	{
		internal SslAuthenticationOptions(SslClientAuthenticationOptions sslClientAuthenticationOptions, RemoteCertValidationCallback remoteCallback, LocalCertSelectionCallback localCallback)
		{
			this.AllowRenegotiation = sslClientAuthenticationOptions.AllowRenegotiation;
			this.ApplicationProtocols = sslClientAuthenticationOptions.ApplicationProtocols;
			this.CertValidationDelegate = remoteCallback;
			this.CheckCertName = true;
			this.EnabledSslProtocols = sslClientAuthenticationOptions.EnabledSslProtocols;
			this.EncryptionPolicy = sslClientAuthenticationOptions.EncryptionPolicy;
			this.IsServer = false;
			this.RemoteCertRequired = true;
			this.RemoteCertificateValidationCallback = sslClientAuthenticationOptions.RemoteCertificateValidationCallback;
			this.TargetHost = sslClientAuthenticationOptions.TargetHost;
			this.CertSelectionDelegate = localCallback;
			this.CertificateRevocationCheckMode = sslClientAuthenticationOptions.CertificateRevocationCheckMode;
			this.ClientCertificates = sslClientAuthenticationOptions.ClientCertificates;
			this.LocalCertificateSelectionCallback = sslClientAuthenticationOptions.LocalCertificateSelectionCallback;
		}

		internal SslAuthenticationOptions(SslServerAuthenticationOptions sslServerAuthenticationOptions)
		{
			this.AllowRenegotiation = sslServerAuthenticationOptions.AllowRenegotiation;
			this.ApplicationProtocols = sslServerAuthenticationOptions.ApplicationProtocols;
			this.CheckCertName = false;
			this.EnabledSslProtocols = sslServerAuthenticationOptions.EnabledSslProtocols;
			this.EncryptionPolicy = sslServerAuthenticationOptions.EncryptionPolicy;
			this.IsServer = true;
			this.RemoteCertRequired = sslServerAuthenticationOptions.ClientCertificateRequired;
			this.RemoteCertificateValidationCallback = sslServerAuthenticationOptions.RemoteCertificateValidationCallback;
			this.TargetHost = string.Empty;
			this.CertificateRevocationCheckMode = sslServerAuthenticationOptions.CertificateRevocationCheckMode;
			this.ServerCertificate = sslServerAuthenticationOptions.ServerCertificate;
		}

		internal bool AllowRenegotiation { get; set; }

		internal string TargetHost { get; set; }

		internal X509CertificateCollection ClientCertificates { get; set; }

		internal List<SslApplicationProtocol> ApplicationProtocols { get; }

		internal bool IsServer { get; set; }

		internal RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }

		internal LocalCertificateSelectionCallback LocalCertificateSelectionCallback { get; set; }

		internal X509Certificate ServerCertificate { get; set; }

		internal SslProtocols EnabledSslProtocols { get; set; }

		internal X509RevocationMode CertificateRevocationCheckMode { get; set; }

		internal EncryptionPolicy EncryptionPolicy { get; set; }

		internal bool RemoteCertRequired { get; set; }

		internal bool CheckCertName { get; set; }

		internal RemoteCertValidationCallback CertValidationDelegate { get; set; }

		internal LocalCertSelectionCallback CertSelectionDelegate { get; set; }

		internal ServerCertSelectionCallback ServerCertSelectionDelegate { get; set; }
	}
}
