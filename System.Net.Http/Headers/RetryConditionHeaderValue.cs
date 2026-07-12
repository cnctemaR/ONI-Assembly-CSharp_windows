using System;
using System.Globalization;

namespace System.Net.Http.Headers
{
	public class RetryConditionHeaderValue : ICloneable
	{
		public RetryConditionHeaderValue(DateTimeOffset date)
		{
			this.Date = new DateTimeOffset?(date);
		}

		public RetryConditionHeaderValue(TimeSpan delta)
		{
			if (delta.TotalSeconds > 4294967295.0)
			{
				throw new ArgumentOutOfRangeException("delta");
			}
			this.Delta = new TimeSpan?(delta);
		}

		public DateTimeOffset? Date { get; private set; }

		public TimeSpan? Delta { get; private set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			RetryConditionHeaderValue retryConditionHeaderValue = obj as RetryConditionHeaderValue;
			return retryConditionHeaderValue != null && retryConditionHeaderValue.Date == this.Date && retryConditionHeaderValue.Delta == this.Delta;
		}

		public override int GetHashCode()
		{
			return this.Date.GetHashCode() ^ this.Delta.GetHashCode();
		}

		public static RetryConditionHeaderValue Parse(string input)
		{
			RetryConditionHeaderValue retryConditionHeaderValue;
			if (RetryConditionHeaderValue.TryParse(input, out retryConditionHeaderValue))
			{
				return retryConditionHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out RetryConditionHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return false;
			}
			TimeSpan? timeSpan = lexer.TryGetTimeSpanValue(token);
			if (timeSpan != null)
			{
				if (lexer.Scan(false) != Token.Type.End)
				{
					return false;
				}
				parsedValue = new RetryConditionHeaderValue(timeSpan.Value);
			}
			else
			{
				DateTimeOffset dateTimeOffset;
				if (!Lexer.TryGetDateValue(input, out dateTimeOffset))
				{
					return false;
				}
				parsedValue = new RetryConditionHeaderValue(dateTimeOffset);
			}
			return true;
		}

		public override string ToString()
		{
			if (this.Delta == null)
			{
				return this.Date.Value.ToString("r", CultureInfo.InvariantCulture);
			}
			return this.Delta.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture);
		}
	}
}
