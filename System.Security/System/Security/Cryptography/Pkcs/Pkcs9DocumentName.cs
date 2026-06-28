using System;
using System.Text;
using Mono.Security;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9DocumentName : Pkcs9AttributeObject
	{
		public Pkcs9DocumentName()
		{
			this.Oid = new Oid("1.3.6.1.4.1.311.88.2.1", null);
		}

		public Pkcs9DocumentName(string documentName)
		{
			if (documentName == null)
			{
				throw new ArgumentNullException("documentName");
			}
			this.Oid = new Oid("1.3.6.1.4.1.311.88.2.1", null);
			this._name = documentName;
			base.RawData = this.Encode();
		}

		public Pkcs9DocumentName(byte[] encodedDocumentName)
		{
			if (encodedDocumentName == null)
			{
				throw new ArgumentNullException("encodedDocumentName");
			}
			this.Oid = new Oid("1.3.6.1.4.1.311.88.2.1", null);
			base.RawData = encodedDocumentName;
			this.Decode(encodedDocumentName);
		}

		public string DocumentName
		{
			get
			{
				return this._name;
			}
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			base.CopyFrom(asnEncodedData);
			this.Decode(base.RawData);
		}

		internal void Decode(byte[] attribute)
		{
			if (attribute[0] != 4)
			{
				return;
			}
			ASN1 asn = new ASN1(attribute);
			byte[] value = asn.Value;
			int num = value.Length;
			if (value[num - 2] == 0)
			{
				num -= 2;
			}
			this._name = Encoding.Unicode.GetString(value, 0, num);
		}

		internal byte[] Encode()
		{
			ASN1 asn = new ASN1(4, Encoding.Unicode.GetBytes(this._name + '\0'));
			return asn.GetBytes();
		}

		internal const string oid = "1.3.6.1.4.1.311.88.2.1";

		internal const string friendlyName = null;

		private string _name;
	}
}
