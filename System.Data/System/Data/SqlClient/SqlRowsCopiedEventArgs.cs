using System;

namespace System.Data.SqlClient
{
	public class SqlRowsCopiedEventArgs : EventArgs
	{
		public SqlRowsCopiedEventArgs(long rowsCopied)
		{
			this.rowsCopied = rowsCopied;
		}

		public bool Abort
		{
			get
			{
				return this.abort;
			}
			set
			{
				this.abort = value;
			}
		}

		public long RowsCopied
		{
			get
			{
				return this.rowsCopied;
			}
		}

		private long rowsCopied;

		private bool abort;
	}
}
