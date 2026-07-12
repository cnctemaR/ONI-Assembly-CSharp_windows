using System;
using System.Globalization;

namespace System.Net.Http.Headers
{
	public class RangeConditionHeaderValue : ICloneable
	{
		public RangeConditionHeaderValue(DateTimeOffset date)
		{
			this.Date = new DateTimeOffset?(date);
		}

		public RangeConditionHeaderValue(EntityTagHeaderValue entityTag)
		{
			if (entityTag == null)
			{
				throw new ArgumentNullException("entityTag");
			}
			this.EntityTag = entityTag;
		}

		public RangeConditionHeaderValue(string entityTag)
			: this(new EntityTagHeaderValue(entityTag))
		{
		}

		public DateTimeOffset? Date { get; private set; }

		public EntityTagHeaderValue EntityTag { get; private set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			RangeConditionHeaderValue rangeConditionHeaderValue = obj as RangeConditionHeaderValue;
			if (rangeConditionHeaderValue == null)
			{
				return false;
			}
			if (this.EntityTag == null)
			{
				return this.Date == rangeConditionHeaderValue.Date;
			}
			return this.EntityTag.Equals(rangeConditionHeaderValue.EntityTag);
		}

		public override int GetHashCode()
		{
			if (this.EntityTag == null)
			{
				return this.Date.GetHashCode();
			}
			return this.EntityTag.GetHashCode();
		}

		public static RangeConditionHeaderValue Parse(string input)
		{
			RangeConditionHeaderValue rangeConditionHeaderValue;
			if (RangeConditionHeaderValue.TryParse(input, out rangeConditionHeaderValue))
			{
				return rangeConditionHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out RangeConditionHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			bool flag;
			if (token == Token.Type.Token)
			{
				if (lexer.GetStringValue(token) != "W")
				{
					DateTimeOffset dateTimeOffset;
					if (!Lexer.TryGetDateValue(input, out dateTimeOffset))
					{
						return false;
					}
					parsedValue = new RangeConditionHeaderValue(dateTimeOffset);
					return true;
				}
				else
				{
					if (lexer.PeekChar() != 47)
					{
						return false;
					}
					flag = true;
					lexer.EatChar();
					token = lexer.Scan(false);
				}
			}
			else
			{
				flag = false;
			}
			if (token != Token.Type.QuotedString)
			{
				return false;
			}
			if (lexer.Scan(false) != Token.Type.End)
			{
				return false;
			}
			parsedValue = new RangeConditionHeaderValue(new EntityTagHeaderValue
			{
				Tag = lexer.GetStringValue(token),
				IsWeak = flag
			});
			return true;
		}

		public override string ToString()
		{
			if (this.EntityTag != null)
			{
				return this.EntityTag.ToString();
			}
			return this.Date.Value.ToString("r", CultureInfo.InvariantCulture);
		}
	}
}
