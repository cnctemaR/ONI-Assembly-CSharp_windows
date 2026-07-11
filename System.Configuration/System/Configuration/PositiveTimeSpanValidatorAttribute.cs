using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class PositiveTimeSpanValidatorAttribute : ConfigurationValidatorAttribute
	{
		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new PositiveTimeSpanValidator();
				}
				return this.instance;
			}
		}

		private ConfigurationValidatorBase instance;
	}
}
