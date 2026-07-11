using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Protocol.Tls;

namespace System.Net.Security
{
	[global::System.MonoTODO("Non-X509Certificate2 certificate is not supported")]
	public class SslStream : AuthenticatedStream
	{
		public SslStream(Stream innerStream)
			: this(innerStream, false)
		{
		}

		public SslStream(Stream innerStream, bool leaveStreamOpen)
			: base(innerStream, leaveStreamOpen)
		{
		}

		[global::System.MonoTODO("certValidationCallback is not passed X509Chain and SslPolicyErrors correctly")]
		public SslStream(Stream innerStream, bool leaveStreamOpen, RemoteCertificateValidationCallback certValidationCallback)
			: this(innerStream, leaveStreamOpen, certValidationCallback, null)
		{
		}

		[global::System.MonoTODO("certValidationCallback is not passed X509Chain and SslPolicyErrors correctly")]
		public SslStream(Stream innerStream, bool leaveStreamOpen, RemoteCertificateValidationCallback certValidationCallback, LocalCertificateSelectionCallback certSelectionCallback)
			: base(innerStream, leaveStreamOpen)
		{
			this.validation_callback = certValidationCallback;
			this.selection_callback = certSelectionCallback;
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
				return this.IsAuthenticated && ((!this.IsServer) ? (this.LocalCertificate != null) : (this.RemoteCertificate != null));
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

		public virtual X509Certificate LocalCertificate
		{
			get
			{
				this.CheckConnectionAuthenticated();
				return (!this.IsServer) ? ((SslClientStream)this.ssl_stream).SelectedClientCertificate : this.ssl_stream.ServerCertificate;
			}
		}

		public virtual X509Certificate RemoteCertificate
		{
			get
			{
				this.CheckConnectionAuthenticated();
				return this.IsServer ? ((SslServerStream)this.ssl_stream).ClientCertificate : this.ssl_stream.ServerCertificate;
			}
		}

		public virtual global::System.Security.Authentication.SslProtocols SslProtocol
		{
			get
			{
				this.CheckConnectionAuthenticated();
				SecurityProtocolType securityProtocol = this.ssl_stream.SecurityProtocol;
				if (securityProtocol == SecurityProtocolType.Default)
				{
					return global::System.Security.Authentication.SslProtocols.Default;
				}
				if (securityProtocol == SecurityProtocolType.Ssl2)
				{
					return global::System.Security.Authentication.SslProtocols.Ssl2;
				}
				if (securityProtocol == SecurityProtocolType.Ssl3)
				{
					return global::System.Security.Authentication.SslProtocols.Ssl3;
				}
				if (securityProtocol != SecurityProtocolType.Tls)
				{
					throw new InvalidOperationException("Not supported SSL/TLS protocol is in use. It is likely a bug in SslStream.");
				}
				return global::System.Security.Authentication.SslProtocols.Tls;
			}
		}

		private X509Certificate OnCertificateSelection(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCerts, X509Certificate serverCert, string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCerts)
		{
			string[] array = new string[(serverRequestedCerts == null) ? 0 : serverRequestedCerts.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = serverRequestedCerts[i].GetIssuerName();
			}
			return this.selection_callback(this, targetHost, clientCerts, serverCert, array);
		}

		public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginAuthenticateAsClient(targetHost, new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection(), global::System.Security.Authentication.SslProtocols.Tls, false, asyncCallback, asyncState);
		}

		public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, global::System.Security.Authentication.SslProtocols sslProtocolType, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.IsAuthenticated)
			{
				throw new InvalidOperationException("This SslStream is already authenticated");
			}
			SslClientStream sslClientStream = new SslClientStream(base.InnerStream, targetHost, !base.LeaveInnerStreamOpen, this.GetMonoSslProtocol(sslProtocolType), clientCertificates);
			sslClientStream.CheckCertRevocationStatus = checkCertificateRevocation;
			sslClientStream.PrivateKeyCertSelectionDelegate = delegate(X509Certificate cert, string host)
			{
				string certHashString = cert.GetCertHashString();
				foreach (X509Certificate x509Certificate in clientCertificates)
				{
					if (!(x509Certificate.GetCertHashString() != certHashString))
					{
						global::System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate2 = x509Certificate as global::System.Security.Cryptography.X509Certificates.X509Certificate2;
						x509Certificate2 = x509Certificate2 ?? new global::System.Security.Cryptography.X509Certificates.X509Certificate2(x509Certificate);
						return x509Certificate2.PrivateKey;
					}
				}
				return null;
			};
			if (this.validation_callback != null)
			{
				sslClientStream.ServerCertValidationDelegate = delegate(X509Certificate cert, int[] certErrors)
				{
					global::System.Security.Cryptography.X509Certificates.X509Chain x509Chain = new global::System.Security.Cryptography.X509Certificates.X509Chain();
					global::System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate3 = cert as global::System.Security.Cryptography.X509Certificates.X509Certificate2;
					if (x509Certificate3 == null)
					{
						x509Certificate3 = new global::System.Security.Cryptography.X509Certificates.X509Certificate2(cert);
					}
					if (!ServicePointManager.CheckCertificateRevocationList)
					{
						x509Chain.ChainPolicy.RevocationMode = global::System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
					}
					SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
					foreach (int num in certErrors)
					{
						int num2 = num;
						if (num2 != -2146762490)
						{
							if (num2 != -2146762481)
							{
								sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
							}
							else
							{
								sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
							}
						}
						else
						{
							sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNotAvailable;
						}
					}
					x509Chain.Build(x509Certificate3);
					foreach (global::System.Security.Cryptography.X509Certificates.X509ChainStatus x509ChainStatus in x509Chain.ChainStatus)
					{
						if (x509ChainStatus.Status != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
						{
							if ((x509ChainStatus.Status & global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.PartialChain) != global::System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
							{
								sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNotAvailable;
							}
							else
							{
								sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
							}
						}
					}
					return this.validation_callback(this, cert, x509Chain, sslPolicyErrors);
				};
			}
			if (this.selection_callback != null)
			{
				sslClientStream.ClientCertSelectionDelegate = new CertificateSelectionCallback(this.OnCertificateSelection);
			}
			this.ssl_stream = sslClientStream;
			return this.BeginWrite(new byte[0], 0, 0, asyncCallback, asyncState);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			this.CheckConnectionAuthenticated();
			return this.ssl_stream.BeginRead(buffer, offset, count, asyncCallback, asyncState);
		}

		public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback callback, object asyncState)
		{
			return this.BeginAuthenticateAsServer(serverCertificate, false, global::System.Security.Authentication.SslProtocols.Tls, false, callback, asyncState);
		}

