using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal abstract class MobileTlsContext : IDisposable
	{
		public MobileTlsContext(MobileAuthenticatedStream parent, bool serverMode, string targetHost, SslProtocols enabledProtocols, X509Certificate serverCertificate, X509CertificateCollection clientCertificates, bool askForClientCert)
		{
			this.parent = parent;
			this.serverMode = serverMode;
			this.targetHost = targetHost;
			this.enabledProtocols = enabledProtocols;
			this.serverCertificate = serverCertificate;
			this.clientCertificates = clientCertificates;
			this.askForClientCert = askForClientCert;
			this.serverName = targetHost;
			if (!string.IsNullOrEmpty(this.serverName))
			{
				int num = this.serverName.IndexOf(':');
				if (num > 0)
				{
					this.serverName = this.serverName.Substring(0, num);
				}
			}
			this.certificateValidator = CertificateValidationHelper.GetInternalValidator(parent.Settings, parent.Provider);
		}

		internal MobileAuthenticatedStream Parent
		{
			get
			{
				return this.parent;
			}
		}

		public MonoTlsSettings Settings
		{
			get
			{
				return this.parent.Settings;
			}
		}

		public MonoTlsProvider Provider
		{
			get
			{
				return this.parent.Provider;
			}
		}

		[Conditional("MONO_TLS_DEBUG")]
		protected void Debug(string message, params object[] args)
		{
		}

		public abstract bool HasContext { get; }

		public abstract bool IsAuthenticated { get; }

		public bool IsServer
		{
			get
			{
				return this.serverMode;
			}
		}

		protected string TargetHost
		{
			get
			{
				return this.targetHost;
			}
		}

		protected string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		protected bool AskForClientCertificate
		{
			get
			{
				return this.askForClientCert;
			}
		}

		protected SslProtocols EnabledProtocols
		{
			get
			{
				return this.enabledProtocols;
			}
		}

		protected X509CertificateCollection ClientCertificates
		{
			get
			{
				return this.clientCertificates;
			}
		}

		protected void GetProtocolVersions(out TlsProtocolCode min, out TlsProtocolCode max)
		{
			if ((this.enabledProtocols & SslProtocols.Tls) != SslProtocols.None)
			{
				min = TlsProtocolCode.Tls10;
			}
			else if ((this.enabledProtocols & SslProtocols.Tls11) != SslProtocols.None)
			{
				min = TlsProtocolCode.Tls11;
			}
			else
			{
				min = TlsProtocolCode.Tls12;
			}
			if ((this.enabledProtocols & SslProtocols.Tls12) != SslProtocols.None)
			{
				max = TlsProtocolCode.Tls12;
				return;
			}
			if ((this.enabledProtocols & SslProtocols.Tls11) != SslProtocols.None)
			{
				max = TlsProtocolCode.Tls11;
				return;
			}
			max = TlsProtocolCode.Tls10;
		}

		public abstract void StartHandshake();

		public abstract bool ProcessHandshake();

		public abstract void FinishHandshake();

		public abstract MonoTlsConnectionInfo ConnectionInfo { get; }

		internal X509Certificate LocalServerCertificate
		{
			get
			{
				return this.serverCertificate;
			}
		}

		internal abstract bool IsRemoteCertificateAvailable { get; }

		internal abstract X509Certificate LocalClientCertificate { get; }

		public abstract X509Certificate RemoteCertificate { get; }

		public abstract TlsProtocols NegotiatedProtocol { get; }

		public abstract void Flush();

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public abstract ValueTuple<int, bool> Read(byte[] buffer, int offset, int count);

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public abstract ValueTuple<int, bool> Write(byte[] buffer, int offset, int count);

		public abstract void Shutdown();

		protected bool ValidateCertificate(X509Certificate leaf, X509Chain chain)
		{
			ValidationResult validationResult = this.certificateValidator.ValidateCertificate(this.TargetHost, this.IsServer, leaf, chain);
			return validationResult != null && validationResult.Trusted && !validationResult.UserDenied;
		}

		protected bool ValidateCertificate(X509CertificateCollection certificates)
		{
			ValidationResult validationResult = this.certificateValidator.ValidateCertificate(this.TargetHost, this.IsServer, certificates);
			return validationResult != null && validationResult.Trusted && !validationResult.UserDenied;
		}

		protected X509Certificate SelectClientCertificate(X509Certificate serverCertificate, string[] acceptableIssuers)
		{
			X509Certificate x509Certificate;
			if (this.certificateValidator.SelectClientCertificate(this.TargetHost, this.ClientCertificates, serverCertificate, acceptableIssuers, out x509Certificate))
			{
				return x509Certificate;
			}
			if (this.clientCertificates == null || this.clientCertificates.Count == 0)
			{
				return null;
			}
			if (this.clientCertificates.Count == 1)
			{
				return this.clientCertificates[0];
			}
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		~MobileTlsContext()
		{
			this.Dispose(false);
		}

		private MobileAuthenticatedStream parent;

		private bool serverMode;

		private string targetHost;

		private string serverName;

		private SslProtocols enabledProtocols;

		private X509Certificate serverCertificate;

		private X509CertificateCollection clientCertificates;

		private bool askForClientCert;

		private ICertificateValidator2 certificateValidator;
	}
}
