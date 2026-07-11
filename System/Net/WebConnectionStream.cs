using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace System.Net
{
	internal class WebConnectionStream : Stream
	{
		public WebConnectionStream(WebConnection cnc, WebConnectionData data)
		{
			if (data == null)
			{
				throw new InvalidOperationException("data was not initialized");
			}
			if (data.Headers == null)
			{
				throw new InvalidOperationException("data.Headers was not initialized");
			}
			if (data.request == null)
			{
				throw new InvalidOperationException("data.request was not initialized");
			}
			this.isRead = true;
			this.cb_wrapper = new AsyncCallback(this.ReadCallbackWrapper);
			this.pending = new ManualResetEvent(true);
			this.request = data.request;
			this.read_timeout = this.request.ReadWriteTimeout;
			this.write_timeout = this.read_timeout;
			this.cnc = cnc;
			string text = data.Headers["Transfer-Encoding"];
			bool flag = text != null && text.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) != -1;
			string text2 = data.Headers["Content-Length"];
			if (!flag && text2 != null && text2 != "")
			{
				try
				{
					this.contentLength = (long)int.Parse(text2);
					if (this.contentLength == 0L && !this.IsNtlmAuth())
					{
						this.ReadAll();
					}
					goto IL_012C;
				}
				catch
				{
					this.contentLength = long.MaxValue;
					goto IL_012C;
				}
			}
			this.contentLength = long.MaxValue;
			IL_012C:
			if (!int.TryParse(text2, out this.stream_length))
			{
				this.stream_length = -1;
			}
		}

		public WebConnectionStream(WebConnection cnc, HttpWebRequest request)
		{
			this.read_timeout = request.ReadWriteTimeout;
			this.write_timeout = this.read_timeout;
			this.isRead = false;
			this.cb_wrapper = new AsyncCallback(this.WriteCallbackWrapper);
			this.cnc = cnc;
			this.request = request;
			this.allowBuffering = request.InternalAllowBuffering;
			this.sendChunked = request.SendChunked;
			if (this.sendChunked)
			{
				this.pending = new ManualResetEvent(true);
				return;
			}
			if (this.allowBuffering)
			{
				this.writeBuffer = new MemoryStream();
			}
		}

		private bool CheckAuthHeader(string headerName)
		{
			string text = this.cnc.Data.Headers[headerName];
			return text != null && text.IndexOf("NTLM", StringComparison.Ordinal) != -1;
		}

		private bool IsNtlmAuth()
		{
			return (this.request.Proxy != null && !this.request.Proxy.IsBypassed(this.request.Address) && this.CheckAuthHeader("Proxy-Authenticate")) || this.CheckAuthHeader("WWW-Authenticate");
		}

		internal void CheckResponseInBuffer()
		{
			if (this.contentLength > 0L && (long)(this.readBufferSize - this.readBufferOffset) >= this.contentLength && !this.IsNtlmAuth())
			{
				this.ReadAll();
			}
		}

		internal HttpWebRequest Request
		{
			get
			{
				return this.request;
			}
		}

		internal WebConnection Connection
		{
			get
			{
				return this.cnc;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return true;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.read_timeout;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.read_timeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this.write_timeout;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.write_timeout = value;
			}
		}

		internal bool CompleteRequestWritten
		{
			get
			{
				return this.complete_request_written;
			}
		}

		internal bool SendChunked
		{
			set
			{
				this.sendChunked = value;
			}
		}

		internal byte[] ReadBuffer
		{
			set
			{
				this.readBuffer = value;
			}
		}

		internal int ReadBufferOffset
		{
			set
			{
				this.readBufferOffset = value;
			}
		}

		internal int ReadBufferSize
		{
			set
			{
				this.readBufferSize = value;
			}
		}

		internal byte[] WriteBuffer
		{
			get
			{
				return this.writeBuffer.GetBuffer();
			}
		}

		internal int WriteBufferLength
		{
			get
			{
				if (this.writeBuffer == null)
				{
					return -1;
				}
				return (int)this.writeBuffer.Length;
			}
		}

		internal void ForceCompletion()
		{
			if (!this.nextReadCalled)
			{
				if (this.contentLength == 9223372036854775807L)
				{
					this.contentLength = 0L;
				}
				this.nextReadCalled = true;
				this.cnc.NextRead();
			}
		}

		internal void CheckComplete()
		{
			if (!this.nextReadCalled && (long)(this.readBufferSize - this.readBufferOffset) == this.contentLength)
			{
				this.nextReadCalled = true;
				this.cnc.NextRead();
			}
		}

		internal void ReadAll()
		{
			if (!this.isRead || this.read_eof || this.totalRead >= this.contentLength || this.nextReadCalled)
			{
				if (this.isRead && !this.nextReadCalled)
				{
					this.nextReadCalled = true;
					this.cnc.NextRead();
				}
				return;
			}
			if (!this.pending.WaitOne(this.ReadTimeout))
			{
				throw new WebException("The operation has timed out.", WebExceptionStatus.Timeout);
			}
			object obj = this.locker;
			lock (obj)
			{
				if (this.totalRead >= this.contentLength)
				{
					return;
				}
				int num = this.readBufferSize - this.readBufferOffset;
				byte[] array2;
				int num3;
				if (this.contentLength == 9223372036854775807L)
				{
					MemoryStream memoryStream = new MemoryStream();
					byte[] array = null;
					if (this.readBuffer != null && num > 0)
					{
						memoryStream.Write(this.readBuffer, this.readBufferOffset, num);
						if (this.readBufferSize >= 8192)
						{
							array = this.readBuffer;
						}
					}
					if (array == null)
					{
						array = new byte[8192];
					}
					int num2;
					while ((num2 = this.cnc.Read(this.request, array, 0, array.Length)) != 0)
					{
						memoryStream.Write(array, 0, num2);
					}
					array2 = memoryStream.GetBuffer();
					num3 = (int)memoryStream.Length;
					this.contentLength = (long)num3;
				}
				else
				{
					num3 = (int)(this.contentLength - this.totalRead);
					array2 = new byte[num3];
					if (this.readBuffer != null && num > 0)
					{
						if (num > num3)
						{
							num = num3;
						}
						Buffer.BlockCopy(this.readBuffer, this.readBufferOffset, array2, 0, num);
					}
					int num4 = num3 - num;
					int num5 = -1;
					while (num4 > 0 && num5 != 0)
					{
						num5 = this.cnc.Read(this.request, array2, num, num4);
						num4 -= num5;
						num += num5;
					}
				}
				this.readBuffer = array2;
				this.readBufferOffset = 0;
				this.readBufferSize = num3;
				this.totalRead = 0L;
				this.nextReadCalled = true;
			}
			this.cnc.NextRead();
		}

		private void WriteCallbackWrapper(IAsyncResult r)
		{
			WebAsyncResult webAsyncResult = r as WebAsyncResult;
			if (webAsyncResult != null && webAsyncResult.AsyncWriteAll)
			{
				return;
			}
			if (r.AsyncState != null)
			{
				webAsyncResult = (WebAsyncResult)r.AsyncState;
				webAsyncResult.InnerAsyncResult = r;
				webAsyncResult.DoCallback();
				return;
			}
			try
			{
				this.EndWrite(r);
			}
			catch
			{
			}
		}

		private void ReadCallbackWrapper(IAsyncResult r)
		{
			if (r.AsyncState != null)
			{
				WebAsyncResult webAsyncResult = (WebAsyncResult)r.AsyncState;
				webAsyncResult.InnerAsyncResult = r;
				webAsyncResult.DoCallback();
				return;
			}
			try
			{
				this.EndRead(r);
			}
			catch
			{
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			AsyncCallback asyncCallback = this.cb_wrapper;
			WebAsyncResult webAsyncResult = (WebAsyncResult)this.BeginRead(buffer, offset, size, asyncCallback, null);
			if (!webAsyncResult.IsCompleted && !webAsyncResult.WaitUntilComplete(this.ReadTimeout, false))
			{
				this.nextReadCalled = true;
				this.cnc.Close(true);
				throw new WebException("The operation has timed out.", WebExceptionStatus.Timeout);
			}
			return this.EndRead(webAsyncResult);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			if (!this.isRead)
			{
				throw new NotSupportedException("this stream does not allow reading");
			}
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
			object obj = this.locker;
			lock (obj)
			{
				this.pendingReads++;
				this.pending.Reset();
			}
			WebAsyncResult webAsyncResult = new WebAsyncResult(cb, state, buffer, offset, size);
			if (this.totalRead >= this.contentLength)
			{
				webAsyncResult.SetCompleted(true, -1);
				webAsyncResult.DoCallback();
				return webAsyncResult;
			}
			int num2 = this.readBufferSize - this.readBufferOffset;
			if (num2 > 0)
			{
				int num3 = ((num2 > size) ? size : num2);
				Buffer.BlockCopy(this.readBuffer, this.readBufferOffset, buffer, offset, num3);
				this.readBufferOffset += num3;
				offset += num3;
				size -= num3;
				this.totalRead += (long)num3;
				if (size == 0 || this.totalRead >= this.contentLength)
				{
					webAsyncResult.SetCompleted(true, num3);
					webAsyncResult.DoCallback();
					return webAsyncResult;
				}
				webAsyncResult.NBytes = num3;
			}
			if (cb != null)
			{
				cb = this.cb_wrapper;
			}
			if (this.contentLength != 9223372036854775807L && this.contentLength - this.totalRead < (long)size)
			{
				size = (int)(this.contentLength - this.totalRead);
			}
			if (!this.read_eof)
			{
				webAsyncResult.InnerAsyncResult = this.cnc.BeginRead(this.request, buffer, offset, size, cb, webAsyncResult);
			}
			else
			{
				webAsyncResult.SetCompleted(true, webAsyncResult.NBytes);
				webAsyncResult.DoCallback();
			}
			return webAsyncResult;
		}

		public override int EndRead(IAsyncResult r)
		{
			WebAsyncResult webAsyncResult = (WebAsyncResult)r;
			if (webAsyncResult.EndCalled)
			{
				int nbytes = webAsyncResult.NBytes;
				if (nbytes < 0)
				{
					return 0;
				}
				return nbytes;
			}
			else
			{
				webAsyncResult.EndCalled = true;
				object obj;
				if (!webAsyncResult.IsCompleted)
				{
					int num = -1;
					try
					{
						num = this.cnc.EndRead(this.request, webAsyncResult);
					}
					catch (Exception ex)
					{
						obj = this.locker;
						lock (obj)
						{
							this.pendingReads--;
							if (this.pendingReads == 0)
							{
								this.pending.Set();
							}
						}
						this.nextReadCalled = true;
						this.cnc.Close(true);
						webAsyncResult.SetCompleted(false, ex);
						webAsyncResult.DoCallback();
						throw;
					}
					if (num < 0)
					{
						num = 0;
						this.read_eof = true;
					}
					this.totalRead += (long)num;
					webAsyncResult.SetCompleted(false, num + webAsyncResult.NBytes);
					webAsyncResult.DoCallback();
					if (num == 0)
					{
						this.contentLength = this.totalRead;
					}
				}
				obj = this.locker;
				lock (obj)
				{
					this.pendingReads--;
					if (this.pendingReads == 0)
					{
						this.pending.Set();
					}
				}
				if (this.totalRead >= this.contentLength && !this.nextReadCalled)
				{
					this.ReadAll();
				}
				int nbytes2 = webAsyncResult.NBytes;
				if (nbytes2 < 0)
				{
					return 0;
				}
				return nbytes2;
			}
		}

		private void WriteAsyncCB(IAsyncResult r)
		{
			WebAsyncResult webAsyncResult = (WebAsyncResult)r.AsyncState;
			webAsyncResult.InnerAsyncResult = null;
			try
			{
				this.cnc.EndWrite(this.request, true, r);
				webAsyncResult.SetCompleted(false, 0);
				if (!this.initRead)
				{
					this.initRead = true;
					this.cnc.InitRead();
				}
			}
			catch (Exception ex)
			{
				this.KillBuffer();
				this.nextReadCalled = true;
				this.cnc.Close(true);
				if (ex is SocketException)
				{
					ex = new IOException("Error writing request", ex);
				}
				webAsyncResult.SetCompleted(false, ex);
			}
			if (this.allowBuffering && !this.sendChunked && this.request.ContentLength > 0L && this.totalWritten == this.request.ContentLength)
			{
				this.complete_request_written = true;
			}
			webAsyncResult.DoCallback();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			if (this.request.Aborted)
			{
				throw new WebException("The request was canceled.", WebExceptionStatus.RequestCanceled);
			}
			if (this.isRead)
			{
				throw new NotSupportedException("this stream does not allow writing");
			}
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
			if (this.sendChunked)
			{
				object obj = this.locker;
				lock (obj)
				{
					this.pendingWrites++;
					this.pending.Reset();
				}
			}
			WebAsyncResult webAsyncResult = new WebAsyncResult(cb, state);
			AsyncCallback asyncCallback = new AsyncCallback(this.WriteAsyncCB);
			if (this.sendChunked)
			{
				this.requestWritten = true;
				string text = string.Format("{0:X}\r\n", size);
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				int num2 = 2 + size + bytes.Length;
				byte[] array = new byte[num2];
				Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
				Buffer.BlockCopy(buffer, offset, array, bytes.Length, size);
				Buffer.BlockCopy(WebConnectionStream.crlf, 0, array, bytes.Length + size, WebConnectionStream.crlf.Length);
				if (this.allowBuffering)
				{
					if (this.writeBuffer == null)
					{
						this.writeBuffer = new MemoryStream();
					}
					this.writeBuffer.Write(buffer, offset, size);
					this.totalWritten += (long)size;
				}
				buffer = array;
				offset = 0;
				size = num2;
			}
			else
			{
				this.CheckWriteOverflow(this.request.ContentLength, this.totalWritten, (long)size);
				if (this.allowBuffering)
				{
					if (this.writeBuffer == null)
					{
						this.writeBuffer = new MemoryStream();
					}
					this.writeBuffer.Write(buffer, offset, size);
					this.totalWritten += (long)size;
					if (this.request.ContentLength <= 0L || this.totalWritten < this.request.ContentLength)
					{
						webAsyncResult.SetCompleted(true, 0);
						webAsyncResult.DoCallback();
						return webAsyncResult;
					}
					webAsyncResult.AsyncWriteAll = true;
					this.requestWritten = true;
					buffer = this.writeBuffer.GetBuffer();
					offset = 0;
					size = (int)this.totalWritten;
				}
			}
			try
			{
				webAsyncResult.InnerAsyncResult = this.cnc.BeginWrite(this.request, buffer, offset, size, asyncCallback, webAsyncResult);
				if (webAsyncResult.InnerAsyncResult == null)
				{
					if (!webAsyncResult.IsCompleted)
					{
						webAsyncResult.SetCompleted(true, 0);
					}
					webAsyncResult.DoCallback();
				}
			}
			catch (Exception)
			{
				if (!this.IgnoreIOErrors)
				{
					throw;
				}
				webAsyncResult.SetCompleted(true, 0);
				webAsyncResult.DoCallback();
			}
			this.totalWritten += (long)size;
			return webAsyncResult;
		}

		private void CheckWriteOverflow(long contentLength, long totalWritten, long size)
		{
			if (contentLength == -1L)
			{
				return;
			}
			long num = contentLength - totalWritten;
			if (size > num)
			{
				this.KillBuffer();
				this.nextReadCalled = true;
				this.cnc.Close(true);
				throw new ProtocolViolationException("The number of bytes to be written is greater than the specified ContentLength.");
			}
		}

		public override void EndWrite(IAsyncResult r)
		{
			if (r == null)
			{
				throw new ArgumentNullException("r");
			}
			WebAsyncResult webAsyncResult = r as WebAsyncResult;
			if (webAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult");
			}
			if (webAsyncResult.EndCalled)
			{
				return;
			}
			if (this.sendChunked)
			{
				object obj = this.locker;
				lock (obj)
				{
					this.pendingWrites--;
					if (this.pendingWrites <= 0)
					{
						this.pending.Set();
					}
				}
			}
			webAsyncResult.EndCalled = true;
			if (webAsyncResult.AsyncWriteAll)
			{
				webAsyncResult.WaitUntilComplete();
				if (webAsyncResult.GotException)
				{
					throw webAsyncResult.Exception;
				}
				return;
			}
			else
			{
				if (this.allowBuffering && !this.sendChunked)
				{
					return;
				}
				if (webAsyncResult.GotException)
				{
					throw webAsyncResult.Exception;
				}
				return;
			}
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			AsyncCallback asyncCallback = this.cb_wrapper;
			WebAsyncResult webAsyncResult = (WebAsyncResult)this.BeginWrite(buffer, offset, size, asyncCallback, null);
			if (!webAsyncResult.IsCompleted && !webAsyncResult.WaitUntilComplete(this.WriteTimeout, false))
			{
				this.KillBuffer();
				this.nextReadCalled = true;
				this.cnc.Close(true);
				throw new IOException("Write timed out.");
			}
			this.EndWrite(webAsyncResult);
		}

		public override void Flush()
		{
		}

		internal void SetHeadersAsync(bool setInternalLength, SimpleAsyncCallback callback)
		{
			SimpleAsyncResult.Run((SimpleAsyncResult r) => this.SetHeadersAsync(r, setInternalLength), callback);
		}

		private bool SetHeadersAsync(SimpleAsyncResult result, bool setInternalLength)
		{
			if (this.headersSent)
			{
				return false;
			}
			string method = this.request.Method;
			bool flag = method == "GET" || method == "CONNECT" || method == "HEAD" || method == "TRACE";
			bool flag2 = method == "PROPFIND" || method == "PROPPATCH" || method == "MKCOL" || method == "COPY" || method == "MOVE" || method == "LOCK" || method == "UNLOCK";
			if (setInternalLength && !flag && this.writeBuffer != null)
			{
				this.request.InternalContentLength = this.writeBuffer.Length;
			}
			bool flag3 = !flag && (this.writeBuffer == null || this.request.ContentLength > -1L);
			if (!this.sendChunked && !flag3 && !flag && !flag2)
			{
				return false;
			}
			this.headersSent = true;
			this.headers = this.request.GetRequestHeaders();
			return this.cnc.BeginWrite(this.request, this.headers, 0, this.headers.Length, delegate(IAsyncResult r)
			{
				try
				{
					this.cnc.EndWrite(this.request, true, r);
					if (!this.initRead)
					{
						this.initRead = true;
						this.cnc.InitRead();
					}
					long num = this.request.ContentLength;
					if (!this.sendChunked && num == 0L)
					{
						this.requestWritten = true;
					}
					result.SetCompleted(false);
				}
				catch (WebException ex)
				{
					result.SetCompleted(false, ex);
				}
				catch (Exception ex2)
				{
					result.SetCompleted(false, new WebException("Error writing headers", WebExceptionStatus.SendFailure, WebExceptionInternalStatus.RequestFatal, ex2));
				}
			}, null) != null;
		}

		internal bool RequestWritten
		{
			get
			{
				return this.requestWritten;
			}
		}

		internal SimpleAsyncResult WriteRequestAsync(SimpleAsyncCallback callback)
		{
			SimpleAsyncResult simpleAsyncResult = this.WriteRequestAsync(callback);
			try
			{
				if (!this.WriteRequestAsync(simpleAsyncResult))
				{
					simpleAsyncResult.SetCompleted(true);
				}
			}
			catch (Exception ex)
			{
				simpleAsyncResult.SetCompleted(true, ex);
			}
			return simpleAsyncResult;
		}

		internal bool WriteRequestAsync(SimpleAsyncResult result)
		{
			if (this.requestWritten)
			{
				return false;
			}
			this.requestWritten = true;
			if (this.sendChunked || !this.allowBuffering || this.writeBuffer == null)
			{
				return false;
			}
			byte[] bytes = this.writeBuffer.GetBuffer();
			int length = (int)this.writeBuffer.Length;
			if (this.request.ContentLength != -1L && this.request.ContentLength < (long)length)
			{
				this.nextReadCalled = true;
				this.cnc.Close(true);
				throw new WebException("Specified Content-Length is less than the number of bytes to write", null, WebExceptionStatus.ServerProtocolViolation, null);
			}
			AsyncCallback <>9__1;
			this.SetHeadersAsync(true, delegate(SimpleAsyncResult inner)
			{
				if (inner.GotException)
				{
					result.SetCompleted(inner.CompletedSynchronouslyPeek, inner.Exception);
					return;
				}
				if (this.cnc.Data.StatusCode != 0 && this.cnc.Data.StatusCode != 100)
				{
					result.SetCompleted(inner.CompletedSynchronouslyPeek);
					return;
				}
				if (!this.initRead)
				{
					this.initRead = true;
					this.cnc.InitRead();
				}
				if (length == 0)
				{
					this.complete_request_written = true;
					result.SetCompleted(inner.CompletedSynchronouslyPeek);
					return;
				}
				WebConnection webConnection = this.cnc;
				HttpWebRequest httpWebRequest = this.request;
				byte[] bytes2 = bytes;
				int num = 0;
				int length2 = length;
				AsyncCallback asyncCallback;
				if ((asyncCallback = <>9__1) == null)
				{
					asyncCallback = (<>9__1 = delegate(IAsyncResult r)
					{
						try
						{
							this.complete_request_written = this.cnc.EndWrite(this.request, false, r);
							result.SetCompleted(false);
						}
						catch (Exception ex)
						{
							result.SetCompleted(false, ex);
						}
					});
				}
				webConnection.BeginWrite(httpWebRequest, bytes2, num, length2, asyncCallback, null);
			});
			return true;
		}

		internal void InternalClose()
		{
			this.disposed = true;
		}

		internal bool GetResponseOnClose { get; set; }

		public override void Close()
		{
			if (this.GetResponseOnClose)
			{
				if (this.disposed)
				{
					return;
				}
				this.disposed = true;
				HttpWebResponse httpWebResponse = (HttpWebResponse)this.request.GetResponse();
				httpWebResponse.ReadAll();
				httpWebResponse.Close();
				return;
			}
			else if (this.sendChunked)
			{
				if (this.disposed)
				{
					return;
				}
				this.disposed = true;
				if (!this.pending.WaitOne(this.WriteTimeout))
				{
					throw new WebException("The operation has timed out.", WebExceptionStatus.Timeout);
				}
				byte[] bytes = Encoding.ASCII.GetBytes("0\r\n\r\n");
				string text = null;
				this.cnc.Write(this.request, bytes, 0, bytes.Length, ref text);
				return;
			}
			else
			{
				if (this.isRead)
				{
					if (!this.nextReadCalled)
					{
						this.CheckComplete();
						if (!this.nextReadCalled)
						{
							this.nextReadCalled = true;
							this.cnc.Close(true);
						}
					}
					return;
				}
				if (!this.allowBuffering)
				{
					this.complete_request_written = true;
					if (!this.initRead)
					{
						this.initRead = true;
						this.cnc.InitRead();
					}
					return;
				}
				if (this.disposed || this.requestWritten)
				{
					return;
				}
				long num = this.request.ContentLength;
				if (!this.sendChunked && num != -1L && this.totalWritten != num)
				{
					IOException ex = new IOException("Cannot close the stream until all bytes are written");
					this.nextReadCalled = true;
					this.cnc.Close(true);
					throw new WebException("Request was cancelled.", WebExceptionStatus.RequestCanceled, WebExceptionInternalStatus.RequestFatal, ex);
				}
				this.disposed = true;
				return;
			}
		}

		internal void KillBuffer()
		{
			this.writeBuffer = null;
		}

		public override long Seek(long a, SeekOrigin b)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long a)
		{
			throw new NotSupportedException();
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanRead
		{
			get
			{
				return !this.disposed && this.isRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this.disposed && !this.isRead;
			}
		}

		public override long Length
		{
			get
			{
				if (!this.isRead)
				{
					throw new NotSupportedException();
				}
				return (long)this.stream_length;
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		private static byte[] crlf = new byte[] { 13, 10 };

		private bool isRead;

		private WebConnection cnc;

		private HttpWebRequest request;

		private byte[] readBuffer;

		private int readBufferOffset;

		private int readBufferSize;

		private int stream_length;

		private long contentLength;

		private long totalRead;

		internal long totalWritten;

		private bool nextReadCalled;

		private int pendingReads;

		private int pendingWrites;

		private ManualResetEvent pending;

		private bool allowBuffering;

		private bool sendChunked;

		private MemoryStream writeBuffer;

		private bool requestWritten;

		private byte[] headers;

		private bool disposed;

		private bool headersSent;

		private object locker = new object();

		private bool initRead;

		private bool read_eof;

		private bool complete_request_written;

		private int read_timeout;

		private int write_timeout;

		private AsyncCallback cb_wrapper;

		internal bool IgnoreIOErrors;
	}
}
