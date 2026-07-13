using System;
using System.Buffers;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
	public abstract class WebSocket : IDisposable
	{
		public abstract WebSocketCloseStatus? CloseStatus { get; }

		public abstract string CloseStatusDescription { get; }

		public abstract string SubProtocol { get; }

		public abstract WebSocketState State { get; }

		public abstract void Abort();

		public abstract Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);

		public abstract Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);

		public abstract void Dispose();

		public abstract Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);

		public abstract Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);

		public virtual async ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
		{
			ArraySegment<byte> arraySegment;
			ValueWebSocketReceiveResult valueWebSocketReceiveResult;
			if (MemoryMarshal.TryGetArray<byte>(buffer, out arraySegment))
			{
				WebSocketReceiveResult webSocketReceiveResult = await this.ReceiveAsync(arraySegment, cancellationToken).ConfigureAwait(false);
				valueWebSocketReceiveResult = new ValueWebSocketReceiveResult(webSocketReceiveResult.Count, webSocketReceiveResult.MessageType, webSocketReceiveResult.EndOfMessage);
			}
			else
			{
				byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
				try
				{
					WebSocketReceiveResult webSocketReceiveResult2 = await this.ReceiveAsync(new ArraySegment<byte>(array, 0, buffer.Length), cancellationToken).ConfigureAwait(false);
					new Span<byte>(array, 0, webSocketReceiveResult2.Count).CopyTo(buffer.Span);
					valueWebSocketReceiveResult = new ValueWebSocketReceiveResult(webSocketReceiveResult2.Count, webSocketReceiveResult2.MessageType, webSocketReceiveResult2.EndOfMessage);
				}
				finally
				{
					ArrayPool<byte>.Shared.Return(array, false);
				}
			}
			return valueWebSocketReceiveResult;
		}

		public virtual ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			ArraySegment<byte> arraySegment;
			return new ValueTask(MemoryMarshal.TryGetArray<byte>(buffer, out arraySegment) ? this.SendAsync(arraySegment, messageType, endOfMessage, cancellationToken) : this.SendWithArrayPoolAsync(buffer, messageType, endOfMessage, cancellationToken));
		}

		private async Task SendWithArrayPoolAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
			try
			{
				buffer.Span.CopyTo(array);
				await this.SendAsync(new ArraySegment<byte>(array, 0, buffer.Length), messageType, endOfMessage, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
		}

		public static TimeSpan DefaultKeepAliveInterval
		{
			get
			{
				return TimeSpan.FromSeconds(30.0);
			}
		}

		protected static void ThrowOnInvalidState(WebSocketState state, params WebSocketState[] validStates)
		{
			string text = string.Empty;
			if (validStates != null && validStates.Length != 0)
			{
				foreach (WebSocketState webSocketState in validStates)
				{
					if (state == webSocketState)
					{
						return;
					}
				}
				text = string.Join<WebSocketState>(", ", validStates);
			}
			throw new WebSocketException(SR.Format("The WebSocket is in an invalid state ('{0}') for this operation. Valid states are: '{1}'", state, text));
		}

		protected static bool IsStateTerminal(WebSocketState state)
		{
			return state == WebSocketState.Closed || state == WebSocketState.Aborted;
		}

		public static ArraySegment<byte> CreateClientBuffer(int receiveBufferSize, int sendBufferSize)
		{
			if (receiveBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("receiveBufferSize", receiveBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			if (sendBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("sendBufferSize", sendBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			return new ArraySegment<byte>(new byte[Math.Max(receiveBufferSize, sendBufferSize)]);
		}

		public static ArraySegment<byte> CreateServerBuffer(int receiveBufferSize)
		{
			if (receiveBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("receiveBufferSize", receiveBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			return new ArraySegment<byte>(new byte[receiveBufferSize]);
		}

		public static WebSocket CreateFromStream(Stream stream, bool isServer, string subProtocol, TimeSpan keepAliveInterval)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead || !stream.CanWrite)
			{
				throw new ArgumentException((!stream.CanRead) ? "The base stream is not readable." : "The base stream is not writeable.", "stream");
			}
			if (subProtocol != null)
			{
				WebSocketValidate.ValidateSubprotocol(subProtocol);
			}
			if (keepAliveInterval != Timeout.InfiniteTimeSpan && keepAliveInterval < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("keepAliveInterval", keepAliveInterval, SR.Format("The argument must be a value greater than {0}.", 0));
			}
			return ManagedWebSocket.CreateFromConnectedStream(stream, isServer, subProtocol, keepAliveInterval);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.")]
		public static bool IsApplicationTargeting45()
		{
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void RegisterPrefixes()
		{
			throw new PlatformNotSupportedException();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static WebSocket CreateClientWebSocket(Stream innerStream, string subProtocol, int receiveBufferSize, int sendBufferSize, TimeSpan keepAliveInterval, bool useZeroMaskingKey, ArraySegment<byte> internalBuffer)
		{
			if (innerStream == null)
			{
				throw new ArgumentNullException("innerStream");
			}
			if (!innerStream.CanRead || !innerStream.CanWrite)
			{
				throw new ArgumentException((!innerStream.CanRead) ? "The base stream is not readable." : "The base stream is not writeable.", "innerStream");
			}
			if (subProtocol != null)
			{
				WebSocketValidate.ValidateSubprotocol(subProtocol);
			}
			if (keepAliveInterval != Timeout.InfiniteTimeSpan && keepAliveInterval < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("keepAliveInterval", keepAliveInterval, SR.Format("The argument must be a value greater than {0}.", 0));
			}
			if (receiveBufferSize <= 0 || sendBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException((receiveBufferSize <= 0) ? "receiveBufferSize" : "sendBufferSize", (receiveBufferSize <= 0) ? receiveBufferSize : sendBufferSize, SR.Format("The argument must be a value greater than {0}.", 0));
			}
			return ManagedWebSocket.CreateFromConnectedStream(innerStream, false, subProtocol, keepAliveInterval);
		}
	}
}
