using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[Microsoft.CodeAnalysis.Embedded]
	[CompilerGenerated]
	internal sealed class NullableContextAttribute : Attribute
	{
		public NullableContextAttribute(byte A_1)
		{
			this.Flag = A_1;
		}

		public readonly byte Flag;
	}
}
