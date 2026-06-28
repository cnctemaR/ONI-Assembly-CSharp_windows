using System;

namespace System.Data
{
	public interface IDataAdapter
	{
		int Fill(DataSet dataSet);

		DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType);

		IDataParameter[] GetFillParameters();

		int Update(DataSet dataSet);

		MissingMappingAction MissingMappingAction { get; set; }

		MissingSchemaAction MissingSchemaAction { get; set; }

		ITableMappingCollection TableMappings { get; }
	}
}
