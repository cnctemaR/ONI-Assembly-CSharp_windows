using System;

namespace System.Data.SqlClient
{
	public class SqlRowsCopiedEventArgs : EventArgs
	{
		public SqlRowsCopiedEventArgs(long rowsCopied)
		{
			this._rowsCopied = rowsCopied;
		}

		public bool Abort
		{
			get
			{
				return this._abort;
			}
			set
			{
				this._abort = value;
			}
		}

		public long RowsCopied
		{
			get
			{
				return this._rowsCopied;
			}
		}

		private bool _abort;

		private long _rowsCopied;
	}
}
