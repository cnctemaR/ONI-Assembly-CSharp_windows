using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace System.Data.SqlClient.SNI
{
	internal class SNINpHandle : SNIHandle
	{
		public SNINpHandle(string serverName, string pipeName, long timerExpire, object callbackObject)
		{
			this._targetServer = serverName;
			this._callbackObject = callbackObject;
			try
			{
				this._pipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.WriteThrough | PipeOptions.Asynchronous);
				if (9223372036854775807L == timerExpire)
				{
					this._pipeStream.Connect(-1);
				}
				else
				{
					TimeSpan timeSpan = DateTime.FromFileTime(timerExpire) - DateTime.Now;
					timeSpan = ((timeSpan.Ticks < 0L) ? TimeSpan.FromTicks(0L) : timeSpan);
					this._pipeStream.Connect((int)timeSpan.TotalMilliseconds);
				}
			}
			catch (TimeoutException ex)
			{
				SNICommon.ReportSNIError(SNIProviders.NP_PROV, 40U, ex);
				this._status = 1U;
				return;
			}
			catch (IOException ex2)
			{
				SNICommon.ReportSNIError(SNIProviders.NP_PROV, 40U, ex2);
				this._status = 1U;
				return;
			}
			if (!this._pipeStream.IsConnected || !this._pipeStream.CanWrite || !this._pipeStream.CanRead)
			{
				SNICommon.ReportSNIError(SNIProviders.NP_PROV, 0U, 40U, string.Empty);
				this._status = 1U;
				return;
			}
			this._sslOverTdsStream = new SslOverTdsStream(this._pipeStream);
			this._sslStream = new SslStream(this._sslOverTdsStream, true, new RemoteCertificateValidationCallback(this.ValidateServerCertificate), null);
			this._stream = this._pipeStream;
			this._status = 0U;
		}

		public override Guid ConnectionId
		{
			get
			{
				return this._connectionId;
			}
		}

		public override uint Status
		{
			get
			{
				return this._status;
			}
		}

		public override uint CheckConnection()
		{
			if (!this._stream.CanWrite || !this._stream.CanRead)
			{
				return 1U;
			}
			return 0U;
		}

		public override void Dispose()
		{
			lock (this)
			{
				if (this._sslOverTdsStream != null)
				{
					this._sslOverTdsStream.Dispose();
					this._sslOverTdsStream = null;
				}
				if (this._sslStream != null)
				{
					this._sslStream.Dispose();
					this._sslStream = null;
				}
				if (this._pipeStream != null)
				{
					this._pipeStream.Dispose();
					this._pipeStream = null;
				}
				this._stream = null;
			}
		}

		public override uint Receive(out SNIPacket packet, int timeout)
		{
			uint num;
			lock (this)
			{
				packet = null;
				try
				{
					packet = new SNIPacket(this._bufferSize);
					packet.ReadFromStream(this._stream);
					if (packet.Length == 0)
					{
						Win32Exception ex = new Win32Exception();
						return this.ReportErrorAndReleasePacket(packet, (uint)ex.NativeErrorCode, 0U, ex.Message);
					}
				}
				catch (ObjectDisposedException ex2)
				{
					return this.ReportErrorAndReleasePacket(packet, ex2);
				}
				catch (IOException ex3)
				{
					return this.ReportErrorAndReleasePacket(packet, ex3);
				}
				num = 0U;
			}
			return num;
		}

		public override uint ReceiveAsync(ref SNIPacket packet)
		{
			packet = new SNIPacket(this._bufferSize);
			uint num;
			try
			{
				packet.ReadFromStreamAsync(this._stream, this._receiveCallback);
				num = 997U;
			}
			catch (ObjectDisposedException ex)
			{
				num = this.ReportErrorAndReleasePacket(packet, ex);
			}
			catch (IOException ex2)
			{
				num = this.ReportErrorAndReleasePacket(packet, ex2);
			}
			return num;
		}

		public override uint Send(SNIPacket packet)
		{
			uint num;
			lock (this)
			{
				try
				{
					packet.WriteToStream(this._stream);
					num = 0U;
				}
				catch (ObjectDisposedException ex)
				{
					num = this.ReportErrorAndReleasePacket(packet, ex);
				}
				catch (IOException ex2)
				{
					num = this.ReportErrorAndReleasePacket(packet, ex2);
				}
			}
			return num;
		}

		public override uint SendAsync(SNIPacket packet, bool disposePacketAfterSendAsync, SNIAsyncCallback callback = null)
		{
			SNIAsyncCallback sniasyncCallback = callback ?? this._sendCallback;
			packet.WriteToStreamAsync(this._stream, sniasyncCallback, SNIProviders.NP_PROV, disposePacketAfterSendAsync);
			return 997U;
		}

		public override void SetAsyncCallbacks(SNIAsyncCallback receiveCallback, SNIAsyncCallback sendCallback)
		{
			this._receiveCallback = receiveCallback;
			this._sendCallback = sendCallback;
		}

		public override uint EnableSsl(uint options)
		{
			this._validateCert = (options & 1U) > 0U;
			try
			{
				this._sslStream.AuthenticateAsClientAsync(this._targetServer).GetAwaiter().GetResult();
				this._sslOverTdsStream.FinishHandshake();
			}
			catch (AuthenticationException ex)
			{
				return SNICommon.ReportSNIError(SNIProviders.NP_PROV, 35U, ex);
			}
			catch (InvalidOperationException ex2)
			{
				return SNICommon.ReportSNIError(SNIProviders.NP_PROV, 35U, ex2);
			}
			this._stream = this._sslStream;
			return 0U;
		}

		public override void DisableSsl()
		{
			this._sslStream.Dispose();
			this._sslStream = null;
			this._sslOverTdsStream.Dispose();
			this._sslOverTdsStream = null;
			this._stream = this._pipeStream;
		}

		private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return !this._validateCert || SNICommon.ValidateSslServerCertificate(this._targetServer, sender, cert, chain, policyErrors);
		}

		public override void SetBufferSize(int bufferSize)
		{
			this._bufferSize = bufferSize;
		}

		private uint ReportErrorAndReleasePacket(SNIPacket packet, Exception sniException)
		{
			if (packet != null)
			{
				packet.Release();
			}
			return SNICommon.ReportSNIError(SNIProviders.NP_PROV, 35U, sniException);
		}

		private uint ReportErrorAndReleasePacket(SNIPacket packet, uint nativeError, uint sniError, string errorMessage)
		{
			if (packet != null)
			{
				packet.Release();
			}
			return SNICommon.ReportSNIError(SNIProviders.NP_PROV, nativeError, sniError, errorMessage);
		}

		internal const string DefaultPipePath = "sql\\query";

		private const int MAX_PIPE_INSTANCES = 255;

		private readonly string _targetServer;

		private readonly object _callbackObject;

		private Stream _stream;

		private NamedPipeClientStream _pipeStream;

		private SslOverTdsStream _sslOverTdsStream;

		private SslStream _sslStream;

		private SNIAsyncCallback _receiveCallback;

		private SNIAsyncCallback _sendCallback;

		private bool _validateCert = true;

		private readonly uint _status = uint.MaxValue;

		private int _bufferSize = 4096;

		private readonly Guid _connectionId = Guid.NewGuid();
	}
}
