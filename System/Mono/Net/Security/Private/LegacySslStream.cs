using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Mono.Security.Interface;
using Mono.Security.Protocol.Tls;
using Mono.Security.X509;

namespace Mono.Net.Security.Private
{
	[MonoTODO("Non-X509Certificate2 certificate is not supported")]
	internal class LegacySslStream : AuthenticatedStream, IMonoSslStream, IDisposable
	{
		public LegacySslStream(Stream innerStream, bool leaveInnerStreamOpen, SslStream owner, MonoTlsProvider provider, MonoTlsSettings settings)
			: base(innerStream, leaveInnerStreamOpen)
		{
			this.SslStream = owner;
			this.Provider = provider;
			this.certificateValidator = ChainValidationHelper.GetInternalValidator(provider, settings);
		}

		public override bool CanRead
		{
			get
			{
				return base.InnerStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return base.InnerStream.CanSeek;
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
				return base.InnerStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return base.InnerStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return base.InnerStream.Position;
			}
			set
			{
				throw new NotSupportedException("This stream does not support seek operations");
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this.ssl_stream != null;
			}
		}

		public override bool IsEncrypted
		{
			get
			{
				return this.IsAuthenticated;
			}
		}

		public override bool IsMutuallyAuthenticated
		{
			get
			{
				if (!this.IsAuthenticated)
				{
					return false;
				}
				if (!this.IsServer)
				{
					return this.LocalCertificate != null;
				}
				return this.RemoteCertificate != null;
			}
		}

		public override bool IsServer
		{
			get
			{
				return this.ssl_stream is SslServerStream;
			}
		}

		public override bool IsSigned
		{
			get
			{
				return this.IsAuthenticated;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return base.InnerStream.ReadTimeout;
			}
			set
			{
				base.InnerStream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return base.InnerStream.WriteTimeout;
			}
			set
			{
				base.InnerStream.WriteTimeout = value;
			}
		}

		public virtual bool CheckCertRevocationStatus
		{
			get
			{
				return this.IsAuthenticated && this.ssl_stream.CheckCertRevocationStatus;
			}
		}

		public virtual global::System.Security.Authentication.CipherAlgorithmType CipherAlgorithm
		{
			get
			{
				this.CheckConnectionAuthenticated();
				switch (this.ssl_stream.CipherAlgorithm)
				{
				case Mono.Security.Protocol.Tls.CipherAlgorithmType.Des:
					return global::System.Security.Authentication.CipherAlgorithmType.Des;
				case Mono.Security.Protocol.Tls.CipherAlgorithmType.None:
					return global::System.Security.Authentication.CipherAlgorithmType.None;
				case Mono.Security.Protocol.Tls.CipherAlgorithmType.Rc2:
					return global::System.Security.Authentication.CipherAlgorithmType.Rc2;
				case Mono.Security.Protocol.Tls.CipherAlgorithmType.Rc4:
					return global::System.Security.Authentication.CipherAlgorithmType.Rc4;
				case Mono.Security.Protocol.Tls.CipherAlgorithmType.Rijndael:
				{
					int cipherStrength = this.ssl_stream.CipherStrength;
					if (cipherStrength == 128)
					{
						return global::System.Security.Authentication.CipherAlgorithmType.Aes128;
					}
					if (cipherStrength == 192)
					{
						return global::System.Security.Authentication.CipherAlgorithmType.Aes192;
					}
					if (cipherStrength == 256)
					{
						return global::System.Security.Authentication.CipherAlgorithmType.Aes256;
					}
					break;
				}
				case Mono.Security.Protocol.Tls.CipherAlgorithmType.TripleDes:
					return global::System.Security.Authentication.CipherAlgorithmType.TripleDes;
				}
				throw new InvalidOperationException("Not supported cipher algorithm is in use. It is likely a bug in SslStream.");
			}
		}

		public virtual int CipherStrength
		{
			get
			{
				this.CheckConnectionAuthenticated();
				return this.ssl_stream.CipherStrength;
			}
		}

		public virtual global::System.Security.Authentication.HashAlgorithmType HashAlgorithm
		{
			get
			{
				this.CheckConnectionAuthenticated();
				switch (this.ssl_stream.HashAlgorithm)
				{
				case Mono.Security.Protocol.Tls.HashAlgorithmType.Md5:
					return global::System.Security.Authentication.HashAlgorithmType.Md5;
				case Mono.Security.Protocol.Tls.HashAlgorithmType.None:
					return global::System.Security.Authentication.HashAlgorithmType.None;
				case Mono.Security.Protocol.Tls.HashAlgorithmType.Sha1:
					return global::System.Security.Authentication.HashAlgorithmType.Sha1;
				default:
					throw new InvalidOperationException("Not supported hash algorithm is in use. It is likely a bug in SslStream.");
				}
			}
		}

