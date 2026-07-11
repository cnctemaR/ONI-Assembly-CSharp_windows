using System;

namespace MIConvexHull
{
	internal class SimpleList<T>
	{
		public T this[int i]
		{
			get
			{
				return this.items[i];
			}
			set
			{
				this.items[i] = value;
			}
		}

		private void EnsureCapacity()
		{
			if (this.capacity == 0)
			{
				this.capacity = 32;
				this.items = new T[32];
				return;
			}
			T[] array = new T[this.capacity * 2];
			Array.Copy(this.items, array, this.capacity);
			this.capacity = 2 * this.capacity;
			this.items = array;
		}

		public void Add(T item)
		{
			if (this.Count + 1 > this.capacity)
			{
				this.EnsureCapacity();
			}
			T[] array = this.items;
			int count = this.Count;
			this.Count = count + 1;
			array[count] = item;
		}

		public void Push(T item)
		{
			if (this.Count + 1 > this.capacity)
			{
				this.EnsureCapacity();
			}
			T[] array = this.items;
			int count = this.Count;
			this.Count = count + 1;
			array[count] = item;
		}

		public T Pop()
		{
			T[] array = this.items;
			int num = this.Count - 1;
			this.Count = num;
			return array[num];
		}

		public void Clear()
		{
			this.Count = 0;
		}

		private int capacity;

		public int Count;

		private T[] items;
	}
}
