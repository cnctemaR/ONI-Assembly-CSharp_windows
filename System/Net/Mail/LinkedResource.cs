using System;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
	public class LinkedResource : AttachmentBase
	{
		public LinkedResource(string fileName)
			: base(fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(string fileName, ContentType contentType)
			: base(fileName, contentType)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(string fileName, string mediaType)
			: base(fileName, mediaType)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(Stream contentStream)
			: base(contentStream)
		{
			if (contentStream == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(Stream contentStream, ContentType contentType)
			: base(contentStream, contentType)
		{
			if (contentStream == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(Stream contentStream, string mediaType)
			: base(contentStream, mediaType)
		{
			if (contentStream == null)
			{
				throw new ArgumentNullException();
			}
		}

		public Uri ContentLink
		{
			get
			{
				return this.contentLink;
			}
			set
			{
				this.contentLink = value;
			}
		}

		public static LinkedResource CreateLinkedResourceFromString(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			return new LinkedResource(new MemoryStream(Encoding.Default.GetBytes(content)))
			{
				TransferEncoding = TransferEncoding.QuotedPrintable
			};
		}

		public static LinkedResource CreateLinkedResourceFromString(string content, ContentType contentType)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			return new LinkedResource(new MemoryStream(Encoding.Default.GetBytes(content)), contentType)
			{
				TransferEncoding = TransferEncoding.QuotedPrintable
			};
		}

		public static LinkedResource CreateLinkedResourceFromString(string content, Encoding contentEncoding, string mediaType)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			return new LinkedResource(new MemoryStream(contentEncoding.GetBytes(content)), mediaType)
			{
				TransferEncoding = TransferEncoding.QuotedPrintable
			};
		}

		private Uri contentLink;
	}
}
