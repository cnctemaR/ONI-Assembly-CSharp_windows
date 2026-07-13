using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	[Embedded]
	[CompilerGenerated]
	internal sealed class NativeIntegerAttribute : Attribute
	{
		public NativeIntegerAttribute()
		{
			this.TransformFlags = new bool[] { true };
		}

		public NativeIntegerAttribute(bool[] A_0)
		{
			this.TransformFlags = A_0;
		}

		public readonly bool[] TransformFlags;
	}
}
