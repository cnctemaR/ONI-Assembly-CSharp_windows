using System;
using System.Collections;
using System.ComponentModel;

namespace System.Data.SqlClient
{
	[ListBindable(false)]
	[Serializable]
	public sealed class SqlErrorCollection : IEnumerable, ICollection
	{
		internal SqlErrorCollection()
		{
		}

		internal SqlErrorCollection(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			this.Add(theClass, lineNumber, message, number, procedure, server, source, state);
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public SqlError this[int index]
		{
			get
			{
				return (SqlError)this.list[index];
			}
		}

		internal void Add(SqlError error)
		{
			this.list.Add(error);
		}

		internal void Add(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			SqlError sqlError = new SqlError(theClass, lineNumber, message, number, procedure, server, source, state);
			this.Add(sqlError);
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public void CopyTo(SqlError[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		private ArrayList list = new ArrayList();
	}
}
