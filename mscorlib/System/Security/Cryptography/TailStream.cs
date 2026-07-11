using System;
using System.IO;

namespace System.Security.Cryptography
{
	internal sealed class TailStream : Stream
	{
		public TailStream(int bufferSize)
		{
			this._Buffer = new byte[bufferSize];
			this._BufferSize = bufferSize;
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
					if (this._Buffer != null)
					{
						Array.Clear(this._Buffer, 0, this._Buffer.Length);
					}
					this._Buffer = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public byte[] Buffer
		{
			get
			{
				return (byte[])this._Buffer.Clone();
			}
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
				return this._Buffer != null;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException(Environment.GetResourceString("Stream does not support seeking."));
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException(Environment.GetResourceString("Stream does not support seeking."));
			}
			set
			{
				throw new NotSupportedException(Environment.GetResourceString("Stream does not support seeking."));
			}
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(Environment.GetResourceString("Stream does not support seeking."));
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(Environment.GetResourceString("Stream does not support seeking."));
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException(Environment.GetResourceString("Stream does not support reading."));
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this._Buffer == null)
			{
				throw new ObjectDisposedException("TailStream");
			}
			if (count == 0)
			{
				return;
			}
			if (this._BufferFull)
			{
				if (count > this._BufferSize)
				{
					global::System.Buffer.InternalBlockCopy(buffer, offset + count - this._BufferSize, this._Buffer, 0, this._BufferSize);
					return;
				}
				global::System.Buffer.InternalBlockCopy(this._Buffer, this._BufferSize - count, this._Buffer, 0, this._BufferSize - count);
				global::System.Buffer.InternalBlockCopy(buffer, offset, this._Buffer, this._BufferSize - count, count);
				return;
			}
			else
			{
				if (count > this._BufferSize)
				{
					global::System.Buffer.InternalBlockCopy(buffer, offset + count - this._BufferSize, this._Buffer, 0, this._BufferSize);
					this._BufferFull = true;
					return;
				}
				if (count + this._BufferIndex >= this._BufferSize)
				{
					global::System.Buffer.InternalBlockCopy(this._Buffer, this._BufferIndex + count - this._BufferSize, this._Buffer, 0, this._BufferSize - count);
					global::System.Buffer.InternalBlockCopy(buffer, offset, this._Buffer, this._BufferIndex, count);
					this._BufferFull = true;
					return;
				}
				global::System.Buffer.InternalBlockCopy(buffer, offset, this._Buffer, this._BufferIndex, count);
				this._BufferIndex += count;
				return;
			}
		}

		private byte[] _Buffer;

		private int _BufferSize;

		private int _BufferIndex;

		private bool _BufferFull;
	}
}
