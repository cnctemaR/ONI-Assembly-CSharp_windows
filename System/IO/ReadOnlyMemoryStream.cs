using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	internal sealed class ReadOnlyMemoryStream : Stream
	{
		public ReadOnlyMemoryStream(ReadOnlyMemory<byte> content)
		{
			this._content = content;
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return (long)this._content.Length;
			}
		}

		public override long Position
		{
			get
			{
				return (long)this._position;
			}
			set
			{
				if (value < 0L || value > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._position = (int)value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long num;
			if (origin != SeekOrigin.Begin)
			{
				if (origin != SeekOrigin.Current)
				{
					if (origin != SeekOrigin.End)
					{
						throw new ArgumentOutOfRangeException("origin");
					}
					num = (long)this._content.Length + offset;
				}
				else
				{
					num = (long)this._position + offset;
				}
			}
			else
			{
				num = offset;
			}
			long num2 = num;
			if (num2 > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (num2 < 0L)
			{
				throw new IOException("An attempt was made to move the position before the beginning of the stream.");
			}
			this._position = (int)num2;
			return (long)this._position;
		}

		public unsafe override int ReadByte()
		{
			ReadOnlySpan<byte> span = this._content.Span;
			if (this._position >= span.Length)
			{
				return -1;
			}
			int position = this._position;
			this._position = position + 1;
			return (int)(*span[position]);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			ReadOnlyMemoryStream.ValidateReadArrayArguments(buffer, offset, count);
			return this.Read(new Span<byte>(buffer, offset, count));
		}

		public override int Read(Span<byte> buffer)
		{
			int num = this._content.Length - this._position;
			if (num <= 0 || buffer.Length == 0)
			{
				return 0;
			}
			if (num <= buffer.Length)
			{
				this._content.Span.Slice(this._position).CopyTo(buffer);
				this._position = this._content.Length;
				return num;
			}
			this._content.Span.Slice(this._position, buffer.Length).CopyTo(buffer);
			this._position += buffer.Length;
			return buffer.Length;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			ReadOnlyMemoryStream.ValidateReadArrayArguments(buffer, offset, count);
			if (!cancellationToken.IsCancellationRequested)
			{
				return Task.FromResult<int>(this.Read(new Span<byte>(buffer, offset, count)));
			}
			return Task.FromCanceled<int>(cancellationToken);
		}

		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(this.Read(buffer.Span));
			}
			return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return TaskToApm.Begin(base.ReadAsync(buffer, offset, count), callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		public override void CopyTo(Stream destination, int bufferSize)
		{
			StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
			if (this._content.Length > this._position)
			{
				destination.Write(this._content.Span.Slice(this._position));
			}
		}

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
			if (this._content.Length <= this._position)
			{
				return Task.CompletedTask;
			}
			return destination.WriteAsync(this._content.Slice(this._position), cancellationToken).AsTask();
		}

		public override void Flush()
		{
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		private static void ValidateReadArrayArguments(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || buffer.Length - offset < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
		}

		private readonly ReadOnlyMemory<byte> _content;

		private int _position;
	}
}
