using System;

namespace System.Data.Odbc
{
	internal enum OdbcInputOutputDirection : short
	{
		Input = 1,
		InputOutput,
		ResultCol,
		Output,
		ReturnValue
	}
}
