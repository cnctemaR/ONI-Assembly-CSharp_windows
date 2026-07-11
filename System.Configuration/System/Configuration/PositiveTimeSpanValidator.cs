using System;

namespace System.Configuration
{
	public class PositiveTimeSpanValidator : ConfigurationValidatorBase
	{
		public override bool CanValidate(Type type)
		{
			return type == typeof(TimeSpan);
		}

		public override void Validate(object value)
		{
			if ((TimeSpan)value <= new TimeSpan(0L))
			{
				throw new ArgumentException("The time span value must be positive");
			}
		}
	}
}
