using System;
using Unity;

namespace System.Data.SqlClient
{
	public sealed class SqlInfoMessageEventArgs : EventArgs
	{
		internal SqlInfoMessageEventArgs(SqlException exception)
		{
			this._exception = exception;
		}

		public SqlErrorCollection Errors
		{
			get
			{
				return this._exception.Errors;
			}
		}

		private bool ShouldSerializeErrors()
		{
			return this._exception != null && 0 < this._exception.Errors.Count;
		}

		public string Message
		{
			get
			{
				return this._exception.Message;
			}
		}

		public string Source
		{
			get
			{
				return this._exception.Source;
			}
		}

		public override string ToString()
		{
			return this.Message;
		}

		internal SqlInfoMessageEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private SqlException _exception;
	}
}
