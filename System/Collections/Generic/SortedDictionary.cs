using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(IDictionaryDebugView<, >))]
	[Serializable]
	public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		public SortedDictionary()
			: this(null)
		{
		}

		public SortedDictionary(IDictionary<TKey, TValue> dictionary)
			: this(dictionary, null)
		{
		}

		public SortedDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this._set = new TreeSet<KeyValuePair<TKey, TValue>>(new SortedDictionary<TKey, TValue>.KeyValuePairComparer(comparer));
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
			{
				this._set.Add(keyValuePair);
			}
		}

		public SortedDictionary(IComparer<TKey> comparer)
		{
			this._set = new TreeSet<KeyValuePair<TKey, TValue>>(new SortedDictionary<TKey, TValue>.KeyValuePairComparer(comparer));
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			this._set.Add(keyValuePair);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(keyValuePair);
			if (node == null)
			{
				return false;
			}
			if (keyValuePair.Value == null)
			{
				return node.Item.Value == null;
			}
			return EqualityComparer<TValue>.Default.Equals(node.Item.Value, keyValuePair.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(keyValuePair);
			if (node == null)
			{
				return false;
			}
			if (EqualityComparer<TValue>.Default.Equals(node.Item.Value, keyValuePair.Value))
			{
				this._set.Remove(keyValuePair);
				return true;
			}
			return false;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return false;
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
				SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
				if (node == null)
				{
					throw new KeyNotFoundException();
				}
				return node.Item.Value;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
				if (node == null)
				{
					this._set.Add(new KeyValuePair<TKey, TValue>(key, value));
					return;
				}
				node.Item = new KeyValuePair<TKey, TValue>(node.Item.Key, value);
				this._set.UpdateVersion();
			}
		}

		public int Count
		{
			get
			{
				return this._set.Count;
			}
		}

		public IComparer<TKey> Comparer
		{
			get
			{
				return ((SortedDictionary<TKey, TValue>.KeyValuePairComparer)this._set.Comparer).keyComparer;
			}
		}

		public SortedDictionary<TKey, TValue>.KeyCollection Keys
		{
			get
			{
				if (this._keys == null)
				{
					this._keys = new SortedDictionary<TKey, TValue>.KeyCollection(this);
				}
				return this._keys;
			}
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		public SortedDictionary<TKey, TValue>.ValueCollection Values
		{
			get
			{
				if (this._values == null)
				{
					this._values = new SortedDictionary<TKey, TValue>.ValueCollection(this);
				}
				return this._values;
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				return this.Values;
			}
		}

		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
		{
			get
			{
				return this.Values;
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this._set.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		public void Clear()
		{
			this._set.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this._set.Contains(new KeyValuePair<TKey, TValue>(key, default(TValue)));
		}

		public bool ContainsValue(TValue value)
		{
			bool found = false;
			if (value == null)
			{
				this._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
				{
					if (node.Item.Value == null)
					{
						found = true;
						return false;
					}
					return true;
				});
			}
			else
			{
				EqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;
				this._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
				{
					if (valueComparer.Equals(node.Item.Value, value))
					{
						found = true;
						return false;
					}
					return true;
				});
			}
			return found;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			this._set.CopyTo(array, index);
		}

		public SortedDictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this, 1);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this, 1);
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this._set.Remove(new KeyValuePair<TKey, TValue>(key, default(TValue)));
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
			if (node == null)
			{
				value = default(TValue);
				return false;
			}
			value = node.Item.Value;
			return true;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this._set).CopyTo(array, index);
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

		object IDictionary.this[object key]
		{
			get
			{
				TValue tvalue;
				if (SortedDictionary<TKey, TValue>.IsCompatibleKey(key) && this.TryGetValue((TKey)((object)key), out tvalue))
				{
					return tvalue;
				}
				return null;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (value == null && default(TValue) != null)
				{
					throw new ArgumentNullException("value");
				}
				try
				{
					TKey tkey = (TKey)((object)key);
					try
					{
						this[tkey] = (TValue)((object)value);
					}
					catch (InvalidCastException)
					{
						throw new ArgumentException(global::SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.", value, typeof(TValue)), "value");
					}
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(global::SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.", key, typeof(TKey)), "key");
				}
			}
		}

		void IDictionary.Add(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (value == null && default(TValue) != null)
			{
				throw new ArgumentNullException("value");
			}
			try
			{
				TKey tkey = (TKey)((object)key);
				try
				{
					this.Add(tkey, (TValue)((object)value));
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(global::SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.", value, typeof(TValue)), "value");
				}
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(global::SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.", key, typeof(TKey)), "key");
			}
		}

		bool IDictionary.Contains(object key)
		{
			return SortedDictionary<TKey, TValue>.IsCompatibleKey(key) && this.ContainsKey((TKey)((object)key));
		}

		private static bool IsCompatibleKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return key is TKey;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this, 2);
		}

		void IDictionary.Remove(object key)
		{
			if (SortedDictionary<TKey, TValue>.IsCompatibleKey(key))
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
				return ((ICollection)this._set).SyncRoot;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this, 1);
		}

		[NonSerialized]
		private SortedDictionary<TKey, TValue>.KeyCollection _keys;

		[NonSerialized]
		private SortedDictionary<TKey, TValue>.ValueCollection _values;

		private TreeSet<KeyValuePair<TKey, TValue>> _set;

		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator, IDictionaryEnumerator
		{
			internal Enumerator(SortedDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
			{
				this._treeEnum = dictionary._set.GetEnumerator();
				this._getEnumeratorRetType = getEnumeratorRetType;
			}

			public bool MoveNext()
			{
				return this._treeEnum.MoveNext();
			}

			public void Dispose()
			{
				this._treeEnum.Dispose();
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this._treeEnum.Current;
				}
			}

			internal bool NotStartedOrEnded
			{
				get
				{
					return this._treeEnum.NotStartedOrEnded;
				}
			}

			internal void Reset()
			{
				this._treeEnum.Reset();
			}

			void IEnumerator.Reset()
			{
				this._treeEnum.Reset();
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.NotStartedOrEnded)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					KeyValuePair<TKey, TValue> keyValuePair;
					if (this._getEnumeratorRetType == 2)
					{
						keyValuePair = this.Current;
						object obj = keyValuePair.Key;
						keyValuePair = this.Current;
						return new DictionaryEntry(obj, keyValuePair.Value);
					}
					keyValuePair = this.Current;
					TKey key = keyValuePair.Key;
					keyValuePair = this.Current;
					return new KeyValuePair<TKey, TValue>(key, keyValuePair.Value);
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					if (this.NotStartedOrEnded)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					KeyValuePair<TKey, TValue> keyValuePair = this.Current;
					return keyValuePair.Key;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					if (this.NotStartedOrEnded)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					KeyValuePair<TKey, TValue> keyValuePair = this.Current;
					return keyValuePair.Value;
				}
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					if (this.NotStartedOrEnded)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					KeyValuePair<TKey, TValue> keyValuePair = this.Current;
					object obj = keyValuePair.Key;
					keyValuePair = this.Current;
					return new DictionaryEntry(obj, keyValuePair.Value);
				}
			}

			private SortedSet<KeyValuePair<TKey, TValue>>.Enumerator _treeEnum;

			private int _getEnumeratorRetType;

			internal const int KeyValuePair = 1;

			internal const int DictEntry = 2;
		}

		[DebuggerDisplay("Count = {Count}")]
		[DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<, >))]
		[Serializable]
		public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection, IReadOnlyCollection<TKey>
		{
			public KeyCollection(SortedDictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this._dictionary = dictionary;
			}

			public SortedDictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dictionary);
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dictionary);
			}

			public void CopyTo(TKey[] array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				if (array.Length - index < this.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				this._dictionary._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
				{
					TKey[] array2 = array;
					int index2 = index;
					index = index2 + 1;
					array2[index2] = node.Item.Key;
					return true;
				});
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
				}
				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException("The lower bound of target array must be zero.", "array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				if (array.Length - index < this._dictionary.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				TKey[] array2 = array as TKey[];
				if (array2 != null)
				{
					this.CopyTo(array2, index);
					return;
				}
				try
				{
					object[] objects = (object[])array;
					this._dictionary._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
					{
						object[] objects2 = objects;
						int index2 = index;
						index = index2 + 1;
						objects2[index2] = node.Item.Key;
						return true;
					});
				}
				catch (ArrayTypeMismatchException)
				{
					throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
				}
			}

			public int Count
			{
				get
				{
					return this._dictionary.Count;
				}
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return this._dictionary.ContainsKey(item);
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
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
					return ((ICollection)this._dictionary).SyncRoot;
				}
			}

			private SortedDictionary<TKey, TValue> _dictionary;

			public struct Enumerator : IEnumerator<TKey>, IDisposable, IEnumerator
			{
				internal Enumerator(SortedDictionary<TKey, TValue> dictionary)
				{
					this._dictEnum = dictionary.GetEnumerator();
				}

				public void Dispose()
				{
					this._dictEnum.Dispose();
				}

				public bool MoveNext()
				{
					return this._dictEnum.MoveNext();
				}

				public TKey Current
				{
					get
					{
						KeyValuePair<TKey, TValue> keyValuePair = this._dictEnum.Current;
						return keyValuePair.Key;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						if (this._dictEnum.NotStartedOrEnded)
						{
							throw new InvalidOperationException("Enumeration has either not started or has already finished.");
						}
						return this.Current;
					}
				}

				void IEnumerator.Reset()
				{
					this._dictEnum.Reset();
				}

				private SortedDictionary<TKey, TValue>.Enumerator _dictEnum;
			}
		}

		[DebuggerDisplay("Count = {Count}")]
		[DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<, >))]
		[Serializable]
		public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection, IReadOnlyCollection<TValue>
		{
			public ValueCollection(SortedDictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this._dictionary = dictionary;
			}

			public SortedDictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dictionary);
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dictionary);
			}

			public void CopyTo(TValue[] array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				if (array.Length - index < this.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				this._dictionary._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
				{
					TValue[] array2 = array;
					int index2 = index;
					index = index2 + 1;
					array2[index2] = node.Item.Value;
					return true;
				});
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
				}
				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException("The lower bound of target array must be zero.", "array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				if (array.Length - index < this._dictionary.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				TValue[] array2 = array as TValue[];
				if (array2 != null)
				{
					this.CopyTo(array2, index);
					return;
				}
				try
				{
					object[] objects = (object[])array;
					this._dictionary._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
					{
						object[] objects2 = objects;
						int index2 = index;
						index = index2 + 1;
						objects2[index2] = node.Item.Value;
						return true;
					});
				}
				catch (ArrayTypeMismatchException)
				{
					throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
				}
			}

			public int Count
			{
				get
				{
					return this._dictionary.Count;
				}
			}

			bool ICollection<TValue>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return this._dictionary.ContainsValue(item);
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
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
					return ((ICollection)this._dictionary).SyncRoot;
				}
			}

			private SortedDictionary<TKey, TValue> _dictionary;

			public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
			{
				internal Enumerator(SortedDictionary<TKey, TValue> dictionary)
				{
					this._dictEnum = dictionary.GetEnumerator();
				}

				public void Dispose()
				{
					this._dictEnum.Dispose();
				}

				public bool MoveNext()
				{
					return this._dictEnum.MoveNext();
				}

				public TValue Current
				{
					get
					{
						KeyValuePair<TKey, TValue> keyValuePair = this._dictEnum.Current;
						return keyValuePair.Value;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						if (this._dictEnum.NotStartedOrEnded)
						{
							throw new InvalidOperationException("Enumeration has either not started or has already finished.");
						}
						return this.Current;
					}
				}

				void IEnumerator.Reset()
				{
					this._dictEnum.Reset();
				}

				private SortedDictionary<TKey, TValue>.Enumerator _dictEnum;
			}
		}

		[Serializable]
		internal sealed class KeyValuePairComparer : Comparer<KeyValuePair<TKey, TValue>>
		{
			public KeyValuePairComparer(IComparer<TKey> keyComparer)
			{
				if (keyComparer == null)
				{
					this.keyComparer = Comparer<TKey>.Default;
					return;
				}
				this.keyComparer = keyComparer;
			}

			public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
			{
				return this.keyComparer.Compare(x.Key, y.Key);
			}

			internal IComparer<TKey> keyComparer;
		}
	}
}
