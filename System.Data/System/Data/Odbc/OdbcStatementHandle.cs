using System;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace System.Data.Odbc
{
	internal sealed class OdbcStatementHandle : OdbcHandle
	{
		internal OdbcStatementHandle(OdbcConnectionHandle connectionHandle)
			: base(ODBC32.SQL_HANDLE.STMT, connectionHandle)
		{
		}

		internal ODBC32.RetCode BindColumn2(int columnNumber, ODBC32.SQL_C targetType, HandleRef buffer, IntPtr length, IntPtr srLen_or_Ind)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLBindCol(this, checked((ushort)columnNumber), targetType, buffer, length, srLen_or_Ind);
			ODBC.TraceODBC(3, "SQLBindCol", retCode);
			return retCode;
		}

		internal ODBC32.RetCode BindColumn3(int columnNumber, ODBC32.SQL_C targetType, IntPtr srLen_or_Ind)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLBindCol(this, checked((ushort)columnNumber), targetType, ADP.PtrZero, ADP.PtrZero, srLen_or_Ind);
			ODBC.TraceODBC(3, "SQLBindCol", retCode);
			return retCode;
		}

		internal ODBC32.RetCode BindParameter(short ordinal, short parameterDirection, ODBC32.SQL_C sqlctype, ODBC32.SQL_TYPE sqltype, IntPtr cchSize, IntPtr scale, HandleRef buffer, IntPtr bufferLength, HandleRef intbuffer)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLBindParameter(this, checked((ushort)ordinal), parameterDirection, sqlctype, (short)sqltype, cchSize, scale, buffer, bufferLength, intbuffer);
			ODBC.TraceODBC(3, "SQLBindParameter", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Cancel()
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLCancel(this);
			ODBC.TraceODBC(3, "SQLCancel", retCode);
			return retCode;
		}

		internal ODBC32.RetCode CloseCursor()
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLCloseCursor(this);
			ODBC.TraceODBC(3, "SQLCloseCursor", retCode);
			return retCode;
		}

		internal ODBC32.RetCode ColumnAttribute(int columnNumber, short fieldIdentifier, CNativeBuffer characterAttribute, out short stringLength, out SQLLEN numericAttribute)
		{
			IntPtr intPtr;
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLColAttributeW(this, checked((short)columnNumber), fieldIdentifier, characterAttribute, characterAttribute.ShortLength, out stringLength, out intPtr);
			numericAttribute = new SQLLEN(intPtr);
			ODBC.TraceODBC(3, "SQLColAttributeW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Columns(string tableCatalog, string tableSchema, string tableName, string columnName)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLColumnsW(this, tableCatalog, ODBC.ShortStringLength(tableCatalog), tableSchema, ODBC.ShortStringLength(tableSchema), tableName, ODBC.ShortStringLength(tableName), columnName, ODBC.ShortStringLength(columnName));
			ODBC.TraceODBC(3, "SQLColumnsW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Execute()
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLExecute(this);
			ODBC.TraceODBC(3, "SQLExecute", retCode);
			return retCode;
		}

		internal ODBC32.RetCode ExecuteDirect(string commandText)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLExecDirectW(this, commandText, -3);
			ODBC.TraceODBC(3, "SQLExecDirectW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Fetch()
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLFetch(this);
			ODBC.TraceODBC(3, "SQLFetch", retCode);
			return retCode;
		}

		internal ODBC32.RetCode FreeStatement(ODBC32.STMT stmt)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLFreeStmt(this, stmt);
			ODBC.TraceODBC(3, "SQLFreeStmt", retCode);
			return retCode;
		}

		internal ODBC32.RetCode GetData(int index, ODBC32.SQL_C sqlctype, CNativeBuffer buffer, int cb, out IntPtr cbActual)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLGetData(this, checked((ushort)index), sqlctype, buffer, new IntPtr(cb), out cbActual);
			ODBC.TraceODBC(3, "SQLGetData", retCode);
			return retCode;
		}

		internal ODBC32.RetCode GetStatementAttribute(ODBC32.SQL_ATTR attribute, out IntPtr value, out int stringLength)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLGetStmtAttrW(this, attribute, out value, ADP.PtrSize, out stringLength);
			ODBC.TraceODBC(3, "SQLGetStmtAttrW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode GetTypeInfo(short fSqlType)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLGetTypeInfo(this, fSqlType);
			ODBC.TraceODBC(3, "SQLGetTypeInfo", retCode);
			return retCode;
		}

		internal ODBC32.RetCode MoreResults()
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLMoreResults(this);
			ODBC.TraceODBC(3, "SQLMoreResults", retCode);
			return retCode;
		}

		internal ODBC32.RetCode NumberOfResultColumns(out short columnsAffected)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLNumResultCols(this, out columnsAffected);
			ODBC.TraceODBC(3, "SQLNumResultCols", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Prepare(string commandText)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLPrepareW(this, commandText, -3);
			ODBC.TraceODBC(3, "SQLPrepareW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode PrimaryKeys(string catalogName, string schemaName, string tableName)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLPrimaryKeysW(this, catalogName, ODBC.ShortStringLength(catalogName), schemaName, ODBC.ShortStringLength(schemaName), tableName, ODBC.ShortStringLength(tableName));
			ODBC.TraceODBC(3, "SQLPrimaryKeysW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Procedures(string procedureCatalog, string procedureSchema, string procedureName)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLProceduresW(this, procedureCatalog, ODBC.ShortStringLength(procedureCatalog), procedureSchema, ODBC.ShortStringLength(procedureSchema), procedureName, ODBC.ShortStringLength(procedureName));
			ODBC.TraceODBC(3, "SQLProceduresW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode ProcedureColumns(string procedureCatalog, string procedureSchema, string procedureName, string columnName)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLProcedureColumnsW(this, procedureCatalog, ODBC.ShortStringLength(procedureCatalog), procedureSchema, ODBC.ShortStringLength(procedureSchema), procedureName, ODBC.ShortStringLength(procedureName), columnName, ODBC.ShortStringLength(columnName));
			ODBC.TraceODBC(3, "SQLProcedureColumnsW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode RowCount(out SQLLEN rowCount)
		{
			IntPtr intPtr;
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLRowCount(this, out intPtr);
			rowCount = new SQLLEN(intPtr);
			ODBC.TraceODBC(3, "SQLRowCount", retCode);
			return retCode;
		}

		internal ODBC32.RetCode SetStatementAttribute(ODBC32.SQL_ATTR attribute, IntPtr value, ODBC32.SQL_IS stringLength)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLSetStmtAttrW(this, (int)attribute, value, (int)stringLength);
			ODBC.TraceODBC(3, "SQLSetStmtAttrW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode SpecialColumns(string quotedTable)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLSpecialColumnsW(this, ODBC32.SQL_SPECIALCOLS.ROWVER, null, 0, null, 0, quotedTable, ODBC.ShortStringLength(quotedTable), ODBC32.SQL_SCOPE.SESSION, ODBC32.SQL_NULLABILITY.NO_NULLS);
			ODBC.TraceODBC(3, "SQLSpecialColumnsW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Statistics(string tableCatalog, string tableSchema, string tableName, short unique, short accuracy)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLStatisticsW(this, tableCatalog, ODBC.ShortStringLength(tableCatalog), tableSchema, ODBC.ShortStringLength(tableSchema), tableName, ODBC.ShortStringLength(tableName), unique, accuracy);
			ODBC.TraceODBC(3, "SQLStatisticsW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Statistics(string tableName)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLStatisticsW(this, null, 0, null, 0, tableName, ODBC.ShortStringLength(tableName), 0, 1);
			ODBC.TraceODBC(3, "SQLStatisticsW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode Tables(string tableCatalog, string tableSchema, string tableName, string tableType)
		{
			ODBC32.RetCode retCode = UnsafeNativeMethods.SQLTablesW(this, tableCatalog, ODBC.ShortStringLength(tableCatalog), tableSchema, ODBC.ShortStringLength(tableSchema), tableName, ODBC.ShortStringLength(tableName), tableType, ODBC.ShortStringLength(tableType));
			ODBC.TraceODBC(3, "SQLTablesW", retCode);
			return retCode;
		}
	}
}
