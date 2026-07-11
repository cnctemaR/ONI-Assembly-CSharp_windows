using System;

namespace System.Data.SqlClient
{
	internal sealed class _SqlMetaData : SqlMetaDataPriv
	{
		internal _SqlMetaData(int ordinal)
		{
			this.ordinal = ordinal;
		}

		internal string serverName
		{
			get
			{
				return this.multiPartTableName.ServerName;
			}
		}

		internal string catalogName
		{
			get
			{
				return this.multiPartTableName.CatalogName;
			}
		}

		internal string schemaName
		{
			get
			{
				return this.multiPartTableName.SchemaName;
			}
		}

		internal string tableName
		{
			get
			{
				return this.multiPartTableName.TableName;
			}
		}

		internal bool IsNewKatmaiDateTimeType
		{
			get
			{
				return SqlDbType.Date == this.type || SqlDbType.Time == this.type || SqlDbType.DateTime2 == this.type || SqlDbType.DateTimeOffset == this.type;
			}
		}

		internal bool IsLargeUdt
		{
			get
			{
				return this.type == SqlDbType.Udt && this.length == int.MaxValue;
			}
		}

		public object Clone()
		{
			_SqlMetaData sqlMetaData = new _SqlMetaData(this.ordinal);
			sqlMetaData.CopyFrom(this);
			sqlMetaData.column = this.column;
			sqlMetaData.baseColumn = this.baseColumn;
			sqlMetaData.multiPartTableName = this.multiPartTableName;
			sqlMetaData.updatability = this.updatability;
			sqlMetaData.tableNum = this.tableNum;
			sqlMetaData.isDifferentName = this.isDifferentName;
			sqlMetaData.isKey = this.isKey;
			sqlMetaData.isHidden = this.isHidden;
			sqlMetaData.isExpression = this.isExpression;
			sqlMetaData.isIdentity = this.isIdentity;
			sqlMetaData.isColumnSet = this.isColumnSet;
			sqlMetaData.op = this.op;
			sqlMetaData.operand = this.operand;
			return sqlMetaData;
		}

		internal string column;

		internal string baseColumn;

		internal MultiPartTableName multiPartTableName;

		internal readonly int ordinal;

		internal byte updatability;

		internal byte tableNum;

		internal bool isDifferentName;

		internal bool isKey;

		internal bool isHidden;

		internal bool isExpression;

		internal bool isIdentity;

		internal bool isColumnSet;

		internal byte op;

		internal ushort operand;
	}
}
