using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers
{
	public class ContentDispositionHeaderValue : ICloneable
	{
		private ContentDispositionHeaderValue()
		{
		}

		public ContentDispositionHeaderValue(string dispositionType)
		{
			this.DispositionType = dispositionType;
		}

		protected ContentDispositionHeaderValue(ContentDispositionHeaderValue source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.dispositionType = source.dispositionType;
			if (source.parameters != null)
			{
				foreach (NameValueHeaderValue nameValueHeaderValue in source.parameters)
				{
					this.Parameters.Add(new NameValueHeaderValue(nameValueHeaderValue));
				}
			}
		}

		public DateTimeOffset? CreationDate
		{
			get
			{
				return this.GetDateValue("creation-date");
			}
			set
			{
				this.SetDateValue("creation-date", value);
			}
		}

		public string DispositionType
		{
			get
			{
				return this.dispositionType;
			}
			set
			{
				Parser.Token.Check(value);
				this.dispositionType = value;
			}
		}

		public string FileName
		{
			get
			{
				string text = this.FindParameter("filename");
				if (text == null)
				{
					return null;
				}
				return ContentDispositionHeaderValue.DecodeValue(text, false);
			}
			set
			{
				if (value != null)
				{
					value = ContentDispositionHeaderValue.EncodeBase64Value(value);
				}
				this.SetValue("filename", value);
			}
		}

		public string FileNameStar
		{
			get
			{
				string text = this.FindParameter("filename*");
				if (text == null)
				{
					return null;
				}
				return ContentDispositionHeaderValue.DecodeValue(text, true);
			}
			set
			{
				if (value != null)
				{
					value = ContentDispositionHeaderValue.EncodeRFC5987(value);
				}
				this.SetValue("filename*", value);
			}
		}

		public DateTimeOffset? ModificationDate
		{
			get
			{
				return this.GetDateValue("modification-date");
			}
			set
			{
				this.SetDateValue("modification-date", value);
			}
		}

		public string Name
		{
			get
			{
				string text = this.FindParameter("name");
				if (text == null)
				{
					return null;
				}
				return ContentDispositionHeaderValue.DecodeValue(text, false);
			}
			set
			{
				if (value != null)
				{
					value = ContentDispositionHeaderValue.EncodeBase64Value(value);
				}
				this.SetValue("name", value);
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

		public DateTimeOffset? ReadDate
		{
			get
			{
				return this.GetDateValue("read-date");
			}
			set
			{
				this.SetDateValue("read-date", value);
			}
		}

		public long? Size
		{
			get
			{
				long num;
				if (Parser.Long.TryParse(this.FindParameter("size"), out num))
				{
					return new long?(num);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.SetValue("size", null);
					return;
				}
				long? num = value;
				long num2 = 0L;
				if ((num.GetValueOrDefault() < num2) & (num != null))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetValue("size", value.Value.ToString(CultureInfo.InvariantCulture));
			}
		}

		object ICloneable.Clone()
		{
			return new ContentDispositionHeaderValue(this);
		}

		public override bool Equals(object obj)
		{
			ContentDispositionHeaderValue contentDispositionHeaderValue = obj as ContentDispositionHeaderValue;
			return contentDispositionHeaderValue != null && string.Equals(contentDispositionHeaderValue.dispositionType, this.dispositionType, StringComparison.OrdinalIgnoreCase) && contentDispositionHeaderValue.parameters.SequenceEqual<NameValueHeaderValue>(this.parameters);
		}

		private string FindParameter(string name)
		{
			if (this.parameters == null)
			{
				return null;
			}
			foreach (NameValueHeaderValue nameValueHeaderValue in this.parameters)
			{
				if (string.Equals(nameValueHeaderValue.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return nameValueHeaderValue.Value;
				}
			}
			return null;
		}

		private DateTimeOffset? GetDateValue(string name)
		{
			string text = this.FindParameter(name);
			if (text == null || text == null)
			{
				return null;
			}
			if (text.Length < 3)
			{
				return null;
			}
			if (text[0] == '"')
			{
				text = text.Substring(1, text.Length - 2);
			}
			DateTimeOffset dateTimeOffset;
			if (Lexer.TryGetDateValue(text, out dateTimeOffset))
			{
				return new DateTimeOffset?(dateTimeOffset);
			}
			return null;
		}

		private static string EncodeBase64Value(string value)
		{
			bool flag = value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"';
			if (flag)
			{
				value = value.Substring(1, value.Length - 2);
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] > '\u007f')
				{
					Encoding utf = Encoding.UTF8;
					return string.Format("\"=?{0}?B?{1}?=\"", utf.WebName, Convert.ToBase64String(utf.GetBytes(value)));
				}
			}
			if (flag || !Lexer.IsValidToken(value))
			{
				return "\"" + value + "\"";
			}
			return value;
		}

		private static string EncodeRFC5987(string value)
		{
			Encoding utf = Encoding.UTF8;
			StringBuilder stringBuilder = new StringBuilder(value.Length + 11);
			stringBuilder.Append(utf.WebName);
			stringBuilder.Append('\'');
			stringBuilder.Append('\'');
			foreach (char c in value)
			{
				if (c > '\u007f')
				{
					foreach (byte b in utf.GetBytes(new char[] { c }))
					{
						stringBuilder.Append('%');
						stringBuilder.Append(b.ToString("X2"));
					}
				}
				else if (!Lexer.IsValidCharacter(c) || c == '*' || c == '?' || c == '%')
				{
					stringBuilder.Append(Uri.HexEscape(c));
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private static string DecodeValue(string value, bool extendedNotation)
		{
			if (value.Length < 2)
			{
				return value;
			}
			string[] array;
			Encoding encoding;
			if (value[0] == '"')
			{
				array = value.Split('?', StringSplitOptions.None);
				if (array.Length != 5 || array[0] != "\"=" || array[4] != "=\"" || (array[2] != "B" && array[2] != "b"))
				{
					return value;
				}
				try
				{
					encoding = Encoding.GetEncoding(array[1]);
					return encoding.GetString(Convert.FromBase64String(array[3]));
				}
				catch
				{
					return value;
				}
			}
			if (!extendedNotation)
			{
				return value;
			}
			array = value.Split('\'', StringSplitOptions.None);
			if (array.Length != 3)
			{
				return null;
			}
			try
			{
				encoding = Encoding.GetEncoding(array[0]);
			}
			catch
			{
				return null;
			}
			value = array[2];
			if (value.IndexOf('%') < 0)
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = null;
			int num = 0;
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				if (c == '%')
				{
					char c2 = c;
					c = Uri.HexUnescape(value, ref i);
					if (c != c2)
					{
						if (array2 == null)
						{
							array2 = new byte[value.Length - i + 1];
						}
						array2[num++] = (byte)c;
						continue;
					}
				}
				else
				{
					i++;
				}
				if (num != 0)
				{
					stringBuilder.Append(encoding.GetChars(array2, 0, num));
					num = 0;
				}
				stringBuilder.Append(c);
			}
			if (num != 0)
			{
				stringBuilder.Append(encoding.GetChars(array2, 0, num));
			}
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			return this.dispositionType.ToLowerInvariant().GetHashCode() ^ HashCodeCalculator.Calculate<NameValueHeaderValue>(this.parameters);
		}

		public static ContentDispositionHeaderValue Parse(string input)
		{
			ContentDispositionHeaderValue contentDispositionHeaderValue;
			if (ContentDispositionHeaderValue.TryParse(input, out contentDispositionHeaderValue))
			{
				return contentDispositionHeaderValue;
			}
			throw new FormatException(input);
		}

		private void SetDateValue(string key, DateTimeOffset? value)
		{
			this.SetValue(key, (value == null) ? null : ("\"" + value.Value.ToString("r", CultureInfo.InvariantCulture) + "\""));
		}

		private void SetValue(string key, string value)
		{
			if (this.parameters == null)
			{
				this.parameters = new List<NameValueHeaderValue>();
			}
			this.parameters.SetValue(key, value);
		}

		public override string ToString()
		{
			return this.dispositionType + this.parameters.ToString<NameValueHeaderValue>();
		}

		public static bool TryParse(string input, out ContentDispositionHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			if (token.Kind != Token.Type.Token)
			{
				return false;
			}
			List<NameValueHeaderValue> list = null;
			string stringValue = lexer.GetStringValue(token);
			token = lexer.Scan(false);
			Token.Type kind = token.Kind;
			if (kind != Token.Type.End)
			{
				if (kind != Token.Type.SeparatorSemicolon)
				{
					return false;
				}
				if (!NameValueHeaderValue.TryParseParameters(lexer, out list, out token) || token != Token.Type.End)
				{
					return false;
				}
			}
			parsedValue = new ContentDispositionHeaderValue
			{
				dispositionType = stringValue,
				parameters = list
			};
			return true;
		}

		private string dispositionType;

		private List<NameValueHeaderValue> parameters;
	}
}
