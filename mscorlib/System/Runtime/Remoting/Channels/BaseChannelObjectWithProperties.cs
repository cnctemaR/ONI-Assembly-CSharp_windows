using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;

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
			[SecuritySafeCritical]
			get
			{
				return this.table.Count;
			}
		}

		public virtual bool IsFixedSize
		{
			[SecuritySafeCritical]
			get
			{
				return true;
			}
		}

		public virtual bool IsReadOnly
		{
			[SecuritySafeCritical]
			get
			{
				return false;
			}
		}

		public virtual bool IsSynchronized
		{
			[SecuritySafeCritical]
			get
			{
				return false;
			}
		}

		public virtual object this[object key]
		{
			[SecuritySafeCritical]
			get
			{
				throw new NotImplementedException();
			}
			[SecuritySafeCritical]
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual ICollection Keys
		{
			[SecuritySafeCritical]
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
			[SecuritySafeCritical]
			get
			{
				return this;
			}
		}

		public virtual ICollection Values
		{
			[SecuritySafeCritical]
			get
			{
				return this.table.Values;
			}
		}

		[SecuritySafeCritical]
		public virtual void Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		[SecuritySafeCritical]
		public virtual void Clear()
		{
			throw new NotSupportedException();
		}

		[SecuritySafeCritical]
		public virtual bool Contains(object key)
		{
			return this.table.Contains(key);
		}

		[SecuritySafeCritical]
		public virtual void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		[SecuritySafeCritical]
		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return this.table.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.table.GetEnumerator();
		}

		[SecuritySafeCritical]
		public virtual void Remove(object key)
		{
			throw new NotSupportedException();
		}

		private Hashtable table;
	}
}
