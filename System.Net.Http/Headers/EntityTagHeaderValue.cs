using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class EntityTagHeaderValue : ICloneable
	{
		public EntityTagHeaderValue(string tag)
		{
			Parser.Token.CheckQuotedString(tag);
			this.Tag = tag;
		}

		public EntityTagHeaderValue(string tag, bool isWeak)
			: this(tag)
		{
			this.IsWeak = isWeak;
		}

		internal EntityTagHeaderValue()
		{
		}

		public static EntityTagHeaderValue Any
		{
			get
			{
				return EntityTagHeaderValue.any;
			}
		}

		public bool IsWeak { get; internal set; }

		public string Tag { get; internal set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			EntityTagHeaderValue entityTagHeaderValue = obj as EntityTagHeaderValue;
			return entityTagHeaderValue != null && entityTagHeaderValue.Tag == this.Tag && string.Equals(entityTagHeaderValue.Tag, this.Tag, StringComparison.Ordinal);
		}

		public override int GetHashCode()
		{
			return this.IsWeak.GetHashCode() ^ this.Tag.GetHashCode();
		}

		public static EntityTagHeaderValue Parse(string input)
		{
			EntityTagHeaderValue entityTagHeaderValue;
			if (EntityTagHeaderValue.TryParse(input, out entityTagHeaderValue))
			{
				return entityTagHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out EntityTagHeaderValue parsedValue)
		{
			Token token;
			if (EntityTagHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		private static bool TryParseElement(Lexer lexer, out EntityTagHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			bool flag = false;
			if (t == Token.Type.Token)
			{
				string stringValue = lexer.GetStringValue(t);
				if (stringValue == "*")
				{
					parsedValue = EntityTagHeaderValue.any;
					t = lexer.Scan(false);
					return true;
				}
				if (stringValue != "W" || lexer.PeekChar() != 47)
				{
					return false;
				}
				flag = true;
				lexer.EatChar();
				t = lexer.Scan(false);
			}
			if (t != Token.Type.QuotedString)
			{
				return false;
			}
			parsedValue = new EntityTagHeaderValue();
			parsedValue.Tag = lexer.GetStringValue(t);
			parsedValue.IsWeak = flag;
			t = lexer.Scan(false);
			return true;
		}

		internal static bool TryParse(string input, int minimalCount, out List<EntityTagHeaderValue> result)
		{
			return CollectionParser.TryParse<EntityTagHeaderValue>(input, minimalCount, new ElementTryParser<EntityTagHeaderValue>(EntityTagHeaderValue.TryParseElement), out result);
		}

		public override string ToString()
		{
			if (!this.IsWeak)
			{
				return this.Tag;
			}
			return "W/" + this.Tag;
		}

		private static readonly EntityTagHeaderValue any = new EntityTagHeaderValue
		{
			Tag = "*"
		};
	}
}
