using System;

namespace System.IO.Compression
{
	internal sealed class WrappedStream : Stream
	{
		internal WrappedStream(Stream baseStream, bool closeBaseStream)
			: this(baseStream, closeBaseStream, null, null)
		{
		}

		private WrappedStream(Stream baseStream, bool closeBaseStream, ZipArchiveEntry entry, Action<ZipArchiveEntry> onClosed)
		{
			this._baseStream = baseStream;
			this._closeBaseStream = closeBaseStream;
			this._onClosed = onClosed;
			this._zipArchiveEntry = entry;
			this._isDisposed = false;
		}

		internal WrappedStream(Stream baseStream, ZipArchiveEntry entry, Action<ZipArchiveEntry> onClosed)
			: this(baseStream, false, entry, onClosed)
		{
		}

		public override long Length
		{
			get
			{
				this.ThrowIfDisposed();
				return this._baseStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				this.ThrowIfDisposed();
				return this._baseStream.Position;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfCantSeek();
				this._baseStream.Position = value;
			}
		}

		public override bool CanRead
		{
			get
			{
				return !this._isDisposed && this._baseStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return !this._isDisposed && this._baseStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this._isDisposed && this._baseStream.CanWrite;
			}
		}

		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString(), "A stream from ZipArchiveEntry has been disposed.");
			}
		}

		private void ThrowIfCantRead()
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("This stream from ZipArchiveEntry does not support reading.");
			}
		}

		private void ThrowIfCantWrite()
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("This stream from ZipArchiveEntry does not support writing.");
			}
		}

		private void ThrowIfCantSeek()
		{
			if (!this.CanSeek)
			{
				throw new NotSupportedException("This stream from ZipArchiveEntry does not support seeking.");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCantRead();
			return this._baseStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCantSeek();
			return this._baseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCantSeek();
			this.ThrowIfCantWrite();
			this._baseStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCantWrite();
			this._baseStream.Write(buffer, offset, count);
		}

		public override void Flush()
		{
			this.ThrowIfDisposed();
			this.ThrowIfCantWrite();
			this._baseStream.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this._isDisposed)
			{
				Action<ZipArchiveEntry> onClosed = this._onClosed;
				if (onClosed != null)
				{
					onClosed(this._zipArchiveEntry);
				}
				if (this._closeBaseStream)
				{
					this._baseStream.Dispose();
				}
				this._isDisposed = true;
			}
			base.Dispose(disposing);
		}

		private readonly Stream _baseStream;

		private readonly bool _closeBaseStream;

		private readonly Action<ZipArchiveEntry> _onClosed;

		private readonly ZipArchiveEntry _zipArchiveEntry;

		private bool _isDisposed;
	}
}
