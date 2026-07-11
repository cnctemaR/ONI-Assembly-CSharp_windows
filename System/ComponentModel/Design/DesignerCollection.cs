using System;
using System.Collections;

namespace System.ComponentModel.Design
{
	public class DesignerCollection : ICollection, IEnumerable
	{
		public DesignerCollection(IDesignerHost[] designers)
		{
			this.designers = new ArrayList(designers);
		}

		public DesignerCollection(IList designers)
		{
			this.designers = new ArrayList(designers);
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.designers.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.designers.SyncRoot;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.designers.CopyTo(array, index);
		}

		public int Count
		{
			get
			{
				return this.designers.Count;
			}
		}

		public virtual IDesignerHost this[int index]
		{
			get
			{
				return (IDesignerHost)this.designers[index];
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.designers.GetEnumerator();
		}

		private ArrayList designers;
	}
}
