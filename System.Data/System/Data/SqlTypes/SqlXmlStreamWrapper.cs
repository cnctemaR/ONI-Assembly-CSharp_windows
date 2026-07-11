using System;
using System.Data.Common;
using System.IO;

namespace System.Data.SqlTypes
{
	internal sealed class SqlXmlStreamWrapper : Stream
	{
		internal SqlXmlStreamWrapper(Stream stream)
		{
			this._stream = stream;
			this._lPosition = 0L;
			this._isClosed = false;
		}

		public override bool CanRead
		{
			get
			{
				return !this.IsStreamClosed() && this._stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return !this.IsStreamClosed() && this._stream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this.IsStreamClosed() && this._stream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				this.ThrowIfStreamClosed("get_Length");
				this.ThrowIfStreamCannotSeek("get_Length");
				return this._stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				this.ThrowIfStreamClosed("get_Position");
				this.ThrowIfStreamCannotSeek("get_Position");
				return this._lPosition;
			}
			set
			{
				this.ThrowIfStreamClosed("set_Position");
				this.ThrowIfStreamCannotSeek("set_Position");
				if (value < 0L || value > this._stream.Length)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._lPosition = value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.ThrowIfStreamClosed("Seek");
			this.ThrowIfStreamCannotSeek("Seek");
			switch (origin)
			{
			case SeekOrigin.Begin:
				if (offset < 0L || offset > this._stream.Length)
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				this._lPosition = offset;
				break;
			case SeekOrigin.Current:
			{
				long num = this._lPosition + offset;
				if (num < 0L || num > this._stream.Length)
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				this._lPosition = num;
				break;
			}
			case SeekOrigin.End:
			{
				long num = this._stream.Length + offset;
				if (num < 0L || num > this._stream.Length)
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				this._lPosition = num;
				break;
			}
			default:
				throw ADP.InvalidSeekOrigin("offset");
			}
			return this._lPosition;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.ThrowIfStreamClosed("Read");
			this.ThrowIfStreamCannotRead("Read");
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this._stream.CanSeek && this._stream.Position != this._lPosition)
			{
				this._stream.Seek(this._lPosition, SeekOrigin.Begin);
			}
			int num = this._stream.Read(buffer, offset, count);
			this._lPosition += (long)num;
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.ThrowIfStreamClosed("Write");
			this.ThrowIfStreamCannotWrite("Write");
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this._stream.CanSeek && this._stream.Position != this._lPosition)
			{
				this._stream.Seek(this._lPosition, SeekOrigin.Begin);
			}
			this._stream.Write(buffer, offset, count);
			this._lPosition += (long)count;
		}

		public override int ReadByte()
		{
			this.ThrowIfStreamClosed("ReadByte");
			this.ThrowIfStreamCannotRead("ReadByte");
			if (this._stream.CanSeek && this._lPosition >= this._stream.Length)
			{
				return -1;
			}
			if (this._stream.CanSeek && this._stream.Position != this._lPosition)
			{
				this._stream.Seek(this._lPosition, SeekOrigin.Begin);
			}
			int num = this._stream.ReadByte();
			this._lPosition += 1L;
			return num;
		}

		public override void WriteByte(byte value)
		{
			this.ThrowIfStreamClosed("WriteByte");
			this.ThrowIfStreamCannotWrite("WriteByte");
			if (this._stream.CanSeek && this._stream.Position != this._lPosition)
			{
				this._stream.Seek(this._lPosition, SeekOrigin.Begin);
			}
			this._stream.WriteByte(value);
			this._lPosition += 1L;
		}

		public override void SetLength(long value)
		{
			this.ThrowIfStreamClosed("SetLength");
			this.ThrowIfStreamCannotSeek("SetLength");
			this._stream.SetLength(value);
			if (this._lPosition > value)
			{
				this._lPosition = value;
			}
		}

		public override void Flush()
		{
			if (this._stream != null)
			{
				this._stream.Flush();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				this._isClosed = true;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void ThrowIfStreamCannotSeek(string method)
		{
			if (!this._stream.CanSeek)
			{
				throw new NotSupportedException(SQLResource.InvalidOpStreamNonSeekable(method));
			}
		}

		private void ThrowIfStreamCannotRead(string method)
		{
			if (!this._stream.CanRead)
			{
				throw new NotSupportedException(SQLResource.InvalidOpStreamNonReadable(method));
			}
		}

		private void ThrowIfStreamCannotWrite(string method)
		{
			if (!this._stream.CanWrite)
			{
				throw new NotSupportedException(SQLResource.InvalidOpStreamNonWritable(method));
			}
		}

		private void ThrowIfStreamClosed(string method)
		{
			if (this.IsStreamClosed())
			{
				throw new ObjectDisposedException(SQLResource.InvalidOpStreamClosed(method));
			}
		}

		private bool IsStreamClosed()
		{
			return this._isClosed || this._stream == null || (!this._stream.CanRead && !this._stream.CanWrite && !this._stream.CanSeek);
		}

		private Stream _stream;

		private long _lPosition;

		private bool _isClosed;
	}
}
