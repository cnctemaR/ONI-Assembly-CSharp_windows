using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Net.Http.Headers
{
	public class StringWithQualityHeaderValue : ICloneable
	{
		public StringWithQualityHeaderValue(string value)
		{
			Parser.Token.Check(value);
			this.Value = value;
		}

		public StringWithQualityHeaderValue(string value, double quality)
			: this(value)
		{
			if (quality < 0.0 || quality > 1.0)
			{
				throw new ArgumentOutOfRangeException("quality");
			}
			this.Quality = new double?(quality);
		}

		private StringWithQualityHeaderValue()
		{
		}

		public double? Quality { get; private set; }

		public string Value { get; private set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			StringWithQualityHeaderValue stringWithQualityHeaderValue = obj as StringWithQualityHeaderValue;
			if (stringWithQualityHeaderValue != null && string.Equals(stringWithQualityHeaderValue.Value, this.Value, StringComparison.OrdinalIgnoreCase))
			{
				double? quality = stringWithQualityHeaderValue.Quality;
				double? quality2 = this.Quality;
				return (quality.GetValueOrDefault() == quality2.GetValueOrDefault()) & (quality != null == (quality2 != null));
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Value.ToLowerInvariant().GetHashCode() ^ this.Quality.GetHashCode();
		}

		public static StringWithQualityHeaderValue Parse(string input)
		{
			StringWithQualityHeaderValue stringWithQualityHeaderValue;
			if (StringWithQualityHeaderValue.TryParse(input, out stringWithQualityHeaderValue))
			{
				return stringWithQualityHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out StringWithQualityHeaderValue parsedValue)
		{
			Token token;
			if (StringWithQualityHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<StringWithQualityHeaderValue> result)
		{
			return CollectionParser.TryParse<StringWithQualityHeaderValue>(input, minimalCount, new ElementTryParser<StringWithQualityHeaderValue>(StringWithQualityHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out StringWithQualityHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			StringWithQualityHeaderValue stringWithQualityHeaderValue = new StringWithQualityHeaderValue();
			stringWithQualityHeaderValue.Value = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorSemicolon)
			{
				t = lexer.Scan(false);
				if (t != Token.Type.Token)
				{
					return false;
				}
				string stringValue = lexer.GetStringValue(t);
				if (stringValue != "q" && stringValue != "Q")
				{
					return false;
				}
				t = lexer.Scan(false);
				if (t != Token.Type.SeparatorEqual)
				{
					return false;
				}
				t = lexer.Scan(false);
				double num;
				if (!lexer.TryGetDoubleValue(t, out num))
				{
					return false;
				}
				if (num > 1.0)
				{
					return false;
				}
				stringWithQualityHeaderValue.Quality = new double?(num);
				t = lexer.Scan(false);
			}
			parsedValue = stringWithQualityHeaderValue;
			return true;
		}

		public override string ToString()
		{
			if (this.Quality != null)
			{
				return this.Value + "; q=" + this.Quality.Value.ToString("0.0##", CultureInfo.InvariantCulture);
			}
			return this.Value;
		}
	}
}
