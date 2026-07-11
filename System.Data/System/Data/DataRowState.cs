using System;

namespace System.Data
{
	[Flags]
	public enum DataRowState
	{
		Added = 4,
		Deleted = 8,
		Detached = 1,
		Modified = 16,
		Unchanged = 2
	}
}
