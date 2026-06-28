using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	public class InternalDataCollectionBase : IEnumerable, ICollection
	{
		public InternalDataCollectionBase()
		{
			this.list = new ArrayList();
		}

		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return this.synchronized;
			}
		}

		protected virtual ArrayList List
		{
			get
			{
				return this.list;
			}
		}

		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public virtual void CopyTo(Array ar, int index)
		{
			this.list.CopyTo(ar, index);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		internal Array ToArray(Type type)
		{
			return this.list.ToArray(type);
		}

		private ArrayList list;

		private bool readOnly;

		private bool synchronized;
	}
}
