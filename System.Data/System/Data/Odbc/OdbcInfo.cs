using System;

namespace System.Data.Odbc
{
	internal enum OdbcInfo : ushort
	{
		DataSourceName = 2,
		DriverName = 6,
		DriverVersion,
		DatabaseName = 16,
		DbmsVersion = 18,
		IdentifierQuoteChar = 29
	}
}
