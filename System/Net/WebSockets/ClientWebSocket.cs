using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
	public sealed class ClientWebSocket : WebSocket
	{
		public ClientWebSocket()
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Enter(this, null, ".ctor");
			}
			WebSocketHandle.CheckPlatformSupport();
			this._state = 0;
			this._options = new ClientWebSocketOptions();
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Exit(this, null, ".ctor");
			}
		}

		public ClientWebSocketOptions Options
		{
			get
			{
				return this._options;
			}
		}

		public override WebSocketCloseStatus? CloseStatus
		{
			get
			{
				if (WebSocketHandle.IsValid(this._innerWebSocket))
				{
					return this._innerWebSocket.CloseStatus;
				}
				return null;
			}
		}

		public override string CloseStatusDescription
		{
			get
			{
				if (WebSocketHandle.IsValid(this._innerWebSocket))
				{
					return this._innerWebSocket.CloseStatusDescription;
				}
				return null;
			}
		}

		public override string SubProtocol
		{
			get
			{
				if (WebSocketHandle.IsValid(this._innerWebSocket))
				{
					return this._innerWebSocket.SubProtocol;
				}
				return null;
			}
		}

		public override WebSocketState State
		{
			get
			{
				if (WebSocketHandle.IsValid(this._innerWebSocket))
				{
					return this._innerWebSocket.State;
				}
				ClientWebSocket.InternalState state = (ClientWebSocket.InternalState)this._state;
				if (state == ClientWebSocket.InternalState.Created)
				{
					return WebSocketState.None;
				}
				if (state != ClientWebSocket.InternalState.Connecting)
				{
					return WebSocketState.Closed;
				}
				return WebSocketState.Connecting;
			}
		}

		public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (!uri.IsAbsoluteUri)
			{
				throw new ArgumentException("This operation is not supported for a relative URI.", "uri");
			}
			if (uri.Scheme != "ws" && uri.Scheme != "wss")
			{
				throw new ArgumentException("Only Uris starting with 'ws://' or 'wss://' are supported.", "uri");
			}
			ClientWebSocket.InternalState internalState = (ClientWebSocket.InternalState)Interlocked.CompareExchange(ref this._state, 1, 0);
			if (internalState == ClientWebSocket.InternalState.Disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (internalState != ClientWebSocket.InternalState.Created)
			{
				throw new InvalidOperationException("The WebSocket has already been started.");
			}
			this._options.SetToReadOnly();
			return this.ConnectAsyncCore(uri, cancellationToken);
		}

		private async Task ConnectAsyncCore(Uri uri, CancellationToken cancellationToken)
		{
			this._innerWebSocket = WebSocketHandle.Create();
			try
			{
				if (Interlocked.CompareExchange(ref this._state, 2, 1) != 1)
				{
					throw new ObjectDisposedException(base.GetType().FullName);
				}
				await this._innerWebSocket.ConnectAsyncCore(uri, cancellationToken, this._options).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Error(this, ex, "ConnectAsyncCore");
				}
				throw;
			}
		}

		public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			this.ThrowIfNotConnected();
			return this._innerWebSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
		}

		public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			this.ThrowIfNotConnected();
			return this._innerWebSocket.ReceiveAsync(buffer, cancellationToken);
		}

		public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			this.ThrowIfNotConnected();
			return this._innerWebSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
		}

		public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			this.ThrowIfNotConnected();
			return this._innerWebSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
		}

		public override void Abort()
		{
			if (this._state == 3)
			{
				return;
			}
			if (WebSocketHandle.IsValid(this._innerWebSocket))
			{
				this._innerWebSocket.Abort();
			}
			this.Dispose();
		}

		public override void Dispose()
		{
			if (Interlocked.Exchange(ref this._state, 3) == 3)
			{
				return;
			}
			if (WebSocketHandle.IsValid(this._innerWebSocket))
			{
				this._innerWebSocket.Dispose();
			}
		}

		private void ThrowIfNotConnected()
		{
			if (this._state == 3)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (this._state != 2)
			{
				throw new InvalidOperationException("The WebSocket is not connected.");
			}
		}

		private readonly ClientWebSocketOptions _options;

		private WebSocketHandle _innerWebSocket;

		private int _state;

		private enum InternalState
		{
			Created,
			Connecting,
			Connected,
			Disposed
		}
	}
}
