using System;
using System.Runtime.CompilerServices;

namespace MonoMod.ModInterop
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	internal sealed class ModImportNameAttribute : Attribute
	{
		public string Name { get; }

		public ModImportNameAttribute(string name)
		{
			this.Name = name;
		}
	}
}
