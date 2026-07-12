using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public sealed class MediaTypeWithQualityHeaderValue : MediaTypeHeaderValue
	{
		public MediaTypeWithQualityHeaderValue(string mediaType)
			: base(mediaType)
		{
		}

		public MediaTypeWithQualityHeaderValue(string mediaType, double quality)
			: this(mediaType)
		{
			this.Quality = new double?(quality);
		}

		private MediaTypeWithQualityHeaderValue()
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

		public new static MediaTypeWithQualityHeaderValue Parse(string input)
		{
			MediaTypeWithQualityHeaderValue mediaTypeWithQualityHeaderValue;
			if (MediaTypeWithQualityHeaderValue.TryParse(input, out mediaTypeWithQualityHeaderValue))
			{
				return mediaTypeWithQualityHeaderValue;
			}
			throw new FormatException();
		}

		public static bool TryParse(string input, out MediaTypeWithQualityHeaderValue parsedValue)
		{
			Token token;
			if (MediaTypeWithQualityHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		private static bool TryParseElement(Lexer lexer, out MediaTypeWithQualityHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			List<NameValueHeaderValue> list = null;
			string text;
			Token? token = MediaTypeHeaderValue.TryParseMediaType(lexer, out text);
			if (token == null)
			{
				t = Token.Empty;
				return false;
			}
			t = token.Value;
			if (t == Token.Type.SeparatorSemicolon && !NameValueHeaderValue.TryParseParameters(lexer, out list, out t))
			{
				return false;
			}
			parsedValue = new MediaTypeWithQualityHeaderValue();
			parsedValue.media_type = text;
			parsedValue.parameters = list;
			return true;
		}

		internal static bool TryParse(string input, int minimalCount, out List<MediaTypeWithQualityHeaderValue> result)
		{
			return CollectionParser.TryParse<MediaTypeWithQualityHeaderValue>(input, minimalCount, new ElementTryParser<MediaTypeWithQualityHeaderValue>(MediaTypeWithQualityHeaderValue.TryParseElement), out result);
		}
	}
}
