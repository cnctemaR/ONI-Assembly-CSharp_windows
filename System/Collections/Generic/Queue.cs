using System;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
	[ComVisible(false)]
	[Serializable]
	public class Queue<T> : IEnumerable<T>, ICollection, IEnumerable
	{
		public Queue()
		{
			this._array = new T[0];
		}

		public Queue(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this._array = new T[count];
		}

		public Queue(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			ICollection<T> collection2 = collection as ICollection<T>;
			int num = ((collection2 == null) ? 0 : collection2.Count);
			this._array = new T[num];
			foreach (T t in collection)
			{
				this.Enqueue(t);
			}
		}

		void ICollection.CopyTo(Array array, int idx)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (idx > array.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Length - idx < this._size)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (this._size == 0)
			{
				return;
			}
			try
			{
				int num = this._array.Length;
				int num2 = num - this._head;
				Array.Copy(this._array, this._head, array, idx, Math.Min(this._size, num2));
				if (this._size > num2)
				{
					Array.Copy(this._array, 0, array, idx + num2, this._size - num2);
				}
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException();
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
			Array.Clear(this._array, 0, this._array.Length);
			this._head = (this._tail = (this._size = 0));
			this._version++;
		}

		public bool Contains(T item)
		{
			if (item == null)
			{
				foreach (T t in this)
				{
					if (t == null)
					{
						return true;
					}
				}
			}
			else
			{
				foreach (T t2 in this)
				{
					if (item.Equals(t2))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CopyTo(T[] array, int idx)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			((ICollection)this).CopyTo(array, idx);
		}

		public T Dequeue()
		{
			T t = this.Peek();
			this._array[this._head] = default(T);
			if (++this._head == this._array.Length)
			{
				this._head = 0;
			}
			this._size--;
			this._version++;
			return t;
		}

		public T Peek()
		{
			if (this._size == 0)
			{
				throw new InvalidOperationException();
			}
			return this._array[this._head];
		}

		public void Enqueue(T item)
		{
			if (this._size == this._array.Length || this._tail == this._array.Length)
			{
				this.SetCapacity(Math.Max(Math.Max(this._size, this._tail) * 2, 4));
			}
			this._array[this._tail] = item;
			if (++this._tail == this._array.Length)
			{
				this._tail = 0;
			}
			this._size++;
			this._version++;
		}

		public T[] ToArray()
		{
			T[] array = new T[this._size];
			this.CopyTo(array, 0);
			return array;
		}

		public void TrimExcess()
		{
			if ((double)this._size < (double)this._array.Length * 0.9)
			{
				this.SetCapacity(this._size);
			}
		}

		private void SetCapacity(int new_size)
		{
			if (new_size == this._array.Length)
			{
				return;
			}
			if (new_size < this._size)
			{
				throw new InvalidOperationException("shouldnt happen");
			}
			T[] array = new T[new_size];
			if (this._size > 0)
			{
				this.CopyTo(array, 0);
			}
			this._array = array;
			this._tail = this._size;
			this._head = 0;
			this._version++;
		}

		public int Count
		{
			get
			{
				return this._size;
			}
		}

		public Queue<T>.Enumerator GetEnumerator()
		{
			return new Queue<T>.Enumerator(this);
		}

		private T[] _array;

		private int _head;

		private int _tail;

		private int _size;

		private int _version;

		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
		{
			internal Enumerator(Queue<T> q)
			{
				this.q = q;
				this.idx = -2;
				this.ver = q._version;
			}

			void IEnumerator.Reset()
			{
				if (this.ver != this.q._version)
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
				if (this.ver != this.q._version)
				{
					throw new InvalidOperationException();
				}
				if (this.idx == -2)
				{
					this.idx = this.q._size;
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
					return this.q._array[(this.q._size - 1 - this.idx + this.q._head) % this.q._array.Length];
				}
			}

			private const int NOT_STARTED = -2;

			private const int FINISHED = -1;

			private Queue<T> q;

			private int idx;

			private int ver;
		}
	}
}
