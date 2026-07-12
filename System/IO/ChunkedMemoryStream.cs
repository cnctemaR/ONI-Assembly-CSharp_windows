using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	internal sealed class ChunkedMemoryStream : Stream
	{
		internal ChunkedMemoryStream()
		{
		}

		public byte[] ToArray()
		{
			byte[] array = new byte[this._totalLength];
			int num = 0;
			for (ChunkedMemoryStream.MemoryChunk memoryChunk = this._headChunk; memoryChunk != null; memoryChunk = memoryChunk._next)
			{
				Buffer.BlockCopy(memoryChunk._buffer, 0, array, num, memoryChunk._freeOffset);
				num += memoryChunk._freeOffset;
			}
			return array;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			while (count > 0)
			{
				if (this._currentChunk != null)
				{
					int num = this._currentChunk._buffer.Length - this._currentChunk._freeOffset;
					if (num > 0)
					{
						int num2 = Math.Min(num, count);
						Buffer.BlockCopy(buffer, offset, this._currentChunk._buffer, this._currentChunk._freeOffset, num2);
						count -= num2;
						offset += num2;
						this._totalLength += num2;
						this._currentChunk._freeOffset += num2;
						continue;
					}
				}
				this.AppendChunk((long)count);
			}
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			this.Write(buffer, offset, count);
			return Task.CompletedTask;
		}

		private void AppendChunk(long count)
		{
			int num = ((this._currentChunk != null) ? (this._currentChunk._buffer.Length * 2) : 1024);
			if (count > (long)num)
			{
				num = (int)Math.Min(count, 1048576L);
			}
			ChunkedMemoryStream.MemoryChunk memoryChunk = new ChunkedMemoryStream.MemoryChunk(num);
			if (this._currentChunk == null)
			{
				this._headChunk = (this._currentChunk = memoryChunk);
				return;
			}
			this._currentChunk._next = memoryChunk;
			this._currentChunk = memoryChunk;
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

		public override long Length
		{
			get
			{
				return (long)this._totalLength;
			}
		}

		public override void Flush()
		{
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
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

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			if (this._currentChunk != null)
			{
				throw new NotSupportedException();
			}
			this.AppendChunk(value);
		}

		private ChunkedMemoryStream.MemoryChunk _headChunk;

		private ChunkedMemoryStream.MemoryChunk _currentChunk;

		private const int InitialChunkDefaultSize = 1024;

		private const int MaxChunkSize = 1048576;

		private int _totalLength;

		private sealed class MemoryChunk
		{
			internal MemoryChunk(int bufferSize)
			{
				this._buffer = new byte[bufferSize];
			}

			internal readonly byte[] _buffer;

			internal int _freeOffset;

			internal ChunkedMemoryStream.MemoryChunk _next;
		}
	}
}
