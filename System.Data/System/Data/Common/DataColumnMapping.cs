using System;
using System.ComponentModel;

namespace System.Data.Common
{
	[TypeConverter("System.Data.Common.DataColumnMapping+DataColumnMappingConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class DataColumnMapping : MarshalByRefObject, IColumnMapping, ICloneable
	{
		public DataColumnMapping()
		{
			this.sourceColumn = string.Empty;
			this.dataSetColumn = string.Empty;
		}

		public DataColumnMapping(string sourceColumn, string dataSetColumn)
		{
			this.sourceColumn = sourceColumn;
			this.dataSetColumn = dataSetColumn;
		}

		object ICloneable.Clone()
		{
			return new DataColumnMapping(this.SourceColumn, this.DataSetColumn);
		}

		[DefaultValue("")]
		public string DataSetColumn
		{
			get
			{
				return this.dataSetColumn;
			}
			set
			{
				this.dataSetColumn = value;
			}
		}

		[DefaultValue("")]
		public string SourceColumn
		{
			get
			{
				return this.sourceColumn;
			}
			set
			{
				this.sourceColumn = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DataColumn GetDataColumnBySchemaAction(DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
		{
			if (dataTable.Columns.Contains(this.dataSetColumn))
			{
				return dataTable.Columns[this.dataSetColumn];
			}
			if (schemaAction == MissingSchemaAction.Ignore)
			{
				return null;
			}
			if (schemaAction == MissingSchemaAction.Error)
			{
				throw new InvalidOperationException(string.Format("Missing the DataColumn '{0}' in the DataTable '{1}' for the SourceColumn '{2}'", this.DataSetColumn, dataTable.TableName, this.SourceColumn));
			}
			return new DataColumn(this.dataSetColumn, dataType);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataColumn GetDataColumnBySchemaAction(string sourceColumn, string dataSetColumn, DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
		{
			if (dataTable.Columns.Contains(dataSetColumn))
			{
				return dataTable.Columns[dataSetColumn];
			}
			if (schemaAction == MissingSchemaAction.Ignore)
			{
				return null;
			}
			if (schemaAction == MissingSchemaAction.Error)
			{
				throw new InvalidOperationException(string.Format("Missing the DataColumn '{0}' in the DataTable '{1}' for the SourceColumn '{2}'", dataSetColumn, dataTable.TableName, sourceColumn));
			}
			return new DataColumn(dataSetColumn, dataType);
		}

		public override string ToString()
		{
			return this.SourceColumn;
		}

		private string sourceColumn;

		private string dataSetColumn;
	}
}
