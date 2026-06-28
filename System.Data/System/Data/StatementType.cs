using System;

namespace System.Data
{
	public enum StatementType
	{
		Select,
		Insert,
		Update,
		Batch = 4,
		Delete = 3
	}
}