		public virtual int HashStrength
		{
			get
			{
				this.CheckConnectionAuthenticated();
				return this.ssl_stream.HashStrength;
			}
		}

		public virtual global::System.Security.Authentication.ExchangeAlgorithmType KeyExchangeAlgorithm
		{
			get
			{
				this.CheckConnectionAuthenticated();
				switch (this.ssl_stream.KeyExchangeAlgorithm)
				{
				case Mono.Security.Protocol.Tls.ExchangeAlgorithmType.DiffieHellman:
					return global::System.Security.Authentication.ExchangeAlgorithmType.DiffieHellman;
				case Mono.Security.Protocol.Tls.ExchangeAlgorithmType.None:
					return global::System.Security.Authentication.ExchangeAlgorithmType.None;
				case Mono.Security.Protocol.Tls.ExchangeAlgorithmType.RsaKeyX:
					return global::System.Security.Authentication.ExchangeAlgorithmType.RsaKeyX;
				case Mono.Security.Protocol.Tls.ExchangeAlgorithmType.RsaSign:
					return global::System.Security.Authentication.ExchangeAlgorithmType.RsaSign;
				}
				throw new InvalidOperationException("Not supported exchange algorithm is in use. It is likely a bug in SslStream.");
			}
		}

		public virtual int KeyExchangeStrength
		{
			get
			{
				this.CheckConnectionAuthenticated();
				return this.ssl_stream.KeyExchangeStrength;
			}
		}

		global::System.Security.Cryptography.X509Certificates.X509Certificate IMonoSslStream.InternalLocalCertificate
		{
			get
			{
				if (!this.IsServer)
				{
					return ((SslClientStream)this.ssl_stream).SelectedClientCertificate;
				}
				return this.ssl_stream.ServerCertificate;
			}
		}

		public virtual global::System.Security.Cryptography.X509Certificates.X509Certificate LocalCertificate
		{
			get
			{
				this.CheckConnectionAuthenticated();
				if (!this.IsServer)
				{
					return ((SslClientStream)this.ssl_stream).SelectedClientCertificate;
				}
				return this.ssl_stream.ServerCertificate;
			}
		}

		public virtual global::System.Security.Cryptography.X509Certificates.X509Certificate RemoteCertificate
		{
			get
			{
				this.CheckConnectionAuthenticated();
				if (this.IsServer)
				{
					return ((SslServerStream)this.ssl_stream).ClientCertificate;
				}
				return this.ssl_stream.ServerCertificate;
			}
		}

		public virtual SslProtocols SslProtocol
		{
			get
			{
				this.CheckConnectionAuthenticated();
				Mono.Security.Protocol.Tls.SecurityProtocolType securityProtocol = this.ssl_stream.SecurityProtocol;
				if (securityProtocol <= Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl2)
				{
					if (securityProtocol == Mono.Security.Protocol.Tls.SecurityProtocolType.Default)
					{
						return SslProtocols.Default;
					}
					if (securityProtocol == Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl2)
					{
						return SslProtocols.Ssl2;
					}
				}
				else
				{
					if (securityProtocol == Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl3)
					{
						return SslProtocols.Ssl3;
					}
					if (securityProtocol == Mono.Security.Protocol.Tls.SecurityProtocolType.Tls)
					{
						return SslProtocols.Tls;
					}
				}
				throw new InvalidOperationException("Not supported SSL/TLS protocol is in use. It is likely a bug in SslStream.");
			}
		}

		private global::System.Security.Cryptography.X509Certificates.X509Certificate OnCertificateSelection(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCerts, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCert, string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCerts)
		{
			string[] array = new string[(serverRequestedCerts != null) ? serverRequestedCerts.Count : 0];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = serverRequestedCerts[i].GetIssuerName();
			}
			global::System.Security.Cryptography.X509Certificates.X509Certificate x509Certificate;
			this.certificateValidator.SelectClientCertificate(targetHost, clientCerts, serverCert, array, out x509Certificate);
			return x509Certificate;
		}

