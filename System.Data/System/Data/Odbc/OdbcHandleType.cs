using System;

namespace System.Data.Odbc
{
	internal enum OdbcHandleType : short
	{
		Env = 1,
		Dbc,
		Stmt,
		Desc
	}
}
