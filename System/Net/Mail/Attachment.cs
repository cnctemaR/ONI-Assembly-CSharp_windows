using System;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
	public class Attachment : AttachmentBase
	{
		public Attachment(string fileName)
			: base(fileName)
		{
			this.InitName(fileName);
		}

		public Attachment(string fileName, string mediaType)
			: base(fileName, mediaType)
		{
			this.InitName(fileName);
		}

		public Attachment(string fileName, global::System.Net.Mime.ContentType contentType)
			: base(fileName, contentType)
		{
			this.InitName(fileName);
		}

		public Attachment(Stream contentStream, global::System.Net.Mime.ContentType contentType)
			: base(contentStream, contentType)
		{
		}

		public Attachment(Stream contentStream, string name)
			: base(contentStream)
		{
			this.Name = name;
		}

		public Attachment(Stream contentStream, string name, string mediaType)
			: base(contentStream, mediaType)
		{
			this.Name = name;
		}

		public global::System.Net.Mime.ContentDisposition ContentDisposition
		{
			get
			{
				return this.contentDisposition;
			}
		}

		public string Name
		{
			get
			{
				return base.ContentType.Name;
			}
			set
			{
				base.ContentType.Name = value;
			}
		}

		public Encoding NameEncoding
		{
			get
			{
				return this.nameEncoding;
			}
			set
			{
				this.nameEncoding = value;
			}
		}

		public static Attachment CreateAttachmentFromString(string content, global::System.Net.Mime.ContentType contentType)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(content);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			return new Attachment(memoryStream, contentType)
			{
				TransferEncoding = global::System.Net.Mime.TransferEncoding.QuotedPrintable
			};
		}

		public static Attachment CreateAttachmentFromString(string content, string name)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(content);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			return new Attachment(memoryStream, new global::System.Net.Mime.ContentType("text/plain"))
			{
				TransferEncoding = global::System.Net.Mime.TransferEncoding.QuotedPrintable,
				Name = name
			};
		}

		public static Attachment CreateAttachmentFromString(string content, string name, Encoding contentEncoding, string mediaType)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream, contentEncoding);
			streamWriter.Write(content);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			return new Attachment(memoryStream, name, mediaType)
			{
				TransferEncoding = global::System.Net.Mime.ContentType.GuessTransferEncoding(contentEncoding),
				ContentType = 
				{
					CharSet = streamWriter.Encoding.BodyName
				}
			};
		}

		private void InitName(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			this.Name = Path.GetFileName(fileName);
		}

		private global::System.Net.Mime.ContentDisposition contentDisposition = new global::System.Net.Mime.ContentDisposition();

		private Encoding nameEncoding;
	}
}
