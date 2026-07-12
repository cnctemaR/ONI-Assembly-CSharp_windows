using System;
using System.Collections.Generic;

namespace System.Collections.Specialized
{
	internal class GenericAdapter : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		internal GenericAdapter(StringDictionary stringDictionary)
		{
			this.m_stringDictionary = stringDictionary;
		}

		public void Add(string key, string value)
		{
			this[key] = value;
		}

		public bool ContainsKey(string key)
		{
			return this.m_stringDictionary.ContainsKey(key);
		}

		public void Clear()
		{
			this.m_stringDictionary.Clear();
		}

		public int Count
		{
			get
			{
				return this.m_stringDictionary.Count;
			}
		}

		public string this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (!this.m_stringDictionary.ContainsKey(key))
				{
					throw new KeyNotFoundException();
				}
				return this.m_stringDictionary[key];
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				this.m_stringDictionary[key] = value;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				if (this._keys == null)
				{
					this._keys = new GenericAdapter.ICollectionToGenericCollectionAdapter(this.m_stringDictionary, GenericAdapter.KeyOrValue.Key);
				}
				return this._keys;
			}
		}

		public ICollection<string> Values
		{
			get
			{
				if (this._values == null)
				{
					this._values = new GenericAdapter.ICollectionToGenericCollectionAdapter(this.m_stringDictionary, GenericAdapter.KeyOrValue.Value);
				}
				return this._values;
			}
		}

		public bool Remove(string key)
		{
			if (!this.m_stringDictionary.ContainsKey(key))
			{
				return false;
			}
			this.m_stringDictionary.Remove(key);
			return true;
		}

		public bool TryGetValue(string key, out string value)
		{
			if (!this.m_stringDictionary.ContainsKey(key))
			{
				value = null;
				return false;
			}
			value = this.m_stringDictionary[key];
			return true;
		}

		void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
		{
			this.m_stringDictionary.Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
		{
			string text;
			return this.TryGetValue(item.Key, out text) && text.Equals(item.Value);
		}

		void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", SR.GetString("Array cannot be null."));
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", SR.GetString("Non-negative number required."));
			}
			if (array.Length - arrayIndex < this.Count)
			{
				throw new ArgumentException(SR.GetString("Destination array is not long enough to copy all the items in the collection. Check array index and length."));
			}
			int num = arrayIndex;
			foreach (object obj in this.m_stringDictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				array[num++] = new KeyValuePair<string, string>((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
			}
		}

		bool ICollection<KeyValuePair<string, string>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
		{
			if (!((ICollection<KeyValuePair<string, string>>)this).Contains(item))
			{
				return false;
			}
			this.m_stringDictionary.Remove(item.Key);
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			foreach (object obj in this.m_stringDictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				yield return new KeyValuePair<string, string>((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		private StringDictionary m_stringDictionary;

		private GenericAdapter.ICollectionToGenericCollectionAdapter _values;

		private GenericAdapter.ICollectionToGenericCollectionAdapter _keys;

		internal enum KeyOrValue
		{
			Key,
			Value
		}

		private class ICollectionToGenericCollectionAdapter : ICollection<string>, IEnumerable<string>, IEnumerable
		{
			public ICollectionToGenericCollectionAdapter(StringDictionary source, GenericAdapter.KeyOrValue keyOrValue)
			{
				if (source == null)
				{
					throw new ArgumentNullException("source");
				}
				this._internal = source;
				this._keyOrValue = keyOrValue;
			}

			public void Add(string item)
			{
				this.ThrowNotSupportedException();
			}

			public void Clear()
			{
				this.ThrowNotSupportedException();
			}

			public void ThrowNotSupportedException()
			{
				if (this._keyOrValue == GenericAdapter.KeyOrValue.Key)
				{
					throw new NotSupportedException(SR.GetString("Mutating a key collection derived from a dictionary is not allowed."));
				}
				throw new NotSupportedException(SR.GetString("Mutating a value collection derived from a dictionary is not allowed."));
			}

			public bool Contains(string item)
			{
				if (this._keyOrValue == GenericAdapter.KeyOrValue.Key)
				{
					return this._internal.ContainsKey(item);
				}
				return this._internal.ContainsValue(item);
			}

			public void CopyTo(string[] array, int arrayIndex)
			{
				this.GetUnderlyingCollection().CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get
				{
					return this._internal.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public bool Remove(string item)
			{
				this.ThrowNotSupportedException();
				return false;
			}

			private ICollection GetUnderlyingCollection()
			{
				if (this._keyOrValue == GenericAdapter.KeyOrValue.Key)
				{
					return this._internal.Keys;
				}
				return this._internal.Values;
			}

			public IEnumerator<string> GetEnumerator()
			{
				ICollection underlyingCollection = this.GetUnderlyingCollection();
				foreach (object obj in underlyingCollection)
				{
					string text = (string)obj;
					yield return text;
				}
				IEnumerator enumerator = null;
				yield break;
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetUnderlyingCollection().GetEnumerator();
			}

			private StringDictionary _internal;

			private GenericAdapter.KeyOrValue _keyOrValue;
		}
	}
}
