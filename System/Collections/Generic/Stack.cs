using System;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
	[ComVisible(false)]
	[Serializable]
	public class Stack<T> : ICollection, IEnumerable<T>, IEnumerable
	{
		public Stack()
		{
		}

		public Stack(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this._array = new T[count];
		}

		public Stack(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				this._size = collection2.Count;
				this._array = new T[this._size];
				collection2.CopyTo(this._array, 0);
			}
			else
			{
				foreach (T t in collection)
				{
					this.Push(t);
				}
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
				return this;
			}
		}

		void ICollection.CopyTo(Array dest, int idx)
		{
			try
			{
				if (this._array != null)
				{
					this._array.CopyTo(dest, idx);
					Array.Reverse(dest, idx, this._size);
				}
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException();
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Clear()
		{
			if (this._array != null)
			{
				Array.Clear(this._array, 0, this._array.Length);
			}
			this._size = 0;
			this._version++;
		}

		public bool Contains(T t)
		{
			return this._array != null && Array.IndexOf<T>(this._array, t, 0, this._size) != -1;
		}

		public void CopyTo(T[] dest, int idx)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (idx < 0)
			{
				throw new ArgumentOutOfRangeException("idx");
			}
			if (this._array != null)
			{
				Array.Copy(this._array, 0, dest, idx, this._size);
				Array.Reverse(dest, idx, this._size);
			}
		}

		public T Peek()
		{
			if (this._size == 0)
			{
				throw new InvalidOperationException();
			}
			return this._array[this._size - 1];
		}

		public T Pop()
		{
			if (this._size == 0)
			{
				throw new InvalidOperationException();
			}
			this._version++;
			T t = this._array[--this._size];
			this._array[this._size] = default(T);
			return t;
		}

		public void Push(T t)
		{
			if (this._array == null || this._size == this._array.Length)
			{
				Array.Resize<T>(ref this._array, (this._size != 0) ? (2 * this._size) : 16);
			}
			this._version++;
			this._array[this._size++] = t;
		}

		public T[] ToArray()
		{
			T[] array = new T[this._size];
			this.CopyTo(array, 0);
			return array;
		}

		public void TrimExcess()
		{
			if (this._array != null && (double)this._size < (double)this._array.Length * 0.9)
			{
				Array.Resize<T>(ref this._array, this._size);
			}
			this._version++;
		}

		public int Count
		{
			get
			{
				return this._size;
			}
		}

		public Stack<T>.Enumerator GetEnumerator()
		{
			return new Stack<T>.Enumerator(this);
		}

		private const int INITIAL_SIZE = 16;

		private T[] _array;

		private int _size;

		private int _version;

		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
		{
			internal Enumerator(Stack<T> t)
			{
				this.parent = t;
				this.idx = -2;
				this._version = t._version;
			}

			void IEnumerator.Reset()
			{
				if (this._version != this.parent._version)
				{
					throw new InvalidOperationException();
				}
				this.idx = -2;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
				this.idx = -2;
			}

			public bool MoveNext()
			{
				if (this._version != this.parent._version)
				{
					throw new InvalidOperationException();
				}
				if (this.idx == -2)
				{
					this.idx = this.parent._size;
				}
				return this.idx != -1 && --this.idx != -1;
			}

			public T Current
			{
				get
				{
					if (this.idx < 0)
					{
						throw new InvalidOperationException();
					}
					return this.parent._array[this.idx];
				}
			}

			private const int NOT_STARTED = -2;

			private const int FINISHED = -1;

			private Stack<T> parent;

			private int idx;

			private int _version;
		}
	}
}
