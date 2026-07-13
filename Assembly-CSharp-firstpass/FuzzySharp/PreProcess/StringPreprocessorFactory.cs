using System;
using System.Text.RegularExpressions;

namespace FuzzySharp.PreProcess
{
	internal class StringPreprocessorFactory
	{
		private static string Default(string input)
		{
			input = Regex.Replace(input, StringPreprocessorFactory.pattern, " ");
			input = input.ToLower();
			return input.Trim();
		}

		public static Func<string, string> GetPreprocessor(PreprocessMode mode)
		{
			if (mode == PreprocessMode.Full)
			{
				return new Func<string, string>(StringPreprocessorFactory.Default);
			}
			if (mode != PreprocessMode.None)
			{
				throw new InvalidOperationException(string.Format("Invalid string preprocessor mode: {0}", mode));
			}
			return (string s) => s;
		}

		private static string pattern = "[^ a-zA-Z0-9]";
	}
}
