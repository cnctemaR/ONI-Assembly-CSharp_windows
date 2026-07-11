using System;
using System.Diagnostics;

namespace Internal.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("ALWAYSREMOVED")]
	internal class RelocatedTypeAttribute : Attribute
	{
		public RelocatedTypeAttribute(string originalAssemblySimpleName)
		{
		}
	}
}
