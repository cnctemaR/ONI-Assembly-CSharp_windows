using System;
using System.Collections;

namespace System.ComponentModel.Design
{
	public class DesignerCollection : ICollection, IEnumerable
	{
		public DesignerCollection(IDesignerHost[] designers)
		{
			if (designers != null)
			{
				this._designers = new ArrayList(designers);
				return;
			}
			this._designers = new ArrayList();
		}

		public DesignerCollection(IList designers)
		{
			this._designers = designers;
		}

		public int Count
		{
			get
			{
				return this._designers.Count;
			}
		}

		public virtual IDesignerHost this[int index]
		{
			get
			{
				return (IDesignerHost)this._designers[index];
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this._designers.GetEnumerator();
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this._designers.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private IList _designers;
	}
}
