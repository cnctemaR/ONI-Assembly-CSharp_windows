using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net.Http.Headers
{
	public class WarningHeaderValue : ICloneable
	{
		public WarningHeaderValue(int code, string agent, string text)
		{
			if (!WarningHeaderValue.IsCodeValid(code))
			{
				throw new ArgumentOutOfRangeException("code");
			}
			Parser.Uri.Check(agent);
			Parser.Token.CheckQuotedString(text);
			this.Code = code;
			this.Agent = agent;
			this.Text = text;
		}

		public WarningHeaderValue(int code, string agent, string text, DateTimeOffset date)
			: this(code, agent, text)
		{
			this.Date = new DateTimeOffset?(date);
		}

		private WarningHeaderValue()
		{
		}

		public string Agent { get; private set; }

		public int Code { get; private set; }

		public DateTimeOffset? Date { get; private set; }

		public string Text { get; private set; }

		private static bool IsCodeValid(int code)
		{
			return code >= 0 && code < 1000;
		}

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			WarningHeaderValue warningHeaderValue = obj as WarningHeaderValue;
			return warningHeaderValue != null && (this.Code == warningHeaderValue.Code && string.Equals(warningHeaderValue.Agent, this.Agent, StringComparison.OrdinalIgnoreCase) && this.Text == warningHeaderValue.Text) && this.Date == warningHeaderValue.Date;
		}

		public override int GetHashCode()
		{
			return this.Code.GetHashCode() ^ this.Agent.ToLowerInvariant().GetHashCode() ^ this.Text.GetHashCode() ^ this.Date.GetHashCode();
		}

		public static WarningHeaderValue Parse(string input)
		{
			WarningHeaderValue warningHeaderValue;
			if (WarningHeaderValue.TryParse(input, out warningHeaderValue))
			{
				return warningHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out WarningHeaderValue parsedValue)
		{
			Token token;
			if (WarningHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<WarningHeaderValue> result)
		{
			return CollectionParser.TryParse<WarningHeaderValue>(input, minimalCount, new ElementTryParser<WarningHeaderValue>(WarningHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out WarningHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			int num;
			if (!lexer.TryGetNumericValue(t, out num) || !WarningHeaderValue.IsCodeValid(num))
			{
				return false;
			}
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			Token token = t;
			if (lexer.PeekChar() == 58)
			{
				lexer.EatChar();
				token = lexer.Scan(false);
				if (token != Token.Type.Token)
				{
					return false;
				}
			}
			WarningHeaderValue warningHeaderValue = new WarningHeaderValue();
			warningHeaderValue.Code = num;
			warningHeaderValue.Agent = lexer.GetStringValue(t, token);
			t = lexer.Scan(false);
			if (t != Token.Type.QuotedString)
			{
				return false;
			}
			warningHeaderValue.Text = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.QuotedString)
			{
				DateTimeOffset dateTimeOffset;
				if (!lexer.TryGetDateValue(t, out dateTimeOffset))
				{
					return false;
				}
				warningHeaderValue.Date = new DateTimeOffset?(dateTimeOffset);
				t = lexer.Scan(false);
			}
			parsedValue = warningHeaderValue;
			return true;
		}

		public override string ToString()
		{
			string text = string.Concat(new string[]
			{
				this.Code.ToString("000"),
				" ",
				this.Agent,
				" ",
				this.Text
			});
			if (this.Date != null)
			{
				text = text + " \"" + this.Date.Value.ToString("r", CultureInfo.InvariantCulture) + "\"";
			}
			return text;
		}
	}
}
