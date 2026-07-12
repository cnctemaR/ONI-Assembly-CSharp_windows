using System;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
	public class StringContent : ByteArrayContent
	{
		public StringContent(string content)
			: this(content, null, null)
		{
		}

		public StringContent(string content, Encoding encoding)
			: this(content, encoding, null)
		{
		}

		public StringContent(string content, Encoding encoding, string mediaType)
			: base(StringContent.GetByteArray(content, encoding))
		{
			base.Headers.ContentType = new MediaTypeHeaderValue(mediaType ?? "text/plain")
			{
				CharSet = (encoding ?? Encoding.UTF8).WebName
			};
		}

		private static byte[] GetByteArray(string content, Encoding encoding)
		{
			return (encoding ?? Encoding.UTF8).GetBytes(content);
		}
	}
}
