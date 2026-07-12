using System;

namespace System.Net
{
	internal static class HttpValidationHelpers
	{
		internal static string CheckBadHeaderNameChars(string name)
		{
			if (HttpValidationHelpers.IsInvalidMethodOrHeaderString(name))
			{
				throw new ArgumentException("Specified value has invalid HTTP Header characters.", "name");
			}
			if (HttpValidationHelpers.ContainsNonAsciiChars(name))
			{
				throw new ArgumentException("Specified value has invalid HTTP Header characters.", "name");
			}
			return name;
		}

		internal static bool ContainsNonAsciiChars(string token)
		{
			for (int i = 0; i < token.Length; i++)
			{
				if (token[i] < ' ' || token[i] > '~')
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsValidToken(string token)
		{
			return token.Length > 0 && !HttpValidationHelpers.IsInvalidMethodOrHeaderString(token) && !HttpValidationHelpers.ContainsNonAsciiChars(token);
		}

		public static string CheckBadHeaderValueChars(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			value = value.Trim(HttpValidationHelpers.s_httpTrimCharacters);
			int num = 0;
			for (int i = 0; i < value.Length; i++)
			{
				char c = 'ÿ' & value[i];
				switch (num)
				{
				case 0:
					if (c == '\r')
					{
						num = 1;
					}
					else if (c == '\n')
					{
						num = 2;
					}
					else if (c == '\u007f' || (c < ' ' && c != '\t'))
					{
						throw new ArgumentException("Specified value has invalid Control characters.", "value");
					}
					break;
				case 1:
					if (c != '\n')
					{
						throw new ArgumentException("Specified value has invalid CRLF characters.", "value");
					}
					num = 2;
					break;
				case 2:
					if (c != ' ' && c != '\t')
					{
						throw new ArgumentException("Specified value has invalid Control characters.", "value");
					}
					num = 0;
					break;
				}
			}
			if (num != 0)
			{
				throw new ArgumentException("Specified value has invalid CRLF characters.", "value");
			}
			return value;
		}

		public static bool IsInvalidMethodOrHeaderString(string stringValue)
		{
			foreach (char c in stringValue)
			{
				if (c <= '/')
				{
					if (c <= ' ')
					{
						switch (c)
						{
						case '\t':
						case '\n':
						case '\r':
							return true;
						case '\v':
						case '\f':
							break;
						default:
							if (c == ' ')
							{
								return true;
							}
							break;
						}
					}
					else
					{
						if (c == '"')
						{
							return true;
						}
						switch (c)
						{
						case '\'':
						case '(':
						case ')':
						case ',':
						case '/':
							return true;
						}
					}
				}
				else if (c <= ']')
				{
					switch (c)
					{
					case ':':
					case ';':
					case '<':
					case '=':
					case '>':
					case '?':
					case '@':
						return true;
					default:
						switch (c)
						{
						case '[':
						case '\\':
						case ']':
							return true;
						}
						break;
					}
				}
				else if (c == '{' || c == '}')
				{
					return true;
				}
			}
			return false;
		}

		private static readonly char[] s_httpTrimCharacters = new char[] { '\t', '\n', '\v', '\f', '\r', ' ' };
	}
}
