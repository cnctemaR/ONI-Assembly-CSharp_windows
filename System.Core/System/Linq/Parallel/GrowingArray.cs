using System;

namespace System.Linq.Parallel
{
	internal class GrowingArray<T>
	{
		internal GrowingArray()
		{
			this._array = new T[1024];
			this._count = 0;
		}

		internal T[] InternalArray
		{
			get
			{
				return this._array;
			}
		}

		internal int Count
		{
			get
			{
				return this._count;
			}
		}

		internal void Add(T element)
		{
			if (this._count >= this._array.Length)
			{
				this.GrowArray(2 * this._array.Length);
			}
			T[] array = this._array;
			int count = this._count;
			this._count = count + 1;
			array[count] = element;
		}

		private void GrowArray(int newSize)
		{
			T[] array = new T[newSize];
			this._array.CopyTo(array, 0);
			this._array = array;
		}

		internal void CopyFrom(T[] otherArray, int otherCount)
		{
			if (this._count + otherCount > this._array.Length)
			{
				this.GrowArray(this._count + otherCount);
			}
			Array.Copy(otherArray, 0, this._array, this._count, otherCount);
			this._count += otherCount;
		}

		private T[] _array;

		private int _count;

		private const int DEFAULT_ARRAY_SIZE = 1024;
	}
}
