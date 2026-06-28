using System;

namespace System.Data.SqlClient
{
	internal class SqlAsyncState
	{
		public SqlAsyncState(object userState)
		{
			this._userState = userState;
		}

		public object UserState
		{
			get
			{
				return this._userState;
			}
			set
			{
				this._userState = value;
			}
		}

		private object _userState;
	}
}