		public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, global::System.Security.Authentication.SslProtocols sslProtocolType, bool checkCertificateRevocation, AsyncCallback callback, object asyncState)
		{
			if (this.IsAuthenticated)
			{
				throw new InvalidOperationException("This SslStream is already authenticated");
			}
			SslServerStream sslServerStream = new SslServerStream(base.InnerStream, serverCertificate, clientCertificateRequired, !base.LeaveInnerStreamOpen, this.GetMonoSslProtocol(sslProtocolType));
			sslServerStream.CheckCertRevocationStatus = checkCertificateRevocation;
			sslServerStream.PrivateKeyCertSelectionDelegate = delegate(X509Certificate cert, string targetHost)
			{
				global::System.Security.Cryptography.X509Certificates.X509Certificate2 x509Certificate = (serverCertificate as global::System.Security.Cryptography.X509Certificates.X509Certificate2) ?? new global::System.Security.Cryptography.X509Certificates.X509Certificate2(serverCertificate);
				return (x509Certificate == null) ? null : x509Certificate.PrivateKey;
			};
			if (this.validation_callback != null)
			{
				sslServerStream.ClientCertValidationDelegate = delegate(X509Certificate cert, int[] certErrors)
				{
					global::System.Security.Cryptography.X509Certificates.X509Chain x509Chain = null;
					if (cert is global::System.Security.Cryptography.X509Certificates.X509Certificate2)
					{
						x509Chain = new global::System.Security.Cryptography.X509Certificates.X509Chain();
						x509Chain.Build((global::System.Security.Cryptography.X509Certificates.X509Certificate2)cert);
					}
					SslPolicyErrors sslPolicyErrors = ((certErrors.Length <= 0) ? SslPolicyErrors.None : SslPolicyErrors.RemoteCertificateChainErrors);
					return this.validation_callback(this, cert, x509Chain, sslPolicyErrors);
				};
			}
			this.ssl_stream = sslServerStream;
			return this.BeginRead(new byte[0], 0, 0, callback, asyncState);
		}

		private SecurityProtocolType GetMonoSslProtocol(global::System.Security.Authentication.SslProtocols ms)
		{
			if (ms == global::System.Security.Authentication.SslProtocols.Ssl2)
			{
				return SecurityProtocolType.Ssl2;
			}
			if (ms == global::System.Security.Authentication.SslProtocols.Ssl3)
			{
				return SecurityProtocolType.Ssl3;
			}
			if (ms != global::System.Security.Authentication.SslProtocols.Tls)
			{
				return SecurityProtocolType.Default;
			}
			return SecurityProtocolType.Tls;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			this.CheckConnectionAuthenticated();
			return this.ssl_stream.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
		}

		public virtual void AuthenticateAsClient(string targetHost)
		{
			this.AuthenticateAsClient(targetHost, new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection(), global::System.Security.Authentication.SslProtocols.Tls, false);
		}

		public virtual void AuthenticateAsClient(string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, global::System.Security.Authentication.SslProtocols sslProtocolType, bool checkCertificateRevocation)
		{
			this.EndAuthenticateAsClient(this.BeginAuthenticateAsClient(targetHost, clientCertificates, sslProtocolType, checkCertificateRevocation, null, null));
		}

		public virtual void AuthenticateAsServer(X509Certificate serverCertificate)
		{
			this.AuthenticateAsServer(serverCertificate, false, global::System.Security.Authentication.SslProtocols.Tls, false);
		}

		public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, global::System.Security.Authentication.SslProtocols sslProtocolType, bool checkCertificateRevocation)
		{
			this.EndAuthenticateAsServer(this.BeginAuthenticateAsServer(serverCertificate, clientCertificateRequired, sslProtocolType, checkCertificateRevocation, null, null));
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
			}
			else
			{
				this.ssl_stream.EndWrite(asyncResult);
			}
		}

		public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
		{
			this.CheckConnectionAuthenticated();
			if (this.CanRead)
			{
				this.ssl_stream.EndRead(asyncResult);
			}
			else
			{
				this.ssl_stream.EndWrite(asyncResult);
			}
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

		private SslStreamBase ssl_stream;

		private RemoteCertificateValidationCallback validation_callback;

		private LocalCertificateSelectionCallback selection_callback;
	}
}
