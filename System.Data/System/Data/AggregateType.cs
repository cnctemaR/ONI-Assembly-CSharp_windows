using System;

namespace System.Data
{
	internal enum AggregateType
	{
		None,
		Sum = 4,
		Mean,
		Min,
		Max,
		First,
		Count,
		Var,
		StDev
	}
}
