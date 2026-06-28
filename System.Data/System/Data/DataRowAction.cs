using System;

namespace System.Data
{
	[Flags]
	public enum DataRowAction
	{
		Add = 16,
		Change = 2,
		ChangeCurrentAndOriginal = 64,
		ChangeOriginal = 32,
		Commit = 8,
		Delete = 1,
		Nothing = 0,
		Rollback = 4
	}
}
