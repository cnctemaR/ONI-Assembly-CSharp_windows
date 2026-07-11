using System;
using System.ComponentModel;

namespace System.Data.Common
{
	[TypeConverter("System.Data.Common.DataColumnMapping+DataColumnMappingConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class DataColumnMapping : MarshalByRefObject, IColumnMapping, ICloneable
	{
		public DataColumnMapping()
		{
		}

		public DataColumnMapping(string sourceColumn, string dataSetColumn)
		{
		}

		[DefaultValue("")]
		public string DataSetColumn
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
		public string SourceColumn
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public DataColumn GetDataColumnBySchemaAction(DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
		{
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataColumn GetDataColumnBySchemaAction(string sourceColumn, string dataSetColumn, DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
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
