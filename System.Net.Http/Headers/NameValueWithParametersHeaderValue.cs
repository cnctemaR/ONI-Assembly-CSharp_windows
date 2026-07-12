using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class NameValueWithParametersHeaderValue : NameValueHeaderValue, ICloneable
	{
		public NameValueWithParametersHeaderValue(string name)
			: base(name)
		{
		}

		public NameValueWithParametersHeaderValue(string name, string value)
			: base(name, value)
		{
		}

		protected NameValueWithParametersHeaderValue(NameValueWithParametersHeaderValue source)
			: base(source)
		{
			if (source.parameters != null)
			{
				foreach (NameValueHeaderValue nameValueHeaderValue in source.parameters)
				{
					this.Parameters.Add(nameValueHeaderValue);
				}
			}
		}

		private NameValueWithParametersHeaderValue()
		{
		}

		public ICollection<NameValueHeaderValue> Parameters
		{
			get
			{
				List<NameValueHeaderValue> list;
				if ((list = this.parameters) == null)
				{
					list = (this.parameters = new List<NameValueHeaderValue>());
				}
				return list;
			}
		}

		object ICloneable.Clone()
		{
			return new NameValueWithParametersHeaderValue(this);
		}

		public override bool Equals(object obj)
		{
			NameValueWithParametersHeaderValue nameValueWithParametersHeaderValue = obj as NameValueWithParametersHeaderValue;
			return nameValueWithParametersHeaderValue != null && base.Equals(obj) && nameValueWithParametersHeaderValue.parameters.SequenceEqual<NameValueHeaderValue>(this.parameters);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ HashCodeCalculator.Calculate<NameValueHeaderValue>(this.parameters);
		}

		public new static NameValueWithParametersHeaderValue Parse(string input)
		{
			NameValueWithParametersHeaderValue nameValueWithParametersHeaderValue;
			if (NameValueWithParametersHeaderValue.TryParse(input, out nameValueWithParametersHeaderValue))
			{
				return nameValueWithParametersHeaderValue;
			}
			throw new FormatException(input);
		}

		public override string ToString()
		{
			if (this.parameters == null || this.parameters.Count == 0)
			{
				return base.ToString();
			}
			return base.ToString() + this.parameters.ToString<NameValueHeaderValue>();
		}

		public static bool TryParse(string input, out NameValueWithParametersHeaderValue parsedValue)
		{
			Token token;
			if (NameValueWithParametersHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<NameValueWithParametersHeaderValue> result)
		{
			return CollectionParser.TryParse<NameValueWithParametersHeaderValue>(input, minimalCount, new ElementTryParser<NameValueWithParametersHeaderValue>(NameValueWithParametersHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out NameValueWithParametersHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			parsedValue = new NameValueWithParametersHeaderValue
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
			if (t == Token.Type.SeparatorSemicolon)
			{
				List<NameValueHeaderValue> list;
				if (!NameValueHeaderValue.TryParseParameters(lexer, out list, out t))
				{
					return false;
				}
				parsedValue.parameters = list;
			}
			return true;
		}

		private List<NameValueHeaderValue> parameters;
	}
}
