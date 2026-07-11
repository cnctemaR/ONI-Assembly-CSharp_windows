using System;

namespace System.Data
{
	[Flags]
	public enum CommandBehavior
	{
		CloseConnection = 32,
		Default = 0,
		KeyInfo = 4,
		SchemaOnly = 2,
		SequentialAccess = 16,
		SingleResult = 1,
		SingleRow = 8
	}
}
