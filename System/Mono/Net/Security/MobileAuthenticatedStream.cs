using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
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
		public MobileAuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen, SslStream owner, MonoTlsSettings settings, MonoTlsProvider provider)
			: base(innerStream, leaveInnerStreamOpen)
		{
			this.SslStream = owner;
			this.Settings = settings;
			this.Provider = provider;
			this.readBuffer = new BufferOffsetSize2(16834);
			this.writeBuffer = new BufferOffsetSize2(16384);
		}

		public SslStream SslStream { get; }

		public MonoTlsSettings Settings { get; }

		public MonoTlsProvider Provider { get; }

		internal bool HasContext
		{
			get
			{
				return this.xobileTlsContext != null;
			}
		}

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
			if (e is OperationCanceledException || e is IOException || e is ObjectDisposedException || e is AuthenticationException)
			{
				return e;
			}
			return new AuthenticationException("A call to SSPI failed, see inner exception.", e);
		}

		internal static Exception GetIOException(Exception e, string message)
		{
			if (e is OperationCanceledException || e is IOException || e is ObjectDisposedException || e is AuthenticationException)
			{
				return e;
			}
			return new IOException(message, e);
		}

		internal ExceptionDispatchInfo SetException(Exception e)
		{
			ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
			return Interlocked.CompareExchange<ExceptionDispatchInfo>(ref this.lastException, exceptionDispatchInfo, null) ?? exceptionDispatchInfo;
		}

		private SslProtocols DefaultProtocols
		{
			get
			{
				return SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
			}
		}

		public void AuthenticateAsClient(string targetHost)
		{
			this.AuthenticateAsClient(targetHost, new X509CertificateCollection(), this.DefaultProtocols, false);
		}

		public void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			this.ProcessAuthentication(true, false, targetHost, enabledSslProtocols, null, clientCertificates, false).Wait();
		}

		public IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginAuthenticateAsClient(targetHost, new X509CertificateCollection(), this.DefaultProtocols, false, asyncCallback, asyncState);
		}

		public IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.ProcessAuthentication(false, false, targetHost, enabledSslProtocols, null, clientCertificates, false), asyncCallback, asyncState);
		}

		public void EndAuthenticateAsClient(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		public void AuthenticateAsServer(X509Certificate serverCertificate)
		{
			this.AuthenticateAsServer(serverCertificate, false, this.DefaultProtocols, false);
		}

		public void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			this.ProcessAuthentication(true, true, string.Empty, enabledSslProtocols, serverCertificate, null, clientCertificateRequired).Wait();
		}

		public IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginAuthenticateAsServer(serverCertificate, false, this.DefaultProtocols, false, asyncCallback, asyncState);
		}

		public IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.ProcessAuthentication(false, true, string.Empty, enabledSslProtocols, serverCertificate, null, clientCertificateRequired), asyncCallback, asyncState);
		}

		public void EndAuthenticateAsServer(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		public Task AuthenticateAsClientAsync(string targetHost)
		{
			return this.ProcessAuthentication(false, false, targetHost, this.DefaultProtocols, null, null, false);
		}

		public Task AuthenticateAsClientAsync(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			return this.ProcessAuthentication(false, false, targetHost, enabledSslProtocols, null, clientCertificates, false);
		}

		public Task AuthenticateAsServerAsync(X509Certificate serverCertificate)
		{
			return this.AuthenticateAsServerAsync(serverCertificate, false, this.DefaultProtocols, false);
		}

		public Task AuthenticateAsServerAsync(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			return this.ProcessAuthentication(false, true, string.Empty, enabledSslProtocols, serverCertificate, null, clientCertificateRequired);
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

		private async Task ProcessAuthentication(bool runSynchronously, bool serverMode, string targetHost, SslProtocols enabledProtocols, X509Certificate serverCertificate, X509CertificateCollection clientCertificates, bool clientCertRequired)
		{
			if (serverMode)
			{
				if (serverCertificate == null)
				{
					throw new ArgumentException("serverCertificate");
				}
			}
			else
			{
				if (targetHost == null)
				{
					throw new ArgumentException("targetHost");
				}
				if (targetHost.Length == 0)
				{
					targetHost = "?" + Interlocked.Increment(ref MobileAuthenticatedStream.uniqueNameInteger).ToString(NumberFormatInfo.InvariantInfo);
				}
			}
			if (this.lastException != null)
			{
				this.lastException.Throw();
			}
			AsyncHandshakeRequest asyncHandshakeRequest = new AsyncHandshakeRequest(this, runSynchronously);
			if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncHandshakeRequest, asyncHandshakeRequest, null) != null)
			{
				throw new InvalidOperationException("Invalid nested call.");
			}
			if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncReadRequest, asyncHandshakeRequest, null) != null)
			{
				throw new InvalidOperationException("Invalid nested call.");
			}
			if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncWriteRequest, asyncHandshakeRequest, null) != null)
			{
				throw new InvalidOperationException("Invalid nested call.");
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
					this.xobileTlsContext = this.CreateContext(serverMode, targetHost, enabledProtocols, serverCertificate, clientCertificates, clientCertRequired);
				}
				try
				{
					asyncProtocolResult = await asyncHandshakeRequest.StartOperation(CancellationToken.None).ConfigureAwait(false);
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

		protected abstract MobileTlsContext CreateContext(bool serverMode, string targetHost, SslProtocols enabledProtocols, X509Certificate serverCertificate, X509CertificateCollection clientCertificates, bool askForClientCert);

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			AsyncReadRequest asyncReadRequest = new AsyncReadRequest(this, false, buffer, offset, count);
			return TaskToApm.Begin(this.StartOperation(MobileAuthenticatedStream.OperationType.Read, asyncReadRequest, CancellationToken.None), asyncCallback, asyncState);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			AsyncWriteRequest asyncWriteRequest = new AsyncWriteRequest(this, false, buffer, offset, count);
			return TaskToApm.Begin(this.StartOperation(MobileAuthenticatedStream.OperationType.Write, asyncWriteRequest, CancellationToken.None), asyncCallback, asyncState);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			AsyncReadRequest asyncReadRequest = new AsyncReadRequest(this, true, buffer, offset, count);
			return this.StartOperation(MobileAuthenticatedStream.OperationType.Read, asyncReadRequest, CancellationToken.None).Result;
		}

		public void Write(byte[] buffer)
		{
			this.Write(buffer, 0, buffer.Length);
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

		private async Task<int> StartOperation(MobileAuthenticatedStream.OperationType type, AsyncProtocolRequest asyncRequest, CancellationToken cancellationToken)
		{
			this.CheckThrow(true, type > MobileAuthenticatedStream.OperationType.Read);
			if (type == MobileAuthenticatedStream.OperationType.Read)
			{
				if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncReadRequest, asyncRequest, null) != null)
				{
					throw new InvalidOperationException("Invalid nested call.");
				}
			}
			else if (Interlocked.CompareExchange<AsyncProtocolRequest>(ref this.asyncWriteRequest, asyncRequest, null) != null)
			{
				throw new InvalidOperationException("Invalid nested call.");
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
		protected internal void Debug(string message, params object[] args)
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
				AsyncProtocolRequest asyncProtocolRequest = this.asyncHandshakeRequest ?? this.asyncWriteRequest;
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

		internal AsyncOperationStatus ProcessHandshake(AsyncOperationStatus status)
		{
			object obj = this.ioLock;
			AsyncOperationStatus asyncOperationStatus;
			lock (obj)
			{
				if (status == AsyncOperationStatus.Initialize)
				{
					this.xobileTlsContext.StartHandshake();
					asyncOperationStatus = AsyncOperationStatus.Continue;
				}
				else
				{
					if (status == AsyncOperationStatus.ReadDone)
					{
						throw new IOException("Authentication failed because the remote party has closed the transport stream.");
					}
					if (status != AsyncOperationStatus.Continue)
					{
						throw new InvalidOperationException();
					}
					AsyncOperationStatus asyncOperationStatus2 = AsyncOperationStatus.Continue;
					if (this.xobileTlsContext.ProcessHandshake())
					{
						this.xobileTlsContext.FinishHandshake();
						asyncOperationStatus2 = AsyncOperationStatus.Complete;
					}
					if (this.lastException != null)
					{
						this.lastException.Throw();
					}
					asyncOperationStatus = asyncOperationStatus2;
				}
			}
			return asyncOperationStatus;
		}

		internal ValueTuple<int, bool> ProcessRead(BufferOffsetSize userBuffer)
		{
			object obj = this.ioLock;
			ValueTuple<int, bool> valueTuple2;
			lock (obj)
			{
				ValueTuple<int, bool> valueTuple = this.xobileTlsContext.Read(userBuffer.Buffer, userBuffer.Offset, userBuffer.Size);
				if (this.lastException != null)
				{
					this.lastException.Throw();
				}
				valueTuple2 = valueTuple;
			}
			return valueTuple2;
		}

		internal ValueTuple<int, bool> ProcessWrite(BufferOffsetSize userBuffer)
		{
			object obj = this.ioLock;
			ValueTuple<int, bool> valueTuple2;
			lock (obj)
			{
				ValueTuple<int, bool> valueTuple = this.xobileTlsContext.Write(userBuffer.Buffer, userBuffer.Offset, userBuffer.Size);
				if (this.lastException != null)
				{
					this.lastException.Throw();
				}
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
				this.xobileTlsContext.Shutdown();
				this.shutdown = true;
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
					this.lastException = ExceptionDispatchInfo.Capture(new ObjectDisposedException("MobileAuthenticatedStream"));
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
				throw new NotImplementedException();
			}
		}

		public int HashStrength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int KeyExchangeStrength
		{
			get
			{
				throw new NotImplementedException();
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

		private static int uniqueNameInteger = 123;

		private static int nextId;

		internal readonly int ID = ++MobileAuthenticatedStream.nextId;

		private enum OperationType
		{
			Read,
			Write,
			Shutdown
		}
	}
}
