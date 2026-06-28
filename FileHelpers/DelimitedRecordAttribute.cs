using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DelimitedRecordAttribute : TypedRecordAttribute
	{
		public string Separator { get; private set; }

		public DelimitedRecordAttribute(string delimiter)
		{
			if (this.Separator != string.Empty)
			{
				this.Separator = delimiter;
				return;
			}
			throw new ArgumentException("Given delimiter cannot be <> \"\"", "delimiter");
		}
	}
}
