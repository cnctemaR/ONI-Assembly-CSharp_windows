using System;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9ContentType : Pkcs9AttributeObject
	{
		public Pkcs9ContentType()
			: base(Oid.FromOidValue("1.2.840.113549.1.9.3", OidGroup.ExtensionOrAttribute))
		{
		}

		public Oid ContentType
		{
			get
			{
				Oid oid;
				if ((oid = this._lazyContentType) == null)
				{
					oid = (this._lazyContentType = Pkcs9ContentType.Decode(base.RawData));
				}
				return oid;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			base.CopyFrom(asnEncodedData);
			this._lazyContentType = null;
		}

		private static Oid Decode(byte[] rawData)
		{
			if (rawData == null)
			{
				return null;
			}
			return new Oid(PkcsPal.Instance.DecodeOid(rawData));
		}

		private volatile Oid _lazyContentType;
	}
}
