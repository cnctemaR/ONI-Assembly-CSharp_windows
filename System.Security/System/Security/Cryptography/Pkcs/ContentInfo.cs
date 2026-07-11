using System;
using Mono.Security;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class ContentInfo
	{
		public ContentInfo(byte[] content)
			: this(new Oid("1.2.840.113549.1.7.1"), content)
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
			this._oid = contentType;
			this._content = content;
		}

		~ContentInfo()
		{
		}

		public byte[] Content
		{
			get
			{
				return (byte[])this._content.Clone();
			}
		}

		public Oid ContentType
		{
			get
			{
				return this._oid;
			}
		}

		[MonoTODO("MS is stricter than us about the content structure")]
		public static Oid GetContentType(byte[] encodedMessage)
		{
			if (encodedMessage == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			Oid oid;
			try
			{
				PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(encodedMessage);
				string contentType = contentInfo.ContentType;
				if (!(contentType == "1.2.840.113549.1.7.1") && !(contentType == "1.2.840.113549.1.7.2") && !(contentType == "1.2.840.113549.1.7.3") && !(contentType == "1.2.840.113549.1.7.5") && !(contentType == "1.2.840.113549.1.7.6"))
				{
					throw new CryptographicException(string.Format(Locale.GetText("Bad ASN1 - invalid OID '{0}'"), contentInfo.ContentType));
				}
				oid = new Oid(contentInfo.ContentType);
			}
			catch (Exception ex)
			{
				throw new CryptographicException(Locale.GetText("Bad ASN1 - invalid structure"), ex);
			}
			return oid;
		}

		private Oid _oid;

		private byte[] _content;
	}
}
