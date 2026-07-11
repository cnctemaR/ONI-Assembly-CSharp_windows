using System;

namespace System.Data.SqlClient
{
	public enum SqlNotificationInfo
	{
		Truncate,
		Insert,
		Update,
		Delete,
		Drop,
		Alter,
		Restart,
		Error,
		Query,
		Invalid,
		Options,
		Isolation,
		Expired,
		Resource,
		PreviousFire,
		TemplateLimit,
		Merge,
		Unknown = -1,
		AlreadyChanged = -2
	}
}
