using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldValueDiscardedAttribute : Attribute
	{
	}
}
