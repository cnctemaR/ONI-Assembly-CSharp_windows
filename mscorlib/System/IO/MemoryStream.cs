using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	[ComVisible(true)]
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
				throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("Capacity must be positive."));
			}
			this._buffer = new byte[capacity];
			this._capacity = capacity;
			this._expandable = true;
			this._writable = true;
			this._exposable = true;
			this._origin = 0;
			this._isOpen = true;
		}

		public MemoryStream(byte[] buffer)
			: this(buffer, true)
		{
		}

		public MemoryStream(byte[] buffer, bool writable)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			this._buffer = buffer;
			this._length = (this._capacity = buffer.Length);
			this._writable = writable;
			this._exposable = false;
			this._origin = 0;
			this._isOpen = true;
		}

		public MemoryStream(byte[] buffer, int index, int count)
			: this(buffer, index, count, true, false)
		{
		}

		public MemoryStream(byte[] buffer, int index, int count, bool writable)
			: this(buffer, index, count, writable, false)
		{
		}

		public MemoryStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			this._buffer = buffer;
			this._position = index;
			this._origin = index;
			this._length = (this._capacity = index + count);
			this._writable = writable;
			this._exposable = publiclyVisible;
			this._expandable = false;
			this._isOpen = true;
		}

		public override bool CanRead
		{
			get
			{
				return this._isOpen;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this._isOpen;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._writable;
			}
		}

		private void EnsureWriteable()
		{
			if (!this.CanWrite)
			{
				__Error.WriteNotSupported();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this._isOpen = false;
					this._writable = false;
					this._expandable = false;
					this._lastReadTask = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private bool EnsureCapacity(int value)
		{
			if (value < 0)
			{
				throw new IOException(Environment.GetResourceString("Stream was too long."));
			}
			if (value > this._capacity)
			{
				int num = value;
				if (num < 256)
				{
					num = 256;
				}
				if (num < this._capacity * 2)
				{
					num = this._capacity * 2;
				}
				if (this._capacity * 2 > 2147483591)
				{
					num = ((value > 2147483591) ? value : 2147483591);
				}
				this.Capacity = num;
				return true;
			}
			return false;
		}

		public override void Flush()
		{
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation(cancellationToken);
			}
			Task task;
			try
			{
				this.Flush();
				task = Task.CompletedTask;
			}
			catch (Exception ex)
			{
				task = Task.FromException(ex);
			}
			return task;
		}

		public virtual byte[] GetBuffer()
		{
			if (!this._exposable)
			{
				throw new UnauthorizedAccessException(Environment.GetResourceString("MemoryStream's internal buffer cannot be accessed."));
			}
			return this._buffer;
		}

		public virtual bool TryGetBuffer(out ArraySegment<byte> buffer)
		{
			if (!this._exposable)
			{
				buffer = default(ArraySegment<byte>);
				return false;
			}
			buffer = new ArraySegment<byte>(this._buffer, this._origin, this._length - this._origin);
			return true;
		}

		internal byte[] InternalGetBuffer()
		{
			return this._buffer;
		}

		[FriendAccessAllowed]
		internal void InternalGetOriginAndLength(out int origin, out int length)
		{
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			origin = this._origin;
			length = this._length;
		}

		internal int InternalGetPosition()
		{
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			return this._position;
		}

		internal int InternalReadInt32()
		{
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			int num = (this._position += 4);
			if (num > this._length)
			{
				this._position = this._length;
				__Error.EndOfFile();
			}
			return (int)this._buffer[num - 4] | ((int)this._buffer[num - 3] << 8) | ((int)this._buffer[num - 2] << 16) | ((int)this._buffer[num - 1] << 24);
		}

		internal int InternalEmulateRead(int count)
		{
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			int num = this._length - this._position;
			if (num > count)
			{
				num = count;
			}
			if (num < 0)
			{
				num = 0;
			}
			this._position += num;
			return num;
		}

		public virtual int Capacity
		{
			get
			{
				if (!this._isOpen)
				{
					__Error.StreamIsClosed();
				}
				return this._capacity - this._origin;
			}
			set
			{
				if ((long)value < this.Length)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("capacity was less than the current size."));
				}
				if (!this._isOpen)
				{
					__Error.StreamIsClosed();
				}
				if (!this._expandable && value != this.Capacity)
				{
					__Error.MemoryStreamNotExpandable();
				}
				if (this._expandable && value != this._capacity)
				{
					if (value > 0)
					{
						byte[] array = new byte[value];
						if (this._length > 0)
						{
							Buffer.InternalBlockCopy(this._buffer, 0, array, 0, this._length);
						}
						this._buffer = array;
					}
					else
					{
						this._buffer = null;
					}
					this._capacity = value;
				}
			}
		}

		public override long Length
		{
			get
			{
				if (!this._isOpen)
				{
					__Error.StreamIsClosed();
				}
				return (long)(this._length - this._origin);
			}
		}

		public override long Position
		{
			get
			{
				if (!this._isOpen)
				{
					__Error.StreamIsClosed();
				}
				return (long)(this._position - this._origin);
			}
			set
			{
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("Non-negative number required."));
				}
				if (!this._isOpen)
				{
					__Error.StreamIsClosed();
				}
				if (value > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("Stream length must be non-negative and less than 2^31 - 1 - origin."));
				}
				this._position = this._origin + (int)value;
			}
		}

		public override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			int num = this._length - this._position;
			if (num > count)
			{
				num = count;
			}
			if (num <= 0)
			{
				return 0;
			}
			if (num <= 8)
			{
				int num2 = num;
				while (--num2 >= 0)
				{
					buffer[offset + num2] = this._buffer[this._position + num2];
				}
			}
			else
			{
				Buffer.InternalBlockCopy(this._buffer, this._position, buffer, offset, num);
			}
			this._position += num;
			return num;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation<int>(cancellationToken);
			}
			Task<int> task;
			try
			{
				int num = this.Read(buffer, offset, count);
				Task<int> lastReadTask = this._lastReadTask;
				Task<int> task2;
				if (lastReadTask == null || lastReadTask.Result != num)
				{
					task = (this._lastReadTask = Task.FromResult<int>(num));
					task2 = task;
				}
				else
				{
					task2 = lastReadTask;
				}
				task = task2;
			}
			catch (OperationCanceledException ex)
			{
				task = Task.FromCancellation<int>(ex);
			}
			catch (Exception ex2)
			{
				task = Task.FromException<int>(ex2);
			}
			return task;
		}

		public override int ReadByte()
		{
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			if (this._position >= this._length)
			{
				return -1;
			}
			byte[] buffer = this._buffer;
			int position = this._position;
			this._position = position + 1;
			return buffer[position];
		}

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("Positive number required."));
			}
			if (!this.CanRead && !this.CanWrite)
			{
				throw new ObjectDisposedException(null, Environment.GetResourceString("Cannot access a closed Stream."));
			}
			if (!destination.CanRead && !destination.CanWrite)
			{
				throw new ObjectDisposedException("destination", Environment.GetResourceString("Cannot access a closed Stream."));
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException(Environment.GetResourceString("Stream does not support reading."));
			}
			if (!destination.CanWrite)
			{
				throw new NotSupportedException(Environment.GetResourceString("Stream does not support writing."));
			}
			if (base.GetType() != typeof(MemoryStream))
			{
				return base.CopyToAsync(destination, bufferSize, cancellationToken);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation(cancellationToken);
			}
			int position = this._position;
			int num = this.InternalEmulateRead(this._length - this._position);
			MemoryStream memoryStream = destination as MemoryStream;
			if (memoryStream == null)
			{
				return destination.WriteAsync(this._buffer, position, num, cancellationToken);
			}
			Task task;
			try
			{
				memoryStream.Write(this._buffer, position, num);
				task = Task.CompletedTask;
			}
			catch (Exception ex)
			{
				task = Task.FromException(ex);
			}
			return task;
		}

		public override long Seek(long offset, SeekOrigin loc)
		{
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			if (offset > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Stream length must be non-negative and less than 2^31 - 1 - origin."));
			}
			switch (loc)
			{
			case SeekOrigin.Begin:
			{
				int num = this._origin + (int)offset;
				if (offset < 0L || num < this._origin)
				{
					throw new IOException(Environment.GetResourceString("An attempt was made to move the position before the beginning of the stream."));
				}
				this._position = num;
				break;
			}
			case SeekOrigin.Current:
			{
				int num2 = this._position + (int)offset;
				if ((long)this._position + offset < (long)this._origin || num2 < this._origin)
				{
					throw new IOException(Environment.GetResourceString("An attempt was made to move the position before the beginning of the stream."));
				}
				this._position = num2;
				break;
			}
			case SeekOrigin.End:
			{
				int num3 = this._length + (int)offset;
				if ((long)this._length + offset < (long)this._origin || num3 < this._origin)
				{
					throw new IOException(Environment.GetResourceString("An attempt was made to move the position before the beginning of the stream."));
				}
				this._position = num3;
				break;
			}
			default:
				throw new ArgumentException(Environment.GetResourceString("Invalid seek origin."));
			}
			return (long)this._position;
		}

		public override void SetLength(long value)
		{
			if (value < 0L || value > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("Stream length must be non-negative and less than 2^31 - 1 - origin."));
			}
			this.EnsureWriteable();
			if (value > (long)(2147483647 - this._origin))
			{
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("Stream length must be non-negative and less than 2^31 - 1 - origin."));
			}
			int num = this._origin + (int)value;
			if (!this.EnsureCapacity(num) && num > this._length)
			{
				Array.Clear(this._buffer, this._length, num - this._length);
			}
			this._length = num;
			if (this._position > num)
			{
				this._position = num;
			}
		}

		public virtual byte[] ToArray()
		{
			if (this._length - this._origin == 0)
			{
				return EmptyArray<byte>.Value;
			}
			byte[] array = new byte[this._length - this._origin];
			Buffer.InternalBlockCopy(this._buffer, this._origin, array, 0, this._length - this._origin);
			return array;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			this.EnsureWriteable();
			int num = this._position + count;
			if (num < 0)
			{
				throw new IOException(Environment.GetResourceString("Stream was too long."));
			}
			if (num > this._length)
			{
				bool flag = this._position > this._length;
				if (num > this._capacity && this.EnsureCapacity(num))
				{
					flag = false;
				}
				if (flag)
				{
					Array.Clear(this._buffer, this._length, num - this._length);
				}
				this._length = num;
			}
			if (count <= 8 && buffer != this._buffer)
			{
				int num2 = count;
				while (--num2 >= 0)
				{
					this._buffer[this._position + num2] = buffer[offset + num2];
				}
			}
			else
			{
				Buffer.InternalBlockCopy(buffer, offset, this._buffer, this._position, count);
			}
			this._position = num;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation(cancellationToken);
			}
			Task task;
			try
			{
				this.Write(buffer, offset, count);
				task = Task.CompletedTask;
			}
			catch (OperationCanceledException ex)
			{
				task = Task.FromCancellation<VoidTaskResult>(ex);
			}
			catch (Exception ex2)
			{
				task = Task.FromException(ex2);
			}
			return task;
		}

		public override void WriteByte(byte value)
		{
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			this.EnsureWriteable();
			if (this._position >= this._length)
			{
				int num = this._position + 1;
				bool flag = this._position > this._length;
				if (num >= this._capacity && this.EnsureCapacity(num))
				{
					flag = false;
				}
				if (flag)
				{
					Array.Clear(this._buffer, this._length, this._position - this._length);
				}
				this._length = num;
			}
			byte[] buffer = this._buffer;
			int position = this._position;
			this._position = position + 1;
			buffer[position] = value;
		}

		public virtual void WriteTo(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream", Environment.GetResourceString("Stream cannot be null."));
			}
			if (!this._isOpen)
			{
				__Error.StreamIsClosed();
			}
			stream.Write(this._buffer, this._origin, this._length - this._origin);
		}

		private byte[] _buffer;

		private int _origin;

		private int _position;

		private int _length;

		private int _capacity;

		private bool _expandable;

		private bool _writable;

		private bool _exposable;

		private bool _isOpen;

		[NonSerialized]
		private Task<int> _lastReadTask;

		private const int MemStreamMaxLength = 2147483647;
	}
}
