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
			}
			else
			{
				T[] array = new T[this.capacity * 2];
				Array.Copy(this.items, array, this.capacity);
				this.capacity = 2 * this.capacity;
				this.items = array;
			}
		}

		public void Add(T item)
		{
			if (this.Count + 1 > this.capacity)
			{
				this.EnsureCapacity();
			}
			this.items[this.Count++] = item;
		}

		public void Push(T item)
		{
			if (this.Count + 1 > this.capacity)
			{
				this.EnsureCapacity();
			}
			this.items[this.Count++] = item;
		}

		public T Pop()
		{
			return this.items[--this.Count];
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
