using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class IgnoreFirstAttribute : Attribute
	{
		public int NumberOfLines { get; private set; }

		public IgnoreFirstAttribute()
			: this(1)
		{
		}

		public IgnoreFirstAttribute(int numberOfLines)
		{
			this.NumberOfLines = numberOfLines;
		}
	}
}
