using System;
using System.Runtime.Serialization;

namespace System.Collections.Specialized
{
	[Serializable]
	public class OrderedDictionary : IDictionary, ICollection, IEnumerable, IDeserializationCallback, IOrderedDictionary, ISerializable
	{
		public OrderedDictionary()
		{
			this.list = new ArrayList();
			this.hash = new Hashtable();
		}

		public OrderedDictionary(int capacity)
		{
			this.initialCapacity = ((capacity >= 0) ? capacity : 0);
			this.list = new ArrayList(this.initialCapacity);
			this.hash = new Hashtable(this.initialCapacity);
		}

		public OrderedDictionary(IEqualityComparer equalityComparer)
		{
			this.list = new ArrayList();
			this.hash = new Hashtable(equalityComparer);
			this.comparer = equalityComparer;
		}

		public OrderedDictionary(int capacity, IEqualityComparer equalityComparer)
		{
			this.initialCapacity = ((capacity >= 0) ? capacity : 0);
			this.list = new ArrayList(this.initialCapacity);
			this.hash = new Hashtable(this.initialCapacity, equalityComparer);
			this.comparer = equalityComparer;
		}

		protected OrderedDictionary(SerializationInfo info, StreamingContext context)
		{
			this.serializationInfo = info;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			if (this.serializationInfo == null)
			{
				return;
			}
			this.comparer = (IEqualityComparer)this.serializationInfo.GetValue("KeyComparer", typeof(IEqualityComparer));
			this.readOnly = this.serializationInfo.GetBoolean("ReadOnly");
			this.initialCapacity = this.serializationInfo.GetInt32("InitialCapacity");
			if (this.list == null)
			{
				this.list = new ArrayList();
			}
			else
			{
				this.list.Clear();
			}
			this.hash = new Hashtable(this.comparer);
			object[] array = (object[])this.serializationInfo.GetValue("ArrayList", typeof(object[]));
			foreach (DictionaryEntry dictionaryEntry in array)
			{
				this.hash.Add(dictionaryEntry.Key, dictionaryEntry.Value);
				this.list.Add(dictionaryEntry);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		protected virtual void OnDeserialization(object sender)
		{
			((IDeserializationCallback)this).OnDeserialization(sender);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("KeyComparer", this.comparer, typeof(IEqualityComparer));
			info.AddValue("ReadOnly", this.readOnly);
			info.AddValue("InitialCapacity", this.initialCapacity);
			object[] array = new object[this.hash.Count];
			this.hash.CopyTo(array, 0);
			info.AddValue("ArrayList", array);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public bool IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public object this[object key]
		{
			get
			{
				return this.hash[key];
			}
			set
			{
				this.WriteCheck();
				if (this.hash.Contains(key))
				{
					int num = this.FindListEntry(key);
					this.list[num] = new DictionaryEntry(key, value);
				}
				else
				{
					this.list.Add(new DictionaryEntry(key, value));
				}
				this.hash[key] = value;
			}
		}

		public object this[int index]
		{
			get
			{
				return ((DictionaryEntry)this.list[index]).Value;
			}
			set
			{
				this.WriteCheck();
				DictionaryEntry dictionaryEntry = (DictionaryEntry)this.list[index];
				dictionaryEntry.Value = value;
				this.list[index] = dictionaryEntry;
				this.hash[dictionaryEntry.Key] = value;
			}
		}

		public ICollection Keys
		{
			get
			{
				return new OrderedDictionary.OrderedCollection(this.list, true);
			}
		}

		public ICollection Values
		{
			get
			{
				return new OrderedDictionary.OrderedCollection(this.list, false);
			}
		}

		public void Add(object key, object value)
		{
			this.WriteCheck();
			this.hash.Add(key, value);
			this.list.Add(new DictionaryEntry(key, value));
		}

		public void Clear()
		{
			this.WriteCheck();
			this.hash.Clear();
			this.list.Clear();
		}

		public bool Contains(object key)
		{
			return this.hash.Contains(key);
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new OrderedDictionary.OrderedEntryCollectionEnumerator(this.list.GetEnumerator());
		}

		public void Remove(object key)
		{
			this.WriteCheck();
			if (this.hash.Contains(key))
			{
				this.hash.Remove(key);
				int num = this.FindListEntry(key);
				this.list.RemoveAt(num);
			}
		}

		private int FindListEntry(object key)
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)this.list[i];
				if ((this.comparer == null) ? dictionaryEntry.Key.Equals(key) : this.comparer.Equals(dictionaryEntry.Key, key))
				{
					return i;
				}
			}
			return -1;
		}

