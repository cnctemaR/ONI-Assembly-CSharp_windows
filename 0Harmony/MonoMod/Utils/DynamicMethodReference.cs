using System;
using System.Reflection;
using Mono.Cecil;

namespace MonoMod.Utils
{
	internal class DynamicMethodReference : MethodReference
	{
		public DynamicMethodReference(ModuleDefinition module, MethodInfo dm)
			: base("", module.TypeSystem.Void)
		{
			this.DynamicMethod = dm;
		}

		public MethodInfo DynamicMethod;
	}
}
