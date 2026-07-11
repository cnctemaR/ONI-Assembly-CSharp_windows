using System;
using System.Text;
using Mono.Security;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs9DocumentDescription : Pkcs9AttributeObject
	{
		public Pkcs9DocumentDescription()
		{
			base.Oid = new Oid("1.3.6.1.4.1.311.88.2.2", null);
		}

		public Pkcs9DocumentDescription(string documentDescription)
		{
			if (documentDescription == null)
			{
				throw new ArgumentNullException("documentName");
			}
			base.Oid = new Oid("1.3.6.1.4.1.311.88.2.2", null);
			this._desc = documentDescription;
			base.RawData = this.Encode();
		}

		public Pkcs9DocumentDescription(byte[] encodedDocumentDescription)
		{
			if (encodedDocumentDescription == null)
			{
				throw new ArgumentNullException("encodedDocumentDescription");
			}
			base.Oid = new Oid("1.3.6.1.4.1.311.88.2.2", null);
			base.RawData = encodedDocumentDescription;
			this.Decode(encodedDocumentDescription);
		}

		public string DocumentDescription
		{
			get
			{
				return this._desc;
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
			byte[] value = new ASN1(attribute).Value;
			int num = value.Length;
			if (value[num - 2] == 0)
			{
				num -= 2;
			}
			this._desc = Encoding.Unicode.GetString(value, 0, num);
		}

		internal byte[] Encode()
		{
			return new ASN1(4, Encoding.Unicode.GetBytes(this._desc + "\0")).GetBytes();
		}

		internal const string oid = "1.3.6.1.4.1.311.88.2.2";

		internal const string friendlyName = null;

		private string _desc;
	}
}
