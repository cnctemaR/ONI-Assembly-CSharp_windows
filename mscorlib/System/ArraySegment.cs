using System;
using System.Collections;
using System.Collections.Generic;

namespace System
{
	[Serializable]
	public struct ArraySegment<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T>
	{
		public ArraySegment(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			this._array = array;
			this._offset = 0;
			this._count = array.Length;
		}

		public ArraySegment(T[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
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
			this._array = array;
			this._offset = offset;
			this._count = count;
		}

		public T[] Array
		{
			get
			{
				return this._array;
			}
		}

		public int Offset
		{
			get
			{
				return this._offset;
			}
		}

		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public override int GetHashCode()
		{
			if (this._array != null)
			{
				return this._array.GetHashCode() ^ this._offset ^ this._count;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			return obj is ArraySegment<T> && this.Equals((ArraySegment<T>)obj);
		}

		public bool Equals(ArraySegment<T> obj)
		{
			return obj._array == this._array && obj._offset == this._offset && obj._count == this._count;
		}

		public static bool operator ==(ArraySegment<T> a, ArraySegment<T> b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ArraySegment<T> a, ArraySegment<T> b)
		{
			return !(a == b);
		}

		T IList<T>.this[int index]
		{
			get
			{
				if (this._array == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("The underlying array is null."));
				}
				if (index < 0 || index >= this._count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._array[this._offset + index];
			}
			set
			{
				if (this._array == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("The underlying array is null."));
				}
				if (index < 0 || index >= this._count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this._array[this._offset + index] = value;
			}
		}

		int IList<T>.IndexOf(T item)
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The underlying array is null."));
			}
			int num = global::System.Array.IndexOf<T>(this._array, item, this._offset, this._count);
			if (num < 0)
			{
				return -1;
			}
			return num - this._offset;
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		T IReadOnlyList<T>.this[int index]
		{
			get
			{
				if (this._array == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("The underlying array is null."));
				}
				if (index < 0 || index >= this._count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._array[this._offset + index];
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Contains(T item)
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The underlying array is null."));
			}
			return global::System.Array.IndexOf<T>(this._array, item, this._offset, this._count) >= 0;
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The underlying array is null."));
			}
			global::System.Array.Copy(this._array, this._offset, array, arrayIndex, this._count);
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The underlying array is null."));
			}
			return new ArraySegment<T>.ArraySegmentEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The underlying array is null."));
			}
			return new ArraySegment<T>.ArraySegmentEnumerator(this);
		}

		private T[] _array;

		private int _offset;

		private int _count;

		[Serializable]
		private sealed class ArraySegmentEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal ArraySegmentEnumerator(ArraySegment<T> arraySegment)
			{
				this._array = arraySegment._array;
				this._start = arraySegment._offset;
				this._end = this._start + arraySegment._count;
				this._current = this._start - 1;
			}

			public bool MoveNext()
			{
				if (this._current < this._end)
				{
					this._current++;
					return this._current < this._end;
				}
				return false;
			}

			public T Current
			{
				get
				{
					if (this._current < this._start)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Enumeration has not started. Call MoveNext."));
					}
					if (this._current >= this._end)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Enumeration already finished."));
					}
					return this._array[this._current];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				this._current = this._start - 1;
			}

			public void Dispose()
			{
			}

			private T[] _array;

			private int _start;

			private int _end;

			private int _current;
		}
	}
}
