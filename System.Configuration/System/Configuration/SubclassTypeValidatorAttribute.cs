using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SubclassTypeValidatorAttribute : ConfigurationValidatorAttribute
	{
		public SubclassTypeValidatorAttribute(Type baseClass)
		{
			this.baseClass = baseClass;
		}

		public Type BaseClass
		{
			get
			{
				return this.baseClass;
			}
		}

		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new SubclassTypeValidator(this.baseClass);
				}
				return this.instance;
			}
		}

		private Type baseClass;

		private ConfigurationValidatorBase instance;
	}
}
