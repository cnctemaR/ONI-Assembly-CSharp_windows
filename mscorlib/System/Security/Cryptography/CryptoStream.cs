using System;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class CryptoStream : Stream
	{
		public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
		{
			if (mode == CryptoStreamMode.Read && !stream.CanRead)
			{
				throw new ArgumentException(Locale.GetText("Can't read on stream"));
			}
			if (mode == CryptoStreamMode.Write && !stream.CanWrite)
			{
				throw new ArgumentException(Locale.GetText("Can't write on stream"));
			}
			this._stream = stream;
			this._transform = transform;
			this._mode = mode;
			this._disposed = false;
			if (transform != null)
			{
				if (mode == CryptoStreamMode.Read)
				{
					this._currentBlock = new byte[transform.InputBlockSize];
					this._workingBlock = new byte[transform.InputBlockSize];
				}
				else if (mode == CryptoStreamMode.Write)
				{
					this._currentBlock = new byte[transform.OutputBlockSize];
					this._workingBlock = new byte[transform.OutputBlockSize];
				}
			}
		}

		~CryptoStream()
		{
			this.Dispose(false);
		}

		public override bool CanRead
		{
			get
			{
				return this._mode == CryptoStreamMode.Read;
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
				return this._mode == CryptoStreamMode.Write;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("Length");
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException("Position");
			}
			set
			{
				throw new NotSupportedException("Position");
			}
		}

		public void Clear()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override void Close()
		{
			if (!this._flushedFinalBlock && this._mode == CryptoStreamMode.Write)
			{
				this.FlushFinalBlock();
			}
			if (this._stream != null)
			{
				this._stream.Close();
			}
		}

		public override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			if (this._mode != CryptoStreamMode.Read)
			{
				throw new NotSupportedException(Locale.GetText("not in Read mode"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Locale.GetText("negative"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Locale.GetText("negative"));
			}
			if (offset > buffer.Length - count)
			{
				throw new ArgumentException("(offset+count)", Locale.GetText("buffer overflow"));
			}
			if (this._workingBlock == null)
			{
				return 0;
			}
			int num = 0;
			if (count == 0 || (this._transformedPos == this._transformedCount && this._endOfStream))
			{
				return num;
			}
			if (this._waitingBlock == null)
			{
				this._transformedBlock = new byte[this._transform.OutputBlockSize << 2];
				this._transformedPos = 0;
				this._transformedCount = 0;
				this._waitingBlock = new byte[this._transform.InputBlockSize];
				this._waitingCount = this._stream.Read(this._waitingBlock, 0, this._waitingBlock.Length);
			}
			while (count > 0)
			{
				int num2 = this._transformedCount - this._transformedPos;
				if (num2 < this._transform.InputBlockSize)
				{
					int num3 = 0;
					this._workingCount = this._stream.Read(this._workingBlock, 0, this._transform.InputBlockSize);
					this._endOfStream = this._workingCount < this._transform.InputBlockSize;
					if (!this._endOfStream)
					{
						num3 = this._transform.TransformBlock(this._waitingBlock, 0, this._waitingBlock.Length, this._transformedBlock, this._transformedCount);
						Buffer.BlockCopy(this._workingBlock, 0, this._waitingBlock, 0, this._workingCount);
						this._waitingCount = this._workingCount;
					}
					else
					{
						if (this._workingCount > 0)
						{
							num3 = this._transform.TransformBlock(this._waitingBlock, 0, this._waitingBlock.Length, this._transformedBlock, this._transformedCount);
							Buffer.BlockCopy(this._workingBlock, 0, this._waitingBlock, 0, this._workingCount);
							this._waitingCount = this._workingCount;
							num2 += num3;
							this._transformedCount += num3;
						}
						if (!this._flushedFinalBlock)
						{
							byte[] array = this._transform.TransformFinalBlock(this._waitingBlock, 0, this._waitingCount);
							num3 = array.Length;
							Buffer.BlockCopy(array, 0, this._transformedBlock, this._transformedCount, array.Length);
							Array.Clear(array, 0, array.Length);
							this._flushedFinalBlock = true;
						}
					}
					num2 += num3;
					this._transformedCount += num3;
				}
				if (this._transformedPos > this._transform.OutputBlockSize)
				{
					Buffer.BlockCopy(this._transformedBlock, this._transformedPos, this._transformedBlock, 0, num2);
					this._transformedCount -= this._transformedPos;
					this._transformedPos = 0;
				}
				num2 = ((count >= num2) ? num2 : count);
				if (num2 > 0)
				{
					Buffer.BlockCopy(this._transformedBlock, this._transformedPos, buffer, offset, num2);
					this._transformedPos += num2;
					num += num2;
					offset += num2;
					count -= num2;
				}
				if ((num2 != this._transform.InputBlockSize && this._waitingCount != this._transform.InputBlockSize) || this._endOfStream)
				{
					count = 0;
				}
			}
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this._mode != CryptoStreamMode.Write)
			{
				throw new NotSupportedException(Locale.GetText("not in Write mode"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Locale.GetText("negative"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Locale.GetText("negative"));
			}
			if (offset > buffer.Length - count)
			{
				throw new ArgumentException("(offset+count)", Locale.GetText("buffer overflow"));
			}
			if (this._stream == null)
			{
				throw new ArgumentNullException("inner stream was diposed");
			}
			int num = count;
			if (this._partialCount > 0 && this._partialCount != this._transform.InputBlockSize)
			{
				int num2 = this._transform.InputBlockSize - this._partialCount;
				num2 = ((count >= num2) ? num2 : count);
				Buffer.BlockCopy(buffer, offset, this._workingBlock, this._partialCount, num2);
				this._partialCount += num2;
				offset += num2;
				count -= num2;
			}
			int num3 = offset;
			while (count > 0)
			{
				if (this._partialCount == this._transform.InputBlockSize)
				{
					int num4 = this._transform.TransformBlock(this._workingBlock, 0, this._partialCount, this._currentBlock, 0);
					this._stream.Write(this._currentBlock, 0, num4);
					this._partialCount = 0;
				}
				if (this._transform.CanTransformMultipleBlocks)
				{
					int num5 = count & ~(this._transform.InputBlockSize - 1);
					int num6 = count & (this._transform.InputBlockSize - 1);
					int num7 = (1 + num5 / this._transform.InputBlockSize) * this._transform.OutputBlockSize;
					if (this._workingBlock.Length < num7)
					{
						Array.Clear(this._workingBlock, 0, this._workingBlock.Length);
						this._workingBlock = new byte[num7];
					}
					if (num5 > 0)
					{
						int num8 = this._transform.TransformBlock(buffer, offset, num5, this._workingBlock, 0);
						this._stream.Write(this._workingBlock, 0, num8);
					}
					if (num6 > 0)
					{
						Buffer.BlockCopy(buffer, num - num6, this._workingBlock, 0, num6);
					}
					this._partialCount = num6;
					count = 0;
				}
				else
				{
					int num9 = Math.Min(this._transform.InputBlockSize - this._partialCount, count);
					Buffer.BlockCopy(buffer, num3, this._workingBlock, this._partialCount, num9);
					num3 += num9;
					this._partialCount += num9;
					count -= num9;
				}
			}
		}

		public override void Flush()
		{
			if (this._stream != null)
			{
				this._stream.Flush();
			}
		}

		public void FlushFinalBlock()
		{
			if (this._flushedFinalBlock)
			{
				throw new NotSupportedException(Locale.GetText("This method cannot be called twice."));
			}
			if (this._disposed)
			{
				throw new NotSupportedException(Locale.GetText("CryptoStream was disposed."));
			}
			if (this._mode != CryptoStreamMode.Write)
			{
				return;
			}
			this._flushedFinalBlock = true;
			byte[] array = this._transform.TransformFinalBlock(this._workingBlock, 0, this._partialCount);
			if (this._stream != null)
			{
				this._stream.Write(array, 0, array.Length);
				if (this._stream is CryptoStream)
				{
					(this._stream as CryptoStream).FlushFinalBlock();
				}
				this._stream.Flush();
			}
			Array.Clear(array, 0, array.Length);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("SetLength");
		}

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				this._disposed = true;
				if (this._workingBlock != null)
				{
					Array.Clear(this._workingBlock, 0, this._workingBlock.Length);
				}
				if (this._currentBlock != null)
				{
					Array.Clear(this._currentBlock, 0, this._currentBlock.Length);
				}
				if (disposing)
				{
					this._stream = null;
					this._workingBlock = null;
					this._currentBlock = null;
				}
			}
		}

		private Stream _stream;

		private ICryptoTransform _transform;

		private CryptoStreamMode _mode;

		private byte[] _currentBlock;

		private bool _disposed;

		private bool _flushedFinalBlock;

		private int _partialCount;

		private bool _endOfStream;

		private byte[] _waitingBlock;

		private int _waitingCount;

		private byte[] _transformedBlock;

		private int _transformedPos;

		private int _transformedCount;

		private byte[] _workingBlock;

		private int _workingCount;
	}
}
