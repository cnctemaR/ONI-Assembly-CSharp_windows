using System;

namespace System.Data.Odbc
{
	public sealed class OdbcInfoMessageEventArgs : EventArgs
	{
		internal OdbcInfoMessageEventArgs()
		{
		}

		public OdbcErrorCollection Errors
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

		public override string ToString()
		{
			throw null;
		}
	}
}
