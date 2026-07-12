using System;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9MessageDigest : Pkcs9AttributeObject
	{
		public Pkcs9MessageDigest()
			: base(Oid.FromOidValue("1.2.840.113549.1.9.4", OidGroup.ExtensionOrAttribute))
		{
		}

		internal Pkcs9MessageDigest(ReadOnlySpan<byte> signatureDigest)
		{
			using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
			{
				asnWriter.WriteOctetString(signatureDigest);
				base.RawData = asnWriter.Encode();
			}
		}

		public byte[] MessageDigest
		{
			get
			{
				byte[] array;
				if ((array = this._lazyMessageDigest) == null)
				{
					array = (this._lazyMessageDigest = Pkcs9MessageDigest.Decode(base.RawData));
				}
				return array;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			base.CopyFrom(asnEncodedData);
			this._lazyMessageDigest = null;
		}

		private static byte[] Decode(byte[] rawData)
		{
			if (rawData == null)
			{
				return null;
			}
			return PkcsPal.Instance.DecodeOctetString(rawData);
		}

		private volatile byte[] _lazyMessageDigest;
	}
}
