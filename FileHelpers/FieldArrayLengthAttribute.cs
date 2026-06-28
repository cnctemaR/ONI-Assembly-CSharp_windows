using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldArrayLengthAttribute : Attribute
	{
		public FieldArrayLengthAttribute(int minLength, int maxLength)
		{
			this.MinLength = minLength;
			this.MaxLength = maxLength;
		}

		public FieldArrayLengthAttribute(int length)
			: this(length, length)
		{
		}

		public int MinLength { get; private set; }

		public int MaxLength { get; private set; }
	}
}
