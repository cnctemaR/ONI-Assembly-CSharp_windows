using System;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers
{
	public class ContentRangeHeaderValue : ICloneable
	{
		private ContentRangeHeaderValue()
		{
		}

		public ContentRangeHeaderValue(long length)
		{
			if (length < 0L)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			this.Length = new long?(length);
		}

		public ContentRangeHeaderValue(long from, long to)
		{
			if (from < 0L || from > to)
			{
				throw new ArgumentOutOfRangeException("from");
			}
			this.From = new long?(from);
			this.To = new long?(to);
		}

		public ContentRangeHeaderValue(long from, long to, long length)
			: this(from, to)
		{
			if (length < 0L)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (to > length)
			{
				throw new ArgumentOutOfRangeException("to");
			}
			this.Length = new long?(length);
		}

		public long? From { get; private set; }

		public bool HasLength
		{
			get
			{
				return this.Length != null;
			}
		}

		public bool HasRange
		{
			get
			{
				return this.From != null;
			}
		}

		public long? Length { get; private set; }

		public long? To { get; private set; }

		public string Unit
		{
			get
			{
				return this.unit;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Unit");
				}
				Parser.Token.Check(value);
				this.unit = value;
			}
		}

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			ContentRangeHeaderValue contentRangeHeaderValue = obj as ContentRangeHeaderValue;
			if (contentRangeHeaderValue == null)
			{
				return false;
			}
			long? num = contentRangeHeaderValue.Length;
			long? num2 = this.Length;
			if ((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null)))
			{
				num2 = contentRangeHeaderValue.From;
				num = this.From;
				if ((num2.GetValueOrDefault() == num.GetValueOrDefault()) & (num2 != null == (num != null)))
				{
					num = contentRangeHeaderValue.To;
					num2 = this.To;
					if ((num.GetValueOrDefault() == num2.GetValueOrDefault()) & (num != null == (num2 != null)))
					{
						return string.Equals(contentRangeHeaderValue.unit, this.unit, StringComparison.OrdinalIgnoreCase);
					}
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Unit.GetHashCode() ^ this.Length.GetHashCode() ^ this.From.GetHashCode() ^ this.To.GetHashCode() ^ this.unit.ToLowerInvariant().GetHashCode();
		}

		public static ContentRangeHeaderValue Parse(string input)
		{
			ContentRangeHeaderValue contentRangeHeaderValue;
			if (ContentRangeHeaderValue.TryParse(input, out contentRangeHeaderValue))
			{
				return contentRangeHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out ContentRangeHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return false;
			}
			ContentRangeHeaderValue contentRangeHeaderValue = new ContentRangeHeaderValue();
			contentRangeHeaderValue.unit = lexer.GetStringValue(token);
			token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return false;
			}
			if (!lexer.IsStarStringValue(token))
			{
				long num;
				if (!lexer.TryGetNumericValue(token, out num))
				{
					string stringValue = lexer.GetStringValue(token);
					if (stringValue.Length < 3)
					{
						return false;
					}
					string[] array = stringValue.Split('-', StringSplitOptions.None);
					if (array.Length != 2)
					{
						return false;
					}
					if (!long.TryParse(array[0], NumberStyles.None, CultureInfo.InvariantCulture, out num))
					{
						return false;
					}
					contentRangeHeaderValue.From = new long?(num);
					if (!long.TryParse(array[1], NumberStyles.None, CultureInfo.InvariantCulture, out num))
					{
						return false;
					}
					contentRangeHeaderValue.To = new long?(num);
				}
				else
				{
					contentRangeHeaderValue.From = new long?(num);
					token = lexer.Scan(true);
					if (token != Token.Type.SeparatorDash)
					{
						return false;
					}
					token = lexer.Scan(false);
					if (!lexer.TryGetNumericValue(token, out num))
					{
						return false;
					}
					contentRangeHeaderValue.To = new long?(num);
				}
			}
			token = lexer.Scan(false);
			if (token != Token.Type.SeparatorSlash)
			{
				return false;
			}
			token = lexer.Scan(false);
			if (!lexer.IsStarStringValue(token))
			{
				long num2;
				if (!lexer.TryGetNumericValue(token, out num2))
				{
					return false;
				}
				contentRangeHeaderValue.Length = new long?(num2);
			}
			token = lexer.Scan(false);
			if (token != Token.Type.End)
			{
				return false;
			}
			parsedValue = contentRangeHeaderValue;
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.unit);
			stringBuilder.Append(" ");
			if (this.From == null)
			{
				stringBuilder.Append("*");
			}
			else
			{
				stringBuilder.Append(this.From.Value.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append("-");
				stringBuilder.Append(this.To.Value.ToString(CultureInfo.InvariantCulture));
			}
			stringBuilder.Append("/");
			stringBuilder.Append((this.Length == null) ? "*" : this.Length.Value.ToString(CultureInfo.InvariantCulture));
			return stringBuilder.ToString();
		}

		private string unit = "bytes";
	}
}
