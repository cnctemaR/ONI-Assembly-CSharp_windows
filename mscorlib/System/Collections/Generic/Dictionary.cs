using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Collections.Generic
{
	[DebuggerDisplay("Count={Count}")]
	[ComVisible(false)]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
	[Serializable]
	public class Dictionary<TKey, TValue> : IEnumerable, ISerializable, ICollection, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IDictionary, IDeserializationCallback
	{
		public Dictionary()
		{
			this.Init(10, null);
		}

		public Dictionary(IEqualityComparer<TKey> comparer)
		{
			this.Init(10, comparer);
		}

		public Dictionary(IDictionary<TKey, TValue> dictionary)
			: this(dictionary, null)
		{
		}

		public Dictionary(int capacity)
		{
			this.Init(capacity, null);
		}

		public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			int num = dictionary.Count;
			this.Init(num, comparer);
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public Dictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			this.Init(capacity, comparer);
		}

		protected Dictionary(SerializationInfo info, StreamingContext context)
		{
			this.serialization_info = info;
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

		ICollection IDictionary.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return this.Values;
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
				if (key is TKey && this.ContainsKey((TKey)((object)key)))
				{
					return this[this.ToTKey(key)];
				}
				return null;
			}
			set
			{
				this[this.ToTKey(key)] = this.ToTValue(value);
			}
		}

		void IDictionary.Add(object key, object value)
		{
			this.Add(this.ToTKey(key), this.ToTValue(value));
		}

		bool IDictionary.Contains(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return key is TKey && this.ContainsKey((TKey)((object)key));
		}

		void IDictionary.Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key is TKey)
			{
				this.Remove((TKey)((object)key));
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

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			this.Add(keyValuePair.Key, keyValuePair.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			return this.ContainsKeyValuePair(keyValuePair);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			this.CopyTo(array, index);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			return this.ContainsKeyValuePair(keyValuePair) && this.Remove(keyValuePair.Key);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			KeyValuePair<TKey, TValue>[] array2 = array as KeyValuePair<TKey, TValue>[];
			if (array2 != null)
			{
				this.CopyTo(array2, index);
				return;
			}
			this.CopyToCheck(array, index);
			DictionaryEntry[] array3 = array as DictionaryEntry[];
			if (array3 != null)
			{
				this.Do_CopyTo<DictionaryEntry, DictionaryEntry>(array3, index, (TKey key, TValue value) => new DictionaryEntry(key, value));
				return;
			}
			this.Do_ICollectionCopyTo<KeyValuePair<TKey, TValue>>(array, index, new Dictionary<TKey, TValue>.Transform<KeyValuePair<TKey, TValue>>(Dictionary<TKey, TValue>.make_pair));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Dictionary<TKey, TValue>.Enumerator(this);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new Dictionary<TKey, TValue>.Enumerator(this);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new Dictionary<TKey, TValue>.ShimEnumerator(this);
		}

		public int Count
		{
			get
			{
				return this.count;
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
				int num = this.hcp.GetHashCode(key) | int.MinValue;
				for (int num2 = this.table[(num & int.MaxValue) % this.table.Length] - 1; num2 != -1; num2 = this.linkSlots[num2].Next)
				{
					if (this.linkSlots[num2].HashCode == num && this.hcp.Equals(this.keySlots[num2], key))
					{
						return this.valueSlots[num2];
					}
				}
				throw new KeyNotFoundException();
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int num = this.hcp.GetHashCode(key) | int.MinValue;
				int num2 = (num & int.MaxValue) % this.table.Length;
				int num3 = this.table[num2] - 1;
				int num4 = -1;
				if (num3 != -1)
				{
					while (this.linkSlots[num3].HashCode != num || !this.hcp.Equals(this.keySlots[num3], key))
					{
						num4 = num3;
						num3 = this.linkSlots[num3].Next;
						if (num3 == -1)
						{
							break;
						}
					}
				}
				if (num3 == -1)
				{
					if (++this.count > this.threshold)
					{
						this.Resize();
						num2 = (num & int.MaxValue) % this.table.Length;
					}
					num3 = this.emptySlot;
					if (num3 == -1)
					{
						num3 = this.touchedSlots++;
					}
					else
					{
						this.emptySlot = this.linkSlots[num3].Next;
					}
					this.linkSlots[num3].Next = this.table[num2] - 1;
					this.table[num2] = num3 + 1;
					this.linkSlots[num3].HashCode = num;
					this.keySlots[num3] = key;
				}
				else if (num4 != -1)
				{
					this.linkSlots[num4].Next = this.linkSlots[num3].Next;
					this.linkSlots[num3].Next = this.table[num2] - 1;
					this.table[num2] = num3 + 1;
				}
				this.valueSlots[num3] = value;
				this.generation++;
			}
		}

		private void Init(int capacity, IEqualityComparer<TKey> hcp)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.hcp = ((hcp == null) ? EqualityComparer<TKey>.Default : hcp);
			if (capacity == 0)
			{
				capacity = 10;
			}
			capacity = (int)((float)capacity / 0.9f) + 1;
			this.InitArrays(capacity);
			this.generation = 0;
		}

		private void InitArrays(int size)
		{
			this.table = new int[size];
			this.linkSlots = new Link[size];
			this.emptySlot = -1;
			this.keySlots = new TKey[size];
			this.valueSlots = new TValue[size];
			this.touchedSlots = 0;
			this.threshold = (int)((float)this.table.Length * 0.9f);
			if (this.threshold == 0 && this.table.Length > 0)
			{
				this.threshold = 1;
			}
		}

		private void CopyToCheck(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index > array.Length)
			{
				throw new ArgumentException("index larger than largest valid index of array");
			}
			if (array.Length - index < this.Count)
			{
				throw new ArgumentException("Destination array cannot hold the requested elements!");
			}
		}

		private void Do_CopyTo<TRet, TElem>(TElem[] array, int index, Dictionary<TKey, TValue>.Transform<TRet> transform) where TRet : TElem
		{
			for (int i = 0; i < this.touchedSlots; i++)
			{
				if ((this.linkSlots[i].HashCode & -2147483648) != 0)
				{
					array[index++] = (TElem)((object)transform(this.keySlots[i], this.valueSlots[i]));
				}
			}
		}

		private static KeyValuePair<TKey, TValue> make_pair(TKey key, TValue value)
		{
			return new KeyValuePair<TKey, TValue>(key, value);
		}

		private static TKey pick_key(TKey key, TValue value)
		{
			return key;
		}

		private static TValue pick_value(TKey key, TValue value)
		{
			return value;
		}

		private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			this.CopyToCheck(array, index);
			this.Do_CopyTo<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>>(array, index, new Dictionary<TKey, TValue>.Transform<KeyValuePair<TKey, TValue>>(Dictionary<TKey, TValue>.make_pair));
		}

		private void Do_ICollectionCopyTo<TRet>(Array array, int index, Dictionary<TKey, TValue>.Transform<TRet> transform)
		{
			Type typeFromHandle = typeof(TRet);
			Type elementType = array.GetType().GetElementType();
			try
			{
				if ((typeFromHandle.IsPrimitive || elementType.IsPrimitive) && !elementType.IsAssignableFrom(typeFromHandle))
				{
					throw new Exception();
				}
				this.Do_CopyTo<TRet, object>((object[])array, index, transform);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Cannot copy source collection elements to destination array", "array", ex);
			}
		}

		private void Resize()
		{
			int num = Hashtable.ToPrime((this.table.Length << 1) | 1);
			int[] array = new int[num];
			Link[] array2 = new Link[num];
			for (int i = 0; i < this.table.Length; i++)
			{
				for (int num2 = this.table[i] - 1; num2 != -1; num2 = this.linkSlots[num2].Next)
				{
					int num3 = (array2[num2].HashCode = this.hcp.GetHashCode(this.keySlots[num2]) | int.MinValue);
					int num4 = (num3 & int.MaxValue) % num;
					array2[num2].Next = array[num4] - 1;
					array[num4] = num2 + 1;
				}
			}
			this.table = array;
			this.linkSlots = array2;
			TKey[] array3 = new TKey[num];
			TValue[] array4 = new TValue[num];
			Array.Copy(this.keySlots, 0, array3, 0, this.touchedSlots);
			Array.Copy(this.valueSlots, 0, array4, 0, this.touchedSlots);
			this.keySlots = array3;
			this.valueSlots = array4;
			this.threshold = (int)((float)num * 0.9f);
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.hcp.GetHashCode(key) | int.MinValue;
			int num2 = (num & int.MaxValue) % this.table.Length;
			int num3;
			for (num3 = this.table[num2] - 1; num3 != -1; num3 = this.linkSlots[num3].Next)
			{
				if (this.linkSlots[num3].HashCode == num && this.hcp.Equals(this.keySlots[num3], key))
				{
					throw new ArgumentException("An element with the same key already exists in the dictionary.");
				}
			}
			if (++this.count > this.threshold)
			{
				this.Resize();
				num2 = (num & int.MaxValue) % this.table.Length;
			}
			num3 = this.emptySlot;
			if (num3 == -1)
			{
				num3 = this.touchedSlots++;
			}
			else
			{
				this.emptySlot = this.linkSlots[num3].Next;
			}
			this.linkSlots[num3].HashCode = num;
			this.linkSlots[num3].Next = this.table[num2] - 1;
			this.table[num2] = num3 + 1;
			this.keySlots[num3] = key;
			this.valueSlots[num3] = value;
			this.generation++;
		}

		public IEqualityComparer<TKey> Comparer
		{
			get
			{
				return this.hcp;
			}
		}

		public void Clear()
		{
			this.count = 0;
			Array.Clear(this.table, 0, this.table.Length);
			Array.Clear(this.keySlots, 0, this.keySlots.Length);
			Array.Clear(this.valueSlots, 0, this.valueSlots.Length);
			Array.Clear(this.linkSlots, 0, this.linkSlots.Length);
			this.emptySlot = -1;
			this.touchedSlots = 0;
			this.generation++;
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.hcp.GetHashCode(key) | int.MinValue;
			for (int num2 = this.table[(num & int.MaxValue) % this.table.Length] - 1; num2 != -1; num2 = this.linkSlots[num2].Next)
			{
				if (this.linkSlots[num2].HashCode == num && this.hcp.Equals(this.keySlots[num2], key))
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsValue(TValue value)
		{
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			for (int i = 0; i < this.table.Length; i++)
			{
				for (int num = this.table[i] - 1; num != -1; num = this.linkSlots[num].Next)
				{
					if (@default.Equals(this.valueSlots[num], value))
					{
						return true;
					}
				}
			}
			return false;
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", this.generation);
			info.AddValue("Comparer", this.hcp);
			KeyValuePair<TKey, TValue>[] array = null;
			if (this.count > 0)
			{
				array = new KeyValuePair<TKey, TValue>[this.count];
				this.CopyTo(array, 0);
			}
			info.AddValue("HashSize", this.table.Length);
			info.AddValue("KeyValuePairs", array);
		}

		public virtual void OnDeserialization(object sender)
		{
			if (this.serialization_info == null)
			{
				return;
			}
			this.generation = this.serialization_info.GetInt32("Version");
			this.hcp = (IEqualityComparer<TKey>)this.serialization_info.GetValue("Comparer", typeof(IEqualityComparer<TKey>));
			int num = this.serialization_info.GetInt32("HashSize");
			KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])this.serialization_info.GetValue("KeyValuePairs", typeof(KeyValuePair<TKey, TValue>[]));
			if (num < 10)
			{
				num = 10;
			}
			this.InitArrays(num);
			this.count = 0;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					this.Add(array[i].Key, array[i].Value);
				}
			}
			this.generation++;
			this.serialization_info = null;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.hcp.GetHashCode(key) | int.MinValue;
			int num2 = (num & int.MaxValue) % this.table.Length;
			int num3 = this.table[num2] - 1;
			if (num3 == -1)
			{
				return false;
			}
			int num4 = -1;
			while (this.linkSlots[num3].HashCode != num || !this.hcp.Equals(this.keySlots[num3], key))
			{
				num4 = num3;
				num3 = this.linkSlots[num3].Next;
				if (num3 == -1)
				{
					IL_00A4:
					if (num3 == -1)
					{
						return false;
					}
					this.count--;
					if (num4 == -1)
					{
						this.table[num2] = this.linkSlots[num3].Next + 1;
					}
					else
					{
						this.linkSlots[num4].Next = this.linkSlots[num3].Next;
					}
					this.linkSlots[num3].Next = this.emptySlot;
					this.emptySlot = num3;
					this.linkSlots[num3].HashCode = 0;
					this.keySlots[num3] = default(TKey);
					this.valueSlots[num3] = default(TValue);
					this.generation++;
					return true;
				}
			}
			goto IL_00A4;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this.hcp.GetHashCode(key) | int.MinValue;
			for (int num2 = this.table[(num & int.MaxValue) % this.table.Length] - 1; num2 != -1; num2 = this.linkSlots[num2].Next)
			{
				if (this.linkSlots[num2].HashCode == num && this.hcp.Equals(this.keySlots[num2], key))
				{
					value = this.valueSlots[num2];
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		public Dictionary<TKey, TValue>.KeyCollection Keys
		{
			get
			{
				return new Dictionary<TKey, TValue>.KeyCollection(this);
			}
		}

		public Dictionary<TKey, TValue>.ValueCollection Values
		{
			get
			{
				return new Dictionary<TKey, TValue>.ValueCollection(this);
			}
		}

		private TKey ToTKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!(key is TKey))
			{
				throw new ArgumentException("not of type: " + typeof(TKey).ToString(), "key");
			}
			return (TKey)((object)key);
		}

		private TValue ToTValue(object value)
		{
			if (value == null && !typeof(TValue).IsValueType)
			{
				return default(TValue);
			}
			if (!(value is TValue))
			{
				throw new ArgumentException("not of type: " + typeof(TValue).ToString(), "value");
			}
			return (TValue)((object)value);
		}

		private bool ContainsKeyValuePair(KeyValuePair<TKey, TValue> pair)
		{
			TValue tvalue;
			return this.TryGetValue(pair.Key, out tvalue) && EqualityComparer<TValue>.Default.Equals(pair.Value, tvalue);
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new Dictionary<TKey, TValue>.Enumerator(this);
		}

		private const int INITIAL_SIZE = 10;

		private const float DEFAULT_LOAD_FACTOR = 0.9f;

		private const int NO_SLOT = -1;

		private const int HASH_FLAG = -2147483648;

		private int[] table;

		private Link[] linkSlots;

		private TKey[] keySlots;

		private TValue[] valueSlots;

		private int touchedSlots;

		private int emptySlot;

		private int count;

		private int threshold;

		private IEqualityComparer<TKey> hcp;

		private SerializationInfo serialization_info;

		private int generation;

		[Serializable]
		private class ShimEnumerator : IEnumerator, IDictionaryEnumerator
		{
			public ShimEnumerator(Dictionary<TKey, TValue> host)
			{
				this.host_enumerator = host.GetEnumerator();
			}

			public void Dispose()
			{
				this.host_enumerator.Dispose();
			}

			public bool MoveNext()
			{
				return this.host_enumerator.MoveNext();
			}

			public DictionaryEntry Entry
			{
				get
				{
					return ((IDictionaryEnumerator)this.host_enumerator).Entry;
				}
			}

			public object Key
			{
				get
				{
					KeyValuePair<TKey, TValue> keyValuePair = this.host_enumerator.Current;
					return keyValuePair.Key;
				}
			}

			public object Value
			{
				get
				{
					KeyValuePair<TKey, TValue> keyValuePair = this.host_enumerator.Current;
					return keyValuePair.Value;
				}
			}

			public object Current
			{
				get
				{
					return this.Entry;
				}
			}

			public void Reset()
			{
				this.host_enumerator.Reset();
			}

			private Dictionary<TKey, TValue>.Enumerator host_enumerator;
		}

		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
		{
			internal Enumerator(Dictionary<TKey, TValue> dictionary)
			{
				this.dictionary = dictionary;
				this.stamp = dictionary.generation;
			}

			object IEnumerator.Current
			{
				get
				{
					this.VerifyCurrent();
					return this.current;
				}
			}

			void IEnumerator.Reset()
			{
				this.Reset();
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					this.VerifyCurrent();
					return new DictionaryEntry(this.current.Key, this.current.Value);
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					return this.CurrentKey;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					return this.CurrentValue;
				}
			}

			public bool MoveNext()
			{
				this.VerifyState();
				if (this.next < 0)
				{
					return false;
				}
				while (this.next < this.dictionary.touchedSlots)
				{
					int num = this.next++;
					if ((this.dictionary.linkSlots[num].HashCode & -2147483648) != 0)
					{
						this.current = new KeyValuePair<TKey, TValue>(this.dictionary.keySlots[num], this.dictionary.valueSlots[num]);
						return true;
					}
				}
				this.next = -1;
				return false;
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this.current;
				}
			}

			internal TKey CurrentKey
			{
				get
				{
					this.VerifyCurrent();
					return this.current.Key;
				}
			}

			internal TValue CurrentValue
			{
				get
				{
					this.VerifyCurrent();
					return this.current.Value;
				}
			}

			internal void Reset()
			{
				this.VerifyState();
				this.next = 0;
			}

			private void VerifyState()
			{
				if (this.dictionary == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (this.dictionary.generation != this.stamp)
				{
					throw new InvalidOperationException("out of sync");
				}
			}

			private void VerifyCurrent()
			{
				this.VerifyState();
				if (this.next <= 0)
				{
					throw new InvalidOperationException("Current is not valid");
				}
			}

			public void Dispose()
			{
				this.dictionary = null;
			}

			private Dictionary<TKey, TValue> dictionary;

			private int next;

			private int stamp;

			internal KeyValuePair<TKey, TValue> current;
		}

		[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
		[DebuggerDisplay("Count={Count}")]
		[Serializable]
		public sealed class KeyCollection : IEnumerable, ICollection, ICollection<TKey>, IEnumerable<TKey>
		{
			public KeyCollection(Dictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this.dictionary = dictionary;
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return this.dictionary.ContainsKey(item);
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				TKey[] array2 = array as TKey[];
				if (array2 != null)
				{
					this.CopyTo(array2, index);
					return;
				}
				this.dictionary.CopyToCheck(array, index);
				this.dictionary.Do_ICollectionCopyTo<TKey>(array, index, new Dictionary<TKey, TValue>.Transform<TKey>(Dictionary<TKey, TValue>.pick_key));
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
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
					return ((ICollection)this.dictionary).SyncRoot;
				}
			}

			public void CopyTo(TKey[] array, int index)
			{
				this.dictionary.CopyToCheck(array, index);
				this.dictionary.Do_CopyTo<TKey, TKey>(array, index, new Dictionary<TKey, TValue>.Transform<TKey>(Dictionary<TKey, TValue>.pick_key));
			}

			public Dictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
			{
				return new Dictionary<TKey, TValue>.KeyCollection.Enumerator(this.dictionary);
			}

			public int Count
			{
				get
				{
					return this.dictionary.Count;
				}
			}

			private Dictionary<TKey, TValue> dictionary;

			[Serializable]
			public struct Enumerator : IEnumerator, IDisposable, IEnumerator<TKey>
			{
				internal Enumerator(Dictionary<TKey, TValue> host)
				{
					this.host_enumerator = host.GetEnumerator();
				}

				object IEnumerator.Current
				{
					get
					{
						return this.host_enumerator.CurrentKey;
					}
				}

				void IEnumerator.Reset()
				{
					this.host_enumerator.Reset();
				}

				public void Dispose()
				{
					this.host_enumerator.Dispose();
				}

				public bool MoveNext()
				{
					return this.host_enumerator.MoveNext();
				}

				public TKey Current
				{
					get
					{
						return this.host_enumerator.current.Key;
					}
				}

				private Dictionary<TKey, TValue>.Enumerator host_enumerator;
			}
		}

		[DebuggerDisplay("Count={Count}")]
		[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
		[Serializable]
		public sealed class ValueCollection : IEnumerable, ICollection, ICollection<TValue>, IEnumerable<TValue>
		{
			public ValueCollection(Dictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this.dictionary = dictionary;
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return this.dictionary.ContainsValue(item);
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				TValue[] array2 = array as TValue[];
				if (array2 != null)
				{
					this.CopyTo(array2, index);
					return;
				}
				this.dictionary.CopyToCheck(array, index);
				this.dictionary.Do_ICollectionCopyTo<TValue>(array, index, new Dictionary<TKey, TValue>.Transform<TValue>(Dictionary<TKey, TValue>.pick_value));
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			bool ICollection<TValue>.IsReadOnly
			{
				get
				{
					return true;
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
					return ((ICollection)this.dictionary).SyncRoot;
				}
			}

			public void CopyTo(TValue[] array, int index)
			{
				this.dictionary.CopyToCheck(array, index);
				this.dictionary.Do_CopyTo<TValue, TValue>(array, index, new Dictionary<TKey, TValue>.Transform<TValue>(Dictionary<TKey, TValue>.pick_value));
			}

			public Dictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
			{
				return new Dictionary<TKey, TValue>.ValueCollection.Enumerator(this.dictionary);
			}

			public int Count
			{
				get
				{
					return this.dictionary.Count;
				}
			}

			private Dictionary<TKey, TValue> dictionary;

			[Serializable]
			public struct Enumerator : IEnumerator, IDisposable, IEnumerator<TValue>
			{
				internal Enumerator(Dictionary<TKey, TValue> host)
				{
					this.host_enumerator = host.GetEnumerator();
				}

				object IEnumerator.Current
				{
					get
					{
						return this.host_enumerator.CurrentValue;
					}
				}

				void IEnumerator.Reset()
				{
					this.host_enumerator.Reset();
				}

				public void Dispose()
				{
					this.host_enumerator.Dispose();
				}

				public bool MoveNext()
				{
					return this.host_enumerator.MoveNext();
				}

				public TValue Current
				{
					get
					{
						return this.host_enumerator.current.Value;
					}
				}

				private Dictionary<TKey, TValue>.Enumerator host_enumerator;
			}
		}

		private delegate TRet Transform<TRet>(TKey key, TValue value);
	}
}
