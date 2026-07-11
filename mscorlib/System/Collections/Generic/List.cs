using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace System.Collections.Generic
{
	[DebuggerDisplay("Count={Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView<>))]
	[Serializable]
	public class List<T> : IEnumerable, ICollection, IList, ICollection<T>, IEnumerable<T>, IList<T>
	{
		public List()
		{
			this._items = List<T>.EmptyArray;
		}

		public List(IEnumerable<T> collection)
		{
			this.CheckCollection(collection);
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 == null)
			{
				this._items = List<T>.EmptyArray;
				this.AddEnumerable(collection);
			}
			else
			{
				this._items = new T[collection2.Count];
				this.AddCollection(collection2);
			}
		}

		public List(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this._items = new T[capacity];
		}

		internal List(T[] data, int size)
		{
			this._items = data;
			this._size = size;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			Array.Copy(this._items, 0, array, arrayIndex, this._size);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		int IList.Add(object item)
		{
			try
			{
				this.Add((T)((object)item));
				return this._size - 1;
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			throw new ArgumentException("item");
		}

		bool IList.Contains(object item)
		{
			try
			{
				return this.Contains((T)((object)item));
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			return false;
		}

		int IList.IndexOf(object item)
		{
			try
			{
				return this.IndexOf((T)((object)item));
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			return -1;
		}

		void IList.Insert(int index, object item)
		{
			this.CheckIndex(index);
			try
			{
				this.Insert(index, (T)((object)item));
				return;
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			throw new ArgumentException("item");
		}

		void IList.Remove(object item)
		{
			try
			{
				this.Remove((T)((object)item));
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
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

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				try
				{
					this[index] = (T)((object)value);
					return;
				}
				catch (NullReferenceException)
				{
				}
				catch (InvalidCastException)
				{
				}
				throw new ArgumentException("value");
			}
		}

		public void Add(T item)
		{
			if (this._size == this._items.Length)
			{
				this.GrowIfNeeded(1);
			}
			this._items[this._size++] = item;
			this._version++;
		}

		private void GrowIfNeeded(int newCount)
		{
			int num = this._size + newCount;
			if (num > this._items.Length)
			{
				this.Capacity = Math.Max(Math.Max(this.Capacity * 2, 4), num);
			}
		}

		private void CheckRange(int idx, int count)
		{
			if (idx < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (idx + count > this._size)
			{
				throw new ArgumentException("index and count exceed length of list");
			}
		}

		private void AddCollection(ICollection<T> collection)
		{
			int count = collection.Count;
			if (count == 0)
			{
				return;
			}
			this.GrowIfNeeded(count);
			collection.CopyTo(this._items, this._size);
			this._size += count;
		}

		private void AddEnumerable(IEnumerable<T> enumerable)
		{
			foreach (T t in enumerable)
			{
				this.Add(t);
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			this.CheckCollection(collection);
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				this.AddCollection(collection2);
			}
			else
			{
				this.AddEnumerable(collection);
			}
			this._version++;
		}

		public ReadOnlyCollection<T> AsReadOnly()
		{
			return new ReadOnlyCollection<T>(this);
		}

		public int BinarySearch(T item)
		{
			return Array.BinarySearch<T>(this._items, 0, this._size, item);
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return Array.BinarySearch<T>(this._items, 0, this._size, item, comparer);
		}

		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			this.CheckRange(index, count);
			return Array.BinarySearch<T>(this._items, index, count, item, comparer);
		}

		public void Clear()
		{
			Array.Clear(this._items, 0, this._items.Length);
			this._size = 0;
			this._version++;
		}

		public bool Contains(T item)
		{
			return Array.IndexOf<T>(this._items, item, 0, this._size) != -1;
		}

		public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
		{
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			List<TOutput> list = new List<TOutput>(this._size);
			for (int i = 0; i < this._size; i++)
			{
				list._items[i] = converter(this._items[i]);
			}
			list._size = this._size;
			return list;
		}

		public void CopyTo(T[] array)
		{
			Array.Copy(this._items, 0, array, 0, this._size);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(this._items, 0, array, arrayIndex, this._size);
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			this.CheckRange(index, count);
			Array.Copy(this._items, index, array, arrayIndex, count);
		}

		public bool Exists(Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			return this.GetIndex(0, this._size, match) != -1;
		}

		public T Find(Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			int index = this.GetIndex(0, this._size, match);
			return (index == -1) ? default(T) : this._items[index];
		}

		private static void CheckMatch(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
		}

		public List<T> FindAll(Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			if (this._size <= 65536)
			{
				return this.FindAllStackBits(match);
			}
			return this.FindAllList(match);
		}

		private unsafe List<T> FindAllStackBits(Predicate<T> match)
		{
			uint* ptr;
			uint* ptr2;
			int num;
			uint num2;
			checked
			{
				ptr = stackalloc uint[unchecked(this._size / 32 + 1) * 4];
				ptr2 = ptr;
				num = 0;
				num2 = 2147483648U;
			}
			for (int i = 0; i < this._size; i++)
			{
				if (match(this._items[i]))
				{
					*ptr2 |= num2;
					num++;
				}
				num2 >>= 1;
				if (num2 == 0U)
				{
					ptr2++;
					num2 = 2147483648U;
				}
			}
			T[] array = new T[num];
			num2 = 2147483648U;
			ptr2 = ptr;
			int num3 = 0;
			int num4 = 0;
			while (num4 < this._size && num3 < num)
			{
				if ((*ptr2 & num2) == num2)
				{
					array[num3++] = this._items[num4];
				}
				num2 >>= 1;
				if (num2 == 0U)
				{
					ptr2++;
					num2 = 2147483648U;
				}
				num4++;
			}
			return new List<T>(array, num);
		}

		private List<T> FindAllList(Predicate<T> match)
		{
			List<T> list = new List<T>();
			for (int i = 0; i < this._size; i++)
			{
				if (match(this._items[i]))
				{
					list.Add(this._items[i]);
				}
			}
			return list;
		}

		public int FindIndex(Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			return this.GetIndex(0, this._size, match);
		}

		public int FindIndex(int startIndex, Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			this.CheckIndex(startIndex);
			return this.GetIndex(startIndex, this._size - startIndex, match);
		}

		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			this.CheckRange(startIndex, count);
			return this.GetIndex(startIndex, count, match);
		}

		private int GetIndex(int startIndex, int count, Predicate<T> match)
		{
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (match(this._items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public T FindLast(Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			int lastIndex = this.GetLastIndex(0, this._size, match);
			return (lastIndex != -1) ? this[lastIndex] : default(T);
		}

		public int FindLastIndex(Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			return this.GetLastIndex(0, this._size, match);
		}

		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			this.CheckIndex(startIndex);
			return this.GetLastIndex(0, startIndex + 1, match);
		}

		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			int num = startIndex - count + 1;
			this.CheckRange(num, count);
			return this.GetLastIndex(num, count, match);
		}

		private int GetLastIndex(int startIndex, int count, Predicate<T> match)
		{
			int num = startIndex + count;
			while (num != startIndex)
			{
				if (match(this._items[--num]))
				{
					return num;
				}
			}
			return -1;
		}

		public void ForEach(Action<T> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			for (int i = 0; i < this._size; i++)
			{
				action(this._items[i]);
			}
		}

		public List<T>.Enumerator GetEnumerator()
		{
			return new List<T>.Enumerator(this);
		}

		public List<T> GetRange(int index, int count)
		{
			this.CheckRange(index, count);
			T[] array = new T[count];
			Array.Copy(this._items, index, array, 0, count);
			return new List<T>(array, count);
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf<T>(this._items, item, 0, this._size);
		}

		public int IndexOf(T item, int index)
		{
			this.CheckIndex(index);
			return Array.IndexOf<T>(this._items, item, index, this._size - index);
		}

		public int IndexOf(T item, int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index + count > this._size)
			{
				throw new ArgumentOutOfRangeException("index and count exceed length of list");
			}
			return Array.IndexOf<T>(this._items, item, index, count);
		}

		private void Shift(int start, int delta)
		{
			if (delta < 0)
			{
				start -= delta;
			}
			if (start < this._size)
			{
				Array.Copy(this._items, start, this._items, start + delta, this._size - start);
			}
			this._size += delta;
			if (delta < 0)
			{
				Array.Clear(this._items, this._size, -delta);
			}
		}

		private void CheckIndex(int index)
		{
			if (index < 0 || index > this._size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public void Insert(int index, T item)
		{
			this.CheckIndex(index);
			if (this._size == this._items.Length)
			{
				this.GrowIfNeeded(1);
			}
			this.Shift(index, 1);
			this._items[index] = item;
			this._version++;
		}

		private void CheckCollection(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			this.CheckCollection(collection);
			this.CheckIndex(index);
			if (collection == this)
			{
				T[] array = new T[this._size];
				this.CopyTo(array, 0);
				this.GrowIfNeeded(this._size);
				this.Shift(index, array.Length);
				Array.Copy(array, 0, this._items, index, array.Length);
			}
			else
			{
				ICollection<T> collection2 = collection as ICollection<T>;
				if (collection2 != null)
				{
					this.InsertCollection(index, collection2);
				}
				else
				{
					this.InsertEnumeration(index, collection);
				}
			}
			this._version++;
		}

		private void InsertCollection(int index, ICollection<T> collection)
		{
			int count = collection.Count;
			this.GrowIfNeeded(count);
			this.Shift(index, count);
			collection.CopyTo(this._items, index);
		}

		private void InsertEnumeration(int index, IEnumerable<T> enumerable)
		{
			foreach (T t in enumerable)
			{
				this.Insert(index++, t);
			}
		}

		public int LastIndexOf(T item)
		{
			return Array.LastIndexOf<T>(this._items, item, this._size - 1, this._size);
		}

		public int LastIndexOf(T item, int index)
		{
			this.CheckIndex(index);
			return Array.LastIndexOf<T>(this._items, item, index, index + 1);
		}

		public int LastIndexOf(T item, int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "index is negative");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "count is negative");
			}
			if (index - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("cound", count, "count is too large");
			}
			return Array.LastIndexOf<T>(this._items, item, index, count);
		}

		public bool Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num != -1)
			{
				this.RemoveAt(num);
			}
			return num != -1;
		}

		public int RemoveAll(Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			int i;
			for (i = 0; i < this._size; i++)
			{
				if (match(this._items[i]))
				{
					break;
				}
			}
			if (i == this._size)
			{
				return 0;
			}
			this._version++;
			int j;
			for (j = i + 1; j < this._size; j++)
			{
				if (!match(this._items[j]))
				{
					this._items[i++] = this._items[j];
				}
			}
			if (j - i > 0)
			{
				Array.Clear(this._items, i, j - i);
			}
			this._size = i;
			return j - i;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this._size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.Shift(index, -1);
			Array.Clear(this._items, this._size, 1);
			this._version++;
		}

		public void RemoveRange(int index, int count)
		{
			this.CheckRange(index, count);
			if (count > 0)
			{
				this.Shift(index, -count);
				Array.Clear(this._items, this._size, count);
				this._version++;
			}
		}

		public void Reverse()
		{
			Array.Reverse(this._items, 0, this._size);
			this._version++;
		}

		public void Reverse(int index, int count)
		{
			this.CheckRange(index, count);
			Array.Reverse(this._items, index, count);
			this._version++;
		}

		public void Sort()
		{
			Array.Sort<T>(this._items, 0, this._size, Comparer<T>.Default);
			this._version++;
		}

		public void Sort(IComparer<T> comparer)
		{
			Array.Sort<T>(this._items, 0, this._size, comparer);
			this._version++;
		}

		public void Sort(Comparison<T> comparison)
		{
			Array.Sort<T>(this._items, this._size, comparison);
			this._version++;
		}

		public void Sort(int index, int count, IComparer<T> comparer)
		{
			this.CheckRange(index, count);
			Array.Sort<T>(this._items, index, count, comparer);
			this._version++;
		}

		public T[] ToArray()
		{
			T[] array = new T[this._size];
			Array.Copy(this._items, array, this._size);
			return array;
		}

		public void TrimExcess()
		{
			this.Capacity = this._size;
		}

		public bool TrueForAll(Predicate<T> match)
		{
			List<T>.CheckMatch(match);
			for (int i = 0; i < this._size; i++)
			{
				if (!match(this._items[i]))
				{
					return false;
				}
			}
			return true;
		}

		public int Capacity
		{
			get
			{
				return this._items.Length;
			}
			set
			{
				if (value < this._size)
				{
					throw new ArgumentOutOfRangeException();
				}
				Array.Resize<T>(ref this._items, value);
			}
		}

		public int Count
		{
			get
			{
				return this._size;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= this._size)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._items[index];
			}
			set
			{
				this.CheckIndex(index);
				if (index == this._size)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this._items[index] = value;
			}
		}

		private const int DefaultCapacity = 4;

		private T[] _items;

		private int _size;

		private int _version;

		private static readonly T[] EmptyArray = new T[0];

		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
		{
			internal Enumerator(List<T> l)
			{
				this.l = l;
				this.ver = l._version;
			}

			void IEnumerator.Reset()
			{
				this.VerifyState();
				this.next = 0;
			}

			object IEnumerator.Current
			{
				get
				{
					this.VerifyState();
					if (this.next <= 0)
					{
						throw new InvalidOperationException();
					}
					return this.current;
				}
			}

			public void Dispose()
			{
				this.l = null;
			}

			private void VerifyState()
			{
				if (this.l == null)
				{
					throw new ObjectDisposedException(base.GetType().FullName);
				}
				if (this.ver != this.l._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
			}

			public bool MoveNext()
			{
				this.VerifyState();
				if (this.next < 0)
				{
					return false;
				}
				if (this.next < this.l._size)
				{
					this.current = this.l._items[this.next++];
					return true;
				}
				this.next = -1;
				return false;
			}

			public T Current
			{
				get
				{
					return this.current;
				}
			}

			private List<T> l;

			private int next;

			private int ver;

			private T current;
		}
	}
}
