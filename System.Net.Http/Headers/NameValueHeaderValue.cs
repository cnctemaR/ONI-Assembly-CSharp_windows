using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class NameValueHeaderValue : ICloneable
	{
		public NameValueHeaderValue(string name)
			: this(name, null)
		{
		}

		public NameValueHeaderValue(string name, string value)
		{
			Parser.Token.Check(name);
			this.Name = name;
			this.Value = value;
		}

		protected internal NameValueHeaderValue(NameValueHeaderValue source)
		{
			this.Name = source.Name;
			this.value = source.value;
		}

		internal NameValueHeaderValue()
		{
		}

		public string Name { get; internal set; }

		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					Lexer lexer = new Lexer(value);
					Token token = lexer.Scan(false);
					if (lexer.Scan(false) != Token.Type.End || (token != Token.Type.Token && token != Token.Type.QuotedString))
					{
						throw new FormatException();
					}
					value = lexer.GetStringValue(token);
				}
				this.value = value;
			}
		}

		internal static NameValueHeaderValue Create(string name, string value)
		{
			return new NameValueHeaderValue
			{
				Name = name,
				value = value
			};
		}

		object ICloneable.Clone()
		{
			return new NameValueHeaderValue(this);
		}

		public override int GetHashCode()
		{
			int num = this.Name.ToLowerInvariant().GetHashCode();
			if (!string.IsNullOrEmpty(this.value))
			{
				num ^= this.value.ToLowerInvariant().GetHashCode();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			NameValueHeaderValue nameValueHeaderValue = obj as NameValueHeaderValue;
			if (nameValueHeaderValue == null || !string.Equals(nameValueHeaderValue.Name, this.Name, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.value))
			{
				return string.IsNullOrEmpty(nameValueHeaderValue.value);
			}
			return string.Equals(nameValueHeaderValue.value, this.value, StringComparison.OrdinalIgnoreCase);
		}

		public static NameValueHeaderValue Parse(string input)
		{
			NameValueHeaderValue nameValueHeaderValue;
			if (NameValueHeaderValue.TryParse(input, out nameValueHeaderValue))
			{
				return nameValueHeaderValue;
			}
			throw new FormatException(input);
		}

		internal static bool TryParsePragma(string input, int minimalCount, out List<NameValueHeaderValue> result)
		{
			return CollectionParser.TryParse<NameValueHeaderValue>(input, minimalCount, new ElementTryParser<NameValueHeaderValue>(NameValueHeaderValue.TryParseElement), out result);
		}

		internal static bool TryParseParameters(Lexer lexer, out List<NameValueHeaderValue> result, out Token t)
		{
			List<NameValueHeaderValue> list = new List<NameValueHeaderValue>();
			result = null;
			for (;;)
			{
				Token token = lexer.Scan(false);
				if (token != Token.Type.Token)
				{
					break;
				}
				string text = null;
				t = lexer.Scan(false);
				if (t == Token.Type.SeparatorEqual)
				{
					t = lexer.Scan(false);
					if (t != Token.Type.Token && t != Token.Type.QuotedString)
					{
						return false;
					}
					text = lexer.GetStringValue(t);
					t = lexer.Scan(false);
				}
				list.Add(new NameValueHeaderValue
				{
					Name = lexer.GetStringValue(token),
					value = text
				});
				if (t != Token.Type.SeparatorSemicolon)
				{
					goto Block_5;
				}
			}
			t = Token.Empty;
			return false;
			Block_5:
			result = list;
			return true;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.value))
			{
				return this.Name;
			}
			return this.Name + "=" + this.value;
		}

		public static bool TryParse(string input, out NameValueHeaderValue parsedValue)
		{
			Token token;
			if (NameValueHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		private static bool TryParseElement(Lexer lexer, out NameValueHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			parsedValue = new NameValueHeaderValue
			{
				Name = lexer.GetStringValue(t)
			};
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorEqual)
			{
				t = lexer.Scan(false);
				if (t != Token.Type.Token && t != Token.Type.QuotedString)
				{
					return false;
				}
				parsedValue.value = lexer.GetStringValue(t);
				t = lexer.Scan(false);
			}
			return true;
		}

		internal string value;
	}
}
