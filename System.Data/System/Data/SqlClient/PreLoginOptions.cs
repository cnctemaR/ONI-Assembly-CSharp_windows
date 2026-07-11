using System;

namespace System.Data.SqlClient
{
	internal enum PreLoginOptions
	{
		VERSION,
		ENCRYPT,
		INSTANCE,
		THREADID,
		MARS,
		TRACEID,
		NUMOPT,
		LASTOPT = 255
	}
}
