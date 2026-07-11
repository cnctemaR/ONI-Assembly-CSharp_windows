using System;
using Unity;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlError
	{
		internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber, uint win32ErrorCode, Exception exception = null)
			: this(infoNumber, errorState, errorClass, server, errorMessage, procedure, lineNumber, exception)
		{
			this._win32ErrorCode = (int)win32ErrorCode;
		}

		internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber, Exception exception = null)
		{
			this._source = "Core .Net SqlClient Data Provider";
			base..ctor();
			this._number = infoNumber;
			this._state = errorState;
			this._errorClass = errorClass;
			this._server = server;
			this._message = errorMessage;
			this._procedure = procedure;
			this._lineNumber = lineNumber;
			this._win32ErrorCode = 0;
			this._exception = exception;
		}

		public override string ToString()
		{
			return typeof(SqlError).ToString() + ": " + this._message;
		}

		public string Source
		{
			get
			{
				return this._source;
			}
		}

		public int Number
		{
			get
			{
				return this._number;
			}
		}

		public byte State
		{
			get
			{
				return this._state;
			}
		}

		public byte Class
		{
			get
			{
				return this._errorClass;
			}
		}

		public string Server
		{
			get
			{
				return this._server;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		public string Procedure
		{
			get
			{
				return this._procedure;
			}
		}

		public int LineNumber
		{
			get
			{
				return this._lineNumber;
			}
		}

		internal int Win32ErrorCode
		{
			get
			{
				return this._win32ErrorCode;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this._exception;
			}
		}

		internal SqlError()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string _source;

		private int _number;

		private byte _state;

		private byte _errorClass;

		private string _server;

		private string _message;

		private string _procedure;

		private int _lineNumber;

		private int _win32ErrorCode;

		private Exception _exception;
	}
}
