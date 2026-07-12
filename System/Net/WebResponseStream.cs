using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class WebResponseStream : WebConnectionStream
	{
		public WebRequestStream RequestStream { get; }

		public WebHeaderCollection Headers { get; private set; }

		public HttpStatusCode StatusCode { get; private set; }

		public string StatusDescription { get; private set; }

		public Version Version { get; private set; }

		public bool KeepAlive { get; private set; }

		public WebResponseStream(WebRequestStream request)
			: base(request.Connection, request.Operation, request.InnerStream)
		{
			this.RequestStream = request;
			request.InnerStream.ReadTimeout = this.ReadTimeout;
		}

		public override long Length
		{
			get
			{
				return (long)this.stream_length;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		private protected bool ChunkedRead { protected get; private set; }

		private protected MonoChunkStream ChunkStream { protected get; private set; }

		public override async Task<int> ReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || num < offset)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || num - offset < size)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (Interlocked.CompareExchange(ref this.nestedRead, 1, 0) != 0)
			{
				throw new InvalidOperationException("Invalid nested call.");
			}
			WebCompletionSource completion = new WebCompletionSource();
			while (!cancellationToken.IsCancellationRequested)
			{
				WebCompletionSource webCompletionSource = Interlocked.CompareExchange<WebCompletionSource>(ref this.pendingRead, completion, null);
				if (webCompletionSource == null)
				{
					break;
				}
				await webCompletionSource.WaitForCompletion(true).ConfigureAwait(false);
			}
			int oldBytes = 0;
			int nbytes = 0;
			Exception throwMe = null;
			try
			{
				ValueTuple<int, int> valueTuple = await HttpWebRequest.RunWithTimeout<ValueTuple<int, int>>((CancellationToken ct) => this.ProcessRead(buffer, offset, size, ct), this.ReadTimeout, delegate
				{
					this.Operation.Abort();
					this.InnerStream.Dispose();
				}).ConfigureAwait(false);
				oldBytes = valueTuple.Item1;
				nbytes = valueTuple.Item2;
			}
			catch (Exception ex)
			{
				throwMe = this.GetReadException(WebExceptionStatus.ReceiveFailure, ex, "ReadAsync");
			}
			object obj;
			if (throwMe != null)
			{
				obj = this.locker;
				lock (obj)
				{
					completion.TrySetException(throwMe);
					this.pendingRead = null;
					this.nestedRead = 0;
				}
				this.closed = true;
				base.Operation.CompleteResponseRead(false, throwMe);
				throw throwMe;
			}
			obj = this.locker;
			lock (obj)
			{
				this.pendingRead.TrySetCompleted();
				this.pendingRead = null;
				this.nestedRead = 0;
			}
			if (this.totalRead >= this.contentLength && !this.nextReadCalled && !this.nextReadCalled)
			{
				this.nextReadCalled = true;
				base.Operation.CompleteResponseRead(true, null);
			}
			return oldBytes + nbytes;
		}

		private async Task<ValueTuple<int, int>> ProcessRead(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ValueTuple<int, int> valueTuple;
			if (this.totalRead >= this.contentLength)
			{
				this.read_eof = true;
				this.contentLength = this.totalRead;
				valueTuple = new ValueTuple<int, int>(0, 0);
			}
			else
			{
				int oldBytes = 0;
				BufferOffsetSize bufferOffsetSize = this.readBuffer;
				int num = ((bufferOffsetSize != null) ? bufferOffsetSize.Size : 0);
				if (num > 0)
				{
					int num2 = ((num > size) ? size : num);
					Buffer.BlockCopy(this.readBuffer.Buffer, this.readBuffer.Offset, buffer, offset, num2);
					this.readBuffer.Offset += num2;
					this.readBuffer.Size -= num2;
					offset += num2;
					size -= num2;
					this.totalRead += (long)num2;
					if (this.totalRead >= this.contentLength)
					{
						this.contentLength = this.totalRead;
						this.read_eof = true;
					}
					if (size == 0 || this.totalRead >= this.contentLength)
					{
						return new ValueTuple<int, int>(0, num2);
					}
					oldBytes = num2;
				}
				if (this.contentLength != 9223372036854775807L && this.contentLength - this.totalRead < (long)size)
				{
					size = (int)(this.contentLength - this.totalRead);
				}
				if (this.read_eof)
				{
					this.contentLength = this.totalRead;
					valueTuple = new ValueTuple<int, int>(oldBytes, 0);
				}
				else
				{
					int num3 = await this.InnerReadAsync(buffer, offset, size, cancellationToken).ConfigureAwait(false);
					if (num3 <= 0)
					{
						this.read_eof = true;
						this.contentLength = this.totalRead;
						valueTuple = new ValueTuple<int, int>(oldBytes, 0);
					}
					else
					{
						this.totalRead += (long)num3;
						valueTuple = new ValueTuple<int, int>(oldBytes, num3);
					}
				}
			}
			return valueTuple;
		}

		internal async Task<int> InnerReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			base.Operation.ThrowIfDisposed(cancellationToken);
			int nbytes = 0;
			bool done = false;
			if (!this.ChunkedRead || (!this.ChunkStream.DataAvailable && this.ChunkStream.WantMore))
			{
				int num = await base.InnerStream.ReadAsync(buffer, offset, size, cancellationToken).ConfigureAwait(false);
				nbytes = num;
				if (!this.ChunkedRead)
				{
					return nbytes;
				}
				done = nbytes == 0;
			}
			try
			{
				this.ChunkStream.WriteAndReadBack(buffer, offset, size, ref nbytes);
				if (!done && nbytes == 0 && this.ChunkStream.WantMore)
				{
					int num = await this.EnsureReadAsync(buffer, offset, size, cancellationToken).ConfigureAwait(false);
					nbytes = num;
				}
			}
			catch (Exception ex)
			{
				if (ex is WebException || ex is OperationCanceledException)
				{
					throw;
				}
				throw new WebException("Invalid chunked data.", ex, WebExceptionStatus.ServerProtocolViolation, null);
			}
			if ((done || nbytes == 0) && this.ChunkStream.ChunkLeft != 0)
			{
				throw new WebException("Read error", null, WebExceptionStatus.ReceiveFailure, null);
			}
			return nbytes;
		}

		private async Task<int> EnsureReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			byte[] morebytes = null;
			int nbytes = 0;
			while (nbytes == 0 && this.ChunkStream.WantMore && !cancellationToken.IsCancellationRequested)
			{
				int num = this.ChunkStream.ChunkLeft;
				if (num <= 0)
				{
					num = 1024;
				}
				else if (num > 16384)
				{
					num = 16384;
				}
				if (morebytes == null || morebytes.Length < num)
				{
					morebytes = new byte[num];
				}
				int num2 = await base.InnerStream.ReadAsync(morebytes, 0, num, cancellationToken).ConfigureAwait(false);
				if (num2 <= 0)
				{
					return 0;
				}
				this.ChunkStream.Write(morebytes, 0, num2);
				nbytes += this.ChunkStream.Read(buffer, offset + nbytes, size - nbytes);
			}
			return nbytes;
		}

		private bool CheckAuthHeader(string headerName)
		{
			string text = this.Headers[headerName];
			return text != null && text.IndexOf("NTLM", StringComparison.Ordinal) != -1;
		}

		private bool IsNtlmAuth()
		{
			return (base.Request.Proxy != null && !base.Request.Proxy.IsBypassed(base.Request.Address) && this.CheckAuthHeader("Proxy-Authenticate")) || this.CheckAuthHeader("WWW-Authenticate");
		}

		private bool ExpectContent
		{
			get
			{
				return !(base.Request.Method == "HEAD") && (this.StatusCode >= HttpStatusCode.OK && this.StatusCode != HttpStatusCode.NoContent) && this.StatusCode != HttpStatusCode.NotModified;
			}
		}

		private async Task Initialize(BufferOffsetSize buffer, CancellationToken cancellationToken)
		{
			string text = this.Headers["Transfer-Encoding"];
			bool flag = text != null && text.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) != -1;
			string text2 = this.Headers["Content-Length"];
			if (!flag && !string.IsNullOrEmpty(text2))
			{
				if (!long.TryParse(text2, out this.contentLength))
				{
					this.contentLength = long.MaxValue;
				}
			}
			else
			{
				this.contentLength = long.MaxValue;
			}
			if (this.Version == HttpVersion.Version11 && this.RequestStream.KeepAlive)
			{
				this.KeepAlive = true;
				string text3 = this.Headers[base.ServicePoint.UsesProxy ? "Proxy-Connection" : "Connection"];
				if (text3 != null)
				{
					text3 = text3.ToLower();
					this.KeepAlive = text3.IndexOf("keep-alive", StringComparison.Ordinal) != -1;
					if (text3.IndexOf("close", StringComparison.Ordinal) != -1)
					{
						this.KeepAlive = false;
					}
				}
			}
			if (!int.TryParse(text2, out this.stream_length))
			{
				this.stream_length = -1;
			}
			string me = "WebResponseStream.Initialize()";
			string text4 = null;
			if (this.ExpectContent)
			{
				text4 = this.Headers["Transfer-Encoding"];
			}
			this.ChunkedRead = text4 != null && text4.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) != -1;
			if (!this.ChunkedRead)
			{
				this.readBuffer = buffer;
				try
				{
					if (this.contentLength > 0L && (long)this.readBuffer.Size >= this.contentLength && !this.IsNtlmAuth())
					{
						await this.ReadAllAsync(false, cancellationToken).ConfigureAwait(false);
					}
					goto IL_02DD;
				}
				catch (Exception ex)
				{
					throw this.GetReadException(WebExceptionStatus.ReceiveFailure, ex, me);
				}
			}
			if (this.ChunkStream == null)
			{
				try
				{
					this.ChunkStream = new MonoChunkStream(buffer.Buffer, buffer.Offset, buffer.Offset + buffer.Size, this.Headers);
					goto IL_02DD;
				}
				catch (Exception ex2)
				{
					throw this.GetReadException(WebExceptionStatus.ServerProtocolViolation, ex2, me);
				}
			}
			this.ChunkStream.ResetBuffer();
			try
			{
				this.ChunkStream.Write(buffer.Buffer, buffer.Offset, buffer.Size);
			}
			catch (Exception ex3)
			{
				throw this.GetReadException(WebExceptionStatus.ServerProtocolViolation, ex3, me);
			}
			IL_02DD:
			if (!this.ExpectContent)
			{
				if (!this.closed && !this.nextReadCalled)
				{
					if (this.contentLength == 9223372036854775807L)
					{
						this.contentLength = 0L;
					}
					this.nextReadCalled = true;
				}
				base.Operation.CompleteResponseRead(true, null);
			}
		}

		internal async Task ReadAllAsync(bool resending, CancellationToken cancellationToken)
		{
			if (this.read_eof || this.totalRead >= this.contentLength || this.nextReadCalled)
			{
				if (!this.nextReadCalled)
				{
					this.nextReadCalled = true;
					base.Operation.CompleteResponseRead(true, null);
				}
			}
			else
			{
				Task timeoutTask = Task.Delay(this.ReadTimeout);
				WebCompletionSource completion = new WebCompletionSource();
				ConfiguredTaskAwaitable<Task>.ConfiguredTaskAwaiter configuredTaskAwaiter;
				do
				{
					cancellationToken.ThrowIfCancellationRequested();
					WebCompletionSource webCompletionSource = Interlocked.CompareExchange<WebCompletionSource>(ref this.pendingRead, completion, null);
					if (webCompletionSource == null)
					{
						goto IL_0136;
					}
					Task<bool> task = webCompletionSource.WaitForCompletion(true);
					configuredTaskAwaiter = Task.WhenAny(new Task[] { task, timeoutTask }).ConfigureAwait(false).GetAwaiter();
					if (!configuredTaskAwaiter.IsCompleted)
					{
						await configuredTaskAwaiter;
						ConfiguredTaskAwaitable<Task>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
						configuredTaskAwaiter = configuredTaskAwaiter2;
						configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<Task>.ConfiguredTaskAwaiter);
					}
				}
				while (configuredTaskAwaiter.GetResult() != timeoutTask);
				throw new WebException("The operation has timed out.", WebExceptionStatus.Timeout);
				IL_0136:
				cancellationToken.ThrowIfCancellationRequested();
				try
				{
					if (this.totalRead >= this.contentLength)
					{
						return;
					}
					byte[] b = null;
					if (this.contentLength == 9223372036854775807L && !this.ChunkedRead)
					{
						if (resending)
						{
							this.Close();
							return;
						}
						this.KeepAlive = false;
					}
					int new_size;
					if (this.contentLength == 9223372036854775807L)
					{
						MemoryStream ms = new MemoryStream();
						BufferOffsetSize buffer = null;
						if (this.readBuffer != null && this.readBuffer.Size > 0)
						{
							ms.Write(this.readBuffer.Buffer, this.readBuffer.Offset, this.readBuffer.Size);
							this.readBuffer.Offset = 0;
							this.readBuffer.Size = this.readBuffer.Buffer.Length;
							if (this.readBuffer.Buffer.Length >= 8192)
							{
								buffer = this.readBuffer;
							}
						}
						if (buffer == null)
						{
							buffer = new BufferOffsetSize(new byte[8192], false);
						}
						int read;
						while ((read = await this.InnerReadAsync(buffer.Buffer, buffer.Offset, buffer.Size, cancellationToken)) != 0)
						{
							ms.Write(buffer.Buffer, buffer.Offset, read);
						}
						new_size = (int)ms.Length;
						this.contentLength = (long)new_size;
						this.readBuffer = new BufferOffsetSize(ms.GetBuffer(), 0, new_size, false);
						ms = null;
						buffer = null;
					}
					else
					{
						new_size = (int)(this.contentLength - this.totalRead);
						b = new byte[new_size];
						int readSize = 0;
						if (this.readBuffer != null && this.readBuffer.Size > 0)
						{
							readSize = this.readBuffer.Size;
							if (readSize > new_size)
							{
								readSize = new_size;
							}
							Buffer.BlockCopy(this.readBuffer.Buffer, this.readBuffer.Offset, b, 0, readSize);
						}
						int remaining = new_size - readSize;
						int num = -1;
						while (remaining > 0 && num != 0)
						{
							num = await this.InnerReadAsync(b, readSize, remaining, cancellationToken);
							remaining -= num;
							readSize += num;
						}
					}
					this.readBuffer = new BufferOffsetSize(b, 0, new_size, false);
					this.totalRead = 0L;
					this.nextReadCalled = true;
					completion.TrySetCompleted();
					b = null;
				}
				catch (Exception ex)
				{
					completion.TrySetException(ex);
					throw;
				}
				finally
				{
					this.pendingRead = null;
				}
				base.Operation.CompleteResponseRead(true, null);
			}
		}

		public override Task WriteAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			return Task.FromException(new NotSupportedException("The stream does not support writing."));
		}

		protected override void Close_internal(ref bool disposed)
		{
			if (!this.closed && !this.nextReadCalled)
			{
				this.nextReadCalled = true;
				if (this.totalRead >= this.contentLength)
				{
					disposed = true;
					base.Operation.CompleteResponseRead(true, null);
					return;
				}
				this.closed = true;
				disposed = true;
				base.Operation.CompleteResponseRead(false, null);
			}
		}

		private WebException GetReadException(WebExceptionStatus status, Exception error, string where)
		{
			error = base.GetException(error);
			string.Format("Error getting response stream ({0}): {1}", where, status);
			if (error == null)
			{
				return new WebException(string.Format("Error getting response stream ({0}): {1}", where, status), status);
			}
			WebException ex;
			if ((ex = error as WebException) != null)
			{
				return ex;
			}
			if (base.Operation.Aborted || error is OperationCanceledException || error is ObjectDisposedException)
			{
				return HttpWebRequest.CreateRequestAbortedException();
			}
			return new WebException(string.Format("Error getting response stream ({0}): {1} {2}", where, status, error.Message), status, WebExceptionInternalStatus.RequestFatal, error);
		}

		internal async Task InitReadAsync(CancellationToken cancellationToken)
		{
			BufferOffsetSize buffer = new BufferOffsetSize(new byte[4096], false);
			ReadState state = ReadState.None;
			int position = 0;
			for (;;)
			{
				base.Operation.ThrowIfClosedOrDisposed(cancellationToken);
				int num = await base.InnerStream.ReadAsync(buffer.Buffer, buffer.Offset, buffer.Size, cancellationToken).ConfigureAwait(false);
				if (num == 0)
				{
					break;
				}
				if (num < 0)
				{
					goto Block_2;
				}
				buffer.Offset += num;
				buffer.Size -= num;
				if (state == ReadState.None)
				{
					try
					{
						int num2 = position;
						if (!this.GetResponse(buffer, ref position, ref state))
						{
							position = num2;
						}
					}
					catch (Exception ex)
					{
						throw this.GetReadException(WebExceptionStatus.ServerProtocolViolation, ex, "ReadDoneAsync4");
					}
				}
				if (state == ReadState.Aborted)
				{
					goto Block_4;
				}
				if (state == ReadState.Content)
				{
					goto Block_5;
				}
				int num3 = num * 2;
				if (num3 > buffer.Size)
				{
					byte[] array = new byte[buffer.Buffer.Length + num3];
					Buffer.BlockCopy(buffer.Buffer, 0, array, 0, buffer.Offset);
					buffer = new BufferOffsetSize(array, buffer.Offset, array.Length - buffer.Offset, false);
				}
				state = ReadState.None;
				position = 0;
			}
			throw this.GetReadException(WebExceptionStatus.ReceiveFailure, null, "ReadDoneAsync2");
			Block_2:
			throw this.GetReadException(WebExceptionStatus.ServerProtocolViolation, null, "ReadDoneAsync3");
			Block_4:
			throw this.GetReadException(WebExceptionStatus.RequestCanceled, null, "ReadDoneAsync5");
			Block_5:
			buffer.Size = buffer.Offset - position;
			buffer.Offset = position;
			try
			{
				base.Operation.ThrowIfDisposed(cancellationToken);
				await this.Initialize(buffer, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex2)
			{
				throw this.GetReadException(WebExceptionStatus.ReceiveFailure, ex2, "ReadDoneAsync6");
			}
		}

		private bool GetResponse(BufferOffsetSize buffer, ref int pos, ref ReadState state)
		{
			string text = null;
			bool flag = false;
			bool flag2 = false;
			while (state != ReadState.Aborted)
			{
				if (state != ReadState.None)
				{
					goto IL_00FA;
				}
				if (!WebConnection.ReadLine(buffer.Buffer, ref pos, buffer.Offset, ref text))
				{
					return false;
				}
				if (text == null)
				{
					flag2 = true;
				}
				else
				{
					flag2 = false;
					state = ReadState.Status;
					string[] array = text.Split(new char[] { ' ' });
					if (array.Length < 2)
					{
						throw this.GetReadException(WebExceptionStatus.ServerProtocolViolation, null, "GetResponse");
					}
					if (string.Compare(array[0], "HTTP/1.1", true) == 0)
					{
						this.Version = HttpVersion.Version11;
						base.ServicePoint.SetVersion(HttpVersion.Version11);
					}
					else
					{
						this.Version = HttpVersion.Version10;
						base.ServicePoint.SetVersion(HttpVersion.Version10);
					}
					this.StatusCode = (HttpStatusCode)uint.Parse(array[1]);
					if (array.Length >= 3)
					{
						this.StatusDescription = string.Join(" ", array, 2, array.Length - 2);
					}
					else
					{
						this.StatusDescription = string.Empty;
					}
					if (pos >= buffer.Offset)
					{
						return true;
					}
					goto IL_00FA;
				}
				IL_0287:
				if (!flag2 && !flag)
				{
					throw this.GetReadException(WebExceptionStatus.ServerProtocolViolation, null, "GetResponse");
				}
				continue;
				IL_00FA:
				flag2 = false;
				if (state != ReadState.Status)
				{
					goto IL_0287;
				}
				state = ReadState.Headers;
				this.Headers = new WebHeaderCollection();
				List<string> list = new List<string>();
				bool flag3 = false;
				while (!flag3 && WebConnection.ReadLine(buffer.Buffer, ref pos, buffer.Offset, ref text))
				{
					if (text == null)
					{
						flag3 = true;
					}
					else if (text.Length > 0 && (text[0] == ' ' || text[0] == '\t'))
					{
						int num = list.Count - 1;
						if (num < 0)
						{
							break;
						}
						string text2 = list[num] + text;
						list[num] = text2;
					}
					else
					{
						list.Add(text);
					}
				}
				if (!flag3)
				{
					return false;
				}
				foreach (string text3 in list)
				{
					int num2 = text3.IndexOf(':');
					if (num2 == -1)
					{
						throw new ArgumentException("no colon found", "header");
					}
					string text4 = text3.Substring(0, num2);
					string text5 = text3.Substring(num2 + 1).Trim();
					if (WebHeaderCollection.AllowMultiValues(text4))
					{
						this.Headers.AddInternal(text4, text5);
					}
					else
					{
						this.Headers.SetInternal(text4, text5);
					}
				}
				if (this.StatusCode != HttpStatusCode.Continue)
				{
					state = ReadState.Content;
					return true;
				}
				base.ServicePoint.SendContinue = true;
				if (pos >= buffer.Offset)
				{
					return true;
				}
				if (base.Request.ExpectContinue)
				{
					base.Request.DoContinueDelegate((int)this.StatusCode, this.Headers);
					base.Request.ExpectContinue = false;
				}
				state = ReadState.None;
				flag = true;
				goto IL_0287;
			}
			throw this.GetReadException(WebExceptionStatus.RequestCanceled, null, "GetResponse");
		}

		private BufferOffsetSize readBuffer;

		private long contentLength;

		private long totalRead;

		private bool nextReadCalled;

		private int stream_length;

		private WebCompletionSource pendingRead;

		private object locker = new object();

		private int nestedRead;

		private bool read_eof;

		internal readonly string ME;
	}
}
