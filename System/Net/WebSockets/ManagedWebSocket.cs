using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
	internal sealed class ManagedWebSocket : WebSocket
	{
		public static ManagedWebSocket CreateFromConnectedStream(Stream stream, bool isServer, string subprotocol, TimeSpan keepAliveInterval, int receiveBufferSize, ArraySegment<byte>? receiveBuffer = null)
		{
			return new ManagedWebSocket(stream, isServer, subprotocol, keepAliveInterval, receiveBufferSize, receiveBuffer);
		}

		private object StateUpdateLock
		{
			get
			{
				return this._abortSource;
			}
		}

		private object ReceiveAsyncLock
		{
			get
			{
				return this._utf8TextState;
			}
		}

		private ManagedWebSocket(Stream stream, bool isServer, string subprotocol, TimeSpan keepAliveInterval, int receiveBufferSize, ArraySegment<byte>? receiveBuffer)
		{
			this._stream = stream;
			this._isServer = isServer;
			this._subprotocol = subprotocol;
			if (receiveBuffer != null && receiveBuffer.GetValueOrDefault().Array != null && receiveBuffer.GetValueOrDefault().Offset == 0 && receiveBuffer.GetValueOrDefault().Count == receiveBuffer.GetValueOrDefault().Array.Length && receiveBuffer.GetValueOrDefault().Count >= 14)
			{
				this._receiveBuffer = receiveBuffer.Value.Array;
			}
			else
			{
				this._receiveBufferFromPool = true;
				this._receiveBuffer = ArrayPool<byte>.Shared.Rent(Math.Max(receiveBufferSize, 14));
			}
			this._abortSource.Token.Register(delegate(object s)
			{
				ManagedWebSocket managedWebSocket = (ManagedWebSocket)s;
				object stateUpdateLock = managedWebSocket.StateUpdateLock;
				lock (stateUpdateLock)
				{
					WebSocketState state = managedWebSocket._state;
					if (state != WebSocketState.Closed && state != WebSocketState.Aborted)
					{
						managedWebSocket._state = ((state != WebSocketState.None && state != WebSocketState.Connecting) ? WebSocketState.Aborted : WebSocketState.Closed);
					}
				}
			}, this);
			if (keepAliveInterval > TimeSpan.Zero)
			{
				this._keepAliveTimer = new Timer(delegate(object s)
				{
					((ManagedWebSocket)s).SendKeepAliveFrameAsync();
				}, this, keepAliveInterval, keepAliveInterval);
			}
		}

		public override void Dispose()
		{
			object stateUpdateLock = this.StateUpdateLock;
			lock (stateUpdateLock)
			{
				this.DisposeCore();
			}
		}

		private void DisposeCore()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				Timer keepAliveTimer = this._keepAliveTimer;
				if (keepAliveTimer != null)
				{
					keepAliveTimer.Dispose();
				}
				Stream stream = this._stream;
				if (stream != null)
				{
					stream.Dispose();
				}
				if (this._receiveBufferFromPool)
				{
					byte[] receiveBuffer = this._receiveBuffer;
					this._receiveBuffer = null;
					ArrayPool<byte>.Shared.Return(receiveBuffer, false);
				}
				if (this._state < WebSocketState.Aborted)
				{
					this._state = WebSocketState.Closed;
				}
			}
		}

		public override WebSocketCloseStatus? CloseStatus
		{
			get
			{
				return this._closeStatus;
			}
		}

		public override string CloseStatusDescription
		{
			get
			{
				return this._closeStatusDescription;
			}
		}

		public override WebSocketState State
		{
			get
			{
				return this._state;
			}
		}

		public override string SubProtocol
		{
			get
			{
				return this._subprotocol;
			}
		}

		public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			if (messageType != WebSocketMessageType.Text && messageType != WebSocketMessageType.Binary)
			{
				throw new ArgumentException(global::SR.Format("The message type '{0}' is not allowed for the '{1}' operation. Valid message types are: '{2}, {3}'. To close the WebSocket, use the '{4}' operation instead. ", new object[] { "Close", "SendAsync", "Binary", "Text", "CloseOutputAsync" }), "messageType");
			}
			WebSocketValidate.ValidateArraySegment(buffer, "buffer");
			try
			{
				WebSocketValidate.ThrowIfInvalidState(this._state, this._disposed, ManagedWebSocket.s_validSendStates);
				this.ThrowIfOperationInProgress(this._lastSendAsync, "SendAsync");
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
			ManagedWebSocket.MessageOpcode messageOpcode = (this._lastSendWasFragment ? ManagedWebSocket.MessageOpcode.Continuation : ((messageType == WebSocketMessageType.Binary) ? ManagedWebSocket.MessageOpcode.Binary : ManagedWebSocket.MessageOpcode.Text));
			Task task = this.SendFrameAsync(messageOpcode, endOfMessage, buffer, cancellationToken);
			this._lastSendWasFragment = !endOfMessage;
			this._lastSendAsync = task;
			return task;
		}

		public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			WebSocketValidate.ValidateArraySegment(buffer, "buffer");
			Task<WebSocketReceiveResult> task2;
			try
			{
				WebSocketValidate.ThrowIfInvalidState(this._state, this._disposed, ManagedWebSocket.s_validReceiveStates);
				object receiveAsyncLock = this.ReceiveAsyncLock;
				lock (receiveAsyncLock)
				{
					this.ThrowIfOperationInProgress(this._lastReceiveAsync, "ReceiveAsync");
					Task<WebSocketReceiveResult> task = this.ReceiveAsyncPrivate(buffer, cancellationToken);
					this._lastReceiveAsync = task;
					task2 = task;
				}
			}
			catch (Exception ex)
			{
				task2 = Task.FromException<WebSocketReceiveResult>(ex);
			}
			return task2;
		}

		public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			WebSocketValidate.ValidateCloseStatus(closeStatus, statusDescription);
			try
			{
				WebSocketValidate.ThrowIfInvalidState(this._state, this._disposed, ManagedWebSocket.s_validCloseStates);
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
			return this.CloseAsyncPrivate(closeStatus, statusDescription, cancellationToken);
		}

		public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			WebSocketValidate.ValidateCloseStatus(closeStatus, statusDescription);
			try
			{
				WebSocketValidate.ThrowIfInvalidState(this._state, this._disposed, ManagedWebSocket.s_validCloseOutputStates);
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
			return this.SendCloseFrameAsync(closeStatus, statusDescription, cancellationToken);
		}

		public override void Abort()
		{
			this._abortSource.Cancel();
			this.Dispose();
		}

		private Task SendFrameAsync(ManagedWebSocket.MessageOpcode opcode, bool endOfMessage, ArraySegment<byte> payloadBuffer, CancellationToken cancellationToken)
		{
			if (!cancellationToken.CanBeCanceled && this._sendFrameAsyncLock.Wait(0))
			{
				return this.SendFrameLockAcquiredNonCancelableAsync(opcode, endOfMessage, payloadBuffer);
			}
			return this.SendFrameFallbackAsync(opcode, endOfMessage, payloadBuffer, cancellationToken);
		}

		private Task SendFrameLockAcquiredNonCancelableAsync(ManagedWebSocket.MessageOpcode opcode, bool endOfMessage, ArraySegment<byte> payloadBuffer)
		{
			Task task = null;
			bool flag = true;
			try
			{
				int num = this.WriteFrameToSendBuffer(opcode, endOfMessage, payloadBuffer);
				task = this._stream.WriteAsync(this._sendBuffer, 0, num, CancellationToken.None);
				if (task.IsCompleted)
				{
					return task;
				}
				flag = false;
			}
			catch (Exception ex)
			{
				return Task.FromException((this._state == WebSocketState.Aborted) ? ManagedWebSocket.CreateOperationCanceledException(ex, default(CancellationToken)) : new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex));
			}
			finally
			{
				if (flag)
				{
					this._sendFrameAsyncLock.Release();
					this.ReleaseSendBuffer();
				}
			}
			return task.ContinueWith(delegate(Task t, object s)
			{
				ManagedWebSocket managedWebSocket = (ManagedWebSocket)s;
				managedWebSocket._sendFrameAsyncLock.Release();
				managedWebSocket.ReleaseSendBuffer();
				try
				{
					t.GetAwaiter().GetResult();
				}
				catch (Exception ex2)
				{
					throw (managedWebSocket._state == WebSocketState.Aborted) ? ManagedWebSocket.CreateOperationCanceledException(ex2, default(CancellationToken)) : new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex2);
				}
			}, this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}

		private async Task SendFrameFallbackAsync(ManagedWebSocket.MessageOpcode opcode, bool endOfMessage, ArraySegment<byte> payloadBuffer, CancellationToken cancellationToken)
		{
			await this._sendFrameAsyncLock.WaitAsync().ConfigureAwait(false);
			try
			{
				int num = this.WriteFrameToSendBuffer(opcode, endOfMessage, payloadBuffer);
				using (cancellationToken.Register(delegate(object s)
				{
					((ManagedWebSocket)s).Abort();
				}, this))
				{
					await this._stream.WriteAsync(this._sendBuffer, 0, num, cancellationToken).ConfigureAwait(false);
				}
				CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
			}
			catch (Exception ex)
			{
				throw (this._state == WebSocketState.Aborted) ? ManagedWebSocket.CreateOperationCanceledException(ex, cancellationToken) : new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex);
			}
			finally
			{
				this._sendFrameAsyncLock.Release();
				this.ReleaseSendBuffer();
			}
		}

		private int WriteFrameToSendBuffer(ManagedWebSocket.MessageOpcode opcode, bool endOfMessage, ArraySegment<byte> payloadBuffer)
		{
			this.AllocateSendBuffer(payloadBuffer.Count + 14);
			int? num = null;
			int num2;
			if (this._isServer)
			{
				num2 = ManagedWebSocket.WriteHeader(opcode, this._sendBuffer, payloadBuffer, endOfMessage, false);
			}
			else
			{
				num = new int?(ManagedWebSocket.WriteHeader(opcode, this._sendBuffer, payloadBuffer, endOfMessage, true));
				num2 = num.GetValueOrDefault() + 4;
			}
			if (payloadBuffer.Count > 0)
			{
				Buffer.BlockCopy(payloadBuffer.Array, payloadBuffer.Offset, this._sendBuffer, num2, payloadBuffer.Count);
				if (num != null)
				{
					ManagedWebSocket.ApplyMask(this._sendBuffer, num2, this._sendBuffer, num.Value, 0, (long)payloadBuffer.Count);
				}
			}
			return num2 + payloadBuffer.Count;
		}

		private void SendKeepAliveFrameAsync()
		{
			if (this._sendFrameAsyncLock.Wait(0))
			{
				Task task = this.SendFrameLockAcquiredNonCancelableAsync(ManagedWebSocket.MessageOpcode.Ping, true, new ArraySegment<byte>(Array.Empty<byte>()));
				if (!task.IsCompletedSuccessfully)
				{
					task.ContinueWith(delegate(Task p)
					{
						AggregateException exception = p.Exception;
					}, CancellationToken.None, TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
				}
			}
		}

		private static int WriteHeader(ManagedWebSocket.MessageOpcode opcode, byte[] sendBuffer, ArraySegment<byte> payload, bool endOfMessage, bool useMask)
		{
			sendBuffer[0] = (byte)opcode;
			if (endOfMessage)
			{
				int num = 0;
				sendBuffer[num] |= 128;
			}
			int num2;
			if (payload.Count <= 125)
			{
				sendBuffer[1] = (byte)payload.Count;
				num2 = 2;
			}
			else if (payload.Count <= 65535)
			{
				sendBuffer[1] = 126;
				sendBuffer[2] = (byte)(payload.Count / 256);
				sendBuffer[3] = (byte)payload.Count;
				num2 = 4;
			}
			else
			{
				sendBuffer[1] = 127;
				int num3 = payload.Count;
				for (int i = 9; i >= 2; i--)
				{
					sendBuffer[i] = (byte)num3;
					num3 /= 256;
				}
				num2 = 10;
			}
			if (useMask)
			{
				int num4 = 1;
				sendBuffer[num4] |= 128;
				ManagedWebSocket.WriteRandomMask(sendBuffer, num2);
			}
			return num2;
		}

		private static void WriteRandomMask(byte[] buffer, int offset)
		{
			ManagedWebSocket.s_random.GetBytes(buffer, offset, 4);
		}

		private async Task<WebSocketReceiveResult> ReceiveAsyncPrivate(ArraySegment<byte> payloadBuffer, CancellationToken cancellationToken)
		{
			WebSocketReceiveResult webSocketReceiveResult;
			using (cancellationToken.Register(delegate(object s)
			{
				((ManagedWebSocket)s).Abort();
			}, this))
			{
				try
				{
					ManagedWebSocket.MessageHeader header;
					for (;;)
					{
						header = this._lastReceiveHeader;
						if (header.PayloadLength == 0L)
						{
							if (this._receiveBufferCount < (this._isServer ? 10 : 14))
							{
								if (this._receiveBufferCount < 2)
								{
									await this.EnsureBufferContainsAsync(2, cancellationToken, true).ConfigureAwait(false);
								}
								long num = (long)(this._receiveBuffer[this._receiveBufferOffset + 1] & 127);
								if (this._isServer || num > 125L)
								{
									await this.EnsureBufferContainsAsync(2 + (this._isServer ? 4 : 0) + ((num <= 125L) ? 0 : ((num == 126L) ? 2 : 8)), cancellationToken, true).ConfigureAwait(false);
								}
							}
							if (!this.TryParseMessageHeaderFromReceiveBuffer(out header))
							{
								await this.CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, cancellationToken, null).ConfigureAwait(false);
							}
							this._receivedMaskOffsetOffset = 0;
						}
						if (header.Opcode != ManagedWebSocket.MessageOpcode.Ping && header.Opcode != ManagedWebSocket.MessageOpcode.Pong)
						{
							break;
						}
						await this.HandleReceivedPingPongAsync(header, cancellationToken).ConfigureAwait(false);
					}
					if (header.Opcode == ManagedWebSocket.MessageOpcode.Close)
					{
						webSocketReceiveResult = await this.HandleReceivedCloseAsync(header, cancellationToken).ConfigureAwait(false);
					}
					else
					{
						if (header.Opcode == ManagedWebSocket.MessageOpcode.Continuation)
						{
							header.Opcode = this._lastReceiveHeader.Opcode;
						}
						int bytesToRead = (int)Math.Min((long)payloadBuffer.Count, header.PayloadLength);
						if (bytesToRead == 0)
						{
							this._lastReceiveHeader = header;
							webSocketReceiveResult = new WebSocketReceiveResult(0, (header.Opcode == ManagedWebSocket.MessageOpcode.Text) ? WebSocketMessageType.Text : WebSocketMessageType.Binary, header.PayloadLength == 0L && header.Fin);
						}
						else
						{
							if (this._receiveBufferCount == 0)
							{
								await this.EnsureBufferContainsAsync(1, cancellationToken, false).ConfigureAwait(false);
							}
							int bytesToCopy = Math.Min(bytesToRead, this._receiveBufferCount);
							if (this._isServer)
							{
								this._receivedMaskOffsetOffset = ManagedWebSocket.ApplyMask(this._receiveBuffer, this._receiveBufferOffset, header.Mask, this._receivedMaskOffsetOffset, (long)bytesToCopy);
							}
							Buffer.BlockCopy(this._receiveBuffer, this._receiveBufferOffset, payloadBuffer.Array, payloadBuffer.Offset, bytesToCopy);
							this.ConsumeFromBuffer(bytesToCopy);
							header.PayloadLength -= (long)bytesToCopy;
							if (header.Opcode == ManagedWebSocket.MessageOpcode.Text && !ManagedWebSocket.TryValidateUtf8(new ArraySegment<byte>(payloadBuffer.Array, payloadBuffer.Offset, bytesToCopy), header.Fin, this._utf8TextState))
							{
								await this.CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.InvalidPayloadData, WebSocketError.Faulted, cancellationToken, null).ConfigureAwait(false);
							}
							this._lastReceiveHeader = header;
							webSocketReceiveResult = new WebSocketReceiveResult(bytesToCopy, (header.Opcode == ManagedWebSocket.MessageOpcode.Text) ? WebSocketMessageType.Text : WebSocketMessageType.Binary, bytesToCopy == 0 || (header.Fin && header.PayloadLength == 0L));
						}
					}
				}
				catch (Exception ex)
				{
					if (this._state == WebSocketState.Aborted)
					{
						throw new OperationCanceledException("Aborted", ex);
					}
					throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex);
				}
			}
			return webSocketReceiveResult;
		}

		private async Task<WebSocketReceiveResult> HandleReceivedCloseAsync(ManagedWebSocket.MessageHeader header, CancellationToken cancellationToken)
		{
			object stateUpdateLock = this.StateUpdateLock;
			lock (stateUpdateLock)
			{
				this._receivedCloseFrame = true;
				if (this._state < WebSocketState.CloseReceived)
				{
					this._state = WebSocketState.CloseReceived;
				}
			}
			WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure;
			string closeStatusDescription = string.Empty;
			if (header.PayloadLength == 1L)
			{
				await this.CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, cancellationToken, null).ConfigureAwait(false);
			}
			else if (header.PayloadLength >= 2L)
			{
				if ((long)this._receiveBufferCount < header.PayloadLength)
				{
					await this.EnsureBufferContainsAsync((int)header.PayloadLength, cancellationToken, true).ConfigureAwait(false);
				}
				if (this._isServer)
				{
					ManagedWebSocket.ApplyMask(this._receiveBuffer, this._receiveBufferOffset, header.Mask, 0, header.PayloadLength);
				}
				closeStatus = (WebSocketCloseStatus)(((int)this._receiveBuffer[this._receiveBufferOffset] << 8) | (int)this._receiveBuffer[this._receiveBufferOffset + 1]);
				if (!ManagedWebSocket.IsValidCloseStatus(closeStatus))
				{
					await this.CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, cancellationToken, null).ConfigureAwait(false);
				}
				if (header.PayloadLength > 2L)
				{
					int num = 0;
					try
					{
						closeStatusDescription = ManagedWebSocket.s_textEncoding.GetString(this._receiveBuffer, this._receiveBufferOffset + 2, (int)header.PayloadLength - 2);
					}
					catch (DecoderFallbackException stateUpdateLock)
					{
						num = 1;
					}
					if (num == 1)
					{
						await this.CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, cancellationToken, (DecoderFallbackException)stateUpdateLock).ConfigureAwait(false);
					}
				}
				this.ConsumeFromBuffer((int)header.PayloadLength);
			}
			this._closeStatus = new WebSocketCloseStatus?(closeStatus);
			this._closeStatusDescription = closeStatusDescription;
			return new WebSocketReceiveResult(0, WebSocketMessageType.Close, true, new WebSocketCloseStatus?(closeStatus), closeStatusDescription);
		}

		private async Task HandleReceivedPingPongAsync(ManagedWebSocket.MessageHeader header, CancellationToken cancellationToken)
		{
			if (header.PayloadLength > 0L && (long)this._receiveBufferCount < header.PayloadLength)
			{
				await this.EnsureBufferContainsAsync((int)header.PayloadLength, cancellationToken, true).ConfigureAwait(false);
			}
			if (header.Opcode == ManagedWebSocket.MessageOpcode.Ping)
			{
				if (this._isServer)
				{
					ManagedWebSocket.ApplyMask(this._receiveBuffer, this._receiveBufferOffset, header.Mask, 0, header.PayloadLength);
				}
				await this.SendFrameAsync(ManagedWebSocket.MessageOpcode.Pong, true, new ArraySegment<byte>(this._receiveBuffer, this._receiveBufferOffset, (int)header.PayloadLength), cancellationToken).ConfigureAwait(false);
			}
			if (header.PayloadLength > 0L)
			{
				this.ConsumeFromBuffer((int)header.PayloadLength);
			}
		}

		private static bool IsValidCloseStatus(WebSocketCloseStatus closeStatus)
		{
			return closeStatus >= WebSocketCloseStatus.NormalClosure && closeStatus < (WebSocketCloseStatus)5000 && (closeStatus >= (WebSocketCloseStatus)3000 || (closeStatus - WebSocketCloseStatus.NormalClosure <= 3 || closeStatus - WebSocketCloseStatus.InvalidPayloadData <= 4));
		}

		private async Task CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus closeStatus, WebSocketError error, CancellationToken cancellationToken, Exception innerException = null)
		{
			if (!this._sentCloseFrame)
			{
				await this.CloseOutputAsync(closeStatus, string.Empty, cancellationToken).ConfigureAwait(false);
			}
			this._receiveBufferCount = 0;
			throw new WebSocketException(error, innerException);
		}

		private bool TryParseMessageHeaderFromReceiveBuffer(out ManagedWebSocket.MessageHeader resultHeader)
		{
			ManagedWebSocket.MessageHeader messageHeader = default(ManagedWebSocket.MessageHeader);
			messageHeader.Fin = (this._receiveBuffer[this._receiveBufferOffset] & 128) > 0;
			bool flag = (this._receiveBuffer[this._receiveBufferOffset] & 112) > 0;
			messageHeader.Opcode = (ManagedWebSocket.MessageOpcode)(this._receiveBuffer[this._receiveBufferOffset] & 15);
			bool flag2 = (this._receiveBuffer[this._receiveBufferOffset + 1] & 128) > 0;
			messageHeader.PayloadLength = (long)(this._receiveBuffer[this._receiveBufferOffset + 1] & 127);
			this.ConsumeFromBuffer(2);
			if (messageHeader.PayloadLength == 126L)
			{
				messageHeader.PayloadLength = (long)(((int)this._receiveBuffer[this._receiveBufferOffset] << 8) | (int)this._receiveBuffer[this._receiveBufferOffset + 1]);
				this.ConsumeFromBuffer(2);
			}
			else if (messageHeader.PayloadLength == 127L)
			{
				messageHeader.PayloadLength = 0L;
				for (int i = 0; i < 8; i++)
				{
					messageHeader.PayloadLength = (messageHeader.PayloadLength << 8) | (long)((ulong)this._receiveBuffer[this._receiveBufferOffset + i]);
				}
				this.ConsumeFromBuffer(8);
			}
			bool flag3 = flag;
			if (flag2)
			{
				if (!this._isServer)
				{
					flag3 = true;
				}
				messageHeader.Mask = ManagedWebSocket.CombineMaskBytes(this._receiveBuffer, this._receiveBufferOffset);
				this.ConsumeFromBuffer(4);
			}
			switch (messageHeader.Opcode)
			{
			case ManagedWebSocket.MessageOpcode.Continuation:
				if (this._lastReceiveHeader.Fin)
				{
					flag3 = true;
					goto IL_01B8;
				}
				goto IL_01B8;
			case ManagedWebSocket.MessageOpcode.Text:
			case ManagedWebSocket.MessageOpcode.Binary:
				if (!this._lastReceiveHeader.Fin)
				{
					flag3 = true;
					goto IL_01B8;
				}
				goto IL_01B8;
			case ManagedWebSocket.MessageOpcode.Close:
			case ManagedWebSocket.MessageOpcode.Ping:
			case ManagedWebSocket.MessageOpcode.Pong:
				if (messageHeader.PayloadLength > 125L || !messageHeader.Fin)
				{
					flag3 = true;
					goto IL_01B8;
				}
				goto IL_01B8;
			}
			flag3 = true;
			IL_01B8:
			resultHeader = messageHeader;
			return !flag3;
		}

		private async Task CloseAsyncPrivate(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			if (!this._sentCloseFrame)
			{
				await this.SendCloseFrameAsync(closeStatus, statusDescription, cancellationToken).ConfigureAwait(false);
			}
			byte[] closeBuffer = ArrayPool<byte>.Shared.Rent(139);
			object obj;
			try
			{
				while (!this._receivedCloseFrame)
				{
					obj = this.ReceiveAsyncLock;
					Task<WebSocketReceiveResult> task;
					lock (obj)
					{
						if (this._receivedCloseFrame)
						{
							break;
						}
						task = this._lastReceiveAsync;
						if (task == null || (task.Status == TaskStatus.RanToCompletion && task.Result.MessageType != WebSocketMessageType.Close))
						{
							task = (this._lastReceiveAsync = this.ReceiveAsyncPrivate(new ArraySegment<byte>(closeBuffer), cancellationToken));
						}
					}
					await task.ConfigureAwait(false);
				}
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(closeBuffer, false);
			}
			obj = this.StateUpdateLock;
			lock (obj)
			{
				this.DisposeCore();
				if (this._state < WebSocketState.Closed)
				{
					this._state = WebSocketState.Closed;
				}
			}
		}

		private async Task SendCloseFrameAsync(WebSocketCloseStatus closeStatus, string closeStatusDescription, CancellationToken cancellationToken)
		{
			byte[] buffer = null;
			try
			{
				int num = 2;
				if (string.IsNullOrEmpty(closeStatusDescription))
				{
					buffer = ArrayPool<byte>.Shared.Rent(num);
				}
				else
				{
					num += ManagedWebSocket.s_textEncoding.GetByteCount(closeStatusDescription);
					buffer = ArrayPool<byte>.Shared.Rent(num);
					ManagedWebSocket.s_textEncoding.GetBytes(closeStatusDescription, 0, closeStatusDescription.Length, buffer, 2);
				}
				ushort num2 = (ushort)closeStatus;
				buffer[0] = (byte)(num2 >> 8);
				buffer[1] = (byte)(num2 & 255);
				await this.SendFrameAsync(ManagedWebSocket.MessageOpcode.Close, true, new ArraySegment<byte>(buffer, 0, num), cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				if (buffer != null)
				{
					ArrayPool<byte>.Shared.Return(buffer, false);
				}
			}
			object stateUpdateLock = this.StateUpdateLock;
			lock (stateUpdateLock)
			{
				this._sentCloseFrame = true;
				if (this._state <= WebSocketState.CloseReceived)
				{
					this._state = WebSocketState.CloseSent;
				}
			}
		}

		private void ConsumeFromBuffer(int count)
		{
			this._receiveBufferCount -= count;
			this._receiveBufferOffset += count;
		}

		private async Task EnsureBufferContainsAsync(int minimumRequiredBytes, CancellationToken cancellationToken, bool throwOnPrematureClosure = true)
		{
			if (this._receiveBufferCount < minimumRequiredBytes)
			{
				if (this._receiveBufferCount > 0)
				{
					Buffer.BlockCopy(this._receiveBuffer, this._receiveBufferOffset, this._receiveBuffer, 0, this._receiveBufferCount);
				}
				this._receiveBufferOffset = 0;
				while (this._receiveBufferCount < minimumRequiredBytes)
				{
					int num = await this._stream.ReadAsync(this._receiveBuffer, this._receiveBufferCount, this._receiveBuffer.Length - this._receiveBufferCount, cancellationToken).ConfigureAwait(false);
					this._receiveBufferCount += num;
					if (num == 0)
					{
						if (this._disposed)
						{
							throw new ObjectDisposedException("WebSocket");
						}
						if (throwOnPrematureClosure)
						{
							throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
						}
						break;
					}
				}
			}
		}

		private void AllocateSendBuffer(int minLength)
		{
			this._sendBuffer = ArrayPool<byte>.Shared.Rent(minLength);
		}

		private void ReleaseSendBuffer()
		{
			byte[] sendBuffer = this._sendBuffer;
			if (sendBuffer != null)
			{
				this._sendBuffer = null;
				ArrayPool<byte>.Shared.Return(sendBuffer, false);
			}
		}

		private static int CombineMaskBytes(byte[] buffer, int maskOffset)
		{
			return BitConverter.ToInt32(buffer, maskOffset);
		}

		private static int ApplyMask(byte[] toMask, int toMaskOffset, byte[] mask, int maskOffset, int maskOffsetIndex, long count)
		{
			return ManagedWebSocket.ApplyMask(toMask, toMaskOffset, ManagedWebSocket.CombineMaskBytes(mask, maskOffset), maskOffsetIndex, count);
		}

		private unsafe static int ApplyMask(byte[] toMask, int toMaskOffset, int mask, int maskIndex, long count)
		{
			int num = maskIndex * 8;
			int num2 = (int)(((uint)mask >> num) | (uint)((uint)mask << 32 - num));
			if (count > 0L)
			{
				fixed (byte[] array = toMask)
				{
					byte* ptr;
					if (toMask == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					byte* ptr2 = ptr + toMaskOffset;
					if (ptr2 % 4L == null)
					{
						while (count >= 4L)
						{
							count -= 4L;
							*(int*)ptr2 ^= num2;
							ptr2 += 4;
						}
					}
					if (count > 0L)
					{
						byte* ptr3 = (byte*)(&mask);
						byte* ptr4 = ptr2 + count;
						while (ptr2 < ptr4)
						{
							byte* ptr5 = ptr2++;
							*ptr5 ^= ptr3[maskIndex];
							maskIndex = (maskIndex + 1) & 3;
						}
					}
				}
			}
			return maskIndex;
		}

		private void ThrowIfOperationInProgress(Task operationTask, [CallerMemberName] string methodName = null)
		{
			if (operationTask != null && !operationTask.IsCompleted)
			{
				this.Abort();
				throw new InvalidOperationException(global::SR.Format("There is already one outstanding '{0}' call for this WebSocket instance. ReceiveAsync and SendAsync can be called simultaneously, but at most one outstanding operation for each of them is allowed at the same time.", methodName));
			}
		}

		private static Exception CreateOperationCanceledException(Exception innerException, CancellationToken cancellationToken = default(CancellationToken))
		{
			return new OperationCanceledException(new OperationCanceledException().Message, innerException, cancellationToken);
		}

		private static bool TryValidateUtf8(ArraySegment<byte> arraySegment, bool endOfMessage, ManagedWebSocket.Utf8MessageState state)
		{
			int i = arraySegment.Offset;
			while (i < arraySegment.Offset + arraySegment.Count)
			{
				if (!state.SequenceInProgress)
				{
					state.SequenceInProgress = true;
					byte b = arraySegment.Array[i];
					i++;
					if ((b & 128) == 0)
					{
						state.AdditionalBytesExpected = 0;
						state.CurrentDecodeBits = (int)(b & 127);
						state.ExpectedValueMin = 0;
					}
					else
					{
						if ((b & 192) == 128)
						{
							return false;
						}
						if ((b & 224) == 192)
						{
							state.AdditionalBytesExpected = 1;
							state.CurrentDecodeBits = (int)(b & 31);
							state.ExpectedValueMin = 128;
						}
						else if ((b & 240) == 224)
						{
							state.AdditionalBytesExpected = 2;
							state.CurrentDecodeBits = (int)(b & 15);
							state.ExpectedValueMin = 2048;
						}
						else
						{
							if ((b & 248) != 240)
							{
								return false;
							}
							state.AdditionalBytesExpected = 3;
							state.CurrentDecodeBits = (int)(b & 7);
							state.ExpectedValueMin = 65536;
						}
					}
				}
				while (state.AdditionalBytesExpected > 0 && i < arraySegment.Offset + arraySegment.Count)
				{
					byte b2 = arraySegment.Array[i];
					if ((b2 & 192) != 128)
					{
						return false;
					}
					i++;
					state.AdditionalBytesExpected--;
					state.CurrentDecodeBits = (state.CurrentDecodeBits << 6) | (int)(b2 & 63);
					if (state.AdditionalBytesExpected == 1 && state.CurrentDecodeBits >= 864 && state.CurrentDecodeBits <= 895)
					{
						return false;
					}
					if (state.AdditionalBytesExpected == 2 && state.CurrentDecodeBits >= 272)
					{
						return false;
					}
				}
				if (state.AdditionalBytesExpected == 0)
				{
					state.SequenceInProgress = false;
					if (state.CurrentDecodeBits < state.ExpectedValueMin)
					{
						return false;
					}
				}
			}
			return !endOfMessage || !state.SequenceInProgress;
		}

		private static readonly RandomNumberGenerator s_random = RandomNumberGenerator.Create();

		private static readonly UTF8Encoding s_textEncoding = new UTF8Encoding(false, true);

		private static readonly WebSocketState[] s_validSendStates = new WebSocketState[]
		{
			WebSocketState.Open,
			WebSocketState.CloseReceived
		};

		private static readonly WebSocketState[] s_validReceiveStates = new WebSocketState[]
		{
			WebSocketState.Open,
			WebSocketState.CloseSent
		};

		private static readonly WebSocketState[] s_validCloseOutputStates = new WebSocketState[]
		{
			WebSocketState.Open,
			WebSocketState.CloseReceived
		};

		private static readonly WebSocketState[] s_validCloseStates = new WebSocketState[]
		{
			WebSocketState.Open,
			WebSocketState.CloseReceived,
			WebSocketState.CloseSent
		};

		private const int MaxMessageHeaderLength = 14;

		private const int MaxControlPayloadLength = 125;

		private const int MaskLength = 4;

		private readonly Stream _stream;

		private readonly bool _isServer;

		private readonly string _subprotocol;

		private readonly Timer _keepAliveTimer;

		private readonly CancellationTokenSource _abortSource = new CancellationTokenSource();

		private byte[] _receiveBuffer;

		private readonly bool _receiveBufferFromPool;

		private readonly ManagedWebSocket.Utf8MessageState _utf8TextState = new ManagedWebSocket.Utf8MessageState();

		private readonly SemaphoreSlim _sendFrameAsyncLock = new SemaphoreSlim(1, 1);

		private WebSocketState _state = WebSocketState.Open;

		private bool _disposed;

		private bool _sentCloseFrame;

		private bool _receivedCloseFrame;

		private WebSocketCloseStatus? _closeStatus;

		private string _closeStatusDescription;

		private ManagedWebSocket.MessageHeader _lastReceiveHeader = new ManagedWebSocket.MessageHeader
		{
			Opcode = ManagedWebSocket.MessageOpcode.Text,
			Fin = true
		};

		private int _receiveBufferOffset;

		private int _receiveBufferCount;

		private int _receivedMaskOffsetOffset;

		private byte[] _sendBuffer;

		private bool _lastSendWasFragment;

		private Task _lastSendAsync;

		private Task<WebSocketReceiveResult> _lastReceiveAsync;

		private sealed class Utf8MessageState
		{
			internal bool SequenceInProgress;

			internal int AdditionalBytesExpected;

			internal int ExpectedValueMin;

			internal int CurrentDecodeBits;
		}

		private enum MessageOpcode : byte
		{
			Continuation,
			Text,
			Binary,
			Close = 8,
			Ping,
			Pong
		}

		[StructLayout(LayoutKind.Auto)]
		private struct MessageHeader
		{
			internal ManagedWebSocket.MessageOpcode Opcode;

			internal bool Fin;

			internal long PayloadLength;

			internal int Mask;
		}
	}
}
