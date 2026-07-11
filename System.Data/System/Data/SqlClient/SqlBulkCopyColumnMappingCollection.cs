using System;
using System.Collections;

namespace System.Data.SqlClient
{
	public sealed class SqlBulkCopyColumnMappingCollection : CollectionBase
	{
		internal SqlBulkCopyColumnMappingCollection()
		{
		}

		public SqlBulkCopyColumnMapping this[int index]
		{
			get
			{
				throw null;
			}
		}

		public SqlBulkCopyColumnMapping Add(SqlBulkCopyColumnMapping bulkCopyColumnMapping)
		{
			throw null;
		}

		public SqlBulkCopyColumnMapping Add(int sourceColumnIndex, int destinationColumnIndex)
		{
			throw null;
		}

		public SqlBulkCopyColumnMapping Add(int sourceColumnIndex, string destinationColumn)
		{
			throw null;
		}

		public SqlBulkCopyColumnMapping Add(string sourceColumn, int destinationColumnIndex)
		{
			throw null;
		}

		public SqlBulkCopyColumnMapping Add(string sourceColumn, string destinationColumn)
		{
			throw null;
		}

		public new void Clear()
		{
		}

		public bool Contains(SqlBulkCopyColumnMapping value)
		{
			throw null;
		}

		public void CopyTo(SqlBulkCopyColumnMapping[] array, int index)
		{
		}

		public int IndexOf(SqlBulkCopyColumnMapping value)
		{
			throw null;
		}

		public void Insert(int index, SqlBulkCopyColumnMapping value)
		{
		}

		public void Remove(SqlBulkCopyColumnMapping value)
		{
		}

		public new void RemoveAt(int index)
		{
		}
	}
}
