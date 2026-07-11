using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression
{
	public class GZipStream : Stream
	{
		public GZipStream(Stream stream, CompressionMode mode)
			: this(stream, mode, false)
		{
		}

		public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
		{
			this._deflateStream = new DeflateStream(stream, mode, leaveOpen, 31);
		}

		public GZipStream(Stream stream, CompressionLevel compressionLevel)
			: this(stream, compressionLevel, false)
		{
		}

		public GZipStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen)
		{
			this._deflateStream = new DeflateStream(stream, compressionLevel, leaveOpen, 31);
		}

		public override bool CanRead
		{
			get
			{
				DeflateStream deflateStream = this._deflateStream;
				return deflateStream != null && deflateStream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				DeflateStream deflateStream = this._deflateStream;
				return deflateStream != null && deflateStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				DeflateStream deflateStream = this._deflateStream;
				return deflateStream != null && deflateStream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("This operation is not supported.");
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException("This operation is not supported.");
			}
			set
			{
				throw new NotSupportedException("This operation is not supported.");
			}
		}

		public override void Flush()
		{
			this.CheckDeflateStream();
			this._deflateStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("This operation is not supported.");
		}

		public override int ReadByte()
		{
			this.CheckDeflateStream();
			return this._deflateStream.ReadByte();
		}

		public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.ReadAsync(array, offset, count, CancellationToken.None), asyncCallback, asyncState);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		public override int Read(byte[] array, int offset, int count)
		{
			this.CheckDeflateStream();
			return this._deflateStream.Read(array, offset, count);
		}

		public override int Read(Span<byte> destination)
		{
			if (base.GetType() != typeof(GZipStream))
			{
				return base.Read(destination);
			}
			this.CheckDeflateStream();
			return this._deflateStream.ReadCore(destination);
		}

		public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.WriteAsync(array, offset, count, CancellationToken.None), asyncCallback, asyncState);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		public override void Write(byte[] array, int offset, int count)
		{
			this.CheckDeflateStream();
			this._deflateStream.Write(array, offset, count);
		}

		public override void Write(ReadOnlySpan<byte> source)
		{
			if (base.GetType() != typeof(GZipStream))
			{
				base.Write(source);
				return;
			}
			this.CheckDeflateStream();
			this._deflateStream.WriteCore(source);
		}

		public override void CopyTo(Stream destination, int bufferSize)
		{
			this.CheckDeflateStream();
			this._deflateStream.CopyTo(destination, bufferSize);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._deflateStream != null)
				{
					this._deflateStream.Dispose();
				}
				this._deflateStream = null;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public Stream BaseStream
		{
			get
			{
				DeflateStream deflateStream = this._deflateStream;
				if (deflateStream == null)
				{
					return null;
				}
				return deflateStream.BaseStream;
			}
		}

		public override Task<int> ReadAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckDeflateStream();
			return this._deflateStream.ReadAsync(array, offset, count, cancellationToken);
		}

		public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (base.GetType() != typeof(GZipStream))
			{
				return base.ReadAsync(destination, cancellationToken);
			}
			this.CheckDeflateStream();
			return this._deflateStream.ReadAsyncMemory(destination, cancellationToken);
		}

		public override Task WriteAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckDeflateStream();
			return this._deflateStream.WriteAsync(array, offset, count, cancellationToken);
		}

		public override Task WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (base.GetType() != typeof(GZipStream))
			{
				return base.WriteAsync(source, cancellationToken);
			}
			this.CheckDeflateStream();
			return this._deflateStream.WriteAsyncMemory(source, cancellationToken);
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			this.CheckDeflateStream();
			return this._deflateStream.FlushAsync(cancellationToken);
		}

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			this.CheckDeflateStream();
			return this._deflateStream.CopyToAsync(destination, bufferSize, cancellationToken);
		}

		private void CheckDeflateStream()
		{
			if (this._deflateStream == null)
			{
				GZipStream.ThrowStreamClosedException();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowStreamClosedException()
		{
			throw new ObjectDisposedException(null, "Can not access a closed Stream.");
		}

		private DeflateStream _deflateStream;
	}
}
