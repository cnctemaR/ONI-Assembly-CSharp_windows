using System;

namespace System.Runtime.CompilerServices
{
	[NullableContext(2)]
	internal interface ITuple
	{
		int Length { get; }

		object this[int index] { get; }
	}
}
