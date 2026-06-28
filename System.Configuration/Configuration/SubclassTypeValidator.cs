using System;

namespace System.Configuration
{
	public sealed class SubclassTypeValidator : ConfigurationValidatorBase
	{
		public SubclassTypeValidator(Type baseClass)
		{
			this.baseClass = baseClass;
		}

		public override bool CanValidate(Type type)
		{
			return type == typeof(Type);
		}

		public override void Validate(object value)
		{
			Type type = (Type)value;
			if (!this.baseClass.IsAssignableFrom(type))
			{
				throw new ArgumentException("The value must be a subclass");
			}
		}

		private Type baseClass;
	}
}
