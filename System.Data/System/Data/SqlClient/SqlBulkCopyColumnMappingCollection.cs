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
				if (index < 0 || index > base.Count)
				{
					throw new ArgumentOutOfRangeException("Index is out of range");
				}
				return (SqlBulkCopyColumnMapping)base.List[index];
			}
		}

		public SqlBulkCopyColumnMapping Add(SqlBulkCopyColumnMapping bulkCopyColumnMapping)
		{
			if (bulkCopyColumnMapping == null)
			{
				throw new ArgumentNullException("bulkCopyColumnMapping");
			}
			base.List.Add(bulkCopyColumnMapping);
			return bulkCopyColumnMapping;
		}

		public SqlBulkCopyColumnMapping Add(int sourceColumnIndex, int destinationColumnIndex)
		{
			SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = new SqlBulkCopyColumnMapping(sourceColumnIndex, destinationColumnIndex);
			return this.Add(sqlBulkCopyColumnMapping);
		}

		public SqlBulkCopyColumnMapping Add(int sourceColumnIndex, string destinationColumn)
		{
			SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = new SqlBulkCopyColumnMapping(sourceColumnIndex, destinationColumn);
			return this.Add(sqlBulkCopyColumnMapping);
		}

		public SqlBulkCopyColumnMapping Add(string sourceColumn, int destinationColumnIndex)
		{
			SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = new SqlBulkCopyColumnMapping(sourceColumn, destinationColumnIndex);
			return this.Add(sqlBulkCopyColumnMapping);
		}

		public SqlBulkCopyColumnMapping Add(string sourceColumn, string destinationColumn)
		{
			SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = new SqlBulkCopyColumnMapping(sourceColumn, destinationColumn);
			return this.Add(sqlBulkCopyColumnMapping);
		}

		public new void Clear()
		{
			base.List.Clear();
		}

		public bool Contains(SqlBulkCopyColumnMapping value)
		{
			return base.List.Contains(value);
		}

		public int IndexOf(SqlBulkCopyColumnMapping value)
		{
			return base.List.IndexOf(value);
		}

		public void CopyTo(SqlBulkCopyColumnMapping[] array, int index)
		{
			if (index < 0 || index > base.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = base.Count;
			if (num - index > array.Length)
			{
				num = array.Length;
			}
			int i = index;
			int num2 = 0;
			while (i < base.Count)
			{
				array[num2] = (SqlBulkCopyColumnMapping)base.List[i];
				i++;
				num2++;
			}
		}

		public void Insert(int index, SqlBulkCopyColumnMapping value)
		{
			if (index < 0 || index > base.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range");
			}
			base.List.Insert(index, value);
		}

		public void Remove(SqlBulkCopyColumnMapping value)
		{
			base.List.Remove(value);
		}

		public new void RemoveAt(int index)
		{
			if (index < 0 || index > base.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range");
			}
			base.RemoveAt(index);
		}
	}
}
