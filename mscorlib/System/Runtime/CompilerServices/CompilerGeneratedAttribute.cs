using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.All, Inherited = true)]
	[Serializable]
	public sealed class CompilerGeneratedAttribute : Attribute
	{
	}
}
