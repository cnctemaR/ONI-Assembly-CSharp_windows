using System;

namespace System.Data
{
	[Flags]
	public enum DataViewRowState
	{
		None = 0,
		Unchanged = 2,
		Added = 4,
		Deleted = 8,
		ModifiedCurrent = 16,
		ModifiedOriginal = 32,
		OriginalRows = 42,
		CurrentRows = 22
	}
}
