using System;

namespace System.Data
{
	[Flags]
	public enum DataRowAction
	{
		Nothing = 0,
		Delete = 1,
		Change = 2,
		Rollback = 4,
		Commit = 8,
		Add = 16,
		ChangeOriginal = 32,
		ChangeCurrentAndOriginal = 64
	}
}
