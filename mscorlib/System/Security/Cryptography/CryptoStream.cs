using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Security.Cryptography
{
	public class CryptoStream : Stream, IDisposable
	{
		public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
			: this(stream, transform, mode, false)
		{
		}

		public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode, bool leaveOpen)
		{
			this._stream = stream;
			this._transformMode = mode;
			this._transform = transform;
			this._leaveOpen = leaveOpen;
			CryptoStreamMode transformMode = this._transformMode;
			if (transformMode != CryptoStreamMode.Read)
			{
				if (transformMode != CryptoStreamMode.Write)
				{
					throw new ArgumentException("Argument {0} should be larger than {1}.");
				}
				if (!this._stream.CanWrite)
				{
					throw new ArgumentException(SR.Format("Stream was not writable.", "stream"));
				}
				this._canWrite = true;
			}
			else
			{
				if (!this._stream.CanRead)
				{
					throw new ArgumentException(SR.Format("Stream was not readable.", "stream"));
				}
				this._canRead = true;
			}
			this.InitializeBuffer();
		}

		public override bool CanRead
		{
			get
			{
				return this._canRead;
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
				return this._canWrite;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("Stream does not support seeking.");
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException("Stream does not support seeking.");
			}
			set
			{
				throw new NotSupportedException("Stream does not support seeking.");
			}
		}

		public bool HasFlushedFinalBlock
		{
			get
			{
				return this._finalBlockTransformed;
			}
		}

		public void FlushFinalBlock()
		{
			if (this._finalBlockTransformed)
			{
				throw new NotSupportedException("FlushFinalBlock() method was called twice on a CryptoStream. It can only be called once.");
			}
			byte[] array = this._transform.TransformFinalBlock(this._inputBuffer, 0, this._inputBufferIndex);
			this._finalBlockTransformed = true;
			if (this._canWrite && this._outputBufferIndex > 0)
			{
				this._stream.Write(this._outputBuffer, 0, this._outputBufferIndex);
				this._outputBufferIndex = 0;
			}
			if (this._canWrite)
			{
				this._stream.Write(array, 0, array.Length);
			}
			CryptoStream cryptoStream = this._stream as CryptoStream;
			if (cryptoStream != null)
			{
				if (!cryptoStream.HasFlushedFinalBlock)
				{
					cryptoStream.FlushFinalBlock();
				}
			}
			else
			{
				this._stream.Flush();
			}
			if (this._inputBuffer != null)
			{
				Array.Clear(this._inputBuffer, 0, this._inputBuffer.Length);
			}
			if (this._outputBuffer != null)
			{
				Array.Clear(this._outputBuffer, 0, this._outputBuffer.Length);
			}
		}

		public override void Flush()
		{
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			if (base.GetType() != typeof(CryptoStream))
			{
				return base.FlushAsync(cancellationToken);
			}
			if (!cancellationToken.IsCancellationRequested)
			{
				return Task.CompletedTask;
			}
			return Task.FromCanceled(cancellationToken);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Stream does not support seeking.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("Stream does not support seeking.");
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckReadArguments(buffer, offset, count);
			return this.ReadAsyncInternal(buffer, offset, count, cancellationToken);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return TaskToApm.Begin(this.ReadAsync(buffer, offset, count, CancellationToken.None), callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		private async Task<int> ReadAsyncInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			SemaphoreSlim semaphore = this.AsyncActiveSemaphore;
			await semaphore.WaitAsync().ForceAsync();
			int num;
			try
			{
				num = await this.ReadAsyncCore(buffer, offset, count, cancellationToken, true);
			}
			finally
			{
				semaphore.Release();
			}
			return num;
		}

		public override int ReadByte()
		{
			if (this._outputBufferIndex > 1)
			{
				int num = (int)this._outputBuffer[0];
				Buffer.BlockCopy(this._outputBuffer, 1, this._outputBuffer, 0, this._outputBufferIndex - 1);
				this._outputBufferIndex--;
				return num;
			}
			return base.ReadByte();
		}

		public override void WriteByte(byte value)
		{
			if (this._inputBufferIndex + 1 < this._inputBlockSize)
			{
				byte[] inputBuffer = this._inputBuffer;
				int inputBufferIndex = this._inputBufferIndex;
				this._inputBufferIndex = inputBufferIndex + 1;
				inputBuffer[inputBufferIndex] = value;
				return;
			}
			base.WriteByte(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckReadArguments(buffer, offset, count);
			return this.ReadAsyncCore(buffer, offset, count, default(CancellationToken), false).GetAwaiter().GetResult();
		}

		private void CheckReadArguments(byte[] buffer, int offset, int count)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("Stream does not support reading.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
		}

		private async Task<int> ReadAsyncCore(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool useAsync)
		{
			int bytesToDeliver = count;
			int currentOutputIndex = offset;
			if (this._outputBufferIndex != 0)
			{
				if (this._outputBufferIndex > count)
				{
					Buffer.BlockCopy(this._outputBuffer, 0, buffer, offset, count);
					Buffer.BlockCopy(this._outputBuffer, count, this._outputBuffer, 0, this._outputBufferIndex - count);
					this._outputBufferIndex -= count;
					int num = this._outputBuffer.Length - this._outputBufferIndex;
					CryptographicOperations.ZeroMemory(new Span<byte>(this._outputBuffer, this._outputBufferIndex, num));
					return count;
				}
				Buffer.BlockCopy(this._outputBuffer, 0, buffer, offset, this._outputBufferIndex);
				bytesToDeliver -= this._outputBufferIndex;
				currentOutputIndex += this._outputBufferIndex;
				int num2 = this._outputBuffer.Length - this._outputBufferIndex;
				CryptographicOperations.ZeroMemory(new Span<byte>(this._outputBuffer, this._outputBufferIndex, num2));
				this._outputBufferIndex = 0;
			}
			int num3;
			if (this._finalBlockTransformed)
			{
				num3 = count - bytesToDeliver;
			}
			else
			{
				int num4 = bytesToDeliver / this._outputBlockSize;
				if (num4 > 1 && this._transform.CanTransformMultipleBlocks)
				{
					int numWholeBlocksInBytes = num4 * this._inputBlockSize;
					byte[] tempInputBuffer = ArrayPool<byte>.Shared.Rent(numWholeBlocksInBytes);
					byte[] tempOutputBuffer = null;
					try
					{
						int num5;
						if (useAsync)
						{
							num5 = await this._stream.ReadAsync(new Memory<byte>(tempInputBuffer, this._inputBufferIndex, numWholeBlocksInBytes - this._inputBufferIndex), cancellationToken);
						}
						else
						{
							num5 = this._stream.Read(tempInputBuffer, this._inputBufferIndex, numWholeBlocksInBytes - this._inputBufferIndex);
						}
						int num6 = num5;
						int num7 = this._inputBufferIndex + num6;
						if (num7 < this._inputBlockSize)
						{
							Buffer.BlockCopy(tempInputBuffer, this._inputBufferIndex, this._inputBuffer, this._inputBufferIndex, num6);
							this._inputBufferIndex = num7;
						}
						else
						{
							Buffer.BlockCopy(this._inputBuffer, 0, tempInputBuffer, 0, this._inputBufferIndex);
							CryptographicOperations.ZeroMemory(new Span<byte>(this._inputBuffer, 0, this._inputBufferIndex));
							num6 += this._inputBufferIndex;
							this._inputBufferIndex = 0;
							int num8 = num6 / this._inputBlockSize;
							int num9 = num8 * this._inputBlockSize;
							int num10 = num6 - num9;
							if (num10 != 0)
							{
								this._inputBufferIndex = num10;
								Buffer.BlockCopy(tempInputBuffer, num9, this._inputBuffer, 0, num10);
							}
							tempOutputBuffer = ArrayPool<byte>.Shared.Rent(num8 * this._outputBlockSize);
							int num11 = this._transform.TransformBlock(tempInputBuffer, 0, num9, tempOutputBuffer, 0);
							Buffer.BlockCopy(tempOutputBuffer, 0, buffer, currentOutputIndex, num11);
							CryptographicOperations.ZeroMemory(new Span<byte>(tempOutputBuffer, 0, num11));
							ArrayPool<byte>.Shared.Return(tempOutputBuffer, false);
							tempOutputBuffer = null;
							bytesToDeliver -= num11;
							currentOutputIndex += num11;
						}
					}
					finally
					{
						if (tempOutputBuffer != null)
						{
							CryptographicOperations.ZeroMemory(tempOutputBuffer);
							ArrayPool<byte>.Shared.Return(tempOutputBuffer, false);
							tempOutputBuffer = null;
						}
						CryptographicOperations.ZeroMemory(new Span<byte>(tempInputBuffer, 0, numWholeBlocksInBytes));
						ArrayPool<byte>.Shared.Return(tempInputBuffer, false);
						tempInputBuffer = null;
					}
					tempInputBuffer = null;
					tempOutputBuffer = null;
				}
				while (bytesToDeliver > 0)
				{
					while (this._inputBufferIndex < this._inputBlockSize)
					{
						int num5;
						if (useAsync)
						{
							num5 = await this._stream.ReadAsync(new Memory<byte>(this._inputBuffer, this._inputBufferIndex, this._inputBlockSize - this._inputBufferIndex), cancellationToken);
						}
						else
						{
							num5 = this._stream.Read(this._inputBuffer, this._inputBufferIndex, this._inputBlockSize - this._inputBufferIndex);
						}
						int num6 = num5;
						if (num6 != 0)
						{
							this._inputBufferIndex += num6;
						}
						else
						{
							byte[] array = this._transform.TransformFinalBlock(this._inputBuffer, 0, this._inputBufferIndex);
							this._outputBuffer = array;
							this._outputBufferIndex = array.Length;
							this._finalBlockTransformed = true;
							if (bytesToDeliver < this._outputBufferIndex)
							{
								Buffer.BlockCopy(this._outputBuffer, 0, buffer, currentOutputIndex, bytesToDeliver);
								this._outputBufferIndex -= bytesToDeliver;
								Buffer.BlockCopy(this._outputBuffer, bytesToDeliver, this._outputBuffer, 0, this._outputBufferIndex);
								int num12 = this._outputBuffer.Length - this._outputBufferIndex;
								CryptographicOperations.ZeroMemory(new Span<byte>(this._outputBuffer, this._outputBufferIndex, num12));
								return count;
							}
							Buffer.BlockCopy(this._outputBuffer, 0, buffer, currentOutputIndex, this._outputBufferIndex);
							bytesToDeliver -= this._outputBufferIndex;
							this._outputBufferIndex = 0;
							CryptographicOperations.ZeroMemory(this._outputBuffer);
							return count - bytesToDeliver;
						}
					}
					int num11 = this._transform.TransformBlock(this._inputBuffer, 0, this._inputBlockSize, this._outputBuffer, 0);
					this._inputBufferIndex = 0;
					if (bytesToDeliver < num11)
					{
						Buffer.BlockCopy(this._outputBuffer, 0, buffer, currentOutputIndex, bytesToDeliver);
						this._outputBufferIndex = num11 - bytesToDeliver;
						Buffer.BlockCopy(this._outputBuffer, bytesToDeliver, this._outputBuffer, 0, this._outputBufferIndex);
						int num13 = this._outputBuffer.Length - this._outputBufferIndex;
						CryptographicOperations.ZeroMemory(new Span<byte>(this._outputBuffer, this._outputBufferIndex, num13));
						return count;
					}
					Buffer.BlockCopy(this._outputBuffer, 0, buffer, currentOutputIndex, num11);
					CryptographicOperations.ZeroMemory(new Span<byte>(this._outputBuffer, 0, num11));
					currentOutputIndex += num11;
					bytesToDeliver -= num11;
				}
				num3 = count;
			}
			return num3;
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckWriteArguments(buffer, offset, count);
			return this.WriteAsyncInternal(buffer, offset, count, cancellationToken);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return TaskToApm.Begin(this.WriteAsync(buffer, offset, count, CancellationToken.None), callback, state);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		private async Task WriteAsyncInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			SemaphoreSlim semaphore = this.AsyncActiveSemaphore;
			await semaphore.WaitAsync().ForceAsync();
			try
			{
				await this.WriteAsyncCore(buffer, offset, count, cancellationToken, true);
			}
			finally
			{
				semaphore.Release();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckWriteArguments(buffer, offset, count);
			this.WriteAsyncCore(buffer, offset, count, default(CancellationToken), false).GetAwaiter().GetResult();
		}

		private void CheckWriteArguments(byte[] buffer, int offset, int count)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
		}

		private async Task WriteAsyncCore(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool useAsync)
		{
			int bytesToWrite = count;
			int currentInputIndex = offset;
			if (this._inputBufferIndex > 0)
			{
				if (count < this._inputBlockSize - this._inputBufferIndex)
				{
					Buffer.BlockCopy(buffer, offset, this._inputBuffer, this._inputBufferIndex, count);
					this._inputBufferIndex += count;
					return;
				}
				Buffer.BlockCopy(buffer, offset, this._inputBuffer, this._inputBufferIndex, this._inputBlockSize - this._inputBufferIndex);
				currentInputIndex += this._inputBlockSize - this._inputBufferIndex;
				bytesToWrite -= this._inputBlockSize - this._inputBufferIndex;
				this._inputBufferIndex = this._inputBlockSize;
			}
			if (this._outputBufferIndex > 0)
			{
				if (useAsync)
				{
					await this._stream.WriteAsync(new ReadOnlyMemory<byte>(this._outputBuffer, 0, this._outputBufferIndex), cancellationToken);
				}
				else
				{
					this._stream.Write(this._outputBuffer, 0, this._outputBufferIndex);
				}
				this._outputBufferIndex = 0;
			}
			if (this._inputBufferIndex == this._inputBlockSize)
			{
				int numOutputBytes = this._transform.TransformBlock(this._inputBuffer, 0, this._inputBlockSize, this._outputBuffer, 0);
				if (useAsync)
				{
					await this._stream.WriteAsync(new ReadOnlyMemory<byte>(this._outputBuffer, 0, numOutputBytes), cancellationToken);
				}
				else
				{
					this._stream.Write(this._outputBuffer, 0, numOutputBytes);
				}
				this._inputBufferIndex = 0;
			}
			while (bytesToWrite > 0)
			{
				if (bytesToWrite < this._inputBlockSize)
				{
					Buffer.BlockCopy(buffer, currentInputIndex, this._inputBuffer, 0, bytesToWrite);
					this._inputBufferIndex += bytesToWrite;
					break;
				}
				int num = bytesToWrite / this._inputBlockSize;
				if (this._transform.CanTransformMultipleBlocks && num > 1)
				{
					int numWholeBlocksInBytes = num * this._inputBlockSize;
					byte[] tempOutputBuffer = ArrayPool<byte>.Shared.Rent(num * this._outputBlockSize);
					int numOutputBytes = 0;
					try
					{
						numOutputBytes = this._transform.TransformBlock(buffer, currentInputIndex, numWholeBlocksInBytes, tempOutputBuffer, 0);
						if (useAsync)
						{
							await this._stream.WriteAsync(new ReadOnlyMemory<byte>(tempOutputBuffer, 0, numOutputBytes), cancellationToken);
						}
						else
						{
							this._stream.Write(tempOutputBuffer, 0, numOutputBytes);
						}
						currentInputIndex += numWholeBlocksInBytes;
						bytesToWrite -= numWholeBlocksInBytes;
					}
					finally
					{
						CryptographicOperations.ZeroMemory(new Span<byte>(tempOutputBuffer, 0, numOutputBytes));
						ArrayPool<byte>.Shared.Return(tempOutputBuffer, false);
						tempOutputBuffer = null;
					}
					tempOutputBuffer = null;
				}
				else
				{
					int numOutputBytes = this._transform.TransformBlock(buffer, currentInputIndex, this._inputBlockSize, this._outputBuffer, 0);
					if (useAsync)
					{
						await this._stream.WriteAsync(new ReadOnlyMemory<byte>(this._outputBuffer, 0, numOutputBytes), cancellationToken);
					}
					else
					{
						this._stream.Write(this._outputBuffer, 0, numOutputBytes);
					}
					currentInputIndex += this._inputBlockSize;
					bytesToWrite -= this._inputBlockSize;
				}
			}
		}

		public void Clear()
		{
			this.Close();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (!this._finalBlockTransformed)
					{
						this.FlushFinalBlock();
					}
					if (!this._leaveOpen)
					{
						this._stream.Dispose();
					}
				}
			}
			finally
			{
				try
				{
					this._finalBlockTransformed = true;
					if (this._inputBuffer != null)
					{
						Array.Clear(this._inputBuffer, 0, this._inputBuffer.Length);
					}
					if (this._outputBuffer != null)
					{
						Array.Clear(this._outputBuffer, 0, this._outputBuffer.Length);
					}
					this._inputBuffer = null;
					this._outputBuffer = null;
					this._canRead = false;
					this._canWrite = false;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		private void InitializeBuffer()
		{
			if (this._transform != null)
			{
				this._inputBlockSize = this._transform.InputBlockSize;
				this._inputBuffer = new byte[this._inputBlockSize];
				this._outputBlockSize = this._transform.OutputBlockSize;
				this._outputBuffer = new byte[this._outputBlockSize];
			}
		}

		private SemaphoreSlim AsyncActiveSemaphore
		{
			get
			{
				return LazyInitializer.EnsureInitialized<SemaphoreSlim>(ref this._lazyAsyncActiveSemaphore, () => new SemaphoreSlim(1, 1));
			}
		}

		private readonly Stream _stream;

		private readonly ICryptoTransform _transform;

		private readonly CryptoStreamMode _transformMode;

		private byte[] _inputBuffer;

		private int _inputBufferIndex;

		private int _inputBlockSize;

		private byte[] _outputBuffer;

		private int _outputBufferIndex;

		private int _outputBlockSize;

		private bool _canRead;

		private bool _canWrite;

		private bool _finalBlockTransformed;

		private SemaphoreSlim _lazyAsyncActiveSemaphore;

		private readonly bool _leaveOpen;
	}
}
