using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace System.Net.Mime
{
	public class ContentType
	{
		public ContentType()
		{
			this.mediaType = "application/octet-stream";
		}

		public ContentType(string contentType)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length < 1)
			{
				throw new ArgumentException("contentType");
			}
			int num = contentType.IndexOf(';');
			if (num > 0)
			{
				string[] array = contentType.Split(new char[] { ';' });
				this.MediaType = array[0].Trim();
				for (int i = 1; i < array.Length; i++)
				{
					this.Parse(array[i]);
				}
			}
			else
			{
				this.MediaType = contentType.Trim();
			}
		}

		private void Parse(string pair)
		{
			if (pair == null || pair.Length < 1)
			{
				return;
			}
			string[] array = pair.Split(new char[] { '=' });
			if (array.Length == 2)
			{
				this.parameters.Add(array[0].Trim(), array[1].Trim());
			}
		}

		private static Encoding UTF8Unmarked
		{
			get
			{
				if (ContentType.utf8unmarked == null)
				{
					ContentType.utf8unmarked = new UTF8Encoding(false);
				}
				return ContentType.utf8unmarked;
			}
		}

		public string Boundary
		{
			get
			{
				return this.parameters["boundary"];
			}
			set
			{
				this.parameters["boundary"] = value;
			}
		}

		public string CharSet
		{
			get
			{
				return this.parameters["charset"];
			}
			set
			{
				this.parameters["charset"] = value;
			}
		}

		public string MediaType
		{
			get
			{
				return this.mediaType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.Length < 1)
				{
					throw new ArgumentException();
				}
				if (value.IndexOf('/') < 1)
				{
					throw new FormatException();
				}
				if (value.IndexOf(';') != -1)
				{
					throw new FormatException();
				}
				this.mediaType = value;
			}
		}

		public string Name
		{
			get
			{
				return this.parameters["name"];
			}
			set
			{
				this.parameters["name"] = value;
			}
		}

		public global::System.Collections.Specialized.StringDictionary Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ContentType);
		}

		private bool Equals(ContentType other)
		{
			return other != null && this.ToString() == other.ToString();
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Encoding encoding = ((this.CharSet == null) ? Encoding.UTF8 : Encoding.GetEncoding(this.CharSet));
			stringBuilder.Append(this.MediaType);
			if (this.Parameters != null && this.Parameters.Count > 0)
			{
				foreach (object obj in this.parameters)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (dictionaryEntry.Value != null && dictionaryEntry.Value.ToString().Length > 0)
					{
						stringBuilder.Append("; ");
						stringBuilder.Append(dictionaryEntry.Key);
						stringBuilder.Append("=");
						stringBuilder.Append(ContentType.WrapIfEspecialsExist(ContentType.EncodeSubjectRFC2047(dictionaryEntry.Value as string, encoding)));
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static string WrapIfEspecialsExist(string s)
		{
			s = s.Replace("\"", "\\\"");
			if (s.IndexOfAny(ContentType.especials) >= 0)
			{
				return '"' + s + '"';
			}
			return s;
		}

		internal static Encoding GuessEncoding(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] >= '\u0080')
				{
					return ContentType.UTF8Unmarked;
				}
			}
			return null;
		}

		internal static TransferEncoding GuessTransferEncoding(Encoding enc)
		{
			if (Encoding.ASCII.Equals(enc))
			{
				return TransferEncoding.SevenBit;
			}
			if (Encoding.UTF8.CodePage == enc.CodePage || Encoding.Unicode.CodePage == enc.CodePage || Encoding.UTF32.CodePage == enc.CodePage)
			{
				return TransferEncoding.Base64;
			}
			return TransferEncoding.QuotedPrintable;
		}

		internal static string To2047(byte[] bytes)
		{
			StringWriter stringWriter = new StringWriter();
			foreach (byte b in bytes)
			{
				if (b > 127 || b == 9)
				{
					stringWriter.Write("=");
					stringWriter.Write(Convert.ToString(b, 16).ToUpper());
				}
				else
				{
					stringWriter.Write(Convert.ToChar(b));
				}
			}
			return stringWriter.GetStringBuilder().ToString();
		}

		internal static string EncodeSubjectRFC2047(string s, Encoding enc)
		{
			if (s == null || Encoding.ASCII.Equals(enc))
			{
				return s;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] >= '\u0080')
				{
					string text = ContentType.To2047(enc.GetBytes(s));
					return string.Concat(new string[] { "=?", enc.HeaderName, "?Q?", text, "?=" });
				}
			}
			return s;
		}

		private static Encoding utf8unmarked;

		private string mediaType;

		private global::System.Collections.Specialized.StringDictionary parameters = new global::System.Collections.Specialized.StringDictionary();

		private static readonly char[] especials = new char[]
		{
			'(', ')', '<', '>', '@', ',', ';', ':', '<', '>',
			'/', '[', ']', '?', '.', '='
		};
	}
}
