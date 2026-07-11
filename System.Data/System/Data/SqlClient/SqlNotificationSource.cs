using System;

namespace System.Data.SqlClient
{
	public enum SqlNotificationSource
	{
		Data,
		Timeout,
		Object,
		Database,
		System,
		Statement,
		Environment,
		Execution,
		Owner,
		Unknown = -1,
		Client = -2
	}
}
