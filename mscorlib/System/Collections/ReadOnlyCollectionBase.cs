using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Serializable]
	public abstract class ReadOnlyCollectionBase : IEnumerable, ICollection
	{
		protected ReadOnlyCollectionBase()
		{
			this.list = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			ArrayList innerList = this.InnerList;
			lock (innerList)
			{
				this.InnerList.CopyTo(array, index);
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.InnerList.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.InnerList.IsSynchronized;
			}
		}

		public virtual int Count
		{
			get
			{
				return this.InnerList.Count;
			}
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.InnerList.GetEnumerator();
		}

		protected ArrayList InnerList
		{
			get
			{
				return this.list;
			}
		}

		private ArrayList list;
	}
}
