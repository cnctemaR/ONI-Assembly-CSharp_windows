using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class IgnoreEmptyLinesAttribute : Attribute
	{
		public bool IgnoreSpaces { get; private set; }

		public IgnoreEmptyLinesAttribute()
		{
			this.IgnoreSpaces = false;
		}

		public IgnoreEmptyLinesAttribute(bool ignoreSpaces)
		{
			this.IgnoreSpaces = ignoreSpaces;
		}
	}
}
