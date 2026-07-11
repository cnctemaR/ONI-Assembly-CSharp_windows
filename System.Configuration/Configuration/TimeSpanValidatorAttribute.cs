using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class TimeSpanValidatorAttribute : ConfigurationValidatorAttribute
	{
		public string MaxValueString
		{
			get
			{
				return this.maxValueString;
			}
			set
			{
				this.maxValueString = value;
				this.instance = null;
			}
		}

		public string MinValueString
		{
			get
			{
				return this.minValueString;
			}
			set
			{
				this.minValueString = value;
				this.instance = null;
			}
		}

		public TimeSpan MaxValue
		{
			get
			{
				return TimeSpan.Parse(this.maxValueString);
			}
		}

		public TimeSpan MinValue
		{
			get
			{
				return TimeSpan.Parse(this.minValueString);
			}
		}

		public bool ExcludeRange
		{
			get
			{
				return this.excludeRange;
			}
			set
			{
				this.excludeRange = value;
				this.instance = null;
			}
		}

		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new TimeSpanValidator(this.MinValue, this.MaxValue, this.excludeRange);
				}
				return this.instance;
			}
		}

		public const string TimeSpanMaxValue = "10675199.02:48:05.4775807";

		public const string TimeSpanMinValue = "-10675199.02:48:05.4775808";

		private bool excludeRange;

		private string maxValueString = "10675199.02:48:05.4775807";

		private string minValueString = "-10675199.02:48:05.4775808";

		private ConfigurationValidatorBase instance;
	}
}
