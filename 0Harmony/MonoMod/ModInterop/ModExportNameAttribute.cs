using System;
using System.Runtime.CompilerServices;

namespace MonoMod.ModInterop
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class ModExportNameAttribute : Attribute
	{
		public string Name { get; }

		public ModExportNameAttribute(string name)
		{
			this.Name = name;
		}
	}
}
