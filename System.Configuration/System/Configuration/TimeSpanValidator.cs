using System;

namespace System.Configuration
{
	public class TimeSpanValidator : ConfigurationValidatorBase
	{
		public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue)
			: this(minValue, maxValue, false, 0L)
		{
		}

		public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue, bool rangeIsExclusive)
			: this(minValue, maxValue, rangeIsExclusive, 0L)
		{
		}

		public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue, bool rangeIsExclusive, long resolutionInSeconds)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.rangeIsExclusive = rangeIsExclusive;
			this.resolutionInSeconds = resolutionInSeconds;
		}

		public override bool CanValidate(Type type)
		{
			return type == typeof(TimeSpan);
		}

		public override void Validate(object value)
		{
			TimeSpan timeSpan = (TimeSpan)value;
			if (!this.rangeIsExclusive)
			{
				if (timeSpan < this.minValue || timeSpan > this.maxValue)
				{
					throw new ArgumentException("The value must be in the range " + this.minValue.ToString() + " - " + this.maxValue.ToString());
				}
			}
			else if (timeSpan >= this.minValue && timeSpan <= this.maxValue)
			{
				throw new ArgumentException("The value must not be in the range " + this.minValue.ToString() + " - " + this.maxValue.ToString());
			}
			if (this.resolutionInSeconds != 0L && timeSpan.Ticks % (10000000L * this.resolutionInSeconds) != 0L)
			{
				throw new ArgumentException("The value must have a resolution of " + TimeSpan.FromTicks(10000000L * this.resolutionInSeconds).ToString());
			}
		}

		private bool rangeIsExclusive;

		private TimeSpan minValue;

		private TimeSpan maxValue;

		private long resolutionInSeconds;
	}
}
