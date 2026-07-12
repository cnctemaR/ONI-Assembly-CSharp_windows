using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Http.Headers
{
	public class RangeHeaderValue : ICloneable
	{
		public RangeHeaderValue()
		{
			this.unit = "bytes";
		}

		public RangeHeaderValue(long? from, long? to)
			: this()
		{
			this.Ranges.Add(new RangeItemHeaderValue(from, to));
		}

		private RangeHeaderValue(RangeHeaderValue source)
			: this()
		{
			if (source.ranges != null)
			{
				foreach (RangeItemHeaderValue rangeItemHeaderValue in source.ranges)
				{
					this.Ranges.Add(rangeItemHeaderValue);
				}
			}
		}

		public ICollection<RangeItemHeaderValue> Ranges
		{
			get
			{
				List<RangeItemHeaderValue> list;
				if ((list = this.ranges) == null)
				{
					list = (this.ranges = new List<RangeItemHeaderValue>());
				}
				return list;
			}
		}

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
			return new RangeHeaderValue(this);
		}

		public override bool Equals(object obj)
		{
			RangeHeaderValue rangeHeaderValue = obj as RangeHeaderValue;
			return rangeHeaderValue != null && string.Equals(rangeHeaderValue.Unit, this.Unit, StringComparison.OrdinalIgnoreCase) && rangeHeaderValue.ranges.SequenceEqual<RangeItemHeaderValue>(this.ranges);
		}

		public override int GetHashCode()
		{
			return this.Unit.ToLowerInvariant().GetHashCode() ^ HashCodeCalculator.Calculate<RangeItemHeaderValue>(this.ranges);
		}

		public static RangeHeaderValue Parse(string input)
		{
			RangeHeaderValue rangeHeaderValue;
			if (RangeHeaderValue.TryParse(input, out rangeHeaderValue))
			{
				return rangeHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out RangeHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return false;
			}
			RangeHeaderValue rangeHeaderValue = new RangeHeaderValue();
			rangeHeaderValue.unit = lexer.GetStringValue(token);
			token = lexer.Scan(false);
			if (token != Token.Type.SeparatorEqual)
			{
				return false;
			}
			for (;;)
			{
				long? num = null;
				long? num2 = null;
				bool flag = false;
				token = lexer.Scan(true);
				Token.Type kind = token.Kind;
				if (kind != Token.Type.Token)
				{
					if (kind != Token.Type.SeparatorDash)
					{
						return false;
					}
					token = lexer.Scan(false);
					long num3;
					if (!lexer.TryGetNumericValue(token, out num3))
					{
						break;
					}
					num2 = new long?(num3);
				}
				else
				{
					string stringValue = lexer.GetStringValue(token);
					string[] array = stringValue.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
					long num3;
					if (!Parser.Long.TryParse(array[0], out num3))
					{
						return false;
					}
					int num4 = array.Length;
					if (num4 != 1)
					{
						if (num4 != 2)
						{
							return false;
						}
						num = new long?(num3);
						if (!Parser.Long.TryParse(array[1], out num3))
						{
							return false;
						}
						num2 = new long?(num3);
						long? num5 = num2;
						long? num6 = num;
						if ((num5.GetValueOrDefault() < num6.GetValueOrDefault()) & ((num5 != null) & (num6 != null)))
						{
							return false;
						}
					}
					else
					{
						token = lexer.Scan(true);
						num = new long?(num3);
						Token.Type kind2 = token.Kind;
						if (kind2 != Token.Type.End)
						{
							if (kind2 != Token.Type.SeparatorDash)
							{
								if (kind2 != Token.Type.SeparatorComma)
								{
									return false;
								}
								flag = true;
							}
							else
							{
								token = lexer.Scan(false);
								if (token != Token.Type.Token)
								{
									flag = true;
								}
								else
								{
									if (!lexer.TryGetNumericValue(token, out num3))
									{
										return false;
									}
									num2 = new long?(num3);
									long? num6 = num2;
									long? num5 = num;
									if ((num6.GetValueOrDefault() < num5.GetValueOrDefault()) & ((num6 != null) & (num5 != null)))
									{
										return false;
									}
								}
							}
						}
						else
						{
							if (stringValue.Length > 0 && stringValue[stringValue.Length - 1] != '-')
							{
								return false;
							}
							flag = true;
						}
					}
				}
				rangeHeaderValue.Ranges.Add(new RangeItemHeaderValue(num, num2));
				if (!flag)
				{
					token = lexer.Scan(false);
				}
				if (token != Token.Type.SeparatorComma)
				{
					goto Block_20;
				}
			}
			return false;
			Block_20:
			if (token != Token.Type.End)
			{
				return false;
			}
			parsedValue = rangeHeaderValue;
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.unit);
			stringBuilder.Append("=");
			for (int i = 0; i < this.Ranges.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(this.ranges[i]);
			}
			return stringBuilder.ToString();
		}

		private List<RangeItemHeaderValue> ranges;

		private string unit;
	}
}
