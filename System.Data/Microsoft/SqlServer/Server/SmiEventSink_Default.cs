using System;
using System.Data.SqlClient;

namespace Microsoft.SqlServer.Server
{
	internal class SmiEventSink_Default : SmiEventSink
	{
		internal bool HasMessages
		{
			get
			{
				return this._errors != null || this._warnings != null;
			}
		}

		internal virtual string ServerVersion
		{
			get
			{
				return null;
			}
		}

		protected virtual void DispatchMessages()
		{
			SqlException ex = this.ProcessMessages(true);
			if (ex != null)
			{
				throw ex;
			}
		}

		protected SqlException ProcessMessages(bool ignoreWarnings)
		{
			SqlException ex = null;
			SqlErrorCollection sqlErrorCollection = null;
			if (this._errors != null)
			{
				if (this._warnings != null)
				{
					foreach (object obj in this._warnings)
					{
						SqlError sqlError = (SqlError)obj;
						this._errors.Add(sqlError);
					}
				}
				sqlErrorCollection = this._errors;
				this._errors = null;
				this._warnings = null;
			}
			else
			{
				if (!ignoreWarnings)
				{
					sqlErrorCollection = this._warnings;
				}
				this._warnings = null;
			}
			if (sqlErrorCollection != null)
			{
				ex = SqlException.CreateException(sqlErrorCollection, this.ServerVersion);
			}
			return ex;
		}

		internal void ProcessMessagesAndThrow()
		{
			if (this.HasMessages)
			{
				this.DispatchMessages();
			}
		}

		internal SmiEventSink_Default()
		{
		}

		private SqlErrorCollection _errors;

		private SqlErrorCollection _warnings;
	}
}
