using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[DebuggerDisplay("Count={Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
	[Serializable]
	public class ArrayList : IEnumerable, ICloneable, ICollection, IList
	{
		public ArrayList()
		{
			this._items = ArrayList.EmptyArray;
		}

		public ArrayList(ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			Array array = c as Array;
			if (array != null && array.Rank != 1)
			{
				throw new RankException();
			}
			this._items = new object[c.Count];
			this.AddRange(c);
		}

		public ArrayList(int capacity)
		{
			if (capacity < 0)
			{
				ArrayList.ThrowNewArgumentOutOfRangeException("capacity", capacity, "The initial capacity can't be smaller than zero.");
			}
			if (capacity == 0)
			{
				capacity = 4;
			}
			this._items = new object[capacity];
		}

		private ArrayList(int initialCapacity, bool forceZeroSize)
		{
			if (forceZeroSize)
			{
				this._items = null;
				return;
			}
			throw new InvalidOperationException("Use ArrayList(int)");
		}

		private ArrayList(object[] array, int index, int count)
		{
			if (count == 0)
			{
				this._items = new object[4];
			}
			else
			{
				this._items = new object[count];
			}
			Array.Copy(array, index, this._items, 0, count);
			this._size = count;
		}

		public virtual object this[int index]
		{
			get
			{
				if (index < 0 || index >= this._size)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index is less than 0 or more than or equal to the list count.");
				}
				return this._items[index];
			}
			set
			{
				if (index < 0 || index >= this._size)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index is less than 0 or more than or equal to the list count.");
				}
				this._items[index] = value;
				this._version++;
			}
		}

		public virtual int Count
		{
			get
			{
				return this._size;
			}
		}

		public virtual int Capacity
		{
			get
			{
				return this._items.Length;
			}
			set
			{
				if (value < this._size)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("Capacity", value, "Must be more than count.");
				}
				object[] array = new object[value];
				Array.Copy(this._items, 0, array, 0, this._size);
				this._items = array;
			}
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		private void EnsureCapacity(int count)
		{
			if (count <= this._items.Length)
			{
				return;
			}
			int i = this._items.Length << 1;
			if (i == 0)
			{
				i = 4;
			}
			while (i < count)
			{
				i <<= 1;
			}
			object[] array = new object[i];
			Array.Copy(this._items, 0, array, 0, this._items.Length);
			this._items = array;
		}

		private void Shift(int index, int count)
		{
			if (count > 0)
			{
				if (this._size + count > this._items.Length)
				{
					int i;
					for (i = ((this._items.Length <= 0) ? 1 : (this._items.Length << 1)); i < this._size + count; i <<= 1)
					{
					}
					object[] array = new object[i];
					Array.Copy(this._items, 0, array, 0, index);
					Array.Copy(this._items, index, array, index + count, this._size - index);
					this._items = array;
				}
				else
				{
					Array.Copy(this._items, index, this._items, index + count, this._size - index);
				}
			}
			else if (count < 0)
			{
				int num = index - count;
				Array.Copy(this._items, num, this._items, index, this._size - num);
				Array.Clear(this._items, this._size + count, -count);
			}
		}

		public virtual int Add(object value)
		{
			if (this._items.Length <= this._size)
			{
				this.EnsureCapacity(this._size + 1);
			}
			this._items[this._size] = value;
			this._version++;
			return this._size++;
		}

		public virtual void Clear()
		{
			Array.Clear(this._items, 0, this._size);
			this._size = 0;
			this._version++;
		}

		public virtual bool Contains(object item)
		{
			return this.IndexOf(item, 0, this._size) > -1;
		}

		internal virtual bool Contains(object value, int startIndex, int count)
		{
			return this.IndexOf(value, startIndex, count) > -1;
		}

		public virtual int IndexOf(object value)
		{
			return this.IndexOf(value, 0);
		}

		public virtual int IndexOf(object value, int startIndex)
		{
			return this.IndexOf(value, startIndex, this._size - startIndex);
		}

		public virtual int IndexOf(object value, int startIndex, int count)
		{
			if (startIndex < 0 || startIndex > this._size)
			{
				ArrayList.ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "Does not specify valid index.");
			}
			if (count < 0)
			{
				ArrayList.ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
			}
			if (startIndex > this._size - count)
			{
				throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
			}
			return Array.IndexOf<object>(this._items, value, startIndex, count);
		}

		public virtual int LastIndexOf(object value)
		{
			return this.LastIndexOf(value, this._size - 1);
		}

		public virtual int LastIndexOf(object value, int startIndex)
		{
			return this.LastIndexOf(value, startIndex, startIndex + 1);
		}

		public virtual int LastIndexOf(object value, int startIndex, int count)
		{
			return Array.LastIndexOf<object>(this._items, value, startIndex, count);
		}

		public virtual void Insert(int index, object value)
		{
			if (index < 0 || index > this._size)
			{
				ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
			}
			this.Shift(index, 1);
			this._items[index] = value;
			this._size++;
			this._version++;
		}

		public virtual void InsertRange(int index, ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (index < 0 || index > this._size)
			{
				ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
			}
			int count = c.Count;
			if (this._items.Length < this._size + count)
			{
				this.EnsureCapacity(this._size + count);
			}
			if (index < this._size)
			{
				Array.Copy(this._items, index, this._items, index + count, this._size - index);
			}
			if (this == c.SyncRoot)
			{
				Array.Copy(this._items, 0, this._items, index, index);
				Array.Copy(this._items, index + count, this._items, index << 1, this._size - index);
			}
			else
			{
				c.CopyTo(this._items, index);
			}
			this._size += c.Count;
			this._version++;
		}

		public virtual void Remove(object obj)
		{
			int num = this.IndexOf(obj);
			if (num > -1)
			{
				this.RemoveAt(num);
			}
			this._version++;
		}

		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= this._size)
			{
				ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Less than 0 or more than list count.");
			}
			this.Shift(index, -1);
			this._size--;
			this._version++;
		}

		public virtual void RemoveRange(int index, int count)
		{
			ArrayList.CheckRange(index, count, this._size);
			this.Shift(index, -count);
			this._size -= count;
			this._version++;
		}

		public virtual void Reverse()
		{
			Array.Reverse(this._items, 0, this._size);
			this._version++;
		}

		public virtual void Reverse(int index, int count)
		{
			ArrayList.CheckRange(index, count, this._size);
			Array.Reverse(this._items, index, count);
			this._version++;
		}

		public virtual void CopyTo(Array array)
		{
			Array.Copy(this._items, array, this._size);
		}

		public virtual void CopyTo(Array array, int arrayIndex)
		{
			this.CopyTo(0, array, arrayIndex, this._size);
		}

		public virtual void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Must have only 1 dimensions.", "array");
			}
			Array.Copy(this._items, index, array, arrayIndex, count);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new ArrayList.SimpleEnumerator(this);
		}

		public virtual IEnumerator GetEnumerator(int index, int count)
		{
			ArrayList.CheckRange(index, count, this._size);
			return new ArrayList.ArrayListEnumerator(this, index, count);
		}

		public virtual void AddRange(ICollection c)
		{
			this.InsertRange(this._size, c);
		}

		public virtual int BinarySearch(object value)
		{
			int num;
			try
			{
				num = Array.BinarySearch<object>(this._items, 0, this._size, value);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
			return num;
		}

		public virtual int BinarySearch(object value, IComparer comparer)
		{
			int num;
			try
			{
				num = Array.BinarySearch(this._items, 0, this._size, value, comparer);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
			return num;
		}

		public virtual int BinarySearch(int index, int count, object value, IComparer comparer)
		{
			int num;
			try
			{
				num = Array.BinarySearch(this._items, index, count, value, comparer);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
			return num;
		}

		public virtual ArrayList GetRange(int index, int count)
		{
			ArrayList.CheckRange(index, count, this._size);
			if (this.IsSynchronized)
			{
				return ArrayList.Synchronized(new ArrayList.RangedArrayList(this, index, count));
			}
			return new ArrayList.RangedArrayList(this, index, count);
		}

		public virtual void SetRange(int index, ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (index < 0 || index + c.Count > this._size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			c.CopyTo(this._items, index);
			this._version++;
		}

		public virtual void TrimToSize()
		{
			if (this._items.Length > this._size)
			{
				object[] array;
				if (this._size == 0)
				{
					array = new object[4];
				}
				else
				{
					array = new object[this._size];
				}
				Array.Copy(this._items, 0, array, 0, this._size);
				this._items = array;
			}
		}

		public virtual void Sort()
		{
			Array.Sort<object>(this._items, 0, this._size);
			this._version++;
		}

		public virtual void Sort(IComparer comparer)
		{
			Array.Sort(this._items, 0, this._size, comparer);
		}

		public virtual void Sort(int index, int count, IComparer comparer)
		{
			ArrayList.CheckRange(index, count, this._size);
			Array.Sort(this._items, index, count, comparer);
		}

		public virtual object[] ToArray()
		{
			object[] array = new object[this._size];
			this.CopyTo(array);
			return array;
		}

		public virtual Array ToArray(Type type)
		{
			Array array = Array.CreateInstance(type, this._size);
			this.CopyTo(array);
			return array;
		}

		public virtual object Clone()
		{
			return new ArrayList(this._items, 0, this._size);
		}

		internal static void CheckRange(int index, int count, int listCount)
		{
			if (index < 0)
			{
				ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Can't be less than 0.");
			}
			if (count < 0)
			{
				ArrayList.ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
			}
			if (index > listCount - count)
			{
				throw new ArgumentException("Index and count do not denote a valid range of elements.", "index");
			}
		}

		internal static void ThrowNewArgumentOutOfRangeException(string name, object actual, string message)
		{
			throw new ArgumentOutOfRangeException(name, actual, message);
		}

		public static ArrayList Adapter(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			ArrayList arrayList = list as ArrayList;
			if (arrayList != null)
			{
				return arrayList;
			}
			arrayList = new ArrayList.ArrayListAdapter(list);
			if (list.IsSynchronized)
			{
				return ArrayList.Synchronized(arrayList);
			}
			return arrayList;
		}

		public static ArrayList Synchronized(ArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsSynchronized)
			{
				return list;
			}
			return new ArrayList.SynchronizedArrayListWrapper(list);
		}

		public static IList Synchronized(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsSynchronized)
			{
				return list;
			}
			return new ArrayList.SynchronizedListWrapper(list);
		}

		public static ArrayList ReadOnly(ArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsReadOnly)
			{
				return list;
			}
			return new ArrayList.ReadOnlyArrayListWrapper(list);
		}

		public static IList ReadOnly(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsReadOnly)
			{
				return list;
			}
			return new ArrayList.ReadOnlyListWrapper(list);
		}

		public static ArrayList FixedSize(ArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsFixedSize)
			{
				return list;
			}
			return new ArrayList.FixedSizeArrayListWrapper(list);
		}

		public static IList FixedSize(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsFixedSize)
			{
				return list;
			}
			return new ArrayList.FixedSizeListWrapper(list);
		}

		public static ArrayList Repeat(object value, int count)
		{
			ArrayList arrayList = new ArrayList(count);
			for (int i = 0; i < count; i++)
			{
				arrayList.Add(value);
			}
			return arrayList;
		}

		private const int DefaultInitialCapacity = 4;

		private int _size;

		private object[] _items;

		private int _version;

		private static readonly object[] EmptyArray = new object[0];

		private sealed class ArrayListEnumerator : IEnumerator, ICloneable
		{
			public ArrayListEnumerator(ArrayList list)
				: this(list, 0, list.Count)
			{
			}

			public ArrayListEnumerator(ArrayList list, int index, int count)
			{
				this.m_List = list;
				this.m_Index = index;
				this.m_Count = count;
				this.m_Pos = this.m_Index - 1;
				this.m_Current = null;
				this.m_ExpectedStateChanges = list._version;
			}

			public object Clone()
			{
				return base.MemberwiseClone();
			}

			public object Current
			{
				get
				{
					if (this.m_Pos == this.m_Index - 1)
					{
						throw new InvalidOperationException("Enumerator unusable (Reset pending, or past end of array.");
					}
					return this.m_Current;
				}
			}

			public bool MoveNext()
			{
				if (this.m_List._version != this.m_ExpectedStateChanges)
				{
					throw new InvalidOperationException("List has changed.");
				}
				this.m_Pos++;
				if (this.m_Pos - this.m_Index < this.m_Count)
				{
					this.m_Current = this.m_List[this.m_Pos];
					return true;
				}
				return false;
			}

			public void Reset()
			{
				this.m_Current = null;
				this.m_Pos = this.m_Index - 1;
			}

			private int m_Pos;

			private int m_Index;

			private int m_Count;

			private object m_Current;

			private ArrayList m_List;

			private int m_ExpectedStateChanges;
		}

		private sealed class SimpleEnumerator : IEnumerator, ICloneable
		{
			public SimpleEnumerator(ArrayList list)
			{
				this.list = list;
				this.index = -1;
				this.version = list._version;
				this.currentElement = ArrayList.SimpleEnumerator.endFlag;
			}

			public object Clone()
			{
				return base.MemberwiseClone();
			}

			public bool MoveNext()
			{
				if (this.version != this.list._version)
				{
					throw new InvalidOperationException("List has changed.");
				}
				if (++this.index < this.list.Count)
				{
					this.currentElement = this.list[this.index];
					return true;
				}
				this.currentElement = ArrayList.SimpleEnumerator.endFlag;
				return false;
			}

			public object Current
			{
				get
				{
					if (this.currentElement != ArrayList.SimpleEnumerator.endFlag)
					{
						return this.currentElement;
					}
					if (this.index == -1)
					{
						throw new InvalidOperationException("Enumerator not started");
					}
					throw new InvalidOperationException("Enumerator ended");
				}
			}

			public void Reset()
			{
				if (this.version != this.list._version)
				{
					throw new InvalidOperationException("List has changed.");
				}
				this.currentElement = ArrayList.SimpleEnumerator.endFlag;
				this.index = -1;
			}

			private ArrayList list;

			private int index;

			private int version;

			private object currentElement;

			private static object endFlag = new object();
		}

		[Serializable]
		private sealed class ArrayListAdapter : ArrayList
		{
			public ArrayListAdapter(IList adaptee)
				: base(0, true)
			{
				this.m_Adaptee = adaptee;
			}

			public override object this[int index]
			{
				get
				{
					return this.m_Adaptee[index];
				}
				set
				{
					this.m_Adaptee[index] = value;
				}
			}

			public override int Count
			{
				get
				{
					return this.m_Adaptee.Count;
				}
			}

			public override int Capacity
			{
				get
				{
					return this.m_Adaptee.Count;
				}
				set
				{
					if (value < this.m_Adaptee.Count)
					{
						throw new ArgumentException("capacity");
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return this.m_Adaptee.IsFixedSize;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this.m_Adaptee.IsReadOnly;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this.m_Adaptee.SyncRoot;
				}
			}

			public override int Add(object value)
			{
				return this.m_Adaptee.Add(value);
			}

			public override void Clear()
			{
				this.m_Adaptee.Clear();
			}

			public override bool Contains(object value)
			{
				return this.m_Adaptee.Contains(value);
			}

			public override int IndexOf(object value)
			{
				return this.m_Adaptee.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return this.IndexOf(value, startIndex, this.m_Adaptee.Count - startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > this.m_Adaptee.Count)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "Does not specify valid index.");
				}
				if (count < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
				}
				if (startIndex > this.m_Adaptee.Count - count)
				{
					throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
				}
				if (value == null)
				{
					for (int i = startIndex; i < startIndex + count; i++)
					{
						if (this.m_Adaptee[i] == null)
						{
							return i;
						}
					}
				}
				else
				{
					for (int j = startIndex; j < startIndex + count; j++)
					{
						if (value.Equals(this.m_Adaptee[j]))
						{
							return j;
						}
					}
				}
				return -1;
			}

			public override int LastIndexOf(object value)
			{
				return this.LastIndexOf(value, this.m_Adaptee.Count - 1);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return this.LastIndexOf(value, startIndex, startIndex + 1);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "< 0");
				}
				if (count < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("count", count, "count is negative.");
				}
				if (startIndex - count + 1 < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("count", count, "count is too large.");
				}
				if (value == null)
				{
					for (int i = startIndex; i > startIndex - count; i--)
					{
						if (this.m_Adaptee[i] == null)
						{
							return i;
						}
					}
				}
				else
				{
					for (int j = startIndex; j > startIndex - count; j--)
					{
						if (value.Equals(this.m_Adaptee[j]))
						{
							return j;
						}
					}
				}
				return -1;
			}

			public override void Insert(int index, object value)
			{
				this.m_Adaptee.Insert(index, value);
			}

			public override void InsertRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException("c");
				}
				if (index > this.m_Adaptee.Count)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				foreach (object obj in c)
				{
					this.m_Adaptee.Insert(index++, obj);
				}
			}

			public override void Remove(object value)
			{
				this.m_Adaptee.Remove(value);
			}

			public override void RemoveAt(int index)
			{
				this.m_Adaptee.RemoveAt(index);
			}

			public override void RemoveRange(int index, int count)
			{
				ArrayList.CheckRange(index, count, this.m_Adaptee.Count);
				for (int i = 0; i < count; i++)
				{
					this.m_Adaptee.RemoveAt(index);
				}
			}

			public override void Reverse()
			{
				this.Reverse(0, this.m_Adaptee.Count);
			}

			public override void Reverse(int index, int count)
			{
				ArrayList.CheckRange(index, count, this.m_Adaptee.Count);
				for (int i = 0; i < count / 2; i++)
				{
					object obj = this.m_Adaptee[i + index];
					this.m_Adaptee[i + index] = this.m_Adaptee[index + count - i + index - 1];
					this.m_Adaptee[index + count - i + index - 1] = obj;
				}
			}

			public override void SetRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException("c");
				}
				if (index < 0 || index + c.Count > this.m_Adaptee.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				int num = index;
				foreach (object obj in c)
				{
					this.m_Adaptee[num++] = obj;
				}
			}

			public override void CopyTo(Array array)
			{
				this.m_Adaptee.CopyTo(array, 0);
			}

			public override void CopyTo(Array array, int index)
			{
				this.m_Adaptee.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				if (index < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Can't be less than zero.");
				}
				if (arrayIndex < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("arrayIndex", arrayIndex, "Can't be less than zero.");
				}
				if (count < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Can't be less than zero.");
				}
				if (index >= this.m_Adaptee.Count)
				{
					throw new ArgumentException("Can't be more or equal to list count.", "index");
				}
				if (array.Rank > 1)
				{
					throw new ArgumentException("Can't copy into multi-dimensional array.");
				}
				if (arrayIndex >= array.Length)
				{
					throw new ArgumentException("arrayIndex can't be greater than array.Length - 1.");
				}
				if (array.Length - arrayIndex + 1 < count)
				{
					throw new ArgumentException("Destination array is too small.");
				}
				if (index > this.m_Adaptee.Count - count)
				{
					throw new ArgumentException("Index and count do not denote a valid range of elements.", "index");
				}
				for (int i = 0; i < count; i++)
				{
					array.SetValue(this.m_Adaptee[index + i], arrayIndex + i);
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return this.m_Adaptee.IsSynchronized;
				}
			}

			public override IEnumerator GetEnumerator()
			{
				return this.m_Adaptee.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				ArrayList.CheckRange(index, count, this.m_Adaptee.Count);
				return new ArrayList.ArrayListAdapter.EnumeratorWithRange(this.m_Adaptee.GetEnumerator(), index, count);
			}

			public override void AddRange(ICollection c)
			{
				foreach (object obj in c)
				{
					this.m_Adaptee.Add(obj);
				}
			}

			public override int BinarySearch(object value)
			{
				return this.BinarySearch(value, null);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return this.BinarySearch(0, this.m_Adaptee.Count, value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				ArrayList.CheckRange(index, count, this.m_Adaptee.Count);
				if (comparer == null)
				{
					comparer = Comparer.Default;
				}
				int i = index;
				int num = index + count - 1;
				while (i <= num)
				{
					int num2 = i + (num - i) / 2;
					int num3 = comparer.Compare(value, this.m_Adaptee[num2]);
					if (num3 < 0)
					{
						num = num2 - 1;
					}
					else
					{
						if (num3 <= 0)
						{
							return num2;
						}
						i = num2 + 1;
					}
				}
				return ~i;
			}

			public override object Clone()
			{
				return new ArrayList.ArrayListAdapter(this.m_Adaptee);
			}

			public override ArrayList GetRange(int index, int count)
			{
				ArrayList.CheckRange(index, count, this.m_Adaptee.Count);
				return new ArrayList.RangedArrayList(this, index, count);
			}

			public override void TrimToSize()
			{
			}

			public override void Sort()
			{
				this.Sort(Comparer.Default);
			}

			public override void Sort(IComparer comparer)
			{
				this.Sort(0, this.m_Adaptee.Count, comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				ArrayList.CheckRange(index, count, this.m_Adaptee.Count);
				if (comparer == null)
				{
					comparer = Comparer.Default;
				}
				ArrayList.ArrayListAdapter.QuickSort(this.m_Adaptee, index, index + count - 1, comparer);
			}

			private static void Swap(IList list, int x, int y)
			{
				object obj = list[x];
				list[x] = list[y];
				list[y] = obj;
			}

			internal static void QuickSort(IList list, int left, int right, IComparer comparer)
			{
				if (left >= right)
				{
					return;
				}
				int num = left + (right - left) / 2;
				if (comparer.Compare(list[num], list[left]) < 0)
				{
					ArrayList.ArrayListAdapter.Swap(list, num, left);
				}
				if (comparer.Compare(list[right], list[left]) < 0)
				{
					ArrayList.ArrayListAdapter.Swap(list, right, left);
				}
				if (comparer.Compare(list[right], list[num]) < 0)
				{
					ArrayList.ArrayListAdapter.Swap(list, right, num);
				}
				if (right - left + 1 <= 3)
				{
					return;
				}
				ArrayList.ArrayListAdapter.Swap(list, right - 1, num);
				object obj = list[right - 1];
				int num2 = left;
				int num3 = right - 1;
				for (;;)
				{
					while (comparer.Compare(list[++num2], obj) < 0)
					{
					}
					while (comparer.Compare(list[--num3], obj) > 0)
					{
					}
					if (num2 >= num3)
					{
						break;
					}
					ArrayList.ArrayListAdapter.Swap(list, num2, num3);
				}
				ArrayList.ArrayListAdapter.Swap(list, right - 1, num2);
				ArrayList.ArrayListAdapter.QuickSort(list, left, num2 - 1, comparer);
				ArrayList.ArrayListAdapter.QuickSort(list, num2 + 1, right, comparer);
			}

			public override object[] ToArray()
			{
				object[] array = new object[this.m_Adaptee.Count];
				this.m_Adaptee.CopyTo(array, 0);
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				Array array = Array.CreateInstance(elementType, this.m_Adaptee.Count);
				this.m_Adaptee.CopyTo(array, 0);
				return array;
			}

			private IList m_Adaptee;

			private sealed class EnumeratorWithRange : IEnumerator, ICloneable
			{
				public EnumeratorWithRange(IEnumerator enumerator, int index, int count)
				{
					this.m_Count = 0;
					this.m_StartIndex = index;
					this.m_MaxCount = count;
					this.m_Enumerator = enumerator;
					this.Reset();
				}

				public object Clone()
				{
					return base.MemberwiseClone();
				}

				public object Current
				{
					get
					{
						return this.m_Enumerator.Current;
					}
				}

				public bool MoveNext()
				{
					if (this.m_Count >= this.m_MaxCount)
					{
						return false;
					}
					this.m_Count++;
					return this.m_Enumerator.MoveNext();
				}

				public void Reset()
				{
					this.m_Count = 0;
					this.m_Enumerator.Reset();
					for (int i = 0; i < this.m_StartIndex; i++)
					{
						this.m_Enumerator.MoveNext();
					}
				}

				private int m_StartIndex;

				private int m_Count;

				private int m_MaxCount;

				private IEnumerator m_Enumerator;
			}
		}

		[Serializable]
		private class ArrayListWrapper : ArrayList
		{
			public ArrayListWrapper(ArrayList innerArrayList)
			{
				this.m_InnerArrayList = innerArrayList;
			}

			public override object this[int index]
			{
				get
				{
					return this.m_InnerArrayList[index];
				}
				set
				{
					this.m_InnerArrayList[index] = value;
				}
			}

			public override int Count
			{
				get
				{
					return this.m_InnerArrayList.Count;
				}
			}

			public override int Capacity
			{
				get
				{
					return this.m_InnerArrayList.Capacity;
				}
				set
				{
					this.m_InnerArrayList.Capacity = value;
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return this.m_InnerArrayList.IsFixedSize;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this.m_InnerArrayList.IsReadOnly;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return this.m_InnerArrayList.IsSynchronized;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this.m_InnerArrayList.SyncRoot;
				}
			}

			public override int Add(object value)
			{
				return this.m_InnerArrayList.Add(value);
			}

			public override void Clear()
			{
				this.m_InnerArrayList.Clear();
			}

			public override bool Contains(object value)
			{
				return this.m_InnerArrayList.Contains(value);
			}

			public override int IndexOf(object value)
			{
				return this.m_InnerArrayList.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return this.m_InnerArrayList.IndexOf(value, startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				return this.m_InnerArrayList.IndexOf(value, startIndex, count);
			}

			public override int LastIndexOf(object value)
			{
				return this.m_InnerArrayList.LastIndexOf(value);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return this.m_InnerArrayList.LastIndexOf(value, startIndex);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				return this.m_InnerArrayList.LastIndexOf(value, startIndex, count);
			}

			public override void Insert(int index, object value)
			{
				this.m_InnerArrayList.Insert(index, value);
			}

			public override void InsertRange(int index, ICollection c)
			{
				this.m_InnerArrayList.InsertRange(index, c);
			}

			public override void Remove(object value)
			{
				this.m_InnerArrayList.Remove(value);
			}

			public override void RemoveAt(int index)
			{
				this.m_InnerArrayList.RemoveAt(index);
			}

			public override void RemoveRange(int index, int count)
			{
				this.m_InnerArrayList.RemoveRange(index, count);
			}

			public override void Reverse()
			{
				this.m_InnerArrayList.Reverse();
			}

			public override void Reverse(int index, int count)
			{
				this.m_InnerArrayList.Reverse(index, count);
			}

			public override void SetRange(int index, ICollection c)
			{
				this.m_InnerArrayList.SetRange(index, c);
			}

			public override void CopyTo(Array array)
			{
				this.m_InnerArrayList.CopyTo(array);
			}

			public override void CopyTo(Array array, int index)
			{
				this.m_InnerArrayList.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				this.m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return this.m_InnerArrayList.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				return this.m_InnerArrayList.GetEnumerator(index, count);
			}

			public override void AddRange(ICollection c)
			{
				this.m_InnerArrayList.AddRange(c);
			}

			public override int BinarySearch(object value)
			{
				return this.m_InnerArrayList.BinarySearch(value);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return this.m_InnerArrayList.BinarySearch(value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				return this.m_InnerArrayList.BinarySearch(index, count, value, comparer);
			}

			public override object Clone()
			{
				return this.m_InnerArrayList.Clone();
			}

			public override ArrayList GetRange(int index, int count)
			{
				return this.m_InnerArrayList.GetRange(index, count);
			}

			public override void TrimToSize()
			{
				this.m_InnerArrayList.TrimToSize();
			}

			public override void Sort()
			{
				this.m_InnerArrayList.Sort();
			}

			public override void Sort(IComparer comparer)
			{
				this.m_InnerArrayList.Sort(comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				this.m_InnerArrayList.Sort(index, count, comparer);
			}

			public override object[] ToArray()
			{
				return this.m_InnerArrayList.ToArray();
			}

			public override Array ToArray(Type elementType)
			{
				return this.m_InnerArrayList.ToArray(elementType);
			}

			protected ArrayList m_InnerArrayList;
		}

		[Serializable]
		private sealed class SynchronizedArrayListWrapper : ArrayList.ArrayListWrapper
		{
			internal SynchronizedArrayListWrapper(ArrayList innerArrayList)
				: base(innerArrayList)
			{
				this.m_SyncRoot = innerArrayList.SyncRoot;
			}

			public override object this[int index]
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					object obj;
					lock (syncRoot)
					{
						obj = this.m_InnerArrayList[index];
					}
					return obj;
				}
				set
				{
					object syncRoot = this.m_SyncRoot;
					lock (syncRoot)
					{
						this.m_InnerArrayList[index] = value;
					}
				}
			}

			public override int Count
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					int count;
					lock (syncRoot)
					{
						count = this.m_InnerArrayList.Count;
					}
					return count;
				}
			}

			public override int Capacity
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					int capacity;
					lock (syncRoot)
					{
						capacity = this.m_InnerArrayList.Capacity;
					}
					return capacity;
				}
				set
				{
					object syncRoot = this.m_SyncRoot;
					lock (syncRoot)
					{
						this.m_InnerArrayList.Capacity = value;
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					bool isFixedSize;
					lock (syncRoot)
					{
						isFixedSize = this.m_InnerArrayList.IsFixedSize;
					}
					return isFixedSize;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					bool isReadOnly;
					lock (syncRoot)
					{
						isReadOnly = this.m_InnerArrayList.IsReadOnly;
					}
					return isReadOnly;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this.m_SyncRoot;
				}
			}

			public override int Add(object value)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.Add(value);
				}
				return num;
			}

			public override void Clear()
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.Clear();
				}
			}

			public override bool Contains(object value)
			{
				object syncRoot = this.m_SyncRoot;
				bool flag;
				lock (syncRoot)
				{
					flag = this.m_InnerArrayList.Contains(value);
				}
				return flag;
			}

			public override int IndexOf(object value)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.IndexOf(value);
				}
				return num;
			}

			public override int IndexOf(object value, int startIndex)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.IndexOf(value, startIndex);
				}
				return num;
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.IndexOf(value, startIndex, count);
				}
				return num;
			}

			public override int LastIndexOf(object value)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.LastIndexOf(value);
				}
				return num;
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.LastIndexOf(value, startIndex);
				}
				return num;
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.LastIndexOf(value, startIndex, count);
				}
				return num;
			}

			public override void Insert(int index, object value)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.Insert(index, value);
				}
			}

			public override void InsertRange(int index, ICollection c)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.InsertRange(index, c);
				}
			}

			public override void Remove(object value)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.RemoveAt(index);
				}
			}

			public override void RemoveRange(int index, int count)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.RemoveRange(index, count);
				}
			}

			public override void Reverse()
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.Reverse();
				}
			}

			public override void Reverse(int index, int count)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.Reverse(index, count);
				}
			}

			public override void CopyTo(Array array)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.CopyTo(array);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.CopyTo(array, index);
				}
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				object syncRoot = this.m_SyncRoot;
				IEnumerator enumerator;
				lock (syncRoot)
				{
					enumerator = this.m_InnerArrayList.GetEnumerator();
				}
				return enumerator;
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				object syncRoot = this.m_SyncRoot;
				IEnumerator enumerator;
				lock (syncRoot)
				{
					enumerator = this.m_InnerArrayList.GetEnumerator(index, count);
				}
				return enumerator;
			}

			public override void AddRange(ICollection c)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.AddRange(c);
				}
			}

			public override int BinarySearch(object value)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.BinarySearch(value);
				}
				return num;
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.BinarySearch(value, comparer);
				}
				return num;
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerArrayList.BinarySearch(index, count, value, comparer);
				}
				return num;
			}

			public override object Clone()
			{
				object syncRoot = this.m_SyncRoot;
				object obj;
				lock (syncRoot)
				{
					obj = this.m_InnerArrayList.Clone();
				}
				return obj;
			}

			public override ArrayList GetRange(int index, int count)
			{
				object syncRoot = this.m_SyncRoot;
				ArrayList range;
				lock (syncRoot)
				{
					range = this.m_InnerArrayList.GetRange(index, count);
				}
				return range;
			}

			public override void TrimToSize()
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.TrimToSize();
				}
			}

			public override void Sort()
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.Sort();
				}
			}

			public override void Sort(IComparer comparer)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.Sort(comparer);
				}
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerArrayList.Sort(index, count, comparer);
				}
			}

			public override object[] ToArray()
			{
				object syncRoot = this.m_SyncRoot;
				object[] array;
				lock (syncRoot)
				{
					array = this.m_InnerArrayList.ToArray();
				}
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				object syncRoot = this.m_SyncRoot;
				Array array;
				lock (syncRoot)
				{
					array = this.m_InnerArrayList.ToArray(elementType);
				}
				return array;
			}

			private object m_SyncRoot;
		}

		[Serializable]
		private class FixedSizeArrayListWrapper : ArrayList.ArrayListWrapper
		{
			public FixedSizeArrayListWrapper(ArrayList innerList)
				: base(innerList)
			{
			}

			protected virtual string ErrorMessage
			{
				get
				{
					return "Can't add or remove from a fixed-size list.";
				}
			}

			public override int Capacity
			{
				get
				{
					return base.Capacity;
				}
				set
				{
					throw new NotSupportedException(this.ErrorMessage);
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public override int Add(object value)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void AddRange(ICollection c)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Clear()
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Insert(int index, object value)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void InsertRange(int index, ICollection c)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException(this.ErrorMessage);
			}
		}

		[Serializable]
		private sealed class ReadOnlyArrayListWrapper : ArrayList.FixedSizeArrayListWrapper
		{
			public ReadOnlyArrayListWrapper(ArrayList innerArrayList)
				: base(innerArrayList)
			{
			}

			protected override string ErrorMessage
			{
				get
				{
					return "Can't modify a readonly list.";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override object this[int index]
			{
				get
				{
					return this.m_InnerArrayList[index];
				}
				set
				{
					throw new NotSupportedException(this.ErrorMessage);
				}
			}

			public override void Reverse()
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Reverse(int index, int count)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void SetRange(int index, ICollection c)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Sort()
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Sort(IComparer comparer)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}
		}

		[Serializable]
		private sealed class RangedArrayList : ArrayList.ArrayListWrapper
		{
			public RangedArrayList(ArrayList innerList, int index, int count)
				: base(innerList)
			{
				this.m_InnerIndex = index;
				this.m_InnerCount = count;
				this.m_InnerStateChanges = innerList._version;
			}

			public override bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

			public override object this[int index]
			{
				get
				{
					if (index < 0 || index > this.m_InnerCount)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return this.m_InnerArrayList[this.m_InnerIndex + index];
				}
				set
				{
					if (index < 0 || index > this.m_InnerCount)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					this.m_InnerArrayList[this.m_InnerIndex + index] = value;
				}
			}

			public override int Count
			{
				get
				{
					this.VerifyStateChanges();
					return this.m_InnerCount;
				}
			}

			public override int Capacity
			{
				get
				{
					return this.m_InnerArrayList.Capacity;
				}
				set
				{
					if (value < this.m_InnerCount)
					{
						throw new ArgumentOutOfRangeException();
					}
				}
			}

			private void VerifyStateChanges()
			{
				if (this.m_InnerStateChanges != this.m_InnerArrayList._version)
				{
					throw new InvalidOperationException("ArrayList view is invalid because the underlying ArrayList was modified.");
				}
			}

			public override int Add(object value)
			{
				this.VerifyStateChanges();
				this.m_InnerArrayList.Insert(this.m_InnerIndex + this.m_InnerCount, value);
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
				return ++this.m_InnerCount;
			}

			public override void Clear()
			{
				this.VerifyStateChanges();
				this.m_InnerArrayList.RemoveRange(this.m_InnerIndex, this.m_InnerCount);
				this.m_InnerCount = 0;
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override bool Contains(object value)
			{
				return this.m_InnerArrayList.Contains(value, this.m_InnerIndex, this.m_InnerCount);
			}

			public override int IndexOf(object value)
			{
				return this.IndexOf(value, 0);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return this.IndexOf(value, startIndex, this.m_InnerCount - startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > this.m_InnerCount)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "Does not specify valid index.");
				}
				if (count < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
				}
				if (startIndex > this.m_InnerCount - count)
				{
					throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
				}
				int num = this.m_InnerArrayList.IndexOf(value, this.m_InnerIndex + startIndex, count);
				if (num == -1)
				{
					return -1;
				}
				return num - this.m_InnerIndex;
			}

			public override int LastIndexOf(object value)
			{
				return this.LastIndexOf(value, this.m_InnerCount - 1);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return this.LastIndexOf(value, startIndex, startIndex + 1);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "< 0");
				}
				if (count < 0)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("count", count, "count is negative.");
				}
				int num = this.m_InnerArrayList.LastIndexOf(value, this.m_InnerIndex + startIndex, count);
				if (num == -1)
				{
					return -1;
				}
				return num - this.m_InnerIndex;
			}

			public override void Insert(int index, object value)
			{
				this.VerifyStateChanges();
				if (index < 0 || index > this.m_InnerCount)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				this.m_InnerArrayList.Insert(this.m_InnerIndex + index, value);
				this.m_InnerCount++;
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override void InsertRange(int index, ICollection c)
			{
				this.VerifyStateChanges();
				if (index < 0 || index > this.m_InnerCount)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				this.m_InnerArrayList.InsertRange(this.m_InnerIndex + index, c);
				this.m_InnerCount += c.Count;
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override void Remove(object value)
			{
				this.VerifyStateChanges();
				int num = this.IndexOf(value);
				if (num > -1)
				{
					this.RemoveAt(num);
				}
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override void RemoveAt(int index)
			{
				this.VerifyStateChanges();
				if (index < 0 || index > this.m_InnerCount)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				this.m_InnerArrayList.RemoveAt(this.m_InnerIndex + index);
				this.m_InnerCount--;
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override void RemoveRange(int index, int count)
			{
				this.VerifyStateChanges();
				ArrayList.CheckRange(index, count, this.m_InnerCount);
				this.m_InnerArrayList.RemoveRange(this.m_InnerIndex + index, count);
				this.m_InnerCount -= count;
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override void Reverse()
			{
				this.Reverse(0, this.m_InnerCount);
			}

			public override void Reverse(int index, int count)
			{
				this.VerifyStateChanges();
				ArrayList.CheckRange(index, count, this.m_InnerCount);
				this.m_InnerArrayList.Reverse(this.m_InnerIndex + index, count);
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override void SetRange(int index, ICollection c)
			{
				this.VerifyStateChanges();
				if (index < 0 || index > this.m_InnerCount)
				{
					ArrayList.ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				this.m_InnerArrayList.SetRange(this.m_InnerIndex + index, c);
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override void CopyTo(Array array)
			{
				this.CopyTo(array, 0);
			}

			public override void CopyTo(Array array, int index)
			{
				this.CopyTo(0, array, index, this.m_InnerCount);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				ArrayList.CheckRange(index, count, this.m_InnerCount);
				this.m_InnerArrayList.CopyTo(this.m_InnerIndex + index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return this.GetEnumerator(0, this.m_InnerCount);
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				ArrayList.CheckRange(index, count, this.m_InnerCount);
				return this.m_InnerArrayList.GetEnumerator(this.m_InnerIndex + index, count);
			}

			public override void AddRange(ICollection c)
			{
				this.VerifyStateChanges();
				this.m_InnerArrayList.InsertRange(this.m_InnerCount, c);
				this.m_InnerCount += c.Count;
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override int BinarySearch(object value)
			{
				return this.BinarySearch(0, this.m_InnerCount, value, Comparer.Default);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return this.BinarySearch(0, this.m_InnerCount, value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				ArrayList.CheckRange(index, count, this.m_InnerCount);
				return this.m_InnerArrayList.BinarySearch(this.m_InnerIndex + index, count, value, comparer);
			}

			public override object Clone()
			{
				return new ArrayList.RangedArrayList((ArrayList)this.m_InnerArrayList.Clone(), this.m_InnerIndex, this.m_InnerCount);
			}

			public override ArrayList GetRange(int index, int count)
			{
				ArrayList.CheckRange(index, count, this.m_InnerCount);
				return new ArrayList.RangedArrayList(this, index, count);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException();
			}

			public override void Sort()
			{
				this.Sort(Comparer.Default);
			}

			public override void Sort(IComparer comparer)
			{
				this.Sort(0, this.m_InnerCount, comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				this.VerifyStateChanges();
				ArrayList.CheckRange(index, count, this.m_InnerCount);
				this.m_InnerArrayList.Sort(this.m_InnerIndex + index, count, comparer);
				this.m_InnerStateChanges = this.m_InnerArrayList._version;
			}

			public override object[] ToArray()
			{
				object[] array = new object[this.m_InnerCount];
				this.m_InnerArrayList.CopyTo(this.m_InnerIndex, array, 0, this.m_InnerCount);
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				Array array = Array.CreateInstance(elementType, this.m_InnerCount);
				this.m_InnerArrayList.CopyTo(this.m_InnerIndex, array, 0, this.m_InnerCount);
				return array;
			}

			private int m_InnerIndex;

			private int m_InnerCount;

			private int m_InnerStateChanges;
		}

		[Serializable]
		private sealed class SynchronizedListWrapper : ArrayList.ListWrapper
		{
			public SynchronizedListWrapper(IList innerList)
				: base(innerList)
			{
				this.m_SyncRoot = innerList.SyncRoot;
			}

			public override int Count
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					int count;
					lock (syncRoot)
					{
						count = this.m_InnerList.Count;
					}
					return count;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object SyncRoot
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					object syncRoot2;
					lock (syncRoot)
					{
						syncRoot2 = this.m_InnerList.SyncRoot;
					}
					return syncRoot2;
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					bool isFixedSize;
					lock (syncRoot)
					{
						isFixedSize = this.m_InnerList.IsFixedSize;
					}
					return isFixedSize;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					bool isReadOnly;
					lock (syncRoot)
					{
						isReadOnly = this.m_InnerList.IsReadOnly;
					}
					return isReadOnly;
				}
			}

			public override object this[int index]
			{
				get
				{
					object syncRoot = this.m_SyncRoot;
					object obj;
					lock (syncRoot)
					{
						obj = this.m_InnerList[index];
					}
					return obj;
				}
				set
				{
					object syncRoot = this.m_SyncRoot;
					lock (syncRoot)
					{
						this.m_InnerList[index] = value;
					}
				}
			}

			public override int Add(object value)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerList.Add(value);
				}
				return num;
			}

			public override void Clear()
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerList.Clear();
				}
			}

			public override bool Contains(object value)
			{
				object syncRoot = this.m_SyncRoot;
				bool flag;
				lock (syncRoot)
				{
					flag = this.m_InnerList.Contains(value);
				}
				return flag;
			}

			public override int IndexOf(object value)
			{
				object syncRoot = this.m_SyncRoot;
				int num;
				lock (syncRoot)
				{
					num = this.m_InnerList.IndexOf(value);
				}
				return num;
			}

			public override void Insert(int index, object value)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerList.Insert(index, value);
				}
			}

			public override void Remove(object value)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerList.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerList.RemoveAt(index);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				object syncRoot = this.m_SyncRoot;
				lock (syncRoot)
				{
					this.m_InnerList.CopyTo(array, index);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				object syncRoot = this.m_SyncRoot;
				IEnumerator enumerator;
				lock (syncRoot)
				{
					enumerator = this.m_InnerList.GetEnumerator();
				}
				return enumerator;
			}

			private object m_SyncRoot;
		}

		[Serializable]
		private class FixedSizeListWrapper : ArrayList.ListWrapper
		{
			public FixedSizeListWrapper(IList innerList)
				: base(innerList)
			{
			}

			protected virtual string ErrorMessage
			{
				get
				{
					return "List is fixed-size.";
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public override int Add(object value)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Clear()
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Insert(int index, object value)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException(this.ErrorMessage);
			}
		}

		[Serializable]
		private sealed class ReadOnlyListWrapper : ArrayList.FixedSizeListWrapper
		{
			public ReadOnlyListWrapper(IList innerList)
				: base(innerList)
			{
			}

			protected override string ErrorMessage
			{
				get
				{
					return "List is read-only.";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override object this[int index]
			{
				get
				{
					return this.m_InnerList[index];
				}
				set
				{
					throw new NotSupportedException(this.ErrorMessage);
				}
			}
		}

		[Serializable]
		private class ListWrapper : IEnumerable, ICollection, IList
		{
			public ListWrapper(IList innerList)
			{
				this.m_InnerList = innerList;
			}

			public virtual object this[int index]
			{
				get
				{
					return this.m_InnerList[index];
				}
				set
				{
					this.m_InnerList[index] = value;
				}
			}

			public virtual int Count
			{
				get
				{
					return this.m_InnerList.Count;
				}
			}

			public virtual bool IsSynchronized
			{
				get
				{
					return this.m_InnerList.IsSynchronized;
				}
			}

			public virtual object SyncRoot
			{
				get
				{
					return this.m_InnerList.SyncRoot;
				}
			}

			public virtual bool IsFixedSize
			{
				get
				{
					return this.m_InnerList.IsFixedSize;
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return this.m_InnerList.IsReadOnly;
				}
			}

			public virtual int Add(object value)
			{
				return this.m_InnerList.Add(value);
			}

			public virtual void Clear()
			{
				this.m_InnerList.Clear();
			}

			public virtual bool Contains(object value)
			{
				return this.m_InnerList.Contains(value);
			}

			public virtual int IndexOf(object value)
			{
				return this.m_InnerList.IndexOf(value);
			}

			public virtual void Insert(int index, object value)
			{
				this.m_InnerList.Insert(index, value);
			}

			public virtual void Remove(object value)
			{
				this.m_InnerList.Remove(value);
			}

			public virtual void RemoveAt(int index)
			{
				this.m_InnerList.RemoveAt(index);
			}

			public virtual void CopyTo(Array array, int index)
			{
				this.m_InnerList.CopyTo(array, index);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return this.m_InnerList.GetEnumerator();
			}

			protected IList m_InnerList;
		}
	}
}
