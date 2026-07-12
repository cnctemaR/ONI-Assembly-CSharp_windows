using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class FixedBufferAttribute : Attribute
	{
		public FixedBufferAttribute(Type elementType, int length)
		{
			this.ElementType = elementType;
			this.Length = length;
		}

		public Type ElementType { get; }

		public int Length { get; }
	}
}
