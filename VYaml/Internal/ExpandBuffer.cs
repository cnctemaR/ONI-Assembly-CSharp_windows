using System;
using System.Runtime.CompilerServices;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class ExpandBuffer<[Nullable(2)] T>
	{
		public int Length { get; private set; }

		public ExpandBuffer(int capacity)
		{
			this.buffer = new T[capacity];
			this.Length = 0;
		}

		public ref T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ref this.buffer[index];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		public Span<T> AsSpan()
		{
			return this.buffer.AsSpan<T>(0, this.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		public Span<T> AsSpan(int length)
		{
			if (length > this.buffer.Length)
			{
				this.SetCapacity(this.buffer.Length * 2);
			}
			return this.buffer.AsSpan<T>(0, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			this.Length = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Peek()
		{
			return ref this.buffer[this.Length - 1];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Pop()
		{
			if (this.Length == 0)
			{
				throw new InvalidOperationException("Cannot pop the empty buffer");
			}
			T[] array = this.buffer;
			int num = this.Length - 1;
			this.Length = num;
			return ref array[num];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryPop(out T value)
		{
			if (this.Length == 0)
			{
				value = default(T);
				return false;
			}
			value = *this.Pop();
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			if (this.Length >= this.buffer.Length)
			{
				this.Grow();
			}
			T[] array = this.buffer;
			int length = this.Length;
			this.Length = length + 1;
			array[length] = item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetCapacity(int newCapacity)
		{
			if (this.buffer.Length >= newCapacity)
			{
				return;
			}
			T[] array = new T[newCapacity];
			this.buffer.AsSpan<T>(0, this.Length).CopyTo(array);
			this.buffer = array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Grow()
		{
			int num = this.buffer.Length * 200 / 100;
			if (num < this.buffer.Length + 4)
			{
				num = this.buffer.Length + 4;
			}
			this.SetCapacity(num);
		}

		private const int MinimumGrow = 4;

		private const int GrowFactor = 200;

		private T[] buffer;
	}
}
