using System;
using Mono.Security;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9MessageDigest : Pkcs9AttributeObject
	{
		public Pkcs9MessageDigest()
		{
			base.Oid = new Oid("1.2.840.113549.1.9.4", "Message Digest");
			this._encoded = null;
		}

		internal Pkcs9MessageDigest(byte[] messageDigest, bool encoded)
		{
			if (messageDigest == null)
			{
				throw new ArgumentNullException("messageDigest");
			}
			if (encoded)
			{
				base.Oid = new Oid("1.2.840.113549.1.9.4", "Message Digest");
				base.RawData = messageDigest;
				this.Decode(messageDigest);
				return;
			}
			base.Oid = new Oid("1.2.840.113549.1.9.4", "Message Digest");
			this._messageDigest = (byte[])this._messageDigest.Clone();
			base.RawData = this.Encode();
		}

		public byte[] MessageDigest
		{
			get
			{
				if (this._encoded != null)
				{
					this.Decode(this._encoded);
				}
				return this._messageDigest;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			base.CopyFrom(asnEncodedData);
			this._encoded = asnEncodedData.RawData;
		}

		internal void Decode(byte[] attribute)
		{
			if (attribute == null || attribute[0] != 4)
			{
				throw new CryptographicException(Locale.GetText("Expected an OCTETSTRING."));
			}
			ASN1 asn = new ASN1(attribute);
			this._messageDigest = asn.Value;
			this._encoded = null;
		}

		internal byte[] Encode()
		{
			return new ASN1(4, this._messageDigest).GetBytes();
		}

		internal const string oid = "1.2.840.113549.1.9.4";

		internal const string friendlyName = "Message Digest";

		private byte[] _messageDigest;

		private byte[] _encoded;
	}
}
