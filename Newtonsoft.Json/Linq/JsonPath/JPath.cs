using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class JPath
	{
		public List<PathFilter> Filters { get; private set; }

		public JPath(string expression)
		{
			ValidationUtils.ArgumentNotNull(expression, "expression");
			this._expression = expression;
			this.Filters = new List<PathFilter>();
			this.ParseMain();
		}

		private void ParseMain()
		{
			int num = this._currentIndex;
			this.EatWhitespace();
			if (this._expression.Length == this._currentIndex)
			{
				return;
			}
			if (this._expression[this._currentIndex] == '$')
			{
				if (this._expression.Length == 1)
				{
					return;
				}
				char c = this._expression[this._currentIndex + 1];
				if (c == '.' || c == '[')
				{
					this._currentIndex++;
					num = this._currentIndex;
				}
			}
			if (!this.ParsePath(this.Filters, num, false))
			{
				int currentIndex = this._currentIndex;
				this.EatWhitespace();
				if (this._currentIndex < this._expression.Length)
				{
					throw new JsonException("Unexpected character while parsing path: " + this._expression[currentIndex]);
				}
			}
		}

		private bool ParsePath(List<PathFilter> filters, int currentPartStartIndex, bool query)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			while (this._currentIndex < this._expression.Length && !flag4)
			{
				char c = this._expression[this._currentIndex];
				char c2 = c;
				if (c2 <= ')')
				{
					if (c2 != ' ')
					{
						switch (c2)
						{
						case '(':
							break;
						case ')':
							goto IL_00EF;
						default:
							goto IL_01D0;
						}
					}
					else
					{
						if (this._currentIndex < this._expression.Length)
						{
							flag4 = true;
							continue;
						}
						continue;
					}
				}
				else
				{
					if (c2 == '.')
					{
						if (this._currentIndex > currentPartStartIndex)
						{
							string text = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
							if (text == "*")
							{
								text = null;
							}
							PathFilter pathFilter = (flag ? new ScanFilter
							{
								Name = text
							} : new FieldFilter
							{
								Name = text
							});
							filters.Add(pathFilter);
							flag = false;
						}
						if (this._currentIndex + 1 < this._expression.Length && this._expression[this._currentIndex + 1] == '.')
						{
							flag = true;
							this._currentIndex++;
						}
						this._currentIndex++;
						currentPartStartIndex = this._currentIndex;
						flag2 = false;
						flag3 = true;
						continue;
					}
					switch (c2)
					{
					case '[':
						break;
					case '\\':
						goto IL_01D0;
					case ']':
						goto IL_00EF;
					default:
						goto IL_01D0;
					}
				}
				if (this._currentIndex > currentPartStartIndex)
				{
					string text2 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
					PathFilter pathFilter2 = (flag ? new ScanFilter
					{
						Name = text2
					} : new FieldFilter
					{
						Name = text2
					});
					filters.Add(pathFilter2);
					flag = false;
				}
				filters.Add(this.ParseIndexer(c));
				this._currentIndex++;
				currentPartStartIndex = this._currentIndex;
				flag2 = true;
				flag3 = false;
				continue;
				IL_00EF:
				flag4 = true;
				continue;
				IL_01D0:
				if (query && (c == '=' || c == '<' || c == '!' || c == '>' || c == '|' || c == '&'))
				{
					flag4 = true;
				}
				else
				{
					if (flag2)
					{
						throw new JsonException("Unexpected character following indexer: " + c);
					}
					this._currentIndex++;
				}
			}
			bool flag5 = this._currentIndex == this._expression.Length;
			if (this._currentIndex > currentPartStartIndex)
			{
				string text3 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex).TrimEnd(new char[0]);
				if (text3 == "*")
				{
					text3 = null;
				}
				PathFilter pathFilter3 = (flag ? new ScanFilter
				{
					Name = text3
				} : new FieldFilter
				{
					Name = text3
				});
				filters.Add(pathFilter3);
			}
			else if (flag3 && (flag5 || query))
			{
				throw new JsonException("Unexpected end while parsing path.");
			}
			return flag5;
		}

		private PathFilter ParseIndexer(char indexerOpenChar)
		{
			this._currentIndex++;
			char c = ((indexerOpenChar == '[') ? ']' : ')');
			this.EnsureLength("Path ended with open indexer.");
			this.EatWhitespace();
			if (this._expression[this._currentIndex] == '\'')
			{
				return this.ParseQuotedField(c);
			}
			if (this._expression[this._currentIndex] == '?')
			{
				return this.ParseQuery(c);
			}
			return this.ParseArrayIndexer(c);
		}

		private PathFilter ParseArrayIndexer(char indexerCloseChar)
		{
			int num = this._currentIndex;
			int? num2 = null;
			List<int> list = null;
			int num3 = 0;
			int? num4 = null;
			int? num5 = null;
			int? num6 = null;
			while (this._currentIndex < this._expression.Length)
			{
				char c = this._expression[this._currentIndex];
				if (c == ' ')
				{
					num2 = new int?(this._currentIndex);
					this.EatWhitespace();
				}
				else if (c == indexerCloseChar)
				{
					int num7 = (num2 ?? this._currentIndex) - num;
					if (list != null)
					{
						if (num7 == 0)
						{
							throw new JsonException("Array index expected.");
						}
						string text = this._expression.Substring(num, num7);
						int num8 = Convert.ToInt32(text, CultureInfo.InvariantCulture);
						list.Add(num8);
						return new ArrayMultipleIndexFilter
						{
							Indexes = list
						};
					}
					else
					{
						if (num3 > 0)
						{
							if (num7 > 0)
							{
								string text2 = this._expression.Substring(num, num7);
								int num9 = Convert.ToInt32(text2, CultureInfo.InvariantCulture);
								if (num3 == 1)
								{
									num5 = new int?(num9);
								}
								else
								{
									num6 = new int?(num9);
								}
							}
							return new ArraySliceFilter
							{
								Start = num4,
								End = num5,
								Step = num6
							};
						}
						if (num7 == 0)
						{
							throw new JsonException("Array index expected.");
						}
						string text3 = this._expression.Substring(num, num7);
						int num10 = Convert.ToInt32(text3, CultureInfo.InvariantCulture);
						return new ArrayIndexFilter
						{
							Index = new int?(num10)
						};
					}
				}
				else if (c == ',')
				{
					int num11 = (num2 ?? this._currentIndex) - num;
					if (num11 == 0)
					{
						throw new JsonException("Array index expected.");
					}
					if (list == null)
					{
						list = new List<int>();
					}
					string text4 = this._expression.Substring(num, num11);
					list.Add(Convert.ToInt32(text4, CultureInfo.InvariantCulture));
					this._currentIndex++;
					this.EatWhitespace();
					num = this._currentIndex;
					num2 = null;
				}
				else if (c == '*')
				{
					this._currentIndex++;
					this.EnsureLength("Path ended with open indexer.");
					this.EatWhitespace();
					if (this._expression[this._currentIndex] != indexerCloseChar)
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c);
					}
					return new ArrayIndexFilter();
				}
				else if (c == ':')
				{
					int num12 = (num2 ?? this._currentIndex) - num;
					if (num12 > 0)
					{
						string text5 = this._expression.Substring(num, num12);
						int num13 = Convert.ToInt32(text5, CultureInfo.InvariantCulture);
						if (num3 == 0)
						{
							num4 = new int?(num13);
						}
						else if (num3 == 1)
						{
							num5 = new int?(num13);
						}
						else
						{
							num6 = new int?(num13);
						}
					}
					num3++;
					this._currentIndex++;
					this.EatWhitespace();
					num = this._currentIndex;
					num2 = null;
				}
				else
				{
					if (!char.IsDigit(c) && c != '-')
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c);
					}
					if (num2 != null)
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c);
					}
					this._currentIndex++;
				}
			}
			throw new JsonException("Path ended with open indexer.");
		}

		private void EatWhitespace()
		{
			while (this._currentIndex < this._expression.Length)
			{
				if (this._expression[this._currentIndex] != ' ')
				{
					return;
				}
				this._currentIndex++;
			}
		}

		private PathFilter ParseQuery(char indexerCloseChar)
		{
			this._currentIndex++;
			this.EnsureLength("Path ended with open indexer.");
			if (this._expression[this._currentIndex] != '(')
			{
				throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex]);
			}
			this._currentIndex++;
			QueryExpression queryExpression = this.ParseExpression();
			this._currentIndex++;
			this.EnsureLength("Path ended with open indexer.");
			this.EatWhitespace();
			if (this._expression[this._currentIndex] != indexerCloseChar)
			{
				throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex]);
			}
			return new QueryFilter
			{
				Expression = queryExpression
			};
		}

		private QueryExpression ParseExpression()
		{
			QueryExpression queryExpression = null;
			CompositeExpression compositeExpression = null;
			while (this._currentIndex < this._expression.Length)
			{
				this.EatWhitespace();
				if (this._expression[this._currentIndex] != '@')
				{
					throw new JsonException("Unexpected character while parsing path query: " + this._expression[this._currentIndex]);
				}
				this._currentIndex++;
				List<PathFilter> list = new List<PathFilter>();
				if (this.ParsePath(list, this._currentIndex, true))
				{
					throw new JsonException("Path ended with open query.");
				}
				this.EatWhitespace();
				this.EnsureLength("Path ended with open query.");
				object obj = null;
				QueryOperator queryOperator;
				if (this._expression[this._currentIndex] == ')' || this._expression[this._currentIndex] == '|' || this._expression[this._currentIndex] == '&')
				{
					queryOperator = QueryOperator.Exists;
				}
				else
				{
					queryOperator = this.ParseOperator();
					this.EatWhitespace();
					this.EnsureLength("Path ended with open query.");
					obj = this.ParseValue();
					this.EatWhitespace();
					this.EnsureLength("Path ended with open query.");
				}
				BooleanQueryExpression booleanQueryExpression = new BooleanQueryExpression
				{
					Path = list,
					Operator = queryOperator,
					Value = ((queryOperator != QueryOperator.Exists) ? new JValue(obj) : null)
				};
				if (this._expression[this._currentIndex] == ')')
				{
					if (compositeExpression != null)
					{
						compositeExpression.Expressions.Add(booleanQueryExpression);
						return queryExpression;
					}
					return booleanQueryExpression;
				}
				else
				{
					if (this._expression[this._currentIndex] == '&' && this.Match("&&"))
					{
						if (compositeExpression == null || compositeExpression.Operator != QueryOperator.And)
						{
							CompositeExpression compositeExpression2 = new CompositeExpression
							{
								Operator = QueryOperator.And
							};
							if (compositeExpression != null)
							{
								compositeExpression.Expressions.Add(compositeExpression2);
							}
							compositeExpression = compositeExpression2;
							if (queryExpression == null)
							{
								queryExpression = compositeExpression;
							}
						}
						compositeExpression.Expressions.Add(booleanQueryExpression);
					}
					if (this._expression[this._currentIndex] == '|' && this.Match("||"))
					{
						if (compositeExpression == null || compositeExpression.Operator != QueryOperator.Or)
						{
							CompositeExpression compositeExpression3 = new CompositeExpression
							{
								Operator = QueryOperator.Or
							};
							if (compositeExpression != null)
							{
								compositeExpression.Expressions.Add(compositeExpression3);
							}
							compositeExpression = compositeExpression3;
							if (queryExpression == null)
							{
								queryExpression = compositeExpression;
							}
						}
						compositeExpression.Expressions.Add(booleanQueryExpression);
					}
				}
			}
			throw new JsonException("Path ended with open query.");
		}

		private object ParseValue()
		{
			char c = this._expression[this._currentIndex];
			if (c == '\'')
			{
				return this.ReadQuotedString();
			}
			if (char.IsDigit(c) || c == '-')
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(c);
				this._currentIndex++;
				while (this._currentIndex < this._expression.Length)
				{
					c = this._expression[this._currentIndex];
					if (c == ' ' || c == ')')
					{
						string text = stringBuilder.ToString();
						if (text.IndexOfAny(new char[] { '.', 'E', 'e' }) != -1)
						{
							double num;
							if (double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num))
							{
								return num;
							}
							throw new JsonException("Could not read query value.");
						}
						else
						{
							long num2;
							if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
							{
								return num2;
							}
							throw new JsonException("Could not read query value.");
						}
					}
					else
					{
						stringBuilder.Append(c);
						this._currentIndex++;
					}
				}
			}
			else if (c == 't')
			{
				if (this.Match("true"))
				{
					return true;
				}
			}
			else if (c == 'f')
			{
				if (this.Match("false"))
				{
					return false;
				}
			}
			else if (c == 'n' && this.Match("null"))
			{
				return null;
			}
			throw new JsonException("Could not read query value.");
		}

		private string ReadQuotedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this._currentIndex++;
			while (this._currentIndex < this._expression.Length)
			{
				char c = this._expression[this._currentIndex];
				if (c == '\\' && this._currentIndex + 1 < this._expression.Length)
				{
					this._currentIndex++;
					if (this._expression[this._currentIndex] == '\'')
					{
						stringBuilder.Append('\'');
					}
					else
					{
						if (this._expression[this._currentIndex] != '\\')
						{
							throw new JsonException("Unknown escape chracter: \\" + this._expression[this._currentIndex]);
						}
						stringBuilder.Append('\\');
					}
					this._currentIndex++;
				}
				else
				{
					if (c == '\'')
					{
						this._currentIndex++;
						return stringBuilder.ToString();
					}
					this._currentIndex++;
					stringBuilder.Append(c);
				}
			}
			throw new JsonException("Path ended with an open string.");
		}

		private bool Match(string s)
		{
			int num = this._currentIndex;
			foreach (char c in s)
			{
				if (num >= this._expression.Length || this._expression[num] != c)
				{
					return false;
				}
				num++;
			}
			this._currentIndex = num;
			return true;
		}

		private QueryOperator ParseOperator()
		{
			if (this._currentIndex + 1 >= this._expression.Length)
			{
				throw new JsonException("Path ended with open query.");
			}
			if (this.Match("=="))
			{
				return QueryOperator.Equals;
			}
			if (this.Match("!=") || this.Match("<>"))
			{
				return QueryOperator.NotEquals;
			}
			if (this.Match("<="))
			{
				return QueryOperator.LessThanOrEquals;
			}
			if (this.Match("<"))
			{
				return QueryOperator.LessThan;
			}
			if (this.Match(">="))
			{
				return QueryOperator.GreaterThanOrEquals;
			}
			if (this.Match(">"))
			{
				return QueryOperator.GreaterThan;
			}
			throw new JsonException("Could not read query operator.");
		}

		private PathFilter ParseQuotedField(char indexerCloseChar)
		{
			List<string> list = null;
			while (this._currentIndex < this._expression.Length)
			{
				string text = this.ReadQuotedString();
				this.EatWhitespace();
				this.EnsureLength("Path ended with open indexer.");
				if (this._expression[this._currentIndex] == indexerCloseChar)
				{
					if (list != null)
					{
						list.Add(text);
						return new FieldMultipleFilter
						{
							Names = list
						};
					}
					return new FieldFilter
					{
						Name = text
					};
				}
				else
				{
					if (this._expression[this._currentIndex] != ',')
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex]);
					}
					this._currentIndex++;
					this.EatWhitespace();
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(text);
				}
			}
			throw new JsonException("Path ended with open indexer.");
		}

		private void EnsureLength(string message)
		{
			if (this._currentIndex >= this._expression.Length)
			{
				throw new JsonException(message);
			}
		}

		internal IEnumerable<JToken> Evaluate(JToken t, bool errorWhenNoMatch)
		{
			return JPath.Evaluate(this.Filters, t, errorWhenNoMatch);
		}

		internal static IEnumerable<JToken> Evaluate(List<PathFilter> filters, JToken t, bool errorWhenNoMatch)
		{
			IEnumerable<JToken> enumerable = new JToken[] { t };
			foreach (PathFilter pathFilter in filters)
			{
				enumerable = pathFilter.ExecuteFilter(enumerable, errorWhenNoMatch);
			}
			return enumerable;
		}

		private readonly string _expression;

		private int _currentIndex;
	}
}
