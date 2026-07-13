using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class NullablePublicOnlyAttribute : Attribute
	{
		public NullablePublicOnlyAttribute(bool A_1)
		{
			this.IncludesInternals = A_1;
		}

		public readonly bool IncludesInternals;
	}
}
