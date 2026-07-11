using System;

namespace System.Collections
{
	[Serializable]
	internal sealed class EmptyReadOnlyDictionaryInternal : IDictionary, ICollection, IEnumerable
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new EmptyReadOnlyDictionaryInternal.NodeEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException(Environment.GetResourceString("Only single dimensional arrays are supported for the requested action."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Non-negative number required."));
			}
			if (array.Length - index < this.Count)
			{
				throw new ArgumentException(Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."), "index");
			}
		}

		public int Count
		{
			get
			{
				return 0;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object this[object key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key", Environment.GetResourceString("Key cannot be null."));
				}
				return null;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key", Environment.GetResourceString("Key cannot be null."));
				}
				if (!key.GetType().IsSerializable)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument passed in is not serializable."), "key");
				}
				if (value != null && !value.GetType().IsSerializable)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument passed in is not serializable."), "value");
				}
				throw new InvalidOperationException(Environment.GetResourceString("Instance is read-only."));
			}
		}

		public ICollection Keys
		{
			get
			{
				return EmptyArray<object>.Value;
			}
		}

		public ICollection Values
		{
			get
			{
				return EmptyArray<object>.Value;
			}
		}

		public bool Contains(object key)
		{
			return false;
		}

		public void Add(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", Environment.GetResourceString("Key cannot be null."));
			}
			if (!key.GetType().IsSerializable)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument passed in is not serializable."), "key");
			}
			if (value != null && !value.GetType().IsSerializable)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument passed in is not serializable."), "value");
			}
			throw new InvalidOperationException(Environment.GetResourceString("Instance is read-only."));
		}

		public void Clear()
		{
			throw new InvalidOperationException(Environment.GetResourceString("Instance is read-only."));
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new EmptyReadOnlyDictionaryInternal.NodeEnumerator();
		}

		public void Remove(object key)
		{
			throw new InvalidOperationException(Environment.GetResourceString("Instance is read-only."));
		}

		private sealed class NodeEnumerator : IDictionaryEnumerator, IEnumerator
		{
			public bool MoveNext()
			{
				return false;
			}

			public object Current
			{
				get
				{
					throw new InvalidOperationException(Environment.GetResourceString("Enumeration has either not started or has already finished."));
				}
			}

			public void Reset()
			{
			}

			public object Key
			{
				get
				{
					throw new InvalidOperationException(Environment.GetResourceString("Enumeration has either not started or has already finished."));
				}
			}

			public object Value
			{
				get
				{
					throw new InvalidOperationException(Environment.GetResourceString("Enumeration has either not started or has already finished."));
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					throw new InvalidOperationException(Environment.GetResourceString("Enumeration has either not started or has already finished."));
				}
			}
		}
	}
}
