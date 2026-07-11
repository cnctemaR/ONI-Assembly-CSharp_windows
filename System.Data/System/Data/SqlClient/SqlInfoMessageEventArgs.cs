using System;

namespace System.Data.SqlClient
{
	public sealed class SqlInfoMessageEventArgs : EventArgs
	{
		internal SqlInfoMessageEventArgs()
		{
		}

		public SqlErrorCollection Errors
		{
			get
			{
				throw null;
			}
		}

		public string Message
		{
			get
			{
				throw null;
			}
		}

		public string Source
		{
			get
			{
				throw null;
			}
		}

		public override string ToString()
		{
			throw null;
		}
	}
}
