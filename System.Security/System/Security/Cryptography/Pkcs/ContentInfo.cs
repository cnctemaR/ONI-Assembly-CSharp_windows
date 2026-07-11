using System;
using System.Collections.Generic;
using Mono.Security;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class ContentInfo
	{
		public ContentInfo(byte[] content)
			: this(new Oid("1.2.840.113549.1.7.1"), content)
		{
		}

		public ContentInfo(Oid oid, byte[] content)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			this._oid = oid;
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
			try
			{
				PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(encodedMessage);
				string contentType = contentInfo.ContentType;
				if (contentType != null)
				{
					if (ContentInfo.<>f__switch$map0 == null)
					{
						ContentInfo.<>f__switch$map0 = new Dictionary<string, int>(5)
						{
							{ "1.2.840.113549.1.7.1", 0 },
							{ "1.2.840.113549.1.7.2", 0 },
							{ "1.2.840.113549.1.7.3", 0 },
							{ "1.2.840.113549.1.7.5", 0 },
							{ "1.2.840.113549.1.7.6", 0 }
						};
					}
					int num;
					if (ContentInfo.<>f__switch$map0.TryGetValue(contentType, out num))
					{
						if (num == 0)
						{
							return new Oid(contentInfo.ContentType);
						}
					}
				}
				string text = Locale.GetText("Bad ASN1 - invalid OID '{0}'");
				throw new CryptographicException(string.Format(text, contentInfo.ContentType));
			}
			catch (Exception ex)
			{
				throw new CryptographicException(Locale.GetText("Bad ASN1 - invalid structure"), ex);
			}
			Oid oid;
			return oid;
		}

		private Oid _oid;

		private byte[] _content;
	}
}
