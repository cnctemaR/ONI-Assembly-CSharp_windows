using System;
using System.Collections.Generic;

namespace System.Collections.Specialized
{
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
				return (string)this.contents[key.ToLowerInvariant()];
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				this.contents[key.ToLowerInvariant()] = value;
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
			this.contents.Add(key.ToLowerInvariant(), value);
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
			return this.contents.ContainsKey(key.ToLowerInvariant());
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
			this.contents.Remove(key.ToLowerInvariant());
		}

		internal void ReplaceHashtable(Hashtable useThisHashtableInstead)
		{
			this.contents = useThisHashtableInstead;
		}

		internal IDictionary<string, string> AsGenericDictionary()
		{
			return new GenericAdapter(this);
		}

		internal Hashtable contents = new Hashtable();
	}
}
