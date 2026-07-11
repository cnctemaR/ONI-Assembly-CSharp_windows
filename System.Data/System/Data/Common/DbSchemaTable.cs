using System;

namespace System.Data.Common
{
	internal sealed class DbSchemaTable
	{
		internal DbSchemaTable(DataTable dataTable, bool returnProviderSpecificTypes)
		{
			this._dataTable = dataTable;
			this._columns = dataTable.Columns;
			this._returnProviderSpecificTypes = returnProviderSpecificTypes;
		}

		internal DataColumn ColumnName
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.ColumnName);
			}
		}

		internal DataColumn Size
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.ColumnSize);
			}
		}

		internal DataColumn BaseServerName
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.BaseServerName);
			}
		}

		internal DataColumn BaseColumnName
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.BaseColumnName);
			}
		}

		internal DataColumn BaseTableName
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.BaseTableName);
			}
		}

		internal DataColumn BaseCatalogName
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.BaseCatalogName);
			}
		}

		internal DataColumn BaseSchemaName
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.BaseSchemaName);
			}
		}

		internal DataColumn IsAutoIncrement
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.IsAutoIncrement);
			}
		}

		internal DataColumn IsUnique
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.IsUnique);
			}
		}

		internal DataColumn IsKey
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.IsKey);
			}
		}

		internal DataColumn IsRowVersion
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.IsRowVersion);
			}
		}

		internal DataColumn AllowDBNull
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.AllowDBNull);
			}
		}

		internal DataColumn IsExpression
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.IsExpression);
			}
		}

		internal DataColumn IsHidden
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.IsHidden);
			}
		}

		internal DataColumn IsLong
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.IsLong);
			}
		}

		internal DataColumn IsReadOnly
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.IsReadOnly);
			}
		}

		internal DataColumn UnsortedIndex
		{
			get
			{
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.SchemaMappingUnsortedIndex);
			}
		}

		internal DataColumn DataType
		{
			get
			{
				if (this._returnProviderSpecificTypes)
				{
					return this.CachedDataColumn(DbSchemaTable.ColumnEnum.ProviderSpecificDataType, DbSchemaTable.ColumnEnum.DataType);
				}
				return this.CachedDataColumn(DbSchemaTable.ColumnEnum.DataType);
			}
		}

		private DataColumn CachedDataColumn(DbSchemaTable.ColumnEnum column)
		{
			return this.CachedDataColumn(column, column);
		}

		private DataColumn CachedDataColumn(DbSchemaTable.ColumnEnum column, DbSchemaTable.ColumnEnum column2)
		{
			DataColumn dataColumn = this._columnCache[(int)column];
			if (dataColumn == null)
			{
				int num = this._columns.IndexOf(DbSchemaTable.s_DBCOLUMN_NAME[(int)column]);
				if (-1 == num && column != column2)
				{
					num = this._columns.IndexOf(DbSchemaTable.s_DBCOLUMN_NAME[(int)column2]);
				}
				if (-1 != num)
				{
					dataColumn = this._columns[num];
					this._columnCache[(int)column] = dataColumn;
				}
			}
			return dataColumn;
		}

		private static readonly string[] s_DBCOLUMN_NAME = new string[]
		{
			SchemaTableColumn.ColumnName,
			SchemaTableColumn.ColumnOrdinal,
			SchemaTableColumn.ColumnSize,
			SchemaTableOptionalColumn.BaseServerName,
			SchemaTableOptionalColumn.BaseCatalogName,
			SchemaTableColumn.BaseColumnName,
			SchemaTableColumn.BaseSchemaName,
			SchemaTableColumn.BaseTableName,
			SchemaTableOptionalColumn.IsAutoIncrement,
			SchemaTableColumn.IsUnique,
			SchemaTableColumn.IsKey,
			SchemaTableOptionalColumn.IsRowVersion,
			SchemaTableColumn.DataType,
			SchemaTableOptionalColumn.ProviderSpecificDataType,
			SchemaTableColumn.AllowDBNull,
			SchemaTableColumn.ProviderType,
			SchemaTableColumn.IsExpression,
			SchemaTableOptionalColumn.IsHidden,
			SchemaTableColumn.IsLong,
			SchemaTableOptionalColumn.IsReadOnly,
			"SchemaMapping Unsorted Index"
		};

		internal DataTable _dataTable;

		private DataColumnCollection _columns;

		private DataColumn[] _columnCache = new DataColumn[DbSchemaTable.s_DBCOLUMN_NAME.Length];

		private bool _returnProviderSpecificTypes;

		private enum ColumnEnum
		{
			ColumnName,
			ColumnOrdinal,
			ColumnSize,
			BaseServerName,
			BaseCatalogName,
			BaseColumnName,
			BaseSchemaName,
			BaseTableName,
			IsAutoIncrement,
			IsUnique,
			IsKey,
			IsRowVersion,
			DataType,
			ProviderSpecificDataType,
			AllowDBNull,
			ProviderType,
			IsExpression,
			IsHidden,
			IsLong,
			IsReadOnly,
			SchemaMappingUnsortedIndex
		}
	}
}
