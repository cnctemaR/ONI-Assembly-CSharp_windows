using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Collections.Specialized
{
	[DesignerSerializer("System.Diagnostics.Design.StringDictionaryCodeDomSerializer, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Serializable]
	public class StringDictionary : IEnumerable
	{
		public virtual int Count
		{
			get
			{
				return this.contents.Count;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return this.contents.IsSynchronized;
			}
		}

		public virtual string this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				return (string)this.contents[key.ToLower(CultureInfo.InvariantCulture)];
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				this.contents[key.ToLower(CultureInfo.InvariantCulture)] = value;
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				return this.contents.Keys;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this.contents.SyncRoot;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				return this.contents.Values;
			}
		}

		public virtual void Add(string key, string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.contents.Add(key.ToLower(CultureInfo.InvariantCulture), value);
		}

		public virtual void Clear()
		{
			this.contents.Clear();
		}

		public virtual bool ContainsKey(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this.contents.ContainsKey(key.ToLower(CultureInfo.InvariantCulture));
		}

		public virtual bool ContainsValue(string value)
		{
			return this.contents.ContainsValue(value);
		}

		public virtual void CopyTo(Array array, int index)
		{
			this.contents.CopyTo(array, index);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.contents.GetEnumerator();
		}

		public virtual void Remove(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.contents.Remove(key.ToLower(CultureInfo.InvariantCulture));
		}

		internal void ReplaceHashtable(Hashtable useThisHashtableInstead)
		{
			this.contents = useThisHashtableInstead;
		}

		internal IDictionary<string, string> AsGenericDictionary()
		{
			return new StringDictionary.GenericAdapter(this);
		}

		internal Hashtable contents = new Hashtable();

		private class GenericAdapter : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
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
						this._keys = new StringDictionary.GenericAdapter.ICollectionToGenericCollectionAdapter(this.m_stringDictionary, StringDictionary.GenericAdapter.KeyOrValue.Key);
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
						this._values = new StringDictionary.GenericAdapter.ICollectionToGenericCollectionAdapter(this.m_stringDictionary, StringDictionary.GenericAdapter.KeyOrValue.Value);
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
					throw new ArgumentNullException("array", global::SR.GetString("Array cannot be null."));
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex", global::SR.GetString("Non-negative number required."));
				}
				if (array.Length - arrayIndex < this.Count)
				{
					throw new ArgumentException(global::SR.GetString("Destination array is not long enough to copy all the items in the collection. Check array index and length."));
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

			private StringDictionary.GenericAdapter.ICollectionToGenericCollectionAdapter _values;

			private StringDictionary.GenericAdapter.ICollectionToGenericCollectionAdapter _keys;

			internal enum KeyOrValue
			{
				Key,
				Value
			}

			private class ICollectionToGenericCollectionAdapter : ICollection<string>, IEnumerable<string>, IEnumerable
			{
				public ICollectionToGenericCollectionAdapter(StringDictionary source, StringDictionary.GenericAdapter.KeyOrValue keyOrValue)
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
					if (this._keyOrValue == StringDictionary.GenericAdapter.KeyOrValue.Key)
					{
						throw new NotSupportedException(global::SR.GetString("Mutating a key collection derived from a dictionary is not allowed."));
					}
					throw new NotSupportedException(global::SR.GetString("Mutating a value collection derived from a dictionary is not allowed."));
				}

				public bool Contains(string item)
				{
					if (this._keyOrValue == StringDictionary.GenericAdapter.KeyOrValue.Key)
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
					if (this._keyOrValue == StringDictionary.GenericAdapter.KeyOrValue.Key)
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

				private StringDictionary.GenericAdapter.KeyOrValue _keyOrValue;
			}
		}
	}
}
