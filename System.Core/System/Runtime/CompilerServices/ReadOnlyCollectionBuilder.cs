using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	public sealed class ReadOnlyCollectionBuilder<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
	{
		public ReadOnlyCollectionBuilder()
		{
			this._items = Array.Empty<T>();
		}

		public ReadOnlyCollectionBuilder(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this._items = new T[capacity];
		}

		public ReadOnlyCollectionBuilder(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				int count = collection2.Count;
				this._items = new T[count];
				collection2.CopyTo(this._items, 0);
				this._size = count;
				return;
			}
			this._size = 0;
			this._items = new T[4];
			foreach (T t in collection)
			{
				this.Add(t);
			}
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
					throw new ArgumentOutOfRangeException("value");
				}
				if (value != this._items.Length)
				{
					if (value > 0)
					{
						T[] array = new T[value];
						if (this._size > 0)
						{
							Array.Copy(this._items, 0, array, 0, this._size);
						}
						this._items = array;
						return;
					}
					this._items = Array.Empty<T>();
				}
			}
		}

		public int Count
		{
			get
			{
				return this._size;
			}
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf<T>(this._items, item, 0, this._size);
		}

		public void Insert(int index, T item)
		{
			if (index > this._size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (this._size == this._items.Length)
			{
				this.EnsureCapacity(this._size + 1);
			}
			if (index < this._size)
			{
				Array.Copy(this._items, index, this._items, index + 1, this._size - index);
			}
			this._items[index] = item;
			this._size++;
			this._version++;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this._size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._size--;
			if (index < this._size)
			{
				Array.Copy(this._items, index + 1, this._items, index, this._size - index);
			}
			this._items[this._size] = default(T);
			this._version++;
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
				if (index >= this._size)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this._items[index] = value;
				this._version++;
			}
		}

		public void Add(T item)
		{
			if (this._size == this._items.Length)
			{
				this.EnsureCapacity(this._size + 1);
			}
			T[] items = this._items;
			int size = this._size;
			this._size = size + 1;
			items[size] = item;
			this._version++;
		}

		public void Clear()
		{
			if (this._size > 0)
			{
				Array.Clear(this._items, 0, this._size);
				this._size = 0;
			}
			this._version++;
		}

		public bool Contains(T item)
		{
			if (item == null)
			{
				for (int i = 0; i < this._size; i++)
				{
					if (this._items[i] == null)
					{
						return true;
					}
				}
				return false;
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int j = 0; j < this._size; j++)
			{
				if (@default.Equals(this._items[j], item))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(this._items, 0, array, arrayIndex, this._size);
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num >= 0)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ReadOnlyCollectionBuilder<T>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		int IList.Add(object value)
		{
			ReadOnlyCollectionBuilder<T>.ValidateNullValue(value, "value");
			try
			{
				this.Add((T)((object)value));
			}
			catch (InvalidCastException)
			{
				throw Error.InvalidTypeException(value, typeof(T), "value");
			}
			return this.Count - 1;
		}

		bool IList.Contains(object value)
		{
			return ReadOnlyCollectionBuilder<T>.IsCompatibleObject(value) && this.Contains((T)((object)value));
		}

		int IList.IndexOf(object value)
		{
			if (ReadOnlyCollectionBuilder<T>.IsCompatibleObject(value))
			{
				return this.IndexOf((T)((object)value));
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			ReadOnlyCollectionBuilder<T>.ValidateNullValue(value, "value");
			try
			{
				this.Insert(index, (T)((object)value));
			}
			catch (InvalidCastException)
			{
				throw Error.InvalidTypeException(value, typeof(T), "value");
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		void IList.Remove(object value)
		{
			if (ReadOnlyCollectionBuilder<T>.IsCompatibleObject(value))
			{
				this.Remove((T)((object)value));
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
				ReadOnlyCollectionBuilder<T>.ValidateNullValue(value, "value");
				try
				{
					this[index] = (T)((object)value);
				}
				catch (InvalidCastException)
				{
					throw Error.InvalidTypeException(value, typeof(T), "value");
				}
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("array");
			}
			Array.Copy(this._items, 0, array, index, this._size);
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

		public void Reverse()
		{
			this.Reverse(0, this.Count);
		}

		public void Reverse(int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			Array.Reverse<T>(this._items, index, count);
			this._version++;
		}

		public T[] ToArray()
		{
			T[] array = new T[this._size];
			Array.Copy(this._items, 0, array, 0, this._size);
			return array;
		}

		public ReadOnlyCollection<T> ToReadOnlyCollection()
		{
			T[] array;
			if (this._size == this._items.Length)
			{
				array = this._items;
			}
			else
			{
				array = this.ToArray();
			}
			this._items = Array.Empty<T>();
			this._size = 0;
			this._version++;
			return new TrueReadOnlyCollection<T>(array);
		}

		private void EnsureCapacity(int min)
		{
			if (this._items.Length < min)
			{
				int num = 4;
				if (this._items.Length != 0)
				{
					num = this._items.Length * 2;
				}
				if (num < min)
				{
					num = min;
				}
				this.Capacity = num;
			}
		}

		private static bool IsCompatibleObject(object value)
		{
			return value is T || (value == null && default(T) == null);
		}

		private static void ValidateNullValue(object value, string argument)
		{
			if (value == null && default(T) != null)
			{
				throw Error.InvalidNullValue(typeof(T), argument);
			}
		}

		private const int DefaultCapacity = 4;

		private T[] _items;

		private int _size;

		private int _version;

		[Serializable]
		private class Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(ReadOnlyCollectionBuilder<T> builder)
			{
				this._builder = builder;
				this._version = builder._version;
				this._index = 0;
				this._current = default(T);
			}

			public T Current
			{
				get
				{
					return this._current;
				}
			}

			public void Dispose()
			{
			}

			object IEnumerator.Current
			{
				get
				{
					if (this._index == 0 || this._index > this._builder._size)
					{
						throw Error.EnumerationIsDone();
					}
					return this._current;
				}
			}

			public bool MoveNext()
			{
				if (this._version != this._builder._version)
				{
					throw Error.CollectionModifiedWhileEnumerating();
				}
				if (this._index < this._builder._size)
				{
					T[] items = this._builder._items;
					int index = this._index;
					this._index = index + 1;
					this._current = items[index];
					return true;
				}
				this._index = this._builder._size + 1;
				this._current = default(T);
				return false;
			}

			void IEnumerator.Reset()
			{
				if (this._version != this._builder._version)
				{
					throw Error.CollectionModifiedWhileEnumerating();
				}
				this._index = 0;
				this._current = default(T);
			}

			private readonly ReadOnlyCollectionBuilder<T> _builder;

			private readonly int _version;

			private int _index;

			private T _current;
		}
	}
}
