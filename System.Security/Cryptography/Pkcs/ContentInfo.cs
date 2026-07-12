using System;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class ContentInfo
	{
		public ContentInfo(byte[] content)
			: this(Oid.FromOidValue("1.2.840.113549.1.7.1", OidGroup.ExtensionOrAttribute), content)
		{
		}

		public ContentInfo(Oid contentType, byte[] content)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			this.ContentType = contentType;
			this.Content = content;
		}

		public Oid ContentType { get; }

		public byte[] Content { get; }

		public static Oid GetContentType(byte[] encodedMessage)
		{
			if (encodedMessage == null)
			{
				throw new ArgumentNullException("encodedMessage");
			}
			return PkcsPal.Instance.GetEncodedMessageType(encodedMessage);
		}
	}
}
