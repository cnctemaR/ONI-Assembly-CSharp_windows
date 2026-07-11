using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Runtime.Collections
{
	internal class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection
	{
		public OrderedDictionary()
		{
			this.privateDictionary = new OrderedDictionary();
		}

		public OrderedDictionary(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary != null)
			{
				this.privateDictionary = new OrderedDictionary();
				foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
				{
					this.privateDictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public int Count
		{
			get
			{
				return this.privateDictionary.Count;
			}
		}

		public bool IsReadOnly
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
					throw Fx.Exception.ArgumentNull("key");
				}
				if (this.privateDictionary.Contains(key))
				{
					return (TValue)((object)this.privateDictionary[key]);
				}
				throw Fx.Exception.AsError(new KeyNotFoundException("Key Not Found In Dictionary"));
			}
			set
			{
				if (key == null)
				{
					throw Fx.Exception.ArgumentNull("key");
				}
				this.privateDictionary[key] = value;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				List<TKey> list = new List<TKey>(this.privateDictionary.Count);
				foreach (object obj in this.privateDictionary.Keys)
				{
					TKey tkey = (TKey)((object)obj);
					list.Add(tkey);
				}
				return list;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				List<TValue> list = new List<TValue>(this.privateDictionary.Count);
				foreach (object obj in this.privateDictionary.Values)
				{
					TValue tvalue = (TValue)((object)obj);
					list.Add(tvalue);
				}
				return list;
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw Fx.Exception.ArgumentNull("key");
			}
			this.privateDictionary.Add(key, value);
		}

		public void Clear()
		{
			this.privateDictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return item.Key != null && this.privateDictionary.Contains(item.Key) && this.privateDictionary[item.Key].Equals(item.Value);
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw Fx.Exception.ArgumentNull("key");
			}
			return this.privateDictionary.Contains(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw Fx.Exception.ArgumentNull("array");
			}
			if (arrayIndex < 0)
			{
				throw Fx.Exception.AsError(new ArgumentOutOfRangeException("arrayIndex"));
			}
			if (array.Rank > 1 || arrayIndex >= array.Length || array.Length - arrayIndex < this.privateDictionary.Count)
			{
				throw Fx.Exception.Argument("array", "Bad Copy To Array");
			}
			int num = arrayIndex;
			foreach (object obj in this.privateDictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				array[num] = new KeyValuePair<TKey, TValue>((TKey)((object)dictionaryEntry.Key), (TValue)((object)dictionaryEntry.Value));
				num++;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (object obj in this.privateDictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				yield return new KeyValuePair<TKey, TValue>((TKey)((object)dictionaryEntry.Key), (TValue)((object)dictionaryEntry.Value));
			}
			IDictionaryEnumerator dictionaryEnumerator = null;
			yield break;
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (this.Contains(item))
			{
				this.privateDictionary.Remove(item.Key);
				return true;
			}
			return false;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw Fx.Exception.ArgumentNull("key");
			}
			if (this.privateDictionary.Contains(key))
			{
				this.privateDictionary.Remove(key);
				return true;
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw Fx.Exception.ArgumentNull("key");
			}
			bool flag = this.privateDictionary.Contains(key);
			value = (flag ? ((TValue)((object)this.privateDictionary[key])) : default(TValue));
			return flag;
		}

		void IDictionary.Add(object key, object value)
		{
			this.privateDictionary.Add(key, value);
		}

		void IDictionary.Clear()
		{
			this.privateDictionary.Clear();
		}

		bool IDictionary.Contains(object key)
		{
			return this.privateDictionary.Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return this.privateDictionary.GetEnumerator();
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return ((IDictionary)this.privateDictionary).IsFixedSize;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.privateDictionary.IsReadOnly;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return this.privateDictionary.Keys;
			}
		}

		void IDictionary.Remove(object key)
		{
			this.privateDictionary.Remove(key);
		}

		ICollection IDictionary.Values
		{
			get
			{
				return this.privateDictionary.Values;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return this.privateDictionary[key];
			}
			set
			{
				this.privateDictionary[key] = value;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.privateDictionary.CopyTo(array, index);
		}

		int ICollection.Count
		{
			get
			{
				return this.privateDictionary.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)this.privateDictionary).IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.privateDictionary).SyncRoot;
			}
		}

		private OrderedDictionary privateDictionary;
	}
}
