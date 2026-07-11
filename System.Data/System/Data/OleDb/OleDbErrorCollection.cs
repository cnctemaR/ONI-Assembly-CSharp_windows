using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.OleDb
{
	[ListBindable(false)]
	[Serializable]
	public sealed class OleDbErrorCollection : ICollection, IEnumerable
	{
		internal OleDbErrorCollection()
		{
		}

		public int Count
		{
			get
			{
				throw null;
			}
		}

		public OleDbError this[int index]
		{
			get
			{
				throw null;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw null;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw null;
			}
		}

		public void CopyTo(Array array, int index)
		{
		}

		public void CopyTo(OleDbError[] array, int index)
		{
		}

		public IEnumerator GetEnumerator()
		{
			throw null;
		}
	}
}
