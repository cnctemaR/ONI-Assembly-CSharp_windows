using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public class MediaTypeHeaderValue : ICloneable
	{
		public MediaTypeHeaderValue(string mediaType)
		{
			this.MediaType = mediaType;
		}

		protected MediaTypeHeaderValue(MediaTypeHeaderValue source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.media_type = source.media_type;
			if (source.parameters != null)
			{
				foreach (NameValueHeaderValue nameValueHeaderValue in source.parameters)
				{
					this.Parameters.Add(new NameValueHeaderValue(nameValueHeaderValue));
				}
			}
		}

		internal MediaTypeHeaderValue()
		{
		}

		public string CharSet
		{
			get
			{
				if (this.parameters == null)
				{
					return null;
				}
				NameValueHeaderValue nameValueHeaderValue = this.parameters.Find((NameValueHeaderValue l) => string.Equals(l.Name, "charset", StringComparison.OrdinalIgnoreCase));
				if (nameValueHeaderValue == null)
				{
					return null;
				}
				return nameValueHeaderValue.Value;
			}
			set
			{
				if (this.parameters == null)
				{
					this.parameters = new List<NameValueHeaderValue>();
				}
				this.parameters.SetValue("charset", value);
			}
		}

		public string MediaType
		{
			get
			{
				return this.media_type;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("MediaType");
				}
				string text;
				Token? token = MediaTypeHeaderValue.TryParseMediaType(new Lexer(value), out text);
				if (token == null || token.Value.Kind != Token.Type.End)
				{
					throw new FormatException();
				}
				this.media_type = text;
			}
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

		object ICloneable.Clone()
		{
			return new MediaTypeHeaderValue(this);
		}

		public override bool Equals(object obj)
		{
			MediaTypeHeaderValue mediaTypeHeaderValue = obj as MediaTypeHeaderValue;
			return mediaTypeHeaderValue != null && string.Equals(mediaTypeHeaderValue.media_type, this.media_type, StringComparison.OrdinalIgnoreCase) && mediaTypeHeaderValue.parameters.SequenceEqual<NameValueHeaderValue>(this.parameters);
		}

		public override int GetHashCode()
		{
			return this.media_type.ToLowerInvariant().GetHashCode() ^ HashCodeCalculator.Calculate<NameValueHeaderValue>(this.parameters);
		}

		public static MediaTypeHeaderValue Parse(string input)
		{
			MediaTypeHeaderValue mediaTypeHeaderValue;
			if (MediaTypeHeaderValue.TryParse(input, out mediaTypeHeaderValue))
			{
				return mediaTypeHeaderValue;
			}
			throw new FormatException(input);
		}

		public override string ToString()
		{
			if (this.parameters == null)
			{
				return this.media_type;
			}
			return this.media_type + this.parameters.ToString<NameValueHeaderValue>();
		}

		public static bool TryParse(string input, out MediaTypeHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			List<NameValueHeaderValue> list = null;
			string text;
			Token? token = MediaTypeHeaderValue.TryParseMediaType(lexer, out text);
			if (token == null)
			{
				return false;
			}
			Token.Type kind = token.Value.Kind;
			if (kind != Token.Type.End)
			{
				if (kind != Token.Type.SeparatorSemicolon)
				{
					return false;
				}
				Token token2;
				if (!NameValueHeaderValue.TryParseParameters(lexer, out list, out token2) || token2 != Token.Type.End)
				{
					return false;
				}
			}
			parsedValue = new MediaTypeHeaderValue
			{
				media_type = text,
				parameters = list
			};
			return true;
		}

		internal static Token? TryParseMediaType(Lexer lexer, out string media)
		{
			media = null;
			Token token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return null;
			}
			if (lexer.Scan(false) != Token.Type.SeparatorSlash)
			{
				return null;
			}
			Token token2 = lexer.Scan(false);
			if (token2 != Token.Type.Token)
			{
				return null;
			}
			media = lexer.GetStringValue(token) + "/" + lexer.GetStringValue(token2);
			return new Token?(lexer.Scan(false));
		}

		internal List<NameValueHeaderValue> parameters;

		internal string media_type;
	}
}
