using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal abstract class MobileAuthenticatedStream : AuthenticatedStream, IMonoSslStream, IDisposable
	{
		public MobileAuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen, SslStream owner, MonoTlsSettings settings, MobileTlsProvider provider)
			: base(innerStream, leaveInnerStreamOpen)
		{
			this.SslStream = owner;
			this.Settings = settings;
			this.Provider = provider;
			this.readBuffer = new BufferOffsetSize2(16500);
			this.writeBuffer = new BufferOffsetSize2(16384);
			this.operation = MobileAuthenticatedStream.Operation.None;
		}

		public SslStream SslStream { get; }

		public MonoTlsSettings Settings { get; }

		public MobileTlsProvider Provider { get; }

		MonoTlsProvider IMonoSslStream.Provider
		{
			get
			{
				return this.Provider;
			}
		}

		internal bool HasContext
		{
			get
			{
				return this.xobileTlsContext != null;
			}
		}

		internal string TargetHost { get; private set; }

		internal void CheckThrow(bool authSuccessCheck, bool shutdownCheck = false)
		{
			if (this.lastException != null)
			{
				this.lastException.Throw();
			}
			if (authSuccessCheck && !this.IsAuthenticated)
			{
				throw new InvalidOperationException("This operation is only allowed using a successfully authenticated context.");
			}
			if (shutdownCheck && this.shutdown)
			{
				throw new InvalidOperationException("Write operations are not allowed after the channel was shutdown.");
			}
		}

		internal static Exception GetSSPIException(Exception e)
		{
			if (e is OperationCanceledException || e is IOException || e is ObjectDisposedException || e is AuthenticationException || e is NotSupportedException)
			{
				return e;
			}
			return new AuthenticationException("Authentication failed, see inner exception.", e);
		}

		internal static Exception GetIOException(Exception e, string message)
		{
			if (e is OperationCanceledException || e is IOException || e is ObjectDisposedException || e is AuthenticationException || e is NotSupportedException)
			{
				return e;
			}
			return new IOException(message, e);
		}

		internal static Exception GetRenegotiationException(string message)
		{
			TlsException ex = new TlsException(AlertDescription.NoRenegotiation, message);
			return new AuthenticationException("Authentication failed, see inner exception.", ex);
		}

		internal static Exception GetInternalError()
		{
			throw new InvalidOperationException("Internal error.");
		}

		internal static Exception GetInvalidNestedCallException()
		{
			throw new InvalidOperationException("Invalid nested call.");
		}

		internal ExceptionDispatchInfo SetException(Exception e)
		{
			ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
			return Interlocked.CompareExchange<ExceptionDispatchInfo>(ref this.lastException, exceptionDispatchInfo, null) ?? exceptionDispatchInfo;
		}

		public void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			MonoSslClientAuthenticationOptions monoSslClientAuthenticationOptions = new MonoSslClientAuthenticationOptions
			{
				TargetHost = targetHost,
				ClientCertificates = clientCertificates,
				EnabledSslProtocols = enabledSslProtocols,
				CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
				EncryptionPolicy = EncryptionPolicy.RequireEncryption
			};
			Task task = this.ProcessAuthentication(true, monoSslClientAuthenticationOptions, CancellationToken.None);
			try
			{
				task.Wait();
			}
			catch (Exception ex)
			{
				throw HttpWebRequest.FlattenException(ex);
			}
		}

		public void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			MonoSslServerAuthenticationOptions monoSslServerAuthenticationOptions = new MonoSslServerAuthenticationOptions
			{
				ServerCertificate = serverCertificate,
				ClientCertificateRequired = clientCertificateRequired,
				EnabledSslProtocols = enabledSslProtocols,
				CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
				EncryptionPolicy = EncryptionPolicy.RequireEncryption
			};
			Task task = this.ProcessAuthentication(true, monoSslServerAuthenticationOptions, CancellationToken.None);
			try
			{
				task.Wait();
			}
			catch (Exception ex)
			{
				throw HttpWebRequest.FlattenException(ex);
			}
		}

		public Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			MonoSslClientAuthenticationOptions monoSslClientAuthenticationOptions = new MonoSslClientAuthenticationOptions
			{
				TargetHost = targetHost,
				ClientCertificates = clientCertificates,
				EnabledSslProtocols = enabledSslProtocols,
				CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
				EncryptionPolicy = EncryptionPolicy.RequireEncryption
			};
			return this.ProcessAuthentication(false, monoSslClientAuthenticationOptions, CancellationToken.None);
		}

		public Task AuthenticateAsClientAsync(IMonoSslClientAuthenticationOptions sslClientAuthenticationOptions, CancellationToken cancellationToken)
		{
			return this.ProcessAuthentication(false, (MonoSslClientAuthenticationOptions)sslClientAuthenticationOptions, cancellationToken);
		}

		public Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			MonoSslServerAuthenticationOptions monoSslServerAuthenticationOptions = new MonoSslServerAuthenticationOptions
			{
				ServerCertificate = serverCertificate,
				ClientCertificateRequired = clientCertificateRequired,
				EnabledSslProtocols = enabledSslProtocols,
				CertificateRevocationCheckMode = (checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck),
				EncryptionPolicy = EncryptionPolicy.RequireEncryption
			};
			return this.ProcessAuthentication(false, monoSslServerAuthenticationOptions, CancellationToken.None);
		}

		public Task AuthenticateAsServerAsync(IMonoSslServerAuthenticationOptions sslServerAuthenticationOptions, CancellationToken cancellationToken)
		{
			return this.ProcessAuthentication(false, (MonoSslServerAuthenticationOptions)sslServerAuthenticationOptions, cancellationToken);
		}

		public Task ShutdownAsync()
		{
			AsyncShutdownRequest asyncShutdownRequest = new AsyncShutdownRequest(this);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Shutdown, asyncShutdownRequest, CancellationToken.None);
		}

		public AuthenticatedStream AuthenticatedStream
		{
			get
			{
				return this;
			}
		}

		private async Task ProcessAuthentication(bool runSynchronously, MonoSslAuthenticationOptions options, CancellationToken cancellationToken)
		{
			if (options.ServerMode)
			{
				if (options.ServerCertificate == null && options.ServerCertSelectionDelegate == null)
				{
					throw new ArgumentException("ServerCertificate");
				}
			}
			else
			{
				if (options.TargetHost == null)
				{
					throw new ArgumentException("TargetHost");
				}
				if (options.TargetHost.Length == 0)
				{
					options.TargetHost = "?" + Interlocked.Increment(ref MobileAuthenticatedStream.uniqueNameInteger).ToString(NumberFormatInfo.InvariantInfo);
				}
				this.TargetHost = options.TargetHost;
			}
			if (this.lastException != null)
			{
				this.lastException.Throw();
			}
			AsyncHandshakeRequest asyncHandshakeRequest = new AsyncHandshakeRequest(this, runSynchronously);
			if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncHandshakeRequest, asyncHandshakeRequest, null) != null)
			{
				throw MobileAuthenticatedStream.GetInvalidNestedCallException();
			}
			if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncReadRequest, asyncHandshakeRequest, null) != null)
			{
				throw MobileAuthenticatedStream.GetInvalidNestedCallException();
			}
			if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncWriteRequest, asyncHandshakeRequest, null) != null)
			{
				throw MobileAuthenticatedStream.GetInvalidNestedCallException();
			}
			AsyncProtocolResult asyncProtocolResult;
			try
			{
				object obj = this.ioLock;
				lock (obj)
				{
					if (this.xobileTlsContext != null)
					{
						throw new InvalidOperationException();
					}
					this.readBuffer.Reset();
					this.writeBuffer.Reset();
					this.xobileTlsContext = this.CreateContext(options);
				}
				try
				{
					asyncProtocolResult = await asyncHandshakeRequest.StartOperation(cancellationToken).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					asyncProtocolResult = new AsyncProtocolResult(this.SetException(MobileAuthenticatedStream.GetSSPIException(ex)));
				}
			}
			finally
			{
				object obj = this.ioLock;
				bool flag = false;
				try
				{
					Monitor.Enter(obj, ref flag);
					this.readBuffer.Reset();
					this.writeBuffer.Reset();
					this.asyncWriteRequest = null;
					this.asyncReadRequest = null;
					this.asyncHandshakeRequest = null;
				}
				finally
				{
					int num;
					if (num < 0 && flag)
					{
						Monitor.Exit(obj);
					}
				}
			}
			if (asyncProtocolResult.Error != null)
			{
				asyncProtocolResult.Error.Throw();
			}
		}

		protected abstract MobileTlsContext CreateContext(MonoSslAuthenticationOptions options);

		public override int Read(byte[] buffer, int offset, int count)
		{
			AsyncReadRequest asyncReadRequest = new AsyncReadRequest(this, true, buffer, offset, count);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Read, asyncReadRequest, CancellationToken.None).Result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			AsyncWriteRequest asyncWriteRequest = new AsyncWriteRequest(this, true, buffer, offset, count);
			this.StartOperation(MobileAuthenticatedStream.OperationType.Write, asyncWriteRequest, CancellationToken.None).Wait();
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			AsyncReadRequest asyncReadRequest = new AsyncReadRequest(this, false, buffer, offset, count);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Read, asyncReadRequest, cancellationToken);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			AsyncWriteRequest asyncWriteRequest = new AsyncWriteRequest(this, false, buffer, offset, count);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Write, asyncWriteRequest, cancellationToken);
		}

		public bool CanRenegotiate
		{
			get
			{
				this.CheckThrow(true, false);
				return this.xobileTlsContext != null && this.xobileTlsContext.CanRenegotiate;
			}
		}

		public Task RenegotiateAsync(CancellationToken cancellationToken)
		{
			AsyncRenegotiateRequest asyncRenegotiateRequest = new AsyncRenegotiateRequest(this);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Renegotiate, asyncRenegotiateRequest, cancellationToken);
		}

		private async Task<int> StartOperation(MobileAuthenticatedStream.OperationType type, AsyncProtocolRequest asyncRequest, CancellationToken cancellationToken)
		{
			this.CheckThrow(true, type > MobileAuthenticatedStream.OperationType.Read);
			if (type == MobileAuthenticatedStream.OperationType.Read)
			{
				if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncReadRequest, asyncRequest, null) != null)
				{
					throw MobileAuthenticatedStream.GetInvalidNestedCallException();
				}
			}
			else if (type == MobileAuthenticatedStream.OperationType.Renegotiate)
			{
				if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncHandshakeRequest, asyncRequest, null) != null)
				{
					throw MobileAuthenticatedStream.GetInvalidNestedCallException();
				}
				if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncReadRequest, asyncRequest, null) != null)
				{
					throw MobileAuthenticatedStream.GetInvalidNestedCallException();
				}
				if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncWriteRequest, asyncRequest, null) != null)
				{
					throw MobileAuthenticatedStream.GetInvalidNestedCallException();
				}
			}
			else if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncWriteRequest, asyncRequest, null) != null)
			{
				throw MobileAuthenticatedStream.GetInvalidNestedCallException();
			}
			AsyncProtocolResult asyncProtocolResult;
			try
			{
				object obj = this.ioLock;
				lock (obj)
				{
					if (type == MobileAuthenticatedStream.OperationType.Read)
					{
						this.readBuffer.Reset();
					}
					else
					{
						this.writeBuffer.Reset();
					}
				}
				asyncProtocolResult = await asyncRequest.StartOperation(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				asyncProtocolResult = new AsyncProtocolResult(this.SetException(MobileAuthenticatedStream.GetIOException(ex, asyncRequest.Name + " failed")));
			}
			finally
			{
				object obj = this.ioLock;
				bool flag = false;
				try
				{
					Monitor.Enter(obj, ref flag);
					if (type == MobileAuthenticatedStream.OperationType.Read)
					{
						this.readBuffer.Reset();
						this.asyncReadRequest = null;
					}
					else if (type == MobileAuthenticatedStream.OperationType.Renegotiate)
					{
						this.readBuffer.Reset();
						this.writeBuffer.Reset();
						this.asyncHandshakeRequest = null;
						this.asyncReadRequest = null;
						this.asyncWriteRequest = null;
					}
					else
					{
						this.writeBuffer.Reset();
						this.asyncWriteRequest = null;
					}
				}
				finally
				{
					int num;
					if (num < 0 && flag)
					{
						Monitor.Exit(obj);
					}
				}
			}
			if (asyncProtocolResult.Error != null)
			{
				asyncProtocolResult.Error.Throw();
			}
			return asyncProtocolResult.UserResult;
		}

		[Conditional("MONO_TLS_DEBUG")]
		protected internal void Debug(string format, params object[] args)
		{
		}

		[Conditional("MONO_TLS_DEBUG")]
		protected internal void Debug(string message)
		{
		}

		internal int InternalRead(byte[] buffer, int offset, int size, out bool outWantMore)
		{
			int num;
			try
			{
				AsyncProtocolRequest asyncProtocolRequest = this.asyncHandshakeRequest ?? this.asyncReadRequest;
				ValueTuple<int, bool> valueTuple = this.InternalRead(asyncProtocolRequest, this.readBuffer, buffer, offset, size);
				int item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				outWantMore = item2;
				num = item;
			}
			catch (Exception ex)
			{
				this.SetException(MobileAuthenticatedStream.GetIOException(ex, "InternalRead() failed"));
				outWantMore = false;
				num = -1;
			}
			return num;
		}

		private ValueTuple<int, bool> InternalRead(AsyncProtocolRequest asyncRequest, BufferOffsetSize internalBuffer, byte[] buffer, int offset, int size)
		{
			if (asyncRequest == null)
			{
				throw new InvalidOperationException();
			}
			if (internalBuffer.Size == 0 && !internalBuffer.Complete)
			{
				internalBuffer.Offset = (internalBuffer.Size = 0);
				asyncRequest.RequestRead(size);
				return new ValueTuple<int, bool>(0, true);
			}
			int num = Math.Min(internalBuffer.Size, size);
			Buffer.BlockCopy(internalBuffer.Buffer, internalBuffer.Offset, buffer, offset, num);
			internalBuffer.Offset += num;
			internalBuffer.Size -= num;
			return new ValueTuple<int, bool>(num, !internalBuffer.Complete && num < size);
		}

		internal bool InternalWrite(byte[] buffer, int offset, int size)
		{
			bool flag;
			try
			{
				AsyncProtocolRequest asyncProtocolRequest;
				switch (this.operation)
				{
				case MobileAuthenticatedStream.Operation.Handshake:
				case MobileAuthenticatedStream.Operation.Renegotiate:
					asyncProtocolRequest = this.asyncHandshakeRequest;
					goto IL_0057;
				case MobileAuthenticatedStream.Operation.Read:
					asyncProtocolRequest = this.asyncReadRequest;
					if (this.xobileTlsContext.PendingRenegotiation())
					{
						goto IL_0057;
					}
					goto IL_0057;
				case MobileAuthenticatedStream.Operation.Write:
				case MobileAuthenticatedStream.Operation.Close:
					asyncProtocolRequest = this.asyncWriteRequest;
					goto IL_0057;
				}
				throw MobileAuthenticatedStream.GetInternalError();
				IL_0057:
				if (asyncProtocolRequest == null && this.operation != MobileAuthenticatedStream.Operation.Close)
				{
					throw MobileAuthenticatedStream.GetInternalError();
				}
				flag = this.InternalWrite(asyncProtocolRequest, this.writeBuffer, buffer, offset, size);
			}
			catch (Exception ex)
			{
				this.SetException(MobileAuthenticatedStream.GetIOException(ex, "InternalWrite() failed"));
				flag = false;
			}
			return flag;
		}

		private bool InternalWrite(AsyncProtocolRequest asyncRequest, BufferOffsetSize2 internalBuffer, byte[] buffer, int offset, int size)
		{
			if (asyncRequest == null)
			{
				if (this.lastException != null)
				{
					return false;
				}
				if (Interlocked.Exchange(ref this.closeRequested, 1) == 0)
				{
					internalBuffer.Reset();
				}
				else if (internalBuffer.Remaining == 0)
				{
					throw new InvalidOperationException();
				}
			}
			internalBuffer.AppendData(buffer, offset, size);
			if (asyncRequest != null)
			{
				asyncRequest.RequestWrite();
			}
			return true;
		}

		internal async Task<int> InnerRead(bool sync, int requestedSize, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int len = Math.Min(this.readBuffer.Remaining, requestedSize);
			if (len == 0)
			{
				throw new InvalidOperationException();
			}
			Task<int> task;
			if (sync)
			{
				task = Task.Run<int>(() => this.InnerStream.Read(this.readBuffer.Buffer, this.readBuffer.EndOffset, len));
			}
			else
			{
				task = base.InnerStream.ReadAsync(this.readBuffer.Buffer, this.readBuffer.EndOffset, len, cancellationToken);
			}
			int num = await task.ConfigureAwait(false);
			if (num >= 0)
			{
				this.readBuffer.Size += num;
				this.readBuffer.TotalBytes += num;
			}
			if (num == 0)
			{
				this.readBuffer.Complete = true;
				if (this.readBuffer.TotalBytes > 0)
				{
					num = -1;
				}
			}
			return num;
		}

		internal async Task InnerWrite(bool sync, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (this.writeBuffer.Size != 0)
			{
				Task task;
				if (sync)
				{
					task = Task.Run(delegate
					{
						base.InnerStream.Write(this.writeBuffer.Buffer, this.writeBuffer.Offset, this.writeBuffer.Size);
					});
				}
				else
				{
					task = base.InnerStream.WriteAsync(this.writeBuffer.Buffer, this.writeBuffer.Offset, this.writeBuffer.Size);
				}
				await task.ConfigureAwait(false);
				this.writeBuffer.TotalBytes += this.writeBuffer.Size;
				BufferOffsetSize bufferOffsetSize = this.writeBuffer;
				BufferOffsetSize bufferOffsetSize2 = this.writeBuffer;
				int num = 0;
				bufferOffsetSize2.Size = num;
				bufferOffsetSize.Offset = num;
			}
		}

		internal AsyncOperationStatus ProcessHandshake(AsyncOperationStatus status, bool renegotiate)
		{
			object obj = this.ioLock;
			AsyncOperationStatus asyncOperationStatus;
			lock (obj)
			{
				switch (this.operation)
				{
				case MobileAuthenticatedStream.Operation.None:
					if (renegotiate)
					{
						throw MobileAuthenticatedStream.GetInternalError();
					}
					this.operation = MobileAuthenticatedStream.Operation.Handshake;
					break;
				case MobileAuthenticatedStream.Operation.Handshake:
				case MobileAuthenticatedStream.Operation.Renegotiate:
					break;
				case MobileAuthenticatedStream.Operation.Authenticated:
					if (!renegotiate)
					{
						throw MobileAuthenticatedStream.GetInternalError();
					}
					this.operation = MobileAuthenticatedStream.Operation.Renegotiate;
					break;
				default:
					throw MobileAuthenticatedStream.GetInternalError();
				}
				switch (status)
				{
				case AsyncOperationStatus.Initialize:
					if (renegotiate)
					{
						this.xobileTlsContext.Renegotiate();
					}
					else
					{
						this.xobileTlsContext.StartHandshake();
					}
					asyncOperationStatus = AsyncOperationStatus.Continue;
					break;
				case AsyncOperationStatus.Continue:
				{
					AsyncOperationStatus asyncOperationStatus2 = AsyncOperationStatus.Continue;
					try
					{
						if (this.xobileTlsContext.ProcessHandshake())
						{
							this.xobileTlsContext.FinishHandshake();
							this.operation = MobileAuthenticatedStream.Operation.Authenticated;
							asyncOperationStatus2 = AsyncOperationStatus.Complete;
						}
					}
					catch (Exception ex)
					{
						this.SetException(MobileAuthenticatedStream.GetSSPIException(ex));
						base.Dispose();
						throw;
					}
					if (this.lastException != null)
					{
						this.lastException.Throw();
					}
					asyncOperationStatus = asyncOperationStatus2;
					break;
				}
				case AsyncOperationStatus.ReadDone:
					throw new IOException("Authentication failed because the remote party has closed the transport stream.");
				default:
					throw new InvalidOperationException();
				}
			}
			return asyncOperationStatus;
		}

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		internal ValueTuple<int, bool> ProcessRead(BufferOffsetSize userBuffer)
		{
			object obj = this.ioLock;
			ValueTuple<int, bool> valueTuple2;
			lock (obj)
			{
				if (this.operation != MobileAuthenticatedStream.Operation.Authenticated)
				{
					throw MobileAuthenticatedStream.GetInternalError();
				}
				this.operation = MobileAuthenticatedStream.Operation.Read;
				ValueTuple<int, bool> valueTuple = this.xobileTlsContext.Read(userBuffer.Buffer, userBuffer.Offset, userBuffer.Size);
				if (this.lastException != null)
				{
					this.lastException.Throw();
				}
				this.operation = MobileAuthenticatedStream.Operation.Authenticated;
				valueTuple2 = valueTuple;
			}
			return valueTuple2;
		}

		[return: TupleElementNames(new string[] { "ret", "wantMore" })]
		internal ValueTuple<int, bool> ProcessWrite(BufferOffsetSize userBuffer)
		{
			object obj = this.ioLock;
			ValueTuple<int, bool> valueTuple2;
			lock (obj)
			{
				if (this.operation != MobileAuthenticatedStream.Operation.Authenticated)
				{
					throw MobileAuthenticatedStream.GetInternalError();
				}
				this.operation = MobileAuthenticatedStream.Operation.Write;
				ValueTuple<int, bool> valueTuple = this.xobileTlsContext.Write(userBuffer.Buffer, userBuffer.Offset, userBuffer.Size);
				if (this.lastException != null)
				{
					this.lastException.Throw();
				}
				this.operation = MobileAuthenticatedStream.Operation.Authenticated;
				valueTuple2 = valueTuple;
			}
			return valueTuple2;
		}

		internal AsyncOperationStatus ProcessShutdown(AsyncOperationStatus status)
		{
			object obj = this.ioLock;
			AsyncOperationStatus asyncOperationStatus;
			lock (obj)
			{
				if (this.operation != MobileAuthenticatedStream.Operation.Authenticated)
				{
					throw MobileAuthenticatedStream.GetInternalError();
				}
				this.operation = MobileAuthenticatedStream.Operation.Close;
				this.xobileTlsContext.Shutdown();
				this.shutdown = true;
				this.operation = MobileAuthenticatedStream.Operation.Authenticated;
				asyncOperationStatus = AsyncOperationStatus.Complete;
			}
			return asyncOperationStatus;
		}

		public override bool IsServer
		{
			get
			{
				this.CheckThrow(false, false);
				return this.xobileTlsContext != null && this.xobileTlsContext.IsServer;
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				object obj = this.ioLock;
				bool flag2;
				lock (obj)
				{
					flag2 = this.xobileTlsContext != null && this.lastException == null && this.xobileTlsContext.IsAuthenticated;
				}
				return flag2;
			}
		}

		public override bool IsMutuallyAuthenticated
		{
			get
			{
				object obj = this.ioLock;
				bool flag2;
				lock (obj)
				{
					if (!this.IsAuthenticated)
					{
						flag2 = false;
					}
					else if ((this.xobileTlsContext.IsServer ? this.xobileTlsContext.LocalServerCertificate : this.xobileTlsContext.LocalClientCertificate) == null)
					{
						flag2 = false;
					}
					else
					{
						flag2 = this.xobileTlsContext.IsRemoteCertificateAvailable;
					}
				}
				return flag2;
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				object obj = this.ioLock;
				lock (obj)
				{
					this.SetException(new ObjectDisposedException("MobileAuthenticatedStream"));
					if (this.xobileTlsContext != null)
					{
						this.xobileTlsContext.Dispose();
						this.xobileTlsContext = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void Flush()
		{
			base.InnerStream.Flush();
		}

		public SslProtocols SslProtocol
		{
			get
			{
				object obj = this.ioLock;
				SslProtocols negotiatedProtocol;
				lock (obj)
				{
					this.CheckThrow(true, false);
					negotiatedProtocol = (SslProtocols)this.xobileTlsContext.NegotiatedProtocol;
				}
				return negotiatedProtocol;
			}
		}

		public X509Certificate RemoteCertificate
		{
			get
			{
				object obj = this.ioLock;
				X509Certificate remoteCertificate;
				lock (obj)
				{
					this.CheckThrow(true, false);
					remoteCertificate = this.xobileTlsContext.RemoteCertificate;
				}
				return remoteCertificate;
			}
		}

		public X509Certificate LocalCertificate
		{
			get
			{
				object obj = this.ioLock;
				X509Certificate internalLocalCertificate;
				lock (obj)
				{
					this.CheckThrow(true, false);
					internalLocalCertificate = this.InternalLocalCertificate;
				}
				return internalLocalCertificate;
			}
		}

		public X509Certificate InternalLocalCertificate
		{
			get
			{
				object obj = this.ioLock;
				X509Certificate x509Certificate;
				lock (obj)
				{
					this.CheckThrow(false, false);
					if (this.xobileTlsContext == null)
					{
						x509Certificate = null;
					}
					else
					{
						x509Certificate = (this.xobileTlsContext.IsServer ? this.xobileTlsContext.LocalServerCertificate : this.xobileTlsContext.LocalClientCertificate);
					}
				}
				return x509Certificate;
			}
		}

		public MonoTlsConnectionInfo GetConnectionInfo()
		{
			object obj = this.ioLock;
			MonoTlsConnectionInfo connectionInfo;
			lock (obj)
			{
				this.CheckThrow(true, false);
				connectionInfo = this.xobileTlsContext.ConnectionInfo;
			}
			return connectionInfo;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			base.InnerStream.SetLength(value);
		}

		public TransportContext TransportContext
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.IsAuthenticated && base.InnerStream.CanRead;
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
				return (this.IsAuthenticated & base.InnerStream.CanWrite) && !this.shutdown;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
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
				throw new NotSupportedException();
			}
		}

		public override bool IsEncrypted
		{
			get
			{
				return this.IsAuthenticated;
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

		public global::System.Security.Authentication.CipherAlgorithmType CipherAlgorithm
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return global::System.Security.Authentication.CipherAlgorithmType.None;
				}
				switch (connectionInfo.CipherAlgorithmType)
				{
				case Mono.Security.Interface.CipherAlgorithmType.Aes128:
				case Mono.Security.Interface.CipherAlgorithmType.AesGcm128:
					return global::System.Security.Authentication.CipherAlgorithmType.Aes128;
				case Mono.Security.Interface.CipherAlgorithmType.Aes256:
				case Mono.Security.Interface.CipherAlgorithmType.AesGcm256:
					return global::System.Security.Authentication.CipherAlgorithmType.Aes256;
				default:
					return global::System.Security.Authentication.CipherAlgorithmType.None;
				}
			}
		}

		public global::System.Security.Authentication.HashAlgorithmType HashAlgorithm
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return global::System.Security.Authentication.HashAlgorithmType.None;
				}
				Mono.Security.Interface.HashAlgorithmType hashAlgorithmType = connectionInfo.HashAlgorithmType;
				if (hashAlgorithmType != Mono.Security.Interface.HashAlgorithmType.Md5)
				{
					if (hashAlgorithmType - Mono.Security.Interface.HashAlgorithmType.Sha1 <= 4)
					{
						return global::System.Security.Authentication.HashAlgorithmType.Sha1;
					}
					if (hashAlgorithmType != Mono.Security.Interface.HashAlgorithmType.Md5Sha1)
					{
						return global::System.Security.Authentication.HashAlgorithmType.None;
					}
				}
				return global::System.Security.Authentication.HashAlgorithmType.Md5;
			}
		}

		public global::System.Security.Authentication.ExchangeAlgorithmType KeyExchangeAlgorithm
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return global::System.Security.Authentication.ExchangeAlgorithmType.None;
				}
				switch (connectionInfo.ExchangeAlgorithmType)
				{
				case Mono.Security.Interface.ExchangeAlgorithmType.Dhe:
				case Mono.Security.Interface.ExchangeAlgorithmType.EcDhe:
					return global::System.Security.Authentication.ExchangeAlgorithmType.DiffieHellman;
				case Mono.Security.Interface.ExchangeAlgorithmType.Rsa:
					return global::System.Security.Authentication.ExchangeAlgorithmType.RsaSign;
				default:
					return global::System.Security.Authentication.ExchangeAlgorithmType.None;
				}
			}
		}

		public int CipherStrength
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return 0;
				}
				switch (connectionInfo.CipherAlgorithmType)
				{
				case Mono.Security.Interface.CipherAlgorithmType.None:
				case Mono.Security.Interface.CipherAlgorithmType.Aes128:
				case Mono.Security.Interface.CipherAlgorithmType.AesGcm128:
					return 128;
				case Mono.Security.Interface.CipherAlgorithmType.Aes256:
				case Mono.Security.Interface.CipherAlgorithmType.AesGcm256:
					return 256;
				default:
					throw new ArgumentOutOfRangeException("CipherAlgorithmType");
				}
			}
		}

		public int HashStrength
		{
			get
			{
				this.CheckThrow(true, false);
				MonoTlsConnectionInfo connectionInfo = this.GetConnectionInfo();
				if (connectionInfo == null)
				{
					return 0;
				}
				Mono.Security.Interface.HashAlgorithmType hashAlgorithmType = connectionInfo.HashAlgorithmType;
				switch (hashAlgorithmType)
				{
				case Mono.Security.Interface.HashAlgorithmType.Md5:
					break;
				case Mono.Security.Interface.HashAlgorithmType.Sha1:
					return 160;
				case Mono.Security.Interface.HashAlgorithmType.Sha224:
					return 224;
				case Mono.Security.Interface.HashAlgorithmType.Sha256:
					return 256;
				case Mono.Security.Interface.HashAlgorithmType.Sha384:
					return 384;
				case Mono.Security.Interface.HashAlgorithmType.Sha512:
					return 512;
				default:
					if (hashAlgorithmType != Mono.Security.Interface.HashAlgorithmType.Md5Sha1)
					{
						throw new ArgumentOutOfRangeException("HashAlgorithmType");
					}
					break;
				}
				return 128;
			}
		}

		public int KeyExchangeStrength
		{
			get
			{
				return 0;
			}
		}

		public bool CheckCertRevocationStatus
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private MobileTlsContext xobileTlsContext;

		private ExceptionDispatchInfo lastException;

		private AsyncProtocolRequest asyncHandshakeRequest;

		private AsyncProtocolRequest asyncReadRequest;

		private AsyncProtocolRequest asyncWriteRequest;

		private BufferOffsetSize2 readBuffer;

		private BufferOffsetSize2 writeBuffer;

		private object ioLock = new object();

		private int closeRequested;

		private bool shutdown;

		private MobileAuthenticatedStream.Operation operation;

		private static int uniqueNameInteger = 123;

		private static int nextId;

		internal readonly int ID = ++MobileAuthenticatedStream.nextId;

		private enum Operation
		{
			None,
			Handshake,
			Authenticated,
			Renegotiate,
			Read,
			Write,
			Close
		}

		private enum OperationType
		{
			Read,
			Write,
			Renegotiate,
			Shutdown
		}
	}
}
