using System;

namespace System.Data.SqlClient
{
	public class SqlRowsCopiedEventArgs : EventArgs
	{
		public SqlRowsCopiedEventArgs(long rowsCopied)
		{
		}

		public bool Abort
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public long RowsCopied
		{
			get
			{
				throw null;
			}
		}
	}
}
