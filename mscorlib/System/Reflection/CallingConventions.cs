using System;

namespace System.Reflection
{
	[Flags]
	public enum CallingConventions
	{
		Standard = 1,
		VarArgs = 2,
		Any = 3,
		HasThis = 32,
		ExplicitThis = 64
	}
}
