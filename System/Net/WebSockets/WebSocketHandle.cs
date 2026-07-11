using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
	internal sealed class WebSocketHandle
	{
		public static WebSocketHandle Create()
		{
			return new WebSocketHandle();
		}

		public static bool IsValid(WebSocketHandle handle)
		{
			return handle != null;
		}

		public WebSocketCloseStatus? CloseStatus
		{
			get
			{
				WebSocket webSocket = this._webSocket;
				if (webSocket == null)
				{
					return null;
				}
				return webSocket.CloseStatus;
			}
		}

		public string CloseStatusDescription
		{
			get
			{
				WebSocket webSocket = this._webSocket;
				if (webSocket == null)
				{
					return null;
				}
				return webSocket.CloseStatusDescription;
			}
		}

		public WebSocketState State
		{
			get
			{
				WebSocket webSocket = this._webSocket;
				if (webSocket == null)
				{
					return this._state;
				}
				return webSocket.State;
			}
		}

		public string SubProtocol
		{
			get
			{
				WebSocket webSocket = this._webSocket;
				if (webSocket == null)
				{
					return null;
				}
				return webSocket.SubProtocol;
			}
		}

		public static void CheckPlatformSupport()
		{
		}

		public void Dispose()
		{
			this._state = WebSocketState.Closed;
			WebSocket webSocket = this._webSocket;
			if (webSocket == null)
			{
				return;
			}
			webSocket.Dispose();
		}

		public void Abort()
		{
			this._abortSource.Cancel();
			WebSocket webSocket = this._webSocket;
			if (webSocket == null)
			{
				return;
			}
			webSocket.Abort();
		}

		public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			return this._webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
		}

		public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			return this._webSocket.ReceiveAsync(buffer, cancellationToken);
		}

		public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			return this._webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
		}

		public Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			return this._webSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
		}

		public async Task ConnectAsyncCore(Uri uri, CancellationToken cancellationToken, ClientWebSocketOptions options)
		{
			using (cancellationToken.Register(delegate(object s)
			{
				((WebSocketHandle)s).Abort();
			}, this))
			{
				try
				{
					Socket socket = await this.ConnectSocketAsync(uri.Host, uri.Port, cancellationToken).ConfigureAwait(false);
					Stream stream = new NetworkStream(socket, true);
					if (uri.Scheme == "wss")
					{
						SslStream sslStream = new SslStream(stream);
						await sslStream.AuthenticateAsClientAsync(uri.Host, options.ClientCertificates, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false).ConfigureAwait(false);
						stream = sslStream;
						sslStream = null;
					}
					KeyValuePair<string, string> secKeyAndSecWebSocketAccept = WebSocketHandle.CreateSecKeyAndSecWebSocketAccept();
					byte[] array = WebSocketHandle.BuildRequestHeader(uri, options, secKeyAndSecWebSocketAccept.Key);
					await stream.WriteAsync(array, 0, array.Length, cancellationToken).ConfigureAwait(false);
					string text = await this.ParseAndValidateConnectResponseAsync(stream, options, secKeyAndSecWebSocketAccept.Value, cancellationToken).ConfigureAwait(false);
					this._webSocket = WebSocket.CreateClientWebSocket(stream, text, options.ReceiveBufferSize, options.SendBufferSize, options.KeepAliveInterval, false, options.Buffer.GetValueOrDefault());
					if (this._state == WebSocketState.Aborted)
					{
						this._webSocket.Abort();
					}
					else if (this._state == WebSocketState.Closed)
					{
						this._webSocket.Dispose();
					}
					stream = null;
					secKeyAndSecWebSocketAccept = default(KeyValuePair<string, string>);
				}
				catch (Exception ex)
				{
					if (this._state < WebSocketState.Closed)
					{
						this._state = WebSocketState.Closed;
					}
					this.Abort();
					if (ex is WebSocketException)
					{
						throw;
					}
					throw new WebSocketException("Unable to connect to the remote server", ex);
				}
			}
		}

		private async Task<Socket> ConnectSocketAsync(string host, int port, CancellationToken cancellationToken)
		{
			IPAddress[] array = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);
			ExceptionDispatchInfo lastException = null;
			foreach (IPAddress ipaddress in array)
			{
				Socket socket = new Socket(ipaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				try
				{
					CancellationToken cancellationToken2;
					using (cancellationToken.Register(delegate(object s)
					{
						((Socket)s).Dispose();
					}, socket))
					{
						cancellationToken2 = this._abortSource.Token;
						using (cancellationToken2.Register(delegate(object s)
						{
							((Socket)s).Dispose();
						}, socket))
						{
							try
							{
								await socket.ConnectAsync(ipaddress, port).ConfigureAwait(false);
							}
							catch (ObjectDisposedException ex)
							{
								CancellationToken cancellationToken3 = (cancellationToken.IsCancellationRequested ? cancellationToken : this._abortSource.Token);
								if (cancellationToken3.IsCancellationRequested)
								{
									throw new OperationCanceledException(new OperationCanceledException().Message, ex, cancellationToken3);
								}
							}
						}
						CancellationTokenRegistration cancellationTokenRegistration2 = default(CancellationTokenRegistration);
					}
					CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
					cancellationToken.ThrowIfCancellationRequested();
					cancellationToken2 = this._abortSource.Token;
					cancellationToken2.ThrowIfCancellationRequested();
					return socket;
				}
				catch (Exception ex2)
				{
					socket.Dispose();
					lastException = ExceptionDispatchInfo.Capture(ex2);
				}
				socket = null;
			}
			IPAddress[] array2 = null;
			ExceptionDispatchInfo exceptionDispatchInfo = lastException;
			if (exceptionDispatchInfo != null)
			{
				exceptionDispatchInfo.Throw();
			}
			throw new WebSocketException("Unable to connect to the remote server");
		}

		private static byte[] BuildRequestHeader(Uri uri, ClientWebSocketOptions options, string secKey)
		{
			StringBuilder stringBuilder;
			if ((stringBuilder = WebSocketHandle.t_cachedStringBuilder) == null)
			{
				stringBuilder = (WebSocketHandle.t_cachedStringBuilder = new StringBuilder());
			}
			StringBuilder stringBuilder2 = stringBuilder;
			byte[] bytes;
			try
			{
				stringBuilder2.Append("GET ").Append(uri.PathAndQuery).Append(" HTTP/1.1\r\n");
				string text = options.RequestHeaders["Host"];
				stringBuilder2.Append("Host: ");
				if (string.IsNullOrEmpty(text))
				{
					stringBuilder2.Append(uri.IdnHost).Append(':').Append(uri.Port)
						.Append("\r\n");
				}
				else
				{
					stringBuilder2.Append(text).Append("\r\n");
				}
				stringBuilder2.Append("Connection: Upgrade\r\n");
				stringBuilder2.Append("Upgrade: websocket\r\n");
				stringBuilder2.Append("Sec-WebSocket-Version: 13\r\n");
				stringBuilder2.Append("Sec-WebSocket-Key: ").Append(secKey).Append("\r\n");
				foreach (string text2 in options.RequestHeaders.AllKeys)
				{
					if (!string.Equals(text2, "Host", StringComparison.OrdinalIgnoreCase))
					{
						stringBuilder2.Append(text2).Append(": ").Append(options.RequestHeaders[text2])
							.Append("\r\n");
					}
				}
				if (options.RequestedSubProtocols.Count > 0)
				{
					stringBuilder2.Append("Sec-WebSocket-Protocol").Append(": ");
					stringBuilder2.Append(options.RequestedSubProtocols[0]);
					for (int j = 1; j < options.RequestedSubProtocols.Count; j++)
					{
						stringBuilder2.Append(", ").Append(options.RequestedSubProtocols[j]);
					}
					stringBuilder2.Append("\r\n");
				}
				if (options.Cookies != null)
				{
					string cookieHeader = options.Cookies.GetCookieHeader(uri);
					if (!string.IsNullOrWhiteSpace(cookieHeader))
					{
						stringBuilder2.Append("Cookie").Append(": ").Append(cookieHeader)
							.Append("\r\n");
					}
				}
				stringBuilder2.Append("\r\n");
				bytes = WebSocketHandle.s_defaultHttpEncoding.GetBytes(stringBuilder2.ToString());
			}
			finally
			{
				stringBuilder2.Clear();
			}
			return bytes;
		}

		private static KeyValuePair<string, string> CreateSecKeyAndSecWebSocketAccept()
		{
			string text = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
			KeyValuePair<string, string> keyValuePair;
			using (SHA1 sha = SHA1.Create())
			{
				keyValuePair = new KeyValuePair<string, string>(text, Convert.ToBase64String(sha.ComputeHash(Encoding.ASCII.GetBytes(text + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))));
			}
			return keyValuePair;
		}

		private async Task<string> ParseAndValidateConnectResponseAsync(Stream stream, ClientWebSocketOptions options, string expectedSecWebSocketAccept, CancellationToken cancellationToken)
		{
			string text = await WebSocketHandle.ReadResponseHeaderLineAsync(stream, cancellationToken).ConfigureAwait(false);
			if (string.IsNullOrEmpty(text))
			{
				throw new WebSocketException(global::SR.Format("Unable to connect to the remote server", Array.Empty<object>()));
			}
			if (!text.StartsWith("HTTP/1.1 ", StringComparison.Ordinal) || text.Length < "HTTP/1.1 101".Length)
			{
				throw new WebSocketException(WebSocketError.HeaderError);
			}
			if (!text.StartsWith("HTTP/1.1 101", StringComparison.Ordinal) || (text.Length > "HTTP/1.1 101".Length && !char.IsWhiteSpace(text["HTTP/1.1 101".Length])))
			{
				throw new WebSocketException("Unable to connect to the remote server");
			}
			bool foundUpgrade = false;
			bool foundConnection = false;
			bool foundSecWebSocketAccept = false;
			string subprotocol = null;
			string line;
			while (!string.IsNullOrEmpty(line = await WebSocketHandle.ReadResponseHeaderLineAsync(stream, cancellationToken).ConfigureAwait(false)))
			{
				int num = line.IndexOf(':');
				if (num == -1)
				{
					throw new WebSocketException(WebSocketError.HeaderError);
				}
				string text2 = line.SubstringTrim(0, num);
				string headerValue = line.SubstringTrim(num + 1);
				WebSocketHandle.ValidateAndTrackHeader("Connection", "Upgrade", text2, headerValue, ref foundConnection);
				WebSocketHandle.ValidateAndTrackHeader("Upgrade", "websocket", text2, headerValue, ref foundUpgrade);
				WebSocketHandle.ValidateAndTrackHeader("Sec-WebSocket-Accept", expectedSecWebSocketAccept, text2, headerValue, ref foundSecWebSocketAccept);
				if (string.Equals("Sec-WebSocket-Protocol", text2, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(headerValue))
				{
					string text3 = options.RequestedSubProtocols.Find((string requested) => string.Equals(requested, headerValue, StringComparison.OrdinalIgnoreCase));
					if (text3 == null || subprotocol != null)
					{
						throw new WebSocketException(WebSocketError.UnsupportedProtocol, global::SR.Format("The WebSocket client request requested '{0}' protocol(s), but server is only accepting '{1}' protocol(s).", string.Join(", ", options.RequestedSubProtocols), subprotocol));
					}
					subprotocol = text3;
				}
			}
			if (!foundUpgrade || !foundConnection || !foundSecWebSocketAccept)
			{
				throw new WebSocketException("Unable to connect to the remote server");
			}
			return subprotocol;
		}

		private static void ValidateAndTrackHeader(string targetHeaderName, string targetHeaderValue, string foundHeaderName, string foundHeaderValue, ref bool foundHeader)
		{
			bool flag = string.Equals(targetHeaderName, foundHeaderName, StringComparison.OrdinalIgnoreCase);
			if (!foundHeader)
			{
				if (flag)
				{
					if (!string.Equals(targetHeaderValue, foundHeaderValue, StringComparison.OrdinalIgnoreCase))
					{
						throw new WebSocketException(global::SR.Format("The '{0}' header value '{1}' is invalid.", targetHeaderName, foundHeaderValue));
					}
					foundHeader = true;
					return;
				}
			}
			else if (flag)
			{
				throw new WebSocketException(global::SR.Format("Unable to connect to the remote server", Array.Empty<object>()));
			}
		}

		private static async Task<string> ReadResponseHeaderLineAsync(Stream stream, CancellationToken cancellationToken)
		{
			StringBuilder sb = WebSocketHandle.t_cachedStringBuilder;
			if (sb != null)
			{
				WebSocketHandle.t_cachedStringBuilder = null;
			}
			else
			{
				sb = new StringBuilder();
			}
			byte[] arr = new byte[1];
			char prevChar = '\0';
			string text;
			try
			{
				for (;;)
				{
					ConfiguredTaskAwaitable<int>.ConfiguredTaskAwaiter configuredTaskAwaiter = stream.ReadAsync(arr, 0, 1, cancellationToken).ConfigureAwait(false).GetAwaiter();
					if (!configuredTaskAwaiter.IsCompleted)
					{
						await configuredTaskAwaiter;
						ConfiguredTaskAwaitable<int>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
						configuredTaskAwaiter = configuredTaskAwaiter2;
						configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<int>.ConfiguredTaskAwaiter);
					}
					if (configuredTaskAwaiter.GetResult() != 1)
					{
						break;
					}
					char c = (char)arr[0];
					if (prevChar == '\r' && c == '\n')
					{
						break;
					}
					sb.Append(c);
					prevChar = c;
				}
				if (sb.Length > 0 && sb[sb.Length - 1] == '\r')
				{
					sb.Length--;
				}
				text = sb.ToString();
			}
			finally
			{
				sb.Clear();
				WebSocketHandle.t_cachedStringBuilder = sb;
			}
			return text;
		}

		[ThreadStatic]
		private static StringBuilder t_cachedStringBuilder;

		private static readonly Encoding s_defaultHttpEncoding = Encoding.GetEncoding(28591);

		private const int DefaultReceiveBufferSize = 4096;

		private const string WSServerGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		private readonly CancellationTokenSource _abortSource = new CancellationTokenSource();

		private WebSocketState _state = WebSocketState.Connecting;

		private WebSocket _webSocket;
	}
}
