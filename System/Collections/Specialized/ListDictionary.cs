using System;

namespace System.Collections.Specialized
{
	[Serializable]
	public class ListDictionary : IDictionary, ICollection, IEnumerable
	{
		public ListDictionary()
		{
			this.count = 0;
			this.version = 0;
			this.comparer = null;
			this.head = null;
		}

		public ListDictionary(IComparer comparer)
			: this()
		{
			this.comparer = comparer;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ListDictionary.DictionaryNodeEnumerator(this);
		}

		private ListDictionary.DictionaryNode FindEntry(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Attempted lookup for a null key.");
			}
			ListDictionary.DictionaryNode dictionaryNode = this.head;
			if (this.comparer == null)
			{
				while (dictionaryNode != null)
				{
					if (key.Equals(dictionaryNode.key))
					{
						break;
					}
					dictionaryNode = dictionaryNode.next;
				}
			}
			else
			{
				while (dictionaryNode != null)
				{
					if (this.comparer.Compare(key, dictionaryNode.key) == 0)
					{
						break;
					}
					dictionaryNode = dictionaryNode.next;
				}
			}
			return dictionaryNode;
		}

		private ListDictionary.DictionaryNode FindEntry(object key, out ListDictionary.DictionaryNode prev)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Attempted lookup for a null key.");
			}
			ListDictionary.DictionaryNode dictionaryNode = this.head;
			prev = null;
			if (this.comparer == null)
			{
				while (dictionaryNode != null)
				{
					if (key.Equals(dictionaryNode.key))
					{
						break;
					}
					prev = dictionaryNode;
					dictionaryNode = dictionaryNode.next;
				}
			}
			else
			{
				while (dictionaryNode != null)
				{
					if (this.comparer.Compare(key, dictionaryNode.key) == 0)
					{
						break;
					}
					prev = dictionaryNode;
					dictionaryNode = dictionaryNode.next;
				}
			}
			return dictionaryNode;
		}

		private void AddImpl(object key, object value, ListDictionary.DictionaryNode prev)
		{
			if (prev == null)
			{
				this.head = new ListDictionary.DictionaryNode(key, value, this.head);
			}
			else
			{
				prev.next = new ListDictionary.DictionaryNode(key, value, prev.next);
			}
			this.count++;
			this.version++;
		}

		public int Count
		{
			get
			{
				return this.count;
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
				return this;
			}
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Array cannot be null.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "index is less than 0");
			}
			if (index > array.Length)
			{
				throw new IndexOutOfRangeException("index is too large");
			}
			if (this.Count > array.Length - index)
			{
				throw new ArgumentException("Not enough room in the array");
			}
			foreach (object obj in this)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				array.SetValue(dictionaryEntry, index++);
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
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
				ListDictionary.DictionaryNode dictionaryNode = this.FindEntry(key);
				return (dictionaryNode != null) ? dictionaryNode.value : null;
			}
			set
			{
				ListDictionary.DictionaryNode dictionaryNode2;
				ListDictionary.DictionaryNode dictionaryNode = this.FindEntry(key, out dictionaryNode2);
				if (dictionaryNode != null)
				{
					dictionaryNode.value = value;
				}
				else
				{
					this.AddImpl(key, value, dictionaryNode2);
				}
			}
		}

		public ICollection Keys
		{
			get
			{
				return new ListDictionary.DictionaryNodeCollection(this, true);
			}
		}

		public ICollection Values
		{
			get
			{
				return new ListDictionary.DictionaryNodeCollection(this, false);
			}
		}

		public void Add(object key, object value)
		{
			ListDictionary.DictionaryNode dictionaryNode2;
			ListDictionary.DictionaryNode dictionaryNode = this.FindEntry(key, out dictionaryNode2);
			if (dictionaryNode != null)
			{
				throw new ArgumentException("key", "Duplicate key in add.");
			}
			this.AddImpl(key, value, dictionaryNode2);
		}

		public void Clear()
		{
			this.head = null;
			this.count = 0;
			this.version++;
		}

		public bool Contains(object key)
		{
			return this.FindEntry(key) != null;
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new ListDictionary.DictionaryNodeEnumerator(this);
		}

		public void Remove(object key)
		{
			ListDictionary.DictionaryNode dictionaryNode2;
			ListDictionary.DictionaryNode dictionaryNode = this.FindEntry(key, out dictionaryNode2);
			if (dictionaryNode == null)
			{
				return;
			}
			if (dictionaryNode2 == null)
			{
				this.head = dictionaryNode.next;
			}
			else
			{
				dictionaryNode2.next = dictionaryNode.next;
			}
			dictionaryNode.value = null;
			this.count--;
			this.version++;
		}

		private int count;

		private int version;

		private ListDictionary.DictionaryNode head;

		private IComparer comparer;

		[Serializable]
		private class DictionaryNode
		{
			public DictionaryNode(object key, object value, ListDictionary.DictionaryNode next)
			{
				this.key = key;
				this.value = value;
				this.next = next;
			}

			public object key;

			public object value;

			public ListDictionary.DictionaryNode next;
		}

		private class DictionaryNodeEnumerator : IEnumerator, IDictionaryEnumerator
		{
			public DictionaryNodeEnumerator(ListDictionary dict)
			{
				this.dict = dict;
				this.version = dict.version;
				this.Reset();
			}

			private void FailFast()
			{
				if (this.version != this.dict.version)
				{
					throw new InvalidOperationException("The ListDictionary's contents changed after this enumerator was instantiated.");
				}
			}

			public bool MoveNext()
			{
				this.FailFast();
				if (this.current == null && !this.isAtStart)
				{
					return false;
				}
				this.current = ((!this.isAtStart) ? this.current.next : this.dict.head);
				this.isAtStart = false;
				return this.current != null;
			}

			public void Reset()
			{
				this.FailFast();
				this.isAtStart = true;
				this.current = null;
			}

			public object Current
			{
				get
				{
					return this.Entry;
				}
			}

			private ListDictionary.DictionaryNode DictionaryNode
			{
				get
				{
					this.FailFast();
					if (this.current == null)
					{
						throw new InvalidOperationException("Enumerator is positioned before the collection's first element or after the last element.");
					}
					return this.current;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					object key = this.DictionaryNode.key;
					return new DictionaryEntry(key, this.current.value);
				}
			}

			public object Key
			{
				get
				{
					return this.DictionaryNode.key;
				}
			}

			public object Value
			{
				get
				{
					return this.DictionaryNode.value;
				}
			}

			private ListDictionary dict;

			private bool isAtStart;

			private ListDictionary.DictionaryNode current;

			private int version;
		}

		private class DictionaryNodeCollection : ICollection, IEnumerable
		{
			public DictionaryNodeCollection(ListDictionary dict, bool isKeyList)
			{
				this.dict = dict;
				this.isKeyList = isKeyList;
			}

			public int Count
			{
				get
				{
					return this.dict.Count;
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
					return this.dict.SyncRoot;
				}
			}

			public void CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array", "Array cannot be null.");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", "index is less than 0");
				}
				if (index > array.Length)
				{
					throw new IndexOutOfRangeException("index is too large");
				}
				if (this.Count > array.Length - index)
				{
					throw new ArgumentException("Not enough room in the array");
				}
				foreach (object obj in this)
				{
					array.SetValue(obj, index++);
				}
			}

			public IEnumerator GetEnumerator()
			{
				return new ListDictionary.DictionaryNodeCollection.DictionaryNodeCollectionEnumerator(this.dict.GetEnumerator(), this.isKeyList);
			}

			private ListDictionary dict;

			private bool isKeyList;

			private class DictionaryNodeCollectionEnumerator : IEnumerator
			{
				public DictionaryNodeCollectionEnumerator(IDictionaryEnumerator inner, bool isKeyList)
				{
					this.inner = inner;
					this.isKeyList = isKeyList;
				}

				public object Current
				{
					get
					{
						return (!this.isKeyList) ? this.inner.Value : this.inner.Key;
					}
				}

				public bool MoveNext()
				{
					return this.inner.MoveNext();
				}

				public void Reset()
				{
					this.inner.Reset();
				}

				private IDictionaryEnumerator inner;

				private bool isKeyList;
			}
		}
	}
}
