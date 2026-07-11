using System;
using System.Runtime.Serialization;

namespace System.Collections.Specialized
{
	[Serializable]
	public abstract class NameObjectCollectionBase : ICollection, IEnumerable, IDeserializationCallback, ISerializable
	{
		protected NameObjectCollectionBase()
		{
			this.m_readonly = false;
			this.m_hashprovider = CaseInsensitiveHashCodeProvider.DefaultInvariant;
			this.m_comparer = CaseInsensitiveComparer.DefaultInvariant;
			this.m_defCapacity = 0;
			this.Init();
		}

		protected NameObjectCollectionBase(int capacity)
		{
			this.m_readonly = false;
			this.m_hashprovider = CaseInsensitiveHashCodeProvider.DefaultInvariant;
			this.m_comparer = CaseInsensitiveComparer.DefaultInvariant;
			this.m_defCapacity = capacity;
			this.Init();
		}

		internal NameObjectCollectionBase(IEqualityComparer equalityComparer, IComparer comparer, IHashCodeProvider hcp)
		{
			this.equality_comparer = equalityComparer;
			this.m_comparer = comparer;
			this.m_hashprovider = hcp;
			this.m_readonly = false;
			this.m_defCapacity = 0;
			this.Init();
		}

		protected NameObjectCollectionBase(IEqualityComparer equalityComparer)
		{
			IEqualityComparer equalityComparer2;
			if (equalityComparer == null)
			{
				IEqualityComparer invariantCultureIgnoreCase = StringComparer.InvariantCultureIgnoreCase;
				equalityComparer2 = invariantCultureIgnoreCase;
			}
			else
			{
				equalityComparer2 = equalityComparer;
			}
			this..ctor(equalityComparer2, null, null);
		}

		[Obsolete("Use NameObjectCollectionBase(IEqualityComparer)")]
		protected NameObjectCollectionBase(IHashCodeProvider hashProvider, IComparer comparer)
		{
			this.m_comparer = comparer;
			this.m_hashprovider = hashProvider;
			this.m_readonly = false;
			this.m_defCapacity = 0;
			this.Init();
		}

		protected NameObjectCollectionBase(SerializationInfo info, StreamingContext context)
		{
			this.infoCopy = info;
		}

		protected NameObjectCollectionBase(int capacity, IEqualityComparer equalityComparer)
		{
			this.m_readonly = false;
			IEqualityComparer equalityComparer2;
			if (equalityComparer == null)
			{
				IEqualityComparer invariantCultureIgnoreCase = StringComparer.InvariantCultureIgnoreCase;
				equalityComparer2 = invariantCultureIgnoreCase;
			}
			else
			{
				equalityComparer2 = equalityComparer;
			}
			this.equality_comparer = equalityComparer2;
			this.m_defCapacity = capacity;
			this.Init();
		}

		[Obsolete("Use NameObjectCollectionBase(int,IEqualityComparer)")]
		protected NameObjectCollectionBase(int capacity, IHashCodeProvider hashProvider, IComparer comparer)
		{
			this.m_readonly = false;
			this.m_hashprovider = hashProvider;
			this.m_comparer = comparer;
			this.m_defCapacity = capacity;
			this.Init();
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

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.Keys).CopyTo(array, index);
		}

		internal IEqualityComparer EqualityComparer
		{
			get
			{
				return this.equality_comparer;
			}
		}

		internal IComparer Comparer
		{
			get
			{
				return this.m_comparer;
			}
		}

		internal IHashCodeProvider HashCodeProvider
		{
			get
			{
				return this.m_hashprovider;
			}
		}

		private void Init()
		{
			if (this.equality_comparer != null)
			{
				this.m_ItemsContainer = new Hashtable(this.m_defCapacity, this.equality_comparer);
			}
			else
			{
				this.m_ItemsContainer = new Hashtable(this.m_defCapacity, this.m_hashprovider, this.m_comparer);
			}
			this.m_ItemsArray = new ArrayList();
			this.m_NullKeyItem = null;
		}

