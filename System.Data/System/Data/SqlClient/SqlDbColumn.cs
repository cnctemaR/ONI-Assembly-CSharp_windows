using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	internal class SqlDbColumn : DbColumn
	{
		internal SqlDbColumn(_SqlMetaData md)
		{
			this._metadata = md;
			this.Populate();
		}

		private void Populate()
		{
			base.AllowDBNull = new bool?(this._metadata.isNullable);
			base.BaseCatalogName = this._metadata.catalogName;
			base.BaseColumnName = this._metadata.baseColumn;
			base.BaseSchemaName = this._metadata.schemaName;
			base.BaseServerName = this._metadata.serverName;
			base.BaseTableName = this._metadata.tableName;
			base.ColumnName = this._metadata.column;
			base.ColumnOrdinal = new int?(this._metadata.ordinal);
			base.ColumnSize = new int?((this._metadata.metaType.IsSizeInCharacters && this._metadata.length != int.MaxValue) ? (this._metadata.length / 2) : this._metadata.length);
			base.IsAutoIncrement = new bool?(this._metadata.isIdentity);
			base.IsIdentity = new bool?(this._metadata.isIdentity);
			base.IsLong = new bool?(this._metadata.metaType.IsLong);
			if (SqlDbType.Timestamp == this._metadata.type)
			{
				base.IsUnique = new bool?(true);
			}
			else
			{
				base.IsUnique = new bool?(false);
			}
			if (255 != this._metadata.precision)
			{
				base.NumericPrecision = new int?((int)this._metadata.precision);
			}
			else
			{
				base.NumericPrecision = new int?((int)this._metadata.metaType.Precision);
			}
			base.IsReadOnly = new bool?(this._metadata.updatability == 0);
			base.UdtAssemblyQualifiedName = this._metadata.udtAssemblyQualifiedName;
		}

		internal bool? SqlIsAliased
		{
			set
			{
				base.IsAliased = value;
			}
		}

		internal bool? SqlIsKey
		{
			set
			{
				base.IsKey = value;
			}
		}

		internal bool? SqlIsHidden
		{
			set
			{
				base.IsHidden = value;
			}
		}

		internal bool? SqlIsExpression
		{
			set
			{
				base.IsExpression = value;
			}
		}

		internal Type SqlDataType
		{
			set
			{
				base.DataType = value;
			}
		}

		internal string SqlDataTypeName
		{
			set
			{
				base.DataTypeName = value;
			}
		}

		internal int? SqlNumericScale
		{
			set
			{
				base.NumericScale = value;
			}
		}

		private readonly _SqlMetaData _metadata;
	}
}
