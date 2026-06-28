using System;

namespace System.Configuration
{
	public abstract class ConfigurationValidatorBase
	{
		public virtual bool CanValidate(Type type)
		{
			return false;
		}

		public abstract void Validate(object value);
	}
}