		public virtual NameObjectCollectionBase.KeysCollection Keys
		{
			get
			{
				if (this.keyscoll == null)
				{
					this.keyscoll = new NameObjectCollectionBase.KeysCollection(this);
				}
				return this.keyscoll;
			}
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new NameObjectCollectionBase._KeysEnumerator(this);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			int count = this.Count;
			string[] array = new string[count];
			object[] array2 = new object[count];
			int num = 0;
			foreach (object obj in this.m_ItemsArray)
			{
				NameObjectCollectionBase._Item item = (NameObjectCollectionBase._Item)obj;
				array[num] = item.key;
				array2[num] = item.value;
				num++;
			}
			if (this.equality_comparer != null)
			{
				info.AddValue("KeyComparer", this.equality_comparer, typeof(IEqualityComparer));
				info.AddValue("Version", 4, typeof(int));
			}
			else
			{
				info.AddValue("HashProvider", this.m_hashprovider, typeof(IHashCodeProvider));
				info.AddValue("Comparer", this.m_comparer, typeof(IComparer));
				info.AddValue("Version", 2, typeof(int));
			}
			info.AddValue("ReadOnly", this.m_readonly);
			info.AddValue("Count", count);
			info.AddValue("Keys", array, typeof(string[]));
			info.AddValue("Values", array2, typeof(object[]));
		}

		public virtual int Count
		{
			get
			{
				return this.m_ItemsArray.Count;
			}
		}

