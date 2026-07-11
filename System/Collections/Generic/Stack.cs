using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Collections.Generic
{
	[DebuggerTypeProxy(typeof(StackDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class Stack<T> : IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
	{
		public Stack()
		{
			this._array = Array.Empty<T>();
		}

		public Stack(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", capacity, "Non-negative number required.");
			}
			this._array = new T[capacity];
		}

		public Stack(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this._array = EnumerableHelpers.ToArray<T>(collection, out this._size);
		}

		public int Count
		{
			get
			{
				return this._size;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		public void Clear()
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				Array.Clear(this._array, 0, this._size);
			}
			this._size = 0;
			this._version++;
		}

		public bool Contains(T item)
		{
			return this._size != 0 && Array.LastIndexOf<T>(this._array, item, this._size - 1) != -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (array.Length - arrayIndex < this._size)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			int i = 0;
			int num = arrayIndex + this._size;
			while (i < this._size)
			{
				array[--num] = this._array[i++];
			}
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("The lower bound of target array must be zero.", "array");
			}
			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (array.Length - arrayIndex < this._size)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			try
			{
				Array.Copy(this._array, 0, array, arrayIndex, this._size);
				Array.Reverse(array, arrayIndex, this._size);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
			}
		}

		public Stack<T>.Enumerator GetEnumerator()
		{
			return new Stack<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Stack<T>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Stack<T>.Enumerator(this);
		}

		public void TrimExcess()
		{
			int num = (int)((double)this._array.Length * 0.9);
			if (this._size < num)
			{
				Array.Resize<T>(ref this._array, this._size);
				this._version++;
			}
		}

		public T Peek()
		{
			if (this._size == 0)
			{
				this.ThrowForEmptyStack();
			}
			return this._array[this._size - 1];
		}

		public bool TryPeek(out T result)
		{
			if (this._size == 0)
			{
				result = default(T);
				return false;
			}
			result = this._array[this._size - 1];
			return true;
		}

		public T Pop()
		{
			if (this._size == 0)
			{
				this.ThrowForEmptyStack();
			}
			this._version++;
			T[] array = this._array;
			int num = this._size - 1;
			this._size = num;
			T t = array[num];
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				this._array[this._size] = default(T);
			}
			return t;
		}

		public bool TryPop(out T result)
		{
			if (this._size == 0)
			{
				result = default(T);
				return false;
			}
			this._version++;
			T[] array = this._array;
			int num = this._size - 1;
			this._size = num;
			result = array[num];
			if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			{
				this._array[this._size] = default(T);
			}
			return true;
		}

		public void Push(T item)
		{
			if (this._size == this._array.Length)
			{
				Array.Resize<T>(ref this._array, (this._array.Length == 0) ? 4 : (2 * this._array.Length));
			}
			T[] array = this._array;
			int size = this._size;
			this._size = size + 1;
			array[size] = item;
			this._version++;
		}

		public T[] ToArray()
		{
			if (this._size == 0)
			{
				return Array.Empty<T>();
			}
			T[] array = new T[this._size];
			for (int i = 0; i < this._size; i++)
			{
				array[i] = this._array[this._size - i - 1];
			}
			return array;
		}

		private void ThrowForEmptyStack()
		{
			throw new InvalidOperationException("Stack empty.");
		}

		private T[] _array;

		private int _size;

		private int _version;

		[NonSerialized]
		private object _syncRoot;

		private const int DefaultCapacity = 4;

		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(Stack<T> stack)
			{
				this._stack = stack;
				this._version = stack._version;
				this._index = -2;
				this._currentElement = default(T);
			}

			public void Dispose()
			{
				this._index = -1;
			}

			public bool MoveNext()
			{
				if (this._version != this._stack._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				if (this._index == -2)
				{
					this._index = this._stack._size - 1;
					bool flag = this._index >= 0;
					if (flag)
					{
						this._currentElement = this._stack._array[this._index];
					}
					return flag;
				}
				if (this._index == -1)
				{
					return false;
				}
				int num = this._index - 1;
				this._index = num;
				bool flag2 = num >= 0;
				if (flag2)
				{
					this._currentElement = this._stack._array[this._index];
					return flag2;
				}
				this._currentElement = default(T);
				return flag2;
			}

			public T Current
			{
				get
				{
					if (this._index < 0)
					{
						this.ThrowEnumerationNotStartedOrEnded();
					}
					return this._currentElement;
				}
			}

			private void ThrowEnumerationNotStartedOrEnded()
			{
				throw new InvalidOperationException((this._index == -2) ? "Enumeration has not started. Call MoveNext." : "Enumeration already finished.");
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
				if (this._version != this._stack._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._index = -2;
				this._currentElement = default(T);
			}

			private readonly Stack<T> _stack;

			private readonly int _version;

			private int _index;

			private T _currentElement;
		}
	}
}
