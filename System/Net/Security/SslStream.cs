using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using Mono.Net.Security.Private;
using Mono.Security.Interface;
using Unity;

namespace System.Net.Security
{
	public class SslStream : AuthenticatedStream
	{
		internal IMonoSslStream Impl
		{
			get
			{
				this.CheckDisposed();
				return this.impl;
			}
		}

		internal MonoTlsProvider Provider
		{
			get
			{
				this.CheckDisposed();
				return this.provider;
			}
		}

		private static MonoTlsProvider GetProvider()
		{
			return MonoTlsProviderFactory.GetProvider();
		}

		public SslStream(Stream innerStream)
			: this(innerStream, false)
		{
		}

		public SslStream(Stream innerStream, bool leaveInnerStreamOpen)
			: base(innerStream, leaveInnerStreamOpen)
		{
			this.provider = SslStream.GetProvider();
			this.impl = this.provider.CreateSslStreamInternal(this, innerStream, leaveInnerStreamOpen, null);
		}

		public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback)
			: this(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, null)
		{
		}

		public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback)
			: base(innerStream, leaveInnerStreamOpen)
		{
			this.provider = SslStream.GetProvider();
			MonoTlsSettings monoTlsSettings = MonoTlsSettings.CopyDefaultSettings();
			monoTlsSettings.RemoteCertificateValidationCallback = CallbackHelpers.PublicToMono(userCertificateValidationCallback);
			monoTlsSettings.ClientCertificateSelectionCallback = CallbackHelpers.PublicToMono(userCertificateSelectionCallback);
			this.impl = this.provider.CreateSslStream(innerStream, leaveInnerStreamOpen, monoTlsSettings);
		}

		[MonoLimitation("encryptionPolicy is ignored")]
		public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy)
			: this(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback)
		{
		}

		internal SslStream(Stream innerStream, bool leaveInnerStreamOpen, MonoTlsProvider provider, MonoTlsSettings settings)
			: base(innerStream, leaveInnerStreamOpen)
		{
			this.provider = provider;
			this.impl = provider.CreateSslStreamInternal(this, innerStream, leaveInnerStreamOpen, settings);
		}

		internal static IMonoSslStream CreateMonoSslStream(Stream innerStream, bool leaveInnerStreamOpen, MonoTlsProvider provider, MonoTlsSettings settings)
		{
			return new SslStream(innerStream, leaveInnerStreamOpen, provider, settings).Impl;
		}

		public virtual void AuthenticateAsClient(string targetHost)
		{
			this.Impl.AuthenticateAsClient(targetHost);
		}

		public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			this.Impl.AuthenticateAsClient(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation);
		}

		public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback asyncCallback, object asyncState)
		{
			return this.Impl.BeginAuthenticateAsClient(targetHost, asyncCallback, asyncState);
		}

		public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			return this.Impl.BeginAuthenticateAsClient(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation, asyncCallback, asyncState);
		}

		public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
		{
			this.Impl.EndAuthenticateAsClient(asyncResult);
		}

		public virtual void AuthenticateAsServer(X509Certificate serverCertificate)
		{
			this.Impl.AuthenticateAsServer(serverCertificate);
		}

		public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			this.Impl.AuthenticateAsServer(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation);
		}

		public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback asyncCallback, object asyncState)
		{
			return this.Impl.BeginAuthenticateAsServer(serverCertificate, asyncCallback, asyncState);
		}

		public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			return this.Impl.BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation, asyncCallback, asyncState);
		}

		public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
		{
			this.Impl.EndAuthenticateAsServer(asyncResult);
		}

		public TransportContext TransportContext
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public virtual Task AuthenticateAsClientAsync(string targetHost)
		{
			return this.Impl.AuthenticateAsClientAsync(targetHost);
		}

		public virtual Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			return this.Impl.AuthenticateAsClientAsync(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation);
		}

		public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate)
		{
			return this.Impl.AuthenticateAsServerAsync(serverCertificate);
		}

		public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			return this.Impl.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation);
		}

		public virtual Task ShutdownAsync()
		{
			return this.Impl.ShutdownAsync();
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this.Impl.IsAuthenticated;
			}
		}

		public override bool IsMutuallyAuthenticated
		{
			get
			{
				return this.Impl.IsMutuallyAuthenticated;
			}
		}

		public override bool IsEncrypted
		{
			get
			{
				return this.Impl.IsEncrypted;
			}
		}

		public override bool IsSigned
		{
			get
			{
				return this.Impl.IsSigned;
			}
		}

		public override bool IsServer
		{
			get
			{
				return this.Impl.IsServer;
			}
		}

		public virtual SslProtocols SslProtocol
		{
			get
			{
				return this.Impl.SslProtocol;
			}
		}

		public virtual bool CheckCertRevocationStatus
		{
			get
			{
				return this.Impl.CheckCertRevocationStatus;
			}
		}

		public virtual X509Certificate LocalCertificate
		{
			get
			{
				return this.Impl.LocalCertificate;
			}
		}

		public virtual X509Certificate RemoteCertificate
		{
			get
			{
				return this.Impl.RemoteCertificate;
			}
		}

		public virtual global::System.Security.Authentication.CipherAlgorithmType CipherAlgorithm
		{
			get
			{
				return this.Impl.CipherAlgorithm;
			}
		}

		public virtual int CipherStrength
		{
			get
			{
				return this.Impl.CipherStrength;
			}
		}

		public virtual global::System.Security.Authentication.HashAlgorithmType HashAlgorithm
		{
			get
			{
				return this.Impl.HashAlgorithm;
			}
		}

		public virtual int HashStrength
		{
			get
			{
				return this.Impl.HashStrength;
			}
		}

		public virtual global::System.Security.Authentication.ExchangeAlgorithmType KeyExchangeAlgorithm
		{
			get
			{
				return this.Impl.KeyExchangeAlgorithm;
			}
		}

		public virtual int KeyExchangeStrength
		{
			get
			{
				return this.Impl.KeyExchangeStrength;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.impl != null && this.impl.CanRead;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return base.InnerStream.CanTimeout;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.impl != null && this.impl.CanWrite;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.Impl.ReadTimeout;
			}
			set
			{
				this.Impl.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.Impl.WriteTimeout;
			}
			set
			{
				this.Impl.WriteTimeout = value;
			}
		}

		public override long Length
		{
			get
			{
				return this.Impl.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.Impl.Position;
			}
			set
			{
				throw new NotSupportedException(global::SR.GetString("This stream does not support seek operations."));
			}
		}

		public override void SetLength(long value)
		{
			this.Impl.SetLength(value);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(global::SR.GetString("This stream does not support seek operations."));
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return base.InnerStream.FlushAsync(cancellationToken);
		}

		public override void Flush()
		{
			base.InnerStream.Flush();
		}

		private void CheckDisposed()
		{
			if (this.impl == null)
			{
				throw new ObjectDisposedException("SslStream");
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this.impl != null && disposing)
				{
					this.impl.Dispose();
					this.impl = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.Impl.Read(buffer, offset, count);
		}

		public void Write(byte[] buffer)
		{
			this.Impl.Write(buffer);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.Impl.Write(buffer, offset, count);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return this.Impl.BeginRead(buffer, offset, count, asyncCallback, asyncState);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return this.Impl.EndRead(asyncResult);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return this.Impl.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			this.Impl.EndWrite(asyncResult);
		}

		public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, bool checkCertificateRevocation)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, bool checkCertificateRevocation)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		private MonoTlsProvider provider;

		private IMonoSslStream impl;
	}
}
