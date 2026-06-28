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

		public LinkedResource(string fileName, global::System.Net.Mime.ContentType contentType)
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

		public LinkedResource(Stream contentStream, global::System.Net.Mime.ContentType contentType)
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

		public global::System.Uri ContentLink
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
			MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(content));
			return new LinkedResource(memoryStream)
			{
				TransferEncoding = global::System.Net.Mime.TransferEncoding.QuotedPrintable
			};
		}

		public static LinkedResource CreateLinkedResourceFromString(string content, global::System.Net.Mime.ContentType contentType)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(content));
			return new LinkedResource(memoryStream, contentType)
			{
				TransferEncoding = global::System.Net.Mime.TransferEncoding.QuotedPrintable
			};
		}

		public static LinkedResource CreateLinkedResourceFromString(string content, Encoding contentEncoding, string mediaType)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			MemoryStream memoryStream = new MemoryStream(contentEncoding.GetBytes(content));
			return new LinkedResource(memoryStream, mediaType)
			{
				TransferEncoding = global::System.Net.Mime.TransferEncoding.QuotedPrintable
			};
		}

		private global::System.Uri contentLink;
	}
}
