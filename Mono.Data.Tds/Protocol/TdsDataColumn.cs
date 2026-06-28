using System;
using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public class TdsDataColumn
	{
		public TdsDataColumn()
		{
			this.IsAutoIncrement = new bool?(false);
			this.IsIdentity = new bool?(false);
			this.IsRowVersion = new bool?(false);
			this.IsUnique = new bool?(false);
			this.IsHidden = new bool?(false);
		}

		public TdsColumnType? ColumnType { get; set; }

		public string ColumnName { get; set; }

		public int? ColumnSize { get; set; }

		public int? ColumnOrdinal { get; set; }

		public bool? IsAutoIncrement { get; set; }

		public bool? IsIdentity { get; set; }

		public bool? IsRowVersion { get; set; }

		public bool? IsUnique { get; set; }

		public bool? IsHidden { get; set; }

		public bool? IsKey { get; set; }

		public bool? IsAliased { get; set; }

		public bool? IsExpression { get; set; }

		public bool? IsReadOnly { get; set; }

		public short? NumericPrecision { get; set; }

		public short? NumericScale { get; set; }

		public string BaseServerName { get; set; }

		public string BaseCatalogName { get; set; }

		public string BaseColumnName { get; set; }

		public string BaseSchemaName { get; set; }

		public string BaseTableName { get; set; }

		public bool? AllowDBNull { get; set; }

		public int? LCID { get; set; }

		public int? SortOrder { get; set; }

		public string DataTypeName { get; set; }

		public object this[string key]
		{
			get
			{
				if (this.properties == null)
				{
					return null;
				}
				return this.properties[key];
			}
			set
			{
				if (this.properties == null)
				{
					this.properties = new Hashtable();
				}
				this.properties[key] = value;
			}
		}

		private Hashtable properties;
	}
}
