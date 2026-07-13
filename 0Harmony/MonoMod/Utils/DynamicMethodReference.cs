using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class DynamicMethodReference : MethodReference
	{
		public MethodInfo DynamicMethod { get; }

		public DynamicMethodReference(ModuleDefinition module, MethodInfo dm)
			: base("", Helpers.ThrowIfNull<ModuleDefinition>(module, "module").TypeSystem.Void)
		{
			this.DynamicMethod = dm;
		}
	}
}
