using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class AuthenticationHeaderValue : ICloneable
	{
		public AuthenticationHeaderValue(string scheme)
			: this(scheme, null)
		{
		}

		public AuthenticationHeaderValue(string scheme, string parameter)
		{
			Parser.Token.Check(scheme);
			this.Scheme = scheme;
			this.Parameter = parameter;
		}

		private AuthenticationHeaderValue()
		{
		}

		public string Parameter { get; private set; }

		public string Scheme { get; private set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			AuthenticationHeaderValue authenticationHeaderValue = obj as AuthenticationHeaderValue;
			return authenticationHeaderValue != null && string.Equals(authenticationHeaderValue.Scheme, this.Scheme, StringComparison.OrdinalIgnoreCase) && authenticationHeaderValue.Parameter == this.Parameter;
		}

		public override int GetHashCode()
		{
			int num = this.Scheme.ToLowerInvariant().GetHashCode();
			if (!string.IsNullOrEmpty(this.Parameter))
			{
				num ^= this.Parameter.ToLowerInvariant().GetHashCode();
			}
			return num;
		}

		public static AuthenticationHeaderValue Parse(string input)
		{
			AuthenticationHeaderValue authenticationHeaderValue;
			if (AuthenticationHeaderValue.TryParse(input, out authenticationHeaderValue))
			{
				return authenticationHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out AuthenticationHeaderValue parsedValue)
		{
			Token token;
			if (AuthenticationHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<AuthenticationHeaderValue> result)
		{
			return CollectionParser.TryParse<AuthenticationHeaderValue>(input, minimalCount, new ElementTryParser<AuthenticationHeaderValue>(AuthenticationHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out AuthenticationHeaderValue parsedValue, out Token t)
		{
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				parsedValue = null;
				return false;
			}
			parsedValue = new AuthenticationHeaderValue();
			parsedValue.Scheme = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.Token)
			{
				parsedValue.Parameter = lexer.GetRemainingStringValue(t.StartPosition);
				t = new Token(Token.Type.End, 0, 0);
			}
			return true;
		}

		public override string ToString()
		{
			if (this.Parameter == null)
			{
				return this.Scheme;
			}
			return this.Scheme + " " + this.Parameter;
		}
	}
}
