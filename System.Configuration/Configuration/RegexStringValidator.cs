using System;
using System.Text.RegularExpressions;

namespace System.Configuration
{
	public class RegexStringValidator : ConfigurationValidatorBase
	{
		public RegexStringValidator(string regex)
		{
			this.regex = regex;
		}

		public override bool CanValidate(Type type)
		{
			return type == typeof(string);
		}

		public override void Validate(object value)
		{
			if (!Regex.IsMatch((string)value, this.regex))
			{
				throw new ArgumentException("The string must match the regexp `{0}'", this.regex);
			}
		}

		private string regex;
	}
}
