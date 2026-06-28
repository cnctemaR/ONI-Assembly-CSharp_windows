using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class FixedLengthRecordAttribute : TypedRecordAttribute
	{
		public FixedMode FixedMode { get; private set; }

		public FixedLengthRecordAttribute()
			: this(FixedMode.ExactLength)
		{
		}

		public FixedLengthRecordAttribute(FixedMode fixedMode)
		{
			this.FixedMode = fixedMode;
		}
	}
}