		public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginAuthenticateAsClient(targetHost, new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection(), SslProtocols.Tls, false, asyncCallback, asyncState);
		}

		public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.IsAuthenticated)
			{
				throw new InvalidOperationException("This SslStream is already authenticated");
			}
			SslClientStream sslClientStream = new SslClientStream(base.InnerStream, targetHost, !base.LeaveInnerStreamOpen, this.GetMonoSslProtocol(enabledSslProtocols), clientCertificates);
			sslClientStream.CheckCertRevocationStatus = checkCertificateRevocation;
			sslClientStream.PrivateKeyCertSelectionDelegate = delegate(global::System.Security.Cryptography.X509Certificates.X509Certificate cert, string host)
			{
				string certHashString = cert.GetCertHashString();
				foreach (global::System.Security.Cryptography.X509Certificates.X509Certificate x509Certificate in clientCertificates)
				{
					if (!(x509Certificate.GetCertHashString() != certHashString))
					{
						return ((x509Certificate as X509Certificate2) ?? new X509Certificate2(x509Certificate)).PrivateKey;
					}
				}
				return null;
			};
			sslClientStream.ServerCertValidation2 += delegate(Mono.Security.X509.X509CertificateCollection mcerts)
			{
				global::System.Security.Cryptography.X509Certificates.X509CertificateCollection x509CertificateCollection = null;
				if (mcerts != null)
				{
					x509CertificateCollection = new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection();
					for (int i = 0; i < mcerts.Count; i++)
					{
						x509CertificateCollection.Add(new X509Certificate2(mcerts[i].RawData));
					}
				}
				return ((ChainValidationHelper)this.certificateValidator).ValidateCertificate(targetHost, false, x509CertificateCollection);
			};
			sslClientStream.ClientCertSelectionDelegate = new CertificateSelectionCallback(this.OnCertificateSelection);
			this.ssl_stream = sslClientStream;
			return this.BeginWrite(new byte[0], 0, 0, asyncCallback, asyncState);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			this.CheckConnectionAuthenticated();
			return this.ssl_stream.BeginRead(buffer, offset, count, asyncCallback, asyncState);
		}

		public virtual IAsyncResult BeginAuthenticateAsServer(global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, AsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginAuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, false, asyncCallback, asyncState);
		}

		public virtual IAsyncResult BeginAuthenticateAsServer(global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.IsAuthenticated)
			{
				throw new InvalidOperationException("This SslStream is already authenticated");
			}
			this.ssl_stream = new SslServerStream(base.InnerStream, serverCertificate, false, clientCertificateRequired, !base.LeaveInnerStreamOpen, this.GetMonoSslProtocol(enabledSslProtocols))
			{
				CheckCertRevocationStatus = checkCertificateRevocation,
				PrivateKeyCertSelectionDelegate = delegate(global::System.Security.Cryptography.X509Certificates.X509Certificate cert, string targetHost)
				{
					X509Certificate2 x509Certificate = (serverCertificate as X509Certificate2) ?? new X509Certificate2(serverCertificate);
					if (x509Certificate == null)
					{
						return null;
					}
					return x509Certificate.PrivateKey;
				},
				ClientCertValidationDelegate = delegate(global::System.Security.Cryptography.X509Certificates.X509Certificate cert, int[] certErrors)
				{
					MonoSslPolicyErrors monoSslPolicyErrors = ((certErrors.Length != 0) ? MonoSslPolicyErrors.RemoteCertificateChainErrors : MonoSslPolicyErrors.None);
					return ((ChainValidationHelper)this.certificateValidator).ValidateClientCertificate(cert, monoSslPolicyErrors);
				}
			};
			return this.BeginWrite(new byte[0], 0, 0, asyncCallback, asyncState);
		}

		private Mono.Security.Protocol.Tls.SecurityProtocolType GetMonoSslProtocol(SslProtocols ms)
		{
			if (ms == SslProtocols.Ssl2)
			{
				return Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl2;
			}
			if (ms == SslProtocols.Ssl3)
			{
				return Mono.Security.Protocol.Tls.SecurityProtocolType.Ssl3;
			}
			if (ms != SslProtocols.Tls)
			{
				return Mono.Security.Protocol.Tls.SecurityProtocolType.Default;
			}
			return Mono.Security.Protocol.Tls.SecurityProtocolType.Tls;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			this.CheckConnectionAuthenticated();
			return this.ssl_stream.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
		}

		public virtual void AuthenticateAsClient(string targetHost)
		{
			this.AuthenticateAsClient(targetHost, new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection(), SslProtocols.Tls, false);
		}

		public virtual void AuthenticateAsClient(string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			this.EndAuthenticateAsClient(this.BeginAuthenticateAsClient(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation, null, null));
		}

		public virtual void AuthenticateAsServer(global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate)
		{
			this.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, false);
		}

		public virtual void AuthenticateAsServer(global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			this.EndAuthenticateAsServer(this.BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation, null, null));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.ssl_stream != null)
				{
					this.ssl_stream.Dispose();
				}
				this.ssl_stream = null;
			}
			base.Dispose(disposing);
		}

		public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
		{
			this.CheckConnectionAuthenticated();
			if (this.CanRead)
			{
				this.ssl_stream.EndRead(asyncResult);
				return;
			}
			this.ssl_stream.EndWrite(asyncResult);
		}

		public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
		{
			this.CheckConnectionAuthenticated();
			if (this.CanRead)
			{
				this.ssl_stream.EndRead(asyncResult);
				return;
			}
			this.ssl_stream.EndWrite(asyncResult);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			this.CheckConnectionAuthenticated();
			return this.ssl_stream.EndRead(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			this.CheckConnectionAuthenticated();
			this.ssl_stream.EndWrite(asyncResult);
		}

		public override void Flush()
		{
			this.CheckConnectionAuthenticated();
			base.InnerStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.EndRead(this.BeginRead(buffer, offset, count, null, null));
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("This stream does not support seek operations");
		}

		public override void SetLength(long value)
		{
			base.InnerStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.EndWrite(this.BeginWrite(buffer, offset, count, null, null));
		}

		public void Write(byte[] buffer)
		{
			this.Write(buffer, 0, buffer.Length);
		}

		private void CheckConnectionAuthenticated()
		{
			if (!this.IsAuthenticated)
			{
				throw new InvalidOperationException("This operation is invalid until it is successfully authenticated");
			}
		}

		public virtual Task AuthenticateAsClientAsync(string targetHost)
		{
			return Task.Factory.FromAsync<string>(new Func<string, AsyncCallback, object, IAsyncResult>(this.BeginAuthenticateAsClient), new Action<IAsyncResult>(this.EndAuthenticateAsClient), targetHost, null);
		}

		public virtual Task AuthenticateAsClientAsync(string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			Tuple<string, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection, SslProtocols, bool, LegacySslStream> tuple = Tuple.Create<string, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection, SslProtocols, bool, LegacySslStream>(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation, this);
			return Task.Factory.FromAsync(delegate(AsyncCallback callback, object state)
			{
				Tuple<string, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection, SslProtocols, bool, LegacySslStream> tuple2 = (Tuple<string, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection, SslProtocols, bool, LegacySslStream>)state;
				return tuple2.Item5.BeginAuthenticateAsClient(tuple2.Item1, tuple2.Item2, tuple2.Item3, tuple2.Item4, callback, null);
			}, new Action<IAsyncResult>(this.EndAuthenticateAsClient), tuple);
		}

		public virtual Task AuthenticateAsServerAsync(global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate)
		{
			return Task.Factory.FromAsync<global::System.Security.Cryptography.X509Certificates.X509Certificate>(new Func<global::System.Security.Cryptography.X509Certificates.X509Certificate, AsyncCallback, object, IAsyncResult>(this.BeginAuthenticateAsServer), new Action<IAsyncResult>(this.EndAuthenticateAsServer), serverCertificate, null);
		}

		public virtual Task AuthenticateAsServerAsync(global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			Tuple<global::System.Security.Cryptography.X509Certificates.X509Certificate, bool, SslProtocols, bool, LegacySslStream> tuple = Tuple.Create<global::System.Security.Cryptography.X509Certificates.X509Certificate, bool, SslProtocols, bool, LegacySslStream>(serverCertificate, clientCertificateRequired, enabledSslProtocols, checkCertificateRevocation, this);
			return Task.Factory.FromAsync(delegate(AsyncCallback callback, object state)
			{
				Tuple<global::System.Security.Cryptography.X509Certificates.X509Certificate, bool, SslProtocols, bool, LegacySslStream> tuple2 = (Tuple<global::System.Security.Cryptography.X509Certificates.X509Certificate, bool, SslProtocols, bool, LegacySslStream>)state;
				return tuple2.Item5.BeginAuthenticateAsServer(tuple2.Item1, tuple2.Item2, tuple2.Item3, tuple2.Item4, callback, null);
			}, new Action<IAsyncResult>(this.EndAuthenticateAsServer), tuple);
		}

		Task IMonoSslStream.ShutdownAsync()
		{
			return Task.CompletedTask;
		}

		AuthenticatedStream IMonoSslStream.AuthenticatedStream
		{
			get
			{
				return this;
			}
		}

		TransportContext IMonoSslStream.TransportContext
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public SslStream SslStream { get; }

		public MonoTlsProvider Provider { get; }

		public MonoTlsConnectionInfo GetConnectionInfo()
		{
			return null;
		}

		private SslStreamBase ssl_stream;

		private ICertificateValidator certificateValidator;
	}
}
