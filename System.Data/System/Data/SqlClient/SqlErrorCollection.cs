using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlErrorCollection : ICollection, IEnumerable
	{
		internal SqlErrorCollection()
		{
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)this._errors).CopyTo(array, index);
		}

		public void CopyTo(SqlError[] array, int index)
		{
			this._errors.CopyTo(array, index);
		}

		public int Count
		{
			get
			{
				return this._errors.Count;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public SqlError this[int index]
		{
			get
			{
				return (SqlError)this._errors[index];
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this._errors.GetEnumerator();
		}

		internal void Add(SqlError error)
		{
			this._errors.Add(error);
		}

		private readonly List<object> _errors = new List<object>();
	}
}
