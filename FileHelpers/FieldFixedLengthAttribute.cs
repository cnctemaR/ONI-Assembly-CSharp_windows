using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldFixedLengthAttribute : FieldAttribute
	{
		public int Length { get; private set; }

		public FieldFixedLengthAttribute(int length)
		{
			if (length > 0)
			{
				this.Length = length;
				return;
			}
			throw new BadUsageException("The FieldFixedLength attribute must be > 0");
		}
	}
}
