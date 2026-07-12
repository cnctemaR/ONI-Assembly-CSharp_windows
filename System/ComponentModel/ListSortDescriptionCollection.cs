using System;
using System.Collections;

namespace System.ComponentModel
{
	public class ListSortDescriptionCollection : IList, ICollection, IEnumerable
	{
		public ListSortDescriptionCollection()
		{
		}

		public ListSortDescriptionCollection(ListSortDescription[] sorts)
		{
			if (sorts != null)
			{
				for (int i = 0; i < sorts.Length; i++)
				{
					this._sorts.Add(sorts[i]);
				}
			}
		}

		public ListSortDescription this[int index]
		{
			get
			{
				return (ListSortDescription)this._sorts[index];
			}
			set
			{
				throw new InvalidOperationException("Once a ListSortDescriptionCollection has been created it can't be modified.");
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new InvalidOperationException("Once a ListSortDescriptionCollection has been created it can't be modified.");
			}
		}

		int IList.Add(object value)
		{
			throw new InvalidOperationException("Once a ListSortDescriptionCollection has been created it can't be modified.");
		}

		void IList.Clear()
		{
			throw new InvalidOperationException("Once a ListSortDescriptionCollection has been created it can't be modified.");
		}

		public bool Contains(object value)
		{
			return ((IList)this._sorts).Contains(value);
		}

		public int IndexOf(object value)
		{
			return ((IList)this._sorts).IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			throw new InvalidOperationException("Once a ListSortDescriptionCollection has been created it can't be modified.");
		}

		void IList.Remove(object value)
		{
			throw new InvalidOperationException("Once a ListSortDescriptionCollection has been created it can't be modified.");
		}

		void IList.RemoveAt(int index)
		{
			throw new InvalidOperationException("Once a ListSortDescriptionCollection has been created it can't be modified.");
		}

		public int Count
		{
			get
			{
				return this._sorts.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this._sorts.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._sorts.GetEnumerator();
		}

		private ArrayList _sorts = new ArrayList();
	}
}
