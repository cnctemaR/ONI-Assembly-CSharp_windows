using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal sealed class MonoSslClientAuthenticationOptions : MonoSslAuthenticationOptions, IMonoSslClientAuthenticationOptions, IMonoAuthenticationOptions
	{
		public SslClientAuthenticationOptions Options { get; }

		public override bool ServerMode
		{
			get
			{
				return false;
			}
		}

		public MonoSslClientAuthenticationOptions(SslClientAuthenticationOptions options)
		{
			this.Options = options;
		}

		public MonoSslClientAuthenticationOptions()
		{
			this.Options = new SslClientAuthenticationOptions();
		}

		public override bool AllowRenegotiation
		{
			get
			{
				return this.Options.AllowRenegotiation;
			}
			set
			{
				this.Options.AllowRenegotiation = value;
			}
		}

		public override RemoteCertificateValidationCallback RemoteCertificateValidationCallback
		{
			get
			{
				return this.Options.RemoteCertificateValidationCallback;
			}
			set
			{
				this.Options.RemoteCertificateValidationCallback = value;
			}
		}

		public override X509RevocationMode CertificateRevocationCheckMode
		{
			get
			{
				return this.Options.CertificateRevocationCheckMode;
			}
			set
			{
				this.Options.CertificateRevocationCheckMode = value;
			}
		}

		public override EncryptionPolicy EncryptionPolicy
		{
			get
			{
				return this.Options.EncryptionPolicy;
			}
			set
			{
				this.Options.EncryptionPolicy = value;
			}
		}

		public override SslProtocols EnabledSslProtocols
		{
			get
			{
				return this.Options.EnabledSslProtocols;
			}
			set
			{
				this.Options.EnabledSslProtocols = value;
			}
		}

		public LocalCertificateSelectionCallback LocalCertificateSelectionCallback
		{
			get
			{
				return this.Options.LocalCertificateSelectionCallback;
			}
			set
			{
				this.Options.LocalCertificateSelectionCallback = value;
			}
		}

		public override string TargetHost
		{
			get
			{
				return this.Options.TargetHost;
			}
			set
			{
				this.Options.TargetHost = value;
			}
		}

		public override bool ClientCertificateRequired
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override X509CertificateCollection ClientCertificates
		{
			get
			{
				return this.Options.ClientCertificates;
			}
			set
			{
				this.Options.ClientCertificates = value;
			}
		}

		public override X509Certificate ServerCertificate
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
