using System;

namespace System.Data.Odbc
{
	internal sealed class DbSchemaInfo
	{
		internal DbSchemaInfo()
		{
		}

		internal string _name;

		internal string _typename;

		internal Type _type;

		internal ODBC32.SQL_TYPE? _dbtype;
	}
}
