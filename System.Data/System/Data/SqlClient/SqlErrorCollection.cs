using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.SqlClient
{
	[ListBindable(false)]
	[Serializable]
	public sealed class SqlErrorCollection : ICollection, IEnumerable
	{
		internal SqlErrorCollection()
		{
		}

		public int Count
		{
			get
			{
				throw null;
			}
		}

		public SqlError this[int index]
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

		public void CopyTo(SqlError[] array, int index)
		{
		}

		public IEnumerator GetEnumerator()
		{
			throw null;
		}
	}
}
