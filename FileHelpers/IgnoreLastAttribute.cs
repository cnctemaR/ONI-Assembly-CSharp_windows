using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class IgnoreLastAttribute : Attribute
	{
		public int NumberOfLines { get; private set; }

		public IgnoreLastAttribute()
			: this(1)
		{
		}

		public IgnoreLastAttribute(int numberOfLines)
		{
			this.NumberOfLines = numberOfLines;
		}
	}
}
