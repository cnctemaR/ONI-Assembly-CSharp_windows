using System;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
	[ComVisible(false)]
	[Serializable]
	public class SortedList<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable, IDictionary<TKey, TValue>
	{
		public SortedList()
			: this(SortedList<TKey, TValue>.INITIAL_SIZE, null)
		{
		}

		public SortedList(int capacity)
			: this(capacity, null)
		{
		}

		public SortedList(int capacity, IComparer<TKey> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity");
			}
			if (capacity == 0)
			{
				this.defaultCapacity = 0;
			}
			else
			{
				this.defaultCapacity = SortedList<TKey, TValue>.INITIAL_SIZE;
			}
			this.Init(comparer, capacity, true);
		}

		public SortedList(IComparer<TKey> comparer)
			: this(SortedList<TKey, TValue>.INITIAL_SIZE, comparer)
		{
		}

		public SortedList(IDictionary<TKey, TValue> dictionary)
			: this(dictionary, null)
		{
		}

		public SortedList(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this.Init(comparer, dictionary.Count, true);
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
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

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (!(key is TKey))
				{
					return null;
				}
				return this[(TKey)((object)key)];
			}
			set
			{
				this[this.ToKey(key)] = this.ToValue(value);
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return new SortedList<TKey, TValue>.ListKeys(this);
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return new SortedList<TKey, TValue>.ListValues(this);
			}
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				return this.Values;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			this.defaultCapacity = SortedList<TKey, TValue>.INITIAL_SIZE;
			this.table = new KeyValuePair<TKey, TValue>[this.defaultCapacity];
			this.inUse = 0;
			this.modificationCount++;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (this.Count == 0)
			{
				return;
			}
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (arrayIndex >= array.Length)
			{
				throw new ArgumentNullException("arrayIndex is greater than or equal to array.Length");
			}
			if (this.Count > array.Length - arrayIndex)
			{
				throw new ArgumentNullException("Not enough space in array from arrayIndex to end of array");
			}
			int num = arrayIndex;
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				array[num++] = keyValuePair;
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			this.Add(keyValuePair.Key, keyValuePair.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			int num = this.Find(keyValuePair.Key);
			return num >= 0 && Comparer<KeyValuePair<TKey, TValue>>.Default.Compare(this.table[num], keyValuePair) == 0;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			int num = this.Find(keyValuePair.Key);
			if (num >= 0 && Comparer<KeyValuePair<TKey, TValue>>.Default.Compare(this.table[num], keyValuePair) == 0)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			for (int i = 0; i < this.inUse; i++)
			{
				KeyValuePair<TKey, TValue> current = this.table[i];
				yield return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void IDictionary.Add(object key, object value)
		{
			this.PutImpl(this.ToKey(key), this.ToValue(value), false);
		}

		bool IDictionary.Contains(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException();
			}
			return key is TKey && this.Find((TKey)((object)key)) >= 0;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new SortedList<TKey, TValue>.Enumerator(this, SortedList<TKey, TValue>.EnumeratorMode.ENTRY_MODE);
		}

		void IDictionary.Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!(key is TKey))
			{
				return;
			}
			int num = this.IndexOfKey((TKey)((object)key));
			if (num >= 0)
			{
				this.RemoveAt(num);
			}
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if (this.Count == 0)
			{
				return;
			}
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("array is multi-dimensional");
			}
			if (arrayIndex >= array.Length)
			{
				throw new ArgumentNullException("arrayIndex is greater than or equal to array.Length");
			}
			if (this.Count > array.Length - arrayIndex)
			{
				throw new ArgumentNullException("Not enough space in array from arrayIndex to end of array");
			}
			IEnumerator<KeyValuePair<TKey, TValue>> enumerator = this.GetEnumerator();
			int num = arrayIndex;
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, TValue> keyValuePair = enumerator.Current;
				array.SetValue(keyValuePair, num++);
			}
		}

		public int Count
		{
			get
			{
				return this.inUse;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int num = this.Find(key);
				if (num >= 0)
				{
					return this.table[num].Value;
				}
				throw new KeyNotFoundException();
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				this.PutImpl(key, value, true);
			}
		}

		public int Capacity
		{
			get
			{
				return this.table.Length;
			}
			set
			{
				int num = this.table.Length;
				if (this.inUse > value)
				{
					throw new ArgumentOutOfRangeException("capacity too small");
				}
				if (value == 0)
				{
					KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[this.defaultCapacity];
					Array.Copy(this.table, array, this.inUse);
					this.table = array;
				}
				else if (value > this.inUse)
				{
					KeyValuePair<TKey, TValue>[] array2 = new KeyValuePair<TKey, TValue>[value];
					Array.Copy(this.table, array2, this.inUse);
					this.table = array2;
				}
				else if (value > num)
				{
					KeyValuePair<TKey, TValue>[] array3 = new KeyValuePair<TKey, TValue>[value];
					Array.Copy(this.table, array3, num);
					this.table = array3;
				}
			}
		}

		public IList<TKey> Keys
		{
			get
			{
				return new SortedList<TKey, TValue>.ListKeys(this);
			}
		}

		public IList<TValue> Values
		{
			get
			{
				return new SortedList<TKey, TValue>.ListValues(this);
			}
		}

		public IComparer<TKey> Comparer
		{
			get
			{
				return this.comparer;
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.PutImpl(key, value, false);
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this.Find(key) >= 0;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			for (int i = 0; i < this.inUse; i++)
			{
				KeyValuePair<TKey, TValue> current = this.table[i];
				yield return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
			}
			yield break;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.IndexOfKey(key);
			if (num >= 0)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		public void Clear()
		{
			this.defaultCapacity = SortedList<TKey, TValue>.INITIAL_SIZE;
			this.table = new KeyValuePair<TKey, TValue>[this.defaultCapacity];
			this.inUse = 0;
			this.modificationCount++;
		}

		public void RemoveAt(int index)
		{
			KeyValuePair<TKey, TValue>[] array = this.table;
			int count = this.Count;
			if (index >= 0 && index < count)
			{
				if (index != count - 1)
				{
					Array.Copy(array, index + 1, array, index, count - 1 - index);
				}
				else
				{
					array[index] = default(KeyValuePair<TKey, TValue>);
				}
				this.inUse--;
				this.modificationCount++;
				return;
			}
			throw new ArgumentOutOfRangeException("index out of range");
		}

		public int IndexOfKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = 0;
			try
			{
				num = this.Find(key);
			}
			catch (Exception)
			{
				throw new InvalidOperationException();
			}
			return num | (num >> 31);
		}

		public int IndexOfValue(TValue value)
		{
			if (this.inUse == 0)
			{
				return -1;
			}
			for (int i = 0; i < this.inUse; i++)
			{
				KeyValuePair<TKey, TValue> keyValuePair = this.table[i];
				if (object.Equals(value, keyValuePair.Value))
				{
					return i;
				}
			}
			return -1;
		}

		public bool ContainsValue(TValue value)
		{
			return this.IndexOfValue(value) >= 0;
		}

		public void TrimExcess()
		{
			if ((double)this.inUse < (double)this.table.Length * 0.9)
			{
				this.Capacity = this.inUse;
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.Find(key);
			if (num >= 0)
			{
				value = this.table[num].Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		private void EnsureCapacity(int n, int free)
		{
			KeyValuePair<TKey, TValue>[] array = this.table;
			KeyValuePair<TKey, TValue>[] array2 = null;
			int capacity = this.Capacity;
			bool flag = free >= 0 && free < this.Count;
			if (n > capacity)
			{
				array2 = new KeyValuePair<TKey, TValue>[n << 1];
			}
			if (array2 != null)
			{
				if (flag)
				{
					if (free > 0)
					{
						Array.Copy(array, 0, array2, 0, free);
					}
					int num = this.Count - free;
					if (num > 0)
					{
						Array.Copy(array, free, array2, free + 1, num);
					}
				}
				else
				{
					Array.Copy(array, array2, this.Count);
				}
				this.table = array2;
			}
			else if (flag)
			{
				Array.Copy(array, free, array, free + 1, this.Count - free);
			}
		}

		private void PutImpl(TKey key, TValue value, bool overwrite)
		{
			if (key == null)
			{
				throw new ArgumentNullException("null key");
			}
			KeyValuePair<TKey, TValue>[] array = this.table;
			int num = -1;
			try
			{
				num = this.Find(key);
			}
			catch (Exception)
			{
				throw new InvalidOperationException();
			}
			if (num >= 0)
			{
				if (!overwrite)
				{
					throw new ArgumentException("element already exists");
				}
				array[num] = new KeyValuePair<TKey, TValue>(key, value);
				this.modificationCount++;
				return;
			}
			else
			{
				num = ~num;
				if (num > this.Capacity + 1)
				{
					throw new Exception(string.Concat(new object[] { "SortedList::internal error (", key, ", ", value, ") at [", num, "]" }));
				}
				this.EnsureCapacity(this.Count + 1, num);
				array = this.table;
				array[num] = new KeyValuePair<TKey, TValue>(key, value);
				this.inUse++;
				this.modificationCount++;
				return;
			}
		}

		private void Init(IComparer<TKey> comparer, int capacity, bool forceSize)
		{
			if (comparer == null)
			{
				comparer = Comparer<TKey>.Default;
			}
			this.comparer = comparer;
			if (!forceSize && capacity < this.defaultCapacity)
			{
				capacity = this.defaultCapacity;
			}
			this.table = new KeyValuePair<TKey, TValue>[capacity];
			this.inUse = 0;
			this.modificationCount = 0;
		}

		private void CopyToArray(Array arr, int i, SortedList<TKey, TValue>.EnumeratorMode mode)
		{
			if (arr == null)
			{
				throw new ArgumentNullException("arr");
			}
			if (i < 0 || i + this.Count > arr.Length)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			IEnumerator enumerator = new SortedList<TKey, TValue>.Enumerator(this, mode);
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				arr.SetValue(obj, i++);
			}
		}

		private int Find(TKey key)
		{
			KeyValuePair<TKey, TValue>[] array = this.table;
			int count = this.Count;
			if (count == 0)
			{
				return -1;
			}
			int i = 0;
			int num = count - 1;
			while (i <= num)
			{
				int num2 = i + num >> 1;
				int num3 = this.comparer.Compare(array[num2].Key, key);
				if (num3 == 0)
				{
					return num2;
				}
				if (num3 < 0)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return ~i;
		}

		private TKey ToKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!(key is TKey))
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"The value \"",
					key,
					"\" isn't of type \"",
					typeof(TKey),
					"\" and can't be used in this generic collection."
				}), "key");
			}
			return (TKey)((object)key);
		}

		private TValue ToValue(object value)
		{
			if (!(value is TValue))
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"The value \"",
					value,
					"\" isn't of type \"",
					typeof(TValue),
					"\" and can't be used in this generic collection."
				}), "value");
			}
			return (TValue)((object)value);
		}

		internal TKey KeyAt(int index)
		{
			if (index >= 0 && index < this.Count)
			{
				return this.table[index].Key;
			}
			throw new ArgumentOutOfRangeException("Index out of range");
		}

		internal TValue ValueAt(int index)
		{
			if (index >= 0 && index < this.Count)
			{
				return this.table[index].Value;
			}
			throw new ArgumentOutOfRangeException("Index out of range");
		}

		private static readonly int INITIAL_SIZE = 16;

		private int inUse;

		private int modificationCount;

		private KeyValuePair<TKey, TValue>[] table;

		private IComparer<TKey> comparer;

		private int defaultCapacity;

		private enum EnumeratorMode
		{
			KEY_MODE,
			VALUE_MODE,
			ENTRY_MODE
		}

		private sealed class Enumerator : IEnumerator, IDictionaryEnumerator, ICloneable
		{
			public Enumerator(SortedList<TKey, TValue> host, SortedList<TKey, TValue>.EnumeratorMode mode)
			{
				this.host = host;
				this.stamp = host.modificationCount;
				this.size = host.Count;
				this.mode = mode;
				this.Reset();
			}

			public Enumerator(SortedList<TKey, TValue> host)
				: this(host, SortedList<TKey, TValue>.EnumeratorMode.ENTRY_MODE)
			{
			}

			public void Reset()
			{
				if (this.host.modificationCount != this.stamp || this.invalid)
				{
					throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
				}
				this.pos = -1;
				this.currentKey = null;
				this.currentValue = null;
			}

			public bool MoveNext()
			{
				if (this.host.modificationCount != this.stamp || this.invalid)
				{
					throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
				}
				KeyValuePair<TKey, TValue>[] table = this.host.table;
				if (++this.pos < this.size)
				{
					KeyValuePair<TKey, TValue> keyValuePair = table[this.pos];
					this.currentKey = keyValuePair.Key;
					this.currentValue = keyValuePair.Value;
					return true;
				}
				this.currentKey = null;
				this.currentValue = null;
				return false;
			}

			public DictionaryEntry Entry
			{
				get
				{
					if (this.invalid || this.pos >= this.size || this.pos == -1)
					{
						throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
					}
					return new DictionaryEntry(this.currentKey, this.currentValue);
				}
			}

			public object Key
			{
				get
				{
					if (this.invalid || this.pos >= this.size || this.pos == -1)
					{
						throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
					}
					return this.currentKey;
				}
			}

			public object Value
			{
				get
				{
					if (this.invalid || this.pos >= this.size || this.pos == -1)
					{
						throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
					}
					return this.currentValue;
				}
			}

			public object Current
			{
				get
				{
					if (this.invalid || this.pos >= this.size || this.pos == -1)
					{
						throw new InvalidOperationException(SortedList<TKey, TValue>.Enumerator.xstr);
					}
					switch (this.mode)
					{
					case SortedList<TKey, TValue>.EnumeratorMode.KEY_MODE:
						return this.currentKey;
					case SortedList<TKey, TValue>.EnumeratorMode.VALUE_MODE:
						return this.currentValue;
					case SortedList<TKey, TValue>.EnumeratorMode.ENTRY_MODE:
						return this.Entry;
					default:
						throw new NotSupportedException(this.mode + " is not a supported mode.");
					}
				}
			}

			public object Clone()
			{
				return new SortedList<TKey, TValue>.Enumerator(this.host, this.mode)
				{
					stamp = this.stamp,
					pos = this.pos,
					size = this.size,
					currentKey = this.currentKey,
					currentValue = this.currentValue,
					invalid = this.invalid
				};
			}

			private SortedList<TKey, TValue> host;

			private int stamp;

			private int pos;

			private int size;

			private SortedList<TKey, TValue>.EnumeratorMode mode;

			private object currentKey;

			private object currentValue;

			private bool invalid;

			private static readonly string xstr = "SortedList.Enumerator: snapshot out of sync.";
		}

		[Serializable]
		public struct KeyEnumerator : IEnumerator, IDisposable, IEnumerator<TKey>
		{
			internal KeyEnumerator(SortedList<TKey, TValue> l)
			{
				this.l = l;
				this.idx = -2;
				this.ver = l.modificationCount;
			}

			void IEnumerator.Reset()
			{
				if (this.ver != this.l.modificationCount)
				{
					throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
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
				if (this.ver != this.l.modificationCount)
				{
					throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
				}
				if (this.idx == -2)
				{
					this.idx = this.l.Count;
				}
				return this.idx != -1 && --this.idx != -1;
			}

			public TKey Current
			{
				get
				{
					if (this.idx < 0)
					{
						throw new InvalidOperationException();
					}
					return this.l.KeyAt(this.l.Count - 1 - this.idx);
				}
			}

			private const int NOT_STARTED = -2;

			private const int FINISHED = -1;

			private SortedList<TKey, TValue> l;

			private int idx;

			private int ver;
		}

		[Serializable]
		public struct ValueEnumerator : IEnumerator, IDisposable, IEnumerator<TValue>
		{
			internal ValueEnumerator(SortedList<TKey, TValue> l)
			{
				this.l = l;
				this.idx = -2;
				this.ver = l.modificationCount;
			}

			void IEnumerator.Reset()
			{
				if (this.ver != this.l.modificationCount)
				{
					throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
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
				if (this.ver != this.l.modificationCount)
				{
					throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
				}
				if (this.idx == -2)
				{
					this.idx = this.l.Count;
				}
				return this.idx != -1 && --this.idx != -1;
			}

			public TValue Current
			{
				get
				{
					if (this.idx < 0)
					{
						throw new InvalidOperationException();
					}
					return this.l.ValueAt(this.l.Count - 1 - this.idx);
				}
			}

			private const int NOT_STARTED = -2;

			private const int FINISHED = -1;

			private SortedList<TKey, TValue> l;

			private int idx;

			private int ver;
		}

		private class ListKeys : ICollection, IEnumerable, IList<TKey>, ICollection<TKey>, IEnumerable<TKey>
		{
			public ListKeys(SortedList<TKey, TValue> host)
			{
				if (host == null)
				{
					throw new ArgumentNullException();
				}
				this.host = host;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < this.host.Count; i++)
				{
					yield return this.host.KeyAt(i);
				}
				yield break;
			}

			public virtual void Add(TKey item)
			{
				throw new NotSupportedException();
			}

			public virtual bool Remove(TKey key)
			{
				throw new NotSupportedException();
			}

			public virtual void Clear()
			{
				throw new NotSupportedException();
			}

			public virtual void CopyTo(TKey[] array, int arrayIndex)
			{
				if (this.host.Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (arrayIndex >= array.Length)
				{
					throw new ArgumentOutOfRangeException("arrayIndex is greater than or equal to array.Length");
				}
				if (this.Count > array.Length - arrayIndex)
				{
					throw new ArgumentOutOfRangeException("Not enough space in array from arrayIndex to end of array");
				}
				int num = arrayIndex;
				for (int i = 0; i < this.Count; i++)
				{
					array[num++] = this.host.KeyAt(i);
				}
			}

			public virtual bool Contains(TKey item)
			{
				return this.host.IndexOfKey(item) > -1;
			}

			public virtual int IndexOf(TKey item)
			{
				return this.host.IndexOfKey(item);
			}

			public virtual void Insert(int index, TKey item)
			{
				throw new NotSupportedException();
			}

			public virtual void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public virtual TKey this[int index]
			{
				get
				{
					return this.host.KeyAt(index);
				}
				set
				{
					throw new NotSupportedException("attempt to modify a key");
				}
			}

			public virtual IEnumerator<TKey> GetEnumerator()
			{
				return new SortedList<TKey, TValue>.KeyEnumerator(this.host);
			}

			public virtual int Count
			{
				get
				{
					return this.host.Count;
				}
			}

			public virtual bool IsSynchronized
			{
				get
				{
					return ((ICollection)this.host).IsSynchronized;
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public virtual object SyncRoot
			{
				get
				{
					return ((ICollection)this.host).SyncRoot;
				}
			}

			public virtual void CopyTo(Array array, int arrayIndex)
			{
				this.host.CopyToArray(array, arrayIndex, SortedList<TKey, TValue>.EnumeratorMode.KEY_MODE);
			}

			private SortedList<TKey, TValue> host;
		}

		private class ListValues : ICollection, IEnumerable, IList<TValue>, ICollection<TValue>, IEnumerable<TValue>
		{
			public ListValues(SortedList<TKey, TValue> host)
			{
				if (host == null)
				{
					throw new ArgumentNullException();
				}
				this.host = host;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				for (int i = 0; i < this.host.Count; i++)
				{
					yield return this.host.ValueAt(i);
				}
				yield break;
			}

			public virtual void Add(TValue item)
			{
				throw new NotSupportedException();
			}

			public virtual bool Remove(TValue value)
			{
				throw new NotSupportedException();
			}

			public virtual void Clear()
			{
				throw new NotSupportedException();
			}

			public virtual void CopyTo(TValue[] array, int arrayIndex)
			{
				if (this.host.Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (arrayIndex >= array.Length)
				{
					throw new ArgumentOutOfRangeException("arrayIndex is greater than or equal to array.Length");
				}
				if (this.Count > array.Length - arrayIndex)
				{
					throw new ArgumentOutOfRangeException("Not enough space in array from arrayIndex to end of array");
				}
				int num = arrayIndex;
				for (int i = 0; i < this.Count; i++)
				{
					array[num++] = this.host.ValueAt(i);
				}
			}

			public virtual bool Contains(TValue item)
			{
				return this.host.IndexOfValue(item) > -1;
			}

			public virtual int IndexOf(TValue item)
			{
				return this.host.IndexOfValue(item);
			}

			public virtual void Insert(int index, TValue item)
			{
				throw new NotSupportedException();
			}

			public virtual void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public virtual TValue this[int index]
			{
				get
				{
					return this.host.ValueAt(index);
				}
				set
				{
					throw new NotSupportedException("attempt to modify a key");
				}
			}

			public virtual IEnumerator<TValue> GetEnumerator()
			{
				return new SortedList<TKey, TValue>.ValueEnumerator(this.host);
			}

			public virtual int Count
			{
				get
				{
					return this.host.Count;
				}
			}

			public virtual bool IsSynchronized
			{
				get
				{
					return ((ICollection)this.host).IsSynchronized;
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public virtual object SyncRoot
			{
				get
				{
					return ((ICollection)this.host).SyncRoot;
				}
			}

			public virtual void CopyTo(Array array, int arrayIndex)
			{
				this.host.CopyToArray(array, arrayIndex, SortedList<TKey, TValue>.EnumeratorMode.VALUE_MODE);
			}

			private SortedList<TKey, TValue> host;
		}
	}
}
