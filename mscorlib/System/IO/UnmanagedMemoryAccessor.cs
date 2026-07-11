using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.IO
{
	public class UnmanagedMemoryAccessor : IDisposable
	{
		protected UnmanagedMemoryAccessor()
		{
			this._isOpen = false;
		}

		[SecuritySafeCritical]
		public UnmanagedMemoryAccessor(SafeBuffer buffer, long offset, long capacity)
		{
			this.Initialize(buffer, offset, capacity, FileAccess.Read);
		}

		[SecuritySafeCritical]
		public UnmanagedMemoryAccessor(SafeBuffer buffer, long offset, long capacity, FileAccess access)
		{
			this.Initialize(buffer, offset, capacity, access);
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected unsafe void Initialize(SafeBuffer buffer, long offset, long capacity, FileAccess access)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0L)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Non-negative number required."));
			}
			if (capacity < 0L)
			{
				throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.ByteLength < (ulong)(offset + capacity))
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and capacity were greater than the size of the view."));
			}
			if (access < FileAccess.Read || access > FileAccess.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("access");
			}
			if (this._isOpen)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The method cannot be called twice on the same instance."));
			}
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				buffer.AcquirePointer(ref ptr);
				if (ptr + offset + capacity < ptr)
				{
					throw new ArgumentException(Environment.GetResourceString("The UnmanagedMemoryAccessor capacity and offset would wrap around the high end of the address space."));
				}
			}
			finally
			{
				if (ptr != null)
				{
					buffer.ReleasePointer();
				}
			}
			this._offset = offset;
			this._buffer = buffer;
			this._capacity = capacity;
			this._access = access;
			this._isOpen = true;
			this._canRead = (this._access & FileAccess.Read) > (FileAccess)0;
			this._canWrite = (this._access & FileAccess.Write) > (FileAccess)0;
		}

		public long Capacity
		{
			get
			{
				return this._capacity;
			}
		}

		public bool CanRead
		{
			get
			{
				return this._isOpen && this._canRead;
			}
		}

		public bool CanWrite
		{
			get
			{
				return this._isOpen && this._canWrite;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			this._isOpen = false;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected bool IsOpen
		{
			get
			{
				return this._isOpen;
			}
		}

		public bool ReadBoolean(long position)
		{
			int num = 1;
			this.EnsureSafeToRead(position, num);
			return this.InternalReadByte(position) > 0;
		}

		public byte ReadByte(long position)
		{
			int num = 1;
			this.EnsureSafeToRead(position, num);
			return this.InternalReadByte(position);
		}

		[SecuritySafeCritical]
		public unsafe char ReadChar(long position)
		{
			int num = 2;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			char c;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				c = (char)(*(ushort*)ptr);
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return c;
		}

		[SecuritySafeCritical]
		public unsafe short ReadInt16(long position)
		{
			int num = 2;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			short num2;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				num2 = *(short*)ptr;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return num2;
		}

		[SecuritySafeCritical]
		public unsafe int ReadInt32(long position)
		{
			int num = 4;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			int num2;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				num2 = *(int*)ptr;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return num2;
		}

		[SecuritySafeCritical]
		public unsafe long ReadInt64(long position)
		{
			int num = 8;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			long num2;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				num2 = *(long*)ptr;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return num2;
		}

		[SecuritySafeCritical]
		public decimal ReadDecimal(long position)
		{
			int num = 16;
			this.EnsureSafeToRead(position, num);
			int[] array = new int[4];
			this.ReadArray<int>(position, array, 0, array.Length);
			return new decimal(array);
		}

		[SecuritySafeCritical]
		public unsafe float ReadSingle(long position)
		{
			int num = 4;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			float num2;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				num2 = *(float*)ptr;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return num2;
		}

		[SecuritySafeCritical]
		public unsafe double ReadDouble(long position)
		{
			int num = 8;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			double num2;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				num2 = *(double*)ptr;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return num2;
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		public unsafe sbyte ReadSByte(long position)
		{
			int num = 1;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			sbyte b;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				b = *(sbyte*)ptr;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return b;
		}

		[CLSCompliant(false)]
		[SecuritySafeCritical]
		public unsafe ushort ReadUInt16(long position)
		{
			int num = 2;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			ushort num2;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				num2 = *(ushort*)ptr;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return num2;
		}

		[CLSCompliant(false)]
		[SecuritySafeCritical]
		public unsafe uint ReadUInt32(long position)
		{
			int num = 4;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			uint num2;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				num2 = *(uint*)ptr;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return num2;
		}

		[CLSCompliant(false)]
		[SecuritySafeCritical]
		public unsafe ulong ReadUInt64(long position)
		{
			int num = 8;
			this.EnsureSafeToRead(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			ulong num2;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				num2 = (ulong)(*(long*)ptr);
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return num2;
		}

		[SecurityCritical]
		public void Read<T>(long position, out T structure) where T : struct
		{
			if (position < 0L)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("Non-negative number required."));
			}
			if (!this._isOpen)
			{
				throw new ObjectDisposedException("UnmanagedMemoryAccessor", Environment.GetResourceString("Cannot access a closed accessor."));
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException(Environment.GetResourceString("Accessor does not support reading."));
			}
			uint num = Marshal.SizeOfType(typeof(T));
			if (position <= this._capacity - (long)((ulong)num))
			{
				structure = this._buffer.Read<T>((ulong)(this._offset + position));
				return;
			}
			if (position >= this._capacity)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("The position may not be greater or equal to the capacity of the accessor."));
			}
			throw new ArgumentException(Environment.GetResourceString("There are not enough bytes remaining in the accessor to read at this position.", new object[] { typeof(T).FullName }), "position");
		}

		[SecurityCritical]
		public int ReadArray<T>(long position, T[] array, int offset, int count) where T : struct
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Buffer cannot be null.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (!this.CanRead)
			{
				if (!this._isOpen)
				{
					throw new ObjectDisposedException("UnmanagedMemoryAccessor", Environment.GetResourceString("Cannot access a closed accessor."));
				}
				throw new NotSupportedException(Environment.GetResourceString("Accessor does not support reading."));
			}
			else
			{
				if (position < 0L)
				{
					throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("Non-negative number required."));
				}
				uint num = Marshal.AlignedSizeOf<T>();
				if (position >= this._capacity)
				{
					throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("The position may not be greater or equal to the capacity of the accessor."));
				}
				int num2 = count;
				long num3 = this._capacity - position;
				if (num3 < 0L)
				{
					num2 = 0;
				}
				else
				{
					ulong num4 = (ulong)num * (ulong)((long)count);
					if (num3 < (long)num4)
					{
						num2 = (int)(num3 / (long)((ulong)num));
					}
				}
				this._buffer.ReadArray<T>((ulong)(this._offset + position), array, offset, num2);
				return num2;
			}
		}

		public void Write(long position, bool value)
		{
			int num = 1;
			this.EnsureSafeToWrite(position, num);
			byte b = (value ? 1 : 0);
			this.InternalWrite(position, b);
		}

		public void Write(long position, byte value)
		{
			int num = 1;
			this.EnsureSafeToWrite(position, num);
			this.InternalWrite(position, value);
		}

		[SecuritySafeCritical]
		public unsafe void Write(long position, char value)
		{
			int num = 2;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(short*)ptr = (short)value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe void Write(long position, short value)
		{
			int num = 2;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(short*)ptr = value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe void Write(long position, int value)
		{
			int num = 4;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(int*)ptr = value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe void Write(long position, long value)
		{
			int num = 8;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(long*)ptr = value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		public void Write(long position, decimal value)
		{
			int num = 16;
			this.EnsureSafeToWrite(position, num);
			byte[] array = new byte[16];
			decimal.GetBytes(value, array);
			int[] array2 = new int[4];
			int num2 = (int)array[12] | ((int)array[13] << 8) | ((int)array[14] << 16) | ((int)array[15] << 24);
			int num3 = (int)array[0] | ((int)array[1] << 8) | ((int)array[2] << 16) | ((int)array[3] << 24);
			int num4 = (int)array[4] | ((int)array[5] << 8) | ((int)array[6] << 16) | ((int)array[7] << 24);
			int num5 = (int)array[8] | ((int)array[9] << 8) | ((int)array[10] << 16) | ((int)array[11] << 24);
			array2[0] = num3;
			array2[1] = num4;
			array2[2] = num5;
			array2[3] = num2;
			this.WriteArray<int>(position, array2, 0, array2.Length);
		}

		[SecuritySafeCritical]
		public unsafe void Write(long position, float value)
		{
			int num = 4;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(float*)ptr = value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe void Write(long position, double value)
		{
			int num = 8;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(double*)ptr = value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		public unsafe void Write(long position, sbyte value)
		{
			int num = 1;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*ptr = (byte)value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		public unsafe void Write(long position, ushort value)
		{
			int num = 2;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(short*)ptr = (short)value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		public unsafe void Write(long position, uint value)
		{
			int num = 4;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(int*)ptr = (int)value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		public unsafe void Write(long position, ulong value)
		{
			int num = 8;
			this.EnsureSafeToWrite(position, num);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				ptr += this._offset + position;
				*(long*)ptr = (long)value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		[SecurityCritical]
		public void Write<T>(long position, ref T structure) where T : struct
		{
			if (position < 0L)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("Non-negative number required."));
			}
			if (!this._isOpen)
			{
				throw new ObjectDisposedException("UnmanagedMemoryAccessor", Environment.GetResourceString("Cannot access a closed accessor."));
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException(Environment.GetResourceString("Accessor does not support writing."));
			}
			uint num = Marshal.SizeOfType(typeof(T));
			if (position <= this._capacity - (long)((ulong)num))
			{
				this._buffer.Write<T>((ulong)(this._offset + position), structure);
				return;
			}
			if (position >= this._capacity)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("The position may not be greater or equal to the capacity of the accessor."));
			}
			throw new ArgumentException(Environment.GetResourceString("There are not enough bytes remaining in the accessor to write at this position.", new object[] { typeof(T).FullName }), "position");
		}

		[SecurityCritical]
		public void WriteArray<T>(long position, T[] array, int offset, int count) where T : struct
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Buffer cannot be null.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (position < 0L)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("Non-negative number required."));
			}
			if (position >= this.Capacity)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("The position may not be greater or equal to the capacity of the accessor."));
			}
			if (!this._isOpen)
			{
				throw new ObjectDisposedException("UnmanagedMemoryAccessor", Environment.GetResourceString("Cannot access a closed accessor."));
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException(Environment.GetResourceString("Accessor does not support writing."));
			}
			this._buffer.WriteArray<T>((ulong)(this._offset + position), array, offset, count);
		}

		[SecuritySafeCritical]
		private unsafe byte InternalReadByte(long position)
		{
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			byte b;
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				b = (ptr + this._offset)[position];
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
			return b;
		}

		[SecuritySafeCritical]
		private unsafe void InternalWrite(long position, byte value)
		{
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this._buffer.AcquirePointer(ref ptr);
				(ptr + this._offset)[position] = value;
			}
			finally
			{
				if (ptr != null)
				{
					this._buffer.ReleasePointer();
				}
			}
		}

		private void EnsureSafeToRead(long position, int sizeOfType)
		{
			if (!this._isOpen)
			{
				throw new ObjectDisposedException("UnmanagedMemoryAccessor", Environment.GetResourceString("Cannot access a closed accessor."));
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException(Environment.GetResourceString("Accessor does not support reading."));
			}
			if (position < 0L)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("Non-negative number required."));
			}
			if (position <= this._capacity - (long)sizeOfType)
			{
				return;
			}
			if (position >= this._capacity)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("The position may not be greater or equal to the capacity of the accessor."));
			}
			throw new ArgumentException(Environment.GetResourceString("There are not enough bytes remaining in the accessor to read at this position."), "position");
		}

		private void EnsureSafeToWrite(long position, int sizeOfType)
		{
			if (!this._isOpen)
			{
				throw new ObjectDisposedException("UnmanagedMemoryAccessor", Environment.GetResourceString("Cannot access a closed accessor."));
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException(Environment.GetResourceString("Accessor does not support writing."));
			}
			if (position < 0L)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("Non-negative number required."));
			}
			if (position <= this._capacity - (long)sizeOfType)
			{
				return;
			}
			if (position >= this._capacity)
			{
				throw new ArgumentOutOfRangeException("position", Environment.GetResourceString("The position may not be greater or equal to the capacity of the accessor."));
			}
			throw new ArgumentException(Environment.GetResourceString("There are not enough bytes remaining in the accessor to write at this position.", new object[] { "Byte" }), "position");
		}

		[SecurityCritical]
		private SafeBuffer _buffer;

		private long _offset;

		private long _capacity;

		private FileAccess _access;

		private bool _isOpen;

		private bool _canRead;

		private bool _canWrite;
	}
}
