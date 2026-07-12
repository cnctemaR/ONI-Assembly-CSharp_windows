using System;
using System.Globalization;
using System.Net.Mail;

namespace System.Net.Http.Headers
{
	internal static class Parser
	{
		public static class Token
		{
			public static bool TryParse(string input, out string result)
			{
				if (input != null && Lexer.IsValidToken(input))
				{
					result = input;
					return true;
				}
				result = null;
				return false;
			}

			public static void Check(string s)
			{
				if (s == null)
				{
					throw new ArgumentNullException();
				}
				if (Lexer.IsValidToken(s))
				{
					return;
				}
				if (s.Length == 0)
				{
					throw new ArgumentException();
				}
				throw new FormatException(s);
			}

			public static bool TryCheck(string s)
			{
				return s != null && Lexer.IsValidToken(s);
			}

			public static void CheckQuotedString(string s)
			{
				if (s == null)
				{
					throw new ArgumentNullException();
				}
				Lexer lexer = new Lexer(s);
				if (lexer.Scan(false) == global::System.Net.Http.Headers.Token.Type.QuotedString && lexer.Scan(false) == global::System.Net.Http.Headers.Token.Type.End)
				{
					return;
				}
				if (s.Length == 0)
				{
					throw new ArgumentException();
				}
				throw new FormatException(s);
			}

			public static void CheckComment(string s)
			{
				if (s == null)
				{
					throw new ArgumentNullException();
				}
				string text;
				if (new Lexer(s).ScanCommentOptional(out text))
				{
					return;
				}
				if (s.Length == 0)
				{
					throw new ArgumentException();
				}
				throw new FormatException(s);
			}
		}

		public static class DateTime
		{
			public static bool TryParse(string input, out DateTimeOffset result)
			{
				return Lexer.TryGetDateValue(input, out result);
			}

			public new static readonly Func<object, string> ToString = (object l) => ((DateTimeOffset)l).ToString("r", CultureInfo.InvariantCulture);
		}

		public static class EmailAddress
		{
			public static bool TryParse(string input, out string result)
			{
				bool flag;
				try
				{
					new MailAddress(input);
					result = input;
					flag = true;
				}
				catch
				{
					result = null;
					flag = false;
				}
				return flag;
			}
		}

		public static class Host
		{
			public static bool TryParse(string input, out string result)
			{
				result = input;
				global::System.Uri uri;
				return global::System.Uri.TryCreate("http://u@" + input + "/", UriKind.Absolute, out uri);
			}
		}

		public static class Int
		{
			public static bool TryParse(string input, out int result)
			{
				return int.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out result);
			}
		}

		public static class Long
		{
			public static bool TryParse(string input, out long result)
			{
				return long.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out result);
			}
		}

		public static class MD5
		{
			public static bool TryParse(string input, out byte[] result)
			{
				bool flag;
				try
				{
					result = Convert.FromBase64String(input);
					flag = true;
				}
				catch
				{
					result = null;
					flag = false;
				}
				return flag;
			}

			public new static readonly Func<object, string> ToString = (object l) => Convert.ToBase64String((byte[])l);
		}

		public static class TimeSpanSeconds
		{
			public static bool TryParse(string input, out TimeSpan result)
			{
				int num;
				if (Parser.Int.TryParse(input, out num))
				{
					result = TimeSpan.FromSeconds((double)num);
					return true;
				}
				result = TimeSpan.Zero;
				return false;
			}
		}

		public static class Uri
		{
			public static bool TryParse(string input, out global::System.Uri result)
			{
				return global::System.Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out result);
			}

			public static void Check(string s)
			{
				if (s == null)
				{
					throw new ArgumentNullException();
				}
				global::System.Uri uri;
				if (Parser.Uri.TryParse(s, out uri))
				{
					return;
				}
				if (s.Length == 0)
				{
					throw new ArgumentException();
				}
				throw new FormatException(s);
			}
		}
	}
}
