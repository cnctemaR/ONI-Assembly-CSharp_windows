using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class TransferCodingHeaderValue : ICloneable
	{
		public TransferCodingHeaderValue(string value)
		{
			Parser.Token.Check(value);
			this.value = value;
		}

		protected TransferCodingHeaderValue(TransferCodingHeaderValue source)
		{
			this.value = source.value;
			if (source.parameters != null)
			{
				foreach (NameValueHeaderValue nameValueHeaderValue in source.parameters)
				{
					this.Parameters.Add(new NameValueHeaderValue(nameValueHeaderValue));
				}
			}
		}

		internal TransferCodingHeaderValue()
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

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		object ICloneable.Clone()
		{
			return new TransferCodingHeaderValue(this);
		}

		public override bool Equals(object obj)
		{
			TransferCodingHeaderValue transferCodingHeaderValue = obj as TransferCodingHeaderValue;
			return transferCodingHeaderValue != null && string.Equals(this.value, transferCodingHeaderValue.value, StringComparison.OrdinalIgnoreCase) && this.parameters.SequenceEqual<NameValueHeaderValue>(transferCodingHeaderValue.parameters);
		}

		public override int GetHashCode()
		{
			int num = this.value.ToLowerInvariant().GetHashCode();
			if (this.parameters != null)
			{
				num ^= HashCodeCalculator.Calculate<NameValueHeaderValue>(this.parameters);
			}
			return num;
		}

		public static TransferCodingHeaderValue Parse(string input)
		{
			TransferCodingHeaderValue transferCodingHeaderValue;
			if (TransferCodingHeaderValue.TryParse(input, out transferCodingHeaderValue))
			{
				return transferCodingHeaderValue;
			}
			throw new FormatException(input);
		}

		public override string ToString()
		{
			return this.value + this.parameters.ToString<NameValueHeaderValue>();
		}

		public static bool TryParse(string input, out TransferCodingHeaderValue parsedValue)
		{
			Token token;
			if (TransferCodingHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<TransferCodingHeaderValue> result)
		{
			return CollectionParser.TryParse<TransferCodingHeaderValue>(input, minimalCount, new ElementTryParser<TransferCodingHeaderValue>(TransferCodingHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out TransferCodingHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			TransferCodingHeaderValue transferCodingHeaderValue = new TransferCodingHeaderValue();
			transferCodingHeaderValue.value = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorSemicolon && (!NameValueHeaderValue.TryParseParameters(lexer, out transferCodingHeaderValue.parameters, out t) || t != Token.Type.End))
			{
				return false;
			}
			parsedValue = transferCodingHeaderValue;
			return true;
		}

		internal string value;

		internal List<NameValueHeaderValue> parameters;
	}
}
