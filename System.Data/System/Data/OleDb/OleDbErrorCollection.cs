using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.OleDb
{
	[ListBindable(false)]
	[Serializable]
	public sealed class OleDbErrorCollection : IEnumerable, ICollection
	{
		internal OleDbErrorCollection()
		{
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.items.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.items.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public OleDbError this[int index]
		{
			get
			{
				return (OleDbError)this.items[index];
			}
		}

		internal void Add(OleDbError error)
		{
			this.items.Add(error);
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < array.GetLowerBound(0) || index > array.GetUpperBound(0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (array.IsFixedSize || index + this.Count > array.GetUpperBound(0))
			{
				throw new ArgumentException("array");
			}
			((OleDbError[])this.items.ToArray()).CopyTo(array, index);
		}

		public void CopyTo(OleDbError[] array, int index)
		{
			this.items.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		private ArrayList items;
	}
}
