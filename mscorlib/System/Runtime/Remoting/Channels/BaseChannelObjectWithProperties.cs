using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public abstract class BaseChannelObjectWithProperties : IDictionary, ICollection, IEnumerable
	{
		protected BaseChannelObjectWithProperties()
		{
			this.table = new Hashtable();
		}

		public virtual int Count
		{
			get
			{
				return this.table.Count;
			}
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual object this[object key]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				return this.table.Keys;
			}
		}

		public virtual IDictionary Properties
		{
			get
			{
				return this;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				return this.table.Values;
			}
		}

		public virtual void Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		public virtual void Clear()
		{
			throw new NotSupportedException();
		}

		public virtual bool Contains(object key)
		{
			return this.table.Contains(key);
		}

		public virtual void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return this.table.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.table.GetEnumerator();
		}

		public virtual void Remove(object key)
		{
			throw new NotSupportedException();
		}

		private Hashtable table;
	}
}
