using System;

namespace System.Data.SqlClient
{
	public class SqlNotificationEventArgs : EventArgs
	{
		public SqlNotificationEventArgs(SqlNotificationType type, SqlNotificationInfo info, SqlNotificationSource source)
		{
			this._info = info;
			this._source = source;
			this._type = type;
		}

		public SqlNotificationType Type
		{
			get
			{
				return this._type;
			}
		}

		public SqlNotificationInfo Info
		{
			get
			{
				return this._info;
			}
		}

		public SqlNotificationSource Source
		{
			get
			{
				return this._source;
			}
		}

		private SqlNotificationType _type;

		private SqlNotificationInfo _info;

		private SqlNotificationSource _source;

		internal static SqlNotificationEventArgs s_notifyError = new SqlNotificationEventArgs(SqlNotificationType.Subscribe, SqlNotificationInfo.Error, SqlNotificationSource.Object);
	}
}
