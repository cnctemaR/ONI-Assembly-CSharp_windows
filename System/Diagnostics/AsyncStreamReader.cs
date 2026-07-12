using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Diagnostics
{
	internal class AsyncStreamReader : IDisposable
	{
		internal AsyncStreamReader(Process process, Stream stream, UserCallBack callback, Encoding encoding)
			: this(process, stream, callback, encoding, 1024)
		{
		}

		internal AsyncStreamReader(Process process, Stream stream, UserCallBack callback, Encoding encoding, int bufferSize)
		{
			this.Init(process, stream, callback, encoding, bufferSize);
			this.messageQueue = new Queue();
		}

		private void Init(Process process, Stream stream, UserCallBack callback, Encoding encoding, int bufferSize)
		{
			this.process = process;
			this.stream = stream;
			this.encoding = encoding;
			this.userCallBack = callback;
			this.decoder = encoding.GetDecoder();
			if (bufferSize < 128)
			{
				bufferSize = 128;
			}
			this.byteBuffer = new byte[bufferSize];
			this._maxCharsPerBuffer = encoding.GetMaxCharCount(bufferSize);
			this.charBuffer = new char[this._maxCharsPerBuffer];
			this.cancelOperation = false;
			this.eofEvent = new ManualResetEvent(false);
			this.sb = null;
			this.bLastCarriageReturn = false;
		}

		public virtual void Close()
		{
			this.Dispose(true);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			object obj = this.syncObject;
			lock (obj)
			{
				if (disposing && this.stream != null)
				{
					if (this.asyncReadResult != null && !this.asyncReadResult.IsCompleted && this.stream is FileStream)
					{
						SafeHandle safeFileHandle = ((FileStream)this.stream).SafeFileHandle;
						MonoIOError monoIOError;
						while (!this.asyncReadResult.IsCompleted && (MonoIO.Cancel(safeFileHandle, out monoIOError) || monoIOError != MonoIOError.ERROR_NOT_SUPPORTED))
						{
							this.asyncReadResult.AsyncWaitHandle.WaitOne(200);
						}
					}
					this.stream.Close();
				}
				if (this.stream != null)
				{
					this.stream = null;
					this.encoding = null;
					this.decoder = null;
					this.byteBuffer = null;
					this.charBuffer = null;
				}
				if (this.eofEvent != null)
				{
					this.eofEvent.Close();
					this.eofEvent = null;
				}
			}
		}

		public virtual Encoding CurrentEncoding
		{
			get
			{
				return this.encoding;
			}
		}

		public virtual Stream BaseStream
		{
			get
			{
				return this.stream;
			}
		}

		internal void BeginReadLine()
		{
			if (this.cancelOperation)
			{
				this.cancelOperation = false;
			}
			if (this.sb == null)
			{
				this.sb = new StringBuilder(1024);
				this.asyncReadResult = this.stream.BeginRead(this.byteBuffer, 0, this.byteBuffer.Length, new AsyncCallback(this.ReadBuffer), null);
				return;
			}
			this.FlushMessageQueue();
		}

		internal void CancelOperation()
		{
			this.cancelOperation = true;
		}

		private void ReadBuffer(IAsyncResult ar)
		{
			int num;
			try
			{
				object obj = this.syncObject;
				lock (obj)
				{
					this.asyncReadResult = null;
					if (this.stream == null)
					{
						num = 0;
					}
					else
					{
						num = this.stream.EndRead(ar);
					}
				}
			}
			catch (IOException)
			{
				num = 0;
			}
			catch (OperationCanceledException)
			{
				num = 0;
			}
			for (;;)
			{
				object obj;
				if (num == 0)
				{
					Queue queue = this.messageQueue;
					lock (queue)
					{
						if (this.sb.Length != 0)
						{
							this.messageQueue.Enqueue(this.sb.ToString());
							this.sb.Length = 0;
						}
						this.messageQueue.Enqueue(null);
					}
					try
					{
						this.FlushMessageQueue();
						break;
					}
					finally
					{
						obj = this.syncObject;
						lock (obj)
						{
							if (this.eofEvent != null)
							{
								try
								{
									this.eofEvent.Set();
								}
								catch (ObjectDisposedException)
								{
								}
							}
						}
					}
				}
				obj = this.syncObject;
				lock (obj)
				{
					if (this.decoder == null)
					{
						num = 0;
						continue;
					}
					int chars = this.decoder.GetChars(this.byteBuffer, 0, num, this.charBuffer, 0);
					this.sb.Append(this.charBuffer, 0, chars);
				}
				this.GetLinesFromStringBuilder();
				obj = this.syncObject;
				lock (obj)
				{
					if (this.stream == null)
					{
						num = 0;
						continue;
					}
					this.asyncReadResult = this.stream.BeginRead(this.byteBuffer, 0, this.byteBuffer.Length, new AsyncCallback(this.ReadBuffer), null);
				}
				break;
			}
		}

		private void GetLinesFromStringBuilder()
		{
			int i = this.currentLinePos;
			int num = 0;
			int length = this.sb.Length;
			if (this.bLastCarriageReturn && length > 0 && this.sb[0] == '\n')
			{
				i = 1;
				num = 1;
				this.bLastCarriageReturn = false;
			}
			while (i < length)
			{
				char c = this.sb[i];
				if (c == '\r' || c == '\n')
				{
					string text = this.sb.ToString(num, i - num);
					num = i + 1;
					if (c == '\r' && num < length && this.sb[num] == '\n')
					{
						num++;
						i++;
					}
					Queue queue = this.messageQueue;
					lock (queue)
					{
						this.messageQueue.Enqueue(text);
					}
				}
				i++;
			}
			if (this.sb[length - 1] == '\r')
			{
				this.bLastCarriageReturn = true;
			}
			if (num < length)
			{
				if (num == 0)
				{
					this.currentLinePos = i;
				}
				else
				{
					this.sb.Remove(0, num);
					this.currentLinePos = 0;
				}
			}
			else
			{
				this.sb.Length = 0;
				this.currentLinePos = 0;
			}
			this.FlushMessageQueue();
		}

		private void FlushMessageQueue()
		{
			while (this.messageQueue.Count > 0)
			{
				Queue queue = this.messageQueue;
				lock (queue)
				{
					if (this.messageQueue.Count > 0)
					{
						string text = (string)this.messageQueue.Dequeue();
						if (!this.cancelOperation)
						{
							this.userCallBack(text);
						}
					}
					continue;
				}
				break;
			}
		}

		internal void WaitUtilEOF()
		{
			if (this.eofEvent != null)
			{
				this.eofEvent.WaitOne();
				this.eofEvent.Close();
				this.eofEvent = null;
			}
		}

		internal const int DefaultBufferSize = 1024;

		private const int MinBufferSize = 128;

		private Stream stream;

		private Encoding encoding;

		private Decoder decoder;

		private byte[] byteBuffer;

		private char[] charBuffer;

		private int _maxCharsPerBuffer;

		private Process process;

		private UserCallBack userCallBack;

		private bool cancelOperation;

		private ManualResetEvent eofEvent;

		private Queue messageQueue;

		private StringBuilder sb;

		private bool bLastCarriageReturn;

		private int currentLinePos;

		private object syncObject = new object();

		private IAsyncResult asyncReadResult;
	}
}
