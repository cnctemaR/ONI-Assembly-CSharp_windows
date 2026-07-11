using System;

namespace System.Xml
{
	internal class ContentTransferEncodingHeader : MimeHeader
	{
		public ContentTransferEncodingHeader(string value)
			: base("content-transfer-encoding", value.ToLowerInvariant())
		{
		}

		public ContentTransferEncodingHeader(ContentTransferEncoding contentTransferEncoding, string value)
			: base("content-transfer-encoding", null)
		{
			this.contentTransferEncoding = contentTransferEncoding;
			this.contentTransferEncodingValue = value;
		}

		public ContentTransferEncoding ContentTransferEncoding
		{
			get
			{
				this.ParseValue();
				return this.contentTransferEncoding;
			}
		}

		public string ContentTransferEncodingValue
		{
			get
			{
				this.ParseValue();
				return this.contentTransferEncodingValue;
			}
		}

		private void ParseValue()
		{
			if (this.contentTransferEncodingValue == null)
			{
				int num = 0;
				this.contentTransferEncodingValue = ((base.Value.Length == 0) ? base.Value : ((base.Value[0] == '"') ? MailBnfHelper.ReadQuotedString(base.Value, ref num, null) : MailBnfHelper.ReadToken(base.Value, ref num, null)));
				string text = this.contentTransferEncodingValue;
				if (text == "7bit")
				{
					this.contentTransferEncoding = ContentTransferEncoding.SevenBit;
					return;
				}
				if (text == "8bit")
				{
					this.contentTransferEncoding = ContentTransferEncoding.EightBit;
					return;
				}
				if (text == "binary")
				{
					this.contentTransferEncoding = ContentTransferEncoding.Binary;
					return;
				}
				this.contentTransferEncoding = ContentTransferEncoding.Other;
			}
		}

		private ContentTransferEncoding contentTransferEncoding;

		private string contentTransferEncodingValue;

		public static readonly ContentTransferEncodingHeader Binary = new ContentTransferEncodingHeader(ContentTransferEncoding.Binary, "binary");

		public static readonly ContentTransferEncodingHeader EightBit = new ContentTransferEncodingHeader(ContentTransferEncoding.EightBit, "8bit");

		public static readonly ContentTransferEncodingHeader SevenBit = new ContentTransferEncodingHeader(ContentTransferEncoding.SevenBit, "7bit");
	}
}
