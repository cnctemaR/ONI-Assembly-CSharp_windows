using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[CLSCompliant(false)]
	public class UnmanagedMemoryStream : Stream
	{
		protected UnmanagedMemoryStream()
		{
			this.closed = true;
		}

		public unsafe UnmanagedMemoryStream(byte* pointer, long length)
		{
			this.Initialize(pointer, length, length, FileAccess.Read);
		}

		public unsafe UnmanagedMemoryStream(byte* pointer, long length, long capacity, FileAccess access)
		{
			this.Initialize(pointer, length, capacity, access);
		}

		internal event EventHandler Closed;

		public override bool CanRead
		{
			get
			{
				return !this.closed && this.fileaccess != FileAccess.Write;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return !this.closed;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !this.closed && this.fileaccess != FileAccess.Read;
			}
		}

		public long Capacity
		{
			get
			{
				if (this.closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				return this.capacity;
			}
		}

		public override long Length
		{
			get
			{
				if (this.closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				return this.length;
			}
		}

		public override long Position
		{
			get
			{
				if (this.closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				return this.current_position;
			}
			set
			{
				if (this.closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", "Non-negative number required.");
				}
				if (value > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("value", "The position is larger than Int32.MaxValue.");
				}
				this.current_position = value;
			}
		}

		public unsafe byte* PositionPointer
		{
			get
			{
				if (this.closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				if (this.current_position >= this.length)
				{
					throw new IndexOutOfRangeException("value");
				}
				return (byte*)(void*)this.initial_pointer + this.current_position;
			}
			set
			{
				if (this.closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				if (value < (byte*)(void*)this.initial_pointer)
				{
					throw new IOException("Address is below the inital address");
				}
				this.Position = (long)(value - (void*)this.initial_pointer);
			}
		}

		public override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			if (this.closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
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
				throw new ArgumentException("The length of the buffer array minus the offset parameter is less than the count parameter");
			}
			if (this.fileaccess == FileAccess.Write)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			if (this.current_position >= this.length)
			{
				return 0;
			}
			int num = ((this.current_position + (long)count >= this.length) ? ((int)(this.length - this.current_position)) : count);
			Marshal.Copy(new IntPtr(this.initial_pointer.ToInt64() + this.current_position), buffer, offset, num);
			this.current_position += (long)num;
			return num;
		}

		public override int ReadByte()
		{
			if (this.closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (this.fileaccess == FileAccess.Write)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			if (this.current_position >= this.length)
			{
				return -1;
			}
			IntPtr intPtr = this.initial_pointer;
			long num;
			this.current_position = (num = this.current_position) + 1L;
			return (int)Marshal.ReadByte(intPtr, (int)num);
		}

		public override long Seek(long offset, SeekOrigin loc)
		{
			if (this.closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			long num;
			switch (loc)
			{
			case SeekOrigin.Begin:
				if (offset < 0L)
				{
					throw new IOException("An attempt was made to seek before the beginning of the stream");
				}
				num = this.initial_position;
				break;
			case SeekOrigin.Current:
				num = this.current_position;
				break;
			case SeekOrigin.End:
				num = this.length;
				break;
			default:
				throw new ArgumentException("Invalid SeekOrigin option");
			}
			num += offset;
			if (num < this.initial_position)
			{
				throw new IOException("An attempt was made to seek before the beginning of the stream");
			}
			this.current_position = num;
			return this.current_position;
		}

		public override void SetLength(long value)
		{
			if (this.closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
			}
			if (value > this.capacity)
			{
				throw new IOException("Unable to expand length of this stream beyond its capacity.");
			}
			if (this.fileaccess == FileAccess.Read)
			{
				throw new NotSupportedException("Stream does not support writing.");
			}
			this.length = value;
			if (this.length < this.current_position)
			{
				this.current_position = this.length;
			}
		}

		public override void Flush()
		{
			if (this.closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.closed)
			{
				return;
			}
			this.closed = true;
			if (this.Closed != null)
			{
				this.Closed(this, null);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("The buffer parameter is a null reference");
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
				throw new ArgumentException("The length of the buffer array minus the offset parameter is less than the count parameter");
			}
			if (this.current_position > this.capacity - (long)count)
			{
				throw new NotSupportedException("Unable to expand length of this stream beyond its capacity.");
			}
			if (this.fileaccess == FileAccess.Read)
			{
				throw new NotSupportedException("Stream does not support writing.");
			}
			for (int i = 0; i < count; i++)
			{
				IntPtr intPtr = this.initial_pointer;
				long num;
				this.current_position = (num = this.current_position) + 1L;
				Marshal.WriteByte(intPtr, (int)num, buffer[offset + i]);
			}
			if (this.current_position > this.length)
			{
				this.length = this.current_position;
			}
		}

		public override void WriteByte(byte value)
		{
			if (this.closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (this.current_position == this.capacity)
			{
				throw new NotSupportedException("The current position is at the end of the capacity of the stream");
			}
			if (this.fileaccess == FileAccess.Read)
			{
				throw new NotSupportedException("Stream does not support writing.");
			}
			Marshal.WriteByte(this.initial_pointer, (int)this.current_position, value);
			this.current_position += 1L;
			if (this.current_position > this.length)
			{
				this.length = this.current_position;
			}
		}

		protected unsafe void Initialize(byte* pointer, long length, long capacity, FileAccess access)
		{
			if (pointer == null)
			{
				throw new ArgumentNullException("pointer");
			}
			if (length < 0L)
			{
				throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
			}
			if (capacity < 0L)
			{
				throw new ArgumentOutOfRangeException("capacity", "Non-negative number required.");
			}
			if (length > capacity)
			{
				throw new ArgumentOutOfRangeException("length", "The length cannot be greater than the capacity.");
			}
			if (access < FileAccess.Read || access > FileAccess.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("access", "Enum value was out of legal range.");
			}
			this.fileaccess = access;
			this.length = length;
			this.capacity = capacity;
			this.initial_position = 0L;
			this.current_position = this.initial_position;
			this.initial_pointer = new IntPtr((void*)pointer);
			this.closed = false;
		}

		private long length;

		private bool closed;

		private long capacity;

		private FileAccess fileaccess;

		private IntPtr initial_pointer;

		private long initial_position;

		private long current_position;
	}
}
