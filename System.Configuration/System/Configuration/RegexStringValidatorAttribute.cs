using System;

namespace System.Configuration
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RegexStringValidatorAttribute : ConfigurationValidatorAttribute
	{
		public RegexStringValidatorAttribute(string regex)
		{
			this.regex = regex;
		}

		public string Regex
		{
			get
			{
				return this.regex;
			}
		}

		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new RegexStringValidator(this.regex);
				}
				return this.instance;
			}
		}

		private string regex;

		private ConfigurationValidatorBase instance;
	}
}
