using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class ProductInfoHeaderValue : ICloneable
	{
		public ProductInfoHeaderValue(ProductHeaderValue product)
		{
			if (product == null)
			{
				throw new ArgumentNullException();
			}
			this.Product = product;
		}

		public ProductInfoHeaderValue(string comment)
		{
			Parser.Token.CheckComment(comment);
			this.Comment = comment;
		}

		public ProductInfoHeaderValue(string productName, string productVersion)
		{
			this.Product = new ProductHeaderValue(productName, productVersion);
		}

		private ProductInfoHeaderValue()
		{
		}

		public string Comment { get; private set; }

		public ProductHeaderValue Product { get; private set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			ProductInfoHeaderValue productInfoHeaderValue = obj as ProductInfoHeaderValue;
			if (productInfoHeaderValue == null)
			{
				return false;
			}
			if (this.Product == null)
			{
				return productInfoHeaderValue.Comment == this.Comment;
			}
			return this.Product.Equals(productInfoHeaderValue.Product);
		}

		public override int GetHashCode()
		{
			if (this.Product == null)
			{
				return this.Comment.GetHashCode();
			}
			return this.Product.GetHashCode();
		}

		public static ProductInfoHeaderValue Parse(string input)
		{
			ProductInfoHeaderValue productInfoHeaderValue;
			if (ProductInfoHeaderValue.TryParse(input, out productInfoHeaderValue))
			{
				return productInfoHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out ProductInfoHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			if (!ProductInfoHeaderValue.TryParseElement(lexer, out parsedValue) || parsedValue == null)
			{
				return false;
			}
			if (lexer.Scan(false) != Token.Type.End)
			{
				parsedValue = null;
				return false;
			}
			return true;
		}

		internal static bool TryParse(string input, int minimalCount, out List<ProductInfoHeaderValue> result)
		{
			List<ProductInfoHeaderValue> list = new List<ProductInfoHeaderValue>();
			Lexer lexer = new Lexer(input);
			result = null;
			ProductInfoHeaderValue productInfoHeaderValue;
			while (ProductInfoHeaderValue.TryParseElement(lexer, out productInfoHeaderValue))
			{
				if (productInfoHeaderValue != null)
				{
					list.Add(productInfoHeaderValue);
					int num = lexer.PeekChar();
					if (num != -1)
					{
						if (num == 9 || num == 32)
						{
							lexer.EatChar();
							continue;
						}
					}
					else if (minimalCount <= list.Count)
					{
						result = list;
						return true;
					}
					return false;
				}
				if (list != null && minimalCount <= list.Count)
				{
					result = list;
					return true;
				}
				return false;
			}
			return false;
		}

		private static bool TryParseElement(Lexer lexer, out ProductInfoHeaderValue parsedValue)
		{
			parsedValue = null;
			string text;
			Token token;
			if (lexer.ScanCommentOptional(out text, out token))
			{
				if (text == null)
				{
					return false;
				}
				parsedValue = new ProductInfoHeaderValue();
				parsedValue.Comment = text;
				return true;
			}
			else
			{
				if (token == Token.Type.End)
				{
					return true;
				}
				if (token != Token.Type.Token)
				{
					return false;
				}
				ProductHeaderValue productHeaderValue = new ProductHeaderValue();
				productHeaderValue.Name = lexer.GetStringValue(token);
				int position = lexer.Position;
				token = lexer.Scan(false);
				if (token == Token.Type.SeparatorSlash)
				{
					token = lexer.Scan(false);
					if (token != Token.Type.Token)
					{
						return false;
					}
					productHeaderValue.Version = lexer.GetStringValue(token);
				}
				else
				{
					lexer.Position = position;
				}
				parsedValue = new ProductInfoHeaderValue(productHeaderValue);
				return true;
			}
		}

		public override string ToString()
		{
			if (this.Product == null)
			{
				return this.Comment;
			}
			return this.Product.ToString();
		}
	}
}
