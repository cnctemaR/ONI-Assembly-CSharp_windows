using System;
using System.ComponentModel;

namespace System.Data.Common
{
	[TypeConverter("System.Data.Common.DataTableMapping+DataTableMappingConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class DataTableMapping : MarshalByRefObject, ITableMapping, ICloneable
	{
		public DataTableMapping()
		{
			this.dataSetTable = string.Empty;
			this.sourceTable = string.Empty;
			this.columnMappings = new DataColumnMappingCollection();
		}

		public DataTableMapping(string sourceTable, string dataSetTable)
			: this()
		{
			this.sourceTable = sourceTable;
			this.dataSetTable = dataSetTable;
		}

		public DataTableMapping(string sourceTable, string dataSetTable, DataColumnMapping[] columnMappings)
			: this(sourceTable, dataSetTable)
		{
			this.columnMappings.AddRange(columnMappings);
		}

		IColumnMappingCollection ITableMapping.ColumnMappings
		{
			get
			{
				return this.ColumnMappings;
			}
		}

		object ICloneable.Clone()
		{
			DataColumnMapping[] array = new DataColumnMapping[this.columnMappings.Count];
			this.columnMappings.CopyTo(array, 0);
			return new DataTableMapping(this.SourceTable, this.DataSetTable, array);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataColumnMappingCollection ColumnMappings
		{
			get
			{
				return this.columnMappings;
			}
		}

		[DefaultValue("")]
		public string DataSetTable
		{
			get
			{
				return this.dataSetTable;
			}
			set
			{
				this.dataSetTable = value;
			}
		}

		[DefaultValue("")]
		public string SourceTable
		{
			get
			{
				return this.sourceTable;
			}
			set
			{
				this.sourceTable = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DataColumnMapping GetColumnMappingBySchemaAction(string sourceColumn, MissingMappingAction mappingAction)
		{
			return DataColumnMappingCollection.GetColumnMappingBySchemaAction(this.columnMappings, sourceColumn, mappingAction);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[MonoTODO]
		public DataColumn GetDataColumn(string sourceColumn, Type dataType, DataTable dataTable, MissingMappingAction mappingAction, MissingSchemaAction schemaAction)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DataTable GetDataTableBySchemaAction(DataSet dataSet, MissingSchemaAction schemaAction)
		{
			if (dataSet.Tables.Contains(this.DataSetTable))
			{
				return dataSet.Tables[this.DataSetTable];
			}
			if (schemaAction == MissingSchemaAction.Ignore)
			{
				return null;
			}
			if (schemaAction == MissingSchemaAction.Error)
			{
				throw new InvalidOperationException(string.Format("Missing the '{0} DataTable for the '{1}' SourceTable", this.DataSetTable, this.SourceTable));
			}
			return new DataTable(this.DataSetTable);
		}

		public override string ToString()
		{
			return this.SourceTable;
		}

		private string sourceTable;

		private string dataSetTable;

		private DataColumnMappingCollection columnMappings;
	}
}
