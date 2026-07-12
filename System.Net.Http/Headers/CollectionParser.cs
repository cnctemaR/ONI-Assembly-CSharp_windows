using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	internal static class CollectionParser
	{
		public static bool TryParse<T>(string input, int minimalCount, ElementTryParser<T> parser, out List<T> result) where T : class
		{
			Lexer lexer = new Lexer(input);
			result = new List<T>();
			T t;
			Token token;
			while (parser(lexer, out t, out token))
			{
				if (t != null)
				{
					result.Add(t);
				}
				if (token != Token.Type.SeparatorComma)
				{
					if (token != Token.Type.End)
					{
						result = null;
						return false;
					}
					if (minimalCount > result.Count)
					{
						result = null;
						return false;
					}
					return true;
				}
			}
			return false;
		}

		public static bool TryParse(string input, int minimalCount, out List<string> result)
		{
			return CollectionParser.TryParse<string>(input, minimalCount, new ElementTryParser<string>(CollectionParser.TryParseStringElement), out result);
		}

		public static bool TryParseRepetition(string input, int minimalCount, out List<string> result)
		{
			return CollectionParser.TryParseRepetition<string>(input, minimalCount, new ElementTryParser<string>(CollectionParser.TryParseStringElement), out result);
		}

		private static bool TryParseStringElement(Lexer lexer, out string parsedValue, out Token t)
		{
			t = lexer.Scan(false);
			if (t == Token.Type.Token)
			{
				parsedValue = lexer.GetStringValue(t);
				if (parsedValue.Length == 0)
				{
					parsedValue = null;
				}
				t = lexer.Scan(false);
			}
			else
			{
				parsedValue = null;
			}
			return true;
		}

		public static bool TryParseRepetition<T>(string input, int minimalCount, ElementTryParser<T> parser, out List<T> result) where T : class
		{
			Lexer lexer = new Lexer(input);
			result = new List<T>();
			T t;
			Token token;
			while (parser(lexer, out t, out token))
			{
				if (t != null)
				{
					result.Add(t);
				}
				if (token == Token.Type.End)
				{
					return minimalCount <= result.Count;
				}
			}
			return false;
		}
	}
}
