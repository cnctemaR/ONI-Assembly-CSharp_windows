using System;

namespace System.Diagnostics
{
	[Flags]
	public enum EventLogPermissionAccess
	{
		None = 0,
		[Obsolete]
		Browse = 2,
		[Obsolete]
		Instrument = 6,
		[Obsolete]
		Audit = 10,
		Write = 16,
		Administer = 48
	}
}
