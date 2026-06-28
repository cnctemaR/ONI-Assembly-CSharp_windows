using System;
using Mono.Security;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9ContentType : Pkcs9AttributeObject
	{
		public Pkcs9ContentType()
		{
			this.Oid = new Oid("1.2.840.113549.1.9.3", "Content Type");
			this._encoded = null;
		}

		internal Pkcs9ContentType(string contentType)
		{
			this.Oid = new Oid("1.2.840.113549.1.9.3", "Content Type");
			this._contentType = new Oid(contentType);
			base.RawData = this.Encode();
			this._encoded = null;
		}

		internal Pkcs9ContentType(byte[] encodedContentType)
		{
			if (encodedContentType == null)
			{
				throw new ArgumentNullException("encodedContentType");
			}
			this.Oid = new Oid("1.2.840.113549.1.9.3", "Content Type");
			base.RawData = encodedContentType;
			this.Decode(encodedContentType);
		}

		public Oid ContentType
		{
			get
			{
				if (this._encoded != null)
				{
					this.Decode(this._encoded);
				}
				return this._contentType;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			base.CopyFrom(asnEncodedData);
			this._encoded = asnEncodedData.RawData;
		}

		internal void Decode(byte[] attribute)
		{
			if (attribute == null || attribute[0] != 6)
			{
				throw new CryptographicException(Locale.GetText("Expected an OID."));
			}
			ASN1 asn = new ASN1(attribute);
			this._contentType = new Oid(ASN1Convert.ToOid(asn));
			this._encoded = null;
		}

		internal byte[] Encode()
		{
			if (this._contentType == null)
			{
				return null;
			}
			return ASN1Convert.FromOid(this._contentType.Value).GetBytes();
		}

		internal const string oid = "1.2.840.113549.1.9.3";

		internal const string friendlyName = "Content Type";

		private Oid _contentType;

		private byte[] _encoded;
	}
}
