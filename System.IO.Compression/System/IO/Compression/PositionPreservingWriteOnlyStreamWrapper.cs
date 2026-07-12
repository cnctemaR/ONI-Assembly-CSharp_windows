using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression
{
	internal sealed class PositionPreservingWriteOnlyStreamWrapper : Stream
	{
		public PositionPreservingWriteOnlyStreamWrapper(Stream stream)
		{
			this._stream = stream;
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Position
		{
			get
			{
				return this._position;
			}
			set
			{
				throw new NotSupportedException("This operation is not supported.");
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this._position += (long)count;
			this._stream.Write(buffer, offset, count);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			this._position += (long)count;
			return this._stream.BeginWrite(buffer, offset, count, callback, state);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			this._stream.EndWrite(asyncResult);
		}

		public override void WriteByte(byte value)
		{
			this._position += 1L;
			this._stream.WriteByte(value);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			this._position += (long)count;
			return this._stream.WriteAsync(buffer, offset, count, cancellationToken);
		}

		public override bool CanTimeout
		{
			get
			{
				return this._stream.CanTimeout;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this._stream.ReadTimeout;
			}
			set
			{
				this._stream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return this._stream.WriteTimeout;
			}
			set
			{
				this._stream.WriteTimeout = value;
			}
		}

		public override void Flush()
		{
			this._stream.Flush();
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return this._stream.FlushAsync(cancellationToken);
		}

		public override void Close()
		{
			this._stream.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._stream.Dispose();
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("This operation is not supported.");
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		private readonly Stream _stream;

		private long _position;
	}
}
