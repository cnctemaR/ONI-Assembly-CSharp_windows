using System;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
	public class AlternateView : AttachmentBase
	{
		public AlternateView(string fileName)
			: base(fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public AlternateView(string fileName, global::System.Net.Mime.ContentType contentType)
			: base(fileName, contentType)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public AlternateView(string fileName, string mediaType)
			: base(fileName, mediaType)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public AlternateView(Stream contentStream)
			: base(contentStream)
		{
		}

		public AlternateView(Stream contentStream, string mediaType)
			: base(contentStream, mediaType)
		{
		}

		public AlternateView(Stream contentStream, global::System.Net.Mime.ContentType contentType)
			: base(contentStream, contentType)
		{
		}

		public global::System.Uri BaseUri
		{
			get
			{
				return this.baseUri;
			}
			set
			{
				this.baseUri = value;
			}
		}

		public LinkedResourceCollection LinkedResources
		{
			get
			{
				return this.linkedResources;
			}
		}

		public static AlternateView CreateAlternateViewFromString(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
			return new AlternateView(memoryStream)
			{
				TransferEncoding = global::System.Net.Mime.TransferEncoding.QuotedPrintable
			};
		}

		public static AlternateView CreateAlternateViewFromString(string content, global::System.Net.Mime.ContentType contentType)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			Encoding encoding = ((contentType.CharSet == null) ? Encoding.UTF8 : Encoding.GetEncoding(contentType.CharSet));
			MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(content));
			return new AlternateView(memoryStream, contentType)
			{
				TransferEncoding = global::System.Net.Mime.TransferEncoding.QuotedPrintable
			};
		}

		public static AlternateView CreateAlternateViewFromString(string content, Encoding encoding, string mediaType)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(content));
			return new AlternateView(memoryStream, new global::System.Net.Mime.ContentType
			{
				MediaType = mediaType,
				CharSet = encoding.HeaderName
			})
			{
				TransferEncoding = global::System.Net.Mime.TransferEncoding.QuotedPrintable
			};
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (LinkedResource linkedResource in this.linkedResources)
				{
					linkedResource.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private global::System.Uri baseUri;

		private LinkedResourceCollection linkedResources = new LinkedResourceCollection();
	}
}
