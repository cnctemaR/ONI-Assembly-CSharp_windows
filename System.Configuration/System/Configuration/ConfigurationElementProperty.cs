using System;

namespace System.Configuration
{
	public sealed class ConfigurationElementProperty
	{
		public ConfigurationElementProperty(ConfigurationValidatorBase validator)
		{
			this.validator = validator;
		}

		public ConfigurationValidatorBase Validator
		{
			get
			{
				return this.validator;
			}
		}

		private ConfigurationValidatorBase validator;
	}
}
