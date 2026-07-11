using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IntegerValidatorAttribute : ConfigurationValidatorAttribute
	{
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

		public int MaxValue
		{
			get
			{
				return this.maxValue;
			}
			set
			{
				this.maxValue = value;
				this.instance = null;
			}
		}

		public int MinValue
		{
			get
			{
				return this.minValue;
			}
			set
			{
				this.minValue = value;
				this.instance = null;
			}
		}

		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new IntegerValidator(this.minValue, this.maxValue, this.excludeRange);
				}
				return this.instance;
			}
		}

		private bool excludeRange;

		private int maxValue;

		private int minValue;

		private ConfigurationValidatorBase instance;
	}
}
