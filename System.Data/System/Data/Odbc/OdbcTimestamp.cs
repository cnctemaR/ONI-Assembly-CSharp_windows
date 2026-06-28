using System;

namespace System.Data.Odbc
{
	internal struct OdbcTimestamp
	{
		internal short year;

		internal ushort month;

		internal ushort day;

		internal ushort hour;

		internal ushort minute;

		internal ushort second;

		internal ulong fraction;
	}
}
