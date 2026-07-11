using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CallbackValidatorAttribute : ConfigurationValidatorAttribute
	{
		public string CallbackMethodName
		{
			get
			{
				return this.callbackMethodName;
			}
			set
			{
				this.callbackMethodName = value;
				this.instance = null;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
				this.instance = null;
			}
		}

		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				return this.instance;
			}
		}

		private string callbackMethodName = string.Empty;

		private Type type;

		private ConfigurationValidatorBase instance;
	}
}
