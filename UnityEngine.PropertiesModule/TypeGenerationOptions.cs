using System;

namespace Unity.Properties
{
	[Flags]
	public enum TypeGenerationOptions
	{
		None = 0,
		ValueType = 2,
		ReferenceType = 4,
		Default = 6
	}
}
