using System;

namespace System.Data.Odbc
{
	internal enum OdbcIsolationLevel
	{
		ReadUncommitted = 1,
		ReadCommitted,
		RepeatableRead = 4,
		Serializable = 8,
		Snapshot = 32
	}
}
