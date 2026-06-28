using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace YamlDotNet.Helpers
{
	internal sealed class GenericDictionaryToNonGenericAdapter : IDictionary, IEnumerable, ICollection
	{
		public GenericDictionaryToNonGenericAdapter(object genericDictionary, Type genericDictionaryType)
		{
			this.genericDictionary = genericDictionary;
			this.genericDictionaryType = genericDictionaryType;
			this.indexerSetter = genericDictionaryType.GetPublicProperty("Item").GetSetMethod();
		}

		public void Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(object key)
		{
			throw new NotSupportedException();
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new GenericDictionaryToNonGenericAdapter.DictionaryEnumerator(this.genericDictionary, this.genericDictionaryType);
		}

		public bool IsFixedSize
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public ICollection Keys
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public void Remove(object key)
		{
			throw new NotSupportedException();
		}

		public ICollection Values
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public object this[object key]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				this.indexerSetter.Invoke(this.genericDictionary, new object[] { key, value });
			}
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.genericDictionary).GetEnumerator();
		}

		private readonly object genericDictionary;

		private readonly Type genericDictionaryType;

		private readonly MethodInfo indexerSetter;

		private class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
		{
			public DictionaryEnumerator(object genericDictionary, Type genericDictionaryType)
			{
				Type[] genericArguments = genericDictionaryType.GetGenericArguments();
				Type type = typeof(KeyValuePair<, >).MakeGenericType(genericArguments);
				this.getKeyMethod = type.GetPublicProperty("Key").GetGetMethod();
				this.getValueMethod = type.GetPublicProperty("Value").GetGetMethod();
				this.enumerator = ((IEnumerable)genericDictionary).GetEnumerator();
			}

			public DictionaryEntry Entry
			{
				get
				{
					return new DictionaryEntry(this.Key, this.Value);
				}
			}

			public object Key
			{
				get
				{
					return this.getKeyMethod.Invoke(this.enumerator.Current, null);
				}
			}

			public object Value
			{
				get
				{
					return this.getValueMethod.Invoke(this.enumerator.Current, null);
				}
			}

			public object Current
			{
				get
				{
					return this.Entry;
				}
			}

			public bool MoveNext()
			{
				return this.enumerator.MoveNext();
			}

			public void Reset()
			{
				this.enumerator.Reset();
			}

			private readonly IEnumerator enumerator;

			private readonly MethodInfo getKeyMethod;

			private readonly MethodInfo getValueMethod;
		}
	}
}
