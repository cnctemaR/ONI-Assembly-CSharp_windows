using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class DelegatedStream : Stream
	{
		protected DelegatedStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this._stream = stream;
			this._netStream = stream as NetworkStream;
		}

		protected Stream BaseStream
		{
			get
			{
				return this._stream;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this._stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this._stream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._stream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				if (!this.CanSeek)
				{
					throw new NotSupportedException("Seeking is not supported on this stream.");
				}
				return this._stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				if (!this.CanSeek)
				{
					throw new NotSupportedException("Seeking is not supported on this stream.");
				}
				return this._stream.Position;
			}
			set
			{
				if (!this.CanSeek)
				{
					throw new NotSupportedException("Seeking is not supported on this stream.");
				}
				this._stream.Position = value;
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("Reading is not supported on this stream.");
			}
			IAsyncResult asyncResult;
			if (this._netStream != null)
			{
				asyncResult = this._netStream.BeginRead(buffer, offset, count, callback, state);
			}
			else
			{
				asyncResult = this._stream.BeginRead(buffer, offset, count, callback, state);
			}
			return asyncResult;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Writing is not supported on this stream.");
			}
			IAsyncResult asyncResult;
			if (this._netStream != null)
			{
				asyncResult = this._netStream.BeginWrite(buffer, offset, count, callback, state);
			}
			else
			{
				asyncResult = this._stream.BeginWrite(buffer, offset, count, callback, state);
			}
			return asyncResult;
		}

		public override void Close()
		{
			this._stream.Close();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("Reading is not supported on this stream.");
			}
			return this._stream.EndRead(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Writing is not supported on this stream.");
			}
			this._stream.EndWrite(asyncResult);
		}

		public override void Flush()
		{
			this._stream.Flush();
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return this._stream.FlushAsync(cancellationToken);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("Reading is not supported on this stream.");
			}
			return this._stream.Read(buffer, offset, count);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("Reading is not supported on this stream.");
			}
			return this._stream.ReadAsync(buffer, offset, count, cancellationToken);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (!this.CanSeek)
			{
				throw new NotSupportedException("Seeking is not supported on this stream.");
			}
			return this._stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			if (!this.CanSeek)
			{
				throw new NotSupportedException("Seeking is not supported on this stream.");
			}
			this._stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Writing is not supported on this stream.");
			}
			this._stream.Write(buffer, offset, count);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Writing is not supported on this stream.");
			}
			return this._stream.WriteAsync(buffer, offset, count, cancellationToken);
		}

		private readonly Stream _stream;

		private readonly NetworkStream _netStream;
	}
}
