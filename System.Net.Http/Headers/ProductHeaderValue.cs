using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class ProductHeaderValue : ICloneable
	{
		public ProductHeaderValue(string name)
		{
			Parser.Token.Check(name);
			this.Name = name;
		}

		public ProductHeaderValue(string name, string version)
			: this(name)
		{
			if (!string.IsNullOrEmpty(version))
			{
				Parser.Token.Check(version);
			}
			this.Version = version;
		}

		internal ProductHeaderValue()
		{
		}

		public string Name { get; internal set; }

		public string Version { get; internal set; }

		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			ProductHeaderValue productHeaderValue = obj as ProductHeaderValue;
			return productHeaderValue != null && string.Equals(productHeaderValue.Name, this.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(productHeaderValue.Version, this.Version, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			int num = this.Name.ToLowerInvariant().GetHashCode();
			if (this.Version != null)
			{
				num ^= this.Version.ToLowerInvariant().GetHashCode();
			}
			return num;
		}

		public static ProductHeaderValue Parse(string input)
		{
			ProductHeaderValue productHeaderValue;
			if (ProductHeaderValue.TryParse(input, out productHeaderValue))
			{
				return productHeaderValue;
			}
			throw new FormatException(input);
		}

		public static bool TryParse(string input, out ProductHeaderValue parsedValue)
		{
			Token token;
			if (ProductHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<ProductHeaderValue> result)
		{
			return CollectionParser.TryParse<ProductHeaderValue>(input, minimalCount, new ElementTryParser<ProductHeaderValue>(ProductHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out ProductHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			parsedValue = new ProductHeaderValue();
			parsedValue.Name = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorSlash)
			{
				t = lexer.Scan(false);
				if (t != Token.Type.Token)
				{
					return false;
				}
				parsedValue.Version = lexer.GetStringValue(t);
				t = lexer.Scan(false);
			}
			return true;
		}

		public override string ToString()
		{
			if (this.Version != null)
			{
				return this.Name + "/" + this.Version;
			}
			return this.Name;
		}
	}
}
