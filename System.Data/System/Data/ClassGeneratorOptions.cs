using System;

namespace System.Data
{
	internal abstract class ClassGeneratorOptions
	{
		internal abstract string DataSetName(string source);

		internal abstract string TableTypeName(string source);

		internal abstract string TableMemberName(string source);

		internal abstract string TableColName(string source);

		internal abstract string TableDelegateName(string source);

		internal abstract string EventArgsName(string source);

		internal abstract string ColumnName(string source);

		internal abstract string RowName(string source);

		internal abstract string RelationName(string source);

		internal abstract string TableAdapterNSName(string source);

		internal abstract string TableAdapterName(string source);

		public bool MakeClassesInsideDataSet = true;
	}
}
