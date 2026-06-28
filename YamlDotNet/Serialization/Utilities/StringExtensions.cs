using System;
using System.Text.RegularExpressions;

namespace YamlDotNet.Serialization.Utilities
{
	internal static class StringExtensions
	{
		private static string ToCamelOrPascalCase(string str, Func<char, char> firstLetterTransform)
		{
			string text = Regex.Replace(str, "([_\\-])(?<char>[a-z])", (Match match) => match.Groups["char"].Value.ToUpperInvariant(), RegexOptions.IgnoreCase);
			return firstLetterTransform(text[0]).ToString() + text.Substring(1);
		}

		public static string ToCamelCase(this string str)
		{
			return StringExtensions.ToCamelOrPascalCase(str, new Func<char, char>(char.ToLowerInvariant));
		}

		public static string ToPascalCase(this string str)
		{
			return StringExtensions.ToCamelOrPascalCase(str, new Func<char, char>(char.ToUpperInvariant));
		}

		public static string FromCamelCase(this string str, string separator)
		{
			str = char.ToLower(str[0]).ToString() + str.Substring(1);
			str = Regex.Replace(str.ToCamelCase(), "(?<char>[A-Z])", (Match match) => separator + match.Groups["char"].Value.ToLowerInvariant());
			return str;
		}
	}
}
