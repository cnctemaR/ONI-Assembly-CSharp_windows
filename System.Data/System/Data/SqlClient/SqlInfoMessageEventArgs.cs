using System;
using System.Collections;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	public sealed class SqlInfoMessageEventArgs : EventArgs
	{
		internal SqlInfoMessageEventArgs(TdsInternalErrorCollection tdsErrors)
		{
			foreach (object obj in ((IEnumerable)tdsErrors))
			{
				TdsInternalError tdsInternalError = (TdsInternalError)obj;
				this.errors.Add(tdsInternalError.Class, tdsInternalError.LineNumber, tdsInternalError.Message, tdsInternalError.Number, tdsInternalError.Procedure, tdsInternalError.Server, "Mono SqlClient Data Provider", tdsInternalError.State);
			}
		}

		public SqlErrorCollection Errors
		{
			get
			{
				return this.errors;
			}
		}

		public string Message
		{
			get
			{
				return this.errors[0].Message;
			}
		}

		public string Source
		{
			get
			{
				return this.errors[0].Source;
			}
		}

		public override string ToString()
		{
			return this.Message;
		}

		private SqlErrorCollection errors = new SqlErrorCollection();
	}
}
