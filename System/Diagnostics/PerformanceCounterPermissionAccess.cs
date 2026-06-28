using System;

namespace System.Diagnostics
{
	[Flags]
	public enum PerformanceCounterPermissionAccess
	{
		None = 0,
		[Obsolete]
		Browse = 1,
		Read = 1,
		Write = 2,
		[Obsolete]
		Instrument = 3,
		Administer = 7
	}
}
