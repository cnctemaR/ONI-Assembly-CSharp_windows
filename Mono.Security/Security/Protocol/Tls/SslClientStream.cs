using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Mono.Security.Interface;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	public class SslClientStream : SslStreamBase
	{
		internal event CertificateValidationCallback ServerCertValidation;

		internal event CertificateSelectionCallback ClientCertSelection;

		internal event PrivateKeySelectionCallback PrivateKeySelection;

		internal Stream InputBuffer
		{
			get
			{
				return this.inputBuffer;
			}
		}

		public global::System.Security.Cryptography.X509Certificates.X509CertificateCollection ClientCertificates
		{
			get
			{
				return this.context.ClientSettings.Certificates;
			}
		}

		public global::System.Security.Cryptography.X509Certificates.X509Certificate SelectedClientCertificate
		{
			get
			{
				return this.context.ClientSettings.ClientCertificate;
			}
		}

		public CertificateValidationCallback ServerCertValidationDelegate
		{
			get
			{
				return this.ServerCertValidation;
			}
			set
			{
				this.ServerCertValidation = value;
			}
		}

		public CertificateSelectionCallback ClientCertSelectionDelegate
		{
			get
			{
				return this.ClientCertSelection;
			}
			set
			{
				this.ClientCertSelection = value;
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

		public event CertificateValidationCallback2 ServerCertValidation2;

		public SslClientStream(Stream stream, string targetHost, bool ownsStream)
			: this(stream, targetHost, ownsStream, SecurityProtocolType.Default, null)
		{
		}

		public SslClientStream(Stream stream, string targetHost, global::System.Security.Cryptography.X509Certificates.X509Certificate clientCertificate)
			: this(stream, targetHost, false, SecurityProtocolType.Default, new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection(new global::System.Security.Cryptography.X509Certificates.X509Certificate[] { clientCertificate }))
		{
		}

		public SslClientStream(Stream stream, string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates)
			: this(stream, targetHost, false, SecurityProtocolType.Default, clientCertificates)
		{
		}

		public SslClientStream(Stream stream, string targetHost, bool ownsStream, SecurityProtocolType securityProtocolType)
			: this(stream, targetHost, ownsStream, securityProtocolType, new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection())
		{
		}

		public SslClientStream(Stream stream, string targetHost, bool ownsStream, SecurityProtocolType securityProtocolType, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates)
			: base(stream, ownsStream)
		{
			if (targetHost == null || targetHost.Length == 0)
			{
				throw new ArgumentNullException("targetHost is null or an empty string.");
			}
			this.context = new ClientContext(this, securityProtocolType, targetHost, clientCertificates);
			this.protocol = new ClientRecordProtocol(this.innerStream, (ClientContext)this.context);
		}

		~SslClientStream()
		{
			base.Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.ServerCertValidation = null;
				this.ClientCertSelection = null;
				this.PrivateKeySelection = null;
				this.ServerCertValidation2 = null;
			}
		}

		private void SafeEndReceiveRecord(IAsyncResult ar, bool ignoreEmpty = false)
		{
			byte[] array = this.protocol.EndReceiveRecord(ar);
			if (!ignoreEmpty && (array == null || array.Length == 0))
			{
				throw new TlsException(AlertDescription.HandshakeFailiure, "The server stopped the handshake.");
			}
		}

		internal override IAsyncResult BeginNegotiateHandshake(AsyncCallback callback, object state)
		{
			if (this.context.HandshakeState != HandshakeState.None)
			{
				this.context.Clear();
			}
			this.context.SupportedCiphers = CipherSuiteFactory.GetSupportedCiphers(false, this.context.SecurityProtocol);
			this.context.HandshakeState = HandshakeState.Started;
			SslClientStream.NegotiateAsyncResult negotiateAsyncResult = new SslClientStream.NegotiateAsyncResult(callback, state, SslClientStream.NegotiateState.SentClientHello);
			this.protocol.BeginSendRecord(HandshakeType.ClientHello, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
			return negotiateAsyncResult;
		}

		internal override void EndNegotiateHandshake(IAsyncResult result)
		{
			SslClientStream.NegotiateAsyncResult negotiateAsyncResult = result as SslClientStream.NegotiateAsyncResult;
			if (negotiateAsyncResult == null)
			{
				throw new ArgumentNullException();
			}
			if (!negotiateAsyncResult.IsCompleted)
			{
				negotiateAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (negotiateAsyncResult.CompletedWithError)
			{
				throw negotiateAsyncResult.AsyncException;
			}
		}

		private void NegotiateAsyncWorker(IAsyncResult result)
		{
			SslClientStream.NegotiateAsyncResult negotiateAsyncResult = result.AsyncState as SslClientStream.NegotiateAsyncResult;
			try
			{
				switch (negotiateAsyncResult.State)
				{
				case SslClientStream.NegotiateState.SentClientHello:
					this.protocol.EndSendRecord(result);
					negotiateAsyncResult.State = SslClientStream.NegotiateState.ReceiveClientHelloResponse;
					this.protocol.BeginReceiveRecord(this.innerStream, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
					goto IL_03C2;
				case SslClientStream.NegotiateState.ReceiveClientHelloResponse:
				{
					this.SafeEndReceiveRecord(result, true);
					if (this.context.LastHandshakeMsg != HandshakeType.ServerHelloDone && (!this.context.AbbreviatedHandshake || this.context.LastHandshakeMsg != HandshakeType.ServerHello))
					{
						this.protocol.BeginReceiveRecord(this.innerStream, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
						goto IL_03C2;
					}
					if (this.context.AbbreviatedHandshake)
					{
						ClientSessionCache.SetContextFromCache(this.context);
						this.context.Negotiating.Cipher.ComputeKeys();
						this.context.Negotiating.Cipher.InitializeCipher();
						negotiateAsyncResult.State = SslClientStream.NegotiateState.SentCipherSpec;
						this.protocol.BeginSendChangeCipherSpec(new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
						goto IL_03C2;
					}
					bool flag = this.context.ServerSettings.CertificateRequest;
					using (MemoryStream memoryStream = new MemoryStream())
					{
						if (this.context.SecurityProtocol == SecurityProtocolType.Ssl3)
						{
							flag = this.context.ClientSettings.Certificates != null && this.context.ClientSettings.Certificates.Count > 0;
						}
						byte[] array;
						if (flag)
						{
							array = this.protocol.EncodeHandshakeRecord(HandshakeType.Certificate);
							memoryStream.Write(array, 0, array.Length);
						}
						array = this.protocol.EncodeHandshakeRecord(HandshakeType.ClientKeyExchange);
						memoryStream.Write(array, 0, array.Length);
						this.context.Negotiating.Cipher.InitializeCipher();
						if (flag && this.context.ClientSettings.ClientCertificate != null)
						{
							array = this.protocol.EncodeHandshakeRecord(HandshakeType.CertificateVerify);
							memoryStream.Write(array, 0, array.Length);
						}
						this.protocol.SendChangeCipherSpec(memoryStream);
						array = this.protocol.EncodeHandshakeRecord(HandshakeType.Finished);
						memoryStream.Write(array, 0, array.Length);
						negotiateAsyncResult.State = SslClientStream.NegotiateState.SentKeyExchange;
						this.innerStream.BeginWrite(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
						goto IL_03C2;
					}
					break;
				}
				case SslClientStream.NegotiateState.SentCipherSpec:
					this.protocol.EndSendChangeCipherSpec(result);
					negotiateAsyncResult.State = SslClientStream.NegotiateState.ReceiveCipherSpecResponse;
					this.protocol.BeginReceiveRecord(this.innerStream, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
					goto IL_03C2;
				case SslClientStream.NegotiateState.ReceiveCipherSpecResponse:
					this.SafeEndReceiveRecord(result, true);
					if (this.context.HandshakeState != HandshakeState.Finished)
					{
						this.protocol.BeginReceiveRecord(this.innerStream, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
						goto IL_03C2;
					}
					negotiateAsyncResult.State = SslClientStream.NegotiateState.SentFinished;
					this.protocol.BeginSendRecord(HandshakeType.Finished, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
					goto IL_03C2;
				case SslClientStream.NegotiateState.SentKeyExchange:
					break;
				case SslClientStream.NegotiateState.ReceiveFinishResponse:
					this.SafeEndReceiveRecord(result, false);
					if (this.context.HandshakeState != HandshakeState.Finished)
					{
						this.protocol.BeginReceiveRecord(this.innerStream, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
						goto IL_03C2;
					}
					this.context.HandshakeMessages.Reset();
					this.context.ClearKeyInfo();
					negotiateAsyncResult.SetComplete();
					goto IL_03C2;
				case SslClientStream.NegotiateState.SentFinished:
					this.protocol.EndSendRecord(result);
					this.context.HandshakeMessages.Reset();
					this.context.ClearKeyInfo();
					negotiateAsyncResult.SetComplete();
					goto IL_03C2;
				default:
					goto IL_03C2;
				}
				this.innerStream.EndWrite(result);
				negotiateAsyncResult.State = SslClientStream.NegotiateState.ReceiveFinishResponse;
				this.protocol.BeginReceiveRecord(this.innerStream, new AsyncCallback(this.NegotiateAsyncWorker), negotiateAsyncResult);
				IL_03C2:;
			}
			catch (TlsException ex)
			{
				try
				{
					Exception ex2 = ex;
					this.protocol.SendAlert(ref ex2);
				}
				catch
				{
				}
				negotiateAsyncResult.SetComplete(new IOException("The authentication or decryption has failed.", ex));
			}
			catch (Exception ex3)
			{
				try
				{
					this.protocol.SendAlert(AlertDescription.InternalError);
				}
				catch
				{
				}
				negotiateAsyncResult.SetComplete(new IOException("The authentication or decryption has failed.", ex3));
			}
		}

		internal override global::System.Security.Cryptography.X509Certificates.X509Certificate OnLocalCertificateSelection(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCertificates)
		{
			if (this.ClientCertSelection != null)
			{
				return this.ClientCertSelection(clientCertificates, serverCertificate, targetHost, serverRequestedCertificates);
			}
			return null;
		}

		internal override bool HaveRemoteValidation2Callback
		{
			get
			{
				return this.ServerCertValidation2 != null;
			}
		}

		internal override ValidationResult OnRemoteCertificateValidation2(Mono.Security.X509.X509CertificateCollection collection)
		{
			CertificateValidationCallback2 serverCertValidation = this.ServerCertValidation2;
			if (serverCertValidation != null)
			{
				return serverCertValidation(collection);
			}
			return null;
		}

		internal override bool OnRemoteCertificateValidation(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] errors)
		{
			if (this.ServerCertValidation != null)
			{
				return this.ServerCertValidation(certificate, errors);
			}
			return errors != null && errors.Length == 0;
		}

		internal virtual bool RaiseServerCertificateValidation(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] certificateErrors)
		{
			return base.RaiseRemoteCertificateValidation(certificate, certificateErrors);
		}

		internal virtual ValidationResult RaiseServerCertificateValidation2(Mono.Security.X509.X509CertificateCollection collection)
		{
			return base.RaiseRemoteCertificateValidation2(collection);
		}

		internal global::System.Security.Cryptography.X509Certificates.X509Certificate RaiseClientCertificateSelection(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, global::System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCertificates)
		{
			return base.RaiseLocalCertificateSelection(clientCertificates, serverCertificate, targetHost, serverRequestedCertificates);
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

		private enum NegotiateState
		{
			SentClientHello,
			ReceiveClientHelloResponse,
			SentCipherSpec,
			ReceiveCipherSpecResponse,
			SentKeyExchange,
			ReceiveFinishResponse,
			SentFinished
		}

		private class NegotiateAsyncResult : IAsyncResult
		{
			public NegotiateAsyncResult(AsyncCallback userCallback, object userState, SslClientStream.NegotiateState state)
			{
				this._userCallback = userCallback;
				this._userState = userState;
				this._state = state;
			}

			public SslClientStream.NegotiateState State
			{
				get
				{
					return this._state;
				}
				set
				{
					this._state = value;
				}
			}

			public object AsyncState
			{
				get
				{
					return this._userState;
				}
			}

			public Exception AsyncException
			{
				get
				{
					return this._asyncException;
				}
			}

			public bool CompletedWithError
			{
				get
				{
					return this.IsCompleted && this._asyncException != null;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					object obj = this.locker;
					lock (obj)
					{
						if (this.handle == null)
						{
							this.handle = new ManualResetEvent(this.completed);
						}
					}
					return this.handle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					object obj = this.locker;
					bool flag2;
					lock (obj)
					{
						flag2 = this.completed;
					}
					return flag2;
				}
			}

			public void SetComplete(Exception ex)
			{
				object obj = this.locker;
				lock (obj)
				{
					if (!this.completed)
					{
						this.completed = true;
						if (this.handle != null)
						{
							this.handle.Set();
						}
						if (this._userCallback != null)
						{
							this._userCallback.BeginInvoke(this, null, null);
						}
						this._asyncException = ex;
					}
				}
			}

			public void SetComplete()
			{
				this.SetComplete(null);
			}

			private object locker = new object();

			private AsyncCallback _userCallback;

			private object _userState;

			private Exception _asyncException;

			private ManualResetEvent handle;

			private SslClientStream.NegotiateState _state;

			private bool completed;
		}
	}
}
