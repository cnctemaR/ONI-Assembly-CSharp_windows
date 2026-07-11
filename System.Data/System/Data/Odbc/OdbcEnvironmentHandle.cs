using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	internal sealed class OdbcEnvironmentHandle : OdbcHandle
	{
		internal OdbcEnvironmentHandle()
			: base(ODBC32.SQL_HANDLE.ENV, null)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLSetEnvAttr(this, ODBC32.SQL_ATTR.ODBC_VERSION, ODBC32.SQL_OV_ODBC3, ODBC32.SQL_IS.INTEGER);
			retCode = UnsafeNativeMethods.SQLSetEnvAttr(this, ODBC32.SQL_ATTR.CONNECTION_POOLING, ODBC32.SQL_CP_ONE_PER_HENV, ODBC32.SQL_IS.INTEGER);
			if (retCode > ODBC32.RetCode.SUCCESS_WITH_INFO)
			{
				base.Dispose();
				throw ODBC.CantEnableConnectionpooling(retCode);
			}
		}
	}
}
