using System;
using System.Runtime.InteropServices;

namespace System.Data.Odbc
{
	internal class libodbc
	{
		internal static OdbcInputOutputDirection ConvertParameterDirection(ParameterDirection dir)
		{
			switch (dir)
			{
			case ParameterDirection.Input:
				return OdbcInputOutputDirection.Input;
			case ParameterDirection.Output:
				return OdbcInputOutputDirection.Output;
			case ParameterDirection.InputOutput:
				return OdbcInputOutputDirection.InputOutput;
			case ParameterDirection.ReturnValue:
				return OdbcInputOutputDirection.ReturnValue;
			}
			return OdbcInputOutputDirection.Input;
		}

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLAllocHandle(OdbcHandleType HandleType, IntPtr InputHandle, ref IntPtr OutputHandlePtr);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLSetEnvAttr(IntPtr EnvHandle, OdbcEnv Attribute, IntPtr Value, int StringLength);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLConnect(IntPtr ConnectionHandle, string ServerName, short NameLength1, string UserName, short NameLength2, string Authentication, short NameLength3);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLDriverConnect(IntPtr ConnectionHandle, IntPtr WindowHandle, string InConnectionString, short StringLength1, string OutConnectionString, short BufferLength, ref short StringLength2Ptr, ushort DriverCompletion);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLExecDirect(IntPtr StatementHandle, string StatementText, int TextLength);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLRowCount(IntPtr StatementHandle, ref int RowCount);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLNumResultCols(IntPtr StatementHandle, ref short ColumnCount);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLFetch(IntPtr StatementHandle);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetData(IntPtr StatementHandle, ushort ColumnNumber, SQL_C_TYPE TargetType, ref bool TargetPtr, int BufferLen, ref int Len);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetData(IntPtr StatementHandle, ushort ColumnNumber, SQL_C_TYPE TargetType, ref double TargetPtr, int BufferLen, ref int Len);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetData(IntPtr StatementHandle, ushort ColumnNumber, SQL_C_TYPE TargetType, ref long TargetPtr, int BufferLen, ref int Len);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetData(IntPtr StatementHandle, ushort ColumnNumber, SQL_C_TYPE TargetType, ref short TargetPtr, int BufferLen, ref int Len);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetData(IntPtr StatementHandle, ushort ColumnNumber, SQL_C_TYPE TargetType, ref float TargetPtr, int BufferLen, ref int Len);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetData(IntPtr StatementHandle, ushort ColumnNumber, SQL_C_TYPE TargetType, ref OdbcTimestamp TargetPtr, int BufferLen, ref int Len);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetData(IntPtr StatementHandle, ushort ColumnNumber, SQL_C_TYPE TargetType, ref int TargetPtr, int BufferLen, ref int Len);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetData(IntPtr StatementHandle, ushort ColumnNumber, SQL_C_TYPE TargetType, byte[] TargetPtr, int BufferLen, ref int Len);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLDescribeCol(IntPtr StatementHandle, ushort ColumnNumber, byte[] ColumnName, short BufferLength, ref short NameLength, ref short DataType, ref uint ColumnSize, ref short DecimalDigits, ref short Nullable);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLFreeHandle(ushort HandleType, IntPtr SqlHandle);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLDisconnect(IntPtr ConnectionHandle);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLPrepare(IntPtr StatementHandle, string Statement, int TextLength);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLExecute(IntPtr StatementHandle);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetConnectAttr(IntPtr ConnectionHandle, OdbcConnectionAttribute Attribute, out int value, int BufferLength, out int StringLength);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLSetConnectAttr(IntPtr ConnectionHandle, OdbcConnectionAttribute Attribute, IntPtr Value, int Length);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLEndTran(int HandleType, IntPtr Handle, short CompletionType);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLBindParameter(IntPtr StatementHandle, ushort ParamNum, short InputOutputType, SQL_C_TYPE ValueType, SQL_TYPE ParamType, uint ColSize, short DecimalDigits, IntPtr ParamValue, int BufLen, IntPtr StrLen);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLCancel(IntPtr StatementHandle);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLCloseCursor(IntPtr StatementHandle);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLError(IntPtr EnvironmentHandle, IntPtr ConnectionHandle, IntPtr StatementHandle, byte[] Sqlstate, ref int NativeError, byte[] MessageText, short BufferLength, ref short TextLength);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetStmtAttr(IntPtr StatementHandle, int Attribute, ref IntPtr Value, int BufLen, int StrLen);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLSetDescField(IntPtr DescriptorHandle, short RecNumber, short FieldIdentifier, byte[] Value, int BufLen);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetDiagRec(OdbcHandleType HandleType, IntPtr Handle, ushort RecordNumber, byte[] Sqlstate, ref int NativeError, byte[] MessageText, short BufferLength, ref short TextLength);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLMoreResults(IntPtr Handle);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLFreeStmt(IntPtr Handle, libodbc.SQLFreeStmtOptions option);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLGetInfo(IntPtr connHandle, OdbcInfo info, byte[] buffer, short buffLength, ref short remainingStrLen);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLColAttribute(IntPtr StmtHandle, short column, FieldIdentifier fieldId, byte[] charAttributePtr, short bufferLength, ref short strLengthPtr, ref int numericAttributePtr);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLPrimaryKeys(IntPtr StmtHandle, string catalog, short catalogLength, string schema, short schemaLength, string tableName, short tableLength);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLStatistics(IntPtr StmtHandle, string catalog, short catalogLength, string schema, short schemaLength, string tableName, short tableLength, short unique, short Reserved);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLBindCol(IntPtr StmtHandle, short column, SQL_C_TYPE targetType, byte[] buffer, int bufferLength, ref int indicator);

		[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
		internal static extern OdbcReturn SQLBindCol(IntPtr StmtHandle, short column, SQL_C_TYPE targetType, ref short value, int bufferLength, ref int indicator);

		internal const int SQL_OV_ODBC2 = 2;

		internal const int SQL_OV_ODBC3 = 3;

		internal const string SQLSTATE_RIGHT_TRUNC = "01004";

		internal const char C_NULL = '\0';

		internal const int SQL_NTS = -3;

		internal const short SQL_TRUE = 1;

		internal const short SQL_FALSE = 0;

		internal const short SQL_INDEX_UNIQUE = 0;

		internal const short SQL_INDEX_ALL = 1;

		internal const short SQL_QUICK = 0;

		internal const short SQL_ENSURE = 1;

		internal const short SQL_NO_NULLS = 0;

		internal const short SQL_NULLABLE = 1;

		internal const short SQL_NULLABLE_UNKNOWN = 2;

		internal const short SQL_ATTR_READONLY = 0;

		internal const short SQL_ATTR_WRITE = 1;

		internal const short SQL_ATTR_READWRITE_UNKNOWN = 2;

		internal enum SQLFreeStmtOptions : short
		{
			Close,
			Drop,
			Unbind,
			ResetParams
		}
	}
}
