using System;

namespace System.Data.OleDb
{
	internal enum GdaTransactionIsolation
	{
		Unknown,
		ReadCommitted,
		ReadUncommitted,
		RepeatableRead,
		Serializable
	}
}
