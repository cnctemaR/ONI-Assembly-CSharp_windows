using System;
using System.Configuration;
using System.IO;

namespace System.Xml.Serialization.Configuration
{
	public class RootedPathValidator : ConfigurationValidatorBase
	{
		public override bool CanValidate(Type type)
		{
			return type == typeof(string);
		}

		public override void Validate(object value)
		{
			string text = value as string;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			text = text.Trim();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (!Path.IsPathRooted(text))
			{
				throw new ConfigurationErrorsException();
			}
			char c = text[0];
			if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
			{
				throw new ConfigurationErrorsException();
			}
		}
	}
}
