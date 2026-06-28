using System;

namespace System.Data.OleDb
{
	[Serializable]
	public sealed class OleDbError
	{
		internal OleDbError(string msg, int code, string source, string sql)
		{
			this.message = msg;
			this.nativeError = code;
			this.source = source;
			this.sqlState = sql;
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public int NativeError
		{
			get
			{
				return this.nativeError;
			}
		}

		public string Source
		{
			get
			{
				return this.source;
			}
		}

		public string SQLState
		{
			get
			{
				return this.sqlState;
			}
		}

		[MonoTODO]
		public override string ToString()
		{
			string text = " <Stack Trace>";
			return "OleDbError:" + this.message + text;
		}

		private string message;

		private int nativeError;

		private string source;

		private string sqlState;
	}
}
