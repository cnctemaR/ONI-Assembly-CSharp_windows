using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	internal interface IDMDGenerator
	{
		MethodInfo Generate(DynamicMethodDefinition dmd, [Nullable(2)] object context);
	}
}
