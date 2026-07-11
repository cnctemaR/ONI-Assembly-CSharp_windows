using System;
using System.IO;

namespace System.Runtime
{
	internal class BufferedOutputStream : Stream
	{
		public BufferedOutputStream()
		{
			this.chunks = new byte[4][];
		}

		public BufferedOutputStream(int initialSize, int maxSize, InternalBufferManager bufferManager)
			: this()
		{
			this.Reinitialize(initialSize, maxSize, bufferManager);
		}

		public BufferedOutputStream(int maxSize)
			: this(0, maxSize, InternalBufferManager.Create(0L, int.MaxValue))
		{
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
				return (long)this.totalSize;
			}
		}

		public override long Position
		{
			get
			{
				throw Fx.Exception.AsError(new NotSupportedException("Seek Not Supported"));
			}
			set
			{
				throw Fx.Exception.AsError(new NotSupportedException("Seek Not Supported"));
			}
		}

		public void Reinitialize(int initialSize, int maxSizeQuota, InternalBufferManager bufferManager)
		{
			this.Reinitialize(initialSize, maxSizeQuota, maxSizeQuota, bufferManager);
		}

		public void Reinitialize(int initialSize, int maxSizeQuota, int effectiveMaxSize, InternalBufferManager bufferManager)
		{
			this.maxSizeQuota = maxSizeQuota;
			this.maxSize = effectiveMaxSize;
			this.bufferManager = bufferManager;
			this.currentChunk = bufferManager.TakeBuffer(initialSize);
			this.currentChunkSize = 0;
			this.totalSize = 0;
			this.chunkCount = 1;
			this.chunks[0] = this.currentChunk;
			this.initialized = true;
		}

		private void AllocNextChunk(int minimumChunkSize)
		{
			int num;
			if (this.currentChunk.Length > 1073741823)
			{
				num = int.MaxValue;
			}
			else
			{
				num = this.currentChunk.Length * 2;
			}
			if (minimumChunkSize > num)
			{
				num = minimumChunkSize;
			}
			byte[] array = this.bufferManager.TakeBuffer(num);
			if (this.chunkCount == this.chunks.Length)
			{
				byte[][] array2 = new byte[this.chunks.Length * 2][];
				Array.Copy(this.chunks, array2, this.chunks.Length);
				this.chunks = array2;
			}
			byte[][] array3 = this.chunks;
			int num2 = this.chunkCount;
			this.chunkCount = num2 + 1;
			array3[num2] = array;
			this.currentChunk = array;
			this.currentChunkSize = 0;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			throw Fx.Exception.AsError(new NotSupportedException("Read Not Supported"));
		}

		public override int EndRead(IAsyncResult result)
		{
			throw Fx.Exception.AsError(new NotSupportedException("Read Not Supported"));
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			this.Write(buffer, offset, size);
			return new CompletedAsyncResult(callback, state);
		}

		public override void EndWrite(IAsyncResult result)
		{
			CompletedAsyncResult.End(result);
		}

		public void Clear()
		{
			if (!this.callerReturnsBuffer)
			{
				for (int i = 0; i < this.chunkCount; i++)
				{
					this.bufferManager.ReturnBuffer(this.chunks[i]);
					this.chunks[i] = null;
				}
			}
			this.callerReturnsBuffer = false;
			this.initialized = false;
			this.bufferReturned = false;
			this.chunkCount = 0;
			this.currentChunk = null;
		}

		public override void Close()
		{
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			throw Fx.Exception.AsError(new NotSupportedException("Read Not Supported"));
		}

		public override int ReadByte()
		{
			throw Fx.Exception.AsError(new NotSupportedException("Read Not Supported"));
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw Fx.Exception.AsError(new NotSupportedException("Seek Not Supported"));
		}

		public override void SetLength(long value)
		{
			throw Fx.Exception.AsError(new NotSupportedException("Seek Not Supported"));
		}

		public MemoryStream ToMemoryStream()
		{
			int num;
			return new MemoryStream(this.ToArray(out num), 0, num);
		}

		public byte[] ToArray(out int bufferSize)
		{
			byte[] array;
			if (this.chunkCount == 1)
			{
				array = this.currentChunk;
				bufferSize = this.currentChunkSize;
				this.callerReturnsBuffer = true;
			}
			else
			{
				array = this.bufferManager.TakeBuffer(this.totalSize);
				int num = 0;
				int num2 = this.chunkCount - 1;
				for (int i = 0; i < num2; i++)
				{
					byte[] array2 = this.chunks[i];
					Buffer.BlockCopy(array2, 0, array, num, array2.Length);
					num += array2.Length;
				}
				Buffer.BlockCopy(this.currentChunk, 0, array, num, this.currentChunkSize);
				bufferSize = this.totalSize;
			}
			this.bufferReturned = true;
			return array;
		}

		public void Skip(int size)
		{
			this.WriteCore(null, 0, size);
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			this.WriteCore(buffer, offset, size);
		}

		protected virtual Exception CreateQuotaExceededException(int maxSizeQuota)
		{
			return new InvalidOperationException(InternalSR.BufferedOutputStreamQuotaExceeded(maxSizeQuota));
		}

		private void WriteCore(byte[] buffer, int offset, int size)
		{
			if (size < 0)
			{
				throw Fx.Exception.ArgumentOutOfRange("size", size, "Value Must Be Non Negative");
			}
			if (2147483647 - size < this.totalSize)
			{
				throw Fx.Exception.AsError(this.CreateQuotaExceededException(this.maxSizeQuota));
			}
			int num = this.totalSize + size;
			if (num > this.maxSize)
			{
				throw Fx.Exception.AsError(this.CreateQuotaExceededException(this.maxSizeQuota));
			}
			int num2 = this.currentChunk.Length - this.currentChunkSize;
			if (size > num2)
			{
				if (num2 > 0)
				{
					if (buffer != null)
					{
						Buffer.BlockCopy(buffer, offset, this.currentChunk, this.currentChunkSize, num2);
					}
					this.currentChunkSize = this.currentChunk.Length;
					offset += num2;
					size -= num2;
				}
				this.AllocNextChunk(size);
			}
			if (buffer != null)
			{
				Buffer.BlockCopy(buffer, offset, this.currentChunk, this.currentChunkSize, size);
			}
			this.totalSize = num;
			this.currentChunkSize += size;
		}

		public override void WriteByte(byte value)
		{
			if (this.totalSize == this.maxSize)
			{
				throw Fx.Exception.AsError(this.CreateQuotaExceededException(this.maxSize));
			}
			if (this.currentChunkSize == this.currentChunk.Length)
			{
				this.AllocNextChunk(1);
			}
			byte[] array = this.currentChunk;
			int num = this.currentChunkSize;
			this.currentChunkSize = num + 1;
			array[num] = value;
		}

		private InternalBufferManager bufferManager;

		private byte[][] chunks;

		private int chunkCount;

		private byte[] currentChunk;

		private int currentChunkSize;

		private int maxSize;

		private int maxSizeQuota;

		private int totalSize;

		private bool callerReturnsBuffer;

		private bool bufferReturned;

		private bool initialized;
	}
}
