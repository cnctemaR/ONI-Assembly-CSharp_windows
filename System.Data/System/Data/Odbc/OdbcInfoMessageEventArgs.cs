using System;
using System.Text;
using Unity;

namespace System.Data.Odbc
{
	public sealed class OdbcInfoMessageEventArgs : EventArgs
	{
		internal OdbcInfoMessageEventArgs(OdbcErrorCollection errors)
		{
			this._errors = errors;
		}

		public OdbcErrorCollection Errors
		{
			get
			{
				return this._errors;
			}
		}

		public string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in this.Errors)
				{
					OdbcError odbcError = (OdbcError)obj;
					if (0 < stringBuilder.Length)
					{
						stringBuilder.Append(Environment.NewLine);
					}
					stringBuilder.Append(odbcError.Message);
				}
				return stringBuilder.ToString();
			}
		}

		public override string ToString()
		{
			return this.Message;
		}

		internal OdbcInfoMessageEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private OdbcErrorCollection _errors;
	}
}
