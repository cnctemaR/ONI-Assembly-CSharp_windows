using System;
using System.Runtime.CompilerServices;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class InsertionQueue<[Nullable(2)] T>
	{
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			private set; }

		public InsertionQueue(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.array = new T[capacity];
			this.headIndex = (this.tailIndex = (this.Count = 0));
		}

		public void Clear()
		{
			this.headIndex = (this.tailIndex = (this.Count = 0));
		}

		public T Peek()
		{
			if (this.Count == 0)
			{
				InsertionQueue<T>.ThrowForEmptyQueue();
			}
			return this.array[this.headIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Enqueue(T item)
		{
			if (this.Count == this.array.Length)
			{
				this.Grow();
			}
			this.array[this.tailIndex] = item;
			this.MoveNext(ref this.tailIndex);
			int count = this.Count;
			this.Count = count + 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Dequeue()
		{
			if (this.Count == 0)
			{
				InsertionQueue<T>.ThrowForEmptyQueue();
			}
			T t = this.array[this.headIndex];
			this.MoveNext(ref this.headIndex);
			int count = this.Count;
			this.Count = count - 1;
			return t;
		}

		public void Insert(int posTo, T item)
		{
			if (this.Count == this.array.Length)
			{
				this.Grow();
			}
			this.MoveNext(ref this.tailIndex);
			int count = this.Count;
			this.Count = count + 1;
			for (int i = this.Count - 1; i > posTo; i--)
			{
				int num = (this.headIndex + i) % this.array.Length;
				int num2 = ((num == 0) ? (this.array.Length - 1) : (num - 1));
				this.array[num] = this.array[num2];
			}
			this.array[(posTo + this.headIndex) % this.array.Length] = item;
		}

		private void Grow()
		{
			int num = (int)((long)this.array.Length * 200L / 100L);
			if (num < this.array.Length + 4)
			{
				num = this.array.Length + 4;
			}
			this.SetCapacity(num);
		}

		private void SetCapacity(int capacity)
		{
			T[] array = new T[capacity];
			if (this.Count > 0)
			{
				if (this.headIndex < this.tailIndex)
				{
					Array.Copy(this.array, this.headIndex, array, 0, this.Count);
				}
				else
				{
					Array.Copy(this.array, this.headIndex, array, 0, this.array.Length - this.headIndex);
					Array.Copy(this.array, 0, array, this.array.Length - this.headIndex, this.tailIndex);
				}
			}
			this.array = array;
			this.headIndex = 0;
			this.tailIndex = ((this.Count == capacity) ? 0 : this.Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MoveNext(ref int index)
		{
			index = (index + 1) % this.array.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ThrowForEmptyQueue()
		{
			throw new InvalidOperationException("EmptyQueue");
		}

		private const int MinimumGrow = 4;

		private const int GrowFactor = 200;

		private T[] array;

		private int headIndex;

		private int tailIndex;
	}
}
