using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace System.Collections.Specialized
{
	[Serializable]
	public class OrderedDictionary : IOrderedDictionary, IDictionary, ICollection, IEnumerable, ISerializable, IDeserializationCallback
	{
		public OrderedDictionary()
			: this(0)
		{
		}

		public OrderedDictionary(int capacity)
			: this(capacity, null)
		{
		}

		public OrderedDictionary(IEqualityComparer comparer)
			: this(0, comparer)
		{
		}

		public OrderedDictionary(int capacity, IEqualityComparer comparer)
		{
			this._initialCapacity = capacity;
			this._comparer = comparer;
		}

		private OrderedDictionary(OrderedDictionary dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this._readOnly = true;
			this._objectsArray = dictionary._objectsArray;
			this._objectsTable = dictionary._objectsTable;
			this._comparer = dictionary._comparer;
			this._initialCapacity = dictionary._initialCapacity;
		}

		protected OrderedDictionary(SerializationInfo info, StreamingContext context)
		{
			this._siInfo = info;
		}

		public int Count
		{
			get
			{
				return this.objectsArray.Count;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return this._readOnly;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this._readOnly;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public ICollection Keys
		{
			get
			{
				return new OrderedDictionary.OrderedDictionaryKeyValueCollection(this.objectsArray, true);
			}
		}

		private ArrayList objectsArray
		{
			get
			{
				if (this._objectsArray == null)
				{
					this._objectsArray = new ArrayList(this._initialCapacity);
				}
				return this._objectsArray;
			}
		}

		private Hashtable objectsTable
		{
			get
			{
				if (this._objectsTable == null)
				{
					this._objectsTable = new Hashtable(this._initialCapacity, this._comparer);
				}
				return this._objectsTable;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		public object this[int index]
		{
			get
			{
				return ((DictionaryEntry)this.objectsArray[index]).Value;
			}
			set
			{
				if (this._readOnly)
				{
					throw new NotSupportedException(global::SR.GetString("The OrderedDictionary is readonly and cannot be modified."));
				}
				if (index < 0 || index >= this.objectsArray.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				object key = ((DictionaryEntry)this.objectsArray[index]).Key;
				this.objectsArray[index] = new DictionaryEntry(key, value);
				this.objectsTable[key] = value;
			}
		}

		public object this[object key]
		{
			get
			{
				return this.objectsTable[key];
			}
			set
			{
				if (this._readOnly)
				{
					throw new NotSupportedException(global::SR.GetString("The OrderedDictionary is readonly and cannot be modified."));
				}
				if (this.objectsTable.Contains(key))
				{
					this.objectsTable[key] = value;
					this.objectsArray[this.IndexOfKey(key)] = new DictionaryEntry(key, value);
					return;
				}
				this.Add(key, value);
			}
		}

		public ICollection Values
		{
			get
			{
				return new OrderedDictionary.OrderedDictionaryKeyValueCollection(this.objectsArray, false);
			}
		}

		public void Add(object key, object value)
		{
			if (this._readOnly)
			{
				throw new NotSupportedException(global::SR.GetString("The OrderedDictionary is readonly and cannot be modified."));
			}
			this.objectsTable.Add(key, value);
			this.objectsArray.Add(new DictionaryEntry(key, value));
		}

		public void Clear()
		{
			if (this._readOnly)
			{
				throw new NotSupportedException(global::SR.GetString("The OrderedDictionary is readonly and cannot be modified."));
			}
			this.objectsTable.Clear();
			this.objectsArray.Clear();
		}

		public OrderedDictionary AsReadOnly()
		{
			return new OrderedDictionary(this);
		}

		public bool Contains(object key)
		{
			return this.objectsTable.Contains(key);
		}

		public void CopyTo(Array array, int index)
		{
			this.objectsTable.CopyTo(array, index);
		}

		private int IndexOfKey(object key)
		{
			for (int i = 0; i < this.objectsArray.Count; i++)
			{
				object key2 = ((DictionaryEntry)this.objectsArray[i]).Key;
				if (this._comparer != null)
				{
					if (this._comparer.Equals(key2, key))
					{
						return i;
					}
				}
				else if (key2.Equals(key))
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, object key, object value)
		{
			if (this._readOnly)
			{
				throw new NotSupportedException(global::SR.GetString("The OrderedDictionary is readonly and cannot be modified."));
			}
			if (index > this.Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.objectsTable.Add(key, value);
			this.objectsArray.Insert(index, new DictionaryEntry(key, value));
		}

		protected virtual void OnDeserialization(object sender)
		{
			if (this._siInfo == null)
			{
				throw new SerializationException(global::SR.GetString("OnDeserialization method was called while the object was not being deserialized."));
			}
			this._comparer = (IEqualityComparer)this._siInfo.GetValue("KeyComparer", typeof(IEqualityComparer));
			this._readOnly = this._siInfo.GetBoolean("ReadOnly");
			this._initialCapacity = this._siInfo.GetInt32("InitialCapacity");
			object[] array = (object[])this._siInfo.GetValue("ArrayList", typeof(object[]));
			if (array != null)
			{
				foreach (object obj in array)
				{
					DictionaryEntry dictionaryEntry;
					try
					{
						dictionaryEntry = (DictionaryEntry)obj;
					}
					catch
					{
						throw new SerializationException(global::SR.GetString("There was an error deserializing the OrderedDictionary.  The ArrayList does not contain DictionaryEntries."));
					}
					this.objectsArray.Add(dictionaryEntry);
					this.objectsTable.Add(dictionaryEntry.Key, dictionaryEntry.Value);
				}
			}
		}

		public void RemoveAt(int index)
		{
			if (this._readOnly)
			{
				throw new NotSupportedException(global::SR.GetString("The OrderedDictionary is readonly and cannot be modified."));
			}
			if (index >= this.Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			object key = ((DictionaryEntry)this.objectsArray[index]).Key;
			this.objectsArray.RemoveAt(index);
			this.objectsTable.Remove(key);
		}

		public void Remove(object key)
		{
			if (this._readOnly)
			{
				throw new NotSupportedException(global::SR.GetString("The OrderedDictionary is readonly and cannot be modified."));
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.IndexOfKey(key);
			if (num < 0)
			{
				return;
			}
			this.objectsTable.Remove(key);
			this.objectsArray.RemoveAt(num);
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new OrderedDictionary.OrderedDictionaryEnumerator(this.objectsArray, 3);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new OrderedDictionary.OrderedDictionaryEnumerator(this.objectsArray, 3);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("KeyComparer", this._comparer, typeof(IEqualityComparer));
			info.AddValue("ReadOnly", this._readOnly);
			info.AddValue("InitialCapacity", this._initialCapacity);
			object[] array = new object[this.Count];
			this._objectsArray.CopyTo(array);
			info.AddValue("ArrayList", array);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			this.OnDeserialization(sender);
		}

		private ArrayList _objectsArray;

		private Hashtable _objectsTable;

		private int _initialCapacity;

		private IEqualityComparer _comparer;

		private bool _readOnly;

		private object _syncRoot;

		private SerializationInfo _siInfo;

		private const string KeyComparerName = "KeyComparer";

		private const string ArrayListName = "ArrayList";

		private const string ReadOnlyName = "ReadOnly";

		private const string InitCapacityName = "InitialCapacity";

		private class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
		{
			internal OrderedDictionaryEnumerator(ArrayList array, int objectReturnType)
			{
				this.arrayEnumerator = array.GetEnumerator();
				this._objectReturnType = objectReturnType;
			}

			public object Current
			{
				get
				{
					if (this._objectReturnType == 1)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)this.arrayEnumerator.Current;
						return dictionaryEntry.Key;
					}
					if (this._objectReturnType == 2)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)this.arrayEnumerator.Current;
						return dictionaryEntry.Value;
					}
					return this.Entry;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)this.arrayEnumerator.Current;
					object key = dictionaryEntry.Key;
					dictionaryEntry = (DictionaryEntry)this.arrayEnumerator.Current;
					return new DictionaryEntry(key, dictionaryEntry.Value);
				}
			}

			public object Key
			{
				get
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)this.arrayEnumerator.Current;
					return dictionaryEntry.Key;
				}
			}

			public object Value
			{
				get
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)this.arrayEnumerator.Current;
					return dictionaryEntry.Value;
				}
			}

			public bool MoveNext()
			{
				return this.arrayEnumerator.MoveNext();
			}

			public void Reset()
			{
				this.arrayEnumerator.Reset();
			}

			private int _objectReturnType;

			internal const int Keys = 1;

			internal const int Values = 2;

			internal const int DictionaryEntry = 3;

			private IEnumerator arrayEnumerator;
		}

		private class OrderedDictionaryKeyValueCollection : ICollection, IEnumerable
		{
			public OrderedDictionaryKeyValueCollection(ArrayList array, bool isKeys)
			{
				this._objects = array;
				this.isKeys = isKeys;
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				foreach (object obj in this._objects)
				{
					array.SetValue(this.isKeys ? ((DictionaryEntry)obj).Key : ((DictionaryEntry)obj).Value, index);
					index++;
				}
			}

			int ICollection.Count
			{
				get
				{
					return this._objects.Count;
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
					return this._objects.SyncRoot;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new OrderedDictionary.OrderedDictionaryEnumerator(this._objects, this.isKeys ? 1 : 2);
			}

			private ArrayList _objects;

			private bool isKeys;
		}
	}
}
