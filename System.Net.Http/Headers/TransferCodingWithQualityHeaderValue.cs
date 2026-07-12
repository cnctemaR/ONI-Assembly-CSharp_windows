using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public sealed class TransferCodingWithQualityHeaderValue : TransferCodingHeaderValue
	{
		public TransferCodingWithQualityHeaderValue(string value)
			: base(value)
		{
		}

		public TransferCodingWithQualityHeaderValue(string value, double quality)
			: this(value)
		{
			this.Quality = new double?(quality);
		}

		private TransferCodingWithQualityHeaderValue()
		{
		}

		public double? Quality
		{
			get
			{
				return QualityValue.GetValue(this.parameters);
			}
			set
			{
				QualityValue.SetValue(ref this.parameters, value);
			}
		}

		public new static TransferCodingWithQualityHeaderValue Parse(string input)
		{
			TransferCodingWithQualityHeaderValue transferCodingWithQualityHeaderValue;
			if (TransferCodingWithQualityHeaderValue.TryParse(input, out transferCodingWithQualityHeaderValue))
			{
				return transferCodingWithQualityHeaderValue;
			}
			throw new FormatException();
		}

		public static bool TryParse(string input, out TransferCodingWithQualityHeaderValue parsedValue)
		{
			Token token;
			if (TransferCodingWithQualityHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<TransferCodingWithQualityHeaderValue> result)
		{
			return CollectionParser.TryParse<TransferCodingWithQualityHeaderValue>(input, minimalCount, new ElementTryParser<TransferCodingWithQualityHeaderValue>(TransferCodingWithQualityHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out TransferCodingWithQualityHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			TransferCodingWithQualityHeaderValue transferCodingWithQualityHeaderValue = new TransferCodingWithQualityHeaderValue();
			transferCodingWithQualityHeaderValue.value = lexer.GetStringValue(t);
			t = lexer.Scan(false);
			if (t == Token.Type.SeparatorSemicolon && (!NameValueHeaderValue.TryParseParameters(lexer, out transferCodingWithQualityHeaderValue.parameters, out t) || t != Token.Type.End))
			{
				return false;
			}
			parsedValue = transferCodingWithQualityHeaderValue;
			return true;
		}
	}
}
