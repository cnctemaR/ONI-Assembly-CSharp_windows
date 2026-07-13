using System;

namespace System.Runtime.CompilerServices
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class IgnoresAccessChecksToAttribute : Attribute
	{
		public string AssemblyName { get; }

		public IgnoresAccessChecksToAttribute(string assemblyName)
		{
			this.AssemblyName = assemblyName;
		}
	}
}
