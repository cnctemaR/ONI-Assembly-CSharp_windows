using System;

namespace System.Data.SqlClient
{
	public class SqlNotificationEventArgs : EventArgs
	{
		public SqlNotificationEventArgs(SqlNotificationType type, SqlNotificationInfo info, SqlNotificationSource source)
		{
			this.type = type;
			this.info = info;
			this.source = source;
		}

		public SqlNotificationType Type
		{
			get
			{
				return this.type;
			}
		}

		public SqlNotificationInfo Info
		{
			get
			{
				return this.info;
			}
		}

		public SqlNotificationSource Source
		{
			get
			{
				return this.source;
			}
		}

		private SqlNotificationType type;

		private SqlNotificationInfo info;

		private SqlNotificationSource source;
	}
}
