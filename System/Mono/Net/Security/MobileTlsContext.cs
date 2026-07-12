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
		protected MobileTlsContext(MobileAuthenticatedStream parent, MonoSslAuthenticationOptions options)
		{
			this.Parent = parent;
			this.Options = options;
			this.IsServer = options.ServerMode;
			this.EnabledProtocols = options.EnabledSslProtocols;
			if (options.ServerMode)
			{
				this.LocalServerCertificate = options.ServerCertificate;
				this.AskForClientCertificate = options.ClientCertificateRequired;
			}
			else
			{
				this.ClientCertificates = options.ClientCertificates;
				this.TargetHost = options.TargetHost;
				this.ServerName = options.TargetHost;
				if (!string.IsNullOrEmpty(this.ServerName))
				{
					int num = this.ServerName.IndexOf(':');
					if (num > 0)
					{
						this.ServerName = this.ServerName.Substring(0, num);
					}
				}
			}
			this.certificateValidator = ChainValidationHelper.GetInternalValidator(parent.SslStream, parent.Provider, parent.Settings);
		}

		internal MonoSslAuthenticationOptions Options { get; }

		internal MobileAuthenticatedStream Parent { get; }

		public MonoTlsSettings Settings
		{
			get
			{
				return this.Parent.Settings;
			}
		}

		public MonoTlsProvider Provider
		{
			get
			{
				return this.Parent.Provider;
			}
		}

		[Conditional("MONO_TLS_DEBUG")]
		protected void Debug(string message, params object[] args)
		{
		}

		public abstract bool HasContext { get; }

		public abstract bool IsAuthenticated { get; }

		public bool IsServer { get; }

		internal string TargetHost { get; }

		protected string ServerName { get; }

		protected bool AskForClientCertificate { get; }

		protected SslProtocols EnabledProtocols { get; }

		protected X509CertificateCollection ClientCertificates { get; }

		internal bool AllowRenegotiation
		{
			get
			{
				return false;
			}
		}

		protected void GetProtocolVersions(out TlsProtocolCode? min, out TlsProtocolCode? max)
		{
			if ((this.EnabledProtocols & SslProtocols.Tls) != SslProtocols.None)
			{
				min = new TlsProtocolCode?(TlsProtocolCode.Tls10);
			}
			else if ((this.EnabledProtocols & SslProtocols.Tls11) != SslProtocols.None)
			{
				min = new TlsProtocolCode?(TlsProtocolCode.Tls11);
			}
			else if ((this.EnabledProtocols & SslProtocols.Tls12) != SslProtocols.None)
			{
				min = new TlsProtocolCode?(TlsProtocolCode.Tls12);
			}
			else
			{
				min = null;
			}
			if ((this.EnabledProtocols & SslProtocols.Tls12) != SslProtocols.None)
			{
				max = new TlsProtocolCode?(TlsProtocolCode.Tls12);
				return;
			}
			if ((this.EnabledProtocols & SslProtocols.Tls11) != SslProtocols.None)
			{
				max = new TlsProtocolCode?(TlsProtocolCode.Tls11);
				return;
			}
			if ((this.EnabledProtocols & SslProtocols.Tls) != SslProtocols.None)
			{
				max = new TlsProtocolCode?(TlsProtocolCode.Tls10);
				return;
			}
			max = null;
		}

		public abstract void StartHandshake();

		public abstract bool ProcessHandshake();

		public abstract void FinishHandshake();

		public abstract MonoTlsConnectionInfo ConnectionInfo { get; }

		internal X509Certificate LocalServerCertificate { get; private set; }

		internal abstract bool IsRemoteCertificateAvailable { get; }

		internal abstract X509Certificate LocalClientCertificate { get; }

		public abstract X509Certificate2 RemoteCertificate { get; }

		public abstract TlsProtocols NegotiatedProtocol { get; }

		public abstract void Flush();

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public abstract ValueTuple<int, bool> Read(byte[] buffer, int offset, int count);

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		public abstract ValueTuple<int, bool> Write(byte[] buffer, int offset, int count);

		public abstract void Shutdown();

		public abstract bool PendingRenegotiation();

		protected bool ValidateCertificate(X509Certificate2 leaf, X509Chain chain)
		{
			ValidationResult validationResult = this.certificateValidator.ValidateCertificate(this.TargetHost, this.IsServer, leaf, chain);
			return validationResult != null && validationResult.Trusted && !validationResult.UserDenied;
		}

		protected bool ValidateCertificate(X509Certificate2Collection certificates)
		{
			ValidationResult validationResult = this.certificateValidator.ValidateCertificate(this.TargetHost, this.IsServer, certificates);
			return validationResult != null && validationResult.Trusted && !validationResult.UserDenied;
		}

		protected X509Certificate SelectServerCertificate(string serverIdentity)
		{
			if (this.Options.ServerCertSelectionDelegate != null)
			{
				this.LocalServerCertificate = this.Options.ServerCertSelectionDelegate(serverIdentity);
				if (this.LocalServerCertificate == null)
				{
					throw new AuthenticationException("The server mode SSL must use a certificate with the associated private key.");
				}
			}
			else if (this.Settings.ClientCertificateSelectionCallback != null)
			{
				X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
				x509CertificateCollection.Add(this.Options.ServerCertificate);
				this.LocalServerCertificate = this.Settings.ClientCertificateSelectionCallback(string.Empty, x509CertificateCollection, null, Array.Empty<string>());
			}
			else
			{
				this.LocalServerCertificate = this.Options.ServerCertificate;
			}
			if (this.LocalServerCertificate == null)
			{
				throw new NotSupportedException("The server mode SSL must use a certificate with the associated private key.");
			}
			return this.LocalServerCertificate;
		}

		protected X509Certificate SelectClientCertificate(string[] acceptableIssuers)
		{
			if (this.Settings.DisallowUnauthenticatedCertificateRequest && !this.IsAuthenticated)
			{
				return null;
			}
			if (this.RemoteCertificate == null)
			{
				throw new TlsException(AlertDescription.InternalError, "Cannot request client certificate before receiving one from the server.");
			}
			X509Certificate x509Certificate;
			if (this.certificateValidator.SelectClientCertificate(this.TargetHost, this.ClientCertificates, this.IsAuthenticated ? this.RemoteCertificate : null, acceptableIssuers, out x509Certificate))
			{
				return x509Certificate;
			}
			if (this.ClientCertificates == null || this.ClientCertificates.Count == 0)
			{
				return null;
			}
			if (acceptableIssuers == null || acceptableIssuers.Length == 0)
			{
				return this.ClientCertificates[0];
			}
			for (int i = 0; i < this.ClientCertificates.Count; i++)
			{
				X509Certificate2 x509Certificate2 = this.ClientCertificates[i] as X509Certificate2;
				if (x509Certificate2 != null)
				{
					X509Chain x509Chain = null;
					try
					{
						x509Chain = new X509Chain();
						x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
						x509Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreInvalidName;
						x509Chain.Build(x509Certificate2);
						if (x509Chain.ChainElements.Count != 0)
						{
							for (int j = 0; j < x509Chain.ChainElements.Count; j++)
							{
								string issuer = x509Chain.ChainElements[j].Certificate.Issuer;
								if (Array.IndexOf<string>(acceptableIssuers, issuer) != -1)
								{
									return x509Certificate2;
								}
							}
						}
					}
					catch
					{
					}
					finally
					{
						if (x509Chain != null)
						{
							x509Chain.Reset();
						}
					}
				}
			}
			return null;
		}

		public abstract bool CanRenegotiate { get; }

		public abstract void Renegotiate();

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

		private ChainValidationHelper certificateValidator;
	}
}
