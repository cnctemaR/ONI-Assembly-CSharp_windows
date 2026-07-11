using System;

namespace System.Data.SqlTypes
{
	internal enum SqlBytesCharsState
	{
		Null,
		Buffer,
		Stream = 3
	}
}