		private void WriteCheck()
		{
			if (this.readOnly)
			{
				throw new NotSupportedException("Collection is read only");
			}
		}

		public OrderedDictionary AsReadOnly()
		{
			return new OrderedDictionary
			{
				list = this.list,
				hash = this.hash,
				comparer = this.comparer,
				readOnly = true
			};
		}

		public void Insert(int index, object key, object value)
		{
			this.WriteCheck();
			this.hash.Add(key, value);
			this.list.Insert(index, new DictionaryEntry(key, value));
		}

		public void RemoveAt(int index)
		{
			this.WriteCheck();
			DictionaryEntry dictionaryEntry = (DictionaryEntry)this.list[index];
			this.list.RemoveAt(index);
			this.hash.Remove(dictionaryEntry.Key);
		}

		private ArrayList list;

		private Hashtable hash;

		private bool readOnly;

		private int initialCapacity;

		private SerializationInfo serializationInfo;

		private IEqualityComparer comparer;

		private class OrderedEntryCollectionEnumerator : IEnumerator, IDictionaryEnumerator
		{
			public OrderedEntryCollectionEnumerator(IEnumerator listEnumerator)
			{
				this.listEnumerator = listEnumerator;
			}

			public bool MoveNext()
			{
				return this.listEnumerator.MoveNext();
			}

			public void Reset()
			{
				this.listEnumerator.Reset();
			}

			public object Current
			{
				get
				{
					return this.listEnumerator.Current;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					return (DictionaryEntry)this.listEnumerator.Current;
				}
			}

			public object Key
			{
				get
				{
					return this.Entry.Key;
				}
			}

			public object Value
			{
				get
				{
					return this.Entry.Value;
				}
			}

			private IEnumerator listEnumerator;
		}

		private class OrderedCollection : ICollection, IEnumerable
		{
			public OrderedCollection(ArrayList list, bool isKeyList)
			{
				this.list = list;
				this.isKeyList = isKeyList;
			}

			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}

			public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

			public object SyncRoot
			{
				get
				{
					return this.list.SyncRoot;
				}
			}

			public void CopyTo(Array array, int index)
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)this.list[i];
					if (this.isKeyList)
					{
						array.SetValue(dictionaryEntry.Key, index + i);
					}
					else
					{
						array.SetValue(dictionaryEntry.Value, index + i);
					}
				}
			}

			public IEnumerator GetEnumerator()
			{
				return new OrderedDictionary.OrderedCollection.OrderedCollectionEnumerator(this.list.GetEnumerator(), this.isKeyList);
			}

			private ArrayList list;

			private bool isKeyList;

			private class OrderedCollectionEnumerator : IEnumerator
			{
				public OrderedCollectionEnumerator(IEnumerator listEnumerator, bool isKeyList)
				{
					this.listEnumerator = listEnumerator;
					this.isKeyList = isKeyList;
				}

				public object Current
				{
					get
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)this.listEnumerator.Current;
						return (!this.isKeyList) ? dictionaryEntry.Value : dictionaryEntry.Key;
					}
				}

				public bool MoveNext()
				{
					return this.listEnumerator.MoveNext();
				}

				public void Reset()
				{
					this.listEnumerator.Reset();
				}

				private bool isKeyList;

				private IEnumerator listEnumerator;
			}
		}
	}
}
