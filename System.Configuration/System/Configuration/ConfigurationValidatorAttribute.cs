using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ConfigurationValidatorAttribute : Attribute
	{
		protected ConfigurationValidatorAttribute()
		{
		}

		public ConfigurationValidatorAttribute(Type validator)
		{
			this.validatorType = validator;
		}

		public virtual ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = (ConfigurationValidatorBase)Activator.CreateInstance(this.validatorType);
				}
				return this.instance;
			}
		}

		public Type ValidatorType
		{
			get
			{
				return this.validatorType;
			}
		}

		private Type validatorType;

		private ConfigurationValidatorBase instance;
	}
}
