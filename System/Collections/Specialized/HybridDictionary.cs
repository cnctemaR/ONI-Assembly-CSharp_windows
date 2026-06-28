using System;

namespace System.Collections.Specialized
{
	[Serializable]
	public class HybridDictionary : IDictionary, ICollection, IEnumerable
	{
		public HybridDictionary()
			: this(0, false)
		{
		}

		public HybridDictionary(bool caseInsensitive)
			: this(0, caseInsensitive)
		{
		}

		public HybridDictionary(int initialSize)
			: this(initialSize, false)
		{
		}

		public HybridDictionary(int initialSize, bool caseInsensitive)
		{
			this.caseInsensitive = caseInsensitive;
			IComparer comparer = ((!caseInsensitive) ? null : CaseInsensitiveComparer.DefaultInvariant);
			IHashCodeProvider hashCodeProvider = ((!caseInsensitive) ? null : CaseInsensitiveHashCodeProvider.DefaultInvariant);
			if (initialSize <= 10)
			{
				this.list = new ListDictionary(comparer);
			}
			else
			{
				this.hashtable = new Hashtable(initialSize, hashCodeProvider, comparer);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private IDictionary inner
		{
			get
			{
				IDictionary dictionary2;
				if (this.list == null)
				{
					IDictionary dictionary = this.hashtable;
					dictionary2 = dictionary;
				}
				else
				{
					dictionary2 = this.list;
				}
				return dictionary2;
			}
		}

		public int Count
		{
			get
			{
				return this.inner.Count;
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
				return this.inner[key];
			}
			set
			{
				this.inner[key] = value;
				if (this.list != null && this.Count > 10)
				{
					this.Switch();
				}
			}
		}

		public ICollection Keys
		{
			get
			{
				return this.inner.Keys;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public ICollection Values
		{
			get
			{
				return this.inner.Values;
			}
		}

		public void Add(object key, object value)
		{
			this.inner.Add(key, value);
			if (this.list != null && this.Count > 10)
			{
				this.Switch();
			}
		}

		public void Clear()
		{
			this.inner.Clear();
		}

		public bool Contains(object key)
		{
			return this.inner.Contains(key);
		}

		public void CopyTo(Array array, int index)
		{
			this.inner.CopyTo(array, index);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return this.inner.GetEnumerator();
		}

		public void Remove(object key)
		{
			this.inner.Remove(key);
		}

		private void Switch()
		{
			IComparer comparer = ((!this.caseInsensitive) ? null : CaseInsensitiveComparer.DefaultInvariant);
			IHashCodeProvider hashCodeProvider = ((!this.caseInsensitive) ? null : CaseInsensitiveHashCodeProvider.DefaultInvariant);
			this.hashtable = new Hashtable(this.list, hashCodeProvider, comparer);
			this.list.Clear();
			this.list = null;
		}

		private const int switchAfter = 10;

		private bool caseInsensitive;

		private Hashtable hashtable;

		private ListDictionary list;
	}
}
