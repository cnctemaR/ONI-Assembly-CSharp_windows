using System;

namespace System.Numerics
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
	internal class JitIntrinsicAttribute : Attribute
	{
	}
}
