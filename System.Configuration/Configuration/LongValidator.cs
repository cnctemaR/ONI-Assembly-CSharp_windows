using System;

namespace System.Configuration
{
	public class LongValidator : ConfigurationValidatorBase
	{
		public LongValidator(long minValue, long maxValue, bool rangeIsExclusive, long resolution)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.rangeIsExclusive = rangeIsExclusive;
			this.resolution = resolution;
		}

		public LongValidator(long minValue, long maxValue, bool rangeIsExclusive)
			: this(minValue, maxValue, rangeIsExclusive, 0L)
		{
		}

		public LongValidator(long minValue, long maxValue)
			: this(minValue, maxValue, false, 0L)
		{
		}

		public override bool CanValidate(Type type)
		{
			return type == typeof(long);
		}

		public override void Validate(object value)
		{
			long num = (long)value;
			if (!this.rangeIsExclusive)
			{
				if (num < this.minValue || num > this.maxValue)
				{
					throw new ArgumentException(string.Concat(new object[] { "The value must be in the range ", this.minValue, " - ", this.maxValue }));
				}
			}
			else if (num >= this.minValue && num <= this.maxValue)
			{
				throw new ArgumentException(string.Concat(new object[] { "The value must not be in the range ", this.minValue, " - ", this.maxValue }));
			}
			if (this.resolution != 0L && num % this.resolution != 0L)
			{
				throw new ArgumentException("The value must have a resolution of " + this.resolution);
			}
		}

		private bool rangeIsExclusive;

		private long minValue;

		private long maxValue;

		private long resolution;
	}
}
