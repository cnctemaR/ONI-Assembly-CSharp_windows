using System;

namespace System.Data.SqlClient
{
	public class SqlNotificationEventArgs : EventArgs
	{
		public SqlNotificationEventArgs(SqlNotificationType type, SqlNotificationInfo info, SqlNotificationSource source)
		{
		}

		public SqlNotificationInfo Info
		{
			get
			{
				throw null;
			}
		}

		public SqlNotificationSource Source
		{
			get
			{
				throw null;
			}
		}

		public SqlNotificationType Type
		{
			get
			{
				throw null;
			}
		}
	}
}
