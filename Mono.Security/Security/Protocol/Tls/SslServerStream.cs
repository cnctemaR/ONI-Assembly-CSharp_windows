using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	public class SslServerStream : SslStreamBase
	{
		internal event CertificateValidationCallback ClientCertValidation;

		internal event PrivateKeySelectionCallback PrivateKeySelection;

		public global::System.Security.Cryptography.X509Certificates.X509Certificate ClientCertificate
		{
			get
			{
				if (this.context.HandshakeState == HandshakeState.Finished)
				{
					return this.context.ClientSettings.ClientCertificate;
				}
				return null;
			}
		}

		public CertificateValidationCallback ClientCertValidationDelegate
		{
			get
			{
				return this.ClientCertValidation;
			}
			set
			{
				this.ClientCertValidation = value;
			}
		}

		public PrivateKeySelectionCallback PrivateKeyCertSelectionDelegate
		{
			get
			{
				return this.PrivateKeySelection;
			}
			set
			{
				this.PrivateKeySelection = value;
			}
		}

		public event CertificateValidationCallback2 ClientCertValidation2;

		public SslServerStream(Stream stream, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate)
			: this(stream, serverCertificate, false, false, SecurityProtocolType.Default)
		{
		}

		public SslServerStream(Stream stream, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool ownsStream)
			: this(stream, serverCertificate, clientCertificateRequired, ownsStream, SecurityProtocolType.Default)
		{
		}

		public SslServerStream(Stream stream, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool requestClientCertificate, bool ownsStream)
			: this(stream, serverCertificate, clientCertificateRequired, requestClientCertificate, ownsStream, SecurityProtocolType.Default)
		{
		}

		public SslServerStream(Stream stream, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool ownsStream, SecurityProtocolType securityProtocolType)
			: this(stream, serverCertificate, clientCertificateRequired, false, ownsStream, securityProtocolType)
		{
		}

		public SslServerStream(Stream stream, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool requestClientCertificate, bool ownsStream, SecurityProtocolType securityProtocolType)
			: base(stream, ownsStream)
		{
			this.context = new ServerContext(this, securityProtocolType, serverCertificate, clientCertificateRequired, requestClientCertificate);
			this.protocol = new ServerRecordProtocol(this.innerStream, (ServerContext)this.context);
		}

		~SslServerStream()
		{
			this.Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.ClientCertValidation = null;
				this.PrivateKeySelection = null;
			}
		}

		internal override IAsyncResult BeginNegotiateHandshake(AsyncCallback callback, object state)
		{
			if (this.context.HandshakeState != HandshakeState.None)
			{
				this.context.Clear();
			}
			this.context.SupportedCiphers = CipherSuiteFactory.GetSupportedCiphers(true, this.context.SecurityProtocol);
			this.context.HandshakeState = HandshakeState.Started;
			return this.protocol.BeginReceiveRecord(this.innerStream, callback, state);
		}

		internal override void EndNegotiateHandshake(IAsyncResult asyncResult)
		{
			this.protocol.EndReceiveRecord(asyncResult);
			if (this.context.LastHandshakeMsg != HandshakeType.ClientHello)
			{
				this.protocol.SendAlert(AlertDescription.UnexpectedMessage);
			}
			this.protocol.SendRecord(HandshakeType.ServerHello);
			this.protocol.SendRecord(HandshakeType.Certificate);
			if (((ServerContext)this.context).ClientCertificateRequired || ((ServerContext)this.context).RequestClientCertificate)
			{
				this.protocol.SendRecord(HandshakeType.CertificateRequest);
			}
			this.protocol.SendRecord(HandshakeType.ServerHelloDone);
			while (this.context.LastHandshakeMsg != HandshakeType.Finished)
			{
				byte[] array = this.protocol.ReceiveRecord(this.innerStream);
				if (array == null || array.Length == 0)
				{
					throw new TlsException(AlertDescription.HandshakeFailiure, "The client stopped the handshake.");
				}
			}
			this.protocol.SendChangeCipherSpec();
			this.protocol.SendRecord(HandshakeType.Finished);
			this.context.HandshakeState = HandshakeState.Finished;
			this.context.HandshakeMessages.Reset();
			this.context.ClearKeyInfo();
		}

		internal override global::System.Security.Cryptography.X509Certificates.X509Certificate OnLocalCertificateSelection(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCertificates)
		{
			throw new NotSupportedException();
		}

		internal override bool OnRemoteCertificateValidation(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] errors)
		{
			if (this.ClientCertValidation != null)
			{
				return this.ClientCertValidation(certificate, errors);
			}
			return errors != null && errors.Length == 0;
		}

		internal override bool HaveRemoteValidation2Callback
		{
			get
			{
				return this.ClientCertValidation2 != null;
			}
		}

		internal override ValidationResult OnRemoteCertificateValidation2(Mono.Security.X509.X509CertificateCollection collection)
		{
			CertificateValidationCallback2 clientCertValidation = this.ClientCertValidation2;
			if (clientCertValidation != null)
			{
				return clientCertValidation(collection);
			}
			return null;
		}

		internal bool RaiseClientCertificateValidation(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] certificateErrors)
		{
			return base.RaiseRemoteCertificateValidation(certificate, certificateErrors);
		}

		internal override AsymmetricAlgorithm OnLocalPrivateKeySelection(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate, string targetHost)
		{
			if (this.PrivateKeySelection != null)
			{
				return this.PrivateKeySelection(certificate, targetHost);
			}
			return null;
		}

		internal AsymmetricAlgorithm RaisePrivateKeySelection(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate, string targetHost)
		{
			return base.RaiseLocalPrivateKeySelection(certificate, targetHost);
		}
	}
}
