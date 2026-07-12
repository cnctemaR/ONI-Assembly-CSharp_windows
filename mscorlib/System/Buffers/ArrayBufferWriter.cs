using System;

namespace System.Buffers
{
	public sealed class ArrayBufferWriter<T> : IBufferWriter<T>
	{
		public ArrayBufferWriter()
		{
			this._buffer = Array.Empty<T>();
			this._index = 0;
		}

		public ArrayBufferWriter(int initialCapacity)
		{
			if (initialCapacity <= 0)
			{
				throw new ArgumentException("initialCapacity");
			}
			this._buffer = new T[initialCapacity];
			this._index = 0;
		}

		public ReadOnlyMemory<T> WrittenMemory
		{
			get
			{
				return this._buffer.AsMemory<T>(0, this._index);
			}
		}

		public ReadOnlySpan<T> WrittenSpan
		{
			get
			{
				return this._buffer.AsSpan<T>(0, this._index);
			}
		}

		public int WrittenCount
		{
			get
			{
				return this._index;
			}
		}

		public int Capacity
		{
			get
			{
				return this._buffer.Length;
			}
		}

		public int FreeCapacity
		{
			get
			{
				return this._buffer.Length - this._index;
			}
		}

		public void Clear()
		{
			this._buffer.AsSpan<T>(0, this._index).Clear();
			this._index = 0;
		}

		public void Advance(int count)
		{
			if (count < 0)
			{
				throw new ArgumentException("count");
			}
			if (this._index > this._buffer.Length - count)
			{
				ArrayBufferWriter<T>.ThrowInvalidOperationException_AdvancedTooFar(this._buffer.Length);
			}
			this._index += count;
		}

		public Memory<T> GetMemory(int sizeHint = 0)
		{
			this.CheckAndResizeBuffer(sizeHint);
			return this._buffer.AsMemory<T>(this._index);
		}

		public Span<T> GetSpan(int sizeHint = 0)
		{
			this.CheckAndResizeBuffer(sizeHint);
			return this._buffer.AsSpan<T>(this._index);
		}

		private void CheckAndResizeBuffer(int sizeHint)
		{
			if (sizeHint < 0)
			{
				throw new ArgumentException("sizeHint");
			}
			if (sizeHint == 0)
			{
				sizeHint = 1;
			}
			if (sizeHint > this.FreeCapacity)
			{
				int num = Math.Max(sizeHint, this._buffer.Length);
				if (this._buffer.Length == 0)
				{
					num = Math.Max(num, 256);
				}
				int num2 = checked(this._buffer.Length + num);
				Array.Resize<T>(ref this._buffer, num2);
			}
		}

		private static void ThrowInvalidOperationException_AdvancedTooFar(int capacity)
		{
			throw new InvalidOperationException(SR.Format("Cannot advance past the end of the buffer, which has a size of {0}.", capacity));
		}

		private T[] _buffer;

		private int _index;

		private const int DefaultInitialBufferSize = 256;
	}
}
