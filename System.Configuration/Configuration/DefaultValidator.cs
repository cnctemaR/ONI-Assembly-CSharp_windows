using System;

namespace System.Configuration
{
	public sealed class DefaultValidator : ConfigurationValidatorBase
	{
		public override bool CanValidate(Type type)
		{
			return true;
		}

		public override void Validate(object value)
		{
		}
	}
}
