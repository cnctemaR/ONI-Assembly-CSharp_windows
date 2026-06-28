using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FieldDelimiterAttribute : FieldAttribute
	{
		public string Delimiter { get; private set; }

		public FieldDelimiterAttribute(string separator)
		{
			if (string.IsNullOrEmpty(separator))
			{
				throw new BadUsageException("The separator parameter of the FieldDelimiter attribute can't be null or empty");
			}
			this.Delimiter = separator;
		}
	}
}
