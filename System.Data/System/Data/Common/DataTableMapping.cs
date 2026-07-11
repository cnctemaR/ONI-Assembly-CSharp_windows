using System;
using System.ComponentModel;

namespace System.Data.Common
{
	[TypeConverter("System.Data.Common.DataTableMapping+DataTableMappingConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class DataTableMapping : MarshalByRefObject, ITableMapping, ICloneable
	{
		public DataTableMapping()
		{
		}

		public DataTableMapping(string sourceTable, string dataSetTable)
		{
		}

		public DataTableMapping(string sourceTable, string dataSetTable, DataColumnMapping[] columnMappings)
		{
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataColumnMappingCollection ColumnMappings
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue("")]
		public string DataSetTable
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue("")]
		public string SourceTable
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IColumnMappingCollection ITableMapping.ColumnMappings
		{
			get
			{
				throw null;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DataColumnMapping GetColumnMappingBySchemaAction(string sourceColumn, MissingMappingAction mappingAction)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[MonoTODO]
		public DataColumn GetDataColumn(string sourceColumn, Type dataType, DataTable dataTable, MissingMappingAction mappingAction, MissingSchemaAction schemaAction)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DataTable GetDataTableBySchemaAction(DataSet dataSet, MissingSchemaAction schemaAction)
		{
			throw null;
		}

		object ICloneable.Clone()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
