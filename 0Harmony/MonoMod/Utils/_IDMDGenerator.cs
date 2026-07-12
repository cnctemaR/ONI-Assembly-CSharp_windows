using System;
using System.Reflection;

namespace MonoMod.Utils
{
	internal interface _IDMDGenerator
	{
		MethodInfo Generate(DynamicMethodDefinition dmd, object context);
	}
}
