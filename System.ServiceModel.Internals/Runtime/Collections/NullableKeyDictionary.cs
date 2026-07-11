using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Runtime.Collections
{
	internal class NullableKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		public NullableKeyDictionary()
		{
			this.innerDictionary = new Dictionary<TKey, TValue>();
		}

		public int Count
		{
			get
			{
				return this.innerDictionary.Count + (this.isNullKeyPresent ? 1 : 0);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return new NullableKeyDictionary<TKey, TValue>.NullKeyDictionaryKeyCollection<TKey, TValue>(this);
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return new NullableKeyDictionary<TKey, TValue>.NullKeyDictionaryValueCollection<TKey, TValue>(this);
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (key != null)
				{
					return this.innerDictionary[key];
				}
				if (this.isNullKeyPresent)
				{
					return this.nullKeyValue;
				}
				throw Fx.Exception.AsError(new KeyNotFoundException());
			}
			set
			{
				if (key == null)
				{
					this.isNullKeyPresent = true;
					this.nullKeyValue = value;
					return;
				}
				this.innerDictionary[key] = value;
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (key != null)
			{
				this.innerDictionary.Add(key, value);
				return;
			}
			if (this.isNullKeyPresent)
			{
				throw Fx.Exception.Argument("key", "Null Key Already Present");
			}
			this.isNullKeyPresent = true;
			this.nullKeyValue = value;
		}

		public bool ContainsKey(TKey key)
		{
			if (key != null)
			{
				return this.innerDictionary.ContainsKey(key);
			}
			return this.isNullKeyPresent;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				bool flag = this.isNullKeyPresent;
				this.isNullKeyPresent = false;
				this.nullKeyValue = default(TValue);
				return flag;
			}
			return this.innerDictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key != null)
			{
				return this.innerDictionary.TryGetValue(key, out value);
			}
			if (this.isNullKeyPresent)
			{
				value = this.nullKeyValue;
				return true;
			}
			value = default(TValue);
			return false;
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			this.isNullKeyPresent = false;
			this.nullKeyValue = default(TValue);
			this.innerDictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (item.Key != null)
			{
				return this.innerDictionary.Contains(item);
			}
			if (!this.isNullKeyPresent)
			{
				return false;
			}
			if (item.Value != null)
			{
				TValue value = item.Value;
				return value.Equals(this.nullKeyValue);
			}
			return this.nullKeyValue == null;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.innerDictionary.CopyTo(array, arrayIndex);
			if (this.isNullKeyPresent)
			{
				array[arrayIndex + this.innerDictionary.Count] = new KeyValuePair<TKey, TValue>(default(TKey), this.nullKeyValue);
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (item.Key != null)
			{
				return this.innerDictionary.Remove(item);
			}
			if (this.Contains(item))
			{
				this.isNullKeyPresent = false;
				this.nullKeyValue = default(TValue);
				return true;
			}
			return false;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this.innerDictionary)
			{
				yield return keyValuePair;
			}
			if (this.isNullKeyPresent)
			{
				yield return new KeyValuePair<TKey, TValue>(default(TKey), this.nullKeyValue);
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
		}

		private bool isNullKeyPresent;

		private TValue nullKeyValue;

		private IDictionary<TKey, TValue> innerDictionary;

		private class NullKeyDictionaryKeyCollection<TypeKey, TypeValue> : ICollection<TypeKey>, IEnumerable<TypeKey>, IEnumerable
		{
			public NullKeyDictionaryKeyCollection(NullableKeyDictionary<TypeKey, TypeValue> nullKeyDictionary)
			{
				this.nullKeyDictionary = nullKeyDictionary;
			}

			public int Count
			{
				get
				{
					int num = this.nullKeyDictionary.innerDictionary.Keys.Count;
					if (this.nullKeyDictionary.isNullKeyPresent)
					{
						num++;
					}
					return num;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public void Add(TypeKey item)
			{
				throw Fx.Exception.AsError(new NotSupportedException("Key Collection Updates Not Allowed"));
			}

			public void Clear()
			{
				throw Fx.Exception.AsError(new NotSupportedException("Key Collection Updates Not Allowed"));
			}

			public bool Contains(TypeKey item)
			{
				if (item != null)
				{
					return this.nullKeyDictionary.innerDictionary.Keys.Contains(item);
				}
				return this.nullKeyDictionary.isNullKeyPresent;
			}

			public void CopyTo(TypeKey[] array, int arrayIndex)
			{
				this.nullKeyDictionary.innerDictionary.Keys.CopyTo(array, arrayIndex);
				if (this.nullKeyDictionary.isNullKeyPresent)
				{
					array[arrayIndex + this.nullKeyDictionary.innerDictionary.Keys.Count] = default(TypeKey);
				}
			}

			public bool Remove(TypeKey item)
			{
				throw Fx.Exception.AsError(new NotSupportedException("Key Collection Updates Not Allowed"));
			}

			public IEnumerator<TypeKey> GetEnumerator()
			{
				foreach (TypeKey typeKey in this.nullKeyDictionary.innerDictionary.Keys)
				{
					yield return typeKey;
				}
				IEnumerator<TypeKey> enumerator = null;
				if (this.nullKeyDictionary.isNullKeyPresent)
				{
					TypeKey typeKey2 = default(TypeKey);
				}
				yield break;
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<TypeKey>)this).GetEnumerator();
			}

			private NullableKeyDictionary<TypeKey, TypeValue> nullKeyDictionary;
		}

		private class NullKeyDictionaryValueCollection<TypeKey, TypeValue> : ICollection<TypeValue>, IEnumerable<TypeValue>, IEnumerable
		{
			public NullKeyDictionaryValueCollection(NullableKeyDictionary<TypeKey, TypeValue> nullKeyDictionary)
			{
				this.nullKeyDictionary = nullKeyDictionary;
			}

			public int Count
			{
				get
				{
					int num = this.nullKeyDictionary.innerDictionary.Values.Count;
					if (this.nullKeyDictionary.isNullKeyPresent)
					{
						num++;
					}
					return num;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public void Add(TypeValue item)
			{
				throw Fx.Exception.AsError(new NotSupportedException("Value Collection Updates Not Allowed"));
			}

			public void Clear()
			{
				throw Fx.Exception.AsError(new NotSupportedException("Value Collection Updates Not Allowed"));
			}

			public bool Contains(TypeValue item)
			{
				return this.nullKeyDictionary.innerDictionary.Values.Contains(item) || (this.nullKeyDictionary.isNullKeyPresent && this.nullKeyDictionary.nullKeyValue.Equals(item));
			}

			public void CopyTo(TypeValue[] array, int arrayIndex)
			{
				this.nullKeyDictionary.innerDictionary.Values.CopyTo(array, arrayIndex);
				if (this.nullKeyDictionary.isNullKeyPresent)
				{
					array[arrayIndex + this.nullKeyDictionary.innerDictionary.Values.Count] = this.nullKeyDictionary.nullKeyValue;
				}
			}

			public bool Remove(TypeValue item)
			{
				throw Fx.Exception.AsError(new NotSupportedException("Value Collection Updates Not Allowed"));
			}

			public IEnumerator<TypeValue> GetEnumerator()
			{
				foreach (TypeValue typeValue in this.nullKeyDictionary.innerDictionary.Values)
				{
					yield return typeValue;
				}
				IEnumerator<TypeValue> enumerator = null;
				if (this.nullKeyDictionary.isNullKeyPresent)
				{
					yield return this.nullKeyDictionary.nullKeyValue;
				}
				yield break;
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<TypeValue>)this).GetEnumerator();
			}

			private NullableKeyDictionary<TypeKey, TypeValue> nullKeyDictionary;
		}
	}
}
