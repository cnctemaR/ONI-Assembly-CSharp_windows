using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Xml
{
	internal class MimeHeaders
	{
		public ContentTypeHeader ContentType
		{
			get
			{
				MimeHeader mimeHeader;
				if (this.headers.TryGetValue("content-type", out mimeHeader))
				{
					return mimeHeader as ContentTypeHeader;
				}
				return null;
			}
		}

		public ContentIDHeader ContentID
		{
			get
			{
				MimeHeader mimeHeader;
				if (this.headers.TryGetValue("content-id", out mimeHeader))
				{
					return mimeHeader as ContentIDHeader;
				}
				return null;
			}
		}

		public ContentTransferEncodingHeader ContentTransferEncoding
		{
			get
			{
				MimeHeader mimeHeader;
				if (this.headers.TryGetValue("content-transfer-encoding", out mimeHeader))
				{
					return mimeHeader as ContentTransferEncodingHeader;
				}
				return null;
			}
		}

		public MimeVersionHeader MimeVersion
		{
			get
			{
				MimeHeader mimeHeader;
				if (this.headers.TryGetValue("mime-version", out mimeHeader))
				{
					return mimeHeader as MimeVersionHeader;
				}
				return null;
			}
		}

		public void Add(string name, string value, ref int remaining)
		{
			if (name == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			if (!(name == "content-type"))
			{
				if (!(name == "content-id"))
				{
					if (!(name == "content-transfer-encoding"))
					{
						if (!(name == "mime-version"))
						{
							remaining += value.Length * 2;
						}
						else
						{
							this.Add(new MimeVersionHeader(value));
						}
					}
					else
					{
						this.Add(new ContentTransferEncodingHeader(value));
					}
				}
				else
				{
					this.Add(new ContentIDHeader(name, value));
				}
			}
			else
			{
				this.Add(new ContentTypeHeader(value));
			}
			remaining += name.Length * 2;
		}

		public void Add(MimeHeader header)
		{
			if (header == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("header");
			}
			MimeHeader mimeHeader;
			if (this.headers.TryGetValue(header.Name, out mimeHeader))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("MIME header '{0}' already exists.", new object[] { header.Name })));
			}
			this.headers.Add(header.Name, header);
		}

		public void Release(ref int remaining)
		{
			foreach (MimeHeader mimeHeader in this.headers.Values)
			{
				remaining += mimeHeader.Value.Length * 2;
			}
		}

		private Dictionary<string, MimeHeader> headers = new Dictionary<string, MimeHeader>();

		private static class Constants
		{
			public const string ContentTransferEncoding = "content-transfer-encoding";

			public const string ContentID = "content-id";

			public const string ContentType = "content-type";

			public const string MimeVersion = "mime-version";
		}
	}
}
