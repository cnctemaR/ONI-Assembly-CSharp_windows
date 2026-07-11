using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class StreamWriter : TextWriter
	{
		public StreamWriter(Stream stream)
			: this(stream, Encoding.UTF8Unmarked, 1024)
		{
		}

		public StreamWriter(Stream stream, Encoding encoding)
			: this(stream, encoding, 1024)
		{
		}

		public StreamWriter(Stream stream, Encoding encoding, int bufferSize)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("Can not write to stream");
			}
			this.internalStream = stream;
			this.Initialize(encoding, bufferSize);
		}

		public StreamWriter(string path)
			: this(path, false, Encoding.UTF8Unmarked, 4096)
		{
		}

		public StreamWriter(string path, bool append)
			: this(path, append, Encoding.UTF8Unmarked, 4096)
		{
		}

		public StreamWriter(string path, bool append, Encoding encoding)
			: this(path, append, encoding, 4096)
		{
		}

		public StreamWriter(string path, bool append, Encoding encoding, int bufferSize)
		{
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			FileMode fileMode;
			if (append)
			{
				fileMode = FileMode.Append;
			}
			else
			{
				fileMode = FileMode.Create;
			}
			this.internalStream = new FileStream(path, fileMode, FileAccess.Write, FileShare.Read);
			if (append)
			{
				this.internalStream.Position = this.internalStream.Length;
			}
			else
			{
				this.internalStream.SetLength(0L);
			}
			this.Initialize(encoding, bufferSize);
		}

		internal void Initialize(Encoding encoding, int bufferSize)
		{
			this.internalEncoding = encoding;
			this.decode_pos = (this.byte_pos = 0);
			int num = Math.Max(bufferSize, 256);
			this.decode_buf = new char[num];
			this.byte_buf = new byte[encoding.GetMaxByteCount(num)];
			if (this.internalStream.CanSeek && this.internalStream.Position > 0L)
			{
				this.preamble_done = true;
			}
		}

		public virtual bool AutoFlush
		{
			get
			{
				return this.iflush;
			}
			set
			{
				this.iflush = value;
				if (this.iflush)
				{
					this.Flush();
				}
			}
		}

		public virtual Stream BaseStream
		{
			get
			{
				return this.internalStream;
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return this.internalEncoding;
			}
		}

		protected override void Dispose(bool disposing)
		{
			Exception ex = null;
			if (!this.DisposedAlready && disposing && this.internalStream != null)
			{
				try
				{
					this.Flush();
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				this.DisposedAlready = true;
				try
				{
					this.internalStream.Close();
				}
				catch (Exception ex3)
				{
					if (ex == null)
					{
						ex = ex3;
					}
				}
			}
			this.internalStream = null;
			this.byte_buf = null;
			this.internalEncoding = null;
			this.decode_buf = null;
			if (ex != null)
			{
				throw ex;
			}
		}

		public override void Flush()
		{
			if (this.DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			this.Decode();
			if (this.byte_pos > 0)
			{
				this.FlushBytes();
				this.internalStream.Flush();
			}
		}

		private void FlushBytes()
		{
			if (!this.preamble_done && this.byte_pos > 0)
			{
				byte[] preamble = this.internalEncoding.GetPreamble();
				if (preamble.Length > 0)
				{
					this.internalStream.Write(preamble, 0, preamble.Length);
				}
				this.preamble_done = true;
			}
			this.internalStream.Write(this.byte_buf, 0, this.byte_pos);
			this.byte_pos = 0;
		}

		private void Decode()
		{
			if (this.byte_pos > 0)
			{
				this.FlushBytes();
			}
			if (this.decode_pos > 0)
			{
				int bytes = this.internalEncoding.GetBytes(this.decode_buf, 0, this.decode_pos, this.byte_buf, this.byte_pos);
				this.byte_pos += bytes;
				this.decode_pos = 0;
			}
		}

		public override void Write(char[] buffer, int index, int count)
		{
			if (this.DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (index > buffer.Length - count)
			{
				throw new ArgumentException("index + count > buffer.Length");
			}
			this.LowLevelWrite(buffer, index, count);
			if (this.iflush)
			{
				this.Flush();
			}
		}

		private void LowLevelWrite(char[] buffer, int index, int count)
		{
			while (count > 0)
			{
				int num = this.decode_buf.Length - this.decode_pos;
				if (num == 0)
				{
					this.Decode();
					num = this.decode_buf.Length;
				}
				if (num > count)
				{
					num = count;
				}
				Buffer.BlockCopy(buffer, index * 2, this.decode_buf, this.decode_pos * 2, num * 2);
				count -= num;
				index += num;
				this.decode_pos += num;
			}
		}

		private void LowLevelWrite(string s)
		{
			int i = s.Length;
			int num = 0;
			while (i > 0)
			{
				int num2 = this.decode_buf.Length - this.decode_pos;
				if (num2 == 0)
				{
					this.Decode();
					num2 = this.decode_buf.Length;
				}
				if (num2 > i)
				{
					num2 = i;
				}
				for (int j = 0; j < num2; j++)
				{
					this.decode_buf[j + this.decode_pos] = s[j + num];
				}
				i -= num2;
				num += num2;
				this.decode_pos += num2;
			}
		}

		public override void Write(char value)
		{
			if (this.DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			if (this.decode_pos >= this.decode_buf.Length)
			{
				this.Decode();
			}
			this.decode_buf[this.decode_pos++] = value;
			if (this.iflush)
			{
				this.Flush();
			}
		}

		public override void Write(char[] buffer)
		{
			if (this.DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			if (buffer != null)
			{
				this.LowLevelWrite(buffer, 0, buffer.Length);
			}
			if (this.iflush)
			{
				this.Flush();
			}
		}

		public override void Write(string value)
		{
			if (this.DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			if (value != null)
			{
				this.LowLevelWrite(value);
			}
			if (this.iflush)
			{
				this.Flush();
			}
		}

		public override void Close()
		{
			this.Dispose(true);
		}

		~StreamWriter()
		{
			this.Dispose(false);
		}

		private const int DefaultBufferSize = 1024;

		private const int DefaultFileBufferSize = 4096;

		private const int MinimumBufferSize = 256;

		private Encoding internalEncoding;

		private Stream internalStream;

		private bool iflush;

		private byte[] byte_buf;

		private int byte_pos;

		private char[] decode_buf;

		private int decode_pos;

		private bool DisposedAlready;

		private bool preamble_done;

		public new static readonly StreamWriter Null = new StreamWriter(Stream.Null, Encoding.UTF8Unmarked, 1);
	}
}
