using System;
using System.Reflection.Emit;
using Mono.Cecil;

namespace MonoMod.Utils
{
	internal class DynamicMethodReference : MethodReference
	{
		public DynamicMethodReference(ModuleDefinition module, DynamicMethod dm)
			: base("", module.TypeSystem.Void)
		{
			this.DynamicMethod = dm;
		}

		public DynamicMethod DynamicMethod;
	}
}
