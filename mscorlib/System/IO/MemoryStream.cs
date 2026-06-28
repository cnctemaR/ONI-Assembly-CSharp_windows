using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with .NET")]
	[Serializable]
	public class MemoryStream : Stream
	{
		public MemoryStream()
			: this(0)
		{
		}

		public MemoryStream(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.canWrite = true;
			this.capacity = capacity;
			this.internalBuffer = new byte[capacity];
			this.expandable = true;
			this.allowGetBuffer = true;
		}

		public MemoryStream(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.InternalConstructor(buffer, 0, buffer.Length, true, false);
		}

		public MemoryStream(byte[] buffer, bool writable)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.InternalConstructor(buffer, 0, buffer.Length, writable, false);
		}

		public MemoryStream(byte[] buffer, int index, int count)
		{
			this.InternalConstructor(buffer, index, count, true, false);
		}

		public MemoryStream(byte[] buffer, int index, int count, bool writable)
		{
			this.InternalConstructor(buffer, index, count, writable, false);
		}

		public MemoryStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible)
		{
			this.InternalConstructor(buffer, index, count, writable, publiclyVisible);
		}

		private void InternalConstructor(byte[] buffer, int index, int count, bool writable, bool publicallyVisible)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("index or count is less than 0.");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("index+count", "The size of the buffer is less than index + count.");
			}
			this.canWrite = writable;
			this.internalBuffer = buffer;
			this.capacity = count + index;
			this.length = this.capacity;
			this.position = index;
			this.initialIndex = index;
			this.allowGetBuffer = publicallyVisible;
			this.expandable = false;
		}

		private void CheckIfClosedThrowDisposed()
		{
			if (this.streamClosed)
			{
				throw new ObjectDisposedException("MemoryStream");
			}
		}

		public override bool CanRead
		{
			get
			{
				return !this.streamClosed;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return !this.streamClosed;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this.streamClosed && this.canWrite;
			}
		}

		public virtual int Capacity
		{
			get
			{
				this.CheckIfClosedThrowDisposed();
				return this.capacity - this.initialIndex;
			}
			set
			{
				this.CheckIfClosedThrowDisposed();
				if (value == this.capacity)
				{
					return;
				}
				if (!this.expandable)
				{
					throw new NotSupportedException("Cannot expand this MemoryStream");
				}
				if (value < 0 || value < this.length)
				{
					throw new ArgumentOutOfRangeException("value", string.Concat(new object[] { "New capacity cannot be negative or less than the current capacity ", value, " ", this.capacity }));
				}
				byte[] array = null;
				if (value != 0)
				{
					array = new byte[value];
					Buffer.BlockCopy(this.internalBuffer, 0, array, 0, this.length);
				}
				this.dirty_bytes = 0;
				this.internalBuffer = array;
				this.capacity = value;
			}
		}

		public override long Length
		{
			get
			{
				this.CheckIfClosedThrowDisposed();
				return (long)(this.length - this.initialIndex);
			}
		}

		public override long Position
		{
			get
			{
				this.CheckIfClosedThrowDisposed();
				return (long)(this.position - this.initialIndex);
			}
			set
			{
				this.CheckIfClosedThrowDisposed();
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", "Position cannot be negative");
				}
				if (value > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("value", "Position must be non-negative and less than 2^31 - 1 - origin");
				}
				this.position = this.initialIndex + (int)value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.streamClosed = true;
			this.expandable = false;
		}

		public override void Flush()
		{
		}

		public virtual byte[] GetBuffer()
		{
			if (!this.allowGetBuffer)
			{
				throw new UnauthorizedAccessException();
			}
			return this.internalBuffer;
		}

		public override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			this.CheckIfClosedThrowDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("offset or count less than zero.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			if (this.position >= this.length || count == 0)
			{
				return 0;
			}
			if (this.position > this.length - count)
			{
				count = this.length - this.position;
			}
			Buffer.BlockCopy(this.internalBuffer, this.position, buffer, offset, count);
			this.position += count;
			return count;
		}

		public override int ReadByte()
		{
			this.CheckIfClosedThrowDisposed();
			if (this.position >= this.length)
			{
				return -1;
			}
			return (int)this.internalBuffer[this.position++];
		}

		public override long Seek(long offset, SeekOrigin loc)
		{
			this.CheckIfClosedThrowDisposed();
			if (offset > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("Offset out of range. " + offset);
			}
			int num;
			switch (loc)
			{
			case SeekOrigin.Begin:
				if (offset < 0L)
				{
					throw new IOException("Attempted to seek before start of MemoryStream.");
				}
				num = this.initialIndex;
				break;
			case SeekOrigin.Current:
				num = this.position;
				break;
			case SeekOrigin.End:
				num = this.length;
				break;
			default:
				throw new ArgumentException("loc", "Invalid SeekOrigin");
			}
			num += (int)offset;
			if (num < this.initialIndex)
			{
				throw new IOException("Attempted to seek before start of MemoryStream.");
			}
			this.position = num;
			return (long)this.position;
		}

		private int CalculateNewCapacity(int minimum)
		{
			if (minimum < 256)
			{
				minimum = 256;
			}
			if (minimum < this.capacity * 2)
			{
				minimum = this.capacity * 2;
			}
			return minimum;
		}

		private void Expand(int newSize)
		{
			if (newSize > this.capacity)
			{
				this.Capacity = this.CalculateNewCapacity(newSize);
			}
			else if (this.dirty_bytes > 0)
			{
				Array.Clear(this.internalBuffer, this.length, this.dirty_bytes);
				this.dirty_bytes = 0;
			}
		}

		public override void SetLength(long value)
		{
			if (!this.expandable && value > (long)this.capacity)
			{
				throw new NotSupportedException("Expanding this MemoryStream is not supported");
			}
			this.CheckIfClosedThrowDisposed();
			if (!this.canWrite)
			{
				throw new NotSupportedException(Locale.GetText("Cannot write to this MemoryStream"));
			}
			if (value < 0L || value + (long)this.initialIndex > 2147483647L)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = (int)value + this.initialIndex;
			if (num > this.length)
			{
				this.Expand(num);
			}
			else if (num < this.length)
			{
				this.dirty_bytes += this.length - num;
			}
			this.length = num;
			if (this.position > this.length)
			{
				this.position = this.length;
			}
		}

		public virtual byte[] ToArray()
		{
			int num = this.length - this.initialIndex;
			byte[] array = new byte[num];
			if (this.internalBuffer != null)
			{
				Buffer.BlockCopy(this.internalBuffer, this.initialIndex, array, 0, num);
			}
			return array;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckIfClosedThrowDisposed();
			if (!this.canWrite)
			{
				throw new NotSupportedException("Cannot write to this stream.");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			if (this.position > this.length - count)
			{
				this.Expand(this.position + count);
			}
			Buffer.BlockCopy(buffer, offset, this.internalBuffer, this.position, count);
			this.position += count;
			if (this.position >= this.length)
			{
				this.length = this.position;
			}
		}

		public override void WriteByte(byte value)
		{
			this.CheckIfClosedThrowDisposed();
			if (!this.canWrite)
			{
				throw new NotSupportedException("Cannot write to this stream.");
			}
			if (this.position >= this.length)
			{
				this.Expand(this.position + 1);
				this.length = this.position + 1;
			}
			this.internalBuffer[this.position++] = value;
		}

		public virtual void WriteTo(Stream stream)
		{
			this.CheckIfClosedThrowDisposed();
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			stream.Write(this.internalBuffer, this.initialIndex, this.length - this.initialIndex);
		}

		private bool canWrite;

		private bool allowGetBuffer;

		private int capacity;

		private int length;

		private byte[] internalBuffer;

		private int initialIndex;

		private bool expandable;

		private bool streamClosed;

		private int position;

		private int dirty_bytes;
	}
}
