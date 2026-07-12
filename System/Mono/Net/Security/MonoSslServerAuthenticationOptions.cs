using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Net.Security.Private;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal sealed class MonoSslServerAuthenticationOptions : MonoSslAuthenticationOptions, IMonoSslServerAuthenticationOptions, IMonoAuthenticationOptions
	{
		public SslServerAuthenticationOptions Options { get; }

		public override bool ServerMode
		{
			get
			{
				return true;
			}
		}

		public MonoSslServerAuthenticationOptions(SslServerAuthenticationOptions options)
		{
			this.Options = options;
		}

		public MonoSslServerAuthenticationOptions()
		{
			this.Options = new SslServerAuthenticationOptions();
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

		public override bool ClientCertificateRequired
		{
			get
			{
				return this.Options.ClientCertificateRequired;
			}
			set
			{
				this.Options.ClientCertificateRequired = value;
			}
		}

		public ServerCertificateSelectionCallback ServerCertificateSelectionCallback
		{
			get
			{
				return this.Options.ServerCertificateSelectionCallback;
			}
			set
			{
				this.Options.ServerCertificateSelectionCallback = value;
			}
		}

		MonoServerCertificateSelectionCallback IMonoSslServerAuthenticationOptions.ServerCertificateSelectionCallback
		{
			get
			{
				return CallbackHelpers.PublicToMono(this.ServerCertificateSelectionCallback);
			}
			set
			{
				this.ServerCertificateSelectionCallback = CallbackHelpers.MonoToPublic(value);
			}
		}

		public override string TargetHost
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

		public override X509Certificate ServerCertificate
		{
			get
			{
				return this.Options.ServerCertificate;
			}
			set
			{
				this.Options.ServerCertificate = value;
			}
		}

		public override X509CertificateCollection ClientCertificates
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