		public virtual void OnDeserialization(object sender)
		{
			SerializationInfo serializationInfo = this.infoCopy;
			if (serializationInfo == null)
			{
				return;
			}
			this.infoCopy = null;
			this.m_hashprovider = (IHashCodeProvider)serializationInfo.GetValue("HashProvider", typeof(IHashCodeProvider));
			if (this.m_hashprovider == null)
			{
				this.equality_comparer = (IEqualityComparer)serializationInfo.GetValue("KeyComparer", typeof(IEqualityComparer));
			}
			else
			{
				this.m_comparer = (IComparer)serializationInfo.GetValue("Comparer", typeof(IComparer));
				if (this.m_comparer == null)
				{
					throw new SerializationException("The comparer is null");
				}
			}
			this.m_readonly = serializationInfo.GetBoolean("ReadOnly");
			string[] array = (string[])serializationInfo.GetValue("Keys", typeof(string[]));
			if (array == null)
			{
				throw new SerializationException("keys is null");
			}
			object[] array2 = (object[])serializationInfo.GetValue("Values", typeof(object[]));
			if (array2 == null)
			{
				throw new SerializationException("values is null");
			}
			this.Init();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				this.BaseAdd(array[i], array2[i]);
			}
		}

		protected bool IsReadOnly
		{
			get
			{
				return this.m_readonly;
			}
			set
			{
				this.m_readonly = value;
			}
		}

		protected void BaseAdd(string name, object value)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			NameObjectCollectionBase._Item item = new NameObjectCollectionBase._Item(name, value);
			if (name == null)
			{
				if (this.m_NullKeyItem == null)
				{
					this.m_NullKeyItem = item;
				}
			}
			else if (this.m_ItemsContainer[name] == null)
			{
				this.m_ItemsContainer.Add(name, item);
			}
			this.m_ItemsArray.Add(item);
		}

		protected void BaseClear()
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			this.Init();
		}

		protected object BaseGet(int index)
		{
			return ((NameObjectCollectionBase._Item)this.m_ItemsArray[index]).value;
		}

		protected object BaseGet(string name)
		{
			NameObjectCollectionBase._Item item = this.FindFirstMatchedItem(name);
			if (item == null)
			{
				return null;
			}
			return item.value;
		}

		protected string[] BaseGetAllKeys()
		{
			int count = this.m_ItemsArray.Count;
			string[] array = new string[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = this.BaseGetKey(i);
			}
			return array;
		}

		protected object[] BaseGetAllValues()
		{
			int count = this.m_ItemsArray.Count;
			object[] array = new object[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = this.BaseGet(i);
			}
			return array;
		}

		protected object[] BaseGetAllValues(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("'type' argument can't be null");
			}
			int count = this.m_ItemsArray.Count;
			object[] array = (object[])Array.CreateInstance(type, count);
			for (int i = 0; i < count; i++)
			{
				array[i] = this.BaseGet(i);
			}
			return array;
		}

		protected string BaseGetKey(int index)
		{
			return ((NameObjectCollectionBase._Item)this.m_ItemsArray[index]).key;
		}

		protected bool BaseHasKeys()
		{
			return this.m_ItemsContainer.Count > 0;
		}

		protected void BaseRemove(string name)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			if (name != null)
			{
				this.m_ItemsContainer.Remove(name);
			}
			else
			{
				this.m_NullKeyItem = null;
			}
			int num = this.m_ItemsArray.Count;
			int i = 0;
			while (i < num)
			{
				string text = this.BaseGetKey(i);
				if (this.Equals(text, name))
				{
					this.m_ItemsArray.RemoveAt(i);
					num--;
				}
				else
				{
					i++;
				}
			}
		}

		protected void BaseRemoveAt(int index)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			string text = this.BaseGetKey(index);
			if (text != null)
			{
				this.m_ItemsContainer.Remove(text);
			}
			else
			{
				this.m_NullKeyItem = null;
			}
			this.m_ItemsArray.RemoveAt(index);
		}

		protected void BaseSet(int index, object value)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			NameObjectCollectionBase._Item item = (NameObjectCollectionBase._Item)this.m_ItemsArray[index];
			item.value = value;
		}

		protected void BaseSet(string name, object value)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			NameObjectCollectionBase._Item item = this.FindFirstMatchedItem(name);
			if (item != null)
			{
				item.value = value;
			}
			else
			{
				this.BaseAdd(name, value);
			}
		}

		[global::System.MonoTODO]
		private NameObjectCollectionBase._Item FindFirstMatchedItem(string name)
		{
			if (name != null)
			{
				return (NameObjectCollectionBase._Item)this.m_ItemsContainer[name];
			}
			return this.m_NullKeyItem;
		}

		internal bool Equals(string s1, string s2)
		{
			if (this.m_comparer != null)
			{
				return this.m_comparer.Compare(s1, s2) == 0;
			}
			return this.equality_comparer.Equals(s1, s2);
		}

		private Hashtable m_ItemsContainer;

		private NameObjectCollectionBase._Item m_NullKeyItem;

		private ArrayList m_ItemsArray;

		private IHashCodeProvider m_hashprovider;

		private IComparer m_comparer;

		private int m_defCapacity;

		private bool m_readonly;

		private SerializationInfo infoCopy;

		private NameObjectCollectionBase.KeysCollection keyscoll;

		private IEqualityComparer equality_comparer;

		internal class _Item
		{
			public _Item(string key, object value)
			{
				this.key = key;
				this.value = value;
			}

			public string key;

			public object value;
		}

		[Serializable]
		internal class _KeysEnumerator : IEnumerator
		{
			internal _KeysEnumerator(NameObjectCollectionBase collection)
			{
				this.m_collection = collection;
				this.Reset();
			}

			public object Current
			{
				get
				{
					if (this.m_position < this.m_collection.Count || this.m_position < 0)
					{
						return this.m_collection.BaseGetKey(this.m_position);
					}
					throw new InvalidOperationException();
				}
			}

			public bool MoveNext()
			{
				return ++this.m_position < this.m_collection.Count;
			}

			public void Reset()
			{
				this.m_position = -1;
			}

			private NameObjectCollectionBase m_collection;

			private int m_position;
		}

		[Serializable]
		public class KeysCollection : ICollection, IEnumerable
		{
			internal KeysCollection(NameObjectCollectionBase collection)
			{
				this.m_collection = collection;
			}

			void ICollection.CopyTo(Array array, int arrayIndex)
			{
				ArrayList itemsArray = this.m_collection.m_ItemsArray;
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}
				if (array.Length > 0 && arrayIndex >= array.Length)
				{
					throw new ArgumentException("arrayIndex is equal to or greater than array.Length");
				}
				if (arrayIndex + itemsArray.Count > array.Length)
				{
					throw new ArgumentException("Not enough room from arrayIndex to end of array for this KeysCollection");
				}
				if (array != null && array.Rank > 1)
				{
					throw new ArgumentException("array is multidimensional");
				}
				object[] array2 = (object[])array;
				int i = 0;
				while (i < itemsArray.Count)
				{
					array2[arrayIndex] = ((NameObjectCollectionBase._Item)itemsArray[i]).key;
					i++;
					arrayIndex++;
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
					return this.m_collection;
				}
			}

			public virtual string Get(int index)
			{
				return this.m_collection.BaseGetKey(index);
			}

			public int Count
			{
				get
				{
					return this.m_collection.Count;
				}
			}

			public string this[int index]
			{
				get
				{
					return this.Get(index);
				}
			}

			public IEnumerator GetEnumerator()
			{
				return new NameObjectCollectionBase._KeysEnumerator(this.m_collection);
			}

			private NameObjectCollectionBase m_collection;
		}
	}
}
