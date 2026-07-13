using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Interop.Attributes
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
	internal sealed class FatInterfaceImplAttribute : Attribute
	{
		public Type FatInterface { get; }

		public FatInterfaceImplAttribute(Type fatInterface)
		{
			this.FatInterface = fatInterface;
		}
	}
}
