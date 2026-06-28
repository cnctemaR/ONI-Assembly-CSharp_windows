using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class IgnoreInheritedClassAttribute : Attribute
	{
	}
}
