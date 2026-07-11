using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	internal sealed class CMDWrapper
	{
		internal CMDWrapper(OdbcConnection connection)
		{
			this._connection = connection;
		}

		internal bool Canceling
		{
			get
			{
				return this._canceling;
			}
			set
			{
				this._canceling = value;
			}
		}

		internal OdbcConnection Connection
		{
			get
			{
				return this._connection;
			}
		}

		internal bool HasBoundColumns
		{
			set
			{
				this._hasBoundColumns = value;
			}
		}

		internal OdbcStatementHandle StatementHandle
		{
			get
			{
				return this._stmt;
			}
		}

		internal OdbcStatementHandle KeyInfoStatement
		{
			get
			{
				return this._keyinfostmt;
			}
		}

		internal void CreateKeyInfoStatementHandle()
		{
			this.DisposeKeyInfoStatementHandle();
			this._keyinfostmt = this._connection.CreateStatementHandle();
		}

		internal void CreateStatementHandle()
		{
			this.DisposeStatementHandle();
			this._stmt = this._connection.CreateStatementHandle();
		}

		internal void Dispose()
		{
			if (this._dataReaderBuf != null)
			{
				this._dataReaderBuf.Dispose();
				this._dataReaderBuf = null;
			}
			this.DisposeStatementHandle();
			CNativeBuffer nativeParameterBuffer = this._nativeParameterBuffer;
			this._nativeParameterBuffer = null;
			if (nativeParameterBuffer != null)
			{
				nativeParameterBuffer.Dispose();
			}
			this._ssKeyInfoModeOn = false;
			this._ssKeyInfoModeOff = false;
		}

		private void DisposeDescriptorHandle()
		{
			OdbcDescriptorHandle hdesc = this._hdesc;
			if (hdesc != null)
			{
				this._hdesc = null;
				hdesc.Dispose();
			}
		}

		internal void DisposeStatementHandle()
		{
			this.DisposeKeyInfoStatementHandle();
			this.DisposeDescriptorHandle();
			OdbcStatementHandle stmt = this._stmt;
			if (stmt != null)
			{
				this._stmt = null;
				stmt.Dispose();
			}
		}

		internal void DisposeKeyInfoStatementHandle()
		{
			OdbcStatementHandle keyinfostmt = this._keyinfostmt;
			if (keyinfostmt != null)
			{
				this._keyinfostmt = null;
				keyinfostmt.Dispose();
			}
		}

		internal void FreeStatementHandle(ODBC32.STMT stmt)
		{
			this.DisposeDescriptorHandle();
			OdbcStatementHandle stmt2 = this._stmt;
			if (stmt2 != null)
			{
				try
				{
					ODBC32.RetCode retCode = stmt2.FreeStatement(stmt);
					this.StatementErrorHandler(retCode);
				}
				catch (Exception ex)
				{
					if (ADP.IsCatchableExceptionType(ex))
					{
						this._stmt = null;
						stmt2.Dispose();
					}
					throw;
				}
			}
		}

		internal void FreeKeyInfoStatementHandle(ODBC32.STMT stmt)
		{
			OdbcStatementHandle keyinfostmt = this._keyinfostmt;
			if (keyinfostmt != null)
			{
				try
				{
					keyinfostmt.FreeStatement(stmt);
				}
				catch (Exception ex)
				{
					if (ADP.IsCatchableExceptionType(ex))
					{
						this._keyinfostmt = null;
						keyinfostmt.Dispose();
					}
					throw;
				}
			}
		}

		internal OdbcDescriptorHandle GetDescriptorHandle(ODBC32.SQL_ATTR attribute)
		{
			OdbcDescriptorHandle odbcDescriptorHandle = this._hdesc;
			if (this._hdesc == null)
			{
				odbcDescriptorHandle = (this._hdesc = new OdbcDescriptorHandle(this._stmt, attribute));
			}
			return odbcDescriptorHandle;
		}

		internal string GetDiagSqlState()
		{
			string text;
			this._stmt.GetDiagnosticField(out text);
			return text;
		}

		internal void StatementErrorHandler(ODBC32.RetCode retcode)
		{
			if (retcode <= ODBC32.RetCode.SUCCESS_WITH_INFO)
			{
				this._connection.HandleErrorNoThrow(this._stmt, retcode);
				return;
			}
			throw this._connection.HandleErrorNoThrow(this._stmt, retcode);
		}

		internal void UnbindStmtColumns()
		{
			if (this._hasBoundColumns)
			{
				this.FreeStatementHandle(ODBC32.STMT.UNBIND);
				this._hasBoundColumns = false;
			}
		}

		private OdbcStatementHandle _stmt;

		private OdbcStatementHandle _keyinfostmt;

		internal OdbcDescriptorHandle _hdesc;

		internal CNativeBuffer _nativeParameterBuffer;

		internal CNativeBuffer _dataReaderBuf;

		private readonly OdbcConnection _connection;

		private bool _canceling;

		internal bool _hasBoundColumns;

		internal bool _ssKeyInfoModeOn;

		internal bool _ssKeyInfoModeOff;
	}
}
