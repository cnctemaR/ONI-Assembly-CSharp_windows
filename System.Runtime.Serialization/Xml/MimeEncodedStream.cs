using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Xml
{
	internal class MimeEncodedStream
	{
		public MimeEncodedStream(string id, string contentEncoding, string value)
		{
			this.Id = id;
			this.ContentEncoding = contentEncoding;
			this.EncodedString = value;
		}

		public string Id { get; set; }

		public string ContentEncoding { get; set; }

		public string EncodedString { get; set; }

		public string DecodedBase64String
		{
			get
			{
				return Convert.ToBase64String(Encoding.ASCII.GetBytes(this.EncodedString));
			}
		}

		public TextReader CreateTextReader()
		{
			string contentEncoding = this.ContentEncoding;
			if (contentEncoding != null)
			{
				if (MimeEncodedStream.<>f__switch$map9 == null)
				{
					MimeEncodedStream.<>f__switch$map9 = new Dictionary<string, int>(2)
					{
						{ "7bit", 0 },
						{ "8bit", 0 }
					};
				}
				int num;
				if (MimeEncodedStream.<>f__switch$map9.TryGetValue(contentEncoding, out num))
				{
					if (num == 0)
					{
						return new StringReader(this.EncodedString);
					}
				}
			}
			return new StringReader(this.DecodedBase64String);
		}
	}
}
