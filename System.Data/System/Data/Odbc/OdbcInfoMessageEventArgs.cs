using System;

namespace System.Data.Odbc
{
	public sealed class OdbcInfoMessageEventArgs : EventArgs
	{
		internal OdbcInfoMessageEventArgs(OdbcErrorCollection errors)
		{
			foreach (object obj in errors)
			{
				OdbcError odbcError = (OdbcError)obj;
				this.errors.Add(odbcError);
			}
		}

		public OdbcErrorCollection Errors
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

		public override string ToString()
		{
			return this.Message;
		}

		private OdbcErrorCollection errors = new OdbcErrorCollection();
	}
}
