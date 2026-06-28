using System;

namespace System.Data.Common
{
	internal class SchemaInfo
	{
		public bool AllowDBNull
		{
			get
			{
				return this.allowDBNull;
			}
			set
			{
				this.allowDBNull = value;
			}
		}

		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
			set
			{
				this.columnName = value;
			}
		}

		public int ColumnOrdinal
		{
			get
			{
				return this.ordinal;
			}
			set
			{
				this.ordinal = value;
			}
		}

		public int ColumnSize
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public string DataTypeName
		{
			get
			{
				return this.dataTypeName;
			}
			set
			{
				this.dataTypeName = value;
			}
		}

		public Type FieldType
		{
			get
			{
				return this.fieldType;
			}
			set
			{
				this.fieldType = value;
			}
		}

		public byte NumericPrecision
		{
			get
			{
				return this.precision;
			}
			set
			{
				this.precision = value;
			}
		}

		public byte NumericScale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		public string TableName
		{
			get
			{
				return this.tableName;
			}
			set
			{
				this.tableName = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
			}
		}

		private string columnName;

		private string tableName;

		private string dataTypeName;

		private bool allowDBNull;

		private bool isReadOnly;

		private int ordinal;

		private int size;

		private byte precision;

		private byte scale;

		private Type fieldType;
	}
}
