using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Hashing
{
	public abstract class NonCryptographicHashAlgorithm
	{
		public int HashLengthInBytes { get; }

		protected NonCryptographicHashAlgorithm(int hashLengthInBytes)
		{
			if (hashLengthInBytes < 1)
			{
				throw new ArgumentOutOfRangeException("hashLengthInBytes");
			}
			this.HashLengthInBytes = hashLengthInBytes;
		}

		public abstract void Append(ReadOnlySpan<byte> source);

		public abstract void Reset();

		protected abstract void GetCurrentHashCore(Span<byte> destination);

		[NullableContext(1)]
		public void Append(byte[] source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.Append(new ReadOnlySpan<byte>(source));
		}

		[NullableContext(1)]
		public void Append(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			stream.CopyTo(new NonCryptographicHashAlgorithm.CopyToDestinationStream(this));
		}

		[NullableContext(1)]
		public Task AppendAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			return stream.CopyToAsync(new NonCryptographicHashAlgorithm.CopyToDestinationStream(this), 81920, cancellationToken);
		}

		[NullableContext(1)]
		public byte[] GetCurrentHash()
		{
			byte[] array = new byte[this.HashLengthInBytes];
			this.GetCurrentHashCore(array);
			return array;
		}

		public bool TryGetCurrentHash(Span<byte> destination, out int bytesWritten)
		{
			if (destination.Length < this.HashLengthInBytes)
			{
				bytesWritten = 0;
				return false;
			}
			this.GetCurrentHashCore(destination.Slice(0, this.HashLengthInBytes));
			bytesWritten = this.HashLengthInBytes;
			return true;
		}

		public int GetCurrentHash(Span<byte> destination)
		{
			if (destination.Length < this.HashLengthInBytes)
			{
				NonCryptographicHashAlgorithm.ThrowDestinationTooShort();
			}
			this.GetCurrentHashCore(destination.Slice(0, this.HashLengthInBytes));
			return this.HashLengthInBytes;
		}

		[NullableContext(1)]
		public byte[] GetHashAndReset()
		{
			byte[] array = new byte[this.HashLengthInBytes];
			this.GetHashAndResetCore(array);
			return array;
		}

		public bool TryGetHashAndReset(Span<byte> destination, out int bytesWritten)
		{
			if (destination.Length < this.HashLengthInBytes)
			{
				bytesWritten = 0;
				return false;
			}
			this.GetHashAndResetCore(destination.Slice(0, this.HashLengthInBytes));
			bytesWritten = this.HashLengthInBytes;
			return true;
		}

		public int GetHashAndReset(Span<byte> destination)
		{
			if (destination.Length < this.HashLengthInBytes)
			{
				NonCryptographicHashAlgorithm.ThrowDestinationTooShort();
			}
			this.GetHashAndResetCore(destination.Slice(0, this.HashLengthInBytes));
			return this.HashLengthInBytes;
		}

		protected virtual void GetHashAndResetCore(Span<byte> destination)
		{
			this.GetCurrentHashCore(destination);
			this.Reset();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use GetCurrentHash() to retrieve the computed hash code.", true)]
		public override int GetHashCode()
		{
			throw new NotSupportedException(SR.NotSupported_GetHashCode);
		}

		[DoesNotReturn]
		private protected static void ThrowDestinationTooShort()
		{
			throw new ArgumentException(SR.Argument_DestinationTooShort, "destination");
		}

		private sealed class CopyToDestinationStream : Stream
		{
			public CopyToDestinationStream(NonCryptographicHashAlgorithm hash)
			{
			}

			public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this.<hash>P.Append(MemoryExtensions.AsSpan<byte>(buffer, offset, count));
			}

			public override void WriteByte(byte value)
			{
				this.<hash>P.Append(new ReadOnlySpan<byte>(new byte[] { value }));
			}

			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				this.<hash>P.Append(MemoryExtensions.AsSpan<byte>(buffer, offset, count));
				return Task.CompletedTask;
			}

			public override void Flush()
			{
			}

			public override Task FlushAsync(CancellationToken cancellationToken)
			{
				return Task.CompletedTask;
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

			public override long Length
			{
				get
				{
					throw new NotSupportedException();
				}
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
				throw new NotSupportedException();
			}

			[CompilerGenerated]
			private NonCryptographicHashAlgorithm <hash>P = hash;
		}
	}
}
