using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class StreamWriter : TextWriter
	{
		private void CheckAsyncTaskInProgress()
		{
			Task asyncWriteTask = this._asyncWriteTask;
			if (asyncWriteTask != null && !asyncWriteTask.IsCompleted)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The stream is currently in use by a previous operation on the stream."));
			}
		}

		internal static Encoding UTF8NoBOM
		{
			[FriendAccessAllowed]
			get
			{
				if (StreamWriter._UTF8NoBOM == null)
				{
					Encoding encoding = new UTF8Encoding(false, true);
					Thread.MemoryBarrier();
					StreamWriter._UTF8NoBOM = encoding;
				}
				return StreamWriter._UTF8NoBOM;
			}
		}

		internal StreamWriter()
			: base(null)
		{
		}

		public StreamWriter(Stream stream)
			: this(stream, StreamWriter.UTF8NoBOM, 1024, false)
		{
		}

		public StreamWriter(Stream stream, Encoding encoding)
			: this(stream, encoding, 1024, false)
		{
		}

		public StreamWriter(Stream stream, Encoding encoding, int bufferSize)
			: this(stream, encoding, bufferSize, false)
		{
		}

		public StreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
			: base(null)
		{
			if (stream == null || encoding == null)
			{
				throw new ArgumentNullException((stream == null) ? "stream" : "encoding");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException(Environment.GetResourceString("Stream was not writable."));
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("Positive number required."));
			}
			this.Init(stream, encoding, bufferSize, leaveOpen);
		}

		public StreamWriter(string path)
			: this(path, false, StreamWriter.UTF8NoBOM, 1024)
		{
		}

		public StreamWriter(string path, bool append)
			: this(path, append, StreamWriter.UTF8NoBOM, 1024)
		{
		}

		public StreamWriter(string path, bool append, Encoding encoding)
			: this(path, append, encoding, 1024)
		{
		}

		[SecuritySafeCritical]
		public StreamWriter(string path, bool append, Encoding encoding, int bufferSize)
			: this(path, append, encoding, bufferSize, true)
		{
		}

		[SecurityCritical]
		internal StreamWriter(string path, bool append, Encoding encoding, int bufferSize, bool checkHost)
			: base(null)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Empty path name is not legal."));
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("Positive number required."));
			}
			Stream stream = StreamWriter.CreateFile(path, append, checkHost);
			this.Init(stream, encoding, bufferSize, false);
		}

		[SecuritySafeCritical]
		private void Init(Stream streamArg, Encoding encodingArg, int bufferSize, bool shouldLeaveOpen)
		{
			this.stream = streamArg;
			this.encoding = encodingArg;
			this.encoder = this.encoding.GetEncoder();
			if (bufferSize < 128)
			{
				bufferSize = 128;
			}
			this.charBuffer = new char[bufferSize];
			this.byteBuffer = new byte[this.encoding.GetMaxByteCount(bufferSize)];
			this.charLen = bufferSize;
			if (this.stream.CanSeek && this.stream.Position > 0L)
			{
				this.haveWrittenPreamble = true;
			}
			this.closable = !shouldLeaveOpen;
		}

		[SecurityCritical]
		private static Stream CreateFile(string path, bool append, bool checkHost)
		{
			FileMode fileMode = (append ? FileMode.Append : FileMode.Create);
			return new FileStream(path, fileMode, FileAccess.Write, FileShare.Read, 4096, FileOptions.SequentialScan, Path.GetFileName(path), false, false, checkHost);
		}

		public override void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this.stream != null)
				{
					if (!disposing)
					{
						if (this.LeaveOpen)
						{
							Stream stream = this.stream;
						}
					}
					else
					{
						this.CheckAsyncTaskInProgress();
						this.Flush(true, true);
					}
				}
			}
			finally
			{
				if (!this.LeaveOpen && this.stream != null)
				{
					try
					{
						if (disposing)
						{
							this.stream.Close();
						}
					}
					finally
					{
						this.stream = null;
						this.byteBuffer = null;
						this.charBuffer = null;
						this.encoding = null;
						this.encoder = null;
						this.charLen = 0;
						base.Dispose(disposing);
					}
				}
			}
		}

		public override void Flush()
		{
			this.CheckAsyncTaskInProgress();
			this.Flush(true, true);
		}

		private void Flush(bool flushStream, bool flushEncoder)
		{
			if (this.stream == null)
			{
				__Error.WriterClosed();
			}
			if (this.charPos == 0 && ((!flushStream && !flushEncoder) || CompatibilitySwitches.IsAppEarlierThanWindowsPhone8))
			{
				return;
			}
			if (!this.haveWrittenPreamble)
			{
				this.haveWrittenPreamble = true;
				byte[] preamble = this.encoding.GetPreamble();
				if (preamble.Length != 0)
				{
					this.stream.Write(preamble, 0, preamble.Length);
				}
			}
			int bytes = this.encoder.GetBytes(this.charBuffer, 0, this.charPos, this.byteBuffer, 0, flushEncoder);
			this.charPos = 0;
			if (bytes > 0)
			{
				this.stream.Write(this.byteBuffer, 0, bytes);
			}
			if (flushStream)
			{
				this.stream.Flush();
			}
		}

		public virtual bool AutoFlush
		{
			get
			{
				return this.autoFlush;
			}
			set
			{
				this.CheckAsyncTaskInProgress();
				this.autoFlush = value;
				if (value)
				{
					this.Flush(true, false);
				}
			}
		}

		public virtual Stream BaseStream
		{
			get
			{
				return this.stream;
			}
		}

		internal bool LeaveOpen
		{
			get
			{
				return !this.closable;
			}
		}

		internal bool HaveWrittenPreamble
		{
			set
			{
				this.haveWrittenPreamble = value;
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
		}

		public override void Write(char value)
		{
			this.CheckAsyncTaskInProgress();
			if (this.charPos == this.charLen)
			{
				this.Flush(false, false);
			}
			this.charBuffer[this.charPos] = value;
			this.charPos++;
			if (this.autoFlush)
			{
				this.Flush(true, false);
			}
		}

		public override void Write(char[] buffer)
		{
			if (buffer == null)
			{
				return;
			}
			this.CheckAsyncTaskInProgress();
			int num = 0;
			int num2;
			for (int i = buffer.Length; i > 0; i -= num2)
			{
				if (this.charPos == this.charLen)
				{
					this.Flush(false, false);
				}
				num2 = this.charLen - this.charPos;
				if (num2 > i)
				{
					num2 = i;
				}
				Buffer.InternalBlockCopy(buffer, num * 2, this.charBuffer, this.charPos * 2, num2 * 2);
				this.charPos += num2;
				num += num2;
			}
			if (this.autoFlush)
			{
				this.Flush(true, false);
			}
		}

		public override void Write(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			this.CheckAsyncTaskInProgress();
			while (count > 0)
			{
				if (this.charPos == this.charLen)
				{
					this.Flush(false, false);
				}
				int num = this.charLen - this.charPos;
				if (num > count)
				{
					num = count;
				}
				Buffer.InternalBlockCopy(buffer, index * 2, this.charBuffer, this.charPos * 2, num * 2);
				this.charPos += num;
				index += num;
				count -= num;
			}
			if (this.autoFlush)
			{
				this.Flush(true, false);
			}
		}

		public override void Write(string value)
		{
			if (value != null)
			{
				this.CheckAsyncTaskInProgress();
				int i = value.Length;
				int num = 0;
				while (i > 0)
				{
					if (this.charPos == this.charLen)
					{
						this.Flush(false, false);
					}
					int num2 = this.charLen - this.charPos;
					if (num2 > i)
					{
						num2 = i;
					}
					value.CopyTo(num, this.charBuffer, this.charPos, num2);
					this.charPos += num2;
					num += num2;
					i -= num2;
				}
				if (this.autoFlush)
				{
					this.Flush(true, false);
				}
			}
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteAsync(char value)
		{
			if (base.GetType() != typeof(StreamWriter))
			{
				return base.WriteAsync(value);
			}
			if (this.stream == null)
			{
				__Error.WriterClosed();
			}
			this.CheckAsyncTaskInProgress();
			Task task = StreamWriter.WriteAsyncInternal(this, value, this.charBuffer, this.charPos, this.charLen, this.CoreNewLine, this.autoFlush, false);
			this._asyncWriteTask = task;
			return task;
		}

		private static async Task WriteAsyncInternal(StreamWriter _this, char value, char[] charBuffer, int charPos, int charLen, char[] coreNewLine, bool autoFlush, bool appendNewLine)
		{
			if (charPos == charLen)
			{
				await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
				charPos = 0;
			}
			charBuffer[charPos] = value;
			charPos++;
			if (appendNewLine)
			{
				for (int i = 0; i < coreNewLine.Length; i++)
				{
					if (charPos == charLen)
					{
						await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
						charPos = 0;
					}
					charBuffer[charPos] = coreNewLine[i];
					charPos++;
				}
			}
			if (autoFlush)
			{
				await _this.FlushAsyncInternal(true, false, charBuffer, charPos).ConfigureAwait(false);
				charPos = 0;
			}
			_this.CharPos_Prop = charPos;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteAsync(string value)
		{
			if (base.GetType() != typeof(StreamWriter))
			{
				return base.WriteAsync(value);
			}
			if (value != null)
			{
				if (this.stream == null)
				{
					__Error.WriterClosed();
				}
				this.CheckAsyncTaskInProgress();
				Task task = StreamWriter.WriteAsyncInternal(this, value, this.charBuffer, this.charPos, this.charLen, this.CoreNewLine, this.autoFlush, false);
				this._asyncWriteTask = task;
				return task;
			}
			return Task.CompletedTask;
		}

		private static async Task WriteAsyncInternal(StreamWriter _this, string value, char[] charBuffer, int charPos, int charLen, char[] coreNewLine, bool autoFlush, bool appendNewLine)
		{
			int count = value.Length;
			int index = 0;
			while (count > 0)
			{
				if (charPos == charLen)
				{
					await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
					charPos = 0;
				}
				int num = charLen - charPos;
				if (num > count)
				{
					num = count;
				}
				value.CopyTo(index, charBuffer, charPos, num);
				charPos += num;
				index += num;
				count -= num;
			}
			if (appendNewLine)
			{
				for (int i = 0; i < coreNewLine.Length; i++)
				{
					if (charPos == charLen)
					{
						await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
						charPos = 0;
					}
					charBuffer[charPos] = coreNewLine[i];
					charPos++;
				}
			}
			if (autoFlush)
			{
				await _this.FlushAsyncInternal(true, false, charBuffer, charPos).ConfigureAwait(false);
				charPos = 0;
			}
			_this.CharPos_Prop = charPos;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteAsync(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (base.GetType() != typeof(StreamWriter))
			{
				return base.WriteAsync(buffer, index, count);
			}
			if (this.stream == null)
			{
				__Error.WriterClosed();
			}
			this.CheckAsyncTaskInProgress();
			Task task = StreamWriter.WriteAsyncInternal(this, buffer, index, count, this.charBuffer, this.charPos, this.charLen, this.CoreNewLine, this.autoFlush, false);
			this._asyncWriteTask = task;
			return task;
		}

		private static async Task WriteAsyncInternal(StreamWriter _this, char[] buffer, int index, int count, char[] charBuffer, int charPos, int charLen, char[] coreNewLine, bool autoFlush, bool appendNewLine)
		{
			while (count > 0)
			{
				if (charPos == charLen)
				{
					await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
					charPos = 0;
				}
				int num = charLen - charPos;
				if (num > count)
				{
					num = count;
				}
				Buffer.InternalBlockCopy(buffer, index * 2, charBuffer, charPos * 2, num * 2);
				charPos += num;
				index += num;
				count -= num;
			}
			if (appendNewLine)
			{
				for (int i = 0; i < coreNewLine.Length; i++)
				{
					if (charPos == charLen)
					{
						await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
						charPos = 0;
					}
					charBuffer[charPos] = coreNewLine[i];
					charPos++;
				}
			}
			if (autoFlush)
			{
				await _this.FlushAsyncInternal(true, false, charBuffer, charPos).ConfigureAwait(false);
				charPos = 0;
			}
			_this.CharPos_Prop = charPos;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteLineAsync()
		{
			if (base.GetType() != typeof(StreamWriter))
			{
				return base.WriteLineAsync();
			}
			if (this.stream == null)
			{
				__Error.WriterClosed();
			}
			this.CheckAsyncTaskInProgress();
			Task task = StreamWriter.WriteAsyncInternal(this, null, 0, 0, this.charBuffer, this.charPos, this.charLen, this.CoreNewLine, this.autoFlush, true);
			this._asyncWriteTask = task;
			return task;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteLineAsync(char value)
		{
			if (base.GetType() != typeof(StreamWriter))
			{
				return base.WriteLineAsync(value);
			}
			if (this.stream == null)
			{
				__Error.WriterClosed();
			}
			this.CheckAsyncTaskInProgress();
			Task task = StreamWriter.WriteAsyncInternal(this, value, this.charBuffer, this.charPos, this.charLen, this.CoreNewLine, this.autoFlush, true);
			this._asyncWriteTask = task;
			return task;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteLineAsync(string value)
		{
			if (base.GetType() != typeof(StreamWriter))
			{
				return base.WriteLineAsync(value);
			}
			if (this.stream == null)
			{
				__Error.WriterClosed();
			}
			this.CheckAsyncTaskInProgress();
			Task task = StreamWriter.WriteAsyncInternal(this, value ?? "", this.charBuffer, this.charPos, this.charLen, this.CoreNewLine, this.autoFlush, true);
			this._asyncWriteTask = task;
			return task;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteLineAsync(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (base.GetType() != typeof(StreamWriter))
			{
				return base.WriteLineAsync(buffer, index, count);
			}
			if (this.stream == null)
			{
				__Error.WriterClosed();
			}
			this.CheckAsyncTaskInProgress();
			Task task = StreamWriter.WriteAsyncInternal(this, buffer, index, count, this.charBuffer, this.charPos, this.charLen, this.CoreNewLine, this.autoFlush, true);
			this._asyncWriteTask = task;
			return task;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task FlushAsync()
		{
			if (base.GetType() != typeof(StreamWriter))
			{
				return base.FlushAsync();
			}
			if (this.stream == null)
			{
				__Error.WriterClosed();
			}
			this.CheckAsyncTaskInProgress();
			Task task = this.FlushAsyncInternal(true, true, this.charBuffer, this.charPos);
			this._asyncWriteTask = task;
			return task;
		}

		private int CharPos_Prop
		{
			set
			{
				this.charPos = value;
			}
		}

		private bool HaveWrittenPreamble_Prop
		{
			set
			{
				this.haveWrittenPreamble = value;
			}
		}

		private Task FlushAsyncInternal(bool flushStream, bool flushEncoder, char[] sCharBuffer, int sCharPos)
		{
			if (sCharPos == 0 && !flushStream && !flushEncoder)
			{
				return Task.CompletedTask;
			}
			Task task = StreamWriter.FlushAsyncInternal(this, flushStream, flushEncoder, sCharBuffer, sCharPos, this.haveWrittenPreamble, this.encoding, this.encoder, this.byteBuffer, this.stream);
			this.charPos = 0;
			return task;
		}

		private static async Task FlushAsyncInternal(StreamWriter _this, bool flushStream, bool flushEncoder, char[] charBuffer, int charPos, bool haveWrittenPreamble, Encoding encoding, Encoder encoder, byte[] byteBuffer, Stream stream)
		{
			if (!haveWrittenPreamble)
			{
				_this.HaveWrittenPreamble_Prop = true;
				byte[] preamble = encoding.GetPreamble();
				if (preamble.Length != 0)
				{
					await stream.WriteAsync(preamble, 0, preamble.Length).ConfigureAwait(false);
				}
			}
			int bytes = encoder.GetBytes(charBuffer, 0, charPos, byteBuffer, 0, flushEncoder);
			if (bytes > 0)
			{
				await stream.WriteAsync(byteBuffer, 0, bytes).ConfigureAwait(false);
			}
			if (flushStream)
			{
				await stream.FlushAsync().ConfigureAwait(false);
			}
		}

		internal const int DefaultBufferSize = 1024;

		private const int DefaultFileStreamBufferSize = 4096;

		private const int MinBufferSize = 128;

		private const int DontCopyOnWriteLineThreshold = 512;

		public new static readonly StreamWriter Null = new StreamWriter(Stream.Null, new UTF8Encoding(false, true), 128, true);

		private Stream stream;

		private Encoding encoding;

		private Encoder encoder;

		private byte[] byteBuffer;

		private char[] charBuffer;

		private int charPos;

		private int charLen;

		private bool autoFlush;

		private bool haveWrittenPreamble;

		private bool closable;

		[NonSerialized]
		private volatile Task _asyncWriteTask;

		private static volatile Encoding _UTF8NoBOM;
	}
}
