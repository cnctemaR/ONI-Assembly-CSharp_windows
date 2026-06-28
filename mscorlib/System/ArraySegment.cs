using System;

namespace System
{
	[Serializable]
	public struct ArraySegment<T>
	{
		public ArraySegment(T[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (offset > array.Length)
			{
				throw new ArgumentException("out of bounds");
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException("out of bounds", "offset");
			}
			this.array = array;
			this.offset = offset;
			this.count = count;
		}

		public ArraySegment(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			this.array = array;
			this.offset = 0;
			this.count = array.Length;
		}

		public T[] Array
		{
			get
			{
				return this.array;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ArraySegment<T> && this.Equals((ArraySegment<T>)obj);
		}

		public bool Equals(ArraySegment<T> obj)
		{
			return this.array == obj.Array && this.offset == obj.Offset && this.count == obj.Count;
		}

		public override int GetHashCode()
		{
			return this.array.GetHashCode() ^ this.offset ^ this.count;
		}

		public static bool operator ==(ArraySegment<T> a, ArraySegment<T> b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ArraySegment<T> a, ArraySegment<T> b)
		{
			return !a.Equals(b);
		}

		private T[] array;

		private int offset;

		private int count;
	}
}
