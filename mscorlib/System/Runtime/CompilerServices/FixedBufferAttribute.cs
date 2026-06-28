using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class FixedBufferAttribute : Attribute
	{
		public FixedBufferAttribute(Type elementType, int length)
		{
			this.elementType = elementType;
			this.length = length;
		}

		public Type ElementType
		{
			get
			{
				return this.elementType;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		private Type elementType;

		private int length;
	}
}
