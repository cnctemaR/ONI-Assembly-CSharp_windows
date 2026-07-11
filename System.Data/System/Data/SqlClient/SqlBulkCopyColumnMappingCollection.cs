using System;
using System.Collections;

namespace System.Data.SqlClient
{
	public sealed class SqlBulkCopyColumnMappingCollection : CollectionBase
	{
		internal SqlBulkCopyColumnMappingCollection()
		{
		}

		internal bool ReadOnly { get; set; }

		public SqlBulkCopyColumnMapping this[int index]
		{
			get
			{
				return (SqlBulkCopyColumnMapping)base.List[index];
			}
		}

		public SqlBulkCopyColumnMapping Add(SqlBulkCopyColumnMapping bulkCopyColumnMapping)
		{
			this.AssertWriteAccess();
			if ((string.IsNullOrEmpty(bulkCopyColumnMapping.SourceColumn) && bulkCopyColumnMapping.SourceOrdinal == -1) || (string.IsNullOrEmpty(bulkCopyColumnMapping.DestinationColumn) && bulkCopyColumnMapping.DestinationOrdinal == -1))
			{
				throw SQL.BulkLoadNonMatchingColumnMapping();
			}
			base.InnerList.Add(bulkCopyColumnMapping);
			return bulkCopyColumnMapping;
		}

		public SqlBulkCopyColumnMapping Add(string sourceColumn, string destinationColumn)
		{
			this.AssertWriteAccess();
			return this.Add(new SqlBulkCopyColumnMapping(sourceColumn, destinationColumn));
		}

		public SqlBulkCopyColumnMapping Add(int sourceColumnIndex, string destinationColumn)
		{
			this.AssertWriteAccess();
			return this.Add(new SqlBulkCopyColumnMapping(sourceColumnIndex, destinationColumn));
		}

		public SqlBulkCopyColumnMapping Add(string sourceColumn, int destinationColumnIndex)
		{
			this.AssertWriteAccess();
			return this.Add(new SqlBulkCopyColumnMapping(sourceColumn, destinationColumnIndex));
		}

		public SqlBulkCopyColumnMapping Add(int sourceColumnIndex, int destinationColumnIndex)
		{
			this.AssertWriteAccess();
			return this.Add(new SqlBulkCopyColumnMapping(sourceColumnIndex, destinationColumnIndex));
		}

		private void AssertWriteAccess()
		{
			if (this.ReadOnly)
			{
				throw SQL.BulkLoadMappingInaccessible();
			}
		}

		public new void Clear()
		{
			this.AssertWriteAccess();
			base.Clear();
		}

		public bool Contains(SqlBulkCopyColumnMapping value)
		{
			return base.InnerList.Contains(value);
		}

		public void CopyTo(SqlBulkCopyColumnMapping[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		internal void CreateDefaultMapping(int columnCount)
		{
			for (int i = 0; i < columnCount; i++)
			{
				base.InnerList.Add(new SqlBulkCopyColumnMapping(i, i));
			}
		}

		public int IndexOf(SqlBulkCopyColumnMapping value)
		{
			return base.InnerList.IndexOf(value);
		}

		public void Insert(int index, SqlBulkCopyColumnMapping value)
		{
			this.AssertWriteAccess();
			base.InnerList.Insert(index, value);
		}

		public void Remove(SqlBulkCopyColumnMapping value)
		{
			this.AssertWriteAccess();
			base.InnerList.Remove(value);
		}

		public new void RemoveAt(int index)
		{
			this.AssertWriteAccess();
			base.RemoveAt(index);
		}

		internal void ValidateCollection()
		{
			foreach (object obj in base.InnerList)
			{
				SqlBulkCopyColumnMapping sqlBulkCopyColumnMapping = (SqlBulkCopyColumnMapping)obj;
				SqlBulkCopyColumnMappingCollection.MappingSchema mappingSchema = ((sqlBulkCopyColumnMapping.SourceOrdinal != -1) ? ((sqlBulkCopyColumnMapping.DestinationOrdinal != -1) ? SqlBulkCopyColumnMappingCollection.MappingSchema.OrdinalsOrdinals : SqlBulkCopyColumnMappingCollection.MappingSchema.OrdinalsNames) : ((sqlBulkCopyColumnMapping.DestinationOrdinal != -1) ? SqlBulkCopyColumnMappingCollection.MappingSchema.NemesOrdinals : SqlBulkCopyColumnMappingCollection.MappingSchema.NamesNames));
				if (this._mappingSchema == SqlBulkCopyColumnMappingCollection.MappingSchema.Undefined)
				{
					this._mappingSchema = mappingSchema;
				}
				else if (this._mappingSchema != mappingSchema)
				{
					throw SQL.BulkLoadMappingsNamesOrOrdinalsOnly();
				}
			}
		}

		private SqlBulkCopyColumnMappingCollection.MappingSchema _mappingSchema;

		private enum MappingSchema
		{
			Undefined,
			NamesNames,
			NemesOrdinals,
			OrdinalsNames,
			OrdinalsOrdinals
		}
	}
}
